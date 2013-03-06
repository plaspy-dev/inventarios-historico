using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class error_error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        CConector conector = new CConector();
        conector = null;
        conector.Query("SELECT ALGO");
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        CConector conector = new CConector();
        conector.Query("SELECT ALGO FROM algo");
    }
}
