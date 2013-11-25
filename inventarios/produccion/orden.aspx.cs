using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;

public partial class produccion_orden : System.Web.UI.Page
{
    [Serializable]
    public class Analisis
    {
        public string IdTabla { get; set; }
        public int IdGrid { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public Dictionary<string, string> Parametros { get; set; }

        public Analisis(int idGrid)
        {
            IdGrid = idGrid;
            Parametros = new Dictionary<string, string>();
        }
    }

    [Serializable]
    public class ProductoIngrediente : Producto {
        public double? CantidadOriginal { get; set; }
        [XmlIgnore]
        public string codigoExtra {
            get
            {
                return codigo+Notificacion;
            }
        }
        [XmlIgnore]
        public string Notificacion { 
            get {
                if (CantidadOriginal == null)
                    return "*";
                if (CantidadOriginal == -1)
                    return "(a)";
                if (CantidadOriginal.Value > this.cantidad)
                    return "-";
                if (CantidadOriginal.Value < this.cantidad)
                    return "+";
                return "";
            } 
        }
        public ProductoIngrediente(string idproducto) : base(idproducto) { }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        ((ing)Master)._ImprimirClicked += new EventHandler(OnImprimir);
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            if (Session["orden_clean"] != null)
            {
                ViewState["clean"] = true;
                Session["orden_clean"] = null;
            } else ViewState["clean"] = false;

            if (Session["loproduccion"] != null)
            {
                ViewState["urlprint"] = Request.Url.AbsolutePath + "?oi=" + Session["loproduccion"].ToString() + "&p=1";
                LImprimir.Text = "Orden de producción " + Session["loproduccion"].ToString() + " Editada.<br/>¿Desea imprimirla?";
                PImprimir.Visible = true;
                Session["loproduccion"] = null;
            }
            BAceptar.Enabled = usuario.CanInsert();
            Utilidades.GetGridColumn(GProductos, "Lote ").Visible = true;
            Utilidades.GetGridColumn(GProductos, "Lote").Visible = false;

            if (!string.IsNullOrEmpty(Request.QueryString["oi"]))
            {
                using (CConector conector = new CConector())
                {
                    Query query = new Query("orden_produccion");

                    query += "id";
                    query += "planeacion";
                    query += "produccion";
                    query += "producto";
                    query += "tercero";
                    query += "lote";
                    query += "cantidad";
                    query += "observaciones";
                    query += "formulacion";
                    query += "estado";
                    query += "anulacion";
                    query += "chartcolor";
                    query += "marmita";

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["oi"])));
                    if (reader != null && reader.Read())
                    {
                        PEditar.Visible = true;
                        Session["parametros"] = Request.Url.Query;
                        ViewState["id"] =
                            TNumero.Text = reader["id"].ToString();

                        PAnulado.Visible = reader["estado"].ToString() != "";
                        PAnular.Visible= BAnular.Visible = usuario.CanAdmin();
                        if (PAnulado.Visible)
                        {
                            LAnulado.Text = "Anulada";
                            LAnulacion.Text = reader["anulacion"].ToString();
                            LMotivoAnulacion.Visible = true;
                            BAnularReal.Enabled=
                            BAnular.Enabled = BAceptar.Enabled = false;
                        }

                        if (Request.QueryString["p"] == "1")
                        {
                            Utilidades.GetGridColumn(GProductos, "Lote ").Visible = false;
                            Utilidades.GetGridColumn(GProductos, "Lote").Visible = true;
                        } else {
                            Utilidades.GetGridColumn(GProductos, "Lote ").Visible = usuario.CanUpdate();
                            Utilidades.GetGridColumn(GProductos, "Lote").Visible = !usuario.CanUpdate();
                        }

                        HAnalisis.NavigateUrl += "?oi=" + TNumero.Text;
                        HProduccion.NavigateUrl += "?oi=" + TNumero.Text;

                        Conector.SelectInDropDown(DMarmita, reader["marmita"]);
                        Conector.SelectInDropDown(DMarmitaI, reader["marmita"]);

                        BAceptar.Enabled = usuario.CanUpdate();
                        ((ing)Master).SetModificacion(TNumero.Text);
                        ((ing)Master).SetPrintFile("orden.html");

                        TFecha.Text = Conector.GetFromMySQLDate(reader["planeacion"]);
                        TFechaProduccion.Text = Conector.GetFromMySQLDate(reader["produccion"]);
                        
                        TClienteID.Text = reader["tercero"].ToString();
                        Tercero tercero= new Tercero(TClienteID.Text);
                        TCliente.Text = tercero.Documento + " " + tercero.Nombre;

                        Producto info = new Producto(reader["producto"].ToString());
                        TProductoID.Text = info.id;
                        TRecetaID.Text = reader["formulacion"].ToString();
                        TProducto.Text = info.NombreCompleto;
                        LEmpresa.Text = info.Empresa;

                        TLote.Text = reader["lote"].ToString();
                        TCantidad.Text = Convert.ToDecimal(reader["cantidad"]).ToString("0.#########", CultureInfo.InvariantCulture);
                        TObservaciones.Text = reader["observaciones"].ToString();
                        TChartColor.Text = reader["chartcolor"].ToString();

                        ViewState["FormatoLote"] = info.FormatoLote.Replace("%codigo%", info.codigo);

                        query = new Query("proceso_orden_produccion");

                        query += "produccion_fecha";
                        query += "produccion_inicio";
                        query += "produccion_fin";
                        query += "produccion_operararios";
                        query += "empaque_fecha";
                        query += "empaque_inicio";
                        query += "empaque_fin";
                        query += "empaque_operarios";
                        query += "unidad1";
                        query += "unidad2";
                        query += "reproceso";
                        query += "presentacion1";
                        query += "presentacion2";
                        query += "observaciones";
                        query += "contenedor";

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(TNumero.Text)));
                        if (reader != null && reader.Read())
                        {
                            ViewState["orden"] = TNumero.Text;

                            DMarmitaI.Visible= LMarmita.Visible = false;

                            TFechaInicio.Text = Conector.GetFromMySQLDate(reader["produccion_fecha"]);
                            TInicioProceso.Text = GetHora(reader["produccion_inicio"]);
                            TFinProceso.Text = GetHora(reader["produccion_fin"]);
                            TFechaEmpaque.Text = Conector.GetFromMySQLDate(reader["empaque_fecha"]);
                            TInicioEmpaque.Text = GetHora(reader["empaque_inicio"]);
                            TFinEmpaque.Text = GetHora(reader["empaque_fin"]);
                            TOperariosProceso.Text = reader["produccion_operararios"].ToString();
                            TOperariosEmpaque.Text = reader["empaque_operarios"].ToString();
                            TCantidadUnidad1.Text = reader["unidad1"] == DBNull.Value ? "" : Convert.ToDouble(reader["unidad1"]).ToString(CultureInfo.InvariantCulture);
                            TCantidadUnidad2.Text = reader["unidad2"] == DBNull.Value ? "" : Convert.ToDouble(reader["unidad2"]).ToString(CultureInfo.InvariantCulture);
                            TCantidadPresentacion1.Text = reader["presentacion1"] == DBNull.Value ? "" : Convert.ToDouble(reader["presentacion1"]).ToString(CultureInfo.InvariantCulture);
                            TCantidadPresentacion2.Text = reader["presentacion2"] == DBNull.Value ? "" : Convert.ToDouble(reader["presentacion2"]).ToString(CultureInfo.InvariantCulture);
                            TReproceso.Text = reader["reproceso"] == DBNull.Value ? "" : Convert.ToDouble(reader["reproceso"]).ToString(CultureInfo.InvariantCulture);
                            TObservacionesProceso.Text = reader["observaciones"].ToString();
                            TContenedor.Text = reader["contenedor"].ToString();
                            double total = 0;
                            try
                            {
                                total += Convert.ToDouble(reader["unidad1"]) * Convert.ToDouble(reader["presentacion1"]);
                            } catch { }
                            try
                            {
                                total += Convert.ToDouble(reader["unidad2"]) * Convert.ToDouble(reader["presentacion2"]);
                            } catch { }
                            LObtenida.Text = total.ToString("0.##");
                            try
                            {
                                total = Convert.ToDouble(TCantidad.Text.Replace(" Kg.", ""), CultureInfo.InvariantCulture) - total;
                                total-=Convert.ToDouble(reader["reproceso"]);
                            }
                            catch { }
                            LMerma.Text = total.ToString("0.##");
                        }

                        List<ProductoIngrediente> lista = new List<ProductoIngrediente>();

                        query = new Query("orden_produccion_ingredientes");

                        query += "id";
                        query += "producto";
                        query += "cantidad";
                        query += "lote";
                        query += "cantidad_original";

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(Request.QueryString["oi"])));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                ProductoIngrediente infoingrediente = new ProductoIngrediente(reader["producto"].ToString());
                                infoingrediente.IdGrid = lista.Count;
                                infoingrediente.IdTabla = reader["id"].ToString();
                                infoingrediente.lote = reader["lote"].ToString();
                                if(reader["cantidad_original"]!=DBNull.Value)
                                    infoingrediente.CantidadOriginal = Convert.ToDouble(reader["cantidad_original"]);
                                infoingrediente.cantidad = Convert.ToDouble(reader["cantidad"]);
                                lista.Add(infoingrediente);
                            }
                        }
                        UpdateProductos(lista);

                        query = new Query("analisis_fisico_quimico");

                        query += "id";
                        query += "fecha";
                        query += "observaciones";

                        List<Analisis> listaanalisis = new List<Analisis>();

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(TNumero.Text) + " ORDER BY fecha DESC"));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Analisis infoanalisis = new Analisis(lista.Count);
                                infoanalisis.IdTabla = reader["id"].ToString();
                                infoanalisis.Fecha = (DateTime)reader["fecha"];
                                infoanalisis.Observaciones = reader["observaciones"].ToString();
                                AddParametros(infoanalisis, reader["id"].ToString(), "analisis_fisico_quimico_parametros");
                                listaanalisis.Add(infoanalisis);
                            }
                        }

                        // HAnalisis.Value = Serializar.SerializeBinary(listaanalisis);
                        UpdateList(GAnalisis, listaanalisis);

                        // analisis microbiologico
                        query = new Query("analisis_microbiologico");

                        query += "id";
                        query += "fecha";
                        query += "observaciones";

                        listaanalisis = new List<Analisis>();

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(TNumero.Text) + " ORDER BY fecha DESC"));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Analisis infoanalisis = new Analisis(lista.Count);
                                infoanalisis.IdTabla = reader["id"].ToString();
                                infoanalisis.Fecha = (DateTime)reader["fecha"];
                                infoanalisis.Observaciones = reader["observaciones"].ToString();
                                AddParametros(infoanalisis, reader["id"].ToString(), "analisis_microbiologico_parametros");
                                listaanalisis.Add(infoanalisis);
                            }
                        }

                        // HMicro.Value = Serializar.SerializeBinary(lista);
                        UpdateList(GMicro, listaanalisis);

                        CSensorial.DataBind();
                        query = new Query("analisis_sensorial_parametros");

                        query += "parametro";
                        query += "valor";

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(Request.QueryString["oi"])));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                ListItem item = CSensorial.Items.FindByValue(reader["parametro"].ToString());
                                if (item != null)
                                    item.Selected = reader["valor"].ToString() == "1";
                            }
                        }

                        CEsterilidad.DataBind();
                        query = new Query("prueba_esterilidad_parametros");

                        query += "parametro";
                        query += "valor";

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(Request.QueryString["oi"])));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                ListItem item = CEsterilidad.Items.FindByValue(reader["parametro"].ToString());
                                if (item != null)
                                    item.Selected = reader["valor"].ToString() == "1";
                            }
                        }
                        return;
                    }
                }
            }
            using (CConector conector = new CConector())
                TNumero.Text = conector.QueryString("SELECT IF(MAX(id),MAX(id)+1,1) FROM orden_produccion");
            TFecha.Text = DateTime.Now.ToShortDateString();
            TNumero.Focus();
        }
    }


    public void UpdateList(GridView grid, List<Analisis> lista)
    {
        if (lista.Count > 0)
        {
            foreach (var info in lista[0].Parametros)
            {
                if (Utilidades.GetGridColumn(grid, info.Key) == null)
                {
                    BoundField lname = new BoundField();
                    lname.HeaderText = info.Key;
                    lname.HtmlEncode = false;
                    lname.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    grid.Columns.Insert(grid.Columns.Count - 1, lname);
                }
            }
        }
        grid.DataSource = lista;
        grid.DataBind();
    }

    public void AddParametros(Analisis info, string analisis, string tabla)
    {

        using (CConector conector = new CConector())
        {
            Query query = new Query(tabla);
            query += "valor";
            query += "parametro";
            MySqlDataReader reader = conector.Query(query.GetSelect("analisis=" + analisis));
            if (reader != null)
            {
                while (reader.Read())
                    info.Parametros.Add(Almacen.GetNombreVarios(reader["parametro"].ToString()), reader["valor"] == DBNull.Value ? "" : Convert.ToDouble(reader["valor"]).ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    protected string GetHora(object hora)
    {

        try
        {
            return hora.ToString().Substring(0, 5);
        }
        catch { }
        return "";
    }

    protected void UpdateProductos(List<ProductoIngrediente> lista)
    {
        List<ProductoIngrediente> activos = lista.Where(x => x.IdGrid != -1).ToList();

        ViewState["productos"] = Serializar.SerializeBinary(lista);
        GProductos.DataSource = activos;
        GProductos.DataBind();

        if (GProductos.FooterRow != null)
        {
            GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = activos.Sum(x => x.cantidad).ToString("0.00");
            int count = activos.Count;
            GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Producto")].Text = count.ToString() + " Producto" + (count == 1 ? "" : "s");
        }
    }

    protected void GProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        List<ProductoIngrediente> lista = Serializar.DesSerializeBinary<List<ProductoIngrediente>>((string)ViewState["productos"]);
        try
        {
            ProductoIngrediente info = lista
                .Where(x => x.IdGrid.ToString() == GProductos.DataKeys[e.RowIndex].Values[0].ToString())
                .ToList().First();
            TIngredienteID.Text = info.id;
            TLoteIngrediente.Text = info.lote;
            TIngrediente.Text = info.NombreCompleto;
            ViewState["cantidadoriginal"] = info.CantidadOriginal;
            TCantidadIngrediente.Text = info.cantidad.ToString(CultureInfo.InvariantCulture);
            info.IdGrid = -1;
            // lista.Remove(info);

            for (int i = 0; i < lista.Count; i++)
                if(lista[i].IdGrid!=-1)
                    lista[i].IdGrid = i;

            UpdateProductos(lista);
        }
        catch { }
        e.Cancel = true;
    }

    protected void BMas_Click(object sender, EventArgs e) {

        if (!IsValid)
            return;

        List<ProductoIngrediente> lista = Serializar.DesSerializeBinary<List<ProductoIngrediente>>((string)ViewState["productos"]);
        ProductoIngrediente info = new ProductoIngrediente(TIngredienteID.Text);
        info.cantidad = Convert.ToDouble(TCantidadIngrediente.Text, CultureInfo.InvariantCulture);
        info.lote = TLoteIngrediente.Text;
        info.IdGrid = lista.Count;
        info.CantidadOriginal = (double ?)ViewState["cantidadoriginal"];
        lista.Add(info);

        UpdateProductos(lista);
        HProductos.Value = Serializar.SerializeBinary(lista.Where(x=> string.IsNullOrEmpty(x.IdTabla)).ToList());

        TIngrediente.Focus();
        TCantidadIngrediente.Text =
        TIngredienteID.Text = TIngrediente.Text =TLoteIngrediente.Text=  "";
        ViewState["cantidadoriginal"] = null;
    }

    protected void BAnular_Click(object sender, EventArgs e) {
        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            Query query = new Query("orden_produccion");

            query["anulacion"] = TMotivoAnulacion.Text;
            query["estado"] = "-1";

            string id = (string)ViewState["id"];
            if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(id))) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            if (conector.InsertModificacion(id) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            conector.Commit();
            Session["orden_clean"] = 1;
            Session["parametros"] = "?oi=" + id;
            ((ing)Master).SetResultado("Orden de producción " + id + " Anulada");
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        if (GProductos.Rows.Count == 0)
        {
            LMensaje.Text = "Por favor ingrese por lo menos un producto";
            TIngrediente.Focus();
            return;
        }

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            Query query = new Query("orden_produccion");

            string id = (string)ViewState["id"];

            query["planeacion"] = Conector.GetMySQLDateFormat(TFecha.Text);
            query["produccion"] = Conector.GetMySQLDateFormat(TFechaProduccion.Text);
            query["tercero"]= TClienteID.Text;
            query["producto"] = TProductoID.Text;
            query["lote"] = TLote.Text;
            query["cantidad"] = TCantidad.Text.Replace(" Kg.", "");
            query["observaciones"] = TObservaciones.Text;
            query["formulacion"] = TRecetaID.Text;
            query["chartcolor"] = TChartColor.Text;
            query["marmita"] = DMarmitaI.Visible ? DMarmitaI.SelectedValue : DMarmita.SelectedValue;
            
            if (string.IsNullOrEmpty(id))
            {
                int res = conector.QueryMod(query.GetInsert(), true);
                if (res == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
                id = res.ToString();
            }
            else
            {
                if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(id))) == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }
            if (conector.InsertModificacion(id) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }


            query = new Query("proceso_orden_produccion");

            string orden = (string)ViewState["orden"];
            query["produccion_fecha"] = Conector.GetMySQLDateFormat(TFechaInicio.Text);
            query["produccion_inicio"] = TInicioProceso.Text;
            query["produccion_fin"] = TFinProceso.Text;
            query["empaque_fecha"] = Conector.GetMySQLDateFormat(TFinEmpaque.Text);
            query["empaque_inicio"] = TInicioEmpaque.Text;
            query["empaque_fin"] = TFinEmpaque.Text;
            query["produccion_operararios"] = TOperariosProceso.Text;
            query["empaque_operarios"] = TOperariosEmpaque.Text;
            query["unidad1"] = TCantidadUnidad1.Text;
            query["unidad2"] = TCantidadUnidad2.Text;
            query["presentacion1"] = TCantidadPresentacion1.Text.Replace(" Kg.", "");
            query["presentacion2"] = TCantidadPresentacion2.Text.Replace(" Kg.", "");
            query["reproceso"] = TReproceso.Text.Replace(" Kg.", "");
            query["observaciones"] = TObservacionesProceso.Text;
            query["contenedor"] = TContenedor.Text;

            if (string.IsNullOrEmpty(orden))
            {
                orden = query["orden"] = id;
                if (conector.QueryMod(query.GetInsert()) == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }
            else
            {
                if (conector.QueryMod(query.GetUpdate("orden=" + Query.GetValor(orden))) == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }

            List<ProductoIngrediente> lista = Serializar.DesSerializeBinary<List<ProductoIngrediente>>((string)ViewState["productos"]);
            query = new Query("orden_produccion_ingredientes");
            query["orden"] = id;

            for (int i = 0; i < lista.Count; i++)
            {
                if (string.IsNullOrEmpty(lista[i].IdTabla))
                {
                    if (lista[i].IdGrid != -1)
                    {
                        query["producto"] = lista[i].id;
                        query["cantidad"] = lista[i].cantidad.ToString(CultureInfo.InvariantCulture);
                        query["cantidad_original"] = lista[i].CantidadOriginal==null ? "" : lista[i].CantidadOriginal.Value.ToString(CultureInfo.InvariantCulture);
                        query["lote"] = Utilidades.GetGridColumn(GProductos, "Lote ").Visible ? GetLoteByRow(lista[i].IdGrid) : query["lote"] = lista[i].lote;
                        if (conector.QueryMod(query.GetInsert()) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                } else if (lista[i].IdGrid == -1) {
                    if (conector.QueryMod("DELETE FROM orden_produccion_ingredientes WHERE id=" + lista[i].IdTabla) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                } else if (Utilidades.GetGridColumn(GProductos, "Lote ").Visible)
                {  // esta es para editarla tiene idtabla
                    query["lote"] = GetLoteByRow(lista[i].IdGrid);
                    if (conector.QueryMod(query.GetUpdate("id=" + lista[i].IdTabla)) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                }
            }

            Query sensorial = new Query("analisis_sensorial_parametros");
            if (conector.QueryMod(sensorial.GetDelete("orden=" + Query.GetValor(ViewState["id"].ToString()))) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            sensorial["orden"] = ViewState["id"].ToString();
            for (int i = 0; i < CSensorial.Items.Count; i++)
            {
                if (CSensorial.Items[i].Selected)
                {
                    sensorial["parametro"] = CSensorial.Items[i].Value;
                    sensorial["valor"] = "1";
                    if (conector.QueryMod(sensorial.GetInsert()) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                }
            }


            Query esterilidad = new Query("prueba_esterilidad_parametros");
            if (conector.QueryMod(esterilidad.GetDelete("orden=" + Query.GetValor(ViewState["id"].ToString()))) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            esterilidad["orden"] = ViewState["id"].ToString();
            for (int i = 0; i < CEsterilidad.Items.Count; i++)
            {
                if (CEsterilidad.Items[i].Selected)
                {
                    esterilidad["parametro"] = CEsterilidad.Items[i].Value;
                    esterilidad["valor"] = "1";
                    if (conector.QueryMod(esterilidad.GetInsert()) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                }
            }

            conector.Commit();
            Session["orden_clean"] = 1;
            Session["parametros"] = "?oi=" + id;
            Session["loproduccion"] = id;
            ((ing)Master).SetResultado("Orden de producción " + id + " Editado");
        }
    }

    protected string GetLoteByRow(int idGrid)
    {
        string id= idGrid.ToString();
        for(int i=0; i<GProductos.Rows.Count; i++) {
            if (GProductos.DataKeys[i].Values["IdGrid"].ToString() == id)
            {
                TextBox lote = (TextBox)GProductos.Rows[i].FindControl("TLote");
                return lote.Text;
            }
        }
        return "";
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Session["orden_clean"] = 1;
        Response.Redirect("~/produccion/orden.aspx");
    }

    protected void CheckEmpty(params TextBox[] textBoxs)
    {
        for(int i=0; i<textBoxs.Length; i++)
            if (textBoxs[i].Text.Length == 0)
                textBoxs[i].Text = "__________";
    }

    protected string RenderCheckBox(CheckBoxList check)
    {
        string resultado = "";
        for (int i = 0; i < check.Items.Count; i++)
        {
            resultado += "<tr>";
            for (int j = 0; j < check.RepeatColumns && i < check.Items.Count; j++, i++)
            {
                resultado += "<td><b>";
                resultado += check.Items[i].Selected ? "X" : "_";
                resultado += "</b> " + check.Items[i].Text + "</td>";
            }
            resultado += "</tr>";
        }
        return resultado;
    }

    protected void OnImprimir(object sender, EventArgs e)
    {
        PrintEventArgs evento = (PrintEventArgs) e;
        GProductos.Columns[0].Visible =false;

        Utilidades.GetGridColumn(GProductos, "Lote ").Visible=false;
        Utilidades.GetGridColumn(GProductos, "Lote").Visible = true;

        evento.Contenido = evento.Contenido.Replace("$claseanulado$", PAnulado.Visible ? "" : "hide");
        evento.Contenido = evento.Contenido.Replace("$mermaclase$", LObtenida.Text == "0" ? "hide" : "");

        evento.Contenido = evento.Contenido.Replace("$szSensorial$", RenderCheckBox(CSensorial));
        evento.Contenido = evento.Contenido.Replace("$szEsterilidad$", RenderCheckBox(CEsterilidad));

        if (LObtenida.Text == "0")
        {
            TCantidadUnidad1.Text = 
                TCantidadPresentacion1.Text=
                TCantidadUnidad2.Text= 
                TCantidadPresentacion2.Text=
                TReproceso.Text=
            LObtenida.Text = "__________";
        }
        CheckEmpty(TContenedor
                ,TOperariosEmpaque
                ,TOperariosProceso
                ,TInicioEmpaque
                ,TInicioProceso
                ,TFinEmpaque
                ,TFinProceso
                ,TFechaEmpaque
                );
        
        int pos = evento.Contenido.IndexOf("<div");
        int posf = evento.Contenido.IndexOf("</div>");

        string contenidoreal = evento.Contenido.Substring(pos, posf - pos + 6);
        string contenido = "";

        // Utilidades.GetGridColumn(GProductos, "Producto").Visible = false;
        contenido += Utilidades.ReplaceContenido(Page.Controls, contenidoreal);
        // Utilidades.GetGridColumn(GProductos, "Producto").Visible = true;

        evento.Contenido= evento.Contenido.Insert(posf + 6, contenido);

    }

    protected void BFormular_Click(object sender, EventArgs e)
    {
        LMensaje.Text = "";
        if (TRecetaID.Text.Length == 0)
        {
            LMensaje.Text = "Por favor seleccione un producto";
            TProducto.Focus();
            return;
        }
        if (TCantidad.Text.Length == 0)
        {
            LMensaje.Text = "Por favor ingrese una cantidad";
            TCantidad.Focus();
            return;
        }

        Producto producto = new Producto(TRecetaID.Text);
        LEmpresa.Text = producto.Empresa;

        using (CConector conector = new CConector())
        {
            double cantidad = Convert.ToDouble(TCantidad.Text.Replace(" Kg.", ""), CultureInfo.InvariantCulture);
            Query query = new Query("formulacion_ingrediente");

            query += "producto";
            query += "cantidad";

            List<ProductoIngrediente> lista = Serializar.DesSerializeBinary<List<ProductoIngrediente>>((string)ViewState["productos"]);
            
            // borramos los que no estan fijos
            lista.RemoveAll(x => string.IsNullOrEmpty(x.IdTabla));
            for (int i = 0; i < lista.Count; i++)
                if (lista[i].IdGrid != -1)
                    lista[i].IdGrid = i;

            MySqlDataReader reader = conector.Query(query.GetSelect("formulacion=" + Query.GetValor(TRecetaID.Text)));
            if (reader != null)
            {
                while (reader.Read())
                {
                    ProductoIngrediente info = new ProductoIngrediente(reader["producto"].ToString());
                    info.cantidad = Convert.ToDouble(reader["cantidad"]) * cantidad;
                    info.IdGrid = lista.Count;
                    info.CantidadOriginal = info.cantidad;
                    lista.Add(info);
                }
            }

            UpdateProductos(lista);
        }
        TLote.Focus();
    }

    protected void BActualizar_Click(object sender, EventArgs e)
    {
        List<ProductoIngrediente> lista = Serializar.DesSerializeBinary<List<ProductoIngrediente>>(HProductos.Value);
        HProductos.Value = "";
        RLoteIngrediente.Enabled = false;
        for (int i = 0; i < lista.Count; i++)
        {
            TIngredienteID.Text = lista[i].id;
            TCantidadIngrediente.Text = lista[i].cantidad.ToString(CultureInfo.InvariantCulture);
            TIngrediente.Text = lista[i].NombreCompleto;
            TLoteIngrediente.Text = lista[i].lote;
            ViewState["cantidadoriginal"]= lista[i].CantidadOriginal;
            Validate("agregar");
            BMas_Click(sender, e);
        }
        RLoteIngrediente.Enabled = true;
    }

    protected void GAnalisis_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1)
        {
            for (int i = 1; i < e.Row.Cells.Count - 1; i++)
                e.Row.Cells[i].Text = ((Analisis)e.Row.DataItem).Parametros[GAnalisis.Columns[i].HeaderText];
        }
    }

    protected void GMicro_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1)
        {
            for (int i = 1; i < e.Row.Cells.Count - 1; i++)
                e.Row.Cells[i].Text = ((Analisis)e.Row.DataItem).Parametros[GMicro.Columns[i].HeaderText];
        }
    }
}
