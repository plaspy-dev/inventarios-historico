using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using SDG.Componentes;

/// <summary>
/// Descripción breve de servicios
/// </summary>
[WebService(Namespace = "https://service.example.invalid/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
[System.Web.Script.Services.ScriptService]
public class servicios : System.Web.Services.WebService {

    public class CJSon
    {
        public String item { set; get; }
        public String value { set; get; }
        public CJSon()
        {
        }
        public CJSon(String v, String i)
        {
            item = i;
            value = v;
        }
    }

    public servicios () {

        //Eliminar la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    [WebMethod]
    public List<CJSon> GetProducto(String q)
    {
        List<CJSon> lista = new List<CJSon>();
        CConector conector = new CConector();

        string query = "SELECT id,nombre FROM producto WHERE ";
        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                if (i > 0)
                    query += " AND ";
                query += String.Format(" (nombre LIKE '%{0}%' OR descripcion LIKE '%{0}%') ", split[i]);
            }
        }
        query += " ORDER BY nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            while (reader.Read())
            {
                lista.Add(new CJSon(reader["id"].ToString(), reader["nombre"].ToString()));
            }
            reader.Close();
        }
        return lista;
    }

    [WebMethod(EnableSession=true)]
    public List<Producto> GetProductoDisponible(String almacen, bool auto, String q)
    {
        List<Producto> lista = new List<Producto>();
        CConector conector = new CConector();

        string query;
        if(auto)
            query = String.Format("SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad,producto.nombre FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) WHERE estado{0}.cantidad>0 ", almacen);
        else query = String.Format("SELECT estado{0}.producto,estado{0}.cantidad,estado{0}.lote,estado{0}.vence,producto.nombre FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) WHERE estado{0}.cantidad>0 ", almacen);

        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += " AND ";
                query += String.Format(" (producto.nombre LIKE '%{0}%' OR producto.descripcion LIKE '%{0}%') ", split[i]);
            }
        }
        if (auto)
            query += String.Format(" GROUP BY estado{0}.producto", almacen);
        query += " ORDER BY producto.nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            while (reader.Read())
            {
                double cantidad= Convert.ToDouble(reader["cantidad"]);
                if (auto)
                {
                    cantidad -= Producto.GetProductoUsado(reader["producto"].ToString());
                    lista.Add(new Producto(reader["producto"].ToString(), reader["nombre"].ToString(), cantidad));
                }
                else
                {
                    cantidad -= Producto.GetProductoUsado(reader["producto"].ToString(), reader["lote"].ToString()
                                           , CConector.GetFromMySQLDate(reader["vence"]));
                    lista.Add(new Producto(reader["producto"].ToString(), reader["nombre"].ToString()
                                           , cantidad, reader["lote"].ToString()
                                           , CConector.GetFromMySQLDate(reader["vence"])));
                }
            }
            reader.Close();
        }
        return lista;
    }
}

