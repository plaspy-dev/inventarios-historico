using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using SDG.Componentes;
using MySql.Data.MySqlClient;

/// <summary>
/// Descripción breve de CConfiguracion
/// </summary>
[Serializable]
public class CConfiguracion
{
    public int nCosto=2;
    public int nValor = 2;
    public int nValorTipo = 0;
    public int nCantidad=0;
    public int nUbicacion = 2;
    public int nDiasVencimiento = 5;
    public int nSeleccionProducto = 0;

    // 
    public bool bVencimiento = true;
    public bool bLote = true;
    public bool bSerial = true;
    public bool bEstado = true;

    // configuracion productos
    public int nProductoLote = 2;
    public int nProductoVence = 2;
    public int nProductoSerial = 2;
    public int nProductoEstado = 2;


    // salidas
    public bool bManejarEntrega=true;
    public bool bTerceroSalida=false;
    public bool bDocumentoSalida = false;
    public bool bCrearTerceroSalida = true;
    public bool bDatosComplementariosTerceroSalida = false;

    // entradas
    public bool bTerceroEntrada = false;
    public bool bDocumentoEntrada = false;
    public bool bCrearTerceroEntradas = true;
    public bool bDatosComplementariosTerceroEntrada = false;


	public CConfiguracion()
	{
	}

    static public CConfiguracion GetConfiguracion() {

        CConfiguracion configuracion = (CConfiguracion)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "c");
        if(configuracion==null) {

            using (CConector conector = new CConector())
            {
                String texto = conector.QueryString("SELECT configuracion FROM configuracion");

                if (string.IsNullOrEmpty(texto))
                {
                    configuracion = new CConfiguracion();
                }
                else
                {
                    StringReader builder = new StringReader(texto);
                    XmlSerializer s = new XmlSerializer(typeof(CConfiguracion));
                    configuracion = (CConfiguracion)s.Deserialize(builder);
                }
            }
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "c", configuracion);
        }
        return configuracion;
    }
    public bool SaveConfiguracion() {

        using (CConector conector = new CConector())
        {
            Query query = new Query("configuracion");

            StringBuilder builder = new StringBuilder();
            XmlSerializer s = new XmlSerializer(this.GetType());
            using (StringWriter writer = new StringWriter(builder))
            {
                s.Serialize(writer, this);
            }
            query["configuracion"] = builder.ToString();

            if (conector.QueryMod("DELETE FROM configuracion") == -1)
                return false;

            if (conector.QueryMod(query.GetInsert()) == -1)
                return false;

            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "c", this);
        }
        return true;
    }
}
