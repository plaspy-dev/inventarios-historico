using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using MySql.Data.MySqlClient;
using SDG.Componentes;

public class Alertas
{
    public String Nombre { get; set; }
    public String Numero { get; set; }
    public String URL { get; set; }
    public String SelectCommand { get; set; }
    public String SelectCount { get; set; }

    public Alertas(string nombre, string numero, string url, string select, string selectcount)
    {
        Nombre = nombre;
        Numero = numero;
        SelectCommand = select;
        URL = url;
        SelectCount = selectcount;
    }

    public static void RemoveAlertas(string almacen) {
        HttpRuntime.Cache.Remove("alertas" + almacen);
    }

    public static List<Alertas> GetFullAlertas(string almacen)
    {
        List<Alertas> alertas = (List<Alertas>)HttpRuntime.Cache["alertas" + almacen];
        
        alertas = null;

        if (alertas == null)
        {
            alertas = new List<Alertas>();

            Query query = new Query("alertas");
            
            query+="nombre";
            query += "consulta";
            query += "url";
            query += "consulta_contar";

            CConector conectorc = new CConector();
            CConector conectortemp = new CConector();
            MySqlDataReader consultas = conectorc.Query(query.GetSelect(""));

            if (consultas != null)
            {
                while (consultas.Read())
                {
                    CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

                    string consulta = consultas["consulta"].ToString();
                    string consultacount = consultas["consulta_contar"].ToString();
                    
                    alertas.Add(new Alertas(
                            consultas["nombre"].ToString(),
                            conectortemp.QueryString(string.Format(consultacount, almacen)),
                            consultas["url"].ToString(),
                            consulta,
                            consultacount
                    ));
                }
            }
            HttpRuntime.Cache.Insert("alertas" + almacen, alertas, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        return alertas;
    }

    public static List<Alertas> GetAlertas(string almacen)
    {
        return (from p in GetFullAlertas(almacen)
         where !string.IsNullOrEmpty(p.Numero) && p.Numero != "0"
         select p).ToList();
    }
}
