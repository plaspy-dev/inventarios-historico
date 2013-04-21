using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Collections;
using System.Globalization;
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
    protected int _nEstado = -1;
    protected string _Empresa;
    public string FormatoLote = "";

    public String id { set; get; }
    public String lote { set; get; }
    public String vence { set; get; }
    public Decimal cantidad { set; get; }
    public string serial { set; get; }
    public int estado { set; get; }
    public int ubicacion { set; get; }
    public Decimal Costo { set; get;}
    public Decimal Valor { set; get; }
    public int DiasV { set; get; }

    [XmlIgnore]
    public bool bIngreso = false;

    [XmlIgnore] // este es el ingreso del repuesto
    public string _Ingreso = "";

    [XmlIgnore] // este es el ingreso del repuesto
    public string Ingreso { set { _Ingreso = value; bIngreso = value.Length > 0; } get { return _Ingreso; } }

    [XmlIgnore]
    public string IdTabla { set; get; }
    [XmlIgnore]
    public int IdGrid { set; get; }
    [XmlIgnore]
    public Producto pProductoAntiguo = null;
    [XmlIgnore]
    public Decimal CostoTotal {
        get { return cantidad*Costo; }
    }
    [XmlIgnore]
    public Decimal ValorTotal
    {
        get { return cantidad * Valor; }
    }

    [XmlIgnore]
    public string szEstado { get {
        return Almacen.GetNombreEstado(estado);
    }}

    public string LoteTemporal {
        get {
            DateTime actual = DateTime.Now;
            string pad = "00";
            string dia= actual.Day.ToString();
            string mes = actual.Month.ToString();

            string lote = FormatoLote
                .Replace("%codigo%", codigo)
                .Replace("%dia%", pad.Substring(0, pad.Length - dia.Length) + dia)
                .Replace("%mes%", pad.Substring(0, pad.Length - mes.Length) + mes)
                .Replace("%ano%",  actual.Year.ToString().Substring(2))
                .Replace("%anoc%", actual.Year.ToString());
            return lote;
        }
    }

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
    public int nEstado
    {
        set
        {
            _nEstado = value;
        }
        get
        {
            if (_nEstado == -1 && !string.IsNullOrEmpty(id))
                _nEstado = GetProducto(id)._nEstado;
            return _nEstado;
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
            if (!string.IsNullOrEmpty(serial))
                completo += " - " + serial;
            if(estado!=0) 
                completo += " [" + szEstado +"]";
            return completo.Trim();
        }
    }

    static public void RemoveProducto(string id) {
        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "p" + id);
        CProductoValor.Delete(id);
    }

    static public Producto GetProducto(string id)
    {
        Producto producto = (Producto)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "p" + id);
        if(producto==null) {
            producto = new Producto();
            Query query = new Query("producto");
            using (CConector conector = new CConector())
            {

                query += "nombre";
                query += "descripcion";
                query += "codigo";
                query += "unidad";
                query += "vence";
                query += "lote";
                query += "serial";
                query += "empresa";
                query += "entero";
                query += "nEstado";
                query += "diasvencimiento";
                query += "formatolote";

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
                    }
                    catch { producto.nVence = 0; }
                    try
                    {
                        producto.nLote = Convert.ToInt32(reader["lote"]);
                    }
                    catch { producto.nLote = 0; }
                    try
                    {
                        producto.nSerial = Convert.ToInt32(reader["serial"]);
                    }
                    catch { producto.nSerial = 0; }
                    try
                    {
                        producto.nNumero = Convert.ToInt32(reader["entero"]);
                    }
                    catch { producto.nNumero = 0; }
                    try
                    {
                        producto.nEstado = Convert.ToInt32(reader["nEstado"]);
                    }
                    catch { producto.nEstado = 0; }
                    try
                    {
                        producto.DiasV = Convert.ToInt32(reader["diasvencimiento"]);
                    }
                    catch { producto.DiasV = 0; }
                    producto.Empresa = reader["empresa"].ToString();
                    producto.FormatoLote = reader["formatolote"].ToString();
                }
            }
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "p" + id, producto);
        }
        return producto;
    }

    public void DeleteCostoPromedio(string almacen) {

        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "cp" + almacen + "_" + id);
    }

    public Decimal GetCostoPromedio(string almacen)
    {
        Costo = 0;
        if (Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "cp" + almacen + "_" + id) != null)
        {
            Costo = (Decimal)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "cp" + almacen + "_" + id);
            return Costo;
        }

        using(CConector conector = new CConector()) 
        {
            string costo = conector.QueryString(string.Format("SELECT costo FROM estado{0} WHERE producto={1}", almacen, id));
            try
            {
                Costo = Convert.ToDecimal(costo);
            }
            catch { }
        }
        Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "cp" + almacen + "_" + id, Costo);
        return Costo;
    }

    public Decimal GetCosto(string proveedor)
    {
        Costo= CProductoValor.GetProducto(id).GetCosto(proveedor);
        return Costo;
    }

    public Decimal GetValor(string cliente, int almacen)
    {
        Valor = CProductoValor.GetProducto(id).GetValor(cliente, almacen);
        return Valor;
    }

    public Producto()
    {
        ubicacion = 0;
        estado = 0;
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
        nEstado = producto.nEstado;
        Costo = producto.Costo;
        DiasV = producto.DiasV;
        FormatoLote = producto.FormatoLote;

        ubicacion = 0;
        estado = 0;
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
        estado = 0;
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
        estado = 0;
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
        estado = 0;
        serial="";
    }

    public Producto(String _producto, Decimal _cantidad, String _lote, String _fecha, int _ubicacion, string _serial, int _estado)
    {
        id = _producto;
        cantidad = _cantidad;
        lote = _lote;
        vence = _fecha;
        ubicacion = _ubicacion;
        estado = _estado;
        serial=_serial;
    }

    public bool GetAcumulado(string almacen, ref decimal saldo)
    {
        return GetAcumulado(almacen, CConector.GetMySQLDateFormat(Ingreso, true), ref saldo);
    }

    public bool GetAcumulado(string almacen, string desde_conhora, ref decimal saldo) {

        if (string.IsNullOrEmpty(desde_conhora))
            return false;
        // iniciamos las variables
        using (CConector conector = new CConector())
        {
            string query = "SET @i=0,@saldo=0;";
            if (conector.QueryMod(query) == -1)
                return false;

            // falta validar tambien el lote, serial, etc...

            // establecemos el costo promedio
            query = string.Format("SELECT * FROM ("
                + " SELECT @i:=@i+1 AS c,@saldo:=@saldo+cantidad AS saldo FROM ("
                + " (SELECT cantidad AS cantidad,ingreso FROM entrada_items{0} WHERE producto={1} AND ingreso<{2} )"
                + " UNION ALL (SELECT -cantidad AS cantidad,ingreso FROM traslado_items{0} WHERE producto={1} AND ingreso<{2} )"
                + " UNION ALL (SELECT -cantidad AS cantidad,ingreso FROM salida_items{0} WHERE producto={1} AND ingreso<{2})"
                + " UNION ALL (SELECT -cantidad AS cantidad,ingreso FROM transformacion_salida{0} WHERE producto={1} AND ingreso<{2})"
                + " UNION ALL (SELECT cantidad AS cantidad,ingreso FROM transformacion_entrada{0} WHERE producto={1} AND ingreso<{2})"
                + " ORDER BY ingreso) AS t"
                + "  ) AS b WHERE c=@i;"
                 , almacen, id, Query.GetValor(desde_conhora));
            MySqlDataReader reader = conector.Query(query);
            saldo = 0;
            if (reader == null)
                return false;
            if (reader.Read())
            {
                try { saldo = Convert.ToDecimal(reader["saldo"]); }
                catch { }
            }
        }
        return true;
    }

    public bool GetAcumulado(string almacen, string desde, ref decimal costoacumulado, ref decimal saldo, ref decimal costop)
    {
        // iniciamos las variables
        using (CConector conector = new CConector())
        {
            string query = "SET @i=0,@costoacumulado=0,@costop=0,@saldo=0;";
            if (conector.QueryMod(query) == -1)
                return false;

            // establecemos el costo promedio
            query = string.Format("SELECT * FROM ("
                + " SELECT @i:=@i+1 AS c,@costoacumulado:=@costoacumulado+IFNULL(costot,@costop*cantidad) AS costoacumulado,@saldo:=@saldo+cantidad AS saldo,@costop:=@costoacumulado/@saldo AS costop FROM ("
                + " (SELECT cantidad AS cantidad,ingreso,costo*cantidad AS costot FROM entrada_items{0} WHERE producto={1} AND DATE(ingreso)<{2} )"
                + " UNION ALL (SELECT -cantidad AS cantidad,ingreso,NULL AS costot FROM traslado_items{0} WHERE producto={1} AND DATE(ingreso)<{2} )"
                + " UNION ALL (SELECT -cantidad AS cantidad,ingreso,NULL AS costot FROM salida_items{0} WHERE producto={1} AND DATE(ingreso)<{2})"
                + " UNION ALL (SELECT -cantidad AS cantidad,ingreso,NULL AS costot FROM transformacion_salida{0} WHERE producto={1} AND DATE(ingreso)<{2})"
                + " UNION ALL (SELECT cantidad AS cantidad,ingreso,costo*cantidad AS costot FROM transformacion_entrada{0} WHERE producto={1} AND DATE(ingreso)<{2})"
                + " ORDER BY ingreso) AS t"
                + "  ) AS b WHERE c=@i;"
                 , almacen, id, Query.GetValor(desde));
            MySqlDataReader reader = conector.Query(query);
            costop = 0;
            costoacumulado = 0;
            saldo = 0;
            if (reader == null)
                return false;
            if (reader.Read())
            {
                try { costoacumulado = Convert.ToDecimal(reader["costoacumulado"]); }
                catch { }
                try { costop = Convert.ToDecimal(reader["costop"]); }
                catch { }
                try { saldo = Convert.ToDecimal(reader["saldo"]); }
                catch { }
            }
        }
        return true;
    }

    public bool RecalcularCosto(Conector conector, string almacen) {

        // iniciamos las variables
        string query = "SET @i=0,@costoacumulado=0,@costop=0,@saldo=0;";
        if (conector.QueryMod(query) == -1)
            return false;

        // establecemos el costo promedio
        query = string.Format("SELECT * FROM ("
            + " SELECT @i:=@i+1 AS c,@costoacumulado:=@costoacumulado+IFNULL(costot,@costop*cantidad) AS costoacumulado,@saldo:=@saldo+cantidad AS saldo,@costop:=@costoacumulado/@saldo AS costop FROM ("
            + " (SELECT cantidad AS cantidad,ingreso,costo*cantidad AS costot FROM entrada_items{0} WHERE producto={1} )"
            + " UNION ALL (SELECT -cantidad AS cantidad,ingreso,NULL AS costot FROM traslado_items{0} WHERE producto={1} )"
            + " UNION ALL (SELECT -cantidad AS cantidad,ingreso,NULL AS costot FROM salida_items{0} WHERE producto={1})"
            + " UNION ALL (SELECT -cantidad AS cantidad,ingreso,NULL AS costot FROM transformacion_salida{0} WHERE producto={1})"
            + " UNION ALL (SELECT cantidad AS cantidad,ingreso,costo*cantidad AS costot FROM transformacion_entrada{0} WHERE producto={1})"
            + " ORDER BY ingreso) AS t"
            + "  ) AS b WHERE c=@i;"
             , almacen, id);
        if (conector.Query(query) == null)
            return false;

        query = string.Format("UPDATE estado{0} SET costo=@costop,costoacumulado=@costoacumulado WHERE producto={1};"
             , almacen, id);
        if (conector.QueryMod(query) == -1)
            return false;

        return true;
    }

    public bool UpdateCosto(Conector conector, string proveedor)
    {
        CProductoValor info = CProductoValor.GetProducto(id);

        if (info.GetCosto() != Costo)
        {
            // actualizamos el costo del producto
            string query = string.Format("UPDATE producto SET costo={1} WHERE id={0};"
                    , id, Costo.ToString(CultureInfo.InvariantCulture));
            if (conector.QueryMod(query) == -1)
                return false;
        }

        if (!string.IsNullOrEmpty(proveedor))
        {
            string query = string.Format("UPDATE costo_proveedor SET costo={2},fecha=SYSDATE() WHERE producto={0} AND proveedor={1};"
                , id, proveedor, Costo.ToString(CultureInfo.InvariantCulture));
            if (conector.QueryMod(query) == -1)
                return false;
            query = string.Format("INSERT IGNORE INTO costo_proveedor (producto,proveedor,costo,fecha) VALUES ({0},{1}, {2},SYSDATE());"
                , id, proveedor, Costo.ToString(CultureInfo.InvariantCulture));
            if (conector.QueryMod(query) == -1)
                return false;
        }
        info.UpdateCostos(proveedor, Costo);
        return true;
    }

    public bool UpdateValor(Conector conector)
    {
        CProductoValor info = CProductoValor.GetProducto(id);
        if(info.GetValorTemp()!=Valor) {

            // actualizamos el costo del producto
            string query = string.Format("UPDATE producto SET valor_temporal={1} WHERE id={0};"
                    , id, Valor.ToString(CultureInfo.InvariantCulture));
            if (conector.QueryMod(query) == -1)
                return false;

            info.UpdateValorTemp(Valor);
        }
        return true;
    }

    public bool DeleteProductoFromEstado(Conector conector, string almacen) {

        if (pProductoAntiguo != null)
        {
            if (!pProductoAntiguo.AddProductoFromEstado(conector, almacen))
                return false;
        }
        if (!bIngreso)
            return true;

        string szLote;
        if (lote.Length == 0)
            szLote = "''";
        else szLote = Query.GetValor(lote);
        string szSerial;
        if (serial.Length == 0)
            szSerial = "''";
        else szSerial = Query.GetValor(serial);
        string szVencimiento = Conector.GetMySQLDateFormat(vence);
        if (szVencimiento.Length == 0)
            szVencimiento = "0000-00-00";

        // actualizamos el estado
        string query = string.Format("UPDATE estado{0} SET cantidad=cantidad-{1},modificacion=SYSDATE() WHERE producto={2} AND lote={3} AND vence={4} AND szSerial={5} AND ubicacion={6} AND estado={7};"
            , almacen, cantidad.ToString(CultureInfo.InvariantCulture), id, szLote, Query.GetValor(szVencimiento), szSerial, ubicacion, estado);
        if (conector.QueryMod(query) == -1)
            return false;

        // actualizamos el estado global
        query = string.Format("UPDATE estado_global SET cantidad=cantidad-{0},modificacion=SYSDATE() WHERE producto={1} AND estado={2};"
            , cantidad.ToString(CultureInfo.InvariantCulture), id, estado);
        if (conector.QueryMod(query) == -1)
            return false;

        // actualizamos el costo acumulado
        query = string.Format("UPDATE estado{0} SET costoacumulado=costoacumulado-({2}*costo) WHERE producto={1};"
            , almacen, id, cantidad.ToString(CultureInfo.InvariantCulture));
        if (conector.QueryMod(query) == -1)
            return false;

        return true;
    }

    public bool AddProductoFromEstado(Conector conector, string almacen)
    {
        if (pProductoAntiguo != null)
        {
            if (!pProductoAntiguo.DeleteProductoFromEstado(conector, almacen))
                return false;
        }
        if (!bIngreso)
            return true;

        string szLote;
        if (lote.Length == 0)
            szLote = "''";
        else szLote = Query.GetValor(lote);
        string szSerial;
        if (serial.Length == 0)
            szSerial = "''";
        else szSerial = Query.GetValor(serial);
        string szVencimiento = Conector.GetMySQLDateFormat(vence);
        if (szVencimiento.Length == 0)
            szVencimiento = "0000-00-00";

        string query = string.Format("UPDATE estado{0} SET cantidad=cantidad+{1},modificacion=SYSDATE()"
                                    + " WHERE producto={2} AND lote={3} AND vence={4} AND szSerial={5} AND ubicacion={6} AND estado={7};"
            , almacen, cantidad.ToString(CultureInfo.InvariantCulture), id, szLote, Query.GetValor(szVencimiento), szSerial, ubicacion, estado);
        if (conector.QueryMod(query) == -1)
            return false;

        // actualizamos el estado global
        query = string.Format("UPDATE estado_global SET cantidad=cantidad+{0},modificacion=SYSDATE() WHERE producto={1} AND estado={2};"
            , cantidad.ToString(CultureInfo.InvariantCulture), id, estado);
        if (conector.QueryMod(query) == -1)
            return false;

        query = string.Format("INSERT IGNORE INTO estado{0} (producto, lote, vence, cantidad,szSerial,ubicacion,estado, modificacion) "
            + " VALUES ({1}, {2}, {3}, {4}, {5},{6},{7}, SYSDATE());"
            , almacen, id, szLote, Query.GetValor(szVencimiento), cantidad.ToString(CultureInfo.InvariantCulture), szSerial, ubicacion, estado);
        if (conector.QueryMod(query) == -1)
            return false;

        // insertamos el producto en el estado global
        query = string.Format("INSERT IGNORE INTO estado_global (producto,estado,cantidad,modificacion) VALUES ({1},{2},{0},SYSDATE());"
            , cantidad.ToString(CultureInfo.InvariantCulture), id, estado);
        if (conector.QueryMod(query) == -1)
            return false;

        // actualizamos el costo promedio y el costo acumulado
        query = string.Format("UPDATE estado{0},(SELECT MAX(costoacumulado)+({2}*{3}) AS costoa,SUM(cantidad) AS cantidad FROM estado{0} WHERE producto={1}) AS t"
                + " SET costoacumulado=t.costoa,costo=t.costoa/t.cantidad WHERE producto={1};"
            , almacen, id, cantidad.ToString(CultureInfo.InvariantCulture), Costo.ToString(CultureInfo.InvariantCulture));
        if (conector.QueryMod(query) == -1)
            return false;

        DeleteCostoPromedio(almacen);
        return true;
    }

    public Query GetQuery(string tabla) {

        Query query = new Query(tabla);

        string szLote;
        if (lote.Length == 0)
            szLote = "''";
        else szLote = Query.GetValor(lote);
        string szSerial;
        if (serial.Length == 0)
            szSerial = "''";
        else szSerial = Query.GetValor(serial);
        string szVencimiento = Conector.GetMySQLDateFormat(vence);
        if (szVencimiento.Length == 0)
            szVencimiento = "0000-00-00";

        query["producto"]= id.ToString();
        query["cantidad"]= cantidad.ToString(CultureInfo.InvariantCulture);
        query["vence"] = szVencimiento;
        query["ubicacion"]= ubicacion.ToString();
        query["estado"]= estado.ToString();
        query["ingreso"] = bIngreso ? "SYSDATE()" : "";
        query["costo"] = Costo.ToString(CultureInfo.InvariantCulture);

        query.AddCampo("lote", szLote, false);
        query.AddCampo("szSerial", szSerial, false);

        return query;
    }
    
    public Producto CopiarProducto() {

        Producto producto = new Producto(id);

        producto.IdTabla = IdTabla;
        producto.cantidad = cantidad;
        producto.vence = vence;
        producto.lote = lote;
        producto.serial = serial;
        producto.estado = estado;
        producto.ubicacion = ubicacion;
        producto.Ingreso = Ingreso;
        producto.Costo = Costo;
        producto.Valor = Valor;

        return producto;
    }
}
