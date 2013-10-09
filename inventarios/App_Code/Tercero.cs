using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDG.Componentes;
using MySql.Data.MySqlClient;

[Serializable]
public class Tercero
{
    public String Id { set; get; }
    public String Nombre { set; get; }
    public String Documento { set; get; }
    public String Direccion { set; get; }
    public String Telefono { set; get; }
    public String Email { set; get; }
    public String NombreCompleto {
        get {
            return Documento + " - " + Nombre;
        }
    }

    public Tercero() { }
    public Tercero(string id, string nombre, string documento, string direccion, string telefono, string email)
    {
        Id = id;
        Nombre = nombre;
        Documento = documento;
        Direccion = direccion;
        Telefono = telefono;
        Email = email;
    }

	public Tercero(string id)
	{
        Id = id;
        if (string.IsNullOrEmpty(id))
            return;
        Tercero info = (Tercero)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "t" + Id);
        if (info == null)
        {
            using (CConector conector = new CConector())
            {
                Query query = new Query("tercero");

                query += "nombre";
                query += "direccion";
                query += "telefono";
                query += "email";
                query += "identificacion";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(id)));
                if (reader != null && reader.Read())
                {
                    Nombre = reader["nombre"].ToString();
                    Direccion = reader["direccion"].ToString();
                    Telefono = reader["telefono"].ToString();
                    Email = reader["email"].ToString();
                    Documento = reader["identificacion"].ToString();
                }
            }
            Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "t" + Id, this);
        } else {
            Nombre = info.Nombre;
            Direccion = info.Direccion;
            Telefono = info.Telefono;
            Email = info.Email;
            Documento = info.Documento;
        }
	}

    static public void RemoveTercero(string id)
    {
        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "t" + id);
    }
}
