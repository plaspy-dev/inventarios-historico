using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class administrador_terceros : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TNombre.Focus();
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
        Query query = new Query("tercero");
        CConector conector = CConector.GetConector();
        string szQuery;
        string mensaje;

        conector.StartTransaction();

        query["nombre"] = TNombre.Text;
        query["descripcion"] = TDescripcion.Text;
        query["tipo"] = DTipo.SelectedValue;
        query["identificacion"] = TIdentificacion.Text;

        if (HId.Value.Length == 0)
        {
            szQuery = query.GetInsert();
            mensaje = TNombre.Text + " ingresado";
        }
        else
        {
            szQuery = query.GetUpdate("id='" + HId.Value + "'");
            mensaje = TNombre.Text + " editado";
        }
        int res = conector.QueryMod(szQuery, true);
        if (res == -1)
            return;
        if (HId.Value.Length == 0)
            HId.Value = res.ToString();

        if (conector.InsertModificacion(HId.Value) == -1)
            return;

        conector.Commit();

        Almacen.RemoveTercero(HId.Value);

        ((ing)Master).SetResultado(mensaje);
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        HId.Value = GResultado.SelectedDataKey.Value.ToString();
        TNombre.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[2].Text).Trim();
        TDescripcion.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[3].Text).Trim();
        TIdentificacion.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[4].Text).Trim();
        Conector.SelectInComboText(DTipo, HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[5].Text).Trim());

        ((ing)Master).SetModificacion(HId.Value);
    }
}
