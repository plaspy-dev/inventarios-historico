using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;

public partial class almacen_estado : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();

        Utilidades.GetGridColumn(GResultados, "Ubicación").Visible = !CTodosAlmacenes.Checked;
        Utilidades.GetGridColumn(GResultados, "Estado").Visible = !CTodosAlmacenes.Checked;
        Utilidades.GetGridColumn(GResultados, "Almacén").Visible= CTodosAlmacenes.Checked && CAlmacen.Checked;

        SKardex.SelectCommand = String.Format(SKardex.SelectCommand, ((ing)Master).GetAlmacen());
        SUbicacion.SelectCommand = String.Format(SUbicacion.SelectCommand, ((ing)Master).GetAlmacen());

        if (CTodosAlmacenes.Checked) {
            string original = SResultados.SelectCommand = String.Format(SResultados.SelectCommand, "0", "%almacen%");
            string original1 = SResultados1.SelectCommand = String.Format(SResultados1.SelectCommand, "0", "%almacen%");

            ArrayList alm = new ArrayList();
            DropDownList almacenes = ((ing)Master).GetDropDownAlmacen();
            for (int i = 0; i < almacenes.Items.Count; i++)
            {
                alm.Add(almacenes.Items[i].Value);
            }

            SResultados.SelectCommand = Almacen.GetAllAlamcenes(SResultados.SelectCommand, alm);
            SResultados1.SelectCommand = Almacen.GetAllAlamcenes(SResultados1.SelectCommand, alm);

            original = original.Replace("estado0.", "")
                  .Replace("producto.", "")
                  .Replace("IFNULL(almacen.nombre,'(Ajustes)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT estado)=1,IFNULL(estado.nombre,'(Ninguno)'), '(Multiple)') AS ", "")
                  ;
            SResultados.SelectCommand = original.Substring(0, original.IndexOf(" FROM "))
                + " FROM ("
                + SResultados.SelectCommand;
            if(CAlmacen.Checked)
                SResultados.SelectCommand += ") AS t GROUP BY producto,lote,vence,almacen ORDER BY codigo,nombre,lote,vence,almacen";
            else SResultados.SelectCommand += ") AS t GROUP BY producto,lote,vence ORDER BY codigo,nombre,lote,vence";


            original1 = original1.Replace("estado0.", "")
                  .Replace("producto.", "")
                  .Replace("IFNULL(almacen.nombre,'(Ajustes)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT estado)=1,IFNULL(estado.nombre,'(Ninguno)'), '(Multiple)') AS ", "")
                  ;
            SResultados1.SelectCommand = original1.Substring(0, original1.IndexOf(" FROM "))
                + " FROM ("
                + SResultados1.SelectCommand;
            if (CAlmacen.Checked)
                SResultados1.SelectCommand += ") AS t GROUP BY producto,lote,vence,almacen ORDER BY nombre,lote,vence,almacen";
            else SResultados1.SelectCommand += ") AS t GROUP BY producto,lote,vence ORDER BY nombre,lote,vence";
        } else {
            SResultados.SelectCommand = String.Format(SResultados.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
            SResultados1.SelectCommand = String.Format(SResultados1.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
        }

        if(!IsPostBack) {

            CAlmacen.Visible= CTodosAlmacenes.Visible = Usuario.GetUsuario()["almacenes"].IndexOf(",") != -1 || Usuario.GetUsuario()["almacenes"] == "-2";

            if(!string.IsNullOrEmpty(Request.QueryString["pid"])) { // Kardex

                PKardex.Visible = true;
                PBuscar.Visible = false;
                Page.Title = "Kardex de " + Request.QueryString["n"];
                if(!string.IsNullOrEmpty(Request.QueryString["l"]) && !string.IsNullOrEmpty(Request.QueryString["v"])) {
                    Page.Title +=" (" + Request.QueryString["l"] + ") " + Request.QueryString["v"];
                }
                GKardex.DataBind();
                Decimal saldo = 0;
                for(int i=GKardex.Rows.Count-1; i>=0; i--) {
                    try
                    {
                        saldo -= Convert.ToDecimal(GKardex.Rows[i].Cells[4].Text);
                    } catch {}
                    try {
                        saldo += Convert.ToDecimal(GKardex.Rows[i].Cells[3].Text);
                    } catch { }
                    GKardex.Rows[i].Cells[5].Text= saldo.ToString();
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
        
        Utilidades.GetGridColumn(GResultados, "Lote").Visible = CDetallado.Checked;
        Utilidades.GetGridColumn(GResultados, "Vence").Visible = CDetallado.Checked;

        if(Utilidades.GetRowsCount(GResultados, LTotal)>0) {

        }
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
                case "3":
                    link.Text = "Remisión No. " + id;
                    link.NavigateUrl = "~/almacen/salida.aspx?si=" + id;
                    break;
                case "4":
                case "5":
                    link.Text = "Transformación No. " + id;
                    link.NavigateUrl = "~/almacen/transformaciones.aspx?tri=" + id;
                    break;
            }
            e.Row.Cells[1].Controls.Add(link);
        }
    }
    protected void BCancelar_Click(object sender, EventArgs e) {

        ViewState["producto"] = null;
        PUbicacion.Visible = false;
        GResultados.SelectedIndex = -1;
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        string []detalle = DProductoUbicaciones.SelectedValue.Split(';');

        Producto producto = new Producto(detalle[0]);
        producto.vence= detalle[1];
        producto.lote= detalle[2];
        producto.ubicacion= Convert.ToInt32(detalle[3]);
        producto.serial= detalle[5];
        producto.estado=Convert.ToInt32(detalle[6]);
        producto.Ingreso = true;

        producto.cantidad = Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture);

        CConector conector = new CConector();
        conector.StartTransaction();

        if (!producto.DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
            return;

        producto.ubicacion = Convert.ToInt32(DNuevaUbicacion.SelectedValue);
        if (!producto.AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
            return;

        conector.Commit();
        ViewState["producto"] = null;
        PUbicacion.Visible = false;
        GResultados.SelectedIndex = -1;
    }

    protected void GResultados_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["producto"] = ((HyperLink)GResultados.SelectedRow.Cells[Utilidades.GetGridColumnIndex(GResultados, "Producto")].Controls[0]).Text;
        DProductoUbicaciones.Items.Clear();
        ArrayList productos;

        ProductoUsado productousado = new ProductoUsado();

        productos = productousado.GetProductos(GResultados.DataKeys[GResultados.SelectedIndex].Values[0].ToString()
            , -1, ((ing)Master).GetAlmacen());

        if (productos!=null) {
            
            for (int i = 0; i < productos.Count; i++)
            {
                Producto producto = (Producto)productos[i];
                string nombre = "";
                if (!string.IsNullOrEmpty(producto.vence))
                    nombre += producto.vence + " - ";
                nombre+= producto.lote;
                if (!string.IsNullOrEmpty(producto.vence))
                    nombre += " - " + producto.serial;
                nombre+= " [" + producto.szEstado + "]";
                nombre+= " " + producto.NombreUbicacion;
                string value = producto.id + ";" + producto.vence + ";" + producto.lote + ";" + producto.ubicacion + ";" + producto.cantidad.ToString(CultureInfo.InvariantCulture) + ";" + producto.serial + ";" + producto.estado;
                DProductoUbicaciones.Items.Add(new ListItem(nombre, value));
            }

            PUbicacion.Visible = true;
        }
    }
    protected void GResultados_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Estado")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            GResultados.SelectedIndex= gvr.RowIndex;

            ViewState["producto"] = ((HyperLink)GResultados.SelectedRow.Cells[Utilidades.GetGridColumnIndex(GResultados, "Producto")].Controls[0]).Text;
            DProductoEstado.Items.Clear();
            ArrayList productos;

            ProductoUsado productousado = new ProductoUsado();

            productos = productousado.GetProductos(GResultados.DataKeys[GResultados.SelectedIndex].Values[0].ToString()
                , -1, ((ing)Master).GetAlmacen());

            if (productos != null)
            {
                for (int i = 0; i < productos.Count; i++)
                {

                    Producto producto = (Producto)productos[i];
                    string nombre = "";
                    if (!string.IsNullOrEmpty(producto.vence))
                        nombre += producto.vence + " - ";
                    nombre += producto.lote;
                    if (!string.IsNullOrEmpty(producto.vence))
                        nombre += " - " + producto.serial;
                    nombre += " [" + producto.szEstado + "]";
                    nombre += " " + producto.NombreUbicacion;

                    string value = producto.id + ";" + producto.vence + ";" + producto.lote + ";" + producto.estado + ";" + producto.cantidad.ToString(CultureInfo.InvariantCulture) + ";" + producto.serial + ";" + producto.ubicacion;
                    DProductoEstado.Items.Add(new ListItem(nombre, value));
                }

                PEstado.Visible = true;
            }
        }
    }
    protected void BCancelarEstado_Click(object sender, EventArgs e)
    {

        ViewState["producto"] = null;
        PEstado.Visible = false;
        GResultados.SelectedIndex = -1;
    }

    protected void BAceptarEstado_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        string[] detalle = DProductoEstado.SelectedValue.Split(';');

        Producto producto = new Producto(detalle[0]);
        producto.vence = detalle[1];
        producto.lote = detalle[2];
        producto.ubicacion = Convert.ToInt32(detalle[6]);
        producto.serial = detalle[5];
        producto.estado = Convert.ToInt32(detalle[3]);
        producto.Ingreso = true;

        producto.cantidad = Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture);

        CConector conector = new CConector();
        conector.StartTransaction();

        if (!producto.DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
            return;

        producto.estado= Convert.ToInt32(DNuevoEstado.SelectedValue);
        if (!producto.AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
            return;

        conector.Commit();
        DEstado.Visible = false;
        ViewState["producto"] = null;
        GResultados.SelectedIndex = -1;
    }
}
