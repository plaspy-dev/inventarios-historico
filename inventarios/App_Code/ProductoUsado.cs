using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using SDG.Componentes;

[Serializable]
public class ProductoUsado
{
    protected List<Producto>pProductos;
    protected List<Producto> pProductosEliminados;

    public ProductoUsado() {
        pProductos = new List<Producto>();
        pProductosEliminados = new List<Producto>();
    }

    private static byte[] Compress(byte[] bytes)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (DeflateStream gzs = new DeflateStream(ms, CompressionMode.Compress, false))
            {
                gzs.Write(bytes, 0, bytes.Length);
            }
            ms.Close();
            return ms.ToArray();
        }
    }

    private static byte[] Decompress(byte[] bytes)
    {
        using (MemoryStream ms = new MemoryStream(bytes, false))
        {
            using (DeflateStream gzs = new DeflateStream(ms, CompressionMode.Decompress, false))
            {
                using (MemoryStream dest = new MemoryStream())
                {
                    byte[] tmp = new byte[bytes.Length];
                    int read;
                    while ((read = gzs.Read(tmp, 0, tmp.Length)) != 0)
                    {
                        dest.Write(tmp, 0, read);
                    }
                    dest.Close();
                    return dest.ToArray();
                }
            }
        }
    }

	public ProductoUsado(string productosserializados)
	{
        if (!string.IsNullOrEmpty(productosserializados))
        {
            try
            {
                productosserializados = UTF8Encoding.UTF8.GetString(Decompress(Convert.FromBase64String(productosserializados)));
                StringReader builder = new StringReader(productosserializados);
                XmlSerializer s = new XmlSerializer(typeof(List<Producto>));
                pProductos = (List<Producto>)s.Deserialize(builder);
                return;
            } catch {}
        }
        pProductos = new List<Producto>();
        pProductosEliminados = new List<Producto>();
	}

    public string SerializeBinary() {

        MemoryStream builder = new MemoryStream();
        BinaryFormatter bFormatter = new BinaryFormatter();

        bFormatter.Serialize(builder, this);
        builder.Flush();
        builder.Position = 0;

        return Convert.ToBase64String(Compress(builder.ToArray()));
    }

    public static ProductoUsado DesSerialize(string variable) {

        if (!string.IsNullOrEmpty(variable)) {
            MemoryStream builder = new MemoryStream(Decompress(Convert.FromBase64String(variable)));
            BinaryFormatter deserializer = new BinaryFormatter();

            ProductoUsado info = (ProductoUsado)deserializer.Deserialize(builder);

            builder.Close();

            return info;
        }
        return new ProductoUsado();
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
        return Convert.ToBase64String(Compress(UTF8Encoding.UTF8.GetBytes(myXml)));
    }

    public List<Producto> GetProductosUsados()
    {
        return pProductos;
    }
    public List<Producto> GetProductoEliminado() {
        
        return pProductosEliminados;
    }
    public void AddProducto(Producto producto) {
        producto.IdGrid = 1;
        if (pProductos.Count > 0)
            producto.IdGrid = pProductos[pProductos.Count - 1].IdGrid + 1;
        pProductos.Add(producto);
    }

    public Decimal GetProductoUsado(string producto)
    {
        var resultado = from p in pProductos
                        where p.id == producto
                        select p;
        return resultado.Sum(p => p.cantidad);
    }
    public Decimal GetProductoUsado(string producto, string lote, string fecha, int ubicacion, int estado)
    {
        var resultado = from p in pProductos
                        where p.id == producto && p.lote == lote && p.vence == fecha && p.ubicacion==ubicacion && p.estado==estado
                        select p;
        return resultado.Sum(p => p.cantidad);
    }

    public Decimal GetTotalProductos()
    {
        return pProductos.Sum(p => p.cantidad);
    }

    public Decimal GetCostoTotal()
    {
        return pProductos.Sum(p => p.CostoTotal);
    }
    
    public Decimal GetValorTotal()
    {
        return pProductos.Sum(p => p.ValorTotal);
    }

    public Decimal GetProductoUsado(string producto, string lote, string fecha)
    {
        var resultado = from p in pProductos
                        where p.id == producto && p.lote == lote && p.vence == fecha
                        select p;
        return resultado.Sum(p => p.cantidad);
    }

    public bool UpdateProductoIngreso(int idGrid, bool ingreso) {
        
        bool actualizado=false;
        List<Producto> info = (from p in pProductos
                               where p.IdGrid == idGrid 
                               && p.bIngreso != ingreso 
                               && (p.pProductoAntiguo == null || p.pProductoAntiguo.bIngreso != ingreso)
                               select p).ToList();
        for (int i = 0; i < info.Count; i++)
        {
            if(!string.IsNullOrEmpty(info[i].IdTabla) && info[i].pProductoAntiguo==null) 
                info[i].pProductoAntiguo = info[i].CopiarProducto();
            info[i].bIngreso = ingreso;
            actualizado = true;
        }
        return actualizado;
    }

    public bool UpdateProductoUbicacion(int idGrid, int ubicacion)
    {

        bool actualizado = false;
        List<Producto> info = (from p in pProductos
                               where p.IdGrid == idGrid
                               && p.ubicacion != ubicacion
                               && (p.pProductoAntiguo == null || p.pProductoAntiguo.ubicacion != ubicacion)
                               select p).ToList();
        for (int i = 0; i < info.Count; i++)
        {
            if (!string.IsNullOrEmpty(info[i].IdTabla) && info[i].pProductoAntiguo == null)
                info[i].pProductoAntiguo = info[i].CopiarProducto();
            info[i].ubicacion = ubicacion;
            actualizado = true;
        }
        return actualizado;
    }

    public bool UpdateProducto(int idGrid, Producto nuevo_producto)
    {
        bool actualizado=false;
        // solo guardamos los que tienen un id en la tabla
        List<Producto> info = (from p in pProductos
                               where p.IdGrid == idGrid
                               select p).ToList();

        for (int i = 0; i < info.Count; i++) {
            if (info[i].cantidad == nuevo_producto.cantidad &&
                info[i].vence == nuevo_producto.vence &&
                info[i].lote == nuevo_producto.lote &&
                info[i].serial == nuevo_producto.serial &&
                info[i].estado == nuevo_producto.estado &&
                info[i].ubicacion == nuevo_producto.ubicacion &&
                info[i].Ingreso == nuevo_producto.Ingreso)
            {   // no se modifico nada
                continue;
            }

            if (!string.IsNullOrEmpty(info[i].IdTabla) && info[i].pProductoAntiguo == null) { //verificamos si nos toca actualizar algo.
                info[i].pProductoAntiguo = info[i].CopiarProducto();
            }
            info[i].cantidad = nuevo_producto.cantidad;
            info[i].vence = nuevo_producto.vence;
            info[i].lote = nuevo_producto.lote;
            info[i].serial = nuevo_producto.serial;
            info[i].estado = nuevo_producto.estado;
            info[i].ubicacion = nuevo_producto.ubicacion;
            info[i].Ingreso = nuevo_producto.Ingreso;
            actualizado = true;
        }
        return actualizado;
    }

    public void DeleteProducto(int idGrid)
    {
        // solo guardamos los que tienen un id en la tabla
        List<Producto> info= (from p in pProductos
                        where p.IdGrid==idGrid && !string.IsNullOrEmpty(p.IdTabla)
                        select p).ToList();
        for (int i = 0; i < info.Count; i++)
            pProductosEliminados.Add(info[i].pProductoAntiguo==null ? info[i] : info[i].pProductoAntiguo);
        // eliminamos a todos.
        pProductos.RemoveAll(p => p.IdGrid==idGrid);
    }

    public Producto GetProducto(int idGrid)
    {
        // solo guardamos los que tienen un id en la tabla
        List<Producto> info = (from p in pProductos
                               where p.IdGrid == idGrid // && !string.IsNullOrEmpty(p.IdTabla)
                               select p).ToList();
        return info.Count==0 ? null : info[0];
    }

    public void DeleteProducto(String producto, Decimal cantidad, string lote, string vencimiento, int ubicacion, int estado, string serial)
    {
        pProductos.RemoveAll(p => p.id == producto && p.cantidad == cantidad && p.lote == lote && p.vence == vencimiento && p.ubicacion == ubicacion && p.serial==serial && p.estado==estado);
    }

    public ArrayList GetProductos(String producto, Decimal cantidad, String almacen, string lote, string vencimiento, int estado, string serial, int ubicacion/*, bool salida=false*/)
    {
        return GetProductos(producto, cantidad, almacen, lote, vencimiento, estado, serial, ubicacion, false);
    }

    public ArrayList GetProductos(String producto, Decimal cantidad, String almacen, string lote, string vencimiento, int estado, string serial, int ubicacion, bool salida)
    {
        ArrayList productos = new ArrayList();
        using (CConector conector = new CConector())
        {

            string szLote;
            string szSerial;
            if (lote.Length == 0)
                szLote = "''";
            else szLote = Query.GetValor(lote);
            if (serial.Length == 0)
                szSerial = "''";
            else szSerial = Query.GetValor(serial);
            string szVencimiento;
            if (vencimiento.Length == 0)
                szVencimiento = "'0000-00-00'";
            else szVencimiento = "STR_TO_DATE(" + Query.GetValor(vencimiento) + ", '%d/%m/%Y')";

            string consulta = string.Format("SELECT SUM(estado{0}.cantidad) FROM estado{0} LEFT JOIN varios ON (varios.id=estado{0}.estado) WHERE estado{0}.producto=" + Query.GetValor(producto)
                + " AND estado{0}.lote=" + szLote + " AND estado{0}.vence=" + szVencimiento
                + " AND estado{0}.estado=" + estado.ToString() + " AND estado{0}.szSerial=" + szSerial
                + " AND estado{0}.ubicacion=" + ubicacion.ToString()
                , almacen);
            if (salida)
                consulta += " AND varios.parametro1 IS NULL";

            string szDisponible = conector.QueryString(consulta);
            Decimal dCantidad = Convert.ToDecimal(szDisponible);
            dCantidad -= GetProductoUsado(producto, lote, vencimiento);

            if (dCantidad < cantidad)
                return null;

            consulta = string.Format("SELECT estado{0}.cantidad,estado{0}.estado,estado{0}.szSerial FROM estado{0} LEFT JOIN varios ON (varios.id=estado{0}.estado) WHERE estado{0}.producto=" + Query.GetValor(producto)
                + " AND estado{0}.lote=" + szLote + " AND estado{0}.vence=" + szVencimiento
                + " AND estado{0}.estado=" + estado.ToString() + " AND estado{0}.szSerial=" + szSerial
                + " AND estado{0}.ubicacion=" + ubicacion.ToString()
                , almacen);
            if (salida)
                consulta += " AND varios.parametro1 IS NULL";

            MySqlDataReader reader = conector.Query(consulta);
            if (reader != null)
            {
                while (reader.Read())
                {
                    dCantidad = Convert.ToDecimal(reader["cantidad"]);
                    int nEstado = Convert.ToInt32(reader["estado"]);
                    object vence = null;
                    try { vence = reader["vence"]; }
                    catch { }

                    dCantidad -= GetProductoUsado(producto, lote, CConector.GetFromMySQLDate(vence), ubicacion, estado);
                    if (dCantidad == 0)
                        continue;
                    if (dCantidad > cantidad)
                        dCantidad = cantidad;

                    Producto info = new Producto(producto, dCantidad, lote, vencimiento, ubicacion, reader["szSerial"].ToString(), nEstado);

                    productos.Add(info);
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
        }

        return productos;
    }

    public ArrayList GetProductos(String producto, Decimal cantidad, String almacen/*, bool salida=false*/) {

        return GetProductos(producto, cantidad, almacen, false);
    }

    public ArrayList GetProductos(String producto, Decimal cantidad, String almacen, bool salida)
    {
        ArrayList productos = new ArrayList();
        using (CConector conector = new CConector())
        {

            string consulta;
            if (salida)
                consulta = string.Format("SELECT SUM(estado{0}.cantidad) FROM estado{0} LEFT JOIN varios ON (varios.id=estado{0}.estado) WHERE estado{0}.producto={1} AND varios.parametro1 IS NULL", almacen, Query.GetValor(producto));
            else
                consulta = string.Format("SELECT SUM(cantidad) FROM estado{0} WHERE producto={1}", almacen, Query.GetValor(producto));

            string szDisponible = conector.QueryString(consulta);
            Decimal dCantidad = Convert.ToDecimal(Convert.ToDecimal(szDisponible));
            if (cantidad == -1)
                cantidad = dCantidad;
            else dCantidad -= GetProductoUsado(producto);
            if (dCantidad < cantidad)
                return null;

            if (salida)
                consulta = string.Format("SELECT estado{0}.cantidad,estado{0}.lote,estado{0}.vence,estado{0}.ubicacion,estado{0}.szSerial,estado{0}.estado FROM estado{0} LEFT JOIN varios ON (varios.id=estado{0}.estado) WHERE estado{0}.producto={1} AND estado{0}.cantidad>0 AND varios.parametro1 IS NULL ORDER BY vence", almacen, Query.GetValor(producto));
            else
                consulta = string.Format("SELECT cantidad,lote,vence,ubicacion,szSerial,estado FROM estado{0} WHERE producto={1} AND cantidad>0 ORDER BY vence", almacen, Query.GetValor(producto));

            MySqlDataReader reader = conector.Query(consulta);
            if (reader != null)
            {
                while (reader.Read())
                {
                    dCantidad = Convert.ToDecimal(reader["cantidad"]);
                    int ubicacion = Convert.ToInt32(reader["ubicacion"]);
                    int estado = Convert.ToInt32(reader["estado"]);
                    object vence = null;
                    try { vence = reader["vence"]; }
                    catch { }

                    dCantidad -= GetProductoUsado(producto, reader["lote"].ToString(), CConector.GetFromMySQLDate(vence), ubicacion, estado);
                    if (dCantidad <= 0)
                        continue;
                    if (dCantidad > cantidad)
                        dCantidad = cantidad;

                    Producto info = new Producto(producto, dCantidad, reader["lote"].ToString(), CConector.GetFromMySQLDate(vence), ubicacion, reader["szSerial"].ToString(), estado);
                    productos.Add(info);

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
        }
        return productos;
    }
}
