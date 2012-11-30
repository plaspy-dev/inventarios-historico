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
            TNumero.Focus();
            Producto.Clean();
            ViewState["almacen"] = ((ing)Master).GetAlmacen();
            SAlmacen.SelectCommand = SAlmacen.SelectCommand.Replace("@almacen", ((ing)Master).GetAlmacen());
            if (((ing)Master).GetAlmacen() == "0" && DAlmacen.Items.Count>0 && DAlmacen.Items[0].Value=="0")
            {
                DAlmacen.Items.RemoveAt(0);
            }

            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
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

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["ti"])));
                if (reader != null && reader.Read())
                {
                    HId.Value = TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetModificacion(HId.Value);
                    TObservaciones.Text = reader["observaciones"].ToString();
                    TFecha.Text = CConector.GetFromMySQLDate(reader["envio"]);
                    TLlegada.Text = CConector.GetFromMySQLDate(reader["llegada"]);
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
        Page.Validate("producto");
        if (!Page.IsValid)
            return;

        ArrayList productos;

        string []producto= TProductoID.Text.Split(';');

        if(producto.Length==1) { // automatico
            productos = Producto.GetProductos(producto[0], Convert.ToDouble(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen());
        } else { // manual
            productos = Producto.GetProductos(producto[0], Convert.ToDouble(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen(), producto[1], producto[2]);
        }
        if (productos == null)
        {
            LMensaje.Text = "No se encontraron las cantidades disponibles.";
            return;
        }

        for(int i=0; i<productos.Count; i++) {

            IOrderedDictionary data = new OrderedDictionary();

            data.Add("nombre", TProducto.Text);
            data.Add("producto", ((Producto)productos[i]).producto);
            data.Add("cantidad", ((Producto)productos[i]).cantidad);
            data.Add("lote", ((Producto)productos[i]).lote);
            data.Add("vence", Convert.ToDateTime(((Producto)productos[i]).vence));

            /*if (sender != BAgregar)
                data.Add("id", TProcedimientoID.Text);*/

            SProductos.GetView().Insert(data, OnDataSourceOperationComplete);
        }

        
        GProductos.SelectedIndex = -1;
        GProductos.DataBind();

        LMensaje.Text = "";
        TProducto.Text = TProductoID.Text = TCantidad.Text = TLote.Text = TVencimiento.Text = "";
        TProducto.Focus();
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

        Query query = new Query("traslado" + ((ing)Master).GetAlmacen());
        CConector conector = new CConector();
        string entradaId = "";

        query["observaciones"] = TObservaciones.Text;
        query["envio"] = CConector.GetMySQLDateFormat(TFecha.Text);
        query["llegada"] = CConector.GetMySQLDateFormat(TLlegada.Text);

        conector.StartTransaction();
        bool existe = HId.Value.Length != 0;

        if (HId.Value.Length == 0)
        {
            query["destino"] = DAlmacen.SelectedValue;

            int res = conector.QueryMod(query.GetInsert(), true);
            if (res == -1)
                return;
            HId.Value = res.ToString();
        }
        else
        {
            if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(HId.Value))) == -1)
                return;
        }

        if (conector.InsertModificacion(HId.Value) == -1)
            return;

        string[] productos = SProductos.GetQuerys();
        for (int i = 0; i < productos.Length; i++)
        {
            if (conector.QueryMod(string.Format(productos[i], ((ing)Master).GetAlmacen()).Replace("@rtraslado", HId.Value)) == -1)
                return;
        }

        // agregamos la entrada

        query = new Query("entrada" + DAlmacen.SelectedValue);

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TLlegada.Text);

        if(existe) {
            entradaId = conector.QueryString("SELECT entrada FROM traslado" + ((ing)Master).GetAlmacen() + " WHERE id=" + HId.Value);
            if (conector.QueryMod(query.GetUpdate("id=" + entradaId)) == -1)
                return;

        } else {
            
            query["desde"] = ((ing)Master).GetAlmacen();
            query["traslado"] = HId.Value;

            int nEntrada = conector.QueryMod(query.GetInsert(), true);
            if (nEntrada == -1)
                return;
            entradaId = nEntrada.ToString();
            conector.QueryMod("UPDATE traslado" + ((ing)Master).GetAlmacen() + " SET entrada=" + entradaId + " WHERE id=" + HId.Value);
        }

        // agregamos los elmentos de la entrada
        ArrayList ids = new ArrayList();
        MySqlDataReader reader = conector.Query(string.Format("SELECT traslado_items0.id FROM traslado_items0 "
                                                        + " LEFT JOIN entrada_items1 ON (entrada_items1.id=traslado_items0.entrada_items) "
                                                        + " WHERE traslado={2} AND entrada_items1.id IS NULL;", ((ing)Master).GetAlmacen(), DAlmacen.SelectedValue, HId.Value));
        if (reader != null)
            while (reader.Read())
                ids.Add(reader["id"].ToString());

        for (int i = 0; i < ids.Count; i++ ) {
            int nEntrada= conector.QueryMod("INSERT INTO entrada_items" + DAlmacen.SelectedValue + " (entrada,producto,cantidad,lote,vence,traslado_items) SELECT " + entradaId + ",producto,cantidad,lote,vence,id FROM traslado_items" + ((ing)Master).GetAlmacen() + " WHERE id=" + ids[i], true);
            if (nEntrada == -1)
                return;
            if (conector.QueryMod("UPDATE traslado_items" + ((ing)Master).GetAlmacen() + " SET entrada_items=" + nEntrada + " WHERE id=" + ids[i]) == -1)
                return;
        }

        if (conector.QueryMod(string.Format("DELETE entrada_items{0} FROM entrada_items{0} LEFT JOIN traslado_items{1} ON (entrada_items{0}.traslado_items=traslado_items{1}.id) WHERE entrada_items{0}.entrada=" + entradaId + " AND traslado_items{1}.id IS NULL;", DAlmacen.SelectedValue, ((ing)Master).GetAlmacen())) == -1)
            return;

        if(TLlegada.Text.Length>0)
            if (conector.QueryMod("UPDATE entrada_items" + DAlmacen.SelectedValue + " SET ingreso=SYSDATE() WHERE entrada=" + entradaId + " AND ingreso IS NULL") == -1)
                return;

        conector.Commit();
        ((ing)Master).SetResultado("Traslado " + HId.Value + " editada.");
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
    protected void GProductos_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        Producto.DeleteProducto(e.Values["producto"].ToString(), Convert.ToDouble(e.Values["cantidad"]), e.Values["lote"].ToString(), e.Values["vence"].ToString());
    }
}
