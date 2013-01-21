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

public partial class almacen_traslado : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();

        SProductos.SelectCommand = string.Format(SProductos.SelectCommand, ((ing)Master).GetAlmacen());
        SProductos.UpdateCommand = string.Format(SProductos.UpdateCommand, ((ing)Master).GetAlmacen());
        SProductos.DeleteCommand = string.Format(SProductos.DeleteCommand, ((ing)Master).GetAlmacen());

        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            if(Session["traslado_clean"]!=null) {

                ViewState["clean"] = true;
                Session["traslado_clean"] = null;
            } else ViewState["clean"] = false;

            CAutomatico.Enabled = usuario.CanAdmin();
            TNumero.Focus();
            ViewState["almacen"] = ((ing)Master).GetAlmacen();
            SAlmacen.SelectCommand = SAlmacen.SelectCommand.Replace("@almacen", ((ing)Master).GetAlmacen());
            if (((ing)Master).GetAlmacen() == "0" && DAlmacen.Items.Count>0 && DAlmacen.Items[0].Value=="0")
            {
                DAlmacen.Items.RemoveAt(0);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["ti"]))
            {
                Query query = new Query("traslado" + ((ing)Master).GetAlmacen());
                CConector conector = new CConector();

                query += "id";
                query += "observaciones";
                query += "envio";
                query += "llegada";
                query += "entrada";
                query += "destino";
                query += "documento";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["ti"])));
                if (reader != null && reader.Read())
                {
                    HId.Value = TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetModificacion(HId.Value, ((ing)Master).GetAlmacen());
                    TObservaciones.Text = reader["observaciones"].ToString();
                    TFecha.Text = CConector.GetFromMySQLDate(reader["envio"]);
                    TLlegada.Text = CConector.GetFromMySQLDate(reader["llegada"]);
                    TDocumento.Text = reader["documento"].ToString();
                    DAlmacen.DataBind();
                    DAlmacen.SelectedValue = reader["destino"].ToString();

                    HEntrada.Text = "Entrada No. " + reader["entrada"].ToString() + " hacia " + DAlmacen.SelectedItem.Text;
                    HEntrada.NavigateUrl = "~/almacen/entrada.aspx?ei=" + reader["entrada"].ToString() + "&alm=" + DAlmacen.SelectedValue;

                    DAlmacen.Enabled = false;
                    TProducto.Focus();
                    GProductos.Columns[0].Visible = usuario.CanDelete();
                    BAceptar.Enabled = usuario.CanUpdate();
                    return;
                }
            }
            TLlegada.ReadOnly = !usuario.CanAdmin();
            BAceptar.Enabled = usuario.CanInsert();
            TNumero.Text = new CConector().QueryString("SELECT IF(MAX(id),MAX(id)+1,1) FROM traslado" + ((ing)Master).GetAlmacen());
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

        REntero.Enabled = producto.nNumero == 1;
        RDecimal.Enabled = producto.nNumero != 1;

        LVencimiento.CssClass = TVencimiento.CssClass = producto.nVence > 0 ? "" : "hide";
        LLote.CssClass = TLote.CssClass = producto.nLote > 0 ? "" : "hide";
        TVencimiento.CssClass += " fecha";
        TLote.CssClass += " tlote";

        Page.Validate("producto");
        if (!Page.IsValid)
            return;

        ProductoUsado productousado = new ProductoUsado(HProductos.Value);
        ArrayList productos;        

        if (saProducto.Length == 1)
        { // automatico
            productos = productousado.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen());
        } else { // manual
            productos = productousado.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen(), saProducto[1], saProducto[2]);
        }
        if (productos == null)
        {
            LMensaje.Text = "No se encontraron las cantidades disponibles.";
            return;
        }

        for(int i=0; i<productos.Count; i++) {

            IOrderedDictionary data = new OrderedDictionary();
            producto = (Producto)productos[i];

            data.Add("codigo", producto.codigo);
            data.Add("nombre", producto.nombre);
            data.Add("producto", producto.id);
            data.Add("cantidad", producto.cantidad);
            data.Add("lote", producto.lote);
            if(string.IsNullOrEmpty(producto.vence))
                data.Add("vence", null);
            else data.Add("vence",  Convert.ToDateTime(producto.vence));
            data.Add("unidad", producto.unidad);
            data.Add("ubicacion", producto.ubicacion);
            data.Add("nubicacion", producto.NombreUbicacion);
            /*if (sender != BAgregar)
                data.Add("id", TProcedimientoID.Text);*/

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

        Query query = new Query("traslado" + ((ing)Master).GetAlmacen());
        CConector conector = new CConector();
        string entradaId = "";

        query["observaciones"] = TObservaciones.Text;
        query["envio"] = CConector.GetMySQLDateFormat(TFecha.Text);
        query["llegada"] = CConector.GetMySQLDateFormat(TLlegada.Text);
        query["documento"] = TDocumento.Text;

        conector.StartTransaction();
        bool existe = HId.Value.Length != 0;

        if (HId.Value.Length == 0)
        {
            query["destino"] = DAlmacen.SelectedValue;

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
            if (conector.QueryMod(string.Format(productos[i], ((ing)Master).GetAlmacen()).Replace("@rtraslado", HId.Value)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
        }

        // agregamos la entrada

        query = new Query("entrada" + DAlmacen.SelectedValue);

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TLlegada.Text);

        if(existe) {
            entradaId = conector.QueryString("SELECT entrada FROM traslado" + ((ing)Master).GetAlmacen() + " WHERE id=" + HId.Value);
            if (conector.QueryMod(query.GetUpdate("id=" + entradaId)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

        } else {
            
            query["desde"] = ((ing)Master).GetAlmacen();
            query["traslado"] = HId.Value;

            int nEntrada = conector.QueryMod(query.GetInsert(), true);
            if (nEntrada == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            entradaId = nEntrada.ToString();
            conector.QueryMod("UPDATE traslado" + ((ing)Master).GetAlmacen() + " SET entrada=" + entradaId + " WHERE id=" + HId.Value);
        }

        if (conector.InsertModificacion(entradaId, ((ing)Master).GetAlmacen(), "~/almacen/entrada.aspx") == -1)
        {
            LMensaje.Text = conector.GetLastError();
            return;
        }

        // agregamos los elmentos de la entrada
        ArrayList ids = new ArrayList();
        MySqlDataReader reader = conector.Query(string.Format("SELECT traslado_items{0}.id FROM traslado_items{0} "
                                                        + " LEFT JOIN entrada_items{1} ON (entrada_items{1}.id=traslado_items{0}.entrada_items) "
                                                        + " WHERE traslado={2} AND entrada_items{1}.id IS NULL;", ((ing)Master).GetAlmacen(), DAlmacen.SelectedValue, HId.Value));
        if (reader != null)
            while (reader.Read())
                ids.Add(reader["id"].ToString());

        for (int i = 0; i < ids.Count; i++ ) {
            int nEntrada= conector.QueryMod("INSERT INTO entrada_items" + DAlmacen.SelectedValue + " (entrada,producto,cantidad,lote,vence,traslado_items) SELECT " + entradaId + ",producto,cantidad,lote,vence,id FROM traslado_items" + ((ing)Master).GetAlmacen() + " WHERE id=" + ids[i], true);
            if (nEntrada == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            if (conector.QueryMod("UPDATE traslado_items" + ((ing)Master).GetAlmacen() + " SET entrada_items=" + nEntrada + " WHERE id=" + ids[i]) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
        }

        if (conector.QueryMod(string.Format("DELETE entrada_items{0} FROM entrada_items{0} LEFT JOIN traslado_items{1} ON (entrada_items{0}.traslado_items=traslado_items{1}.id) WHERE entrada_items{0}.entrada=" + entradaId + " AND traslado_items{1}.id IS NULL;", DAlmacen.SelectedValue, ((ing)Master).GetAlmacen())) == -1)
        {
            LMensaje.Text = conector.GetLastError();
            return;
        }

        if(TLlegada.Text.Length>0)
            if (conector.QueryMod("UPDATE entrada_items" + DAlmacen.SelectedValue + " SET ingreso=SYSDATE() WHERE entrada=" + entradaId + " AND ingreso IS NULL") == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

        conector.Commit();
        Session["traslado_clean"] = 1;
        ((ing)Master).SetResultado("Traslado " + HId.Value + " editada.");
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Session["traslado_clean"]=1;
        Response.Redirect(Request.Url.AbsolutePath);
    }
    protected void GProductos_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        ProductoUsado productousado = new ProductoUsado(HProductos.Value);
        string vence= e.Values["vence"]==null ? "" : e.Values["vence"].ToString();
        string lote = e.Values["lote"] == null ? "" : e.Values["lote"].ToString();
        productousado.DeleteProducto(e.Keys["producto"].ToString(), Convert.ToDecimal(e.Values["cantidad"]), lote, vence, Convert.ToInt32(e.Keys["ubicacion"]));
        HProductos.Value = productousado.Serialize();
    }
    protected void BActualizar_Click(object sender, EventArgs e)
    {
        ProductoUsado productousado= new ProductoUsado(HProductos.Value);
        HProductos.Value = "";
        List<Producto> productos = productousado.GetProductosUsados();
        for(int i=0; i<productos.Count; i++) {
            
            Producto producto = productos[i];
            TProductoID.Text= producto.id;
            TCantidad.Text = producto.cantidad.ToString(CultureInfo.InvariantCulture);
            TLote.Text = producto.lote;
            TVencimiento.Text = producto.vence;
            TProducto.Text = producto.NombreCompleto;
            BAgregar_Click(sender, e);
        }
    }
}
