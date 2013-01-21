using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class almacen_salida : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();

        SProductos.SelectCommand = string.Format(SProductos.SelectCommand, ((ing)Master).GetAlmacen());
        SProductos.UpdateCommand = string.Format(SProductos.UpdateCommand, ((ing)Master).GetAlmacen());
        SProductos.DeleteCommand = string.Format(SProductos.DeleteCommand, ((ing)Master).GetAlmacen());

        if (!IsPostBack)
        {
            if (Session["salida_clean"] != null)
            {

                ViewState["clean"] = true;
                Session["salida_clean"] = null;
            }
            else ViewState["clean"] = false;


            TNumero.Focus();
            ViewState["almacen"] = ((ing)Master).GetAlmacen();

            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            CAutomatico.Enabled = usuario.CanAdmin();

            if (!string.IsNullOrEmpty(Request.QueryString["si"]))
            {
                Query query = new Query("salida" + ((ing)Master).GetAlmacen());
                CConector conector = new CConector();

                query += "id";
                query += "observaciones";
                query += "fecha";
                query += "documento";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["si"])));
                if (reader != null && reader.Read())
                {
                    HId.Value = TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetModificacion(HId.Value, ((ing)Master).GetAlmacen());
                    TObservaciones.Text = reader["observaciones"].ToString();
                    TFecha.Text = CConector.GetFromMySQLDate(reader["fecha"]);
                    TDocumento.Text = reader["documento"].ToString();

                    TProducto.Focus();
                    GProductos.Columns[0].Visible = usuario.CanDelete();
                    BAceptar.Enabled = usuario.CanUpdate();
                    return;
                }
            }
            BAceptar.Enabled = usuario.CanInsert();
            TNumero.Text = new CConector().QueryString("SELECT IF(MAX(id),MAX(id)+1,1) FROM salida" + ((ing)Master).GetAlmacen());
            TFecha.Text = DateTime.Now.ToShortDateString();
            TNumero.Focus();
        }
    }

    protected void GProductos_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void BAgregar_Click(object sender, EventArgs e)
    {
        string[] saProducto = TProductoID.Text.Split(';');
        Producto producto = new Producto(saProducto[0]);
        LVencimiento.CssClass = TVencimiento.CssClass = producto.nVence > 0 ? "" : "hide";
        LLote.CssClass = TLote.CssClass = producto.nLote > 0 ? "" : "hide";
        TVencimiento.CssClass += " fecha";
        TLote.CssClass += " tlote";

        REntero.Enabled = producto.nNumero == 1;
        RDecimal.Enabled = producto.nNumero != 1;

        Page.Validate("producto");
        if (!Page.IsValid)
            return;

        ProductoUsado productousado = new ProductoUsado(HProductos.Value);
        ArrayList productos;

        if (saProducto.Length == 1)
        { // automatico
            productos = productousado.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen());
        }
        else
        { // manual
            productos = productousado.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen(), saProducto[1], saProducto[2]);
        }
        if (productos == null)
        {
            LMensaje.Text = "No se encontraron las cantidades disponibles.";
            return;
        }

        for (int i = 0; i < productos.Count; i++)
        {
            IOrderedDictionary data = new OrderedDictionary();
            producto = (Producto)productos[i];

            data.Add("codigo", producto.codigo);
            data.Add("nombre", producto.nombre);
            data.Add("producto", producto.id);
            data.Add("cantidad", producto.cantidad);
            data.Add("lote", producto.lote);
            data.Add("ubicacion", producto.ubicacion);
            data.Add("nubicacion", producto.NombreUbicacion);

            if (string.IsNullOrEmpty(producto.vence))
                data.Add("vence", null);
            else data.Add("vence", Convert.ToDateTime(producto.vence));

            data.Add("unidad", producto.unidad);

            SProductos.GetView().Insert(data, OnDataSourceOperationComplete);
        }


        GProductos.SelectedIndex = -1;
        GProductos.DataBind();

        LMensaje.Text = "";
        TProducto.Text = TProductoID.Text = TCantidad.Text = TLote.Text = TVencimiento.Text = "";
        TProducto.Focus();

        LVencimiento.CssClass = TVencimiento.CssClass = CAutomatico.Checked ? "hide" : "";
        LLote.CssClass = TLote.CssClass = CAutomatico.Checked ? "hide" : "";
        TVencimiento.CssClass += " fecha";
        TLote.CssClass += " tlote";

        REntero.Enabled = true;
        RDecimal.Enabled = true;

        HProductos.Value = productousado.Serialize();

        ScriptManager.RegisterStartupScript(TProducto, typeof(String), "pregunta", "window.onbeforeunload = function() { return '¿Ha realizado cambios, esta seguro de abandonar la pagina?'; };", true);
    }

    private bool OnDataSourceOperationComplete(int count, Exception e)
    {
        if (e != null)
        {
            throw e;
        }
        return false;
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        if (GProductos.Rows.Count == 0)
        {
            LMensaje.Text = "Por favor ingrese por lo menos un producto";
            return;
        }

        Query query = new Query("salida" + ((ing)Master).GetAlmacen());
        CConector conector = new CConector();

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TFecha.Text);
        query["documento"] = TDocumento.Text;

        conector.StartTransaction();
        bool existe = HId.Value.Length != 0;

        if (HId.Value.Length == 0)
        {
            int res = conector.QueryMod(query.GetInsert(), true);
            if (res == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            HId.Value = res.ToString();
        }
        else
        {
            if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(HId.Value))) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
        }

        if (conector.InsertModificacion(HId.Value, ((ing)Master).GetAlmacen()) == -1)
        {
            LMensaje.Text = conector.GetLastError();
            return;
        }

        string[] productos = SProductos.GetQuerys();
        for (int i = 0; i < productos.Length; i++)
        {
            if (conector.QueryMod(string.Format(productos[i], ((ing)Master).GetAlmacen()).Replace("@rsalida", HId.Value)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
        }

        conector.Commit();
        Session["salida_clean"] = 1;
        ((ing)Master).SetResultado("Remisión " + HId.Value + " editada.");
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Session["salida_clean"] = 1;
        Response.Redirect(Request.Url.AbsolutePath);
    }
    protected void GProductos_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        ProductoUsado productousado = new ProductoUsado(HProductos.Value);
        string vence = e.Values["vence"] == null ? "" : e.Values["vence"].ToString();
        string lote = e.Values["lote"] == null ? "" : e.Values["lote"].ToString();
        productousado.DeleteProducto(e.Keys["producto"].ToString(), Convert.ToDecimal(e.Values["cantidad"]), lote, vence, Convert.ToInt32(e.Keys["ubicacion"]));
        HProductos.Value = productousado.Serialize();
    }
    protected void BActualizar_Click(object sender, EventArgs e)
    {
        ProductoUsado productousado = new ProductoUsado(HProductos.Value);
        HProductos.Value = "";
        List<Producto> productos = productousado.GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            Producto producto = productos[i];
            TProductoID.Text = producto.id;
            TCantidad.Text = producto.cantidad.ToString(CultureInfo.InvariantCulture);
            TLote.Text = producto.lote;
            TVencimiento.Text = producto.vence;
            TProducto.Text = producto.NombreCompleto;
            BAgregar_Click(sender, e);
        }
    }
}
