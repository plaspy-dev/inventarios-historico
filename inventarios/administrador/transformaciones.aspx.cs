using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;

public partial class administrador_transformaciones : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TProducto.Focus();
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)Usuario.GetUsuario();

            BAceptar.Enabled = usuario.CanInsert();
            GResultado.Columns[1].Visible = usuario.CanDelete();
            GResultado.Columns[0].Visible = usuario.CanUpdate();
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("transformacion");
        CConector conector = CConector.GetConector();
        string szQuery;
        string mensaje;

        query["producto1"] = TProductoID.Text;
        query["producto2"] = TProductoID2.Text;
        query["proporcion"] = TProporcion.Text;

        if (HId.Value.Length == 0)
        {
            szQuery = query.GetInsert();
            HId.Value = TProductoID.Text;
            HId2.Value = TProductoID2.Text;
        }
        else
        {
            szQuery = query.GetUpdate("producto1=" + Query.GetValor(HId.Value) + " AND producto2=" + Query.GetValor(HId2.Value));
        }
        mensaje = TProducto.Text + " - " + TProducto2.Text + " editada";

        int res = conector.QueryMod(szQuery);
        if (res == -1)
            return;
        if (conector.InsertModificacion(HId.Value, HId2.Value) == -1)
            return;
        ((ing)Master).SetResultado(mensaje);
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        HId.Value = GResultado.SelectedDataKey.Values[0].ToString();
        HId2.Value = GResultado.SelectedDataKey.Values[1].ToString();

        TProducto.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[2].Text).Trim();
        TProducto2.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[3].Text).Trim();
        TProporcion.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[4].Text).Trim();
        TProductoID.Text = HId.Value;
        TProductoID2.Text = HId2.Value;

        ((ing)Master).SetModificacion(HId.Value, HId2.Value);
    }
}
