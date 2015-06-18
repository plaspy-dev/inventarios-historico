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
    protected class Kardex {

        protected decimal _entradas = 0;
        protected decimal _salidas = 0;
        protected decimal _saldos = 0;

        public string fecha { get; set; }
        public bool bEntradas = false;
        public decimal entradas { get { return _entradas; } set { bEntradas = true; _entradas = value; } }
        public bool bSalidas = false;
        public decimal salidas { get { return _salidas; } set { bSalidas = true; _salidas= value; } }
        public bool bSaldos = false;
        public decimal saldo { get { return _saldos; } set { bSaldos = true; _saldos= value; } }

        public Kardex(string szFecha) {
            fecha = szFecha;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        ((ing)Master).CheckAlmacen();
        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["vid"]))
            {
                Producto info = Producto.GetProducto(Request.QueryString["vid"]);
                TProductoID.Text = info.id;
                TProducto.Text = info.NombreCompleto;
                CDetallado.Checked = true;

                TVence.Text = DateTime.Now.ToShortDateString();
            } else if (!string.IsNullOrEmpty(Request.QueryString["pvid"])) {
                Producto info= Producto.GetProducto(Request.QueryString["pvid"]);
                TProductoID.Text = info.id;
                TProducto.Text = info.NombreCompleto;

                using (CConector conector = new CConector())
                {
                    string dias = conector.QueryString("SELECT dias_alerta FROM producto WHERE id=" + Query.GetValor(info.id));
                    try
                    {
                        TVence.Text = DateTime.Now.AddDays(Convert.ToInt32(dias)).ToShortDateString();
                    }
                    catch { }
                }
                CDetallado.Checked = true;
            } else if (!string.IsNullOrEmpty(Request.QueryString["gpid"])) {

                Producto info = Producto.GetProducto(Request.QueryString["gpid"]);
                TProductoID.Text = info.id;
                TProducto.Text = info.NombreCompleto;
            }

            ((ing)Master).InitAlmacen();
            DropDownList almacenes = ((ing)Master).GetDropDownAlmacen();
            for (int i = 0; i < almacenes.Items.Count; i++)
            {
                ListItem item = new ListItem(almacenes.Items[i].Text, almacenes.Items[i].Value);
                item.Selected = !string.IsNullOrEmpty(Request.QueryString["gpid"]) || almacenes.Items[i].Selected;
                DAlmacenes.Items.Add(item);
            }
            ((ing)Master).RestoreState();
        }

        bool todoslosalmacenes = false;
        string seleccionado = "";
        for(int i=0; i<DAlmacenes.Items.Count; i++) {
            if (DAlmacenes.Items[i].Selected)
            {
                if(seleccionado.Length>0) {
                    todoslosalmacenes = true;
                    break;
                }
                seleccionado = DAlmacenes.Items[i].Value;
            }
        }

        if (!todoslosalmacenes)
        {
            if (seleccionado.Length > 0 && seleccionado != ((ing)Master).GetAlmacen())
                ((ing)Master).GetDropDownAlmacen().SelectedValue = seleccionado;
            else if (seleccionado.Length == 0)
                DAlmacenes.SelectedValue = ((ing)Master).GetAlmacen();
        }

        Utilidades.GetGridColumn(GResultados, "Ubicación").Visible = !todoslosalmacenes && CConfiguracion.GetConfiguracion().nUbicacion != 0;
        Utilidades.GetGridColumn(GResultados, "Estado").Visible = CConfiguracion.GetConfiguracion().bEstado;
        Utilidades.GetGridColumn(GResultados, "Almacén").Visible = todoslosalmacenes && CAlmacen.Checked;

        SKardex.SelectCommand = String.Format(SKardex.SelectCommand, ((ing)Master).GetAlmacen());

        if (todoslosalmacenes)
        {
            ViewState["multiplealmacenes"] = true;
            string original = SResultados.SelectCommand = String.Format(SResultados.SelectCommand, "0", "%almacen%");
            string original1 = SResultados1.SelectCommand = String.Format(SResultados1.SelectCommand, "0", "%almacen%");

            ArrayList alm = new ArrayList();
            for (int i = 0; i < DAlmacenes.Items.Count; i++)
            {
                if (DAlmacenes.Items[i].Selected)
                    alm.Add(DAlmacenes.Items[i].Value);
            }
            SResultados.SelectCommand = Almacen.GetAllAlamcenes(SResultados.SelectCommand, alm);
            SResultados1.SelectCommand = Almacen.GetAllAlamcenes(SResultados1.SelectCommand, alm);

            original = original.Replace("estado0.", "")
                  .Replace("producto.", "")
                  .Replace("IFNULL(almacen.nombre,'(Ajustes)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT IF(DATE(SYSDATE())>=vence AND vence!='0000-00-00',estado,'(Vencido)'))=1,IFNULL(IF(DATE(SYSDATE())>=vence AND vence!='0000-00-00','(Vencido)',estado.nombre),'(Ninguno)'), '(Multiple)') AS ", "")
                  .Replace(",CONCAT(codigo,IFNULL(producto_proveedor.codigo,'')) AS codigo", ",codigo")
                  ;
            SResultados.SelectCommand = original.Substring(0, original.IndexOf(" FROM "))
                + " FROM ("
                + SResultados.SelectCommand;
            if(CAlmacen.Checked)
                SResultados.SelectCommand += ") AS t GROUP BY producto,lote,vence,estado,almacen ORDER BY codigo,nombre,lote,vence,almacen";
            else SResultados.SelectCommand += ") AS t GROUP BY producto,lote,vence,estado ORDER BY codigo,nombre,lote,vence";

            original1 = original1.Replace("estado0.", "")
                  .Replace("producto.", "")
                  .Replace("IFNULL(almacen.nombre,'(Ajustes)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ", "")
                  .Replace("IF(COUNT(DISTINCT IF(DATE(SYSDATE())>=vence AND vence!='0000-00-00',estado,'(Vencido)'))=1,IFNULL(IF(DATE(SYSDATE())>=vence AND vence!='0000-00-00','(Vencido)',estado.nombre),'(Ninguno)'), '(Multiple)') AS ", "")
                  .Replace(",CONCAT(codigo,IFNULL(producto_proveedor.codigo,'')) AS codigo", ",codigo")
                  ;
            SResultados1.SelectCommand = original1.Substring(0, original1.IndexOf(" FROM "))
                + " FROM ("
                + SResultados1.SelectCommand;
            if (CAlmacen.Checked)
                SResultados1.SelectCommand += ") AS t GROUP BY producto,lote,vence,estado,almacen ORDER BY nombre,lote,vence,almacen";
            else SResultados1.SelectCommand += ") AS t GROUP BY producto,lote,estado,vence ORDER BY nombre,lote,vence";
        } else {
            ViewState["multiplealmacenes"] = null;
            SResultados.SelectCommand = String.Format(SResultados.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
            SResultados1.SelectCommand = String.Format(SResultados1.SelectCommand, ((ing)Master).GetAlmacen(), ((ing)Master).GetAlmacen());
        }

        if(!IsPostBack) {

            CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

            // costo
            Utilidades.GetGridColumn(GResultados, "Costo").Visible = configuracion.nCosto != 0;

            // vencimiento
            TVence.Visible = LVence.Visible = configuracion.bVencimiento;

            // ubicacion
            TUbicacion.Visible= TUbicacionID.Visible= LUbicacion.Visible = configuracion.nUbicacion != 0;

            // estado
            DEstado.Visible = LEstado.Visible = configuracion.bEstado;

            LAlmacenes.Visible= DAlmacenes.Visible = CAlmacen.Visible= Usuario.GetUsuario()["almacenes"].IndexOf(",") != -1 || Usuario.GetUsuario()["almacenes"] == "-2";

            if(!string.IsNullOrEmpty(Request.QueryString["pid"])) { // Kardex

                Utilidades.GetGridColumn(GKardex, "Costo U.E.").Visible =
                Utilidades.GetGridColumn(GKardex, "Costo T.E.").Visible =
                Utilidades.GetGridColumn(GKardex, "Costo U.S.").Visible =
                Utilidades.GetGridColumn(GKardex, "Costo T.S.").Visible =
                Utilidades.GetGridColumn(GKardex, "Costo U.").Visible =
                Utilidades.GetGridColumn(GKardex, "Costo T.").Visible =
                chart_costos.Visible = 
                configuracion.nCosto != 0;

                if (TDesde.Text.Length== 0)
                {
                    TDesde.Text = DateTime.Now.AddDays(-DateTime.Now.Day + 1).ToShortDateString();
                    THasta.Text = DateTime.Now.ToShortDateString();
                }

                PKardex.Visible = true;
                PBuscar.Visible = false;
                Page.Title = "Kardex de " + Producto.GetProducto(Request.QueryString["pid"]).nombre;

                BActualizar_Click(sender, e);
            } else {
                PKardex.Visible = false;
                PBuscar.Visible = true;
                BBuscar_Click(sender, e);
            }
        }
    }

    protected Kardex GetFecha(List<Kardex> grafica, string fecha) {

        Kardex kardex = null;
        List<Kardex> resultado = (from p in grafica where p.fecha==fecha select p).ToList();
        if (resultado.Count == 0)
        {
            kardex = new Kardex(fecha);
            grafica.Add(kardex);
        } else kardex = resultado[0];
        return kardex;
    }

    protected void BBuscar_Click(object sender, EventArgs e)
    {
        GResultados.DataSourceID = CDetallado.Checked ? "SResultados1" : "SResultados";

        Utilidades.GetGridColumn(GResultados, "Lote").Visible = CDetallado.Checked && CConfiguracion.GetConfiguracion().bLote;
        Utilidades.GetGridColumn(GResultados, "Vence").Visible = CDetallado.Checked && CConfiguracion.GetConfiguracion().bVencimiento;

        if(Utilidades.GetRowsCount(GResultados, LTotal)>0) {
            ((ing)Master).SetPrintFile("estado.html", true);
        } else
        {
            ((ing)Master).SetPrintFile(null);
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
                case "-1":
                    e.Row.Cells[0].Text = TDesde.Text;
                    e.Row.Cells[1].Text = "Corte a " + TDesde.Text;
                    return;
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
        producto.bIngreso = true;

        producto.cantidad = Convert.ToDouble(TCantidad.Text, CultureInfo.InvariantCulture);

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();

            if (!producto.DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                return;

            producto.ubicacion = TNUbicacionID.Text.Length == 0 ? 0 : Convert.ToInt32(TNUbicacionID.Text);
            producto.GetCostoPromedio(((ing)Master).GetAlmacen());

            if (!producto.AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                return;

            conector.Commit();
            ViewState["producto"] = null;
            PUbicacion.Visible = false;
            GResultados.SelectedIndex = -1;
            GResultados.DataBind();
        }
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

                ListItem item = new ListItem(nombre, value);
                
                if(producto.lote==GResultados.SelectedDataKey.Values["lote"].ToString()
                    && producto.vence == CConector.GetDateFormat(GResultados.SelectedDataKey.Values["vence"].ToString())
                    && producto.NombreUbicacion == GResultados.SelectedDataKey.Values["ubicacion"].ToString()
                    && producto.szEstado==GResultados.SelectedDataKey.Values["estado"].ToString()
                    ) {
                    item.Selected = true;
                }
                DProductoUbicaciones.Items.Add(item);
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

                    ListItem item = new ListItem(nombre, value);

                    if (producto.lote == GResultados.SelectedDataKey.Values["lote"].ToString()
                        && producto.vence == CConector.GetDateFormat(GResultados.SelectedDataKey.Values["vence"].ToString())
                        && producto.NombreUbicacion == GResultados.SelectedDataKey.Values["ubicacion"].ToString()
                        && producto.szEstado == GResultados.SelectedDataKey.Values["estado"].ToString()
                        )
                    {
                        item.Selected = true;
                    }
                    DProductoEstado.Items.Add(item);
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
        producto.bIngreso = true;

        producto.cantidad = Convert.ToDouble(TEstadoCantidad.Text, CultureInfo.InvariantCulture);

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();

            if (!producto.DeleteProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                return;

            producto.estado = Convert.ToInt32(DNuevoEstado.SelectedValue);
            producto.GetCostoPromedio(((ing)Master).GetAlmacen());

            if (!producto.AddProductoFromEstado(conector, ((ing)Master).GetAlmacen()))
                return;

            conector.Commit();
            PEstado.Visible = false;
            ViewState["producto"] = null;
            GResultados.SelectedIndex = -1;
            GResultados.DataBind();
        }
    }
    protected void BActualizar_Click(object sender, EventArgs e)
    {
        try
        {
            if (!IsValid)
                return;
        } catch {}

        GKardex.DataBind();
        if(GKardex.Rows.Count>0)
            ((ing)Master).SetPrintFile("kardex.html", true);
        else ((ing)Master).SetPrintFile(null);

        LSalidas.Text = "";
        LCostos.Text = "";
        LEntradas.Text = "";
        LSaldos.Text = "";

        Decimal saldo = 0, entradas = 0, salidas = 0, costosaldo = 0, costoentradas = 0, costosalidas = 0, costop = 0;

        int columnafecha = Utilidades.GetGridColumnIndex(GKardex, "Fecha");
        int columnasalida = Utilidades.GetGridColumnIndex(GKardex, "Salida");
        int columnaentrada = Utilidades.GetGridColumnIndex(GKardex, "Entrada");
        int columnasaldo = Utilidades.GetGridColumnIndex(GKardex, "Saldo");
        int columnaCostoUEntrada = Utilidades.GetGridColumnIndex(GKardex, "Costo U.E.");
        int columnaCostoTEntrada = Utilidades.GetGridColumnIndex(GKardex, "Costo T.E.");
        int columnaCostoTSalida = Utilidades.GetGridColumnIndex(GKardex, "Costo T.S.");
        int columnaCostoTSaldo = Utilidades.GetGridColumnIndex(GKardex, "Costo T.");
        int columnaCostoPromedio = Utilidades.GetGridColumnIndex(GKardex, "Costo U.");
        int columnaCostoUSalida = Utilidades.GetGridColumnIndex(GKardex, "Costo U.S.");

        List<Kardex> grafica = new List<Kardex>();

        Decimal saldoa = 0, costosaldoa = 0;

        if (new Producto(Request.QueryString["pid"]).GetAcumulado(((ing)Master).GetAlmacen(), Query.GetMySQLDateFormat(TDesde.Text)
            , ref costosaldoa, ref saldoa, ref costop))
        {
            int index = GKardex.Rows.Count - 1;
            GKardex.Rows[index].Cells[columnaentrada].Text = saldoa.ToString();
            GKardex.Rows[index].Cells[columnaCostoUEntrada].Text = string.Format("{0:C}", costop);
            GKardex.Rows[index].Cells[columnaCostoTEntrada].Text = string.Format("{0:C}", costosaldoa);
            costop = 0;
        }

        for (int i = GKardex.Rows.Count - 1; i >= 0; i--)
        {

            string fecha = GKardex.Rows[i].Cells[columnafecha].Text;
            try
            {
                decimal valor = Convert.ToDecimal(GKardex.Rows[i].Cells[columnasalida].Text);
                salidas += valor;
                saldo -= valor;

                GetFecha(grafica, fecha).salidas += valor;

                // calculamos el valor a que salio el producto.
                valor = costop * valor;

                GKardex.Rows[i].Cells[columnaCostoUSalida].Text = string.Format("{0:C}", costop);
                GKardex.Rows[i].Cells[columnaCostoTSalida].Text = string.Format("{0:C}", valor);

                costosalidas += valor;
                costop = costosaldo -= valor;
            }
            catch { }
            try
            {
                decimal valor = Convert.ToDecimal(GKardex.Rows[i].Cells[columnaentrada].Text);
                entradas += valor;
                saldo += valor;

                GetFecha(grafica, fecha).entradas += valor;

                valor = decimal.Parse(GKardex.Rows[i].Cells[columnaCostoTEntrada].Text, NumberStyles.Currency);
                costoentradas += valor;
                costosaldo += valor;

                LCostos.Text += string.Format(",['{0}', {1}]"
                    , fecha
                    , decimal.Parse(GKardex.Rows[i].Cells[columnaCostoUEntrada].Text, NumberStyles.Currency).ToString(CultureInfo.InvariantCulture));
            }
            catch { }

            GetFecha(grafica, fecha).saldo = saldo;

            if (saldo > 0)
                costop = costosaldo / saldo;
            GKardex.Rows[i].Cells[columnasaldo].Text = saldo.ToString();
            GKardex.Rows[i].Cells[columnaCostoTSaldo].Text = string.Format("{0:C}", costosaldo);
            GKardex.Rows[i].Cells[columnaCostoPromedio].Text = string.Format("{0:C}", costop);
        }

        for (int i = 0; i < grafica.Count; i++)
        {
            Kardex kardex = grafica[i];
            if (kardex.bSalidas)
                LSalidas.Text += string.Format(",['{0}', {1}]"
                    , kardex.fecha
                    , kardex.salidas.ToString(CultureInfo.InvariantCulture)
                    );

            if (kardex.bEntradas)
                LEntradas.Text += string.Format(",['{0}', {1}]"
                    , kardex.fecha
                    , kardex.entradas.ToString(CultureInfo.InvariantCulture)
                    );
            if (kardex.bSaldos)
                LSaldos.Text += string.Format(",['{0}', {1}]"
                    , kardex.fecha
                    , kardex.saldo.ToString(CultureInfo.InvariantCulture)
                    );
        }

        // LCostos.Text = ",['2012/02/02', 15.15],['2012/02/01', 16.15]";
        GKardex.FooterRow.Cells[Utilidades.GetGridColumnIndex(GKardex, "Concepto")].Text = "Total sin acumulado";
        GKardex.FooterRow.Cells[columnaentrada].Text = string.Format("{0:F}", entradas - saldoa);
        GKardex.FooterRow.Cells[columnasalida].Text = string.Format("{0:F}", salidas);
        GKardex.FooterRow.Cells[columnaCostoTEntrada].Text = string.Format("{0:C}", costoentradas - costosaldoa);
        GKardex.FooterRow.Cells[columnaCostoTSalida].Text = string.Format("{0:C}", costosalidas);
        GKardex.FooterRow.Cells[columnasaldo].Text = string.Format("{0:F}", entradas - saldoa - salidas);
        //GKardex.FooterRow.Cells[columnaCostoPromedio].Text = costop.ToString();
        GKardex.FooterRow.Cells[columnaCostoTSaldo].Text = string.Format("{0:C}", costoentradas - costosaldoa - costosalidas);

        LDias.Text= LRotacion.Text = "";
        try 
        {
            decimal rotacion = salidas/(entradas/ 2);
            LRotacion.Text = string.Format("Índice de rotación: {0:F}.", rotacion);

            TimeSpan span= Convert.ToDateTime(THasta.Text) - Convert.ToDateTime(TDesde.Text);
            decimal diaseni = span.Days / rotacion;
            LDias.Text = string.Format("<br/>{0:F} días en el inventario.", diaseni);
        } catch {}
    }
}
