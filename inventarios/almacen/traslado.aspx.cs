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
        ((ing)Master)._ImprimirClicked += new EventHandler(OnImprimir);
        ((ing)Master).CheckAlmacen();

        if (!IsPostBack)
        {
            CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

            // lote
            Utilidades.GetGridColumn(GProductos, "Lote").Visible =
            LLoteA.Visible= TLote.Visible = LLote.Visible = configuracion.bLote;

            // vencimiento
            Utilidades.GetGridColumn(GProductos, "Vence").Visible =
            LVencimientoA.Visible= TVencimiento.Visible = LVencimiento.Visible = configuracion.bVencimiento;

            // serial
            Utilidades.GetGridColumn(GProductos, "Serial").Visible =
            LSerialA.Visible= TSerial.Visible = LSerial.Visible = configuracion.bSerial;

            // ubicacion
            Utilidades.GetGridColumn(GProductos, "Ubicación").Visible =
            configuracion.nUbicacion != 0;

            // estado
            Utilidades.GetGridColumn(GProductos, "Estado").Visible =
            LEstadoA.Visible= TEstado.Visible = LEstado.Visible = configuracion.bEstado;


            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            if(Session["traslado_clean"]!=null) {

                ViewState["clean"] = true;
                Session["traslado_clean"] = null;
            } else ViewState["clean"] = false;
            
            if (Session["ltraslado"] != null)
            {
                ViewState["urlprint"] = Request.Url.AbsolutePath + "?ti=" + Session["ltraslado"].ToString() + "&p=1";
                LImprimir.Text = "Traslado " + Session["ltraslado"].ToString() + " Editado.<br/>¿Desea imprimirla?";
                PImprimir.Visible = true;
                Session["ltraslado"] = null;
            }

            CAutomatico.Enabled = usuario.CanAdmin() || configuracion.nSeleccionProducto != 0;
            CAutomatico.Checked = configuracion.nSeleccionProducto != 2;

            TNumero.Focus();
            ViewState["almacen"] = ((ing)Master).GetAlmacen();
            SAlmacen.SelectCommand = SAlmacen.SelectCommand.Replace("@almacen", ((ing)Master).GetAlmacen());
            if (((ing)Master).GetAlmacen() == "0" && DDestino.Items.Count > 0 && DDestino.Items[0].Value == "0")
            {
                DDestino.Items.RemoveAt(0);
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
                query += "estado";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["ti"])));
                if (reader != null && reader.Read())
                {
                    ((ing)Master).SetPrintFile("traslado.html");
                    HId.Value = TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetModificacion(HId.Value, ((ing)Master).GetAlmacen());
                    TObservaciones.Text = reader["observaciones"].ToString();
                    TFecha.Text = CConector.GetFromMySQLDate(reader["envio"]);
                    ViewState["llegada"]= TLlegada.Text = CConector.GetFromMySQLDate(reader["llegada"]);
                    TDocumento.Text = reader["documento"].ToString();
                    DDestino.DataBind();
                    DDestino.SelectedValue = reader["destino"].ToString();
                    PAnulado.Visible = reader["estado"].ToString() != "";

                    HEntrada.Text = "Entrada No. " + reader["entrada"].ToString() + " hacia " + DDestino.SelectedItem.Text;
                    HEntrada.NavigateUrl = "~/almacen/entrada.aspx?ei=" + reader["entrada"].ToString() + "&alm=" + DDestino.SelectedValue;


                    ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);
                    query = new Query("traslado_items" + ((ing)Master).GetAlmacen());

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

                    reader = conector.Query(query.GetSelect("traslado=" + HId.Value));
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

                            productoUsado.AddProducto(info);
                        }
                    }

                    GProductos.DataSource = productoUsado.GetProductosUsados();
                    HUsado.Value = productoUsado.SerializeBinary();
                    GProductos.DataBind();

                    DDestino.Enabled = false;
                    TProducto.Focus();
                    GProductos.Columns[0].Visible = usuario.CanDelete();
                    BAceptar.Enabled = usuario.CanUpdate();
                    BAnular.Visible = usuario.CanAdmin();
                    if (PAnulado.Visible)
                        GProductos.Columns[0].Visible = BAnular.Enabled = BAceptar.Enabled = false;
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

        LVencimientoA.CssClass= LVencimiento.CssClass = TVencimiento.CssClass = producto.nVence > 0 ? "" : "hide";
        LLoteA.CssClass= LLote.CssClass = TLote.CssClass = producto.nLote > 0 ? "" : "hide";
        LEstadoA.CssClass= LEstado.CssClass = TEstado.CssClass = producto.nEstado > 0 ? "" : "hide";
        LSerialA.CssClass= LSerial.CssClass = TSerial.CssClass = producto.nSerial > 0 ? "" : "hide";
        
        if (CAutomatico.Checked)
            TSerial.CssClass = TEstado.CssClass = TLote.CssClass = TVencimiento.CssClass = "hide";

        TVencimiento.CssClass += " fecha";
        TLote.CssClass += " tlote";
        TEstado.CssClass += " tlote";
        TSerial.CssClass += " tlote";

        Page.Validate("producto");
        if (!Page.IsValid)
            return;

        ProductoUsado productousadoNuevo = new ProductoUsado(HProductos.Value);
        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);
        ArrayList productos;

        if (saProducto.Length == 1)
        { // automatico
            productos = productousadoNuevo.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen());
        } else { // manual
            productos = productousadoNuevo.GetProductos(saProducto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen(), saProducto[1], saProducto[2], Convert.ToInt32(saProducto[3]), saProducto[4]);
        }
        if (productos == null)
        {
            LMensaje.Text = "No se encontraron las cantidades disponibles.";
            return;
        }

        for(int i=0; i<productos.Count; i++) {

            producto = (Producto)productos[i];
            productoUsado.AddProducto(producto);
        }


        GProductos.DataSource = productoUsado.GetProductosUsados();
        GProductos.SelectedIndex = -1;
        GProductos.DataBind();

        LMensaje.Text = "";
        TProducto.Text = TProductoID.Text = TCantidad.Text = TLote.Text = TVencimiento.Text = TSerial.Text= TEstado.Text= "";
        TProducto.Focus();

        LVencimientoA.CssClass= LVencimiento.CssClass = TVencimiento.CssClass = CAutomatico.Checked ? "hide" : "";
        LLoteA.CssClass = LLote.CssClass = TLote.CssClass = CAutomatico.Checked ? "hide" : "";
        LSerialA.CssClass= LSerial.CssClass = TSerial.CssClass = CAutomatico.Checked ? "hide" : "";
        LEstadoA.CssClass= LEstado.CssClass = TEstado.CssClass = CAutomatico.Checked ? "hide" : "";
        TVencimiento.CssClass += " fecha";
        TLote.CssClass += " tlote";
        TSerial.CssClass += " tlote";
        TEstado.CssClass += " tlote";

        REntero.Enabled = true;
        RDecimal.Enabled = true;

        HProductos.Value = productousadoNuevo.Serialize();
        HUsado.Value = productoUsado.SerializeBinary();

        ScriptManager.RegisterStartupScript(TProducto, typeof(String), "pregunta", "window.onbeforeunload = function() { return '¿Ha realizado cambios, esta seguro de abandonar la pagina?'; };", true);
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
        if (sender == BAnular)
            query["estado"] = "-1";

        conector.StartTransaction();
        bool existe = HId.Value.Length != 0;

        if (HId.Value.Length == 0)
        {
            query["destino"] = DDestino.SelectedValue;

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

        // agregamos la entrada
        query = new Query("entrada" + DDestino.SelectedValue);

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TLlegada.Text);
        if (sender == BAnular)
            query["estado"] = "-1";

        if (existe)
        {
            entradaId = conector.QueryString("SELECT entrada FROM traslado" + ((ing)Master).GetAlmacen() + " WHERE id=" + HId.Value);
            if (conector.QueryMod(query.GetUpdate("id=" + entradaId)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
        }
        else
        {
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
        

        ProductoUsado productoUsado = ProductoUsado.DesSerialize(HUsado.Value);

        // eliminamos todos los productos
        List<Producto> productos = productoUsado.GetProductoEliminado();
        for (int i = 0; i < productos.Count; i++)
        {
            if (conector.QueryMod(string.Format("DELETE FROM traslado_items{0} WHERE id={1}", ((ing)Master).GetAlmacen(), productos[i].IdTabla)) == -1)
                return;
            if (productos[i].bIngreso)
            {
                if (!productos[i].AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
            }
            if (conector.QueryMod(string.Format("DELETE FROM entrada_items{0} WHERE traslado={1} AND traslado_items={2}", DDestino.SelectedValue, HId.Value, productos[i].IdTabla)) == -1)
                return;
            bool ingresoold = productos[i].bIngreso;
            productos[i].bIngreso = !string.IsNullOrEmpty((string)ViewState["llegada"]);
            if (productos[i].bIngreso)
            {
                if (!productos[i].DeleteProductoFromEstado(conector, DDestino.SelectedValue))
                    return;
            }
            productos[i].bIngreso = ingresoold;
        }

        // agregamos los nuevos
        productos = productoUsado.GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            string salidaid="";

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
            } else {
                productos[i].bIngreso = true;
            }

            if (string.IsNullOrEmpty(productos[i].IdTabla))
                productos[i].GetCostoPromedio(((ing)Master).GetAlmacen());

            query = productos[i].GetQuery("traslado_items" + ((ing)Master).GetAlmacen());
            if (string.IsNullOrEmpty(productos[i].IdTabla))
            {
                query["traslado"] = HId.Value;
                int res= conector.QueryMod(query.GetInsert());
                if (res == -1)
                    return;
                salidaid= res.ToString();
                if (productos[i].bIngreso)
                    if (!productos[i].DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                        return;
            } else if (productos[i].pProductoAntiguo != null)
            { // fue actualizado
                if (conector.QueryMod(query.GetUpdate("id=" + productos[i].IdTabla)) == -1)
                    return;
                if (productos[i].bIngreso)
                    if (!productos[i].DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                        return;
            }

            // establecemos el ingreso de las entradas basados en la fecha de llegada
            productos[i].bIngreso = !string.IsNullOrEmpty((string)ViewState["llegada"]);
            if(productos[i].pProductoAntiguo!=null)
                productos[i].pProductoAntiguo.bIngreso = !string.IsNullOrEmpty((string)ViewState["llegada"]);

            if (sender == BAnular && productos[i].bIngreso)
            {
                bool oldingreso = false;
                if (productos[i].pProductoAntiguo != null)
                {
                    oldingreso = productos[i].pProductoAntiguo.bIngreso;
                    productos[i].pProductoAntiguo.bIngreso = false;
                }

                productos[i].DeleteProductoFromEstado(conector, DDestino.SelectedValue);
                if (productos[i].pProductoAntiguo != null)
                {
                    productos[i].pProductoAntiguo.bIngreso = oldingreso;
                }
            }

            // ahora agregamos la entrada
            productoUsado.UpdateProductoIngreso(productos[i].IdGrid, sender != BAnular && TLlegada.Text.Length > 0); // siempre salen
            query = productos[i].GetQuery("entrada_items" + DDestino.SelectedValue);
            if (string.IsNullOrEmpty(productos[i].IdTabla))
            {
                query["entrada"] = entradaId;
                query["traslado_items"]=salidaid;
                
                int nEntrada = conector.QueryMod(query.GetInsert());
                if (nEntrada == -1)
                    return;
                
                // actualizamos la tabla de traslado_items.
                if (conector.QueryMod("UPDATE traslado_items" + ((ing)Master).GetAlmacen() + " SET entrada_items=" + nEntrada + " WHERE id=" + salidaid) == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }
            else if (productos[i].pProductoAntiguo != null)
            { // fue actualizado
                if (conector.QueryMod(query.GetUpdate("traslado_items=" + productos[i].IdTabla)) == -1)
                    return;
            }
            else
            {
                continue;
            }
            if (productos[i].bIngreso)
            {
                if (!productos[i].AddProductoFromEstado(conector, DDestino.SelectedValue))
                    return;
            }
        }

        conector.Commit();
        Alertas.RemoveAlertas(((ing)Master).GetAlmacen());
        Alertas.RemoveAlertas(DDestino.SelectedValue);
        Session["traslado_clean"] = 1;
        Session["ltraslado"] = HId.Value;

        ((ing)Master).SetResultado("Traslado " + HId.Value + " editada.");
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Session["traslado_clean"]=1;
        Response.Redirect(Request.Url.AbsolutePath);
    }
    
    protected void BActualizar_Click(object sender, EventArgs e)
    {
        ProductoUsado productousadoNuevo = new ProductoUsado(HProductos.Value);
        HProductos.Value = "";
        List<Producto> productos = productousadoNuevo.GetProductosUsados();
        for(int i=0; i<productos.Count; i++) {
            
            Producto producto = productos[i];
            TProductoID.Text= producto.id;
            TCantidad.Text = producto.cantidad.ToString(CultureInfo.InvariantCulture);
            TLote.Text = producto.lote;
            TVencimiento.Text = producto.vence;
            TEstado.Text = producto.szEstado;
            TSerial.Text = producto.serial;
            TProducto.Text = producto.NombreCompleto;
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
    protected void OnImprimir(object sender, EventArgs e)
    {
        GProductos.Columns[0].Visible = false;
    }
}
