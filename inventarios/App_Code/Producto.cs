using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Collections;
using MySql.Data.MySqlClient;
using SDG.Componentes;
using System.IO;
using System.Text;
using System.Xml.Serialization;

/// <summary>
/// Descripción breve de Producto
/// </summary>
[Serializable]
public class Producto
{
    // para no serializar [XmlIgnore]
    protected string _Codigo;
    protected string _Unidad;
    protected string _Descripcion;
    protected string _Nombre;
    protected int _nSerial = -1;
    protected int _nLote = -1;
    protected int _nVence = -1;
    protected int _nNumero= -1;
    protected string _Empresa;

    public String id { set; get; }
    public String lote { set; get; }
    public String vence { set; get; }
    public Decimal cantidad { set; get; }
    public string serial { set; get; }
    public int ubicacion { set; get; }

    [XmlIgnore]
    public string Empresa
    {
        set {
            _Empresa = value;
        }
        get {
            if (_Empresa == null && !string.IsNullOrEmpty(id))
                _Empresa = GetProducto(id).Empresa;
            return _Empresa;
        }
    }
    [XmlIgnore]
    public int nNumero
    {
        set
        {
            _nNumero = value;
        }
        get
        {
            if (_nNumero == -1 && !string.IsNullOrEmpty(id))
                _nNumero = GetProducto(id).nNumero;
            return _nNumero;
        }
    }
    [XmlIgnore]
    public int nSerial { 
        set {
            _nSerial=value;
        }
        get {
            if (_nSerial == -1 && !string.IsNullOrEmpty(id))
                _nSerial = GetProducto(id).nSerial;
            return _nSerial;
        }
    }
    [XmlIgnore]
    public int nLote {
        set
        {
            _nLote = value;
        }
        get
        {
            if (_nLote == -1 && !string.IsNullOrEmpty(id))
                _nLote = GetProducto(id).nLote;
            return _nLote;
        }
    }
    [XmlIgnore]
    public int nVence {
        set
        {
            _nVence = value;
        }
        get
        {
            if (_nVence == -1 && !string.IsNullOrEmpty(id))
                _nVence = GetProducto(id).nVence;
            return _nVence;
        }
    }
    [XmlIgnore]
    public String nombre { set { _Nombre = value; } 
        get {
            if (_Nombre == null && !string.IsNullOrEmpty(id))
                _Nombre = GetProducto(id).nombre;
            return _Nombre;
        } 
    }
    [XmlIgnore]
    public string codigo { 
        set { 
            _Codigo = value; 
        } 
        get {
            if (_Codigo==null && !string.IsNullOrEmpty(id))
                _Codigo = GetProducto(id).codigo;
            return _Codigo;
        } 
    }
    [XmlIgnore]
    public string unidad { set { _Unidad = value; } 
        get { 
            if (_Unidad==null && !string.IsNullOrEmpty(id))
                _Unidad = GetProducto(id).unidad;
            return _Unidad; 
        } 
    }
    [XmlIgnore]
    public string descripcion { set { _Descripcion = value; } 
        get {
            if (_Descripcion == null && !string.IsNullOrEmpty(id))
                _Descripcion = GetProducto(id).descripcion;
            return _Descripcion; 
        } 
    }
    [XmlIgnore]
    public string NombreUbicacion { 
        get {
            return Almacen.GetNombreUbicacion(ubicacion);
        } 
    }
    [XmlIgnore]
    public string NombreCompleto {
        get {
            string completo="";
            if (!string.IsNullOrEmpty(codigo))
                completo = codigo + " - ";
            completo += nombre;
            return completo;
        }
    }
    [XmlIgnore]
    public string ProductoCompleto
    {
        get
        {
            string completo = "";
            if (!string.IsNullOrEmpty(codigo))
                completo = codigo + " - ";
            completo += nombre;
            if (!string.IsNullOrEmpty(lote))
                completo += " (" + lote + ")";
            completo += " " + vence;
            return completo.Trim();
        }
    }
    static public void RemoveProducto(string id) {
        if (HttpRuntime.Cache["producto" + id] != null)
            HttpRuntime.Cache.Remove("producto" + id);
    }

    static public Producto GetProducto(string id)
    {
        Producto producto = (Producto)HttpRuntime.Cache["producto" + id];
        if(producto==null) {
            producto = new Producto();
            Query query = new Query("producto");
            CConector conector = CConector.GetConector();

            query += "nombre";
            query += "descripcion";
            query += "codigo";
            query += "unidad";
            query += "vence";
            query += "lote";
            query += "serial";
            query += "empresa";
            query += "entero";

            MySqlDataReader reader = conector.Query(query.GetSelect("id=" + id));
            if (reader != null && reader.Read())
            {
                producto.id = id;
                producto.nombre = reader["nombre"].ToString();
                producto.descripcion = reader["descripcion"].ToString();
                producto.codigo = reader["codigo"].ToString();
                producto.unidad = reader["unidad"].ToString();
                try
                {
                    producto.nVence = Convert.ToInt32(reader["vence"]);
                } catch { producto.nVence = 0; }
                try
                {
                    producto.nLote = Convert.ToInt32(reader["lote"]);
                }catch { producto.nLote = 0; }
                try
                {
                    producto.nSerial = Convert.ToInt32(reader["serial"]);
                } catch { producto.nSerial = 0;  }
                try
                {
                    producto.nNumero = Convert.ToInt32(reader["entero"]);
                }catch { producto.nNumero = 0; }
                producto.Empresa = reader["empresa"].ToString();
            }
            HttpRuntime.Cache.Insert("producto" + id, producto, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        return producto;
    }

    public Producto()
    {
        ubicacion = 0;
        serial = "";
    }

    public Producto(string idproducto) {

        Producto producto= GetProducto(idproducto);
        
        id = producto.id;
        nombre= producto.nombre;
        descripcion = producto.descripcion;
        codigo= producto.codigo;
        unidad= producto.unidad;
        nVence= producto.nVence;
        nLote= producto.nLote;
        nSerial = producto.nSerial;
        nNumero = producto.nNumero;

        ubicacion = 0;
        serial = "";
    }

    public Producto(String _producto, String _nombre, Decimal _cantidad, String _lote, String _fecha, string _codigo, string _unidad, string _descripcion)
    {
        id = _producto;
        nombre = _nombre;
        cantidad = _cantidad;
        lote = _lote;
        vence = _fecha;
        codigo = _codigo;
        unidad = _unidad;
        descripcion = _descripcion;
        ubicacion=0;
        serial="";
    }

    public Producto(String _producto, String _nombre, string _codigo, string _unidad,string _descripcion)
    {
        id = _producto;
        nombre = _nombre;
        cantidad = 0;
        codigo = _codigo;
        unidad = _unidad;
        descripcion = _descripcion;
        ubicacion=0;
        serial="";
    }

    public Producto(String _producto, String _nombre, Decimal _cantidad, string _codigo, string _unidad, string _descripcion)
    {
        id = _producto;
        nombre = _nombre;
        cantidad = _cantidad;
        codigo = _codigo;
        unidad = _unidad;
        descripcion = _descripcion;
        ubicacion=0;
        serial="";
    }

    public Producto(String _producto, Decimal _cantidad, String _lote, String _fecha, int _ubicacion)
    {
        id = _producto;
        cantidad = _cantidad;
        lote = _lote;
        vence = _fecha;
        ubicacion = _ubicacion;
        serial="";
    }
}