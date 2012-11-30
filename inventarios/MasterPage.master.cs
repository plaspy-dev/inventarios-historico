using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            /*ArrayList values = new ArrayList();
            values.Add(new Menu("~/Default.aspx", "Ingreso"));
            values.Add(new Menu("~/nuevo.aspx", "Nuevo usuario"));
            values.Add(new Menu("~/recordar.aspx", "Recordar contraseña"));
            values.Add(new Menu("~/hardware.aspx", "Dispositivos"));
            values.Add(new Menu("~/faq.aspx", "Preguntas frecuentes"));

            RMenu.DataSource = values;
            RMenu.DataBind();*/
            LPagina.Text = Page.Title;
        }
    }
}
