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
[WebService(Namespace = "http://www.sdg.com.co/")]
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

    [WebMethod(EnableSession=true)]
    public List<CJSon> GetTercero(String q, int tipo) {
        List<CJSon> lista = new List<CJSon>();
        CConector conector = new CConector();

        string szTipo = "(0,1,2)";
        switch(tipo) {
            case 1:
                szTipo = "(0,2)";
                break;
            case 0:
                szTipo = "(1,2)";
                break;
        }
        string query = "SELECT id,IF(identificacion IS NULL,nombre, CONCAT(identificacion,'-',nombre)) AS nombre FROM tercero WHERE estado IS NULL AND tipo IN " + szTipo;
        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += String.Format(" AND (identificacion LIKE '%{0}%' OR nombre LIKE '%{0}%' OR descripcion LIKE '%{0}%') ", split[i]);
            }
        }
        query += " ORDER BY nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            while (reader.Read())
            {
                CJSon info = new CJSon(reader["nombre"].ToString(), reader["id"].ToString());
                lista.Add(info);
            }
            reader.Close();
        }
        return lista;
    }

    [WebMethod]
    public List<CJSon> GetUbicacion(String q, string a)
    {
        List<CJSon> lista = new List<CJSon>();
        CConector conector = new CConector();

        string query = string.Format("SELECT id, nombre FROM ubicacion WHERE almacen={0} AND estado IS NULL", a);
        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += String.Format(" AND nombre LIKE '%{0}%'", split[i]);
            }
        }
        query += " ORDER BY nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            while (reader.Read())
            {
                CJSon info = new CJSon(reader["nombre"].ToString(), reader["id"].ToString());
                lista.Add(info);
            }
            reader.Close();
        }
        return lista;
    }

    [WebMethod(EnableSession=true)]
    public List<Producto> GetProducto(String q, String p)
    {
        List<Producto> lista = new List<Producto>();
        CConector conector = new CConector();

        string query = "SELECT id FROM producto WHERE estado IS NULL";
        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += String.Format(" AND (codigo LIKE '%{0}%' OR descripcion LIKE '%{0}%' OR nombre LIKE '%{0}%' OR empresa LIKE '%{0}%') ", split[i]);
            }
        }
        query += " ORDER BY nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            while (reader.Read())
            {
                Producto info = new Producto(reader["id"].ToString());
                info.GetCosto(p);
                lista.Add(info);
            }
            reader.Close();
        }
        return lista;
    }

    [WebMethod]
    public List<Producto> GetProductoSalida(String almacen, bool auto, String q, String productos, string t)
    {
        List<Producto> lista = new List<Producto>();
        CConector conector = new CConector();

        string query;
        if (auto)
            query = String.Format("SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) LEFT JOIN varios ON (varios.id=estado{0}.estado) WHERE estado{0}.cantidad>0 AND varios.parametro1 IS NULL ", almacen);
        else query = String.Format("SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad,estado{0}.lote,IF(estado{0}.vence='0000-00-00',NULL,estado{0}.vence) AS vence,estado{0}.estado,estado{0}.szSerial FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) LEFT JOIN varios ON (varios.id=estado{0}.estado) WHERE estado{0}.cantidad>0 AND varios.parametro1 IS NULL ", almacen);

        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += " AND ";
                query += String.Format(" (producto.codigo LIKE '%{0}%' OR producto.descripcion LIKE '%{0}%' OR producto.nombre LIKE '%{0}%' OR producto.empresa LIKE '%{0}%') ", split[i]);
            }
        }
        if (auto)
            query += String.Format(" GROUP BY estado{0}.producto", almacen);
        else query += String.Format(" GROUP BY estado{0}.producto,estado{0}.lote,estado{0}.vence,estado{0}.estado,estado{0}.szSerial", almacen);
        query += " ORDER BY producto.nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            ProductoUsado productousado = new ProductoUsado(productos);
            while (reader.Read())
            {
                Decimal cantidad = Convert.ToDecimal(reader["cantidad"]);
                Producto info = new Producto(reader["producto"].ToString());
                if (auto)
                {
                    cantidad -= productousado.GetProductoUsado(info.id);
                }
                else
                {
                    info.lote = reader["lote"].ToString();
                    info.vence = CConector.GetFromMySQLDate(reader["vence"]);
                    info.serial = reader["szSerial"].ToString();
                    info.estado = Convert.ToInt32(reader["estado"]);
                    cantidad -= productousado.GetProductoUsado(info.id, info.lote, info.vence);
                }
                if (cantidad > 0.0000001m)
                {
                    info.cantidad = cantidad;
                    info.GetValor(t, Convert.ToInt32(almacen));
                    lista.Add(info);
                }
            }
            reader.Close();
        }
        return lista;
    }

    [WebMethod]
    public List<Producto> GetProductoDisponible(String almacen, bool auto, String q, String productos)
    {
        List<Producto> lista = new List<Producto>();
        CConector conector = new CConector();

        string query;
        if(auto)
            query = String.Format("SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) WHERE estado{0}.cantidad>0 ", almacen);
        else query = String.Format("SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad,estado{0}.lote,IF(estado{0}.vence='0000-00-00',NULL,estado{0}.vence) AS vence,estado{0}.estado,estado{0}.szSerial FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) WHERE estado{0}.cantidad>0 ", almacen);

        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += " AND ";
                query += String.Format(" (codigo LIKE '%{0}%' OR descripcion LIKE '%{0}%' OR producto.nombre LIKE '%{0}%' OR producto.empresa LIKE '%{0}%') ", split[i]);
            }
        }
        if (auto)
            query += String.Format(" GROUP BY estado{0}.producto", almacen);
        else query += String.Format(" GROUP BY estado{0}.producto,estado{0}.lote,estado{0}.vence,estado{0}.estado,estado{0}.szSerial", almacen);
        query += " ORDER BY producto.nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            ProductoUsado productousado = new ProductoUsado(productos);
            while (reader.Read())
            {
                Decimal cantidad = Convert.ToDecimal(reader["cantidad"]);
                Producto info = new Producto(reader["producto"].ToString());
                if (auto)
                {
                    cantidad -= productousado.GetProductoUsado(info.id);
                }
                else
                {
                    info.lote= reader["lote"].ToString();
                    info.vence = CConector.GetFromMySQLDate(reader["vence"]);
                    info.serial = reader["szSerial"].ToString();
                    info.estado = Convert.ToInt32(reader["estado"]);
                    cantidad -= productousado.GetProductoUsado(info.id,info.lote, info.vence);
                }
                if (cantidad > 0.0000001m)
                {
                    info.cantidad = cantidad;
                    lista.Add(info);
                }
            }
            reader.Close();
        }
        return lista;
    }

    [WebMethod(EnableSession=true)]
    public List<Producto> GetProductoTransferible(String almacen, bool auto, String q)
    {
        List<Producto> lista = new List<Producto>();
        CConector conector = new CConector();

        string query;
        if(auto)
            query = String.Format("SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) WHERE estado{0}.cantidad>0 ", almacen);
        else query = String.Format("SELECT estado{0}.producto,estado{0}.cantidad,estado{0}.lote,IF(estado{0}.vence='0000-00-00',NULL,estado{0}.vence) AS vence FROM estado{0} LEFT JOIN producto ON (producto.id=estado{0}.producto) WHERE estado{0}.cantidad>0 ", almacen);

        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += " AND ";
                query += String.Format(" (codigo LIKE '%{0}%' OR descripcion LIKE '%{0}%' OR producto.nombre LIKE '%{0}%' OR producto.empresa LIKE '%{0}%') ", split[i]);
            }
        }

        // verificamos que exista una tabla de transformaciones.
        if (conector.QueryString("SELECT COUNT(*) FROM transformacion") != "0")
            query += string.Format(" AND EXISTS (SELECT transformacion.producto1 FROM transformacion WHERE transformacion.producto1=estado{0}.producto OR transformacion.producto2=estado{0}.producto )", almacen);

        if (auto)
            query += String.Format(" GROUP BY estado{0}.producto", almacen);
        query += " ORDER BY producto.nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            while (reader.Read())
            {
                Producto info = new Producto(reader["producto"].ToString());
                info.cantidad = Convert.ToDecimal(reader["cantidad"]);

                if (auto)
                {
                    
                }
                else
                {
                    info.lote= reader["lote"].ToString();
                    info.vence= CConector.GetFromMySQLDate(reader["vence"]);
                }
                lista.Add(info);
            }
            reader.Close();
        }
        return lista;
    }

    [WebMethod(EnableSession = true)]
    public List<Producto> GetProductoTransferibleDestino(String producto, String q)
    {
        List<Producto> lista = new List<Producto>();
        CConector conector = new CConector();
        if (string.IsNullOrEmpty(producto))
            return lista;

        string query = "SELECT producto.id,IFNULL(IFNULL(transformacion.proporcion,1/transformacion2.proporcion),1) AS proporcion FROM producto " 
            + " LEFT JOIN transformacion ON (transformacion.producto1=" + Query.GetValor(producto) + " AND transformacion.producto2=producto.id)"
            + " LEFT JOIN transformacion AS transformacion2 ON (transformacion2.producto1=producto.id AND transformacion2.producto2=" + Query.GetValor(producto) + ")"
            + " WHERE producto.estado IS NULL AND producto.id!=" + producto;

        // verificamos que exista una tabla de transformaciones.
        if (conector.QueryString("SELECT COUNT(*) FROM transformacion") != "0")
            query += string.Format(" AND (transformacion.producto1 IS NOT NULL OR transformacion2.producto1 IS NOT NULL)");

        string[] split = q.Trim().Split(new Char[] { ' ', '\0' });

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            { // verificamos que no este vacio
                query += String.Format(" AND (codigo LIKE '%{0}%' OR descripcion LIKE '%{0}%' OR producto.nombre LIKE '%{0}%' OR producto.empresa LIKE '%{0}%') ", split[i]);
            }
        }
        query += " ORDER BY producto.nombre LIMIT 50;";
        MySqlDataReader reader = conector.Query(query);
        if (reader != null)
        {
            while (reader.Read())
            {
                Producto info = new Producto(reader["id"].ToString());
                info.cantidad=Convert.ToDecimal(reader["proporcion"].ToString());
                lista.Add(info);
            }
            reader.Close();
        }
        return lista;
    }
}

