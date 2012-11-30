using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MySql.Data.MySqlClient;
using SDG.Componentes;

public class CConector : Conector
{
    public CConector()
        : base(ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString)
    {
    }

    static public CConector GetConector()
    {
        return new CConector();
    }
}
