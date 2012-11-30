using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Collections;
using SDG.Componentes;

public partial class modificaciones : System.Web.UI.UserControl
{
    class Mods
    {
        protected string _usuario;
        protected string _fecha;

        public Mods(string usuario, string fecha)
        {
            _fecha = fecha;
            _usuario = usuario;
        }

        public string Usuario { get { return _usuario; } }
        public string Fecha { get { return _fecha; } }
    }

    protected string _idModificacion="";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Update();
        }
    }

    public void Update()
    {
        CConector conector = CConector.GetConector();
        Query query = new Query("modificaciones");

        query["usuario"] = "usuarios.nombre";
        query["fecha"] = "modificaciones.fecha";
        string url = HttpContext.Current.Request.Url.AbsolutePath.Replace(HttpContext.Current.Request.ApplicationPath, "~");
        MySqlDataReader reader = conector.Query(query.GetSelect("modificaciones.id='" + IdModificacion + "' AND modificaciones.url='" + url + "' ORDER BY modificaciones.fecha", "LEFT JOIN usuarios ON (usuarios.id=modificaciones.usuario)"));
        if (reader != null)
        {
            PModificaciones.Visible = false;
            if (reader.Read())
            {
                PModificaciones.Visible = true;
                GModificaciones.EmptyDataText = "";
                LCreadoPor.Text = reader["usuario"].ToString();
                LFechaCreacion.Text = reader["fecha"].ToString();
            }
            ArrayList lista = new ArrayList();
            while (reader.Read())
            {
                if (LModificadoPor.Text.Length != 0)
                {
                    lista.Add(new Mods(LModificadoPor.Text, LFechaModificacion.Text));
                }
                LModificadoPor.Text = reader["usuario"].ToString();
                LFechaModificacion.Text = reader["fecha"].ToString();
            }
            GModificaciones.DataSource = lista;
            GModificaciones.DataBind();
        }
    }

    public string IdModificacion
    {
        set { _idModificacion = value; Update(); }
        get { return _idModificacion; }
    }
}
