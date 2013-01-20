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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Update();
        }
    }

    public void SetModificacion(String modificacionId) {
        
        SetModificacion(modificacionId, null);
    }

    public void SetModificacion(String modificacionId, String modificacionId2)
    {
        CConector conector = CConector.GetConector();
        Query query = new Query("modificaciones");

        query["usuario"] = "usuarios.nombre";
        query["fecha"] = "modificaciones.fecha";
        string url = HttpContext.Current.Request.Url.AbsolutePath.Replace(HttpContext.Current.Request.ApplicationPath, "~");
        string condicion="modificaciones.id=" + Query.GetValor(modificacionId);
        if (!string.IsNullOrEmpty(modificacionId2))
        {
            condicion += " AND modificaciones.id2=" + Query.GetValor(modificacionId2);
        }
        else
        {
            condicion += " AND modificaciones.id2 IS NULL";
        }
        condicion+= " AND modificaciones.url=" + Query.GetValor(url);
        condicion += " ORDER BY modificaciones.fecha";

        MySqlDataReader reader = conector.Query(query.GetSelect(condicion, "LEFT JOIN usuarios ON (usuarios.id=modificaciones.usuario)"));
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
        set { SetModificacion(value); }
    }
}
