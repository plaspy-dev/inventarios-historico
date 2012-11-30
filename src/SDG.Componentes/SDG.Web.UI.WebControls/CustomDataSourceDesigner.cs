using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Web.UI.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SDG.Web.UI.WebControls;

public class CustomDataSourceDesigner : DataSourceDesigner
{
	private static readonly string[] _views = new string[1] { "DefaultView" };

	private CustomDesignerDataSourceView _view;

	private bool _inWizard;

	public override bool CanConfigure => true;

	public override bool CanRefreshSchema
	{
		get
		{
			if (!string.IsNullOrEmpty(TypeName) && !string.IsNullOrEmpty(SelectMethod))
			{
				return true;
			}
			return false;
		}
	}

	public string TypeName
	{
		get
		{
			return DataSourceComponent.TypeName;
		}
		set
		{
			if (string.Compare(DataSourceComponent.TypeName, value, ignoreCase: false) != 0)
			{
				DataSourceComponent.TypeName = value;
				if (CanRefreshSchema)
				{
					RefreshSchema(preferSilent: true);
				}
				else
				{
					OnDataSourceChanged(EventArgs.Empty);
				}
				UpdateDesignTimeHtml();
			}
		}
	}

	public string SelectMethod
	{
		get
		{
			return DataSourceComponent.SelectMethod;
		}
		set
		{
			if (string.Compare(DataSourceComponent.SelectMethod, value, ignoreCase: false) != 0)
			{
				DataSourceComponent.SelectMethod = value;
				if (CanRefreshSchema && !_inWizard)
				{
					RefreshSchema(preferSilent: true);
				}
				else
				{
					OnDataSourceChanged(EventArgs.Empty);
				}
				UpdateDesignTimeHtml();
			}
		}
	}

	internal CustomDataSource DataSourceComponent => base.Component as CustomDataSource;

	internal IDataSourceViewSchema DataSourceSchema
	{
		get
		{
			return base.DesignerState["DataSourceSchema"] as IDataSourceViewSchema;
		}
		set
		{
			base.DesignerState["DataSourceSchema"] = value;
		}
	}

	protected CustomDesignerDataSourceView View
	{
		get
		{
			if (_view == null)
			{
				_view = new CustomDesignerDataSourceView(this, _views[0]);
			}
			return _view;
		}
	}

	public override DesignerDataSourceView GetView(string viewName)
	{
		if (viewName == null || (viewName.Length != 0 && string.Compare(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase) != 0))
		{
			throw new ArgumentException("An invalid view was requested", "viewName");
		}
		return View;
	}

	public override string[] GetViewNames()
	{
		return _views;
	}

	public override void Configure()
	{
		_inWizard = true;
		System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(base.Component, ConfigureDataSourceCallback, null, "ConfigureDataSource");
		_inWizard = false;
	}

	protected virtual bool ConfigureDataSourceCallback(object context)
	{
		try
		{
			SuppressDataSourceEvents();
			IServiceProvider site = base.Component.Site;
			if (site == null)
			{
				return false;
			}
			IUIService iUIService = (IUIService)site.GetService(typeof(IUIService));
			if (iUIService == null)
			{
				return false;
			}
			ConfigureDataSource form = new ConfigureDataSource(site, this);
			if (iUIService.ShowDialog(form) == DialogResult.OK)
			{
				OnDataSourceChanged(EventArgs.Empty);
				return true;
			}
		}
		finally
		{
			ResumeDataSourceEvents();
		}
		return false;
	}

	public override void RefreshSchema(bool preferSilent)
	{
		Cursor current = Cursor.Current;
		try
		{
			SuppressDataSourceEvents();
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				Type type = GetType(base.Component.Site, TypeName);
				if (type == null)
				{
					return;
				}
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				MethodInfo methodInfo = null;
				MethodInfo[] array = methods;
				foreach (MethodInfo methodInfo2 in array)
				{
					if (IsMatchingMethod(methodInfo2, SelectMethod))
					{
						methodInfo = methodInfo2;
						break;
					}
				}
				if (methodInfo != null)
				{
					RefreshSchemaInternal(type, methodInfo.Name, methodInfo.ReturnType, preferSilent);
				}
			}
			finally
			{
				Cursor.Current = current;
			}
		}
		finally
		{
			ResumeDataSourceEvents();
		}
	}

	internal void RefreshSchemaInternal(Type typeName, string method, Type returnType, bool preferSilent)
	{
		if (typeName == null || string.IsNullOrEmpty(method) || returnType == null)
		{
			return;
		}
		try
		{
			IDataSourceViewSchema dataSourceSchema = DataSourceSchema;
			IDataSourceViewSchema[] views = new TypeSchema(returnType).GetViews();
			if (views == null || views.Length == 0)
			{
				DataSourceSchema = null;
				return;
			}
			IDataSourceViewSchema dataSourceViewSchema = views[0];
			if (!DataSourceDesigner.ViewSchemasEquivalent(dataSourceSchema, dataSourceViewSchema))
			{
				DataSourceSchema = dataSourceViewSchema;
				OnSchemaRefreshed(EventArgs.Empty);
			}
		}
		catch (Exception ex)
		{
			if (!preferSilent)
			{
				ShowError(DataSourceComponent.Site, "Cannot retrieve type schema for " + returnType.FullName + ". " + ex.Message);
			}
		}
	}

	protected override void PreFilterProperties(IDictionary properties)
	{
		base.PreFilterProperties(properties);
		PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor)properties["TypeName"];
		properties["TypeName"] = TypeDescriptor.CreateProperty(GetType(), oldPropertyDescriptor);
		PropertyDescriptor oldPropertyDescriptor2 = (PropertyDescriptor)properties["SelectMethod"];
		properties["SelectMethod"] = TypeDescriptor.CreateProperty(GetType(), oldPropertyDescriptor2);
	}

	private void ShowError(ISite serviceProvider, string message)
	{
		((IUIService)serviceProvider.GetService(typeof(IUIService)))?.ShowError(message);
	}

	private bool IsMatchingMethod(MethodInfo method, string selectMethod)
	{
		return string.Compare(method.Name, selectMethod, ignoreCase: true) == 0;
	}

	internal static Type GetType(IServiceProvider serviceProvider, string typeName)
	{
		return ((ITypeResolutionService)serviceProvider.GetService(typeof(ITypeResolutionService)))?.GetType(typeName, throwOnError: false, ignoreCase: true);
	}
}
