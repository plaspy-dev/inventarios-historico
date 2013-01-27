using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDG.Componentes;
using MySql.Data.MySqlClient;
/// <summary>
/// Descripción breve de CUsuario
/// </summary>
public class CUsuario : Usuario
{
    public CUsuario() {
    }

    public CUsuario(string id, string nombre, int grupo, string login) : base (id, nombre, grupo, login)
	{
	}

    override protected Usuario ValidateUser(string usuario, string passwd)
    {
        if (passwd.Length > 0 && usuario.Length > 0)
        {
            CConector conector = new CConector();
            MySqlDataReader reader = conector.Query("SELECT id,nombre,grupo,estado,contrasena,almacenes FROM usuarios WHERE usuario=" + Query.GetValor(usuario) + " AND (contrasena=" + Query.GetValor(passwd) + " OR contrasena=PASSWORD(" + Query.GetValor(passwd) + ")) AND estado IS NULL");
            if (reader != null && reader.Read())
            {
                string id = reader["id"].ToString();
                int grupo= Convert.ToInt32(reader["grupo"]);

                CConector.SetCookie("contrasena", passwd, DateTime.Now.AddDays(2));
                CConector.SetCookie("usuario", usuario);

                CUsuario pUsuario = new CUsuario(id, reader["nombre"].ToString(), grupo, usuario);
                pUsuario["almacenes"] = reader["almacenes"].ToString();

                conector.QueryMod("UPDATE usuarios SET ingreso=SYSDATE() WHERE id=" + id);

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
                }

                return pUsuario;
            }
        }
        return null;
    }

    override protected Usuario ValidateUser()
    {
        return ValidateUser(CConector.GetCookie("usuario"), HttpUtility.UrlDecode(CConector.GetCookie("contrasena")));
    }
}
