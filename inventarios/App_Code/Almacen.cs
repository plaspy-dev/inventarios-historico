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

    static public Dictionary<int, string> GetNombreUbicaciones()
    {
        Dictionary<int, string> info = (Dictionary<int, string>)HttpContext.Current.Session["almacen_ubicaciones_nombre"];
        if (info == null)
            info = new Dictionary<int, string>();
        return info;
    }
    
    static public string GetNombreUbicacion(int ubicacion) {

        if (ubicacion==0)
            return "(Ninguna)";
        Dictionary<int, string> info = GetNombreUbicaciones();
        string nombre = null;
        try
        {
            nombre = info[ubicacion];
        }
        catch
        {
            nombre = new CConector().QueryString("SELECT nombre FROM ubicacion WHERE id=" + Query.GetValor(ubicacion.ToString()));
            info[ubicacion] = nombre;
        }
        return nombre;
    }
}
