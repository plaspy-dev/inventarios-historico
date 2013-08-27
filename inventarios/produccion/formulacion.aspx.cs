using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using SDG.Componentes;
using MySql.Data.MySqlClient;
using System.IO;

public partial class produccion_formulacion : System.Web.UI.Page
{
    protected class Analisis {
        public string Nombre { get; set; }
        public string Campo { get; set; }
        public Analisis(string nombre, string campo) {
            Nombre = nombre;
            Campo = campo;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        if (!IsPostBack) {

            GProductos.DataBind();
            CUsuario usuario= (CUsuario)Usuario.GetInstance();
            TProducto.Focus();

            BAceptar.Enabled = usuario.CanInsert();
            if (!string.IsNullOrEmpty(Request.QueryString["fi"])) { 

                using(CConector conector= new CConector()) {
                    Query query= new Query("formulacion");

                    query+="producto";

                    MySqlDataReader reader = conector.Query(query.GetSelect("producto=" + Query.GetValor(Request.QueryString["fi"])));
                    if (reader != null && reader.Read()) {

                        BEliminar.Visible = usuario.CanDelete();
                        BAceptar.Enabled = usuario.CanUpdate();

                        Producto info = new Producto(reader["producto"].ToString());
                        TProducto.Text = info.NombreCompleto;
                        TProductoID.Text = info.id;
                        LUnidad.Text = info.unidad;

                        ((ing)Master).SetModificacion(info.id);

                        ViewState["id"] =
                            SMicro.SelectParameters["formulacion"].DefaultValue =
                            SAnalisis.SelectParameters["formulacion"].DefaultValue =
                        info.id;

                        query = new Query("formulacion_ingrediente");

                        query += "producto";
                        query += "cantidad";

                        List<Producto> lista = new List<Producto>();
                        reader = conector.Query(query.GetSelect("formulacion=" + Query.GetValor(Request.QueryString["fi"])));
                        if (reader != null) {
                            while (reader.Read()) {
                                info = new Producto(reader["producto"].ToString());
                                info.cantidad= Convert.ToDecimal(reader["cantidad"]);
                                info.IdGrid = lista.Count;
                                lista.Add(info);
                            }
                        }

                        UpdateListProductos(lista);
                    }
                }
            }
        }
    }

    protected void UpdateListProductos(List<Producto> lista) {
        HProductos.Value = Serializar.SerializeBinary(lista);
        GProductos.DataSource = lista;
        GProductos.DataBind();

        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");
        int count = lista.Count;
        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Producto")].Text = count.ToString() + " Producto" + (count == 1 ? "" : "s");
    }

    protected void GProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
        try
        {
            Producto info = lista
                .Where(x => x.IdGrid.ToString() == GProductos.DataKeys[e.RowIndex].Values[0].ToString())
                .ToList().First();
            TIngredienteID.Text = info.id;
            TIngrediente.Text= info.NombreCompleto;
            TCantidadIngrediente.Text = info.cantidad.ToString(CultureInfo.InvariantCulture);
            lista.Remove(info);

            for (int i = 0; i < lista.Count; i++)
                lista[i].IdGrid = i;

            UpdateListProductos(lista);
        }
        catch { }
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect("~/produccion/formulacion.aspx?fi=" + GResultado.SelectedValue.ToString());
    }

    protected void BMas_Click(object sender, EventArgs e) {

        if (!IsValid)
            return;

        List<Producto> lista = Serializar.DesSerializeBinary <List<Producto>> (HProductos.Value);
        Producto info = new Producto(TIngredienteID.Text);
        info.cantidad = Convert.ToDecimal(TCantidadIngrediente.Text, CultureInfo.InvariantCulture);
        info.IdGrid = lista.Count;
        lista.Add(info);

        UpdateListProductos(lista);

        TIngrediente.Focus();
        TCantidadIngrediente.Text = 
        TIngredienteID.Text= TIngrediente.Text = "";
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        if (GProductos.Rows.Count == 0)
        {
            LMensaje.Text = "Por favor ingrese por lo menos un producto";
            TIngrediente.Focus();
            return;
        }

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            Query query = new Query("formulacion");
            string id = (string)ViewState["id"];
            query["producto"] = TProductoID.Text;

            if (string.IsNullOrEmpty(id))
            {
                if (conector.QueryMod(query.GetInsert()) == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
                id = TProductoID.Text;
            }
            /*else
            {
                if (conector.QueryMod(query.GetUpdate("producto=" + Query.GetValor(id))) == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }*/
            if (conector.InsertModificacion(id) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            if (conector.QueryMod("DELETE FROM formulacion_parametro WHERE formulacion=" + Query.GetValor(id)) == -1)
            {

                LMensaje.Text = conector.GetLastError();
                return;
            }

            query = new Query("formulacion_parametro");
            query["formulacion"] = id;

            for (int i = 0; i < GAnalisis.Rows.Count; i++) { 
                TextBox maximo= (TextBox) GAnalisis.Rows[i].FindControl("TMaximo");
                TextBox minimo = (TextBox)GAnalisis.Rows[i].FindControl("TMinimo");
                if (maximo.Text.Length > 0 || minimo.Text.Length > 0) {
                    query["minimo"] = minimo.Text;
                    query["maximo"] = maximo.Text;
                    query["parametro"] = GAnalisis.DataKeys[i].Value.ToString();

                    if (conector.QueryMod(query.GetInsert()) == -1) {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                }
            }

            for (int i = 0; i < GMicro.Rows.Count; i++)
            {
                TextBox maximo = (TextBox)GMicro.Rows[i].FindControl("TMaximo");
                TextBox minimo = (TextBox)GMicro.Rows[i].FindControl("TMinimo");
                if (maximo.Text.Length > 0 || minimo.Text.Length > 0)
                {
                    query["minimo"] = minimo.Text;
                    query["maximo"] = maximo.Text;
                    query["parametro"] = GMicro.DataKeys[i].Value.ToString();

                    if (conector.QueryMod(query.GetInsert()) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                }
            }


            if (conector.QueryMod("DELETE FROM formulacion_ingrediente WHERE formulacion=" + Query.GetValor(id)) == -1)
            {

                LMensaje.Text = conector.GetLastError();
                return;
            }

            List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
            query = new Query("formulacion_ingrediente");
            query["formulacion"] = id;

            for (int i = 0; i < lista.Count; i++)
            {

                query["producto"] = lista[i].id;
                query["cantidad"] = lista[i].cantidad.ToString(CultureInfo.InvariantCulture);

                if (conector.QueryMod(query.GetInsert()) == -1)
                {

                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }

            conector.Commit();
        }
        ((ing)Master).SetResultado("Producto " + TProducto.Text + " Editado");
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/produccion/formulacion.aspx");
    }

    protected void BEliminar_Click(object sender, EventArgs e)
    {
        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            string id = ViewState["id"].ToString();
            if (conector.QueryMod("DELETE FROM formulacion_ingrediente WHERE formulacion=" + Query.GetValor(id)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            if (conector.QueryMod("DELETE FROM formulacion WHERE producto=" + Query.GetValor(id)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            conector.Commit();
            ((ing)Master).SetResultado("Producto " + TProducto.Text + " Eliminado");
        }
    }
    protected void BArchivos_Click(object sender, EventArgs e)
    {
        PArchivos.Visible = !PArchivos.Visible;
    }

    protected void BExportar_Click(object sender, EventArgs e)
    {
        bool paging = GResultado.AllowPaging;

        if (paging)
            GResultado.AllowPaging = false;
        GResultado.DataBind();

        Response.Clear();
        Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(Page.Title + ".csv"));
        Response.ContentEncoding = System.Text.Encoding.Unicode;

        Response.Write("id;");
        for (int i = 1; i < GResultado.Columns.Count-1; i++)
        {
            Response.Write(HttpUtility.HtmlDecode(GResultado.Columns[i].HeaderText).Trim());
            Response.Write(";");
        }
        Response.Write("idIngrediente;codigoIngrediente;nombreIngrediente;CantidadIngrediente");
        Response.Write("\n");
        using (CConector conector = new CConector())
        {
            Query query = new Query("formulacion_ingrediente");

            query += "producto";
            query += "cantidad";

            for (int i = 0; i < GResultado.Rows.Count; i++)
            {
                string producto = GResultado.DataKeys[i].Values["id"].ToString();
                producto += ";";
                for (int j = 1; j < GResultado.Columns.Count-1; j++)
                {
                    producto += HttpUtility.HtmlDecode(GResultado.Rows[i].Cells[j].Text).Trim();
                    producto += ";";
                }
                // ahora verificamos cuales son los ingredientes
                MySqlDataReader reader= conector.Query(query.GetSelect("formulacion=" + GResultado.DataKeys[i].Values["id"].ToString()));
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        Producto info = new Producto(reader["producto"].ToString());
                        Response.Write(producto);
                        Response.Write(info.id + ";");
                        Response.Write(info.codigo + ";");
                        Response.Write(info.nombre + ";");
                        Response.Write(reader["cantidad"].ToString() + ";");
                        Response.Write("\n");
                    }
                }
            }
        }

        if (paging)
        {
            GResultado.AllowPaging = true;
        }

        Response.End();
    }

    protected void BImportar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        if (!FImportar.HasFile)
            return;

        List<string> campos = new List<string>();

        int id = -1;
        int cantidad=-1;

        StreamReader reader = new StreamReader(FImportar.FileContent, System.Text.Encoding.UTF7);
        if (reader.Peek() < 0)
        {
            LMensaje.Text = "No se puede leer el archivo";
            return;
        }
        string header = reader.ReadLine();
        string[] textos = header.ToLower().Split(';');
        for (int i = 0; i < textos.Length; i++)
        {
            bool found = false;
            if (textos[i] == "id")
            {
                campos.Add(textos[i]);
                id = i;
                found = true;
            } else if(textos[i]=="idIngrediente".ToLower() || textos[i]=="codigoIngrediente".ToLower() || textos[i]=="nombreIngrediente".ToLower() || textos[i]=="CantidadIngrediente".ToLower()) {
                campos.Add(textos[i].ToLower());
                if(textos[i].ToLower()=="CantidadIngrediente".ToLower()) 
                    cantidad=i;
                found=true;
            } else {
                for (int j = 1; j < GResultado.Columns.Count-1; j++)
                {
                    if (GResultado.Columns[j].HeaderText.ToLower() == textos[i])
                    {
                        campos.Add(((BoundField)GResultado.Columns[j]).DataField);
                        found = true;
                        break;
                    }
                }
            }
            if (textos[i].Length == 0)
                break;
            if (!found)
            {
                LMensaje.Text = "No se encontro la columna '" + textos[i] + "'";
                return;
            }
        }
        if (id == -1)
        {
            // no se encontro el id
            LMensaje.Text = "No se encontro la columna id";
            return;
        }
        if(cantidad==-1) {
            LMensaje.Text = "No se encontro la columna CantidadIngrediente";
            return;
        }
        if (campos.Count == 1)
        {
            LMensaje.Text = "No se encontraron columnas a modificar";
            return;
        }

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            
            Dictionary<string, string> datos = new Dictionary<string, string>();
            Dictionary<string, bool> ProductosIngresados = new Dictionary<string, bool>();
            int nuevos = 0, actualizados = 0, ingredientes=0;

            int linea = 1;

            while (reader.Peek() >= 0)
            {

                header = reader.ReadLine();
                textos = header.Split(';');
                string currentID = "";

                linea++;

                for (int i = 0; i < textos.Length && i < campos.Count; i++)
                {
                    if (i == id)
                    {
                        currentID = textos[i];
                    }
                    else
                    {
                        datos[campos[i]]=textos[i];
                    }
                }

                Query query = new Query("formulacion");
                if (string.IsNullOrEmpty(currentID)) { // es nuevo

                    string condicion = "";
                    try
                    {
                        if (!string.IsNullOrEmpty(datos["codigo"]))
                            condicion = "codigo=" + Query.GetValor(datos["codigo"]);
                    } catch { }
                    try
                    {
                        if (!string.IsNullOrEmpty(datos["nombre"]))
                        {
                            if (!string.IsNullOrEmpty(condicion))
                                condicion += " AND ";
                            condicion += "nombre=" + Query.GetValor(datos["nombre"]);
                        }
                    } catch { }
                    
                    if (string.IsNullOrEmpty(condicion)) {
                        LMensaje.Text = "No se encontro Id,Código o Producto en la línea " + linea;
                        return;
                    }
                    
                    currentID= conector.QueryString(string.Format("SELECT id FROM producto WHERE {0} LIMIT 1", condicion));
                    if (string.IsNullOrEmpty(currentID)) { 
                        LMensaje.Text="No se encontro el nombre o código del producto en la línea " + linea;
                        return;
                    }
                    nuevos++;
                } else {}
                query["producto"] = currentID;
                if (conector.QueryMod(query.GetInsert().Replace("INSERT INTO ", "INSERT IGNORE INTO ")) == -1)
                    return;

                query= new Query("formulacion_ingrediente");

                try
                {
                    bool bDatos = ProductosIngresados[currentID];
                } catch {
                    actualizados++;
                    ProductosIngresados[currentID] = true;
                    if (conector.QueryMod(query.GetDelete("formulacion=" + currentID)) == -1)
                        return;
                }

                query["formulacion"]= currentID;
                try
                {
                    query["cantidad"] = Convert.ToDouble(datos["cantidadingrediente"]).ToString(CultureInfo.InvariantCulture);
                } catch { }


                string szCondicion = "";
                try
                {
                    if (!string.IsNullOrEmpty(datos["codigoingrediente"]))
                        szCondicion = "codigo=" + Query.GetValor(datos["codigoingrediente"]);
                } catch { }
                try
                {
                    if (!string.IsNullOrEmpty(datos["nombreingrediente"]))
                    {
                        if (!string.IsNullOrEmpty(szCondicion))
                            szCondicion += " AND ";
                        szCondicion += "nombre=" + Query.GetValor(datos["nombreingrediente"]);
                    }
                } catch { }
                
                if (string.IsNullOrEmpty(szCondicion)) {
                    LMensaje.Text = "No se encontro Id,codigoIngrediente o nombreIngrediente en la línea " + linea;
                    return;
                }
                
                string IngredienteID= conector.QueryString(string.Format("SELECT id FROM producto WHERE {0} LIMIT 1", szCondicion));
                if (string.IsNullOrEmpty(IngredienteID))
                {
                    LMensaje.Text="No se encontro el nombre o código del ingrediente en la línea " + linea;
                    return;
                }

                query["producto"] = IngredienteID;
                if (conector.QueryMod(query.GetInsert()) == -1)
                {
                    LMensaje.Text = "Ingrediente repetido en la línea " + linea;
                    return;
                }

                ingredientes++;
            }

            conector.Commit();
            GResultado.DataBind();

            LMensaje.Text = string.Format("Importacion realizada con exito.<br/>{0} productos, {1} Nuevos y {2} actualizados. {3} Ingredientes", nuevos + actualizados, nuevos, actualizados, ingredientes);
            PArchivos.Visible = false;
        }
    }
}
