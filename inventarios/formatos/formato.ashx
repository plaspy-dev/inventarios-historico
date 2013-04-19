<%@ WebHandler Language="C#" Class="formato" %>

using System;
using System.IO;
using System.Web;

public class formato : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        if (string.IsNullOrEmpty(context.Request.QueryString["t"])) {
            throw new HttpException(404, "File not found");
        }
        try
        {
            context.Response.AddHeader("content-disposition", "attachment;");
            string archivo = Formato.GetArchivo(context.Request.QueryString["t"].ToLower() + ".html");
            context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(archivo).ToUniversalTime().ToString("R"));
            context.Response.WriteFile(archivo);
        }
        catch {
            throw new HttpException(404, "File not found");
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}