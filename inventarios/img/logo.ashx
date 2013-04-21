<%@ WebHandler Language="C#" Class="logo" %>

using System;
using System.IO;
using System.Configuration;
using System.Web;
using System.Web.SessionState;

public class logo : IHttpHandler, IRequiresSessionState
{
    
    public void ProcessRequest (HttpContext context) {

        context.Response.ContentType = "image/jpeg"; // buscamos el logo jpg
        string archivo= Path.Combine(ConfigurationManager.AppSettings["ruta_formatos"], "logo.jpg");
        if(File.Exists(archivo)) {
            
            context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(archivo).ToUniversalTime().ToString("R"));
            context.Response.WriteFile(archivo);
            return;
        }

        archivo = Formato.GetArchivo("logo.png");
        context.Response.ContentType = "image/png";
        context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(archivo).ToUniversalTime().ToString("R"));
        context.Response.WriteFile(archivo);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}