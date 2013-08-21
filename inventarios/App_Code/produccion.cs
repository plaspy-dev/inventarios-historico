using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SDG.Componentes;
using MySql.Data.MySqlClient;

[WebService(Namespace = "http://www.plaspy.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class produccion : System.Web.Services.WebService {

    public produccion () {
        //Eliminar la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public List<Producto> GetProducto(String q)
    {
        List<Producto> lista = new List<Producto>();
        using (CConector conector = new CConector())
        {
            string query = "SELECT producto.id,formulacion.producto AS formulacion FROM formulacion LEFT JOIN producto ON (formulacion.producto=producto.id) WHERE producto.estado IS NULL";
            string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Length > 0)
                { // verificamos que no este vacio
                    query += String.Format(" AND (producto.codigo LIKE '%{0}%' OR producto.descripcion LIKE '%{0}%' OR producto.nombre LIKE '%{0}%' OR producto.empresa LIKE '%{0}%') ", split[i]);
                }
            }
            query += " ORDER BY producto.nombre LIMIT 20;";
            MySqlDataReader reader = conector.Query(query);
            if (reader != null)
            {
                while (reader.Read())
                {
                    Producto info = new Producto(reader["id"].ToString());
                    info.IdTabla = reader["formulacion"].ToString();
                    lista.Add(info);
                }
                reader.Close();
            }
        }
        return lista;
    }

    [WebMethod(EnableSession = true)]
    public List<Miscelaneos> GetOrdenP(String q)
    {
        List<Miscelaneos> lista = new List<Miscelaneos>();
        using (CConector conector = new CConector())
        {
            string query = "SELECT orden_produccion.id,CONCAT(orden_produccion.planeacion,' - ',orden_produccion.lote,' - ',producto.nombre) AS nombre FROM orden_produccion LEFT JOIN producto ON (orden_produccion.producto=producto.id) WHERE producto.estado IS NULL";
            string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Length > 0)
                { // verificamos que no este vacio
                    query += String.Format(" AND (orden_produccion.planeacion LIKE '%{0}%' OR orden_produccion.lote LIKE '%{0}%' OR producto.codigo LIKE '%{0}%' OR producto.descripcion LIKE '%{0}%' OR producto.nombre LIKE '%{0}%' OR producto.empresa LIKE '%{0}%') ", split[i]);
                }
            }
            query += " ORDER BY orden_produccion.planeacion DESC LIMIT 20;";
            MySqlDataReader reader = conector.Query(query);
            if (reader != null)
            {
                while (reader.Read())
                {
                    Miscelaneos info = new Miscelaneos(reader["id"].ToString(), reader["nombre"].ToString());
                    lista.Add(info);
                }
                reader.Close();
            }
        }
        return lista;
    }
}

