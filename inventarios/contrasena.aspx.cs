using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class contrasena : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        if(!IsPostBack) {
        }
    }
    protected void m_BCambiar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Conector conector = new Conector(ConfigurationManager.ConnectionStrings["ConexionUsuario"].ConnectionString);
        Usuario usuario = Usuario.GetUsuario();
        Query query = new Query("usuarios");

        String id=conector.QueryString("SELECT id FROM usuarios WHERE id='" + usuario.GetID() + "' AND contrasena=PASSWORD("+ Query.GetValor(m_EActual.Text) + ")");
        if(id.Length==0) {

            m_EActual.Focus();
            LMensaje.Text = "Contraseña actual incorrecta.";
            System.Threading.Thread.Sleep(3000);
        } else {
            query.AddCampo("contrasena", "PASSWORD(" + Query.GetValor(m_EConfirmar.Text) + ")", false);
            if (conector.QueryMod(query.GetUpdate("id=" + usuario.GetID())) == -1)
            {
                LMensaje.Text = conector.GetLastError();
            }
            else
            {
                LMensaje.Text = "Contraseña cambiada exitosamente.";
            }
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/inicio.aspx");
    }
}
