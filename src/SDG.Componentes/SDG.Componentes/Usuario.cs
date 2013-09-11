using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;

namespace SDG.Componentes;

public class Usuario
{
	private static bool bGetKey = false;

	protected string m_szNombre;

	protected int m_nGrupo;

	protected string m_szID;

	protected string m_szLogin;

	protected Grupo m_pGrupo;

	protected string m_szUrl;

	protected ArrayList m_saCampos = new ArrayList();

	protected ArrayList m_saValores = new ArrayList();

	public string this[string campo]
	{
		get
		{
			return m_saValores[GetCampo(campo)].ToString();
		}
		set
		{
			m_saValores[GetCampo((campo.Length == 0) ? value : campo)] = value;
		}
	}

	protected Usuario()
	{
		m_szNombre = "";
		m_nGrupo = 0;
		m_szID = "";
		m_szLogin = "";
		m_pGrupo = new Grupo();
		m_szUrl = "";
	}

	public Usuario(string id, string nombre, int grupo, string login)
	{
		if (GetKey())
		{
			m_szNombre = nombre;
			m_nGrupo = grupo;
			m_szID = id;
			m_szLogin = login;
			m_pGrupo = new Grupo();
			m_szUrl = "";
			HttpContext.Current.Session["Usuario"] = this;
		}
	}

	public string GetLogin()
	{
		return m_szLogin;
	}

	public void CreateMenu(TreeView tree)
	{
		m_pGrupo.CreateMenu(tree, "", this);
	}

	public string GetForceURL()
	{
		return m_szUrl;
	}

	public void SetForceURL(string url, Conector conector)
	{
		m_szUrl = url;
		conector.QueryMod("UPDATE usuario SET forceurl=" + Query.GetValor(url) + " WHERE id=" + GetID());
	}

	public void SetForceURL(string url)
	{
		m_szUrl = url;
	}

	public void CreateMenu(TreeView tree, string parametros)
	{
		m_pGrupo.CreateMenu(tree, parametros, this);
	}

	public void SetPermiso(string url, int permiso)
	{
		m_pGrupo.SetPermiso(url, permiso);
	}

	public void AddPermiso(string padre, string nombre, string url, int permiso)
	{
		m_pGrupo.AddPermisos(padre, nombre, url, permiso);
	}

	public ArrayList GetFullMenu()
	{
		return m_pGrupo.GetFullMenu();
	}

	public string GetNombre()
	{
		return m_szNombre;
	}

	public int GetGrupo()
	{
		return m_nGrupo;
	}

	public string GetID()
	{
		return m_szID;
	}

	public virtual int GetPermisos(string url)
	{
		return m_pGrupo.GetPermisos(url);
	}

	public virtual bool CanRead()
	{
		if (m_nGrupo == 0)
		{
			return true;
		}
		return GetPermisos(HttpContext.Current.Request.Url.AbsolutePath) > 0;
	}

	public virtual bool CanInsert()
	{
		if (m_nGrupo == 0)
		{
			return true;
		}
		return (GetPermisos(HttpContext.Current.Request.Url.AbsolutePath) & 4) == 4;
	}

	public virtual bool CanPrint()
	{
		if (m_nGrupo == 0)
		{
			return true;
		}
		return (GetPermisos(HttpContext.Current.Request.Url.AbsolutePath) & 2) == 2;
	}

	public virtual bool CanUpdate()
	{
		if (m_nGrupo == 0)
		{
			return true;
		}
		return (GetPermisos(HttpContext.Current.Request.Url.AbsolutePath) & 8) == 8;
	}

	public virtual bool CanDelete()
	{
		if (m_nGrupo == 0)
		{
			return true;
		}
		return (GetPermisos(HttpContext.Current.Request.Url.AbsolutePath) & 0x10) == 16;
	}

	public virtual bool CanAdmin()
	{
		if (m_nGrupo == 0)
		{
			return true;
		}
		return (GetPermisos(HttpContext.Current.Request.Url.AbsolutePath) & 0x20) == 32;
	}

	public virtual bool CanAccess()
	{
		if (m_nGrupo == 0)
		{
			return true;
		}
		if (HttpContext.Current.Request.Url.AbsolutePath.IndexOf(".asmx") != -1)
		{
			return true;
		}
		if (HttpContext.Current.Request.Url.AbsolutePath == HttpContext.Current.Request.ApplicationPath + "/inicio.aspx")
		{
			return true;
		}
		int permisos = GetPermisos(HttpContext.Current.Request.Url.AbsolutePath);
		return permisos > 0;
	}

	public void Redirect()
	{
		HttpContext.Current.Response.Redirect("~/inicio.aspx");
		HttpContext.Current.Response.End();
	}

	protected virtual Usuario ValidateUser()
	{
		return null;
	}

	protected virtual Usuario ValidateUser(string usuario, string passwd)
	{
		return null;
	}

	protected virtual Usuario IsValidUsuario()
	{
		return this;
	}

	public static Usuario Validate(string usuario, string password)
	{
		if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["Usuario"]))
		{
			try
			{
				Usuario usuario2 = (Usuario)Activator.CreateInstance(Assembly.Load("App_Code").GetType(ConfigurationSettings.AppSettings["Usuario"]));
				return usuario2.ValidateUser(usuario, password);
			}
			catch
			{
			}
		}
		return null;
	}

	public static Usuario Validate()
	{
		if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["Usuario"]))
		{
			try
			{
				Usuario usuario = (Usuario)Activator.CreateInstance(Assembly.Load("App_Code").GetType(ConfigurationSettings.AppSettings["Usuario"]));
				return usuario.ValidateUser();
			}
			catch
			{
			}
		}
		return null;
	}

	public static Usuario GetInstance(bool redirect)
	{
		HttpContext.Current.Response.AddHeader("pragma", "no-cache");
		HttpContext.Current.Response.CacheControl = "Private";
		HttpContext.Current.Response.Expires = 0;
		Usuario usuario = (Usuario)HttpContext.Current.Session["Usuario"];
		usuario = ((usuario != null) ? usuario.IsValidUsuario() : Validate());
		if (usuario == null)
		{
			if (redirect)
			{
				string text = HttpUtility.UrlEncode(HttpContext.Current.Request.Url.AbsolutePath + HttpContext.Current.Request.Url.Query);
				HttpContext.Current.Response.Redirect("~/Default.aspx?from=" + text);
				HttpContext.Current.Response.End();
			}
		}
		else if (!usuario.CanAccess())
		{
			usuario.Redirect();
		}
		return usuario;
	}

	public static Usuario GetInstance()
	{
		return GetInstance(redirect: true);
	}

	public static Usuario GetUsuario()
	{
		return GetInstance(redirect: true);
	}

	protected int GetCampo(string campo)
	{
		int num = -1;
		for (int i = 0; i < m_saCampos.Count; i++)
		{
			if (m_saCampos[i].ToString() == campo)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			num = m_saCampos.Add(campo);
			m_saValores.Add("");
		}
		return num;
	}

	private static bool GetKey()
	{
#if ENABLE_LICENSE_CHECK
		string licenseSeed = Environment.GetEnvironmentVariable("ARCHIVED_DOMAIN_LICENSE_SEED");
		if (String.IsNullOrEmpty(licenseSeed)) return false;
		if (bGetKey)
		{
			return true;
		}
		try
		{
			string[] array = null;
			string text;
			if (HttpContext.Current == null)
			{
				text = Path.GetFileName(Assembly.GetEntryAssembly().Location);
			}
			else
			{
				if (HttpContext.Current.Request.Url.Host == "localhost")
				{
					return true;
				}
				if (string.IsNullOrEmpty(ConfigurationSettings.AppSettings["DomainKey"]))
				{
					return false;
				}
				text = HttpContext.Current.Request.Url.Host;
			}
			array = ConfigurationSettings.AppSettings["DomainKey"].Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (Crypt.GetMD5(Crypt.Encriptar3DES(text.PadRight(29, 'x').PadRight(37, '?').Replace("x", ";"), licenseSeed.Replace("2", "1").Replace("2", "6"))) == array[i])
				{
					bGetKey = true;
					return true;
				}
			}
			return false;
		}
		catch
		{
		}
		return false;
#else
		return true;
#endif
	}
}
