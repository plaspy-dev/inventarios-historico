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
    static public void RemoveEstado(string estado)
    {
        if (HttpRuntime.Cache["estado" + estado] != null)
            HttpRuntime.Cache.Remove("estado" + estado);
    }

    static public string GetNombreEstado(int estado)
    {
        if (estado == 0)
            return "(Ninguno)";
        string nombre = (string)HttpRuntime.Cache["estado" + estado.ToString()];
        if (nombre == null)
        {
            nombre = new CConector().QueryString("SELECT nombre FROM varios WHERE id=" + Query.GetValor(estado.ToString()));
            HttpRuntime.Cache.Insert("estado" + estado.ToString(), nombre, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        return nombre;
    }
    static public void RemoveTercero(string tercero)
    {
        if (HttpRuntime.Cache["tercero" + tercero] != null)
            HttpRuntime.Cache.Remove("tercero" + tercero);
    }

    static public string GetNombreTercero(string tercero)
    {
        if (string.IsNullOrEmpty(tercero) || tercero == "0")
            return "";
        string nombre = (string)HttpRuntime.Cache["tercero" + tercero];
        if (nombre == null)
        {
            nombre = new CConector().QueryString("SELECT IF(identificacion IS NULL,nombre, CONCAT(identificacion,'-',nombre)) FROM tercero WHERE id=" + Query.GetValor(tercero));
            HttpRuntime.Cache.Insert("tercero" + tercero, nombre, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        return nombre;
    }
}
