using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using SDG.Componentes;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string end = Request.QueryString["end"];
            if (end == "1")
            {
                CConector.SetCookie("contrasena","");
                Session.RemoveAll();
                Response.Redirect("~/");
                Response.End();
            }
            CUsuario usuario = (CUsuario)Usuario.Validate(CConector.GetCookie("usuario"), CConector.GetCookie("contrasena"));
            if(usuario!=null) {
                string url = Request.QueryString["from"];
                if (url == null || url.Length == 0)
                    usuario.Redirect();
                else Response.Redirect(url);
                Response.End();
                return;
            }
            m_EUsuario.Text = CConector.GetCookie("usuario");
            if (m_EUsuario.Text.Length == 0)
                m_EUsuario.Focus();
            else m_EContrasena.Focus();

            if (Request.Form["m_EUsuario"] != null && Request.Form["m_EContrasena"] != null)
            {
                m_EUsuario.Text = Request.Form["m_EUsuario"].ToString();
                m_EContrasena.Text = Request.Form["m_EContrasena"].ToString();
                m_BIngresar_Click(sender, e);
            }
        }
    }

    protected void m_BIngresar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        CUsuario usuario = (CUsuario)Usuario.Validate(m_EUsuario.Text, m_EContrasena.Text);
        if(usuario!=null) {

            string url = Request.QueryString["from"];
            if (url == null || url.Length == 0)
                usuario.Redirect();
            else Response.Redirect(url);
            Response.End();
            return;
        }
        m_EContrasena.Text = "";
        m_EUsuario.Focus();
        m_SMensaje.Text = "Usuario o contraseña equivocada.";
        System.Threading.Thread.Sleep(3000);
    }
}
