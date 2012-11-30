using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class almacen_estado : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();

        SResultados.SelectCommand = String.Format(SResultados.SelectCommand, ((ing)Master).GetAlmacen());
        SResultados1.SelectCommand = String.Format(SResultados1.SelectCommand, ((ing)Master).GetAlmacen());
        SKardex.SelectCommand = String.Format(SKardex.SelectCommand, ((ing)Master).GetAlmacen());
        
        if(!IsPostBack) {

            if(!string.IsNullOrEmpty(Request.QueryString["pid"])) { // Kardex
                
                PKardex.Visible = true;
                PBuscar.Visible = false;
                Page.Title = "Kardex de " + Request.QueryString["n"];
                if(!string.IsNullOrEmpty(Request.QueryString["l"]) && !string.IsNullOrEmpty(Request.QueryString["v"])) {
                    Page.Title +=" (" + Request.QueryString["l"] + ") " + Request.QueryString["v"];
                }
                GKardex.DataBind();
                double saldo=0;
                for(int i=GKardex.Rows.Count-1; i>=0; i--) {
                    try
                    {
                        saldo -= Convert.ToDouble(GKardex.Rows[i].Cells[3].Text);
                    } catch(Exception ex) {}
                    try {
                        saldo += Convert.ToDouble(GKardex.Rows[i].Cells[2].Text);
                    } catch (Exception ex) { }
                    GKardex.Rows[i].Cells[4].Text= saldo.ToString();
                }
            } else {
                PKardex.Visible = false;
                PBuscar.Visible = true;
                BBuscar_Click(sender, e);
            }
        }
    }
    protected void BBuscar_Click(object sender, EventArgs e)
    {
        GResultados.DataSourceID = CDetallado.Checked ? "SResultados1" : "SResultados";
        GResultados.Columns[1].Visible = CDetallado.Checked;
        GResultados.Columns[2].Visible = CDetallado.Checked;

        GResultados.AllowPaging = false;
        GResultados.DataBind();
        LTotal.Text = GResultados.Rows.Count + " Resultados";
        if (GResultados.Rows.Count > 0)
        {
            /*((MasterPageI)Master).Excel(GResultados);
            ((MasterPageI)Master).Imprimir(GResultados);*/
        }
        GResultados.AllowPaging = true;
        GResultados.DataBind();
    }
    protected void GKardex_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if(e.Row.RowIndex!=-1) {
            
            String id= e.Row.Cells[1].Text;
            e.Row.Cells[1].Text = "";
            HyperLink link = new HyperLink();
            switch(DataBinder.Eval(e.Row.DataItem,"tipo").ToString()) {
                case "1":
                    link.Text = "Entrada No. " + id;
                    link.NavigateUrl = "~/almacen/entrada.aspx?ei=" + id;
                    break;
                case "2":
                    link.Text = "Traslado No. " + id;
                    link.NavigateUrl = "~/almacen/traslado.aspx?ti=" + id;
                    break;
            }
            e.Row.Cells[1].Controls.Add(link);
        }
    }
}
