using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BeIT.MemCached;

public class Cache
{
    public static object Get(string clave) {

        if(MemcachedClient.Exists("cache")) 
            return MemcachedClient.GetInstance("cache").Get(clave); 
        else
            return HttpRuntime.Cache[clave];
    }

    public static void Set(string clave,object valor) {
        if (MemcachedClient.Exists("cache")) 
            MemcachedClient.GetInstance("cache").Set(clave, valor);
        else HttpRuntime.Cache.Insert(clave, valor, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
    }

    public static void Remove(string clave) {
        if (MemcachedClient.Exists("cache"))
            MemcachedClient.GetInstance("cache").Delete(clave);
        else HttpRuntime.Cache.Remove(clave);
    }
}
