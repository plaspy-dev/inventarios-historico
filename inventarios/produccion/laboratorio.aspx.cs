using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class produccion_laboratorio : System.Web.UI.Page
{
    [Serializable]
    public class Analisis {

        public string IdTabla { get; set; }
        public int IdGrid { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public Dictionary<string, string> Parametros { get; set; }

        public Analisis(int idGrid) {
            IdGrid = idGrid;
            Parametros = new Dictionary<string, string>();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        ((ing)Master)._ImprimirClicked += new EventHandler(OnImprimir);
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();

            TFechaMicro.Enabled= TFecha.Enabled = usuario.CanUpdate();
            TFechaMicro.Text= TFecha.Text = Query.GetFromMySQLDate(DateTime.Now, true);
            
            BAceptar.Enabled = usuario.CanInsert();

            if (!string.IsNullOrEmpty(Request.QueryString["oi"]))
            {
                using (CConector conector = new CConector())
                {
                    Query query = new Query("orden_produccion");

                    query += "id";
                    query += "lote";
                    query += "planeacion";
                    query += "produccion";
                    query += "producto";
                    query += "cantidad";
                    query += "estado";
                    query += "chartcolor";

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["oi"])));
                    if (reader != null && reader.Read())
                    {
                        PAnulado.Visible = reader["estado"].ToString() != "";
                        if (PAnulado.Visible)
                        {
                            LAnulado.Text = "Anulada";
                            BAceptar.Enabled = false;
                        }

                        Session["parametros"] = Request.Url.Query;
                        PBordes.Visible = false;
                        PAnalisis.Visible = true;
                        ViewState["id"]= TNumero.Text = reader["id"].ToString();
                        LCantidad.Text = Convert.ToDecimal(reader["cantidad"]).ToString("0.#########", CultureInfo.InvariantCulture) + " Kg.";
                        TChartColor.Text = reader["chartcolor"].ToString();

                        GAnalisis.Columns[0].Visible= 
                        BAceptar.Enabled = usuario.CanUpdate();
                        GAnalisis.Columns[1].Visible= usuario.CanDelete();

                        ((ing)Master).SetModificacion(TNumero.Text);
                        ((ing)Master).SetPrintFile("laboratorio.html");
                        
                        TLote.Text = reader["lote"].ToString();
                        LPlaneacion.Text = Query.GetFromMySQLDate(reader["planeacion"]);
                        LProduccion.Text = Query.GetFromMySQLDate(reader["produccion"]);
                        Producto producto = new Producto(reader["producto"].ToString());
                        LProducto.Text = producto.NombreCompleto;
                        LCliente.Text= producto.Empresa;
                        SMicro.SelectParameters["producto"].DefaultValue = 
                        SAnalisis.SelectParameters["producto"].DefaultValue = producto.id;

                        query = new Query("analisis_fisico_quimico");

                        query += "id";
                        query+="fecha";
                        query+="observaciones";

                        List<Analisis> lista = new List<Analisis>();

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(TNumero.Text) + " ORDER BY fecha DESC"));
                        if (reader != null) {
                            while (reader.Read()) {

                                Analisis info = new Analisis(lista.Count);
                                info.IdTabla = reader["id"].ToString();
                                info.Fecha = (DateTime) reader["fecha"];
                                info.Observaciones = reader["observaciones"].ToString();
                                AddParametros(info, reader["id"].ToString(), "analisis_fisico_quimico_parametros");
                                lista.Add(info);
                            }
                        }

                        HAnalisis.Value = Serializar.SerializeBinary(lista);
                        UpdateList(GAnalisis, lista.Where(x => x.IdGrid != -1).ToList());

                        // analisis microbiologico
                        query = new Query("analisis_microbiologico");
                        
                        query += "id";
                        query += "fecha";
                        query += "observaciones";

                        lista = new List<Analisis>();

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(TNumero.Text) + " ORDER BY fecha DESC"));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Analisis info = new Analisis(lista.Count);
                                info.IdTabla = reader["id"].ToString();
                                info.Fecha = (DateTime)reader["fecha"];
                                info.Observaciones = reader["observaciones"].ToString();
                                AddParametros(info, reader["id"].ToString(), "analisis_microbiologico_parametros");
                                lista.Add(info);
                            }
                        }

                        HMicro.Value = Serializar.SerializeBinary(lista);
                        UpdateList(GMicro, lista.Where(x => x.IdGrid != -1).ToList());


                        List<Producto> ajustes = new List<Producto>();

                        query = new Query("orden_produccion_ingredientes");

                        query += "id";
                        query += "producto";
                        query += "cantidad";
                        query += "lote";

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(Request.QueryString["oi"]) + " AND cantidad_original=-1"));
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Producto infoingrediente = new Producto(reader["producto"].ToString());
                                infoingrediente.IdGrid = lista.Count;
                                infoingrediente.IdTabla = reader["id"].ToString();
                                infoingrediente.lote = reader["lote"].ToString();
                                infoingrediente.cantidad = Convert.ToDouble(reader["cantidad"]);
                                ajustes.Add(infoingrediente);
                            }
                        }
                        UpdateProductos(ajustes);

                        CSensorial.DataBind();
                        query = new Query("analisis_sensorial_parametros");
                        
                        query+="parametro";
                        query+="valor";

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(Request.QueryString["oi"])));
                        if (reader != null) {
                            while (reader.Read()) {
                                ListItem item= CSensorial.Items.FindByValue(reader["parametro"].ToString());
                                if(item!=null)
                                    item.Selected= reader["valor"].ToString() == "1";
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
            TNumero.Focus();
        }
    }

    public void UpdateList(GridView grid, List<Analisis> lista)
    {
        if (lista.Count > 0)
        {
            foreach(var info in lista[0].Parametros) {
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

    public void AddParametros(Analisis info, string analisis, string tabla) {

        using (CConector conector = new CConector())
        {
            Query query = new Query(tabla);
            query += tabla+ ".valor";
            query += tabla + ".parametro";
            MySqlDataReader reader = conector.Query(query.GetSelect("analisis=" + analisis + " ORDER BY varios.tipo DESC"
                                                       , "LEFT JOIN varios ON (varios.id=" + tabla + ".parametro)"));
            if (reader != null)
            {
                while (reader.Read())
                    info.Parametros.Add(Almacen.GetNombreVarios(reader["parametro"].ToString()), reader["valor"] == DBNull.Value ? "" : Convert.ToDouble(reader["valor"]).ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    protected void BAgregarMicro_Clicked(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HMicro.Value);
        Analisis info = null;

        if (GMicro.SelectedIndex != -1)
        {
            info = lista
                .Where(x => x.IdGrid.ToString() == GMicro.DataKeys[GMicro.SelectedIndex].Values["IdGrid"].ToString())
                .ToList().First();
        }
        if (info == null)
        {
            info = new Analisis(lista.Count);
            lista.Add(info);
        }

        info.Fecha = Convert.ToDateTime(TFechaMicro.Text);
        info.Observaciones = TObservacionesMicro.Text;

        for (int i = 0; i < DMicroSimple.Items.Count; i++)
        {
            RadioButtonList radio = (RadioButtonList)DMicroSimple.Items[i].FindControl("RValor");
            info.Parametros[DMicroSimple.DataKeys[i].ToString()] = radio.SelectedValue;
        }

        DMicroSimple.DataBind();


        for (int i = 0; i < DMicro.Items.Count; i++)
        {
            TextBox text = (TextBox)DMicro.Items[i].FindControl("TValor");
            info.Parametros[DMicro.DataKeys[i].ToString()] = text.Text;
        }

        DMicro.DataBind();

        HMicro.Value = Serializar.SerializeBinary(lista);
        UpdateList(GMicro, lista.Where(x => x.IdGrid != -1).ToList());
        BCancelarMicro_Clicked(sender, e);
    }

    protected void BAgregar_Clicked(object sender, EventArgs e) {
        
        if (!IsValid)
            return;

        List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HAnalisis.Value);
        Analisis info = null;

        if (GAnalisis.SelectedIndex != -1) {
            info = lista
                .Where(x => x.IdGrid.ToString() == GAnalisis.DataKeys[GAnalisis.SelectedIndex].Values["IdGrid"].ToString())
                .ToList().First();
        }
        if (info == null)
        {
            info = new Analisis(lista.Count);
            lista.Add(info);
        }

        info.Fecha = Convert.ToDateTime(TFecha.Text);
        info.Observaciones = TObservaciones.Text;

        for(int i=0; i<DAnalisis.Items.Count; i++) {
            TextBox text = (TextBox)DAnalisis.Items[i].FindControl("TValor");
            info.Parametros[DAnalisis.DataKeys[i].ToString()] = text.Text;
        }

        DAnalisis.DataBind();
        HAnalisis.Value = Serializar.SerializeBinary(lista);
        UpdateList(GAnalisis, lista.Where(x => x.IdGrid != -1).ToList());
        BCancelarAgregar_Clicked(sender, e);
    }

    protected void BCancelarMicro_Clicked(object sender, EventArgs e)
    {
        GMicro.SelectedIndex = -1;
        TFechaMicro.Text = Query.GetFromMySQLDate(DateTime.Now, true);
        BAgregarMicro.Text = "Agregar";
        BCancelarMicro.Visible = false;
        TObservacionesMicro.Text = "";
        SMicro.SelectParameters["analisis"].DefaultValue = "0";
    }

    protected void BCancelarAgregar_Clicked(object sender, EventArgs e) {

        GAnalisis.SelectedIndex = -1;
        TFecha.Text = Query.GetFromMySQLDate(DateTime.Now, true);
        BAgregar.Text = "Agregar";
        BCancelarAgregar.Visible = false;
        TObservaciones.Text ="";
        SMicro.SelectParameters["analisis"].DefaultValue = 
        SAnalisis.SelectParameters["analisis"].DefaultValue = "0";
    }

    protected void BAceptar_Clicked(object sender, EventArgs e) {

        if (!IsValid)
            return;

        if (GAnalisis.Rows.Count == 0 && GMicro.Rows.Count==0)
        {
            LMensaje.Text = "Por favor ingrese por lo menos un análisis";
            // TBrix.Focus();
            return;
        }

        // constuimos los ids y los equivalentes
        Dictionary<string, string> equivalentes = new Dictionary<string, string>();
        for (int j = 0; j < DAnalisis.Items.Count; j++)
            equivalentes[DAnalisis.DataKeys[j].ToString()] = ((HiddenField)DAnalisis.Items[j].FindControl("HId")).Value;
        for (int j = 0; j < DMicro.Items.Count; j++)
            equivalentes[DMicro.DataKeys[j].ToString()] = ((HiddenField)DMicro.Items[j].FindControl("HId")).Value;
        for (int j = 0; j < DMicroSimple.Items.Count; j++)
            equivalentes[DMicroSimple.DataKeys[j].ToString()] = ((HiddenField)DMicroSimple.Items[j].FindControl("HId")).Value;

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();

            Query orden = new Query("orden_produccion");
            orden["chartcolor"] = TChartColor.Text;

            if (conector.QueryMod(orden.GetUpdate("id=" + Query.GetValor(ViewState["id"].ToString()))) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HAnalisis.Value);

            for (int i = 0; i < lista.Count; i++) {

                Query query = new Query("analisis_fisico_quimico");
                if (lista[i].IdGrid != -1)
                {
                    string id=lista[i].IdTabla;

                    query["fecha"] = Query.GetMySQLDateFormat(lista[i].Fecha, true);
                    query["observaciones"] = lista[i].Observaciones;

                    if (string.IsNullOrEmpty(id))
                    {
                        query["orden"] = ViewState["id"].ToString();
                        int res=conector.QueryMod(query.GetInsert(), true);
                        if (res == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                        id= res.ToString();
                    }
                    else
                    {
                        if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(id))) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }

                    query = new Query("analisis_fisico_quimico_parametros");
                    if (conector.QueryMod(query.GetDelete("analisis=" + Query.GetValor(id))) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }

                    query["analisis"] = id;
                    foreach(KeyValuePair<string, string> key in lista[i].Parametros) {

                        try
                        {
                            query["valor"] = key.Value;
                            query["parametro"] = equivalentes[key.Key];

                            if (conector.QueryMod(query.GetInsert()) == -1)
                            {
                                LMensaje.Text = conector.GetLastError();
                                return;
                            }
                        } catch { }
                    }
                } else {
                    if (!string.IsNullOrEmpty(lista[i].IdTabla)) {
                        query = new Query("analisis_fisico_quimico_parametros");
                        if (conector.QueryMod(query.GetDelete("analisis=" + Query.GetValor(lista[i].IdTabla))) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }

                        query = new Query("analisis_fisico_quimico");
                        if (conector.QueryMod(query.GetDelete("id=" + Query.GetValor(lista[i].IdTabla))) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                }
            }

            // analisis microbiologico
            lista = Serializar.DesSerializeBinary<List<Analisis>>(HMicro.Value);

            for (int i = 0; i < lista.Count; i++)
            {

                Query query = new Query("analisis_microbiologico");
                if (lista[i].IdGrid != -1)
                {
                    string id = lista[i].IdTabla;

                    query["fecha"] = Query.GetMySQLDateFormat(lista[i].Fecha, true);
                    query["observaciones"] = lista[i].Observaciones;

                    if (string.IsNullOrEmpty(id))
                    {
                        query["orden"] = ViewState["id"].ToString();
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

                    query = new Query("analisis_microbiologico_parametros");
                    if (conector.QueryMod(query.GetDelete("analisis=" + Query.GetValor(id))) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }

                    query["analisis"] = id;
                    foreach (KeyValuePair<string, string> key in lista[i].Parametros)
                    {
                        try
                        {
                            query["valor"] = key.Value;
                            query["parametro"] = equivalentes[key.Key];

                            if (conector.QueryMod(query.GetInsert()) == -1)
                            {
                                LMensaje.Text = conector.GetLastError();
                                return;
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lista[i].IdTabla))
                    {
                        query = new Query("analisis_microbiologico_parametros");
                        if (conector.QueryMod(query.GetDelete("analisis=" + Query.GetValor(lista[i].IdTabla))) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }

                        query = new Query("analisis_microbiologico");
                        if (conector.QueryMod(query.GetDelete("id=" + Query.GetValor(lista[i].IdTabla))) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                }
            }

            Query sensorial = new Query("analisis_sensorial_parametros");
            if (conector.QueryMod(sensorial.GetDelete("orden=" + Query.GetValor(ViewState["id"].ToString()))) == -1)
            {
                LMensaje.Text= conector.GetLastError();
                return;
            }
            sensorial["orden"] = ViewState["id"].ToString();
            for (int i = 0; i < CSensorial.Items.Count; i++) {
                if (CSensorial.Items[i].Selected) {
                    sensorial["parametro"] = CSensorial.Items[i].Value;
                    sensorial["valor"] = "1";
                    if (conector.QueryMod(sensorial.GetInsert()) == -1) {
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

            // ajustes de laboratorio
            List<Producto> lAjustes = Serializar.DesSerializeBinary<List<Producto>>((string)ViewState["productos"]);
            Query qAjustes = new Query("orden_produccion_ingredientes");
            qAjustes["orden"] = ViewState["id"].ToString();

            for (int i = 0; i < lAjustes.Count; i++)
            {
                if (string.IsNullOrEmpty(lAjustes[i].IdTabla))
                {
                    if (lAjustes[i].IdGrid != -1)
                    {
                        qAjustes["producto"] = lAjustes[i].id;
                        qAjustes["cantidad"] = lAjustes[i].cantidad.ToString(CultureInfo.InvariantCulture);
                        qAjustes["cantidad_original"] = "-1";
                        qAjustes["lote"] = lAjustes[i].lote;
                        if (conector.QueryMod(qAjustes.GetInsert()) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                }
                else if (lAjustes[i].IdGrid == -1)
                {
                    if (conector.QueryMod("DELETE FROM orden_produccion_ingredientes WHERE id=" + lAjustes[i].IdTabla) == -1)
                    {
                        LMensaje.Text = conector.GetLastError();
                        return;
                    }
                }
            }

            if (conector.InsertModificacion(ViewState["id"].ToString()) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            conector.Commit();
            Session["parametros"] = "?oi=" + TNumero.Text;
            ((ing)Master).SetResultado("Orden de producción " + TNumero.Text + " Editada");
        }
    }

    protected void BCancelar_Clicked(object sender, EventArgs e) {
        
        Response.Redirect("~/produccion/laboratorio.aspx");
    }

    protected void GMicro_RowDeleting(object sender, GridViewDeleteEventArgs e) {
        List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HMicro.Value);
        try
        {
            Analisis info = lista
                .Where(x => x.IdGrid.ToString() == GMicro.DataKeys[e.RowIndex].Values["IdGrid"].ToString())
                .ToList().First();

            info.IdGrid = -1;

            for (int i = 0; i < lista.Count; i++)
                if (lista[i].IdGrid != -1)
                    lista[i].IdGrid = i;

            HMicro.Value = Serializar.SerializeBinary(lista);
            GMicro.SelectedIndex = -1;
            UpdateList(GMicro, lista.Where(x => x.IdGrid != -1).ToList());
        }
        catch { }
        e.Cancel = true;
    }

    protected void GAnalisis_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HAnalisis.Value);
        try
        {
            Analisis info = lista
                .Where(x => x.IdGrid.ToString() == GAnalisis.DataKeys[e.RowIndex].Values["IdGrid"].ToString())
                .ToList().First();

            info.IdGrid = -1;

            for (int i = 0; i < lista.Count; i++)
                if (lista[i].IdGrid != -1)
                    lista[i].IdGrid = i;

            HAnalisis.Value = Serializar.SerializeBinary(lista);
            GAnalisis.SelectedIndex = -1;
            UpdateList(GAnalisis, lista.Where(x => x.IdGrid != -1).ToList());
        }
        catch { }
        e.Cancel = true;
    }

    protected void GMicro_SelectedIndexChanged(object sender, EventArgs e) {

        List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HMicro.Value);
        Analisis info = lista
                .Where(x => x.IdGrid.ToString() == GMicro.DataKeys[GMicro.SelectedIndex].Values["IdGrid"].ToString())
                .ToList().First();

        SMicro.SelectParameters["analisis"].DefaultValue = info.IdTabla;
        SMicroSimple.SelectParameters["analisis"].DefaultValue = info.IdTabla;

        TFechaMicro.Text = Query.GetFromMySQLDate(info.Fecha, true);
        TObservacionesMicro.Text = info.Observaciones;
        DMicro.DataBind();
        for (int i = 0; i < DMicro.Items.Count; i++)
        {
            TextBox text = (TextBox)DMicro.Items[i].FindControl("TValor");
            try
            {
                text.Text = info.Parametros[DMicro.DataKeys[i].ToString()];
            }
            catch { }
        }

        DMicroSimple.DataBind();
        for (int i = 0; i < DMicroSimple.Items.Count; i++)
        {
            RadioButtonList radio = (RadioButtonList)DMicroSimple.Items[i].FindControl("RValor");
            try
            {
                radio.SelectedValue= info.Parametros[DMicroSimple.DataKeys[i].ToString()];
            }
            catch { }
        }

        BAgregarMicro.Text = "Editar";
        BCancelarMicro.Visible = true;
    }

    protected void GAnalisis_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HAnalisis.Value);
        Analisis info = lista
                .Where(x => x.IdGrid.ToString() == GAnalisis.DataKeys[GAnalisis.SelectedIndex].Values["IdGrid"].ToString())
                .ToList().First();

        SMicro.SelectParameters["analisis"].DefaultValue = 
        SAnalisis.SelectParameters["analisis"].DefaultValue = info.IdTabla;

        TFecha.Text = Query.GetFromMySQLDate(info.Fecha, true);
        TObservaciones.Text= info.Observaciones;
        DAnalisis.DataBind();

        for (int i = 0; i < DAnalisis.Items.Count; i++)
        {
            TextBox text = (TextBox)DAnalisis.Items[i].FindControl("TValor");
            try
            {
                text.Text = info.Parametros[DAnalisis.DataKeys[i].ToString()];
            } catch { }
        }

        BAgregar.Text = "Editar";
        BCancelarAgregar.Visible = true;
    }

    protected string RenderGrid(GridView grid) {

        string resultado = "";
        if (grid.Rows.Count > 0)
        {
            bool inicio = true;
            for (int i = 3; i < grid.Columns.Count-1; i++)
            {
                if (grid.Columns[i].Visible)
                {
                    if (inicio)
                        resultado += "<tr>";
                    resultado += "<td nowrap=nowrap>" + grid.Columns[i].HeaderText + "</td>";
                    resultado += "<td align=right><b>" + grid.Rows[0].Cells[i].Text + "</b></td>";
                    if (!inicio)
                        resultado += "</tr>";
                    inicio = !inicio;
                }
            }
        } else resultado= "<tr><td colspan='4'>" + grid.EmptyDataText + "</td></tr>";
        return resultado;
    }

    protected string RenderCheckBox(CheckBoxList check)
    {
        string resultado = "";
        for (int i = 0; i < check.Items.Count; i++) {
            resultado += "<tr>";
            for (int j = 0; j < check.RepeatColumns && i<check.Items.Count; j++, i++)
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
        PrintEventArgs evento = (PrintEventArgs)e;
        
        evento.Contenido = evento.Contenido.Replace("$claseanulado$", PAnulado.Visible ? "" : "hide");
        evento.Contenido = evento.Contenido.Replace("$szFechaMicro$", GMicro.Rows.Count == 0 ? "" : GMicro.Rows[0].Cells[Utilidades.GetGridColumnIndex(GMicro, "Fecha")].Text);
        evento.Contenido = evento.Contenido.Replace("$szFechaAnalisis$", GAnalisis.Rows.Count == 0 ? "" : GAnalisis.Rows[0].Cells[Utilidades.GetGridColumnIndex(GAnalisis, "Fecha")].Text);

        evento.Contenido = evento.Contenido.Replace("$szAnalisis$", RenderGrid(GAnalisis));
        evento.Contenido = evento.Contenido.Replace("$szMicro$", RenderGrid(GMicro));
        evento.Contenido = evento.Contenido.Replace("$szSensorial$", RenderCheckBox(CSensorial));
        evento.Contenido = evento.Contenido.Replace("$szEsterilidad$", RenderCheckBox(CEsterilidad));
    }

    protected void BMas_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>((string)ViewState["productos"]);
        Producto info = new Producto(TIngredienteID.Text);
        info.cantidad = Convert.ToDouble(TCantidadIngrediente.Text, CultureInfo.InvariantCulture);
        info.lote = TLoteIngrediente.Text;
        info.IdGrid = lista.Count;
        lista.Add(info);

        UpdateProductos(lista);
        HProductos.Value = Serializar.SerializeBinary(lista.Where(x => string.IsNullOrEmpty(x.IdTabla)).ToList());

        TIngrediente.Focus();
        TCantidadIngrediente.Text =
        TIngredienteID.Text = TIngrediente.Text = TLoteIngrediente.Text = "";
    }

    protected void UpdateProductos(List<Producto> lista)
    {
        List<Producto> activos = lista.Where(x => x.IdGrid != -1).ToList();

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
        List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>((string)ViewState["productos"]);
        try
        {
            Producto info = lista
                .Where(x => x.IdGrid.ToString() == GProductos.DataKeys[e.RowIndex].Values[0].ToString())
                .ToList().First();
            TIngredienteID.Text = info.id;
            TLoteIngrediente.Text = info.lote;
            TIngrediente.Text = info.NombreCompleto;
            TCantidadIngrediente.Text = info.cantidad.ToString(CultureInfo.InvariantCulture);
            info.IdGrid = -1;
            // lista.Remove(info);

            for (int i = 0; i < lista.Count; i++)
                if (lista[i].IdGrid != -1)
                    lista[i].IdGrid = i;

            UpdateProductos(lista);
        }
        catch { }
        e.Cancel = true;
    }

    protected void GAnalisis_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1)
        {
            for (int i = 3; i < e.Row.Cells.Count-1; i++) 
                e.Row.Cells[i].Text = ((Analisis)e.Row.DataItem).Parametros[GAnalisis.Columns[i].HeaderText];
        }
    }

    protected void GMicro_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1)
        {
            for (int i = 3; i < e.Row.Cells.Count - 1; i++)
            {
                for (int j = 0; j < DMicroSimple.Items.Count; j++) {
                    if (GMicro.Columns[i].HeaderText == DMicroSimple.DataKeys[j].ToString()) {
                        switch(((Analisis)e.Row.DataItem).Parametros[GMicro.Columns[i].HeaderText]) {
                            case "0":
                                e.Row.Cells[i].Text = "Ausente";
                                break;
                            case "1":
                                e.Row.Cells[i].Text = "Presente";
                                break;
                        }
                        break;
                    }
                }
                if (string.IsNullOrEmpty(e.Row.Cells[i].Text))
                {
                    e.Row.Cells[i].Text = ((Analisis)e.Row.DataItem).Parametros[GMicro.Columns[i].HeaderText];
                }
            }
        }
    }
}
