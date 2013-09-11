using System;
using System.Collections;
using System.Data;
using System.Globalization;
using MySql.Data.MySqlClient;
using SDG.Componentes;

namespace SDG.Web.UI.WebControls;

[Serializable]
internal class SessionState
{
	[Serializable]
	private class Querys
	{
		public int pos;

		public string query;

		public Querys(int pos, string query)
		{
			this.pos = pos;
			this.query = query;
		}
	}

	public ArrayList aCambios = new ArrayList();

	public string szQuery = "";

	public DataTable dataTable;

	public SessionState(string owner, string query)
	{
		szQuery = query;
		dataTable = new DataTable();
		Utilidades.GetViewState()["SessionState_" + owner] = this;
	}

	public static string GetValor(object valor)
	{
		if (valor == null)
		{
			return "NULL";
		}
		if (valor is float)
		{
			return GetValor(((decimal)valor).ToString(CultureInfo.InvariantCulture));
		}
		if (valor is double num)
		{
			return GetValor(num.ToString(CultureInfo.InvariantCulture));
		}
		if (valor is decimal num2)
		{
			return GetValor(num2.ToString(CultureInfo.InvariantCulture));
		}
		if (valor is DateTime)
		{
			return GetValor(Conector.GetMySQLDateFormat((DateTime)valor, horas: true));
		}
		return GetValor(valor.ToString());
	}

	public static string GetValor(string valor)
	{
		if (valor == null || valor.Length == 0)
		{
			return "NULL";
		}
		switch (valor)
		{
		case "SYSDATE()":
			return valor;
		case "True":
			return "true";
		case "False":
			return "false";
		default:
		{
			string text = valor;
			text = text.Replace("\\", "\\\\");
			text = text.Replace("'", "\\'");
			return "'" + text + "'";
		}
		}
	}

	public string[] GetQuerys()
	{
		string[] array = new string[aCambios.Count];
		for (int i = 0; i < aCambios.Count; i++)
		{
			array[i] = ((Querys)aCambios[i]).query;
		}
		return array;
	}

	public bool Aplicar(string conexion)
	{
		if (aCambios.Count == 0)
		{
			return true;
		}
		MySqlConnection mySqlConnection;
		try
		{
			mySqlConnection = new MySqlConnection(conexion);
			mySqlConnection.Open();
		}
		catch (MySqlException innerException)
		{
			throw new InvalidOperationException(conexion + ": Error en la consulta", innerException);
		}
		for (int i = 0; i < aCambios.Count; i++)
		{
			Querys querys = (Querys)aCambios[i];
			try
			{
				MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
				mySqlCommand.CommandText = querys.query;
				int num = mySqlCommand.ExecuteNonQuery();
				if (num == -1)
				{
					return false;
				}
			}
			catch (MySqlException innerException)
			{
				throw new InvalidOperationException(querys.query + ": Error en la consulta", innerException);
			}
		}
		return true;
	}

	public bool CleanQuerys(int pos)
	{
		if (pos == -1)
		{
			return false;
		}
		bool result = false;
		DataRow dataRow = dataTable.Rows[pos];
		if (dataRow["sqlexists"].ToString().Length == 0)
		{
			for (int i = 0; i < aCambios.Count; i++)
			{
				Querys querys = (Querys)aCambios[i];
				if (querys.pos == pos)
				{
					aCambios.RemoveAt(i);
					i--;
				}
				else if (querys.pos > pos)
				{
					querys.pos--;
				}
			}
			result = true;
		}
		return result;
	}

	public void DeleteRow(int pos)
	{
		DataRow dataRow = dataTable.Rows[pos];
		dataRow.Delete();
	}

	public void AddQuery(string query, int pos)
	{
		aCambios.Add(new Querys(pos, query));
	}

	public static void Clean(string owner)
	{
		Utilidades.GetViewState()["SessionState_" + owner] = null;
	}

	public static SessionState Get(string owner)
	{
		return (SessionState)Utilidades.GetViewState()["SessionState_" + owner];
	}

	public static SessionState Get(string owner, string query)
	{
		SessionState sessionState = (SessionState)Utilidades.GetViewState()["SessionState_" + owner];
		if (sessionState == null || sessionState.szQuery != query)
		{
			return null;
		}
		return sessionState;
	}
}
