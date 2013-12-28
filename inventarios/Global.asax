<%@ Application Language="C#" %>

<script runat="server">
    
    void Application_Start(object sender, EventArgs e) 
    {
        if(!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MemCachedServers"]))
            BeIT.MemCached.MemcachedClient.Setup("cache", new string[] { System.Configuration.ConfigurationManager.AppSettings["MemCachedServers"] });
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Código que se ejecuta cuando se cierra la aplicación
    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        try
        {
            Exception exx = Server.GetLastError().GetBaseException();
            if (exx is HttpException)
            {
                if (((HttpException)(exx)).GetHttpCode() == 404
                    || ((HttpException)(exx)).GetHttpCode() == 403
                    || ((HttpException)(exx)).GetHttpCode() == 405
                    || ((HttpException)(exx)).GetHttpCode() == 411) // omitimos los errores 403 y 404
                    return;
            }
        } catch { }
        SDG.Componentes.Logs.WriteLog(SDG.Componentes.Logs.DumpPage(Server, Request));
        // Server.ClearError();
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Código que se ejecuta cuando se inicia una nueva sesión
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Código que se ejecuta cuando finaliza una sesión. 
        // Nota: El evento Session_End se desencadena sólo con el modo sessionstate
        // se establece como InProc en el archivo Web.config. Si el modo de sesión se establece como StateServer 
        // o SQLServer, el evento no se genera.
    }       
</script>
