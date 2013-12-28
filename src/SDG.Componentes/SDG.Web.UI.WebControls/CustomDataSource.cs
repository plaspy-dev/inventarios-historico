using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;

namespace SDG.Web.UI.WebControls;

[Designer(typeof(CustomDataSourceDesigner))]
[PersistChildren(false)]
[ParseChildren(true)]
public class CustomDataSource : DataSourceControl
{
	protected static readonly string[] _views = new string[1] { "DefaultView" };

	protected string _ConnectionString = "";

	protected string _SelectCommand = "";

	protected string _DeleteCommand = "";

	protected string _UpdateCommand = "";

	protected string _InsertCommand = "";

	protected string _ParentDataSource = "";

	protected CustomDataSourceView _view;

	[Category("ParentDataSource")]
	[DefaultValue("")]
	public string ParentDataSource
	{
		get
		{
			return _ParentDataSource;
		}
		set
		{
			_ParentDataSource = value;
		}
	}

	[DefaultValue("")]
	[Category("InsertCommand")]
	public string InsertCommand
	{
		get
		{
			return _InsertCommand;
		}
		set
		{
			_InsertCommand = value;
		}
	}

	[DefaultValue("")]
	[Category("SelectCommand")]
	public string SelectCommand
	{
		get
		{
			return _SelectCommand;
		}
		set
		{
			_SelectCommand = value;
		}
	}

	[Category("DeleteCommand")]
	[DefaultValue("")]
	public string DeleteCommand
	{
		get
		{
			return _DeleteCommand;
		}
		set
		{
			_DeleteCommand = value;
		}
	}

	[DefaultValue("")]
	[Category("UpdateCommand")]
	public string UpdateCommand
	{
		get
		{
			return _UpdateCommand;
		}
		set
		{
			_UpdateCommand = value;
		}
	}

	[DefaultValue("")]
	[Category("ConnectionString")]
	public string ConnectionString
	{
		get
		{
			return _ConnectionString;
		}
		set
		{
			_ConnectionString = value;
		}
	}

	[Category("Data")]
	[DefaultValue("")]
	public string TypeName
	{
		get
		{
			return View.TypeName;
		}
		set
		{
			View.TypeName = value;
		}
	}

	[DefaultValue("")]
	[Category("Data")]
	public string SelectMethod
	{
		get
		{
			return View.SelectMethod;
		}
		set
		{
			View.SelectMethod = value;
		}
	}

	[Category("Data")]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[DefaultValue(null)]
	[MergableProperty(false)]
	[Editor(typeof(ParameterCollectionEditor), typeof(UITypeEditor))]
	public ParameterCollection SelectParameters => View.SelectParameters;

	[DefaultValue(null)]
	[Editor(typeof(ParameterCollectionEditor), typeof(UITypeEditor))]
	[Category("Data")]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[MergableProperty(false)]
	public ParameterCollection InsertParameters => View.InsertParameters;

	[Category("Data")]
	[Editor(typeof(ParameterCollectionEditor), typeof(UITypeEditor))]
	[DefaultValue(null)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[MergableProperty(false)]
	public ParameterCollection UpdateParameters => View.UpdateParameters;

	protected CustomDataSourceView View
	{
		get
		{
			if (_view == null)
			{
				_view = new CustomDataSourceView(this, _views[0]);
				if (base.IsTrackingViewState)
				{
					((IStateManager)_view).TrackViewState();
				}
			}
			return _view;
		}
	}

	public string ModifyTable => (ParentDataSource.Length == 0) ? ID : ParentDataSource;

	protected override DataSourceView GetView(string viewName)
	{
		if (viewName == null || (viewName.Length != 0 && string.Compare(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase) != 0))
		{
			throw new ArgumentException("An invalid view was requested", "viewName");
		}
		return View;
	}

	protected override ICollection GetViewNames()
	{
		return _views;
	}

	protected override void LoadViewState(object savedState)
	{
		Pair pair = (Pair)savedState;
		if (savedState == null)
		{
			base.LoadViewState(null);
			return;
		}
		base.LoadViewState(pair.First);
		if (pair.Second != null)
		{
			((IStateManager)View).LoadViewState(pair.Second);
		}
	}

	protected override object SaveViewState()
	{
		Pair pair = new Pair();
		pair.First = base.SaveViewState();
		if (_view != null)
		{
			pair.Second = ((IStateManager)View).SaveViewState();
		}
		if (pair.First == null && pair.Second == null)
		{
			return null;
		}
		return pair;
	}

	protected override void TrackViewState()
	{
		base.TrackViewState();
		if (_view != null)
		{
			((IStateManager)View).TrackViewState();
		}
	}

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		if (Page != null)
		{
			Page.LoadComplete += UpdateParameterValues;
		}
	}

	protected virtual void UpdateParameterValues(object sender, EventArgs e)
	{
		SelectParameters.UpdateValues(Context, this);
	}

	public IEnumerable Select()
	{
		return View.Select(DataSourceSelectArguments.Empty);
	}

	public CustomDataSourceView GetView()
	{
		return View;
	}

	public DataTable GetTable()
	{
		return SessionState.Get(ID)?.dataTable;
	}

	public string[] GetQuerys()
	{
		SessionState sessionState = SessionState.Get(ID);
		if (sessionState != null)
		{
			string[] querys = sessionState.GetQuerys();
			SessionState.Clean(ID);
			return querys;
		}
		return null;
	}

	public bool Apply()
	{
		return SessionState.Get(ID)?.Aplicar(ConnectionString) ?? false;
	}
}
