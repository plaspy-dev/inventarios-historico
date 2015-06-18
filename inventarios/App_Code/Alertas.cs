using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using MySql.Data.MySqlClient;
using SDG.Componentes;

[Serializable]
public class Alertas
{
    public String id { get; set; }
    public String Nombre { get; set; }
    public String Numero { 
        get {
            if (!string.IsNullOrEmpty(szAlmacen))
            {
                using(CConector conector = new CConector()) 
                    _Numero = conector.QueryString(string.Format(SelectCount, szAlmacen));
                szAlmacen = null;
            }
            return _Numero;
        } 
    }
    public String URL { get; set; }
    public String SelectCommand { get; set; }
    public String SelectCount { get; set; }
    public String szAlmacen { get; set;}
    protected String _Numero;

    public Alertas(string szId, string nombre, string url, string select, string selectcount, string almacen)
    {
        id = szId;
        Nombre = nombre;
        SelectCommand = select;
        URL = url;
        SelectCount = selectcount;
        szAlmacen = almacen;
    }

    public static void RemoveAlertas(string almacen) {

        GetFullAlertas(almacen).ForEach( t => t.szAlmacen=almacen );
        GetFullAlertas().ForEach(t => t.szAlmacen = "-1");
    }

    public static List<Alertas> GetFullAlertas() {

        return GetFullAlertas(null);
    }

    public static List<Alertas> GetFullAlertas(string almacen)
    {
        List<Alertas> alertas = (List<Alertas>)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + almacen == null ? "ag" : ("a" + almacen));
        if (alertas == null)
        {
            alertas = new List<Alertas>();

            Query query = new Query("alertas");

            query += "id";
            query+="nombre";
            query += "consulta";
            query += "url";
            query += "consulta_contar";

            using (CConector conector = new CConector())
            {
                MySqlDataReader consultas = conector.Query(query.GetSelect(almacen == null ? "global=1" : "global IS NULL"));

                if (consultas != null)
                {
                    while (consultas.Read())
                    {
                        CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

                        string consulta = consultas["consulta"].ToString();
                        string consultacount = consultas["consulta_contar"].ToString();

                        alertas.Add(new Alertas(consultas["id"].ToString(),
                                consultas["nombre"].ToString(),
                                consultas["url"].ToString(),
                                consulta,
                                consultacount,
                                almacen == null ? "-1" : almacen
                        ));
                    }
                }
            }
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + almacen == null ? "ag" : ("a" + almacen), alertas);
        }
        return alertas;
    }

    public static List<Alertas> GetAlertas(string almacen)
    {
        List<string> alertas = (List<string>)HttpContext.Current.Session["alertas"];
        if(alertas==null) {
            Usuario usuario = Usuario.GetUsuario();
            alertas = new List<string>();
            using (CConector conector = new CConector())
            {
                MySqlDataReader reader = conector.Query(string.Format("SELECT alertas.id"
                                + " FROM alertas"
                                + " LEFT JOIN alertas_grupo ON (alertas_grupo.grupo={1} AND alertas_grupo.alerta=alertas.id)"
                                + " LEFT JOIN alertas_usuario ON (alertas_usuario.usuario={0} AND alertas_usuario.alerta=alertas.id)"
                                + " WHERE (IF({1}=0,1, alertas_grupo.alerta) IS NOT NULL AND (alertas_usuario.estado IS NULL OR alertas_usuario.estado=1))"
                                + " OR (IF({1}=0,1, alertas_grupo.alerta) IS NULL AND (alertas_usuario.estado IS NOT NULL AND alertas_usuario.estado=1))"
                                , usuario.GetID()
                                , usuario.GetGrupo()));
                if (reader != null)
                {
                    while (reader.Read())
                        alertas.Add(reader["id"].ToString());
                }
            }
            HttpContext.Current.Session["alertas"]= alertas;
        }

        List<Alertas> listadoalertas = null;
        listadoalertas = (from p in GetFullAlertas(almacen)
                          where alertas.Contains(p.id) && !string.IsNullOrEmpty(p.Numero) && p.Numero != "0"
                          select p).ToList();

        listadoalertas.AddRange((from p in GetFullAlertas()
                                 where alertas.Contains(p.id) && !string.IsNullOrEmpty(p.Numero) && p.Numero != "0"
                                 select p).ToList());

        return listadoalertas;
    }
}
