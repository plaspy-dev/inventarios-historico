using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class produccion_orden : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        ((ing)Master)._ImprimirClicked += new EventHandler(OnImprimir);
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            if (Session["orden_clean"] != null)
            {
                ViewState["clean"] = true;
                Session["orden_clean"] = null;
            } else ViewState["clean"] = false;

            if (Session["lorden"] != null)
            {
                ViewState["urlprint"] = Request.Url.AbsolutePath + "?oi=" + Session["lorden"].ToString() + "&p=1";
                LImprimir.Text = "Orden de producción " + Session["lentrada"].ToString() + " Editada.<br/>¿Desea imprimirla?";
                PImprimir.Visible = true;
                Session["lorden"] = null;
            }
            BAceptar.Enabled = usuario.CanInsert();

            if (!string.IsNullOrEmpty(Request.QueryString["oi"]))
            {
                using (CConector conector = new CConector())
                {
                    Query query = new Query("orden_produccion");

                    query += "id";
                    query += "planeacion";
                    query += "produccion";
                    query += "producto";
                    query += "tercero";
                    query += "lote";
                    query += "cantidad";
                    query += "observaciones";
                    query += "formulacion";

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["oi"])));
                    if (reader != null && reader.Read())
                    {
                        Session["parametros"] = Request.Url.Query;
                        ViewState["id"] = 
                            SAnalisis.SelectParameters["orden"].DefaultValue = 
                            TNumero.Text = reader["id"].ToString();

                        tGAnalisis.Visible =
                        tAnalisis.Visible = true;

                        HAnalisis.NavigateUrl += "?oi=" + TNumero.Text;

                        BAceptar.Enabled = usuario.CanUpdate();
                        ((ing)Master).SetModificacion(TNumero.Text);
                        ((ing)Master).SetPrintFile("orden.html");

                        TFecha.Text = Conector.GetFromMySQLDate(reader["planeacion"]);
                        TFechaProduccion.Text = Conector.GetFromMySQLDate(reader["produccion"]);
                        
                        TClienteID.Text = reader["tercero"].ToString();
                        Tercero tercero= new Tercero(TClienteID.Text);
                        TCliente.Text = tercero.Documento + " " + tercero.Nombre;

                        Producto info = new Producto(reader["producto"].ToString());
                        TProductoID.Text = info.id;
                        TRecetaID.Text = reader["formulacion"].ToString();
                        TProducto.Text = info.NombreCompleto;
                        TLote.Text = reader["lote"].ToString();
                        TCantidad.Text = Convert.ToDecimal(reader["cantidad"]).ToString(CultureInfo.InvariantCulture);
                        TObservaciones.Text = reader["observaciones"].ToString();

                        List<Producto> lista = new List<Producto>();

                        query = new Query("orden_produccion_ingredientes");

                        query += "id";
                        query += "producto";
                        query += "cantidad";

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(Request.QueryString["oi"])));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {

                                info = new Producto(reader["producto"].ToString());
                                info.IdGrid = lista.Count;
                                info.IdTabla = reader["id"].ToString();
                                info.cantidad = Convert.ToDecimal(reader["cantidad"]);
                                lista.Add(info);
                            }
                        }

                        HProductos.Value = Serializar.SerializeBinary(lista);
                        GProductos.DataSource = lista;
                        GProductos.DataBind();

                        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");
                        return;
                    }
                }
            }
            using (CConector conector = new CConector())
                TNumero.Text = conector.QueryString("SELECT IF(MAX(id),MAX(id)+1,1) FROM orden_produccion");
            TFecha.Text = DateTime.Now.ToShortDateString();
            TNumero.Focus();
        }
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
            TIngrediente.Text = info.NombreCompleto;
            TCantidadIngrediente.Text = info.cantidad.ToString(CultureInfo.InvariantCulture);
            info.IdGrid = -1;
            // lista.Remove(info);

            for (int i = 0; i < lista.Count; i++)
                if(lista[i].IdGrid!=-1)
                    lista[i].IdGrid = i;

            HProductos.Value = Serializar.SerializeBinary(lista);
            GProductos.DataSource = lista.Where(x=> x.IdGrid!=-1).ToList();
            GProductos.DataBind();
            GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");
        }
        catch { }
        e.Cancel = true;
    }

    protected void BMas_Click(object sender, EventArgs e) {

        if (!IsValid)
            return;

        List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
        Producto info = new Producto(TIngredienteID.Text);
        info.cantidad = Convert.ToDecimal(TCantidadIngrediente.Text, CultureInfo.InvariantCulture);
        info.IdGrid = lista.Count;
        lista.Add(info);

        HProductos.Value = Serializar.SerializeBinary(lista);
        GProductos.DataSource = lista.Where(x => x.IdGrid != -1).ToList();
        GProductos.DataBind();
        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");

        TIngrediente.Focus();
        TCantidadIngrediente.Text =
        TIngredienteID.Text = TIngrediente.Text = "";
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
            Query query = new Query("orden_produccion");

            string id = (string)ViewState["id"];

            query["planeacion"] = Conector.GetMySQLDateFormat(TFecha.Text);
            query["produccion"] = Conector.GetMySQLDateFormat(TFechaProduccion.Text);
            query["tercero"]= TClienteID.Text;
            query["producto"] = TProductoID.Text;
            query["lote"] = TLote.Text;
            query["cantidad"] = TCantidad.Text;
            query["observaciones"] = TObservaciones.Text;
            query["formulacion"] = TRecetaID.Text;

            if (string.IsNullOrEmpty(id))
            {
                int res = conector.QueryMod(query.GetInsert(), true);
                if (res == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
                id = res.ToString();
            }
            else
            {
                if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(id))) == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }
            if (conector.InsertModificacion(id) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
            query = new Query("orden_produccion_ingredientes");
            query["orden"] = id;

            for (int i = 0; i < lista.Count; i++)
            {
                if (string.IsNullOrEmpty(lista[i].IdTabla))
                {
                    if (lista[i].IdGrid != -1)
                    {
                        query["producto"] = lista[i].id;
                        query["cantidad"] = lista[i].cantidad.ToString(CultureInfo.InvariantCulture);

                        if (conector.QueryMod(query.GetInsert()) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                } else if (lista[i].IdGrid == -1) {
                    if (conector.QueryMod("DELETE FROM orden_produccion_ingredientes WHERE id=" + lista[i].IdTabla) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                }
            }

            conector.Commit();
            Session["orden_clean"] = 1;
            Session["parametros"] = "oi=" + id;
            ((ing)Master).SetResultado("Orden de producción " + id + " Editado");
        }
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Session["orden_clean"] = 1;
        Response.Redirect("~/produccion/orden.aspx");
    }

    protected void OnImprimir(object sender, EventArgs e)
    {
        GProductos.Columns[0].Visible =false;
    }

    protected void BFormular_Click(object sender, EventArgs e)
    {
        LMensaje.Text = "";
        if (TRecetaID.Text.Length == 0)
        {
            LMensaje.Text = "Por favor seleccione un producto";
            TProducto.Focus();
            return;
        }
        if (TCantidad.Text.Length == 0)
        {
            LMensaje.Text = "Por favor ingrese una cantidad";
            TCantidad.Focus();
            return;
        }

        using (CConector conector = new CConector())
        {
            Decimal cantidad = Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture);
            Query query = new Query("formulacion_ingrediente");

            query += "producto";
            query += "cantidad";

            List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
            
            // borramos los que no estan fijos
            lista.RemoveAll(x => string.IsNullOrEmpty(x.IdTabla));
            for (int i = 0; i < lista.Count; i++)
                if (lista[i].IdGrid != -1)
                    lista[i].IdGrid = i;

            MySqlDataReader reader = conector.Query(query.GetSelect("formulacion=" + Query.GetValor(TRecetaID.Text)));
            if (reader != null)
            {
                while (reader.Read())
                {
                    Producto info = new Producto(reader["producto"].ToString());
                    info.cantidad = Convert.ToDecimal(reader["cantidad"])*cantidad;
                    info.IdGrid = lista.Count;
                    lista.Add(info);
                }
            }

            HProductos.Value = Serializar.SerializeBinary(lista);
            GProductos.DataSource = lista.Where(x => x.IdGrid != -1).ToList();
            GProductos.DataBind();

            GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");
        }
        TLote.Focus();
    }

    protected void BActualizar_Click(object sender, EventArgs e)
    {
        List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
        HProductos.Value = "";

        for (int i = 0; i < lista.Count; i++)
        {
            TIngredienteID.Text = lista[i].id;
            TCantidadIngrediente.Text = lista[i].cantidad.ToString(CultureInfo.InvariantCulture);
            TIngrediente.Text = lista[i].NombreCompleto;
            Validate("agregar");
            BMas_Click(sender, e);
        }
    }
}
