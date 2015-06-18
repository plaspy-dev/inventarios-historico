using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class administrador_terceros : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        if (!IsPostBack)
        {
            TIdentificacion.Focus();
            CUsuario usuario = (CUsuario)Usuario.GetUsuario();

            BAceptar.Enabled = usuario.CanInsert();
            GResultado.Columns[1].Visible = usuario.CanDelete();
            GResultado.Columns[0].Visible = usuario.CanUpdate();
            CConfiguracion configuracion= CConfiguracion.GetConfiguracion();

            RDireccion.Visible = RTelefono.Visible = configuracion.bDatosComplementariosTerceroSalida || configuracion.bDatosComplementariosTerceroEntrada;

            if (!string.IsNullOrEmpty(Request.QueryString["ti"]))
            {

                using (CConector conector = new CConector())
                {
                    Query query = new Query("tercero");

                    query += "id";
                    query += "nombre";
                    query += "descripcion";
                    query += "tipo";
                    query += "identificacion";
                    query += "telefono";
                    query += "direccion";
                    query += "email";

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["ti"])));
                    if (reader != null && reader.Read())
                    {

                        HId.Value = reader["id"].ToString();

                        TNombre.Text = reader["nombre"].ToString();
                        TDescripcion.Text = reader["descripcion"].ToString();
                        DTipo.SelectedValue = reader["tipo"].ToString();
                        TIdentificacion.Text = reader["identificacion"].ToString();
                        TTelefono.Text = reader["telefono"].ToString();
                        TDireccion.Text = reader["direccion"].ToString();
                        TEMail.Text = reader["email"].ToString();

                        ((ing)Master).SetModificacion(HId.Value);
                        BAceptar.Enabled = usuario.CanUpdate();
                    }
                }
            }
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("tercero");
        using (CConector conector = new CConector())
        {
            string szQuery;
            string mensaje;

            conector.StartTransaction();

            query["nombre"] = TNombre.Text;
            query["descripcion"] = TDescripcion.Text;
            query["tipo"] = DTipo.SelectedValue;
            query["identificacion"] = TIdentificacion.Text;
            query["telefono"] = TTelefono.Text;
            query["direccion"] = TDireccion.Text;
            query["email"] = TEMail.Text;

            if (HId.Value.Length == 0)
            {
                szQuery = query.GetInsert();
                mensaje = TNombre.Text + " ingresado";
            }
            else
            {
                szQuery = query.GetUpdate("id='" + HId.Value + "'");
                mensaje = TNombre.Text + " editado";
            }
            int res = conector.QueryMod(szQuery, true);
            if (res == -1)
                return;
            if (HId.Value.Length == 0)
                HId.Value = res.ToString();

            if (conector.InsertModificacion(HId.Value) == -1)
                return;

            conector.Commit();

            Tercero.RemoveTercero(HId.Value);

            ((ing)Master).SetResultado(mensaje);
        }
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath + "?ti=" + GResultado.SelectedDataKey.Value.ToString());
    }

    protected void BExportar_Click(object sender, EventArgs e) {
        bool paging = GResultado.AllowPaging;

        if (paging)
        {
            GResultado.AllowPaging = false;
            GResultado.DataBind();
        }

        Response.Clear();
        Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(Page.Title + ".csv"));
        Response.ContentEncoding = System.Text.Encoding.Unicode;

        Response.Write("id;");
        for (int i = 2; i < GResultado.Columns.Count; i++)
        {
            Response.Write(HttpUtility.HtmlDecode(GResultado.Columns[i].HeaderText).Trim());
            Response.Write(";");
        }
        Response.Write("\n");

        for (int i = 0; i < GResultado.Rows.Count; i++)
        {
            Response.Write(GResultado.DataKeys[i].Values["id"].ToString());
            Response.Write(";");
            for (int j = 2; j < GResultado.Columns.Count; j++)
            {
                Response.Write(HttpUtility.HtmlDecode(GResultado.Rows[i].Cells[j].Text).Trim());
                Response.Write(";");
            }
            Response.Write("\n");
        }

        if (paging)
        {
            GResultado.AllowPaging = true;
        }

        Response.End();
    }

    public static Dictionary<string, string> GetDiccionario(DropDownList lista)
    {

        Dictionary<string, string> diccionario = new Dictionary<string, string>();

        for (int i = 0; i < lista.Items.Count; i++)
        {
            if (lista.Items[i].Value != "-1")
                diccionario[lista.Items[i].Text.ToLower()] = lista.Items[i].Value;
        }

        return diccionario;
    }

    protected void BImportar_Click(object sender, EventArgs e) {
        
        if (!IsValid)
            return;
        if (!FImportar.HasFile)
            return;

        List<string> campos = new List<string>();

        int id = -1;

        Query query = new Query("tercero");
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
            }
            else
            {
                for (int j = 2; j < GResultado.Columns.Count; j++)
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
        if (campos.Count == 1)
        {
            LMensaje.Text = "No se encontraron columnas a modificar";
            return;
        }

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            int nuevos = 0, actualizados = 0;
            string valor;
            int linea = 1;

            Dictionary<string, string> tipos = GetDiccionario(DTipo);

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
                        valor = textos[i];
                        switch (campos[i])
                        {
                            case "nombre":
                                if (valor.Length == 0)
                                {
                                    LMensaje.Text = "No se puede ingresar el nombre esta en blanco en la línea " + linea;
                                    return;
                                }
                                break;
                            case "tipo":
                                try
                                {
                                    valor = tipos[textos[i].ToLower()];
                                }
                                catch
                                {
                                    LMensaje.Text = "No existe el tipo llamado '" + textos[i] + "' en línea " + linea;
                                    return;
                                }
                                break;
                        }
                        query[campos[i]] = valor;
                    }
                }
                if (string.IsNullOrEmpty(currentID))
                {
                    nuevos++;
                    if (conector.QueryMod(query.GetInsert()) == -1)
                        return;
                }
                else
                {
                    actualizados++;
                    if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(currentID))) == -1)
                        return;
                    Producto.RemoveProducto(currentID);
                }
            }
            reader.Close();
            conector.Commit();
            GResultado.DataBind();

            LMensaje.Text = string.Format("Importacion realizada con exito.<br/>{0} productos, {1} Nuevos y {2} actualizados", nuevos + actualizados, nuevos, actualizados);
        }
    }

    protected void BArchivos_Click(object sender, EventArgs e)
    {
        PArchivos.Visible = !PArchivos.Visible;
    }
}
