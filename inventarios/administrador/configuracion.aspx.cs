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
            DSeleccionProducto.SelectedValue = configuracion.nSeleccionProducto.ToString();


            DVence.SelectedValue = configuracion.nProductoVence.ToString();
            DLote.SelectedValue = configuracion.nProductoLote.ToString();
            DSerial.SelectedValue = configuracion.nProductoSerial.ToString();
            DEstado.SelectedValue = configuracion.nProductoEstado.ToString();

            TDiasVencimiento.Text = configuracion.nDiasVencimiento.ToString();

            CVencimiento.Checked = configuracion.bVencimiento;
            CLote.Checked = configuracion.bLote;
            CSerial.Checked = configuracion.bSerial;
            CEstado.Checked = configuracion.bEstado;

            // entradas
            CTerceroEntrada.Checked = configuracion.bTerceroEntrada;
            CDocumentoEntrada.Checked = configuracion.bDocumentoEntrada;

            // salidas
            DEntrega.SelectedValue = configuracion.bManejarEntrega ? "0" : "1";
            CTerceroSalida.Checked = configuracion.bTerceroSalida;
            CDocumentoSalida.Checked = configuracion.bDocumentoSalida;
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
        configuracion.nSeleccionProducto = Convert.ToInt32(DSeleccionProducto.SelectedValue);

        configuracion.bVencimiento= CVencimiento.Checked;
        configuracion.bLote= CLote.Checked;
        configuracion.bSerial= CSerial.Checked;
        configuracion.bEstado= CEstado.Checked;

        // entradas
        configuracion.bTerceroEntrada= CTerceroEntrada.Checked;
        configuracion.bDocumentoEntrada = CDocumentoEntrada.Checked;

        // salidas
        configuracion.bManejarEntrega= DEntrega.SelectedValue=="0";
        configuracion.bTerceroSalida= CTerceroSalida.Checked;
        configuracion.bDocumentoSalida = CDocumentoSalida.Checked;

        if(configuracion.SaveConfiguracion()) {
            LMensaje.Text =  "Configuración guardada";
            Alertas.RemoveAlertas(((ing)Master).GetAlmacen());
        } else LMensaje.Text =  "Error guardando la configuración";
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void GAlmacenes_SelectedIndexChanged(object sender, EventArgs e)
    {
        PAlmacen.Visible = true;
        TEntrada.Text= GAlmacenes.SelectedRow.Cells[2].Text;
        TTraslado.Text = GAlmacenes.SelectedRow.Cells[3].Text;
        TSalida.Text = GAlmacenes.SelectedRow.Cells[4].Text;
    }

    protected void BEditar_Click(object sender, EventArgs e)
    {
        CConector conector= new CConector();
        if(TEntrada.Text.Length>0) {
            if (conector.QueryMod(string.Format("ALTER TABLE entrada{0} AUTO_INCREMENT={1}", GAlmacenes.SelectedValue, TEntrada.Text)) == -1)
                return;
        }
        if (TTraslado.Text.Length > 0)
        {
            if (conector.QueryMod(string.Format("ALTER TABLE traslado{0} AUTO_INCREMENT={1}", GAlmacenes.SelectedValue, TTraslado.Text)) == -1)
                return;
        }
        if (TSalida.Text.Length > 0)
        {
            if (conector.QueryMod(string.Format("ALTER TABLE salida{0} AUTO_INCREMENT={1}", GAlmacenes.SelectedValue, TSalida.Text)) == -1)
                return;
        }
        GAlmacenes.DataBind();
        BCancelarEdicion_Click(sender, e);
    }

    protected void BCancelarEdicion_Click(object sender, EventArgs e)
    {
        GAlmacenes.SelectedIndex = -1;
        TTraslado.Text= TSalida.Text= TEntrada.Text = "";
        PAlmacen.Visible = false;
    }
}
