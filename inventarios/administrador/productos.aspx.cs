using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using SDG.Componentes;

public partial class administrador_productos : System.Web.UI.Page
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

            if(!string.IsNullOrEmpty(Request.QueryString["pid"])) {

                Query query = new Query("producto");
                CConector conector = CConector.GetConector();

                query+="nombre";
                query+="descripcion";
                query+="codigo";
                query+="unidad";
                query+="vence";
                query += "lote";
                query += "id";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["pid"])));
                if(reader!=null && reader.Read()) {

                    HId.Value= reader["id"].ToString();
                    TNombre.Text= reader["nombre"].ToString();
                    TDescripcion.Text = reader["descripcion"].ToString();
                    TCodigo.Text = reader["codigo"].ToString();
                    TUnidad.Text = reader["unidad"].ToString();
                    DVence.SelectedValue = reader["vence"].ToString();
                    DLote.SelectedValue = reader["lote"].ToString();
                }
            }
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("producto");
        CConector conector = CConector.GetConector();
        string szQuery;
        string mensaje;

        query["nombre"] = TNombre.Text;
        query["descripcion"] = TDescripcion.Text;
        query["codigo"] = TCodigo.Text;
        query["unidad"] = TUnidad.Text;
        query["vence"] = DVence.SelectedValue;
        query["lote"] = DLote.SelectedValue;

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
        TCodigo.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[3].Text).Trim();
        TUnidad.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[4].Text).Trim();
        TDescripcion.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[5].Text).Trim();
        CConector.SelectInComboText(DVence, HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[6].Text).Trim());
        CConector.SelectInComboText(DLote, HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[7].Text).Trim());

        ((ing)Master).SetModificacion(HId.Value);
    }
}
