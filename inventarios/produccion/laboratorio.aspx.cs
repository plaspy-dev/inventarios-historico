using System;
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
        public string PesoLavado { get; set; }
        public string PesoEscurrido { get; set; }
        public string Observaciones { get; set; }
        public string Brix { get { try { return Parametros["Brix"]; } catch { } return null; } }
        public string Acidez { get { try { return Parametros["Acidez"]; } catch { } return null; } }
        public string Viscosidad { get { try { return Parametros["Viscosidad"]; } catch { } return null; } }
        public string Consistencia { get { try { return Parametros["Consistencia"]; } catch { } return null; } }
        public string PH { get { try { return Parametros["pH"]; } catch { } return null; } }
        public string Densidad { get { try { return Parametros["Densidad"]; } catch { } return null; } }
        public string ChartColor { get { try { return Parametros["Chart-Color"]; } catch { } return null; } }
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
            TFecha.Enabled = Usuario.GetUsuario().CanUpdate();
            TFecha.Text = DateTime.Now.ToShortDateString();
            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
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

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["oi"])));
                    if (reader != null && reader.Read())
                    {
                        Session["parametros"] = Request.Url.Query;
                        PBordes.Visible = false;
                        PAnalisis.Visible = true;
                        ViewState["id"]= TNumero.Text = reader["id"].ToString();
                        LCantidad.Text = Convert.ToDecimal(reader["cantidad"]).ToString("0.#########", CultureInfo.InvariantCulture) + " Kg.";

                        GAnalisis.Columns[0].Visible= 
                        BAceptar.Enabled = usuario.CanUpdate();
                        GAnalisis.Columns[1].Visible= usuario.CanDelete();

                        ((ing)Master).SetModificacion(TNumero.Text);
                        ((ing)Master).SetPrintFile("laboratorio.html");
                        
                        TLote.Text = reader["lote"].ToString();
                        LPlaneacion.Text = Conector.GetFromMySQLDate(reader["planeacion"]);
                        LProduccion.Text = Conector.GetFromMySQLDate(reader["produccion"]);
                        Producto producto = new Producto(reader["producto"].ToString());
                        LProducto.Text = producto.NombreCompleto;
                        LCliente.Text= producto.Empresa;
                        SAnalisis.SelectParameters["producto"].DefaultValue = producto.id;

                        query = new Query("analisis_fisico_quimico");

                        query += "id";
                        query+="fecha";
                        query+="peso_lavado";
                        query+="peso_escurrido";
                        query+="observaciones";

                        List<Analisis> lista = new List<Analisis>();

                        reader = conector.Query(query.GetSelect("orden=" + Query.GetValor(TNumero.Text) + " ORDER BY fecha DESC"));
                        if (reader != null) {
                            while (reader.Read()) {

                                Analisis info = new Analisis(lista.Count);
                                info.IdTabla = reader["id"].ToString();
                                info.Fecha = (DateTime) reader["fecha"];
                                info.PesoLavado = reader["peso_lavado"].ToString();
                                info.PesoEscurrido = reader["peso_escurrido"].ToString();
                                info.Observaciones = reader["observaciones"].ToString();
                                AddParametros(info, reader["id"].ToString());
                                lista.Add(info);
                            }
                        }

                        HAnalisis.Value = Serializar.SerializeBinary(lista);
                        GAnalisis.DataSource = lista.Where(x => x.IdGrid != -1).ToList();
                        GAnalisis.DataBind();

                        return;
                    }
                }
            }
            TNumero.Focus();
        }
    }

    public void AddParametros(Analisis info, string analisis)
    {
        using (CConector conector = new CConector()) {
            Query query = new Query("analisis_fisico_quimico_parametros");
            query += "valor";
            query += "parametro";
            MySqlDataReader reader= conector.Query(query.GetSelect("analisis=" + analisis));
            if (reader != null) {
                while (reader.Read()) 
                    info.Parametros.Add(Almacen.GetNombreVarios(reader["parametro"].ToString()), reader["valor"]==DBNull.Value ? "" : Convert.ToDouble(reader["valor"]).ToString(CultureInfo.InvariantCulture));
            }
        }
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
        info.PesoLavado = TPesoLavado.Text;
        info.PesoEscurrido = TPesoEscurrido.Text;
        info.Observaciones = TObservaciones.Text;

        for(int i=0; i<DAnalisis.Items.Count; i++) {
            TextBox text = (TextBox)DAnalisis.Items[i].FindControl("TValor");
            info.Parametros[DAnalisis.DataKeys[i].ToString()] = text.Text;
        }

        DAnalisis.DataBind();
        HAnalisis.Value = Serializar.SerializeBinary(lista);
        GAnalisis.DataSource = lista.Where(x => x.IdGrid != -1).ToList();
        GAnalisis.DataBind();
        BCancelarAgregar_Clicked(sender, e);
    }

    protected void BCancelarAgregar_Clicked(object sender, EventArgs e) {

        GAnalisis.SelectedIndex = -1;
        TFecha.Text = DateTime.Now.ToShortDateString();
        BAgregar.Text = "Agregar";
        BCancelarAgregar.Visible = false;
        TFecha.Text =
            TPesoEscurrido.Text = TPesoLavado.Text =
            TObservaciones.Text ="";
        SAnalisis.SelectParameters["analisis"].DefaultValue = "0";
    }

    protected void BAceptar_Clicked(object sender, EventArgs e) {

        if (!IsValid)
            return;

        if (GAnalisis.Rows.Count == 0)
        {
            LMensaje.Text = "Por favor ingrese por lo menos un análisis";
            // TBrix.Focus();
            return;
        }

        // constuimos los ids y los equivalentes
        Dictionary<string, string> equivalentes = new Dictionary<string, string>();
        for (int j = 0; j < DAnalisis.Items.Count; j++)
            equivalentes[DAnalisis.DataKeys[j].ToString()] = ((HiddenField)DAnalisis.Items[j].FindControl("HId")).Value;

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();

            List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HAnalisis.Value);

            for (int i = 0; i < lista.Count; i++) {

                Query query = new Query("analisis_fisico_quimico");
                if (lista[i].IdGrid != -1)
                {
                    string id=lista[i].IdTabla;

                    query["fecha"] = Conector.GetMySQLDateFormat(lista[i].Fecha);
                    query["peso_lavado"] = lista[i].PesoLavado;
                    query["peso_escurrido"] = lista[i].PesoEscurrido;
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
            GAnalisis.DataSource = lista.Where(x => x.IdGrid != -1).ToList();
            GAnalisis.DataBind();
        }
        catch { }
        e.Cancel = true;
    }

    protected void GAnalisis_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HAnalisis.Value);
        Analisis info = lista
                .Where(x => x.IdGrid.ToString() == GAnalisis.DataKeys[GAnalisis.SelectedIndex].Values["IdGrid"].ToString())
                .ToList().First();

        SAnalisis.SelectParameters["analisis"].DefaultValue = info.IdTabla;

        TFecha.Text= info.Fecha.ToShortDateString();
        TPesoLavado.Text= info.PesoLavado;
        TPesoEscurrido.Text= info.PesoEscurrido;
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

    protected void OnImprimir(object sender, EventArgs e)
    {
        GAnalisis.Columns[0].Visible = false;
        GAnalisis.Columns[1].Visible = false;
    }
}
