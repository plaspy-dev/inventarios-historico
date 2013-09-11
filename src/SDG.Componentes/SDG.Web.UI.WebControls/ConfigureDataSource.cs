using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace SDG.Web.UI.WebControls;

public class ConfigureDataSource : Form
{
	private IContainer components = null;

	private Label lInstructions;

	private Label label1;

	private ComboBox ddlTypes;

	private ComboBox ddlMethods;

	private Button bOK;

	private Button bCancel;

	private IServiceProvider _serviceProvider;

	private CustomDataSource _component;

	private CustomDataSourceDesigner _designer;

	internal string TypeName
	{
		get
		{
			if (!(ddlTypes.SelectedItem is TypeItem { Name: var name }))
			{
				return string.Empty;
			}
			return name;
		}
		set
		{
			foreach (TypeItem item in ddlTypes.Items)
			{
				if (string.Compare(item.Name, value, ignoreCase: true) == 0)
				{
					ddlTypes.SelectedItem = item;
					break;
				}
			}
		}
	}

	internal string SelectMethod
	{
		get
		{
			string result = string.Empty;
			if (MethodInfo != null)
			{
				result = MethodInfo.Name;
			}
			return result;
		}
		set
		{
			foreach (MethodItem item in ddlMethods.Items)
			{
				if (string.Compare(item.MethodInfo.Name, value, ignoreCase: true) == 0)
				{
					ddlMethods.SelectedItem = item;
					break;
				}
			}
		}
	}

	internal MethodInfo MethodInfo
	{
		get
		{
			if (!(ddlMethods.SelectedItem is MethodItem { MethodInfo: var methodInfo }))
			{
				return null;
			}
			return methodInfo;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.lInstructions = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.ddlTypes = new System.Windows.Forms.ComboBox();
		this.ddlMethods = new System.Windows.Forms.ComboBox();
		this.bOK = new System.Windows.Forms.Button();
		this.bCancel = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.lInstructions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lInstructions.Location = new System.Drawing.Point(12, 9);
		this.lInstructions.Name = "lInstructions";
		this.lInstructions.Size = new System.Drawing.Size(505, 18);
		this.lInstructions.TabIndex = 0;
		this.lInstructions.Text = "Choose a type:";
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label1.Location = new System.Drawing.Point(12, 79);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(505, 18);
		this.label1.TabIndex = 1;
		this.label1.Text = "Choose a method:";
		this.ddlTypes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ddlTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ddlTypes.FormattingEnabled = true;
		this.ddlTypes.Location = new System.Drawing.Point(12, 30);
		this.ddlTypes.Name = "ddlTypes";
		this.ddlTypes.Size = new System.Drawing.Size(502, 21);
		this.ddlTypes.TabIndex = 2;
		this.ddlTypes.SelectedIndexChanged += new System.EventHandler(ddlTypes_SelectedIndexChanged);
		this.ddlMethods.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ddlMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ddlMethods.Enabled = false;
		this.ddlMethods.FormattingEnabled = true;
		this.ddlMethods.Location = new System.Drawing.Point(12, 100);
		this.ddlMethods.Name = "ddlMethods";
		this.ddlMethods.Size = new System.Drawing.Size(502, 21);
		this.ddlMethods.TabIndex = 3;
		this.ddlMethods.SelectedIndexChanged += new System.EventHandler(ddlMethods_SelectedIndexChanged);
		this.bOK.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.bOK.Enabled = false;
		this.bOK.Location = new System.Drawing.Point(182, 143);
		this.bOK.Name = "bOK";
		this.bOK.Size = new System.Drawing.Size(75, 23);
		this.bOK.TabIndex = 4;
		this.bOK.Text = "Accept";
		this.bOK.UseVisualStyleBackColor = true;
		this.bOK.Click += new System.EventHandler(bOK_Click);
		this.bCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.bCancel.Location = new System.Drawing.Point(272, 143);
		this.bCancel.Name = "bCancel";
		this.bCancel.Size = new System.Drawing.Size(75, 23);
		this.bCancel.TabIndex = 5;
		this.bCancel.Text = "Cancel";
		this.bCancel.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(529, 182);
		base.ControlBox = false;
		base.Controls.Add(this.bCancel);
		base.Controls.Add(this.bOK);
		base.Controls.Add(this.ddlMethods);
		base.Controls.Add(this.ddlTypes);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.lInstructions);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ConfigureDataSource";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Configure Data Source";
		base.ResumeLayout(false);
	}

	public ConfigureDataSource()
	{
		InitializeComponent();
	}

	public ConfigureDataSource(IServiceProvider provider, CustomDataSourceDesigner designer)
	{
		InitializeComponent();
		_serviceProvider = provider;
		_component = designer.Component as CustomDataSource;
		_designer = designer;
		DiscoverTypes();
		TypeName = _component.TypeName;
		SelectMethod = _component.SelectMethod;
	}

	private void DiscoverTypes()
	{
		ITypeDiscoveryService typeDiscoveryService = null;
		if (_component.Site != null)
		{
			typeDiscoveryService = (ITypeDiscoveryService)_component.Site.GetService(typeof(ITypeDiscoveryService));
		}
		if (typeDiscoveryService == null)
		{
			return;
		}
		Cursor current = Cursor.Current;
		Cursor.Current = Cursors.WaitCursor;
		try
		{
			ICollection types = typeDiscoveryService.GetTypes(typeof(object), excludeGlobalTypes: true);
			ddlTypes.BeginUpdate();
			ddlTypes.Items.Clear();
			foreach (Type item2 in types)
			{
				TypeItem item = new TypeItem(item2);
				ddlTypes.Items.Add(item);
			}
		}
		finally
		{
			Cursor.Current = current;
			ddlTypes.EndUpdate();
		}
	}

	private void ddlTypes_SelectedIndexChanged(object sender, EventArgs e)
	{
		ddlMethods.Enabled = true;
		FillMethods();
	}

	private void ddlMethods_SelectedIndexChanged(object sender, EventArgs e)
	{
		bOK.Enabled = true;
	}

	private void FillMethods()
	{
		Cursor current = Cursor.Current;
		Cursor.Current = Cursors.WaitCursor;
		try
		{
			MethodInfo[] methods = CustomDataSourceDesigner.GetType(_component.Site, TypeName).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			ddlMethods.BeginUpdate();
			ddlMethods.Items.Clear();
			MethodInfo[] array = methods;
			foreach (MethodInfo methodInfo in array)
			{
				MethodItem item = new MethodItem(methodInfo);
				ddlMethods.Items.Add(item);
			}
		}
		finally
		{
			Cursor.Current = current;
			ddlMethods.EndUpdate();
		}
	}

	private void bOK_Click(object sender, EventArgs e)
	{
		if (string.Compare(TypeName, _component.TypeName, ignoreCase: false) != 0)
		{
			TypeDescriptor.GetProperties(_component)["TypeName"].SetValue(_component, TypeName);
		}
		if (string.Compare(SelectMethod, _component.SelectMethod, ignoreCase: false) != 0)
		{
			TypeDescriptor.GetProperties(_component)["SelectMethod"].SetValue(_component, SelectMethod);
		}
		if (MethodInfo != null)
		{
			_designer.RefreshSchemaInternal(MethodInfo.ReflectedType, MethodInfo.Name, MethodInfo.ReturnType, preferSilent: true);
		}
	}
}
