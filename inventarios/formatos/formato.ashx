<%@ WebHandler Language="C#" Class="formato" %>

using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

public class formato : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {

        if (string.IsNullOrEmpty(context.Request.QueryString["t"])) {
            throw new HttpException(404, "File not found");
        }
        try
        {
            context.Response.AddHeader("content-disposition", "attachment;");
            context.Response.ContentType = "text/html";
            context.Response.Write(Formato.GetStreamFormato(context.Request.QueryString["t"].ToLower() + ".html").ReadToEnd());
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