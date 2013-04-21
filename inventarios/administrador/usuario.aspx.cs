using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Configuration;

public partial class administrador_usuario : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        TNombres.Focus();
        if(!IsPostBack) {
            CUsuario usuario = (CUsuario) Usuario.GetUsuario();

            BAceptar.Enabled = usuario.CanInsert();
            GUsuarios.Columns[1].Visible = usuario.CanDelete();
            GUsuarios.Columns[0].Visible = usuario.CanUpdate();
            ViewState["Almacenes"] = "";
        }
    }
    protected void GUsuarios_SelectedIndexChanged(object sender, EventArgs e)
    {
        RContrasena.Enabled = false;
        HId.Value = GUsuarios.SelectedDataKey.Value.ToString();
        CConector.SelectInComboText(DGrupos, HttpUtility.HtmlDecode(GUsuarios.SelectedRow.Cells[4].Text));
        TNombres.Text = HttpUtility.HtmlDecode(GUsuarios.SelectedRow.Cells[2].Text);
        TUsuario.Text = HttpUtility.HtmlDecode(GUsuarios.SelectedRow.Cells[3].Text);
        ViewState["Almacenes"] = new CConector().QueryString("SELECT almacenes FROM usuarios WHERE id=" + HId.Value);
        GAlmacenes.DataBind();

        ((ing)Master).SetModificacion(HId.Value);
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("usuarios");
        CConector conector = new CConector();
        string szQuery;
        string mensaje;
        conector.StartTransaction();

        query["nombre"] = TNombres.Text;
        query["usuario"] = TUsuario.Text;
        query["grupo"] = DGrupos.SelectedValue;

        string almacenes = "";
        // agregamos los almacenes que puede programar el usuario
        for (int i = 0; i < GAlmacenes.Rows.Count; i++)
        {
            CheckBox box = (CheckBox)GAlmacenes.Rows[i].FindControl("CAlmacen");
            if (box.Checked)
            {
                if (GAlmacenes.DataKeys[i].Value.ToString()=="-2") { // todos los almacenes
                    almacenes= GAlmacenes.DataKeys[i].Value.ToString();
                    break;
                }
                if (almacenes.Length > 0)
                    almacenes += ",";
                almacenes += GAlmacenes.DataKeys[i].Value.ToString();
            }
        }
        query["almacenes"] = almacenes;

        if(TContrasena.Text.Length>0)
            query.AddCampo("contrasena", "PASSWORD(" + Query.GetValor(TContrasena.Text) + ")", false);

        if (HId.Value.Length == 0)
        {
            szQuery = query.GetInsert();
            mensaje = TNombres.Text + " ingresado";
        }
        else
        {
            szQuery = query.GetUpdate("id='" + HId.Value + "'");
            mensaje = TNombres.Text + " editado";
        }
        int res = conector.QueryMod(szQuery, true);
        if ( res== -1)
            return;
        if (HId.Value.Length == 0)
            HId.Value = res.ToString();
        if (conector.InsertModificacion(HId.Value) == -1)
            return;

        if (conector.QueryMod("DELETE FROM alertas_usuario WHERE usuario=" + HId.Value) == -1)
            return;

        query = new Query("alertas_usuario");
        query["usuario"] = HId.Value;

        for (int i = 0; i < GAlertas.Rows.Count; i++)
        {
            CheckBox check = (CheckBox)GAlertas.Rows[i].FindControl("CAlerta");
            if (check.Checked)
            {
                if (GAlertas.DataKeys[i].Values["alerta"].ToString().Length==0) { // el grupo originalmente no tenia esto
                    query["alerta"] = GAlertas.DataKeys[i].Values["id"].ToString();
                    query["estado"] = "1";
                    if (conector.QueryMod(query.GetInsert()) == -1)
                        return;
                }
            } else if (GAlertas.DataKeys[i].Values["alerta"].ToString().Length> 0)
            { // el grupo originalmente lo tiene
                query["alerta"] = GAlertas.DataKeys[i].Values["id"].ToString();
                query["estado"] = "-1";
                if (conector.QueryMod(query.GetInsert()) == -1)
                    return;
            }
        }

        conector.Commit();
        Session["alertas"] = null;
        ((ing)Master).SetResultado(mensaje);
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
    
    protected void GUsuarios_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
    }

    protected void GAlmacenes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1)
        {
            string[] valores = ViewState["Almacenes"].ToString().Split(',');
            for (int i = 0; i < valores.Length; i++)
            {
                if (valores[i]=="-2" || valores[i] == GAlmacenes.DataKeys[e.Row.RowIndex].Value.ToString())
                {
                    CheckBox box = (CheckBox)e.Row.FindControl("CAlmacen");
                    box.Checked = true;
                    break;
                }
            }
        }
    }
}
