using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SDG.Componentes;
using MySql.Data.MySqlClient;
/// <summary>
/// Descripción breve de CUsuario
/// </summary>
public class CUsuario : Usuario
{
    protected DateTime dtFecha;
    protected string szDatabase = "";

    public CUsuario() {
    }

    public CUsuario(string id, string nombre, int grupo, string login) : base (id, nombre, grupo, login)
	{
	}

    public static string GetKeyPassword(DateTime fecha)
    {
        fecha = fecha.AddDays(-31).AddHours(-19).AddMinutes(-11);
        string key = CConector.GetMySQLDateFormat(fecha, true).Substring(3, 16);

        key = key.Replace("-", "!")
            .Replace(":", "?")
            .Replace(" ", "e")
            .Replace("1", "O")
            .Replace("0", "i")
            .Replace("5", "Z");

        return key;
    }
    
    public string GetDatabase() {

        return szDatabase;
    }

    public static string GetDatabase(string usuario) {

        string db= (string)Cache.Get("idbu" + usuario.ToLower());
        if(db==null) {
            // Conector conectorUsuario = new Conector(ConfigurationManager.ConnectionStrings["ConexionUsuario"].ConnectionString);
            // conectorUsuario.QueryString("");
            db = "pulpafruit";
            Cache.Set("idbu" + usuario.ToLower(), db);
        }
        return db;
    }

    override protected Usuario IsValidUsuario()
    {
        object ingreso = Cache.Get("iiu" + m_szLogin.ToLower());
        if(ingreso!=null) {
            DateTime fecha = (DateTime)ingreso;
            if (fecha != dtFecha)
                return null;
        }
        return this;
    }

    override protected Usuario ValidateUser(string usuario, string passwd)
    {
        if (passwd.Length > 0 && usuario.Length > 0)
        {
            Conector conectorUsuario = new Conector(ConfigurationManager.ConnectionStrings["ConexionUsuario"].ConnectionString);

            MySqlDataReader reader = conectorUsuario.Query("SELECT id,nombre,grupo,estado,contrasena,almacenes FROM usuarios WHERE usuario=" + Query.GetValor(usuario) + " AND (contrasena=" + Query.GetValor(passwd) + " OR contrasena=PASSWORD(" + Query.GetValor(passwd) + ")) AND estado IS NULL");
            if (reader != null && reader.Read())
            {
                string id = reader["id"].ToString();
                int grupo= Convert.ToInt32(reader["grupo"]);

                CUsuario pUsuario = new CUsuario(id, reader["nombre"].ToString(), grupo, usuario);
                pUsuario["almacenes"] = reader["almacenes"].ToString();
                pUsuario.szDatabase = "pulpafruit";
                
                CConector.SetCookie("usuario", usuario);
                Cache.Remove("iiu" + pUsuario.m_szLogin.ToLower());

                // obtenemos la fecha de ingreso
                CConector conector = new CConector(pUsuario.szDatabase);
                conector.QueryMod("UPDATE usuarios SET ingreso=SYSDATE() WHERE id=" + id);
                reader = conector.Query("SELECT ingreso FROM usuarios WHERE id=" + Query.GetValor(id) + " AND (estado IS NULL OR estado!=-1)");
                if (reader != null && reader.Read())
                {
                    pUsuario.dtFecha = (DateTime)reader["ingreso"];
                    Cache.Set("iiu" + pUsuario.m_szLogin.ToLower(), pUsuario.dtFecha);
                    CConector.SetCookie("contrasena", Crypt.Encriptar3DES(passwd, GetKeyPassword(pUsuario.dtFecha)), DateTime.Now.AddDays(2));
                }
                Cache.Set("idbu" + pUsuario.m_szLogin.ToLower(), pUsuario.szDatabase);

                if (grupo > 0)
                {
                    reader = conector.Query("SELECT url,permiso FROM permisos WHERE grupo='" + grupo + "'");
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            pUsuario.SetPermiso(reader["url"].ToString(), Convert.ToInt32(reader["permiso"]));
                        }
                    }
                    // agregmos los permisos extras
                    pUsuario.AddPermiso("__no__", "Contraseña", "~/contrasena.aspx", 1);
                    pUsuario.AddPermiso("__no__", "logo", "~/img/logo.ashx", 1);
                    pUsuario.AddPermiso("__no__", "formato", "~/formatos/formato.ashx", 1);
                    pUsuario.AddPermiso("__no__", "formato", "~/webservices/servicios.asmx", 1);
                }

                return pUsuario;
            }
        }
        return null;
    }

    override protected Usuario ValidateUser()
    {
        object ingreso = Cache.Get("iiu" + CConector.GetCookie("usuario").ToLower());

        if(ingreso==null) {
            string db=CUsuario.GetDatabase(CConector.GetCookie("usuario").ToLower());
            if(!string.IsNullOrEmpty(db)) {
                Conector conector = new Conector(string.Format(ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString, db));
                MySqlDataReader reader = conector.Query("SELECT ingreso FROM usuarios WHERE usuario=" + Query.GetValor(CConector.GetCookie("usuario")) + " AND (estado IS NULL OR estado!=-1)");
                if (reader != null && reader.Read())
                {
                    ingreso=reader["ingreso"];
                }
            }
        }
        try
        {
            return ValidateUser(
                CConector.GetCookie("usuario")
                , Crypt.Desencriptar3DES(CConector.GetCookie("contrasena"), GetKeyPassword((DateTime)ingreso)));
        }
        catch { }
        return null;
    }
}
