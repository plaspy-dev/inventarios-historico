using System;
using System.Collections;
using System.Configuration;
using System.Web;

namespace SDG.Componentes;

public class Query
{
	private static bool bGetKey = false;

	protected string m_szTabla;

	protected ArrayList m_saCampos = null;

	protected ArrayList m_saValores = null;

	protected ArrayList m_baQuoted = null;

	public string this[string campo]
	{
		get
		{
			return m_saValores[GetCampo(campo)].ToString();
		}
		set
		{
			m_saValores[GetCampo((campo.Length == 0) ? value : campo)] = ((value == null) ? "" : value);
		}
	}

	public Query(string tabla)
	{
		if (new Random().Next(10) != 2 || GetKey())
		{
			m_szTabla = tabla;
			m_saCampos = new ArrayList();
			m_saValores = new ArrayList();
			m_baQuoted = new ArrayList();
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

	public int GetCampo(string campo)
	{
		int num = -1;
		for (int i = 0; i < m_saCampos.Count; i++)
		{
			if (m_saCampos[i].ToString() == campo)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			num = m_saCampos.Add(campo);
			m_saValores.Add("");
			m_baQuoted.Add(true);
		}
		return num;
	}

	public void AddCampo(string campo, string valor, bool quotes)
	{
		m_saValores[GetCampo(campo)] = ((valor == null) ? "" : valor);
		m_baQuoted[GetCampo(campo)] = quotes;
	}

	public void Clean()
	{
		m_saCampos.Clear();
		m_saValores.Clear();
		m_baQuoted.Clear();
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
		if (m_saValores.Count == 0)
		{
			return "";
		}
		string text = "UPDATE " + m_szTabla + " SET ";
		for (int i = 0; i < m_saCampos.Count; i++)
		{
			if (i != 0)
			{
				text += ",";
			}
			string text2 = (((bool)m_baQuoted[i]) ? GetValor(m_saValores[i].ToString()) : m_saValores[i].ToString());
			object obj = text;
			text = string.Concat(obj, m_saCampos[i], "=", text2);
		}
		if (condicion.Length != 0)
		{
			text = text + " WHERE " + condicion;
		}
		return text;
	}

	public string GetInsert()
	{
		if (m_saValores.Count == 0)
		{
			return "";
		}
		string text = "INSERT INTO " + m_szTabla + " (";
		for (int i = 0; i < m_saCampos.Count; i++)
		{
			if (i != 0)
			{
				text += ",";
			}
			text += m_saCampos[i];
		}
		text += ") VALUES (";
		for (int i = 0; i < m_saValores.Count; i++)
		{
			if (i != 0)
			{
				text += ",";
			}
			text += (((bool)m_baQuoted[i]) ? GetValor(m_saValores[i].ToString()) : m_saValores[i].ToString());
		}
		return text + ");";
	}

	public string GetSelect(string condicion)
	{
		return GetSelect(condicion, "");
	}

	public string GetSelect(string condicion, string join)
	{
		if (m_saValores.Count == 0)
		{
			return "";
		}
		string text = "SELECT ";
		for (int i = 0; i < m_saCampos.Count; i++)
		{
			if (i != 0)
			{
				text += ",";
			}
			text += m_saValores[i];
			if (m_saCampos[i] != m_saValores[i])
			{
				object obj = text;
				text = string.Concat(obj, " AS '", m_saCampos[i], "'");
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
			if (HttpContext.Current.Request.Url.Host == "localhost")
			{
				return true;
			}
			if (string.IsNullOrEmpty(ConfigurationSettings.AppSettings["DomainKey"]))
			{
				return false;
			}
			return Crypt.GetMD5(Crypt.Encriptar3DES(HttpContext.Current.Request.Url.Host.PadRight(29, 'x').PadRight(37, '?').Replace("x", ";"), licenseSeed.Replace("2", "1").Replace("2", "6"))) == ConfigurationSettings.AppSettings["DomainKey"] || Crypt.GetMD5(Crypt.Encriptar3DES(HttpContext.Current.Request.Url.Host.PadRight(29, 'x').PadRight(37, '?').Replace("x", ";"), licenseSeed.Replace("2", "1").Replace("2", "6"))) == ConfigurationSettings.AppSettings["DomainKey1"];
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
