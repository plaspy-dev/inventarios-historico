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
            DValores.SelectedValue = configuracion.nValor.ToString();
            DTipoValores.SelectedValue = configuracion.nValorTipo.ToString();
            DCantidades.SelectedValue= configuracion.nCantidad.ToString();
            DUbicacion.SelectedValue = configuracion.nUbicacion.ToString();

            DVence.SelectedValue = configuracion.nProductoVence.ToString();
            DLote.SelectedValue = configuracion.nProductoLote.ToString();
            DSerial.SelectedValue = configuracion.nProductoSerial.ToString();
            DEstado.SelectedValue = configuracion.nProductoEstado.ToString();

            TDiasVencimiento.Text = configuracion.nDiasVencimiento.ToString();

            CVencimiento.Checked = configuracion.bVencimiento;
            CLote.Checked = configuracion.bLote;
            CSerial.Checked = configuracion.bSerial;
            CEstado.Checked = configuracion.bEstado;

            // salidas
            DEntrega.SelectedValue = configuracion.bManejarEntrega ? "0" : "1";
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

        configuracion.nCosto= Convert.ToInt32(DCostos.SelectedValue);
        configuracion.nValor = Convert.ToInt32(DValores.SelectedValue);
        configuracion.nValorTipo= Convert.ToInt32(DTipoValores.SelectedValue);
        configuracion.nCantidad= Convert.ToInt32(DCantidades.SelectedValue);
        configuracion.nProductoVence = Convert.ToInt32(DVence.SelectedValue);
        configuracion.nProductoLote = Convert.ToInt32(DLote.SelectedValue);
        configuracion.nProductoSerial = Convert.ToInt32(DSerial.SelectedValue);
        configuracion.nProductoEstado = Convert.ToInt32(DEstado.SelectedValue);
        configuracion.nUbicacion = Convert.ToInt32(DUbicacion.SelectedValue);
        configuracion.nDiasVencimiento = Convert.ToInt32(TDiasVencimiento.Text);

        configuracion.bVencimiento= CVencimiento.Checked;
        configuracion.bLote= CLote.Checked;
        configuracion.bSerial= CSerial.Checked;
        configuracion.bEstado= CEstado.Checked;

        // salidas
        configuracion.bManejarEntrega= DEntrega.SelectedValue=="0";

        if(configuracion.SaveConfiguracion()) {
            LMensaje.Text =  "Configuración guardada";
            Alertas.RemoveAlertas(((ing)Master).GetAlmacen());
        } else LMensaje.Text =  "Error guardando la configuración";
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
}
