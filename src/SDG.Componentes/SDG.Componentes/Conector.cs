using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace SDG.Componentes;

public class Conector
{
	protected string m_szLastError;

	protected string m_szQuery;

	protected string m_szConexion;

	protected bool m_bTransaccion = false;

	protected MySqlConnection m_pSQLConector;

	protected MySqlDataReader m_pSQLReader;

	public Conector(string conexionstring)
	{
		m_szQuery = "";
		m_szLastError = "";
		m_pSQLConector = null;
		m_pSQLReader = null;
		m_szConexion = conexionstring;
	}

	~Conector()
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

	public MySqlConnection GetConexion()
	{
		return m_pSQLConector;
	}

	public static string GetCookie(string cookie)
	{
		return GetCookie(cookie, "");
	}

	public static string GetCookie(string cookie, string defecto)
	{
		string result = defecto;
		if (HttpContext.Current.Request.Cookies[cookie] != null)
		{
			result = HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[cookie].Value);
		}
		return result;
	}

	public static int GetCookie(string cookie, int defecto)
	{
		return Convert.ToInt32(GetCookie(cookie, defecto.ToString()));
	}

	public static void SetCookie(string cookie, string valor)
	{
		SetCookie(cookie, valor, DateTime.Now.AddYears(1));
	}

	public static void SetCookie(string cookie, string valor, DateTime expires)
	{
		HttpContext.Current.Response.Cookies[cookie].Value = HttpUtility.UrlEncode(valor);
		HttpContext.Current.Response.Cookies[cookie].Expires = expires;
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
		if (m_pSQLConector == null)
		{
			string szConexion = m_szConexion;
			try
			{
				m_pSQLConector = new MySqlConnection(szConexion);
				m_pSQLConector.Open();
			}
			catch (MySqlException ex)
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

	public MySqlDataAdapter QueryAdaptador(string query)
	{
		return new MySqlDataAdapter(query, m_szConexion);
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
			MySqlCommand mySqlCommand = m_pSQLConector.CreateCommand();
			mySqlCommand.CommandText = query;
			num = mySqlCommand.ExecuteNonQuery();
		}
		catch (MySqlException ex)
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
			if (Query("SELECT LAST_INSERT_ID()") != null && m_pSQLReader.Read())
			{
				num = Convert.ToInt32(m_pSQLReader[0].ToString());
			}
		}
		return num;
	}

	public string QueryString(string query)
	{
		MySqlDataReader mySqlDataReader = Query(query);
		string result = "";
		if (mySqlDataReader != null && mySqlDataReader.Read())
		{
			result = ((mySqlDataReader[0].GetType() != typeof(byte[])) ? mySqlDataReader[0].ToString() : Encoding.UTF8.GetString((byte[])mySqlDataReader[0]));
		}
		try
		{
			mySqlDataReader.Close();
		}
		catch
		{
		}
		return result;
	}

	public MySqlDataReader Query(string query)
	{
		return Query(query, reset: false);
	}

	public MySqlDataReader Query(string query, bool reset)
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
			MySqlCommand mySqlCommand = m_pSQLConector.CreateCommand();
			mySqlCommand.CommandText = query;
			mySqlCommand.CommandTimeout = 0;
			m_pSQLReader = mySqlCommand.ExecuteReader();
		}
		catch (MySqlException ex)
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

	public static bool SelectInComboText(DropDownList lista, string texto)
	{
		bool result = true;
		if (lista.Items.Count < 2)
		{
			lista.DataBind();
		}
		lista.SelectedIndex = -1;
		ListItem listItem = lista.Items.FindByText(texto);
		if (listItem == null)
		{
			result = false;
		}
		else
		{
			listItem.Selected = true;
		}
		return result;
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
		return dateTime.Year + "-" + text + "-" + text2;
	}

	public static string GetHora(object fecha)
	{
		if (fecha == null || fecha.ToString().Length == 0)
		{
			return "";
		}
		DateTime dateTime = (DateTime)fecha;
		string text = ((dateTime.Hour < 10) ? ("0" + dateTime.Hour) : dateTime.Hour.ToString());
		string text2 = ((dateTime.Minute < 10) ? ("0" + dateTime.Minute) : dateTime.Minute.ToString());
		return text + ":" + text2;
	}

	public static string GetDateFormat(string fecha)
	{
		if (fecha.Length == 0 || fecha.Length < 10)
		{
			return "";
		}
		return fecha.Substring(0, 10);
	}

	public static string GetModificacion(string idUsuario, string idDocumento, string id2, string tabla, string url)
	{
		Query query = new Query(tabla);
		query["id"] = idDocumento;
		query["fecha"] = "SYSDATE()";
		query["url"] = url;
		query["usuario"] = idUsuario;
		if (!string.IsNullOrEmpty(id2))
		{
			query["id2"] = id2;
		}
		return query.GetInsert();
	}

	public static string GetModificacion(string idUsuario, string idDocumento, string id2, string tabla)
	{
		return GetModificacion(idUsuario, idDocumento, id2, tabla, HttpContext.Current.Request.Url.AbsolutePath.Replace(HttpContext.Current.Request.ApplicationPath, "~"));
	}

	public static string GetModificacion(string idUsuario, string idDocumento, string id2)
	{
		return GetModificacion(idUsuario, idDocumento, id2, "modificaciones");
	}

	public int InsertModificacion(string idDocumento, string id2)
	{
		Usuario usuario = Usuario.GetUsuario();
		return QueryMod(GetModificacion(usuario.GetID(), idDocumento, id2));
	}

	public int InsertModificacion(string idDocumento)
	{
		Usuario usuario = Usuario.GetUsuario();
		return QueryMod(GetModificacion(usuario.GetID(), idDocumento, null));
	}

	public int InsertModificacion(string idDocumento, string id2, string url)
	{
		Usuario usuario = Usuario.GetUsuario();
		return QueryMod(GetModificacion(usuario.GetID(), idDocumento, id2, "modificaciones", url));
	}

	public int InsertModificacionWeb(string idDocumento)
	{
		Usuario usuario = Usuario.GetUsuario();
		return QueryMod(GetModificacion(usuario.GetID(), idDocumento, null, "modificacionesweb"));
	}

	public static string GetMySQLDateFormat(DateTime fecha)
	{
		return GetMySQLDateFormat(fecha, horas: false);
	}

	public static string GetMySQLDateFormat(DateTime fecha, bool horas)
	{
		bool flag = 0 == 0;
		string text = fecha.Year + "-" + fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0');
		if (horas)
		{
			string text2 = text;
			text = text2 + " " + fecha.Hour.ToString().PadLeft(2, '0') + ":" + fecha.Minute.ToString().PadLeft(2, '0') + ":" + fecha.Second.ToString().PadLeft(2, '0');
		}
		return text;
	}

	public static string GetMySQLDateFormat(string text)
	{
		return GetMySQLDateFormat(text, horas: false);
	}

	public static string GetMySQLDateFormat(string text, bool horas)
	{
		try
		{
			return GetMySQLDateFormat(Convert.ToDateTime(text), horas);
		}
		catch
		{
		}
		return "";
	}

	public static string GetFromMySQLDate(object fecha)
	{
		try
		{
			if (typeof(MySqlDateTime) == fecha.GetType())
			{
				fecha = ((MySqlDateTime)fecha).GetDateTime();
			}
			return ((DateTime)fecha).ToShortDateString();
		}
		catch
		{
		}
		return "";
	}

	public static bool SelectInDropDown(DropDownList lista, object valor)
	{
		return new Conector("").SelectInDropDown(lista, valor, "");
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
			Conector conector = new Conector(m_szConexion);
			string text = "";
			if (query.Length > 0)
			{
				if (query.IndexOf("{") != -1 && query.IndexOf("}") != -1)
				{
					query = string.Format(query, valor);
				}
				text = conector.QueryString(query);
			}
			listItem = new ListItem(text, valor.ToString());
		}
		listItem.Selected = true;
		return result;
	}

	public bool StartTransaction()
	{
		m_bTransaccion = true;
		return QueryMod("START TRANSACTION;") != -1;
	}

	public bool RoolBack()
	{
		m_bTransaccion = false;
		return QueryMod("ROLLBACK;") != -1;
	}

	public bool Commit()
	{
		m_bTransaccion = false;
		return QueryMod("COMMIT;") != -1;
	}
}
