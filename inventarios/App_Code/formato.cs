using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Web;


public class Formato
{
	public Formato()
	{
	}
    
    public static string GetRutaFormatos(string archivo) {
        
        return Path.Combine(ConfigurationManager.AppSettings["ruta_formatos"], archivo);
    }

    public static string GetFormato(string archivo)
    {
        string szArchivo = Path.Combine(ConfigurationManager.AppSettings["ruta_formatos"], archivo);
        if (File.Exists(szArchivo))
            return szArchivo;
        archivo= HttpContext.Current.Server.MapPath("~/formatos/" + archivo);
        return archivo;
    }

    public static string GetArchivo(string archivo)
    {
        string szArchivo = Path.Combine(ConfigurationManager.AppSettings["ruta_formatos"], archivo);
        if (File.Exists(szArchivo))
            return szArchivo;
        return archivo;
    }
}
