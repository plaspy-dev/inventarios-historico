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
        ((ing)Master)._ImprimirClicked += new EventHandler(OnImprimir);
        ((ing)Master).CheckAlmacen();

        if (!IsPostBack)
        {

            CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

            Utilidades.GetGridColumn(GProductos, "Precio U.").Visible =
            Utilidades.GetGridColumn(GProductos, "Precio T.").Visible =
            RValorF.Visible = RValor.Visible =
            TValor.Visible = LValor.Visible = configuracion.nValor != 0;

            TValor.ReadOnly = configuracion.nValorTipo == 1;

            // fecha de entrega
            LEntrega.Visible = REntrega.Visible= 
            TEntrega.Visible = configuracion.bManejarEntrega;

            // lote
            Utilidades.GetGridColumn(GProductos, "Lote").Visible =
            LLoteA.Visible = TLote.Visible = LLote.Visible = configuracion.bLote;

            // vencimiento
            Utilidades.GetGridColumn(GProductos, "Vence").Visible =
            LVencimientoA.Visible = TVencimiento.Visible = LVencimiento.Visible = configuracion.bVencimiento;

            // serial
            Utilidades.GetGridColumn(GProductos, "Serial").Visible =
            LSerialA.Visible = TSerial.Visible = LSerial.Visible = configuracion.bSerial;

            // ubicacion
            Utilidades.GetGridColumn(GProductos, "Ubicación").Visible =
            configuracion.nUbicacion != 0;

            // estado
            Utilidades.GetGridColumn(GProductos, "Estado").Visible =
            LEstadoA.Visible = TEstado.Visible = LEstado.Visible = configuracion.bEstado;

            // tercero
            RTerceroID.Visible = configuracion.bTerceroSalida;
            RDocumento.Visible = configuracion.bDocumentoSalida;

            if (Session["salida_clean"] != null)
            {

                ViewState["clean"] = true;
                Session["salida_clean"] = null;
            }
            else ViewState["clean"] = false;

            if (Session["lsalida"] != null)
            {
                ViewState["urlprint"] = Request.Url.AbsolutePath + "?si=" + Session["lsalida"].ToString() + "&p=1";
                LImprimir.Text = "Salida " + Session["lsalida"].ToString() + " Editada.<br/>¿Desea imprimirla?";
                PImprimir.Visible = true;
                Session["lsalida"] = null;
            }

            TNumero.Focus();
            ViewState["almacen"] = ((ing)Master).GetAlmacen();

            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            CAutomatico.Enabled = usuario.CanAdmin() || configuracion.nSeleccionProducto!=0;
            CAutomatico.Checked = configuracion.nSeleccionProducto != 2;

            if (!string.IsNullOrEmpty(Request.QueryString["si"]))
            {
                Query query = new Query("salida" + ((ing)Master).GetAlmacen());
                CConector conector = new CConector();

                query += "id";
                query += "observaciones";
                query += "fecha";
                query += "entrega";
                query += "documento";
                query += "tercero";
                query += "estado";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["si"])));
                if (reader != null && reader.Read())
                {
                    HId.Value = TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetPrintFile("salida.html");
                    ((ing)Master).SetModificacion(HId.Value, ((ing)Master).GetAlmacen());
                    TObservaciones.Text = reader["observaciones"].ToString();
                    TFecha.Text = CConector.GetFromMySQLDate(reader["fecha"]);
                    TEntrega.Text = CConector.GetFromMySQLDate(reader["entrega"]);
                    TDocumento.Text = reader["documento"].ToString();
                    TTerceroID.Text = reader["tercero"].ToString();
                    TTercero.Text = Almacen.GetNombreTercero(reader["tercero"].ToString());
                    PAnulado.Visible = reader["estado"].ToString() != "";

                    ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);
                    query = new Query("salida_items" + ((ing)Master).GetAlmacen());

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
                    query += "valor";

                    reader = conector.Query(query.GetSelect("salida=" + HId.Value));
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Producto info = new Producto(reader["producto"].ToString());

                            info.IdTabla = reader["id"].ToString();
                            info.cantidad = Convert.ToDecimal(reader["cantidad"]);
                            info.lote = reader["lote"].ToString();
                            info.vence = "";
                            try { info.vence = CConector.GetFromMySQLDate(reader["vence"]); }
                            catch { }
                            info.ubicacion = Convert.ToInt32(reader["ubicacion"]);
                            info.serial = reader["szSerial"].ToString();
                            info.estado = Convert.ToInt32(reader["estado"]);
                            info.Ingreso = reader["ingreso"].ToString();
                            info.Costo = Convert.ToDecimal(reader["costo"]);
                            try
                            {
                                info.Valor = Convert.ToDecimal(reader["valor"]);
                            } catch {}

                            productoUsado.AddProducto(info);
                        }
                    }

                    GProductos.DataSource = productoUsado.GetProductosUsados();
                    HUsado.Value = productoUsado.SerializeBinary();
                    GProductos.DataBind();

                    GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Precio T.")].Text = string.Format("{0:C}", productoUsado.GetValorTotal());
                    GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = string.Format("{0:F}", productoUsado.GetTotalProductos());
                    GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Producto")].Text = "Total";

                    TProducto.Focus();
                    GProductos.Columns[0].Visible = usuario.CanDelete();
                    BAceptar.Enabled = usuario.CanUpdate();
                    BAnular.Visible = usuario.CanAdmin();
                    if (PAnulado.Visible)
                        GProductos.Columns[0].Visible = BAnular.Enabled = BAceptar.Enabled = false;
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
        LVencimientoA.CssClass= LVencimiento.CssClass = TVencimiento.CssClass = producto.nVence > 0 ? "" : "hide";
        LLoteA.CssClass= LLote.CssClass = TLote.CssClass = producto.nLote > 0 ? "" : "hide";
        LEstadoA.CssClass = LEstado.CssClass = TEstado.CssClass = producto.nEstado > 0 ? "" : "hide";
        LSerialA.CssClass= LSerial.CssClass = TSerial.CssClass = producto.nSerial > 0 ? "" : "hide";

        if(CAutomatico.Checked)
            TSerial.CssClass = TEstado.CssClass = TLote.CssClass = TVencimiento.CssClass = "hide";

        TVencimiento.CssClass += " fecha";
        TLote.CssClass += " tlote";
        TEstado.CssClass += " tlote";
        TSerial.CssClass += " tlote";

        REntero.Enabled = producto.nNumero == 1;
        RDecimal.Enabled = producto.nNumero != 1;

        Page.Validate("producto");
        if (!Page.IsValid)
            return;

        ProductoUsado productousadoNuevo = new ProductoUsado(HProductos.Value);
        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);

        ArrayList productos;

        if (saProducto.Length == 1)
        { // automatico
            productos = productousadoNuevo.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen(), true);
        }
        else
        { // manual
            productos = productousadoNuevo.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen(), saProducto[1], saProducto[2], Convert.ToInt32(saProducto[3]), saProducto[4], true);
        }
        if (productos == null)
        {
            LMensaje.Text = "No se encontraron las cantidades disponibles.";
            return;
        }

        for (int i = 0; i < productos.Count; i++)
        {
            producto = (Producto)productos[i];
            producto.Valor = Decimal.Parse(TValor.Text, NumberStyles.Currency, CultureInfo.InvariantCulture);
            productoUsado.AddProducto(producto);
        }


        GProductos.DataSource = productoUsado.GetProductosUsados();
        GProductos.SelectedIndex = -1;
        GProductos.DataBind();

        LMensaje.Text = "";
        TValor.Text= TProducto.Text = TProductoID.Text = TCantidad.Text = TLote.Text = TVencimiento.Text = TSerial.Text = TEstado.Text = "";
        TProducto.Focus();

        LVencimientoA.CssClass = LVencimiento.CssClass = TVencimiento.CssClass = CAutomatico.Checked ? "hide" : "";
        LLoteA.CssClass = LLote.CssClass = TLote.CssClass = CAutomatico.Checked ? "hide" : "";
        LSerialA.CssClass = LSerial.CssClass = TSerial.CssClass = CAutomatico.Checked ? "hide" : "";
        LEstadoA.CssClass= LEstado.CssClass = TEstado.CssClass = CAutomatico.Checked ? "hide" : "";

        TVencimiento.CssClass += " fecha";
        TLote.CssClass += " tlote";
        TSerial.CssClass += " tlote";
        TEstado.CssClass += " tlote";

        REntero.Enabled = true;
        RDecimal.Enabled = true;

        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Precio T.")].Text = string.Format("{0:C}", productoUsado.GetValorTotal());
        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = string.Format("{0:F}", productoUsado.GetTotalProductos());
        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Producto")].Text = "Total";

        HProductos.Value = productousadoNuevo.Serialize();
        HUsado.Value = productoUsado.SerializeBinary();

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

        CConfiguracion configuracion = CConfiguracion.GetConfiguracion();
        Query query = new Query("salida" + ((ing)Master).GetAlmacen());
        CConector conector = new CConector();

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TFecha.Text);
        query["entrega"] = CConector.GetMySQLDateFormat(configuracion.bManejarEntrega ? TEntrega.Text : TFecha.Text);
        query["documento"] = TDocumento.Text;
        query["tercero"] = TTerceroID.Text;
        if (sender == BAnular)
            query["estado"] = "-1";

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

        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);

        // eliminamos todos los productos
        List<Producto> productos = productoUsado.GetProductoEliminado();
        for (int i = 0; i < productos.Count; i++)
        {
            if (conector.QueryMod(string.Format("DELETE FROM salida_items{0} WHERE id={1}", ((ing)Master).GetAlmacen(), productos[i].IdTabla)) == -1)
                return;
            if (productos[i].bIngreso)
            {
                if (!productos[i].AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
            }
        }

        // agregamos los nuevos
        productos = productoUsado.GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            productos[i].bIngreso = true;
            if (sender == BAnular)
            {
                bool oldingresoantiguo = false;
                bool oldingreso = productos[i].bIngreso;
                productos[i].bIngreso = true;
                if (productos[i].pProductoAntiguo != null)
                {
                    oldingresoantiguo = productos[i].pProductoAntiguo.bIngreso;
                    productos[i].pProductoAntiguo.bIngreso = false;
                }
                if (!productos[i].AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
                productos[i].bIngreso = oldingreso;
                if (productos[i].pProductoAntiguo != null)
                {
                    productos[i].pProductoAntiguo.bIngreso = oldingresoantiguo;
                }

                productoUsado.UpdateProductoIngreso(productos[i].IdGrid, false);
            }

            if (string.IsNullOrEmpty(productos[i].IdTabla))
                productos[i].GetCostoPromedio(((ing)Master).GetAlmacen()).ToString(CultureInfo.InvariantCulture);

            query = productos[i].GetQuery("salida_items" + ((ing)Master).GetAlmacen());

            // recalculamos los precios para los que tienen lista de precios cerradas.
            if (configuracion.nValorTipo == 1)
                productos[i].GetValor(TTerceroID.Text, Convert.ToInt32(((ing)Master).GetAlmacen()));
            
            query["valor"] = productos[i].Valor.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(productos[i].IdTabla))
            {
                query["salida"] = HId.Value;
                if (conector.QueryMod(query.GetInsert()) == -1)
                    return;
            }
            else if (productos[i].pProductoAntiguo != null)
            { // fue actualizado
                if (conector.QueryMod(query.GetUpdate("id=" + productos[i].IdTabla)) == -1)
                    return;
            } else {
                continue;
            }
            if (productos[i].bIngreso)
            {
                if (!productos[i].DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
                productos[i].UpdateValor(conector);
            }
        }

        conector.Commit();
        Alertas.RemoveAlertas(((ing)Master).GetAlmacen());
        Session["salida_clean"] = 1;
        Session["lsalida"] = HId.Value;

        ((ing)Master).SetResultado("Remisión " + HId.Value + " editada.");
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Session["salida_clean"] = 1;
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
            TEstado.Text = producto.szEstado;
            TSerial.Text = producto.serial;
            TValor.Text = producto.Valor.ToString(CultureInfo.InvariantCulture);
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

        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Precio T.")].Text = string.Format("{0:C}", productoUsado.GetValorTotal());
        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = string.Format("{0:F}", productoUsado.GetTotalProductos());
        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Producto")].Text = "Total";

        e.Cancel = true;
    }
    protected void OnImprimir(object sender, EventArgs e)
    {
        GProductos.Columns[0].Visible = false;
    }
}
