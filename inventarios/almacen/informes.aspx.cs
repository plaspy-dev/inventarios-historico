using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using SDG.Componentes;

public partial class almacen_informes : System.Web.UI.Page
{
    protected class Almacenes {
        public string Nombre { get; set; }
        protected decimal _valor1 = 0;
        protected decimal _valor2 = 0;
        public bool bValor1 = false;
        public decimal valor1 { get { return _valor1; } set { bValor1 = true; _valor1 = value; } }
        public bool bValor2 = false;
        public decimal valor2 { get { return _valor2; } set { bValor2 = true; _valor2 = value; } }

        public Almacenes(string almacen) {
            Nombre= almacen;
        }
    }
    protected class Resumen
    {
        public string fecha { get; set; }
        public List<Almacenes> almacenes;

        public Resumen(string szFecha, List<Almacenes> alm)
        {
            fecha = szFecha;
            almacenes = new List<Almacenes>();
            foreach (Almacenes info in alm)
            {
                almacenes.Add(new Almacenes(info.Nombre));
            }
        }

        public Almacenes GetAlmacen(string almacen) {
            return (from p in almacenes where p.Nombre==almacen select p).ToList().First();
        }
    }

    protected Almacenes GetFecha(List<Resumen> grafica, string fecha, string almacen, List<Almacenes> alm)
    {
        Resumen info = null;
        try
        {
            info = (from p in grafica where p.fecha == fecha select p).ToList().First();
        } catch {}
        if (info==null)
        {
            info = new Resumen(fecha, alm);
            grafica.Add(info);
        }
        return info.GetAlmacen(almacen);
    }

    protected string GetGrafica(List<Resumen> grafica, string titulo, bool totalizar) {

        string resultado = "";
        if (grafica.Count == 0)
            return resultado;
        Resumen temp= grafica[0];
        resultado+="['Fecha'";
        if (totalizar || temp.almacenes.Count == 1)
        {
            resultado += string.Format(",'{0}'", titulo);
        } else
        {
            foreach (Almacenes alm in temp.almacenes)
            {
                resultado += string.Format(",'{0}'", alm.Nombre);
            }
        }
        resultado+="]";

        foreach (Resumen info in grafica)
        {
            resultado += ",[";
            resultado += string.Format("'{0}'", info.fecha);
            Decimal total = 0;
            foreach (Almacenes alm in info.almacenes)
            {
                if (!totalizar)
                    resultado += string.Format(",{0}", alm.valor1.ToString(CultureInfo.InvariantCulture));
                else total += alm.valor1;
            }
            if(totalizar)
                resultado += string.Format(",{0}", total.ToString(CultureInfo.InvariantCulture));
            resultado += "]";
        }
        return resultado;
    }

    protected string GetGrafica(List<Resumen> grafica, string titulo1, string titulo2, bool totalizar)
    {
        string resultado = "";
        if (grafica.Count == 0)
            return resultado;
        
        bool bTitulo1 = !string.IsNullOrEmpty(titulo1);
        bool bTitulo2 = !string.IsNullOrEmpty(titulo2);

        if (!bTitulo1 && !bTitulo2)
            return resultado;

        Resumen temp = grafica[0];
        resultado += "['Fecha'";
        if (totalizar || temp.almacenes.Count == 1)
        {
            if(bTitulo1)
                resultado += string.Format(",'{0}'", titulo1);
            if (bTitulo2)
                resultado += string.Format(",'{0}'", titulo2);
        }
        else
        {
            foreach (Almacenes alm in temp.almacenes)
            {
                if (bTitulo1)
                    resultado += string.Format(",'{0}'", titulo1 + " " + alm.Nombre);
                if (bTitulo2)
                    resultado += string.Format(",'{0}'", titulo2 + " " + alm.Nombre);
            }
        }
        resultado += "]";

        foreach (Resumen info in grafica)
        {
            resultado += ",[";
            resultado += string.Format("'{0}'", info.fecha);
            Decimal valor1 = 0, valor2 = 0;
            foreach (Almacenes alm in info.almacenes)
            {
                if (!totalizar)
                {
                    if (bTitulo1)
                        resultado += string.Format(",{0}", alm.valor1.ToString(CultureInfo.InvariantCulture));
                    if (bTitulo2)
                        resultado += string.Format(",{0}", alm.valor2.ToString(CultureInfo.InvariantCulture));
                } else {
                    valor1 += alm.valor1;
                    valor2 += alm.valor2;
                }
            }
            if(totalizar) {
                if (bTitulo1)
                    resultado += string.Format(",{0}", valor1.ToString(CultureInfo.InvariantCulture));
                if (bTitulo2)
                    resultado += string.Format(",{0}", valor2.ToString(CultureInfo.InvariantCulture));
            }
            resultado += "]";
        }
        return resultado;
    }

    protected void ReplaceAlmacen(SqlDataSource source, bool todos) {

        if (todos)
        {
            ArrayList alm = new ArrayList();
            for (int i = 0; i < DAlmacenes.Items.Count; i++)
            {
                if (DAlmacenes.Items[i].Selected)
                    alm.Add(DAlmacenes.Items[i].Value);
            }
            source.SelectCommand = Almacen.GetAllAlamcenes(string.Format(source.SelectCommand.Replace("a.id={0}", "a.id=%almacen%"), "0"), alm);
        }
        else
        {
            source.SelectCommand = String.Format(source.SelectCommand, ((ing)Master).GetAlmacen());
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();
        if (!IsPostBack)
        {
            ((ing)Master).InitAlmacen();
            DropDownList almacenes = ((ing)Master).GetDropDownAlmacen();
            for (int i = 0; i < almacenes.Items.Count; i++)
                DAlmacenes.Items.Add(almacenes.Items[i]);
            ((ing)Master).RestoreState();
        }

        bool todoslosalmacenes = false;
        string seleccionado = "";
        for (int i = 0; i < DAlmacenes.Items.Count; i++)
        {
            if (DAlmacenes.Items[i].Selected)
            {
                if (seleccionado.Length > 0)
                {
                    todoslosalmacenes = true;
                    break;
                }
                seleccionado = DAlmacenes.Items[i].Value;
            }
        }
        
        if (!todoslosalmacenes) 
        {
            if(seleccionado.Length > 0 && seleccionado != ((ing)Master).GetAlmacen())
                ((ing)Master).GetDropDownAlmacen().SelectedValue = seleccionado;
            else if(seleccionado.Length==0)
                DAlmacenes.SelectedValue = ((ing)Master).GetAlmacen();
        }

        ReplaceAlmacen(SResultados1, todoslosalmacenes);
        ReplaceAlmacen(SResultados2, todoslosalmacenes);
        ReplaceAlmacen(SResultados3, todoslosalmacenes);
        ReplaceAlmacen(SResultados4, todoslosalmacenes);

        Utilidades.GetGridColumn(GResultados, "Almacén").Visible = todoslosalmacenes;

        CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

        Utilidades.GetGridColumn(GResultados, "Costo").Visible= configuracion.nCosto != 0;
        Utilidades.GetGridColumn(GResultados, "Precio").Visible = configuracion.nValor != 0;

        if(!IsPostBack) {

            LAlmacenes.Visible = DAlmacenes.Visible = Usuario.GetUsuario()["almacenes"].IndexOf(",") != -1 || Usuario.GetUsuario()["almacenes"] == "-2";

            if (THasta.Text.Length == 0)
            {
                THasta.Text = DateTime.Now.ToShortDateString();
                TDesde.Text = DateTime.Now.AddDays(-30).ToShortDateString();
            }

            if(Request.QueryString["p"]=="1")
                BBuscar_Click(sender, e);
        }
    }

    protected void BBuscar_Click(object sender, EventArgs e)
    {
        Page.Title = DVer.SelectedItem.Text;

        CConfiguracion configuracion = CConfiguracion.GetConfiguracion();
        PGraficas.Visible=
        GResultados.Visible = true;

        GResultados.DataSourceID = "SResultados" + DVer.SelectedValue;

        int desdehacia = Utilidades.GetGridColumnIndex(GResultados, "Desde/Hacia");
        if (desdehacia == -1)
        {
            desdehacia = Utilidades.GetGridColumnIndex(GResultados, "Desde");
            if (desdehacia == -1)
                desdehacia = Utilidades.GetGridColumnIndex(GResultados, "Hacia");
        }
        int documento = Utilidades.GetGridColumnIndex(GResultados, "Documento");
        int tercero = Utilidades.GetGridColumnIndex(GResultados, "Tercero");

        switch(DVer.SelectedValue) {
            case "1": // entradas
                GResultados.Columns[desdehacia].Visible = true;
                GResultados.Columns[documento].Visible = true;
                GResultados.Columns[tercero].Visible = true;
                GResultados.Columns[desdehacia].HeaderText = "Desde";
                Utilidades.GetGridColumn(GResultados, "Precio").Visible = false;
                break;
            case "2": // traslados
                GResultados.Columns[desdehacia].Visible = true;
                GResultados.Columns[documento].Visible = true;
                GResultados.Columns[tercero].Visible = false;
                GResultados.Columns[desdehacia].HeaderText = "Hacia";
                Utilidades.GetGridColumn(GResultados, "Precio").Visible = false;
                break;
            case "3": // salidas
                GResultados.Columns[desdehacia].Visible = false;
                GResultados.Columns[documento].Visible = true;
                GResultados.Columns[tercero].Visible = true;
                Utilidades.GetGridColumn(GResultados, "Precio").Visible = configuracion.nValor != 0;
                break;
            case "4": // traslados
                GResultados.Columns[desdehacia].Visible = false;
                GResultados.Columns[documento].Visible = false;
                GResultados.Columns[tercero].Visible = false;
                Utilidades.GetGridColumn(GResultados, "Precio").Visible = false;
                break;
        }

        if (Utilidades.GetRowsCount(GResultados, LTotal) > 0)
        {
            ((ing)Master).SetPrintFile("informes.html", true);
        } else {
            ((ing)Master).SetPrintFile(null);
        }

        Utilidades.GetGridColumn(GResultados, "Estado").Visible= DEstadoDoc.SelectedValue=="0";

        LCostos.Text = "";
        LCantidad.Text = "";
        LProductos.Text = "";
        LDTCostos.Text = "";
        LDTCantidad.Text = "";
        LDTProductos.Text = "";

        chart_costos.Visible = false;

        if(GResultados.FooterRow!=null) {

            List<Almacenes> almacenes= new List<Almacenes>();
            List<Resumen> graficaproductos = new List<Resumen>();
            List<Resumen> graficacantidad = new List<Resumen>();
            List<Resumen> graficacostos = new List<Resumen>();
            
            int colalmacen=Utilidades.GetGridColumnIndex(GResultados, "Almacén");

            dtcantidad.Visible= dtcostos.Visible= dtproductos.Visible=
            LDTCostos.Visible = LDTProductos.Visible = LDTCantidad.Visible = GResultados.Columns[colalmacen].Visible;

            if(GResultados.Columns[colalmacen].Visible) {

                string oldalmacen = "", temp;
                for (int i = 0; i < GResultados.Rows.Count; i++) {
                    temp = GResultados.Rows[i].Cells[colalmacen].Text;
                    if (temp == oldalmacen)
                        continue;
                    oldalmacen = temp;
                    almacenes.Add(new Almacenes(temp));
                }
            } else if(GResultados.Rows.Count>0) {
                almacenes.Add(new Almacenes(GResultados.Rows[0].Cells[colalmacen].Text));
            }

            int coltotalproductos = Utilidades.GetGridColumnIndex(GResultados, "Total productos");
            int colcantidad = Utilidades.GetGridColumnIndex(GResultados, "Cantidad");
            int colcosto = Utilidades.GetGridColumnIndex(GResultados, "Costo");
            int colprecio = Utilidades.GetGridColumnIndex(GResultados, "Precio");
            int colfecha = Utilidades.GetGridColumnIndex(GResultados, "Fecha");

            bool bCosto = GResultados.Columns[colcosto].Visible;
            bool bPrecio = GResultados.Columns[colprecio].Visible;

            int totalproductos = 0;
            Decimal cantidad = 0, costo = 0, precio = 0;

            chart_costos.Visible = bCosto || bPrecio;

            for (int i = 0; i < GResultados.Rows.Count; i++)
            {
                string fecha = ((HyperLink) GResultados.Rows[i].Cells[colfecha].Controls[0]).Text;
                string almacen= GResultados.Rows[i].Cells[colalmacen].Text;
                Almacenes info= GetFecha(graficaproductos, fecha, almacen, almacenes);
                decimal valor = 0;
                
                int productos = Convert.ToInt32(GResultados.Rows[i].Cells[coltotalproductos].Text);
                totalproductos += productos;
                info.valor1 += productos;

                info = GetFecha(graficacantidad, fecha, almacen, almacenes);

                valor = Convert.ToDecimal(GResultados.Rows[i].Cells[colcantidad].Text);
                cantidad += valor;
                info.valor1 += valor;
                
                if(bCosto || bPrecio) {

                    info = GetFecha(graficacostos, fecha, almacen, almacenes);
                    if(bCosto) {

                        valor = decimal.Parse(GResultados.Rows[i].Cells[colcosto].Text, NumberStyles.Currency);
                        costo += valor;
                        info.valor1 += valor;
                    }
                    if (bPrecio)
                    {
                        try
                        {
                            valor = decimal.Parse(GResultados.Rows[i].Cells[colprecio].Text, NumberStyles.Currency);
                            precio += valor;
                            info.valor2 += valor;
                        }
                        catch { }
                    }
                }
            }

            LCantidad.Text = GetGrafica(graficacantidad, "Cantidad", true);
            LProductos.Text = GetGrafica(graficaproductos, "Cantidad", true);
            LCostos.Text= GetGrafica(graficacostos, bCosto ? "Costo" : null, bPrecio ? "Precio" : null, true);

            if (LDTCantidad.Visible)
            {
                LDTCantidad.Text = GetGrafica(graficacantidad, "Cantidad", false);
                LDTProductos.Text = GetGrafica(graficaproductos, "Cantidad", false);
                LDTCostos.Text = GetGrafica(graficacostos, bCosto ? "Costo" : null, bPrecio ? "Precio" : null, false);
            }

            GResultados.FooterRow.Cells[Utilidades.GetGridColumnIndex(GResultados, "Fecha")].Text = "Total";
            GResultados.FooterRow.Cells[coltotalproductos].Text = totalproductos.ToString();
            GResultados.FooterRow.Cells[colcantidad].Text = cantidad.ToString();
            GResultados.FooterRow.Cells[colcosto].Text = string.Format("{0:C}", costo);
            GResultados.FooterRow.Cells[colprecio].Text = string.Format("{0:C}", precio);
        }
    }

    protected void GResultados_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    protected void GResultados_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1)
        {
            int colfecha = Utilidades.GetGridColumnIndex(GResultados, "Fecha");
            int colnumero = Utilidades.GetGridColumnIndex(GResultados, "Número");

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

            String texto = e.Row.Cells[colnumero].Text;
            e.Row.Cells[colnumero].Text = "";
            HyperLink link = new HyperLink();
            link.Text= texto;
            link.NavigateUrl= url;
            e.Row.Cells[colnumero].Controls.Add(link);

            texto = e.Row.Cells[colfecha].Text;
            e.Row.Cells[colfecha].Text = "";
            link = new HyperLink();
            link.Text = texto;
            link.NavigateUrl = url;
            e.Row.Cells[colfecha].Controls.Add(link);
        }
    }
}
