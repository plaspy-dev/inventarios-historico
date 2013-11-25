using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;

namespace SDG.Componentes;

public class Logs
{
	public static string DumpPage(HttpServerUtility Server, HttpRequest Request)
	{
		string text = "";
		Exception ex = null;
		try
		{
			ex = Server.GetLastError().GetBaseException();
		}
		catch
		{
		}
		try
		{
			text = DateTime.Now.ToString() + " Error[app] : " + Request.Url.ToString() + " - '" + ex.Message.ToString() + "'";
			text += "\r\n";
		}
		catch
		{
		}
		try
		{
			text += "Headers";
			text += "\r\n";
			foreach (string header in Request.Headers)
			{
				text = text + header + "=" + Request.Headers[header];
				text += "\r\n";
			}
		}
		catch
		{
		}
		try
		{
			text += "Contenido";
			text += "\r\n";
			Request.InputStream.Seek(0L, SeekOrigin.Begin);
			StreamReader streamReader = new StreamReader(Request.InputStream);
			string text3 = streamReader.ReadToEnd();
			if (!string.IsNullOrEmpty(text3))
			{
				text += text3;
				text += "\r\n";
				text += HttpUtility.UrlDecode(text3);
				text += "\r\n";
			}
		}
		catch
		{
		}
		try
		{
			text += "Form";
			text += "\r\n";
			foreach (string item in Request.Form)
			{
				switch (item)
				{
				case "__EVENTTARGET":
				case "__EVENTARGUMENT":
				case "__VIEWSTATE":
				case "__VIEWSTATEENCRYPTED":
				case "__EVENTVALIDATION":
					continue;
				}
				text = text + item + "=" + Request.Form[item];
				text += "\r\n";
			}
		}
		catch
		{
		}
		try
		{
			text += ex.StackTrace.ToString();
			text += "\r\n";
		}
		catch
		{
		}
		return text + "\r\n";
	}

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
			string text = ((HttpContext.Current != null) ? HttpContext.Current.Request.Url.Segments[1].Replace("/", "") : Path.GetFileName(Assembly.GetEntryAssembly().Location));
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
					using StreamWriter streamWriter2 = File.AppendText(archivo);
					streamWriter2.WriteLine(logs);
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
