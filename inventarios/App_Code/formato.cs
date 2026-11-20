using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using LitS3;

public class Formato
{
	public Formato()
	{
	}

    public static S3Service GetS3() {
        if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["s3Key"]) || string.IsNullOrEmpty(ConfigurationManager.AppSettings["s3Password"]))
            return null;
        
        S3Service servicio = new S3Service();

        servicio.AccessKeyID = ConfigurationManager.AppSettings["s3Key"];
        servicio.SecretAccessKey = ConfigurationManager.AppSettings["s3Password"];
        string port = ConfigurationManager.AppSettings["s3LocalPort"];
        if (!String.IsNullOrEmpty(port)) {
            int localPort;
            if (!Int32.TryParse(port, out localPort) || localPort < 1 || localPort > 65535)
                throw new InvalidOperationException("Invalid local S3 port.");
            servicio.Host = "127.0.0.1";
            servicio.CustomPort = localPort;
            servicio.UseSubdomains = false;
            servicio.UseSsl = false;
        }
        return servicio;
    }

    public static void Remove(string archivo) {

        S3Service servicio = GetS3();
        if(servicio==null) {
            File.Delete(Formato.GetRutaFormatos(archivo));
        } else { // usamos amazon
            servicio.DeleteObject("inventarios", ((CUsuario)Usuario.GetUsuario()).GetDatabase() + "/" + archivo);
        }
    }

    public static void Guardar(FileUpload upload, string archivo, bool publico) {

        S3Service servicio = GetS3();
        if (servicio == null)
        {
            upload.SaveAs(Formato.GetRutaFormatos("logo.jpg"));
        }
        else
        {
            if(publico)
                servicio.AddObject(upload.FileContent, "inventarios", ((CUsuario)Usuario.GetUsuario()).GetDatabase() + "/" + archivo, upload.PostedFile.ContentType, CannedAcl.PublicRead);
            else servicio.AddObject(upload.FileContent, "inventarios", ((CUsuario)Usuario.GetUsuario()).GetDatabase() + "/" + archivo);
        }
    }
    public static StreamReader GetStreamFormato(string archivo)
    {
        S3Service servicio = GetS3();
        if (servicio != null)
        {
            IEnumerable<ListEntry> existe = servicio.ListObjects("inventarios", ((CUsuario)Usuario.GetUsuario()).GetDatabase()+ "/" + archivo);
            if (existe.Any())
                return new StreamReader(servicio.GetObjectStream("inventarios", ((CUsuario)Usuario.GetUsuario()).GetDatabase()+ "/" + archivo));
        }
        return new StreamReader(GetFormato(archivo));
    }

    protected static string GetRutaFormatos(string archivo) {
        
        return Path.Combine(ConfigurationManager.AppSettings["ruta_formatos"], archivo);
    }

    protected static string GetFormato(string archivo)
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

    protected static string GetPathLogo() {
        S3Service servicio = GetS3();
        if (servicio != null)
        {
            IEnumerable<ListEntry> existe = servicio.ListObjects("inventarios", ((CUsuario)Usuario.GetUsuario()).GetDatabase()+ "/" + "logo.jpg");
            if (existe.Any())
                return "https://s3.amazonaws.com/inventarios/" + ((CUsuario)Usuario.GetUsuario()).GetDatabase() + "/logo.jpg";
            existe = servicio.ListObjects("inventarios", ((CUsuario)Usuario.GetUsuario()).GetDatabase() + "/logo.png");
            if (existe.Any())
                return "https://s3.amazonaws.com/inventarios/" + ((CUsuario)Usuario.GetUsuario()).GetDatabase() + "/logo.png";
        }
        // verificamos que tenga un logo personalizado
        string szArchivo = Path.Combine(ConfigurationManager.AppSettings["ruta_formatos"], "logo.jpg");
        if (File.Exists(szArchivo))
            return "~/img/logo.ashx";
        szArchivo = Path.Combine(ConfigurationManager.AppSettings["ruta_formatos"], "logo.png");
        if (File.Exists(szArchivo))
            return "~/img/logo.ashx";
        // el por defecto
        return "~/img/logo.png";
    }

    public static void RemoveLogo() {

        Remove("logo.png");
        Remove("logo.jpg");
        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "l");
    }

    public static string GetLogo() {

        string ruta = (string)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "l");
        if(string.IsNullOrEmpty(ruta)) {
            ruta = GetPathLogo();
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "l", ruta);
        }
        return ruta;
    }
}
