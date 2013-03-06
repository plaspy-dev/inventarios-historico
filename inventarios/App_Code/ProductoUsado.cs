using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using SDG.Componentes;

/// <summary>
/// Descripción breve de ProductoUsado
/// </summary>
public class ProductoUsado
{
    protected List<Producto>pProductos;

    public ProductoUsado() {
        pProductos = new List<Producto>();
    }

	public ProductoUsado(string productosserializados)
	{
        if (string.IsNullOrEmpty(productosserializados))
        {
            pProductos = new List<Producto>();
        }
        else
        {
            productosserializados = Crypt.DesencriptarDES(productosserializados, "InvenTar");
            StringReader builder = new StringReader(productosserializados);
            XmlSerializer s = new XmlSerializer(typeof(List<Producto>));
            pProductos = (List<Producto>)s.Deserialize(builder);
        }
	}

    public string Serialize()
    {
        if (pProductos.Count == 0)
            return "";
        StringBuilder builder = new StringBuilder();
        XmlSerializer s = new XmlSerializer(pProductos.GetType());
        using (StringWriter writer = new StringWriter(builder))
        {
            s.Serialize(writer, pProductos);
        }
        string myXml = builder.ToString();
        return Crypt.EncriptarDES(myXml, "InvenTar");
    }

    public List<Producto> GetProductosUsados()
    {
        return pProductos;
    }

    public Decimal GetProductoUsado(string producto)
    {
        var resultado = from p in pProductos
                        where p.id == producto
                        select p;
        return resultado.Sum(p => p.cantidad);
    }
    public Decimal GetProductoUsado(string producto, string lote, string fecha, int ubicacion)
    {
        var resultado = from p in pProductos
                        where p.id == producto && p.lote == lote && p.vence == fecha && p.ubicacion==ubicacion
                        select p;
        return resultado.Sum(p => p.cantidad);
    }

    public Decimal GetProductoUsado(string producto, string lote, string fecha)
    {
        var resultado = from p in pProductos
                        where p.id == producto && p.lote == lote && p.vence == fecha
                        select p;
        return resultado.Sum(p => p.cantidad);
    }

    public void DeleteProducto(String producto, Decimal cantidad, string lote, string vencimiento, int ubicacion)
    {
        pProductos.RemoveAll(p => p.id == producto && p.cantidad == cantidad && p.lote == lote && p.vence == vencimiento && p.ubicacion == ubicacion);
    }

    public ArrayList GetProductos(String producto, Decimal cantidad, String almacen, string lote, string vencimiento)
    {
        ArrayList productos = new ArrayList();
        CConector conector = new CConector();

        string szLote;
        if (lote.Length == 0)
            szLote = "''";
        else szLote = Query.GetValor(lote);
        string szVencimiento;
        if (vencimiento.Length == 0)
            szVencimiento = "'0000-00-00'";
        else szVencimiento = "STR_TO_DATE(" + Query.GetValor(vencimiento) + ", '%d/%m/%Y')";

        string szDisponible = conector.QueryString(string.Format("SELECT SUM(cantidad) FROM estado{0} WHERE producto=" + Query.GetValor(producto) + " AND lote=" + szLote + " AND vence=" + szVencimiento, almacen));
        Decimal dCantidad = Convert.ToDecimal(szDisponible);
        dCantidad -= GetProductoUsado(producto, lote, vencimiento);

        if (dCantidad < cantidad)
            return null;

        MySqlDataReader reader = conector.Query(string.Format("SELECT cantidad,ubicacion FROM estado{0} WHERE producto=" + Query.GetValor(producto) + " AND lote=" + szLote + " AND vence=" + szVencimiento, almacen));
        if (reader != null)
        {
            while (reader.Read())
            {
                dCantidad = Convert.ToDecimal(reader["cantidad"]);

                int ubicacion = Convert.ToInt32(reader["ubicacion"]);
                object vence = null;
                try { vence = reader["vence"]; }
                catch { }

                dCantidad -= GetProductoUsado(producto, lote, CConector.GetFromMySQLDate(vence), ubicacion);
                if (dCantidad == 0)
                    continue;
                if (dCantidad > cantidad)
                    dCantidad = cantidad;
                productos.Add(new Producto(producto, dCantidad, lote, vencimiento, ubicacion));
                cantidad -= dCantidad;
                if (cantidad == 0)
                    break;
            }
        }

        if (cantidad > 0 || productos.Count == 0)
            return null;

        List<Producto> productosalmacenados = GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {

            productosalmacenados.Add((Producto)productos[i]);
        }

        return productos;
    }

    public ArrayList GetProductos(String producto, Decimal cantidad, String almacen)
    {

        ArrayList productos = new ArrayList();
        CConector conector = new CConector();

        string szDisponible = conector.QueryString(string.Format("SELECT SUM(cantidad) FROM estado{0} WHERE producto=" + Query.GetValor(producto), almacen));
        Decimal dCantidad = Convert.ToDecimal(Convert.ToDecimal(szDisponible));
        if (cantidad == -1)
            cantidad = dCantidad;
        else dCantidad -= GetProductoUsado(producto);
        if (dCantidad < cantidad)
            return null;

        MySqlDataReader reader = conector.Query(string.Format("SELECT cantidad,lote,vence,ubicacion FROM estado{0} WHERE producto=" + Query.GetValor(producto) + " AND cantidad>0 ORDER BY vence", almacen));
        if (reader != null)
        {
            while (reader.Read())
            {
                dCantidad = Convert.ToDecimal(reader["cantidad"]);
                int ubicacion= Convert.ToInt32(reader["ubicacion"]);
                object vence = null;
                try { vence = reader["vence"]; }
                catch { }

                dCantidad -= GetProductoUsado(producto, reader["lote"].ToString(), CConector.GetFromMySQLDate(vence), ubicacion);
                if (dCantidad <= 0)
                    continue;
                if (dCantidad > cantidad)
                    dCantidad = cantidad;
                productos.Add(new Producto(producto, dCantidad, reader["lote"].ToString(), CConector.GetFromMySQLDate(vence), ubicacion));
                cantidad -= dCantidad;
                if (cantidad == 0)
                    break;
            }
        }
        if (cantidad > 0 || productos.Count == 0)
            return null;

        List<Producto> productosalmacenados = GetProductosUsados();
        for (int i = 0; i < productos.Count; i++)
        {
            productosalmacenados.Add((Producto)productos[i]);
        }
        return productos;
    }
}
