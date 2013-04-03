using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class administrador_configuracion : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack) {
            CConfiguracion configuracion = CConfiguracion.GetConfiguracion();
            DCostos.SelectedValue = configuracion.nCosto.ToString();
            DCantidades.SelectedValue= configuracion.nCantidad.ToString();
            DVence.SelectedValue = configuracion.nVence.ToString();
            DLote.SelectedValue = configuracion.nLote.ToString();
            DSerial.SelectedValue = configuracion.nSerial.ToString();
            DEstado.SelectedValue = configuracion.nEstado.ToString();
            DUbicacion.SelectedValue = configuracion.nUbicacion.ToString();
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

        configuracion.nCosto= Convert.ToInt32(DCostos.SelectedValue);
        configuracion.nCantidad= Convert.ToInt32(DCantidades.SelectedValue);
        configuracion.nVence= Convert.ToInt32(DVence.SelectedValue);
        configuracion.nLote= Convert.ToInt32(DLote.SelectedValue);
        configuracion.nSerial= Convert.ToInt32(DSerial.SelectedValue);
        configuracion.nEstado= Convert.ToInt32(DEstado.SelectedValue);
        configuracion.nUbicacion = Convert.ToInt32(DUbicacion.SelectedValue);

        LMensaje.Text = configuracion.SaveConfiguracion() ? "Configuración guardada" : "Error guardando la configuración";
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
}
