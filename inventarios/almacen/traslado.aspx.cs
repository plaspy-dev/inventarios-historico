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
                    ViewState["llegada"]= TLlegada.Text = CConector.GetFromMySQLDate(reader["llegada"]);
                    TDocumento.Text = reader["documento"].ToString();
                    DAlmacen.DataBind();
                    DAlmacen.SelectedValue = reader["destino"].ToString();

                    HEntrada.Text = "Entrada No. " + reader["entrada"].ToString() + " hacia " + DAlmacen.SelectedItem.Text;
                    HEntrada.NavigateUrl = "~/almacen/entrada.aspx?ei=" + reader["entrada"].ToString() + "&alm=" + DAlmacen.SelectedValue;


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
                            info.Ingreso = reader["ingreso"].ToString() != "";

                            productoUsado.AddProducto(info);
                        }
                    }

                    GProductos.DataSource = productoUsado.GetProductosUsados();
                    HUsado.Value = productoUsado.SerializeBinary();
                    GProductos.DataBind();

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
        LEstado.CssClass = TEstado.CssClass = producto.nEstado > 0 ? "" : "hide";
        LSerial.CssClass = TSerial.CssClass = producto.nSerial > 0 ? "" : "hide";
        
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

        LVencimiento.CssClass = TVencimiento.CssClass = CAutomatico.Checked ? "hide" : "";
        LLote.CssClass = TLote.CssClass = CAutomatico.Checked ? "hide" : "";
        LSerial.CssClass = TSerial.CssClass = CAutomatico.Checked ? "hide" : "";
        LEstado.CssClass = TEstado.CssClass = CAutomatico.Checked ? "hide" : "";
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

        // agregamos la entrada
        query = new Query("entrada" + DAlmacen.SelectedValue);

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TLlegada.Text);

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
            if (productos[i].Ingreso)
            {
                if (!productos[i].AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
            }
            if (conector.QueryMod(string.Format("DELETE FROM entrada_items{0} WHERE traslado={1} AND traslado_items={2}", DAlmacen.SelectedValue, HId.Value, productos[i].IdTabla)) == -1)
                return;
            bool ingresoold = productos[i].Ingreso;
            productos[i].Ingreso = !string.IsNullOrEmpty((string)ViewState["llegada"]);
            if (productos[i].Ingreso)
            {
                if (!productos[i].DeleteProductoFromEstado(conector, DAlmacen.SelectedValue))
                    return;
                if (!productos[i].UpdateCosto(conector, DAlmacen.SelectedValue, "-1"))
                    return;
            }
            productos[i].Ingreso = ingresoold;
        }

        // agregamos los nuevos
        productos = productoUsado.GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            string salidaid="";

            productos[i].Ingreso = true;
            query = productos[i].GetQuery("traslado_items" + ((ing)Master).GetAlmacen());
            if (string.IsNullOrEmpty(productos[i].IdTabla))
            {
                query["traslado"] = HId.Value;
                int res= conector.QueryMod(query.GetInsert());
                if (res == -1)
                    return;
                salidaid= res.ToString();
                if (!productos[i].DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
            }
            else if (productos[i].pProductoAntiguo != null)
            { // fue actualizado
                if (conector.QueryMod(query.GetUpdate("id=" + productos[i].IdTabla)) == -1)
                    return;
                if (!productos[i].DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                    return;
            }

            // establecemos el ingreso de las entradas basados en la fecha de llegada
            productos[i].Ingreso= !string.IsNullOrEmpty((string)ViewState["llegada"]);
            if(productos[i].pProductoAntiguo!=null)
                productos[i].pProductoAntiguo.Ingreso = !string.IsNullOrEmpty((string)ViewState["llegada"]);

            // ahora agregamos la entrada
            productoUsado.UpdateProductoIngreso(productos[i].IdGrid, TLlegada.Text.Length>0); // siempre salen
            query = productos[i].GetQuery("entrada_items" + DAlmacen.SelectedValue);
            if (string.IsNullOrEmpty(productos[i].IdTabla))
            {
                query["entrada"] = entradaId;
                query["traslado_items"]=salidaid;
                query["costo"] = productos[i].GetCostoPromedio(((ing)Master).GetAlmacen()).ToString(CultureInfo.InvariantCulture);
                
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
            if (productos[i].Ingreso)
            {
                if (!productos[i].AddProductoFromEstado(conector, DAlmacen.SelectedValue))
                    return;
                if (!productos[i].UpdateCosto(conector, DAlmacen.SelectedValue, ""))
                    return;
            }
        }

        conector.Commit();
        Session["traslado_clean"] = 1;
        ((ing)Master).SetAlerta("Traslado " + HId.Value + " editada.");
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
}
