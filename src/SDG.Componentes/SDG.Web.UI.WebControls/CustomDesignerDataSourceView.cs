using System.Web.UI.Design;

namespace SDG.Web.UI.WebControls;

public class CustomDesignerDataSourceView : DesignerDataSourceView
{
	protected CustomDataSourceDesigner _owner;

	public override IDataSourceViewSchema Schema
	{
		get
		{
			if (!string.IsNullOrEmpty(_owner.TypeName) && !string.IsNullOrEmpty(_owner.SelectMethod))
			{
				return _owner.DataSourceSchema;
			}
			return null;
		}
	}

	public CustomDesignerDataSourceView(CustomDataSourceDesigner owner, string viewName)
		: base(owner, viewName)
	{
		_owner = owner;
	}
}
