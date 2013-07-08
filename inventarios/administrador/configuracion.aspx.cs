using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;

public partial class administrador_configuracion : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        SAlmaceness.SelectCommand = string.Format(SAlmaceness.SelectCommand, ((CUsuario)Usuario.GetUsuario()).GetDatabase());
        HLogo.NavigateUrl = Formato.GetLogo();

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
            CCrearTerceroEntrada.Checked = configuracion.bCrearTerceroEntradas;
            CDatosTerceroEntrada.Checked = configuracion.bDatosComplementariosTerceroEntrada;

            // salidas
            DEntrega.SelectedValue = configuracion.bManejarEntrega ? "0" : "1";
            CTerceroSalida.Checked = configuracion.bTerceroSalida;
            CDocumentoSalida.Checked = configuracion.bDocumentoSalida;
            CCrearTerceroSalida.Checked = configuracion.bCrearTerceroSalida;
            CDatosTerceroSalida.Checked = configuracion.bDatosComplementariosTerceroSalida;

            string[] formatos = { "Entrada", "Traslado", "Salida", "Estado", "Kardex", "Informes" };

            RFormatos.DataSource = from p in formatos select new { Formato=p};
            RFormatos.DataBind();
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
        configuracion.bCrearTerceroEntradas= CCrearTerceroEntrada.Checked;
        configuracion.bDatosComplementariosTerceroEntrada = CDatosTerceroEntrada.Checked;

        // salidas
        configuracion.bManejarEntrega= DEntrega.SelectedValue=="0";
        configuracion.bTerceroSalida= CTerceroSalida.Checked;
        configuracion.bDocumentoSalida = CDocumentoSalida.Checked;
        configuracion.bCrearTerceroSalida = CCrearTerceroSalida.Checked;
        configuracion.bDatosComplementariosTerceroSalida = CDatosTerceroSalida.Checked;

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
        using (CConector conector = new CConector())
        {
            if (TEntrada.Text.Length > 0)
            {
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
    protected void BLogo_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        if (!FLogo.HasFile)
            return;

        Formato.RemoveLogo();

        if(FLogo.FileName.ToLower().IndexOf(".png")==-1)
            Formato.Guardar(FLogo, "logo.jpg", true);
        else Formato.Guardar(FLogo, "logo.png", true);
        LMensajeGlobal.Text = "Se ha actualizado el logo correctamente.<br/> En algunos navegadores es necesario refrescar la página (Ctrl+F5)";
    }

    protected void BELogo_Click(object sender, EventArgs e)
    {
        Formato.RemoveLogo();
        LMensajeGlobal.Text = "Se ha restaurado el logo correctamente.<br/> En algunos navegadores es necesario refrescar la página (Ctrl+F5)";
    }

    protected void BSubirFormato_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Control b = (Control)sender;
        HyperLink link= (HyperLink)b.FindControl("HFormato");
        FileUpload file = (FileUpload)b.FindControl("FFormato");
        if (!file.HasFile)
            return;
        Formato.Guardar(file, link.Text.ToLower() + ".html", false);
        LMensajeGlobal.Text = "Se ha actualizado el formato de " + link.Text + " correctamente.";
    }

    protected void BEliminarFormato_Click(object sender, EventArgs e)
    {
        Control b = (Control)sender;
        HyperLink link = (HyperLink)b.FindControl("HFormato");

        Formato.Remove(link.Text.ToLower() + ".html");
        LMensajeGlobal.Text = "Se ha restaurado el formato " + link.Text + "correctamente.";
    }
}
