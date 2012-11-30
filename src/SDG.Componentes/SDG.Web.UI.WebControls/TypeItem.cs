using System;

namespace SDG.Web.UI.WebControls;

internal class TypeItem
{
	private Type _type;

	public Type Type => _type;

	public string Name => _type.FullName;

	public TypeItem(Type type)
	{
		_type = type;
	}

	public override string ToString()
	{
		return Name;
	}
}
