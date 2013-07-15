using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class produccion_laboratorio : System.Web.UI.Page
{
    [Serializable]
    class Analisis {

        public string IdTabla { get; set; }
        public int IdGrid { get; set; }
        public DateTime Fecha { get; set; }
        public string PesoLavado { get; set; }
        public string PesoEscurrido { get; set; }
        public string Brix { get; set; }
        public string Acidez { get; set; }
        public string Viscosidad { get; set; }
        public string Consistencia { get; set; }
        public string PH { get; set; }
        public string Densidad { get; set; }
        public string ChartColor { get; set; }
        public string Observaciones { get; set; }

        public Analisis(int idGrid) {
            IdGrid = idGrid;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        // ((ing)Master)._ImprimirClicked += new EventHandler(OnImprimir);
        if (!IsPostBack)
        {
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
                    /*query += "tercero";
                    query += "cantidad";
                    query += "observaciones";
                    query += "formulacion";*/

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["oi"])));
                    if (reader != null && reader.Read())
                    {
                        Session["parametros"] = Request.Url.Query;
                        PBordes.Visible = false;
                        TAnalisis.Visible= PAnalisis.Visible = true;
                        ViewState["id"]= TNumero.Text = reader["id"].ToString();

                        GAnalisis.Columns[0].Visible= 
                        BAceptar.Enabled = usuario.CanUpdate();
                        GAnalisis.Columns[1].Visible= usuario.CanDelete();

                        ((ing)Master).SetModificacion(TNumero.Text);
                        // ((ing)Master).SetPrintFile("orden.html");
                        
                        TLote.Text = reader["lote"].ToString();
                        LPlaneacion.Text = Conector.GetFromMySQLDate(reader["planeacion"]);
                        LProduccion.Text = Conector.GetFromMySQLDate(reader["produccion"]);
                        Producto producto = new Producto(reader["producto"].ToString());
                        LProducto.Text = producto.NombreCompleto;

                        query = new Query("analisis_fisico_quimico");

                        query += "id";
                        query+="fecha";
                        query+="peso_lavado";
                        query+="peso_escurrido";
                        query+="brix";
                        query+="acidez";
                        query+="viscosidad";
                        query+="consistencia";
                        query+="ph";
                        query+="densidad";
                        query+="chart_color";
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
                                info.Brix = reader["brix"].ToString();
                                info.Acidez = reader["acidez"].ToString();
                                info.Viscosidad = reader["viscosidad"].ToString();
                                info.Consistencia = reader["consistencia"].ToString();
                                info.PH = reader["ph"].ToString();
                                info.Densidad = reader["densidad"].ToString();
                                info.ChartColor = reader["chart_color"].ToString();
                                info.Observaciones = reader["observaciones"].ToString();
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
        info.Brix = TBrix.Text;
        info.Acidez = TAcidez.Text;
        info.Viscosidad = TViscosidad.Text;
        info.Consistencia = TConsistencia.Text;
        info.PH = TPH.Text;
        info.Densidad = TDensidad.Text;
        info.ChartColor = TChartColor.Text;

        HAnalisis.Value = Serializar.SerializeBinary(lista);
        GAnalisis.DataSource = lista.Where(x => x.IdGrid != -1).ToList();
        GAnalisis.DataBind();
        BCancelarAgregar_Clicked(sender, e);
    }

    protected void BCancelarAgregar_Clicked(object sender, EventArgs e) {

        GAnalisis.SelectedIndex = -1;
        BAgregar.Text = "Agregar";
        BCancelarAgregar.Visible = false;
        TFecha.Text =
            TPesoEscurrido.Text = TPesoLavado.Text =
            TObservaciones.Text =
            TBrix.Text = TAcidez.Text =
            TViscosidad.Text = TConsistencia.Text =
            TPH.Text = TDensidad.Text =
            TChartColor.Text = "";
    }

    protected void BAceptar_Clicked(object sender, EventArgs e) {

        if (!IsValid)
            return;

        if (GAnalisis.Rows.Count == 0)
        {
            LMensaje.Text = "Por favor ingrese por lo menos un análisis";
            TBrix.Focus();
            return;
        }

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();

            List<Analisis> lista = Serializar.DesSerializeBinary<List<Analisis>>(HAnalisis.Value);

            for (int i = 0; i < lista.Count; i++) {

                Query query = new Query("analisis_fisico_quimico");
                if (lista[i].IdGrid != -1)
                {
                    query["fecha"] = Conector.GetMySQLDateFormat(lista[i].Fecha);
                    query["peso_lavado"] = lista[i].PesoLavado;
                    query["peso_escurrido"] = lista[i].PesoEscurrido;
                    query["brix"] = lista[i].Brix;
                    query["acidez"] = lista[i].Acidez;
                    query["viscosidad"] = lista[i].Viscosidad;
                    query["consistencia"] = lista[i].Consistencia;
                    query["ph"] = lista[i].PH;
                    query["densidad"] = lista[i].Densidad;
                    query["chart_color"] = lista[i].ChartColor;
                    query["observaciones"] = lista[i].Observaciones;

                    if (string.IsNullOrEmpty(lista[i].IdTabla))
                    {

                        query["orden"] = ViewState["id"].ToString();
                        if (conector.QueryMod(query.GetInsert()) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                    else
                    {
                        if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(lista[i].IdTabla))) == -1)
                        {
                            LMensaje.Text = conector.GetLastError();
                            return;
                        }
                    }
                } else {
                    if (!string.IsNullOrEmpty(lista[i].IdTabla)) {
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
            Session["parametros"] = "oi=" + TNumero.Text;
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

        TFecha.Text= info.Fecha.ToShortDateString();
        TPesoLavado.Text= info.PesoLavado;
        TPesoEscurrido.Text= info.PesoEscurrido;
        TObservaciones.Text= info.Observaciones;
        TBrix.Text= info.Brix;
        TAcidez.Text= info.Acidez;
        TViscosidad.Text= info.Viscosidad;
        TConsistencia.Text= info.Consistencia;
        TPH.Text= info.PH;
        TDensidad.Text= info.Densidad;
        TChartColor.Text= info.ChartColor;

        BAgregar.Text = "Editar";
        BCancelarAgregar.Visible = true;
    }
}
