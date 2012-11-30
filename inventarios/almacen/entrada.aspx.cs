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

        if(!IsPostBack) {

            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            if(!string.IsNullOrEmpty(Request.QueryString["ei"])) {

                Query query = new Query("entrada" + ((ing)Master).GetAlmacen());
                CConector conector = new CConector();

                query += "id";
                query+= "observaciones";
                query+="fecha";
                query += "desde";
                query += "traslado";

                MySqlDataReader reader= conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["ei"])));
                if(reader!=null && reader.Read()) {

                    HId.Value = TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetModificacion(HId.Value);
                    TObservaciones.Text = reader["observaciones"].ToString();
                    TFecha.Text = CConector.GetFromMySQLDate(reader["fecha"]);

                    bool traslado = false;
                    if (!string.IsNullOrEmpty(reader["traslado"].ToString())) {

                        traslado = true;
                        HTraslado.Text = "Traslado No. " + reader["traslado"].ToString() + " desde ";
                        HTraslado.NavigateUrl = "~/almacen/traslado.aspx?ti=" + reader["traslado"].ToString() + "&alm=" +  reader["desde"].ToString();
                        if (reader["desde"].ToString() == "0")
                            HTraslado.Text += "(Ajustes)";
                        else HTraslado.Text += conector.QueryString("SELECT nombre FROM almacen WHERE id=" + reader["desde"].ToString());
                    }

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
        TProducto.Text= HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[2].Text).Trim();
        TCantidad.Text= Convert.ToDouble(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[3].Text).Trim()).ToString(CultureInfo.InvariantCulture);
        TLote.Text= HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[4].Text).Trim();
        TVencimiento.Text= HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[5].Text).Trim();
        TProductoID.Text = HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[6].Text).Trim();
    }

    protected void BAgregar_Click(object sender, EventArgs e)
    {
        Page.Validate("producto");
        if (!Page.IsValid)
            return;

        IOrderedDictionary data = new OrderedDictionary();
        data.Add("nombre", TProducto.Text);
        data.Add("producto", TProductoID.Text);
        data.Add("cantidad", Convert.ToDouble(TCantidad.Text, CultureInfo.InvariantCulture));
        data.Add("lote", TLote.Text);
        data.Add("vence", Convert.ToDateTime(TVencimiento.Text));

        /*if (sender != BAgregar)
            data.Add("id", TProcedimientoID.Text);*/

        if (GProductos.SelectedIndex == -1)
        {
            SProductos.GetView().Insert(data, OnDataSourceOperationComplete);
        }
        else
        {
            data.Add("id", GProductos.DataKeys[GProductos.SelectedIndex].Value);

            IOrderedDictionary dataold = new OrderedDictionary();

            dataold.Add("nombre", HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[2].Text).Trim());
            dataold.Add("cantidad", Convert.ToDouble(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[3].Text).Trim()));
            dataold.Add("lote", HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[4].Text).Trim());
            dataold.Add("vence", Convert.ToDateTime(HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[5].Text).Trim()));
            dataold.Add("producto", HttpUtility.HtmlDecode(GProductos.SelectedRow.Cells[6].Text).Trim());

            dataold.Add("id", GProductos.DataKeys[GProductos.SelectedIndex].Value);

            SProductos.GetView().Update(data, data, dataold, OnDataSourceOperationComplete);
        }
        GProductos.SelectedIndex = -1;
        GProductos.DataBind();

        TProducto.Text = TProductoID.Text = TCantidad.Text= TLote.Text = TVencimiento.Text = "";
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

        Query query = new Query("entrada" + ((ing)Master).GetAlmacen());
        CConector conector = new CConector();

        query["observaciones"] = TObservaciones.Text;
        query["fecha"] = CConector.GetMySQLDateFormat(TFecha.Text);

        conector.StartTransaction();

        bool existe = HId.Value.Length>0;

        if (HId.Value.Length == 0)
        {
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
            if (conector.QueryMod(string.Format(productos[i], ((ing)Master).GetAlmacen()).Replace("@rentrada", HId.Value)) == -1)
                return;
        }

        if(existe) {

            MySqlDataReader reader = conector.Query("SELECT desde,traslado FROM entrada" + ((ing)Master).GetAlmacen() + " WHERE id=" + HId.Value);
            if (reader != null && reader.Read())
            {
                string almacendesde = reader["desde"].ToString();
                string trasladoId = reader["traslado"].ToString();

                if (almacendesde.Length > 0 && trasladoId.Length > 0)
                    if (conector.QueryMod(string.Format("DELETE traslado_items{0} FROM traslado_items{0} LEFT JOIN entrada_items{1} ON (entrada_items{1}.id=traslado_items{0}.entrada_items) WHERE traslado_items{0}.traslado=" + trasladoId + " AND entrada_items{1}.id IS NULL;", almacendesde, ((ing)Master).GetAlmacen())) == -1)
                        return;

            }
        }

        if(TFecha.Text.Length>0)
            if (conector.QueryMod("UPDATE entrada_items" + ((ing)Master).GetAlmacen() + " SET ingreso=SYSDATE() WHERE entrada=" + HId.Value + " AND ingreso IS NULL") == -1)
                return;

        conector.Commit();
        ((ing)Master).SetResultado("Entrada " + HId.Value + " editada.");
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
}
