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

        SUbicacion.SelectCommand = string.Format(SUbicacion.SelectCommand, ((ing)Master).GetAlmacen());

        if(!IsPostBack) {

            Utilidades.GetGridColumn(GProductos, "Ubicación ").Visible = false;

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

                    if (!string.IsNullOrEmpty(reader["traslado"].ToString())) {

                        ViewState["TrasladoId"] = reader["traslado"].ToString();
                        ViewState["TrasladoDesde"] = reader["desde"].ToString();

                        HTraslado.Text = "Traslado No. " + reader["traslado"].ToString() + " desde ";
                        HTraslado.NavigateUrl = "~/almacen/traslado.aspx?ti=" + reader["traslado"].ToString() + "&alm=" +  reader["desde"].ToString();
                        if (reader["desde"].ToString() == "0")
                            HTraslado.Text += "(Ajustes)";
                        else HTraslado.Text += conector.QueryString("SELECT nombre FROM almacen WHERE id=" + reader["desde"].ToString());
                    }

                    Utilidades.GetGridColumn(GProductos, "Ubicación").Visible = TFecha.Text.Length != 0;
                    Utilidades.GetGridColumn(GProductos, "Ubicación ").Visible = TFecha.Text.Length == 0;

                    ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);

                    query = new Query("entrada_items" + ((ing)Master).GetAlmacen());

                    query += "id";
                    query += "producto";
                    query += "cantidad";
                    query += "lote";
                    query += "vence";
                    query += "ubicacion";
                    query += "szSerial";
                    query += "estado";
                    query += "ingreso";
                    query += "costo";

                    reader= conector.Query(query.GetSelect("entrada=" + HId.Value));
                    if(reader!=null) {
                        while(reader.Read()) {

                            Producto info = new Producto(reader["producto"].ToString());

                            info.IdTabla= reader["id"].ToString();
                            info.cantidad= Convert.ToDecimal(reader["cantidad"]);
                            info.lote= reader["lote"].ToString();
                            info.vence= "";
                            try { info.vence = CConector.GetFromMySQLDate(reader["vence"]); }
                            catch { }
                            info.ubicacion= Convert.ToInt32(reader["ubicacion"]);
                            info.serial= reader["szSerial"].ToString();
                            info.estado= Convert.ToInt32(reader["estado"]);
                            info.Ingreso = reader["ingreso"].ToString()!="";
                            info.Costo = Convert.ToDecimal(reader["costo"]);

                            productoUsado.AddProducto(info);
                        }
                    }

                    GProductos.DataSource = productoUsado.GetProductosUsados();
                    HUsado.Value = productoUsado.SerializeBinary();
                    GProductos.DataBind();

                    TProducto.Focus();
                    GProductos.Columns[0].Visible = string.IsNullOrEmpty((string)ViewState["TrasladoId"]);
                    BAgregar.Enabled = string.IsNullOrEmpty((string)ViewState["TrasladoId"]);
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

        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);
        Producto producto= productoUsado.GetProducto(Convert.ToInt32(GProductos.SelectedDataKey[0]));

        TProducto.Text = producto.nombre;
        TCantidad.Text = producto.cantidad.ToString(CultureInfo.InvariantCulture);
        TLote.Text = producto.lote;
        TVencimiento.Text = producto.vence;
        TProductoID.Text = producto.id;
        TSerial.Text = producto.serial;
        DUbicacion.SelectedValue = producto.ubicacion.ToString();
        DEstado.SelectedValue = producto.estado.ToString();
        TCosto.Text = producto.Costo.ToString(CultureInfo.InvariantCulture);
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
        producto.Costo = Convert.ToDecimal(TCosto.Text, CultureInfo.InvariantCulture);
        producto.estado = Convert.ToInt32(DEstado.SelectedValue);
        producto.ubicacion= Convert.ToInt32(DUbicacion.SelectedValue);

        ProductoUsado productousadoNuevo = new ProductoUsado(HProductos.Value);
        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);

        if (GProductos.SelectedIndex == -1)
        {
            productoUsado.AddProducto(producto);
        }
        else
        {
            // revisar la edicion
            Producto info= productoUsado.GetProducto(Convert.ToInt32(GProductos.DataKeys[GProductos.SelectedIndex].Values[0]));

            productousadoNuevo.DeleteProducto(info.id,info.cantidad,info.lote, info.vence
                , info.ubicacion, info.estado, info.serial);

            productoUsado.UpdateProducto(info.IdGrid, producto);
        }
        
        GProductos.SelectedIndex = -1;
        GProductos.DataSource = productoUsado.GetProductosUsados();
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

        TSerial.Text= TProducto.Text = TProductoID.Text = TCantidad.Text= TLote.Text = TVencimiento.Text = "";
        DEstado.SelectedIndex = 0;
        DUbicacion.SelectedIndex = 0;
        TProducto.Focus();

        productousadoNuevo.GetProductosUsados().Add(producto);
        HProductos.Value = productousadoNuevo.Serialize();
        HUsado.Value = productoUsado.SerializeBinary();

        ScriptManager.RegisterStartupScript(TProducto, typeof(String), "pregunta", "window.onbeforeunload = function() { return '¿Ha realizado cambios, esta seguro de abandonar la pagina?'; };", true);
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

        string almacendesde = string.IsNullOrEmpty((string)ViewState["TrasladoDesde"]) ? "" : "";
        string trasladoId = string.IsNullOrEmpty((string)ViewState["TrasladoId"]) ? "" : "";

        if (almacendesde.Length>0 && trasladoId.Length>0)
        {
            if(conector.QueryMod(string.Format("UPDATE traslado{0} SET llegada={1} WHERE id={2}"
                , almacendesde, Query.GetValor(CConector.GetMySQLDateFormat(TFecha.Text)), trasladoId))==-1)
                return;
        }


        // actualizamos las ubicaciones de todos
        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);
        if (TFecha.Text.Length > 0 && Utilidades.GetGridColumn(GProductos, "Ubicación ").Visible)
        {
            for (int i = 0; i < GProductos.Rows.Count; i++)
            {
                DropDownList ubicacion = (DropDownList)GProductos.Rows[i].FindControl("DUbicacion");
                if (ubicacion.SelectedValue != "0")
                { // se modifico la ubicacion.
                    productoUsado.UpdateProductoUbicacion(Convert.ToInt32(GProductos.DataKeys[i].Values[0]), Convert.ToInt32(ubicacion.SelectedValue));
                }
            }
        }

        // eliminamos todos los productos
        List<Producto> productos= productoUsado.GetProductoEliminado();
        for(int i=0; i<productos.Count; i++) {
            if(conector.QueryMod(string.Format("DELETE FROM entrada_items{0} WHERE id={1}", ((ing)Master).GetAlmacen(), productos[i].IdTabla))==-1)
                return;
            if (productos[i].Ingreso)
            {
                if (!productos[i].DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
                if (!productos[i].UpdateCosto(conector, ((ing)Master).GetAlmacen(), "-1"))
                    return;
            }
            if (almacendesde.Length > 0 && trasladoId.Length > 0)
            {
                if (conector.QueryMod(string.Format("DELETE FROM traslado_items{0} WHERE traslado={1} AND entrada_items={2}", almacendesde, trasladoId, productos[i].IdTabla)) == -1)
                    return;
                bool ingresoold = productos[i].Ingreso;
                productos[i].Ingreso = true;
                if (!productos[i].DeleteProductoFromEstado(conector, almacendesde))
                    return;
                if (!productos[i].UpdateCosto(conector, almacendesde, "-1"))
                    return;
                productos[i].Ingreso = ingresoold;
            }
        }
        // agregamos los nuevos
        productos= productoUsado.GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            productoUsado.UpdateProductoIngreso(productos[i].IdGrid, TFecha.Text.Length > 0);

            query= productos[i].GetQuery("entrada_items" + ((ing)Master).GetAlmacen());
            query["costo"]= productos[i].Costo.ToString(CultureInfo.InvariantCulture);

            if(string.IsNullOrEmpty(productos[i].IdTabla)) {

                query["entrada"] = HId.Value;
                if (conector.QueryMod(query.GetInsert()) == -1)
                    return;
            } else if(productos[i].pProductoAntiguo!=null) { // fue actualizado
                if (conector.QueryMod(query.GetUpdate("id=" + productos[i].IdTabla)) == -1)
                    return;
            } else {
                continue;
            }
            if (productos[i].Ingreso) {
                if (!productos[i].AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
                if (!productos[i].UpdateCosto(conector, ((ing)Master).GetAlmacen(), TTerceroID.Text))
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
        ProductoUsado productousadoNuevo = new ProductoUsado(HProductos.Value);
        HProductos.Value = "";
        List<Producto> productos = productousadoNuevo.GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            Producto producto = productos[i];
            TProductoID.Text = producto.id;
            TCantidad.Text = producto.cantidad.ToString(CultureInfo.InvariantCulture);
            TLote.Text = producto.lote;
            TVencimiento.Text = producto.vence;
            TProducto.Text = producto.NombreCompleto;
            DEstado.SelectedValue = producto.estado.ToString();
            TSerial.Text = producto.serial;
            TCosto.Text = producto.Costo.ToString(CultureInfo.InvariantCulture);
            BAgregar_Click(sender, e);
        }
    }
    protected void GProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        // en la tabla definitiva
        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);
        Producto producto = productoUsado.GetProducto(Convert.ToInt32(GProductos.DataKeys[e.RowIndex].Values[0]));
        if (producto == null)
        {
            e.Cancel = true;
            return;
        }

        // ahora en los temporales
        ProductoUsado productousadoNuevo = new ProductoUsado(HProductos.Value);
        productousadoNuevo.DeleteProducto(producto.id, producto.cantidad, producto.lote, producto.vence, producto.ubicacion, producto.estado, producto.serial);
        HProductos.Value = productousadoNuevo.Serialize();
        

        productoUsado.DeleteProducto(producto.IdGrid);
        GProductos.DataSource = productoUsado.GetProductosUsados();
        GProductos.DataBind();
        HUsado.Value = productoUsado.SerializeBinary();
        e.Cancel = true;
    }
}
