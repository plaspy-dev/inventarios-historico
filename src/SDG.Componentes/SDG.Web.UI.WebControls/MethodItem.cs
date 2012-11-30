using System.Reflection;
using System.Text;

namespace SDG.Web.UI.WebControls;

internal class MethodItem
{
	private MethodInfo _methodInfo;

	public MethodInfo MethodInfo => _methodInfo;

	public MethodItem(MethodInfo methodInfo)
	{
		_methodInfo = methodInfo;
	}

	public override string ToString()
	{
		if (_methodInfo == null)
		{
			return "(none)";
		}
		return GetMethodSignature(_methodInfo);
	}

	private string GetMethodSignature(MethodInfo method)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(method.Name);
		stringBuilder.Append("(");
		ParameterInfo[] parameters = method.GetParameters();
		ParameterInfo[] array = parameters;
		foreach (ParameterInfo parameterInfo in array)
		{
			stringBuilder.Append(parameterInfo.ParameterType.Name);
			stringBuilder.Append(" ");
			stringBuilder.Append(parameterInfo.Name);
			stringBuilder.Append(", ");
		}
		if (parameters.Length > 0)
		{
			stringBuilder.Length -= 2;
		}
		stringBuilder.Append(")");
		if (method.ReturnType != typeof(void))
		{
			stringBuilder.Append(", returns ");
			stringBuilder.Append(method.ReturnType.Name);
		}
		return stringBuilder.ToString();
	}
}
