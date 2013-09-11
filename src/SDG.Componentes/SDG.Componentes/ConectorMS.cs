using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SDG.Componentes;

public class ConectorMS : IDisposable
{
	private static bool bGetKey = false;

	protected string m_szLastError;

	protected string m_szQuery;

	protected string m_szConexion;

	public bool m_bTransaccion = false;

	protected SqlConnection m_pSQLConector;

	protected SqlDataReader m_pSQLReader;

	public ConectorMS(string conexion)
	{
		m_szQuery = "";
		m_szLastError = "";
		m_pSQLConector = null;
		m_pSQLReader = null;
		m_szConexion = conexion;
	}

	~ConectorMS()
	{
		CloseAll();
	}

	void IDisposable.Dispose()
	{
		CloseAll();
	}

	public void Dispose()
	{
		CloseAll();
	}

	private void CloseAll()
	{
		if (m_bTransaccion)
		{
			RoolBack();
		}
		try
		{
			if (m_pSQLReader != null)
			{
				m_pSQLReader.Close();
			}
		}
		catch
		{
		}
		try
		{
			if (m_pSQLConector != null)
			{
				m_pSQLConector.Close();
			}
		}
		catch
		{
		}
	}

	public string GetConexionString()
	{
		return m_szConexion;
	}

	public SqlConnection GetConexion()
	{
		return m_pSQLConector;
	}

	public void ShowError(string error)
	{
		try
		{
			Page page = (Page)HttpContext.Current.Handler;
			if (page != null && page.Master != null)
			{
				Label label = (Label)page.Master.FindControl("LMensaje");
				if (label != null)
				{
					label.Text = error;
				}
			}
		}
		catch
		{
		}
	}

	protected bool IsConect()
	{
		if (m_pSQLConector == null && GetKey())
		{
			string szConexion = m_szConexion;
			try
			{
				m_pSQLConector = new SqlConnection(szConexion);
				m_pSQLConector.Open();
			}
			catch (SqlException ex)
			{
				m_szLastError = ex.Message;
				ShowError(m_szLastError);
				return false;
			}
			catch
			{
			}
		}
		return true;
	}

	public SqlDataAdapter QueryAdaptador(string query)
	{
		return new SqlDataAdapter(query, m_szConexion);
	}

	public int QueryMod(string query)
	{
		return QueryMod(query, identity: false);
	}

	public int QueryMod(string query, bool identity)
	{
		m_szQuery = query;
		if (query.IndexOf(" WHERE") == 0)
		{
			return 0;
		}
		int num = 0;
		if (!IsConect())
		{
			return -1;
		}
		try
		{
			if (m_pSQLReader != null)
			{
				m_pSQLReader.Close();
				m_pSQLReader = null;
			}
		}
		catch
		{
		}
		try
		{
			SqlCommand sqlCommand = m_pSQLConector.CreateCommand();
			sqlCommand.CommandText = query;
			num = sqlCommand.ExecuteNonQuery();
		}
		catch (SqlException ex)
		{
			num = -1;
			m_szLastError = ex.Message;
			ShowError(m_szLastError + "\nEn la consulta:\n" + query);
		}
		catch
		{
		}
		if (identity && num != -1)
		{
			num = -1;
			if (Query("SELECT @@IDENTITY") != null && m_pSQLReader.Read())
			{
				num = Convert.ToInt32(m_pSQLReader[0].ToString());
			}
		}
		return num;
	}

	public string QueryString(string query)
	{
		SqlDataReader sqlDataReader = Query(query);
		string result = "";
		if (sqlDataReader != null && sqlDataReader.Read())
		{
			result = ((sqlDataReader[0].GetType() != typeof(byte[])) ? sqlDataReader[0].ToString() : Encoding.UTF8.GetString((byte[])sqlDataReader[0]));
		}
		try
		{
			sqlDataReader.Close();
		}
		catch
		{
		}
		return result;
	}

	public SqlDataReader Query(string query)
	{
		return Query(query, reset: false);
	}

	public SqlDataReader Query(string query, bool reset)
	{
		if (!IsConect())
		{
			return null;
		}
		if (query == null || query.Length == 0)
		{
			m_szLastError = "";
			return null;
		}
		m_szQuery = query;
		try
		{
			if (m_pSQLReader != null)
			{
				m_pSQLReader.Close();
				m_pSQLReader = null;
			}
		}
		catch
		{
		}
		try
		{
			SqlCommand sqlCommand = m_pSQLConector.CreateCommand();
			sqlCommand.CommandText = query;
			sqlCommand.CommandTimeout = 0;
			m_pSQLReader = sqlCommand.ExecuteReader();
		}
		catch (SqlException ex)
		{
			m_pSQLReader = null;
			m_szLastError = ex.Message;
			ShowError(m_szLastError + "\nEn la consulta:\n" + query);
		}
		catch
		{
		}
		return m_pSQLReader;
	}

	public int GetColCount()
	{
		if (m_pSQLReader == null)
		{
			return 0;
		}
		return m_pSQLReader.FieldCount;
	}

	public string GetLastError()
	{
		if (m_szLastError.Length == 0)
		{
			return "";
		}
		return m_szLastError + "<br>" + m_szQuery;
	}

	public static string GetCondicion(string campo, string dato)
	{
		return GetCondicion(campo, dato, "=");
	}

	public static string GetCondicion(string campo, string dato, string comparador)
	{
		return " WHERE " + campo + " " + comparador + " '" + dato + "'";
	}

	public static string GetDate(object fecha)
	{
		if (fecha == null || fecha.ToString().Length == 0)
		{
			return "";
		}
		DateTime dateTime = (DateTime)fecha;
		string text = ((dateTime.Month < 10) ? ("0" + dateTime.Month) : dateTime.Month.ToString());
		string text2 = ((dateTime.Day < 10) ? ("0" + dateTime.Day) : dateTime.Day.ToString());
		return text2 + "/" + text + "/" + dateTime.Year;
	}

	public static string GetDateFormat(string fecha)
	{
		if (fecha.Length == 0 || fecha.Length < 10)
		{
			return "";
		}
		return fecha.Substring(0, 10);
	}

	public bool SelectInDropDown(DropDownList lista, object valor)
	{
		return new ConectorMS("").SelectInDropDown(lista, valor, "");
	}

	public bool SelectInDropDown(DropDownList lista, object valor, string query)
	{
		bool result = true;
		if (lista.Items.Count < 2)
		{
			lista.DataBind();
		}
		lista.SelectedIndex = -1;
		ListItem listItem = lista.Items.FindByValue(valor.ToString());
		if (listItem == null)
		{
			ConectorMS conectorMS = new ConectorMS(m_szConexion);
			string text = "";
			if (query.Length > 0)
			{
				if (query.IndexOf("{") != -1 && query.IndexOf("}") != -1)
				{
					query = string.Format(query, valor);
				}
				text = conectorMS.QueryString(query);
			}
			listItem = new ListItem(text, valor.ToString());
			lista.Items.Add(listItem);
			result = false;
		}
		listItem.Selected = true;
		return result;
	}

	public bool StartTransaction()
	{
		m_bTransaccion = true;
		return QueryMod("BEGIN TRANSACTION;") != -1;
	}

	public bool RoolBack()
	{
		m_bTransaccion = false;
		bool result = QueryMod("ROLLBACK") != -1;
		CloseAll();
		return result;
	}

	public bool Commit()
	{
		m_bTransaccion = false;
		bool result = QueryMod("COMMIT;") != -1;
		CloseAll();
		return result;
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
