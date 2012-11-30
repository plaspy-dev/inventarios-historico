using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using MySql.Data.MySqlClient;
using SDG.Componentes;

/// <summary>
/// Descripción breve de Producto
/// </summary>
public class Producto
{
    public String producto { set; get; }
    public String nombre { set; get; }
    public String lote { set; get; }
    public String vence { set; get; }
    public double cantidad { set; get; }
    public Producto()
    {
    }
    public Producto(String _producto, String _nombre, double _cantidad, String _lote, String _fecha)
    {
        producto = _producto;
        nombre = _nombre;
        cantidad = _cantidad;
        lote = _lote;
        vence = _fecha;
    }
    public Producto(String _producto, String _nombre, double _cantidad)
    {
        producto = _producto;
        nombre = _nombre;
        cantidad = _cantidad;
    }

    public static void Clean() {

        HttpContext.Current.Session["producto_temp"] = new List<Producto>();
    }

    public static List<Producto> GetProductosUsados() {
        
        List<Producto> productos = (List<Producto>)HttpContext.Current.Session["producto_temp"];
        if(productos==null) {
            productos= new List<Producto>();
            HttpContext.Current.Session["producto_temp"]= productos;
        }
        return productos;
    }

    public static double GetProductoUsado(string producto) {
        
        List<Producto> productos = GetProductosUsados();
        var resultado = from p in productos
                        where p.producto==producto
                        select p;
        return resultado.Sum(p => p.cantidad);
    }

    public static double GetProductoUsado(string producto, string lote, string fecha)
    {
        List<Producto> productos = GetProductosUsados();
        var resultado = from p in productos
                        where p.producto == producto && p.lote==lote && p.vence==fecha
                        select p;
        return resultado.Sum(p => p.cantidad);
    }
    
    public static void DeleteProducto(String producto, double cantidad, string lote, string vencimiento)
    {
        List<Producto> productos = GetProductosUsados();
        productos.RemoveAll(p => p.producto == producto && p.cantidad == cantidad && p.lote == lote && p.vence == vencimiento);
    }

    public static ArrayList GetProductos(String producto, double cantidad, String almacen, string lote, string vencimiento)
    {
        ArrayList productos = new ArrayList();
        CConector conector = new CConector();

        string szDisponible = conector.QueryString(string.Format("SELECT SUM(cantidad) FROM estado{0} WHERE producto=" + Query.GetValor(producto) + " AND lote=" + Query.GetValor(lote) + " AND vence=STR_TO_DATE(" + Query.GetValor(vencimiento) + ", '%d/%m/%Y')", almacen));
        double dCantidad = Convert.ToDouble(szDisponible);
        dCantidad -= GetProductoUsado(producto, lote, vencimiento);

        if (dCantidad < cantidad)
            return null;

        productos.Add(new Producto(producto, "", cantidad, lote, vencimiento));

        List<Producto> productosalmacenados = GetProductosUsados();
        for (int i = 0; i < productos.Count; i++ ) {

            productosalmacenados.Add((Producto)productos[i]);
        }

        return productos;
    }

    public static ArrayList GetProductos(String producto, double cantidad, String almacen) {

        ArrayList productos = new ArrayList();
        CConector conector = new CConector();

        string szDisponible= conector.QueryString(string.Format("SELECT SUM(cantidad) FROM estado{0} WHERE producto=" + Query.GetValor(producto), almacen));
        double dCantidad=Convert.ToDouble(szDisponible);
        dCantidad -= GetProductoUsado(producto);
        if (dCantidad < cantidad)
            return null;
        
        MySqlDataReader reader = conector.Query(string.Format("SELECT cantidad,lote,vence FROM estado{0} WHERE producto=" + Query.GetValor(producto) + " AND cantidad>0 ORDER BY vence", almacen));
        if(reader!=null) {
            while(reader.Read()) {
                
                dCantidad= Convert.ToDouble(reader["cantidad"]);
                dCantidad -= GetProductoUsado(producto, reader["lote"].ToString(), CConector.GetFromMySQLDate(reader["vence"]));
                if (dCantidad == 0)
                    continue;
                if(dCantidad>cantidad) 
                    dCantidad = cantidad;
                productos.Add(new Producto(producto, "", dCantidad, reader["lote"].ToString(), CConector.GetFromMySQLDate(reader["vence"])));
                cantidad-= dCantidad;
                if (cantidad == 0)
                    break;
            }
        }
        if (cantidad > 0 || productos.Count==0)
            return null;

        List<Producto> productosalmacenados = GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            productosalmacenados.Add((Producto)productos[i]);
        }
        return productos;
    }
}