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

    static public void RemoveUbicacion(string idAlmacen)
    {
        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "av" + idAlmacen);
    }

    static public bool GetAlmacenVence(string idAlmacen)
    {
        object retorno = Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "av" + idAlmacen);
        if (retorno == null) {
            using (CConector conector = new CConector())
                retorno = conector.QueryString("SELECT vencido FROM almacen WHERE id=" + Query.GetValor(idAlmacen))=="1";
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "av" + idAlmacen, retorno);
        }
        return (bool)retorno;
    }

    static public void RemoveUbicacion(int ubicacion) {
        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "u" + ubicacion.ToString());
    }
    
    static public string GetNombreUbicacion(int ubicacion) {

        if (ubicacion==0)
            return "(Ninguna)";
        string nombre = (string)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "u" + ubicacion.ToString());
        if(nombre==null) {
            using( CConector conector= new CConector()) 
                nombre = conector.QueryString("SELECT nombre FROM ubicacion WHERE id=" + Query.GetValor(ubicacion.ToString()));
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "u" + ubicacion.ToString(), nombre);
        }
        return nombre;
    }

    static public void RemoveVarios(string estado)
    {
        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "e" + estado);
    }

    static public string GetNombreVarios(string id) {

        if (string.IsNullOrEmpty(id))
            return "";
        string nombre = (string)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "e" + id);
        if (nombre == null)
        {
            using (CConector conector = new CConector())
                nombre = conector.QueryString("SELECT nombre FROM varios WHERE id=" + Query.GetValor(id));
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "e" + id, nombre);
        }
        return nombre;
    }

    static public string GetNombreEstado(int estado)
    {
        if (estado == 0)
            return "(Ninguno)";
        return GetNombreVarios(estado.ToString());
    }
}
