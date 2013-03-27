using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;

public partial class almacen_informes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();
        SUbicacion.SelectCommand = String.Format(SUbicacion.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
        SResultados1.SelectCommand = String.Format(SResultados1.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
        SResultados2.SelectCommand = String.Format(SResultados2.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
        SResultados3.SelectCommand = String.Format(SResultados3.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
        SResultados4.SelectCommand = String.Format(SResultados4.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());

        if(!IsPostBack) {
            THasta.Text= DateTime.Now.ToShortDateString();
            TDesde.Text= DateTime.Now.AddDays(-30).ToShortDateString();
            BBuscar_Click(sender, e);
        }
    }
    protected void BBuscar_Click(object sender, EventArgs e)
    {
        GResultados.DataSourceID = "SResultados" + DVer.SelectedValue;
        switch(DVer.SelectedValue) {
            case "1":
                GResultados.Columns[2].Visible = true;
                GResultados.Columns[3].Visible = true;
                GResultados.Columns[4].Visible = true;
                GResultados.Columns[2].HeaderText = "Desde";
                break;
            case "2":
                GResultados.Columns[2].Visible = true;
                GResultados.Columns[3].Visible = true;
                GResultados.Columns[4].Visible = false;
                GResultados.Columns[2].HeaderText = "Hacia";
                break;
            case "3":
                GResultados.Columns[2].Visible = false;
                GResultados.Columns[3].Visible = true;
                GResultados.Columns[4].Visible = true;
                break;
            case "4":
                GResultados.Columns[2].Visible = false;
                GResultados.Columns[3].Visible = false;
                GResultados.Columns[4].Visible = false;
                break;
        }

        if (Utilidades.GetRowsCount(GResultados, LTotal) > 0)
        {

        }
    }
    protected void GResultados_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    protected void GResultados_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1)
        {
            string url = "";
            switch (DVer.SelectedValue)
            {
                case "1":
                    url = "~/almacen/entrada.aspx?ei=";
                    break;
                case "2":
                    url = "~/almacen/traslado.aspx?ti=";
                    break;
                case "3":
                    url = "~/almacen/salida.aspx?si=";
                    break;
                case "4":
                    url = "~/almacen/transformaciones.aspx?tri=";
                    break;
            }
            url+= DataBinder.Eval(e.Row.DataItem, "id").ToString();

            String texto = e.Row.Cells[0].Text;
            e.Row.Cells[0].Text = "";
            HyperLink link = new HyperLink();
            link.Text= texto;
            link.NavigateUrl= url;
            e.Row.Cells[0].Controls.Add(link);

            texto = e.Row.Cells[1].Text;
            e.Row.Cells[1].Text = "";
            link = new HyperLink();
            link.Text = texto;
            link.NavigateUrl = url;
            e.Row.Cells[1].Controls.Add(link);
        }
    }
}
