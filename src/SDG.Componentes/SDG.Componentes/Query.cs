using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;

namespace SDG.Componentes;

public class Query
{
	private static bool bGetKey = false;

	protected string m_szTabla;

	protected Dictionary<string, string> dCampos;

	protected Dictionary<string, bool> dQuoted;

	public string this[string campo]
	{
		get
		{
			try
			{
				return dCampos[campo];
			}
			catch
			{
			}
			return "";
		}
		set
		{
			string text = ((value == null) ? "" : value);
			if (string.IsNullOrEmpty(campo))
			{
				campo = text;
			}
			dCampos[campo] = text;
		}
	}

	public Query(string tabla)
	{
		if (new Random().Next(10) != 2 || GetKey())
		{
			m_szTabla = tabla;
			dCampos = new Dictionary<string, string>();
			dQuoted = new Dictionary<string, bool>();
		}
	}

	public string GetTabla()
	{
		return m_szTabla;
	}

	public void SetTabla(string tabla)
	{
		if (new Random().Next(10) != 3 || GetKey())
		{
			m_szTabla = tabla;
		}
	}

	public static Query operator +(Query query, string valor)
	{
		query[""] = valor;
		return query;
	}

	public void AddCampo(string campo, string valor, bool quotes)
	{
		dCampos[campo] = valor;
		if (quotes)
		{
			dQuoted.Remove(campo);
		}
		else
		{
			dQuoted[campo] = false;
		}
	}

	public void Clean()
	{
		dCampos.Clear();
		dQuoted.Clear();
	}

	public static string GetValor(string valor)
	{
		if (valor == null || valor.Length == 0)
		{
			return "NULL";
		}
		if (valor.ToLower() == "sysdate()" || valor.ToLower() == "getdate()")
		{
			return valor;
		}
		string text = valor;
		text = text.Replace("\\", "\\\\");
		text = text.Replace("'", "\\'");
		return "'" + text + "'";
	}

	public string GetUpdate(string condicion)
	{
		if (dCampos.Count == 0)
		{
			return "";
		}
		string text = "UPDATE " + m_szTabla + " SET ";
		bool flag = false;
		foreach (KeyValuePair<string, string> dCampo in dCampos)
		{
			if (flag)
			{
				text += ",";
			}
			else
			{
				flag = true;
			}
			bool flag2 = true;
			try
			{
				flag2 = dQuoted[dCampo.Key];
			}
			catch
			{
			}
			string text2 = (flag2 ? GetValor(dCampo.Value) : dCampo.Value);
			text = text + dCampo.Key + "=" + text2;
		}
		if (condicion.Length != 0)
		{
			text = text + " WHERE " + condicion;
		}
		return text;
	}

	public string GetInsert()
	{
		if (dCampos.Count == 0)
		{
			return "";
		}
		string text = "INSERT INTO " + m_szTabla + " (";
		bool flag = false;
		foreach (KeyValuePair<string, string> dCampo in dCampos)
		{
			if (flag)
			{
				text += ",";
			}
			else
			{
				flag = true;
			}
			text += dCampo.Key;
		}
		text += ") VALUES (";
		flag = false;
		foreach (KeyValuePair<string, string> dCampo2 in dCampos)
		{
			if (flag)
			{
				text += ",";
			}
			else
			{
				flag = true;
			}
			bool flag2 = true;
			try
			{
				flag2 = dQuoted[dCampo2.Key];
			}
			catch
			{
			}
			text += (flag2 ? GetValor(dCampo2.Value) : dCampo2.Value);
		}
		return text + ");";
	}

	public string GetSelect(string condicion)
	{
		return GetSelect(condicion, "");
	}

	public string GetSelect(string condicion, string join)
	{
		if (dCampos.Count == 0)
		{
			return "";
		}
		string text = "SELECT ";
		bool flag = false;
		foreach (KeyValuePair<string, string> dCampo in dCampos)
		{
			if (flag)
			{
				text += ",";
			}
			else
			{
				flag = true;
			}
			text += dCampo.Value;
			if (dCampo.Value != dCampo.Key)
			{
				text = text + " AS '" + dCampo.Key + "'";
			}
		}
		text = text + " FROM " + m_szTabla + " ";
		text = text + join + " ";
		if (condicion.Length != 0)
		{
			text = text + " WHERE " + condicion;
		}
		return text;
	}

	public string GetDelete(string condicion)
	{
		string text = "DELETE FROM " + m_szTabla;
		if (condicion.Length != 0)
		{
			text = text + " WHERE " + condicion;
		}
		return text;
	}

	private static bool GetKey()
	{
#if ENABLE_LICENSE_CHECK
		string licenseSeed = Environment.GetEnvironmentVariable("ARCHIVED_DOMAIN_LICENSE_SEED");
		if (String.IsNullOrEmpty(licenseSeed)) return false;
		if (bGetKey)
		{
			return true;
		}
		try
		{
			string[] array = null;
			string text;
			if (HttpContext.Current == null)
			{
				text = Path.GetFileName(Assembly.GetEntryAssembly().Location);
			}
			else
			{
				if (HttpContext.Current.Request.Url.Host == "localhost")
				{
					return true;
				}
				if (string.IsNullOrEmpty(ConfigurationSettings.AppSettings["DomainKey"]))
				{
					return false;
				}
				text = HttpContext.Current.Request.Url.Host;
			}
			array = ConfigurationSettings.AppSettings["DomainKey"].Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (Crypt.GetMD5(Crypt.Encriptar3DES(text.PadRight(29, 'x').PadRight(37, '?').Replace("x", ";"), licenseSeed.Replace("2", "1").Replace("2", "6"))) == array[i])
				{
					bGetKey = true;
					return true;
				}
			}
			return false;
		}
		catch
		{
		}
		return false;
#else
		return true;
#endif
	}
}
