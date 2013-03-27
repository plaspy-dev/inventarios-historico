using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections.Specialized;
using SDG.Componentes;
using MySql.Data.MySqlClient;


public partial class almacen_entrada : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();

        SProductos.SelectCommand = string.Format(SProductos.SelectCommand, ((ing)Master).GetAlmacen());
        SProductos.UpdateCommand = string.Format(SProductos.UpdateCommand, ((ing)Master).GetAlmacen());
        SProductos.DeleteCommand = string.Format(SProductos.DeleteCommand, ((ing)Master).GetAlmacen());
        SUbicacion.SelectCommand = string.Format(SUbicacion.SelectCommand, ((ing)Master).GetAlmacen());

        if(!IsPostBack) {

            GProductos.Columns[10].Visible = false;
            if (Session["entrada_clean"] != null)
            {
                ViewState["clean"] = true;
                Session["entrada_clean"] = null;
            }
            else ViewState["clean"] = false;

            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            if(!string.IsNullOrEmpty(Request.QueryString["ei"])) {

                Query query = new Query("entrada" + ((ing)Master).GetAlmacen());
                CConector conector = new CConector();

                query += "id";
                query+= "observaciones";
                query+="fecha";
                query += "desde";
                query += "traslado";
                query += "documento";
                query += "tercero";

                MySqlDataReader reader= conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["ei"])));
                if(reader!=null && reader.Read()) {

                    HId.Value = TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetModificacion(HId.Value, ((ing)Master).GetAlmacen());
                    TObservaciones.Text = reader["observaciones"].ToString();
                    TFecha.Text = CConector.GetFromMySQLDate(reader["fecha"]);
                    TDocumento.Text = reader["documento"].ToString();
                    TTerceroID.Text = reader["tercero"].ToString();
                    TTercero.Text = Almacen.GetNombreTercero(reader["tercero"].ToString());

                    bool traslado = false;
                    if (!string.IsNullOrEmpty(reader["traslado"].ToString())) {

                        traslado = true;
                        HTraslado.Text = "Traslado No. " + reader["traslado"].ToString() + " desde ";
                        HTraslado.NavigateUrl = "~/almacen/traslado.aspx?ti=" + reader["traslado"].ToString() + "&alm=" +  reader["desde"].ToString();
                        if (reader["desde"].ToString() == "0")
                            HTraslado.Text += "(Ajustes)";
                        else HTraslado.Text += conector.QueryString("SELECT nombre FROM almacen WHERE id=" + reader["desde"].ToString());
                    }

                    GProductos.Columns[9].Visible = TFecha.Text.Length != 0;
                    GProductos.Columns[10].Visible = TFecha.Text.Length == 0;

                    TProducto.Focus();
                    GProductos.Columns[0].Visible = !traslado;
                    BAgregar.Enabled = !traslado;
                    GProductos.Columns[1].Visible = usuario.CanDelete();
                    BAceptar.Enabled = usuario.CanUpdate();
                    return;
                }
            }
            BAceptar.Enabled = usuario.CanInsert();
            TNumero.Text = new CConector().QueryString("SELECT IF(MAX(id),MAX(id)+1,1) FROM entrada" + ((ing)Master).GetAlmacen());
            TFecha.Text = DateTime.Now.ToShortDateString();
            TNumero.Focus();
        }
    }

    protected void GProductos_SelectedIndexChanged(object sender, EventArgs e)
    {
        TProducto.Text= HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[3].Text).Trim();
        TCantidad.Text= Convert.ToDecimal(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[4].Text).Trim()).ToString(CultureInfo.InvariantCulture);
        TUnidad.Text = HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[5].Text).Trim();
        TLote.Text= HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[6].Text).Trim();
        TVencimiento.Text= HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[7].Text).Trim();
        TProductoID.Text = GProductos.DataKeys[GProductos.SelectedIndex].Values[1].ToString();
        TSerial.Text = HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[8].Text).Trim();
        DUbicacion.SelectedValue = GProductos.DataKeys[GProductos.SelectedIndex].Values[2].ToString();
        DEstado.SelectedValue = GProductos.DataKeys[GProductos.SelectedIndex].Values[3].ToString();
    }

    protected void BAgregar_Click(object sender, EventArgs e)
    {
        Producto producto = new Producto(TProductoID.Text);

        RVencimiento.Enabled = producto.nVence==1;
        RLote.Enabled = producto.nLote == 1;
        REntero.Enabled = producto.nNumero == 1;
        RDecimal.Enabled = producto.nNumero != 1;
        RSerial.Enabled = producto.nSerial==1;
        REstado.Enabled = producto.nEstado == 1;

        LSerial.CssClass = TSerial.CssClass = producto.nSerial > 0 ? "" : "hide";
        LEstado.CssClass = DEstado.CssClass = producto.nEstado > 0 ? "" : "hide";
        LVencimiento.CssClass= TVencimiento.CssClass = producto.nVence>0 ? "" : "hide";
        LLote.CssClass = TLote.CssClass = producto.nLote > 0 ? "" : "hide";
        TLote.CssClass += " tlote";
        TVencimiento.CssClass += " fecha";
        TSerial.CssClass += " tlote";

        Page.Validate("producto");
        if (!Page.IsValid)
            return;

        producto.cantidad=Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture);
        producto.lote=TLote.Text;
        producto.vence=TVencimiento.Text;
        producto.serial = TSerial.Text;
        producto.estado = Convert.ToInt32(DEstado.SelectedValue);
        producto.ubicacion= Convert.ToInt32(DUbicacion.SelectedValue);

        ProductoUsado productousado = new ProductoUsado(HProductos.Value);

        IOrderedDictionary data = new OrderedDictionary();
        data.Add("nombre", producto.nombre);
        data.Add("codigo", producto.codigo);
        data.Add("producto", producto.id);
        data.Add("cantidad", producto.cantidad);
        data.Add("lote", producto.lote);
        object vence = null;
        if (!string.IsNullOrEmpty(producto.vence))
            vence = Convert.ToDateTime(producto.vence);
        data.Add("vence", vence);
        data.Add("unidad", producto.unidad);
        data.Add("nubicacion", DUbicacion.SelectedItem.Text);
        data.Add("ubicacion", producto.ubicacion);
        data.Add("estado", producto.estado);
        data.Add("szEstado", producto.szEstado);
        data.Add("szSerial", producto.serial);

        if (GProductos.SelectedIndex == -1)
        {
            SProductos.GetView().Insert(data, OnDataSourceOperationComplete);
        }
        else
        {
            data.Add("id", GProductos.DataKeys[GProductos.SelectedIndex].Values[0]);

            IOrderedDictionary dataold = new OrderedDictionary();

            productousado.DeleteProducto(GProductos.DataKeys[GProductos.SelectedIndex].Values[1].ToString(),
                Convert.ToDecimal(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[4].Text).Trim()),
                HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[6].Text).Trim(),
                HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[7].Text).Trim(),
                Convert.ToInt32(GProductos.DataKeys[GProductos.SelectedIndex].Values[2]),
                Convert.ToInt32(GProductos.DataKeys[GProductos.SelectedIndex].Values[3]),
                HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[8].Text).Trim()
                );

            dataold.Add("nombre", HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[3].Text).Trim());
            dataold.Add("cantidad", Convert.ToDecimal(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[4].Text).Trim()));
            dataold.Add("lote", HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[6].Text).Trim());
            dataold.Add("vence", Convert.ToDateTime(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[7].Text).Trim()));
            dataold.Add("szSerial", Convert.ToDateTime(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[8].Text).Trim()));

            dataold.Add("id", GProductos.DataKeys[GProductos.SelectedIndex].Values[0]);

            SProductos.GetView().Update(data, data, dataold, OnDataSourceOperationComplete);
        }
        GProductos.SelectedIndex = -1;
        GProductos.DataBind();

        RVencimiento.Enabled = true;
        RLote.Enabled = true;
        REntero.Enabled = true;
        RDecimal.Enabled = true;
        REstado.Enabled = true;
        RSerial.Enabled = true;

        LVencimiento.CssClass = TVencimiento.CssClass = "";
        LLote.CssClass = TLote.CssClass = "";
        LEstado.CssClass = DEstado.CssClass = "";
        LSerial.CssClass = TSerial.CssClass = "";

        TLote.CssClass += " tlote";
        TSerial.CssClass += " tlote";
        TVencimiento.CssClass +=" fecha";

        TSerial.Text= TUnidad.Text= TProducto.Text = TProductoID.Text = TCantidad.Text= TLote.Text = TVencimiento.Text = "";
        DEstado.SelectedIndex = 0;
        DUbicacion.SelectedIndex = 0;
        TProducto.Focus();

        productousado.GetProductosUsados().Add(producto);
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

    protected void GProductos_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        ProductoUsado productousado = new ProductoUsado(HProductos.Value);
        string vence = e.Values["vence"] == null ? "" : e.Values["vence"].ToString();
        string lote = e.Values["lote"] == null ? "" : e.Values["lote"].ToString();
        string serial = e.Values["szSerial"] == null ? "" : e.Values["szSerial"].ToString();
        productousado.DeleteProducto(e.Keys["producto"].ToString(), Convert.ToDecimal(e.Values["cantidad"]), lote, vence, Convert.ToInt32(e.Keys["ubicacion"]), Convert.ToInt32(e.Keys["estado"]), serial);
        HProductos.Value = productousado.Serialize();
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        if (GProductos.Rows.Count == 0) {
            LMensaje.Text = "Por favor ingrese por lo menos un producto";
            return;
        }

        Query query = new Query("entrada" + ((ing)Master).GetAlmacen());
        CConector conector = new CConector();

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TFecha.Text);
        query["documento"] = TDocumento.Text;
        query["tercero"] = TTerceroID.Text;

        conector.StartTransaction();

        bool existe = HId.Value.Length>0;

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
            if (conector.QueryMod(string.Format(productos[i], ((ing)Master).GetAlmacen()).Replace("@rentrada", HId.Value)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
        }

        if(existe) {

            MySqlDataReader reader = conector.Query("SELECT desde,traslado FROM entrada" + ((ing)Master).GetAlmacen() + " WHERE id=" + HId.Value);
            if (reader != null && reader.Read())
            {
                string almacendesde = reader["desde"].ToString();
                string trasladoId = reader["traslado"].ToString();

                if (almacendesde.Length > 0 && trasladoId.Length > 0)
                    if (conector.QueryMod(string.Format("DELETE traslado_items{0} FROM traslado_items{0} LEFT JOIN entrada_items{1} ON (entrada_items{1}.id=traslado_items{0}.entrada_items) WHERE traslado_items{0}.traslado=" + trasladoId + " AND entrada_items{1}.id IS NULL;", almacendesde, ((ing)Master).GetAlmacen())) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }

            }
        }

        if (TFecha.Text.Length > 0)
        {
            if(GProductos.Columns[10].Visible) {
                for (int i = 0; i < GProductos.Rows.Count; i++)
                {
                    DropDownList ubicacion = (DropDownList)GProductos.Rows[i].FindControl("DUbicacion");
                    if(ubicacion.SelectedValue!="0") { // se modifico la ubicacion.
                        if (conector.QueryMod("UPDATE entrada_items" + ((ing)Master).GetAlmacen() + " SET ubicacion=" + ubicacion.SelectedValue + " WHERE id=" + GProductos.DataKeys[i].Values[0]) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                }
            }
            if (conector.QueryMod("UPDATE entrada_items" + ((ing)Master).GetAlmacen() + " SET ingreso=SYSDATE() WHERE entrada=" + HId.Value + " AND ingreso IS NULL") == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
        }

        conector.Commit();
        Session["entrada_clean"] = 1;
        ((ing)Master).SetAlerta("Entrada " + HId.Value + " editada.");
        ((ing)Master).SetResultado("Entrada " + HId.Value + " editada.");
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Session["entrada_clean"] = 1;
        Response.Redirect(Request.Url.AbsolutePath);
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
            TUnidad.Text = producto.unidad;
            DEstado.SelectedValue = producto.estado.ToString();
            TSerial.Text = producto.serial;
            BAgregar_Click(sender, e);
        }
    }
}
