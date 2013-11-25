using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace SDG.Web.UI.WebControls;

public class CustomDataSourceView : DataSourceView, IStateManager
{
	private bool _tracking;

	protected CustomDataSource _owner;

	protected string _typeName;

	protected string _selectMethod;

	protected ParameterCollection _selectParameters;

	protected ParameterCollection _UpdateParameters;

	protected ParameterCollection _InsertParameters;

	protected bool _init;

	public override bool CanInsert => false;

	public override bool CanUpdate => false;

	public override bool CanDelete => false;

	public override bool CanRetrieveTotalRowCount => false;

	public override bool CanSort => false;

	public override bool CanPage => false;

	public string TypeName
	{
		get
		{
			if (_typeName == null)
			{
				return string.Empty;
			}
			return _typeName;
		}
		set
		{
			if (TypeName != value)
			{
				_typeName = value;
				OnDataSourceViewChanged(EventArgs.Empty);
			}
		}
	}

	public string SelectMethod
	{
		get
		{
			if (_selectMethod == null)
			{
				return string.Empty;
			}
			return _selectMethod;
		}
		set
		{
			if (SelectMethod != value)
			{
				_selectMethod = value;
				OnDataSourceViewChanged(EventArgs.Empty);
			}
		}
	}

	public ParameterCollection UpdateParameters
	{
		get
		{
			if (_UpdateParameters == null)
			{
				_UpdateParameters = new ParameterCollection();
				_UpdateParameters.ParametersChanged += ParametersChangedEventHandler;
				if (_tracking)
				{
					((IStateManager)_UpdateParameters).TrackViewState();
				}
			}
			return _UpdateParameters;
		}
	}

	public ParameterCollection InsertParameters
	{
		get
		{
			if (_InsertParameters == null)
			{
				_InsertParameters = new ParameterCollection();
				_InsertParameters.ParametersChanged += ParametersChangedEventHandler;
				if (_tracking)
				{
					((IStateManager)_InsertParameters).TrackViewState();
				}
			}
			return _InsertParameters;
		}
	}

	public ParameterCollection SelectParameters
	{
		get
		{
			if (_selectParameters == null)
			{
				_selectParameters = new ParameterCollection();
				_selectParameters.ParametersChanged += ParametersChangedEventHandler;
				if (_tracking)
				{
					((IStateManager)_selectParameters).TrackViewState();
				}
			}
			return _selectParameters;
		}
	}

	bool IStateManager.IsTrackingViewState => _tracking;

	public CustomDataSourceView(CustomDataSource owner, string name)
		: base(owner, name)
	{
		_owner = owner;
	}

	void IStateManager.LoadViewState(object savedState)
	{
		LoadViewState(savedState);
	}

	object IStateManager.SaveViewState()
	{
		return SaveViewState();
	}

	void IStateManager.TrackViewState()
	{
		TrackViewState();
	}

	protected virtual void LoadViewState(object savedState)
	{
		if (savedState != null && savedState != null)
		{
			((IStateManager)SelectParameters).LoadViewState(savedState);
			((IStateManager)InsertParameters).LoadViewState(savedState);
			((IStateManager)UpdateParameters).LoadViewState(savedState);
		}
	}

	protected virtual object SaveViewState()
	{
		if (_UpdateParameters != null)
		{
			((IStateManager)_UpdateParameters).SaveViewState();
		}
		if (_InsertParameters != null)
		{
			((IStateManager)_InsertParameters).SaveViewState();
		}
		if (_selectParameters != null)
		{
			return ((IStateManager)_selectParameters).SaveViewState();
		}
		return null;
	}

	protected virtual void TrackViewState()
	{
		_tracking = true;
		if (_selectParameters != null)
		{
			((IStateManager)_selectParameters).TrackViewState();
		}
		if (_InsertParameters != null)
		{
			((IStateManager)_InsertParameters).TrackViewState();
		}
		if (_UpdateParameters != null)
		{
			((IStateManager)_UpdateParameters).TrackViewState();
		}
	}

	protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
	{
		if (_owner.SelectCommand.Length == 0)
		{
			throw new InvalidOperationException(_owner.ID + ": There isn't a SelectCommand defined");
		}
		arguments.RaiseUnsupportedCapabilitiesError(this);
		IOrderedDictionary orderedDictionary = null;
		try
		{
			orderedDictionary = SelectParameters.GetValues(HttpContext.Current, _owner);
		}
		catch
		{
		}
		string text = _owner.SelectCommand;
		SessionState sessionState2;
		if (_owner.ParentDataSource.Length > 0)
		{
			SessionState sessionState = SessionState.Get(_owner.ModifyTable);
			for (int i = 0; i < sessionState.dataTable.Rows.Count; i++)
			{
				bool flag = true;
				DataRow dataRow = sessionState.dataTable.Rows[i];
				if (orderedDictionary == null)
				{
					continue;
				}
				foreach (DictionaryEntry item in orderedDictionary)
				{
					if (sessionState.dataTable.Columns.IndexOf(item.Key.ToString()) > -1 && dataRow[item.Key.ToString()].ToString() != item.Value.ToString())
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					sessionState2 = new SessionState(_owner.ID, text);
					for (int j = 0; j < sessionState.dataTable.Columns.Count; j++)
					{
						sessionState2.dataTable.Columns.Add(sessionState.dataTable.Columns[j].Caption, sessionState.dataTable.Columns[j].DataType);
					}
					sessionState2.dataTable.Rows.Add(dataRow.ItemArray);
					if (sessionState2.dataTable.Rows.Count == 0)
					{
						return null;
					}
					return sessionState2.dataTable.AsDataView();
				}
			}
		}
		if (orderedDictionary != null)
		{
			foreach (object key in orderedDictionary.Keys)
			{
				_ = orderedDictionary[key];
				text = text.Replace("@" + key.ToString(), SessionState.GetValor(orderedDictionary[key]));
			}
		}
		Page page = (Page)HttpContext.Current.Handler;
		if (page.IsPostBack || _init)
		{
			sessionState2 = SessionState.Get(_owner.ID, text);
			if (sessionState2 != null)
			{
				if (sessionState2.dataTable.Rows.Count == 0)
				{
					return null;
				}
				return sessionState2.dataTable.AsDataView();
			}
		}
		_init = true;
		sessionState2 = new SessionState(_owner.ID, text);
		MySqlConnection mySqlConnection;
		try
		{
			mySqlConnection = new MySqlConnection(_owner.ConnectionString);
			mySqlConnection.Open();
		}
		catch (MySqlException innerException)
		{
			throw new InvalidOperationException(_owner.ID + ": Error en la consulta", innerException);
		}
		MySqlDataReader mySqlDataReader = null;
		try
		{
			MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
			mySqlCommand.CommandText = text;
			mySqlCommand.CommandTimeout = 0;
			mySqlDataReader = mySqlCommand.ExecuteReader();
		}
		catch (MySqlException innerException2)
		{
			throw new InvalidOperationException(_owner.ID + ": Error en la consulta", innerException2);
		}
		if (mySqlDataReader != null)
		{
			for (int k = 0; k < mySqlDataReader.FieldCount; k++)
			{
				sessionState2.dataTable.Columns.Add(mySqlDataReader.GetName(k), mySqlDataReader.GetFieldType(k));
			}
			sessionState2.dataTable.Columns.Add("sqlexists");
			object[] values = new object[mySqlDataReader.FieldCount + 1];
			while (mySqlDataReader.Read())
			{
				mySqlDataReader.GetValues(values);
				DataRow dataRow2 = sessionState2.dataTable.Rows.Add(values);
				dataRow2["sqlexists"] = true;
			}
		}
		mySqlDataReader.Close();
		mySqlConnection.Close();
		mySqlConnection.Dispose();
		if (sessionState2.dataTable.Rows.Count == 0)
		{
			return null;
		}
		return sessionState2.dataTable.AsDataView();
	}

	protected override int ExecuteInsert(IDictionary values)
	{
		if (_owner.InsertCommand.Length == 0)
		{
			throw new InvalidOperationException(_owner.ID + ": There isn't a InsertCommand defined");
		}
		string text = _owner.InsertCommand;
		SessionState sessionState = SessionState.Get(_owner.ModifyTable);
		DataRow dataRow = sessionState.dataTable.Rows.Add(new object[sessionState.dataTable.Columns.Count]);
		foreach (DictionaryEntry value in values)
		{
			dataRow[value.Key.ToString()] = ((value.Value == null) ? DBNull.Value : value.Value);
			text = text.Replace("@" + value.Key.ToString(), SessionState.GetValor(value.Value));
		}
		try
		{
			IOrderedDictionary values2 = InsertParameters.GetValues(HttpContext.Current, _owner);
			foreach (DictionaryEntry item in values2)
			{
				if (sessionState.dataTable.Columns.IndexOf(item.Key.ToString()) > -1)
				{
					dataRow[item.Key.ToString()] = ((item.Value == null) ? DBNull.Value : item.Value);
				}
				text = text.Replace("@" + item.Key.ToString(), SessionState.GetValor(item.Value));
			}
		}
		catch
		{
		}
		try
		{
			IOrderedDictionary values3 = SelectParameters.GetValues(HttpContext.Current, _owner);
			foreach (DictionaryEntry item2 in values3)
			{
				if (sessionState.dataTable.Columns.IndexOf(item2.Key.ToString()) <= -1 || dataRow[item2.Key.ToString()].ToString().Length != 0)
				{
					continue;
				}
				string text2 = new Random(DateTime.Now.Millisecond).Next().ToString();
				for (int i = 0; i < sessionState.dataTable.Rows.Count; i++)
				{
					if (dataRow[item2.Key.ToString()].ToString() == text2)
					{
						text2 = new Random(DateTime.Now.Millisecond).Next().ToString();
						i = -1;
					}
				}
				dataRow[item2.Key.ToString()] = text2;
			}
		}
		catch
		{
		}
		sessionState.AddQuery(text, sessionState.dataTable.Rows.Count - 1);
		return 1;
	}

	protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
	{
		if (_owner.UpdateCommand.Length == 0)
		{
			throw new InvalidOperationException(_owner.ID + ": There isn't a UpdateCommand defined");
		}
		string text = _owner.UpdateCommand;
		SessionState sessionState = SessionState.Get(_owner.ModifyTable);
		DataRow dataRow = null;
		int num = -1;
		for (int i = 0; i < sessionState.dataTable.Rows.Count; i++)
		{
			bool flag = true;
			foreach (DictionaryEntry oldValue in oldValues)
			{
				string text2 = sessionState.dataTable.Rows[i][oldValue.Key.ToString()].ToString();
				string text3 = ((oldValue.Value == null) ? "" : oldValue.Value.ToString());
				if (!(text2 != text3))
				{
					continue;
				}
				bool flag2 = false;
				try
				{
					flag2 = Convert.ToDouble(text2) == Convert.ToDouble(text3);
				}
				catch
				{
				}
				if (!flag2)
				{
					try
					{
						flag2 = Convert.ToDateTime(text2) == Convert.ToDateTime(text3);
					}
					catch
					{
					}
					if (!flag2)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				dataRow = sessionState.dataTable.Rows[i];
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return 0;
		}
		if (sessionState.CleanQuerys(num))
		{
			text = _owner.InsertCommand;
			try
			{
				IOrderedDictionary values2 = InsertParameters.GetValues(HttpContext.Current, _owner);
				foreach (DictionaryEntry item in values2)
				{
					if (sessionState.dataTable.Columns.IndexOf(item.Key.ToString()) > -1)
					{
						dataRow[item.Key.ToString()] = ((item.Value == null) ? DBNull.Value : item.Value);
					}
					text = text.Replace("@" + item.Key.ToString(), SessionState.GetValor(item.Value));
				}
			}
			catch
			{
			}
		}
		foreach (DictionaryEntry key in keys)
		{
			dataRow[key.Key.ToString()] = ((key.Value == null) ? DBNull.Value : key.Value);
			text = text.Replace("@" + key.Key.ToString(), SessionState.GetValor(key.Value));
		}
		foreach (DictionaryEntry value in values)
		{
			dataRow[value.Key.ToString()] = ((value.Value == null) ? DBNull.Value : value.Value);
			text = text.Replace("@" + value.Key.ToString(), SessionState.GetValor(value.Value));
		}
		try
		{
			IOrderedDictionary values3 = UpdateParameters.GetValues(HttpContext.Current, _owner);
			foreach (DictionaryEntry item2 in values3)
			{
				dataRow[item2.Key.ToString()] = ((item2.Value == null) ? DBNull.Value : item2.Value);
				text = text.Replace("@" + item2.Key.ToString(), SessionState.GetValor(item2.Value));
			}
		}
		catch
		{
		}
		sessionState.AddQuery(text, num);
		return 1;
	}

	protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
	{
		if (_owner.DeleteCommand.Length == 0)
		{
			throw new InvalidOperationException(_owner.ID + ": There isn't a DeleteCommand defined");
		}
		string text = _owner.DeleteCommand;
		SessionState sessionState = SessionState.Get(_owner.ModifyTable);
		DataRow dataRow = null;
		int num = -1;
		for (int i = 0; i < sessionState.dataTable.Rows.Count; i++)
		{
			bool flag = true;
			foreach (DictionaryEntry oldValue in oldValues)
			{
				string text2 = sessionState.dataTable.Rows[i][oldValue.Key.ToString()].ToString();
				string text3 = ((oldValue.Value == null) ? "" : oldValue.Value.ToString());
				if (!(text2 != text3))
				{
					continue;
				}
				bool flag2 = false;
				try
				{
					flag2 = Convert.ToDouble(text2) == Convert.ToDouble(text3);
				}
				catch
				{
				}
				if (!flag2)
				{
					try
					{
						flag2 = Convert.ToDateTime(text2) == Convert.ToDateTime(text3);
					}
					catch
					{
					}
					if (!flag2)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				num = i;
				dataRow = sessionState.dataTable.Rows[i];
				break;
			}
		}
		if (num == -1)
		{
			return 0;
		}
		if (sessionState.CleanQuerys(num))
		{
			sessionState.DeleteRow(num);
			return 1;
		}
		foreach (DictionaryEntry key in keys)
		{
			dataRow[key.Key.ToString()] = ((key.Value == null) ? DBNull.Value : key.Value);
			text = text.Replace("@" + key.Key.ToString(), SessionState.GetValor(key.Value));
		}
		sessionState.AddQuery(text, num);
		sessionState.DeleteRow(num);
		return 1;
	}

	public IEnumerable Select(DataSourceSelectArguments args)
	{
		return ExecuteSelect(args);
	}

	protected void ParametersChangedEventHandler(object o, EventArgs e)
	{
		OnDataSourceViewChanged(EventArgs.Empty);
	}
}
