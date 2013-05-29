using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MySql.Data.MySqlClient;
using SDG.Componentes;
using SDG.Web.UI.WebControls;

public class CConector : Conector
{
    public CConector()
        : base(ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString)
    {
        m_szConexion= string.Format(m_szConexion, ((CUsuario)Usuario.GetUsuario()).GetDatabase());
    }

    public CConector(string szDb)
        : base(ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString)
    {
        m_szConexion = string.Format(m_szConexion, szDb);
    }

    static public void ReplaceConnectionString(ControlCollection controles)
    {
        ReplaceConnectionString(controles, (CUsuario)Usuario.GetUsuario());
    }

    static public void ReplaceConnectionString(ControlCollection controles, CUsuario usuario) {

        foreach (Control control in controles)
        {
            if(control is SqlDataSource) {
                SqlDataSource sql= (SqlDataSource)control;
                sql.ConnectionString= string.Format(sql.ConnectionString, usuario.GetDatabase());
            } else if(control is CustomDataSource) {
                CustomDataSource sql = (CustomDataSource)control;
                sql.ConnectionString = string.Format(sql.ConnectionString, usuario.GetDatabase());
            }
            if (control.HasControls())
                ReplaceConnectionString(control.Controls, usuario);
        }
    }
}
