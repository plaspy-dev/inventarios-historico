using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDG.Componentes;

/// <summary>
/// Descripción breve de Almacen
/// </summary>
public class Almacen
{
    static public string []TablaAlmacenes= {"entrada", "entrada_items", "estado", "traslado","traslado_items","salida","salida_items", "transformacion_salida", "transformacion_entrada"};

    static public string ReplaceAlmacenTables(string query, string newAlmacen, string oldAlmacen)
    {
        query = query.Replace("%almacen%", newAlmacen);
        for (int i = 0; i < TablaAlmacenes.Length; i++)
        {
            query = query.Replace(TablaAlmacenes[i] + oldAlmacen, TablaAlmacenes[i] + newAlmacen);
        }
        return query;
    }

    static public string ReplaceAlmacenTables(string query, string newAlmacen) {

        return ReplaceAlmacenTables(query, newAlmacen, "0");
    }

    static public string GetAllAlamcenes(string query, ArrayList idAlmacenes) {

        string full="";
        for (int i = 0; i < idAlmacenes.Count; i++) {
            if (full.Length != 0)
                full += " UNION ALL ";
            full += "("
                + ReplaceAlmacenTables(query, idAlmacenes[i].ToString())
                +")";
        }
        return full;
    }

    static public void RemoveUbicacion(int ubicacion) {
        if (HttpRuntime.Cache["ubicacion" + ubicacion.ToString()] != null)
            HttpRuntime.Cache.Remove("ubicacion" + ubicacion.ToString());
    }
    
    static public string GetNombreUbicacion(int ubicacion) {

        if (ubicacion==0)
            return "(Ninguna)";
        string nombre= (string)HttpRuntime.Cache["ubicacion" + ubicacion.ToString()];
        if(nombre==null) {
            nombre = new CConector().QueryString("SELECT nombre FROM ubicacion WHERE id=" + Query.GetValor(ubicacion.ToString()));
            HttpRuntime.Cache.Insert("ubicacion" + ubicacion.ToString(), nombre, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        return nombre;
    }
}
