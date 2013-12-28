using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class almacen_imprimir : System.Web.UI.UserControl
{
    public void Show(string url, string mensaje) {

        PImprimir.Visible = true;
        string script = "$('#dialog-confirm').dialog({resizable: false,modal: true,buttons: { 'Imprimir': function() { window.open('" 
            + url 
            + "', '_blank'); $(this).dialog('close'); }, 'Cancelar' : function() {$(this).dialog('close');}}});";
        ScriptManager.RegisterClientScriptBlock(PImprimir, typeof(string), "imprimir", script, true);
        LImprimir.Text = mensaje;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
