using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using SDG.Componentes;
using MySql.Data.MySqlClient;

/// <summary>
/// Descripción breve de CConfiguracion
/// </summary>
public class CConfiguracion
{
    public int nCosto=2;
    public int nCantidad=0;
    public int nLote = 2;
    public int nVence = 2;
    public int nSerial = 2;
    public int nUbicacion = 2;
    public int nEstado = 2;
    public int nDiasVencimiento = 5;

	public CConfiguracion()
	{
	}

    static public CConfiguracion GetConfiguracion() {
        CConfiguracion configuracion = (CConfiguracion)HttpRuntime.Cache["configuracion"];
        if(configuracion==null) {            
            
            CConector conector = new CConector();
            String texto = conector.QueryString("SELECT configuracion FROM configuracion");

            if (string.IsNullOrEmpty(texto))
            {
                configuracion = new CConfiguracion();
            }
            else
            {
                StringReader builder = new StringReader(texto);
                XmlSerializer s = new XmlSerializer(typeof(CConfiguracion));
                configuracion = (CConfiguracion)s.Deserialize(builder);
            }

            HttpRuntime.Cache.Insert("configuracion", configuracion, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        return configuracion;
    }
    public bool SaveConfiguracion() {

        CConector conector = new CConector();
        Query query = new Query("configuracion");

        StringBuilder builder = new StringBuilder();
        XmlSerializer s = new XmlSerializer(this.GetType());
        using (StringWriter writer = new StringWriter(builder))
        {
            s.Serialize(writer, this);
        }
        query["configuracion"]= builder.ToString();

        if (conector.QueryMod("DELETE FROM configuracion") == -1)
            return false;

        if (conector.QueryMod(query.GetInsert()) == -1)
            return false;

        HttpRuntime.Cache.Insert("configuracion", this, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
        return true;
    }
}
