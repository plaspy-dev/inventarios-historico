using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace SDG.Componentes;

public class Logs
{
	public static bool WriteLog(string logs)
	{
		string archivo = "";
		if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["filelogs"]))
		{
			archivo = ConfigurationSettings.AppSettings["filelogs"].ToString();
		}
		return WriteLog(logs, archivo);
	}

	public static bool WriteLog(string logs, string archivo)
	{
		try
		{
			string text = HttpContext.Current.Request.Url.Segments[1].Replace("/", "");
			EventLog.WriteEntry("Application", text + ": " + logs);
		}
		catch
		{
		}
		try
		{
			if (!string.IsNullOrEmpty(archivo))
			{
				if (!File.Exists(archivo))
				{
					using StreamWriter streamWriter = File.CreateText(archivo);
					streamWriter.WriteLine(logs);
				}
				else
				{
					using StreamWriter streamWriter = File.AppendText(archivo);
					streamWriter.WriteLine(logs);
				}
				return true;
			}
		}
		catch
		{
		}
		return false;
	}
}
