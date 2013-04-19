using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using SDG.Componentes;

public partial class ing : System.Web.UI.MasterPage
{
    public event EventHandler _ImprimirClicked = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        // CUsuario.CheckVariables();
        CUsuario usuario = (CUsuario)Usuario.GetUsuario();

        if (Session["szMensaje"] != null)
        {
            LPagina.Text += " " + Session["szMensaje"].ToString();
            Session["szMensaje"] = null;
        }
        else
        {
            LPagina.Text = Page.Title;
        }
        if (Session["szAlerta"] != null)
        {
            LAlerta.Text = Session["szAlerta"].ToString();
            Session["szAlerta"] = null;
        }
        else
        {
            LAlerta.Text = "";
        }

        if (!IsPostBack)
        {
            InitAlmacen();
            CheckAlmacen();
            LUsuario.Text = usuario.GetNombre();

            RAlertas.DataSource = Alertas.GetAlertas(GetAlmacen());
            RAlertas.DataBind();

            if (!string.IsNullOrEmpty((string)ViewState["printfile"]) && usuario.CanPrint()) {

                if (Request.QueryString["p"] == "1")
                {
                    // llamamos a la pagina cliente
                    if (_ImprimirClicked != null)
                    {
                        _ImprimirClicked(this, null);
                    }

                    StreamReader reader = new StreamReader(Formato.GetRutaFormatos(ViewState["printfile"].ToString()));
                    string Contenido = reader.ReadToEnd();
                    reader.Close();

                    Contenido = Utilidades.ReplaceContenido(Page.Controls, Contenido);
                    // agregamos el script para imprimir
#if DEBUG
                    Contenido= Contenido.Replace("</body>", "<script type='text/javascript'>window.onload = function () {}</script></body>");
                    // contenido= contenido.Replace("</body>", "<script type='text/javascript'>window.onload = function () {if(window.name=='popup')return;var venImp = window.open('../imp.htm', 'popup');venImp.document.write(document.documentElement.innerHTML);venImp.document.close();if(venImp.print) {venImp.print();}window.history.back();}</script></body>");
#else
                    Contenido = Contenido.Replace("</body>", "<script type='text/javascript'>window.onload = function () {if(window.print) {window.print();}window.close();}</script></body>");
                    // contenido = contenido.Replace("</body>", "<script type='text/javascript'>window.onload = function () {if(window.name=='popup')return;var venImp = window.open('../imp.htm', 'popup');venImp.document.write(document.documentElement.innerHTML);venImp.document.close();if(venImp.print) {venImp.print();venImp.close();}window.history.back();}</script></body>");
#endif
                    Response.Clear();
                    Response.Write(Contenido);
                    Response.End();
                }
                else if (Request.QueryString["e"] == "1")
                {
                    if (_ImprimirClicked != null)
                    {
                        _ImprimirClicked(this, null);
                    }
                    StreamReader reader = new StreamReader(Formato.GetRutaFormatos(ViewState["printfile"].ToString()));
                    string Contenido = reader.ReadToEnd();
                    reader.Close();

                    Contenido = Utilidades.ReplaceContenido(Page.Controls, Contenido);

                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(Page.Title) + ".xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = Encoding.GetEncoding("utf-8");
                    Response.Write(Contenido);
                    Response.End();
                }
            }
        }
        usuario.CreateMenu(TMenu, Session["parametros"] == null ? "" : Session["parametros"].ToString());
    }
    public void SetPrintFile(string archivo)
    {
        SetPrintFile(archivo, false);
    }

    public void RestoreState() {
        try
        {
            if (!string.IsNullOrEmpty(Request.QueryString["ss"]))
                Utilidades.RestoreState(ContentPlaceHolder1.Controls, Crypt.DesencriptarDES(Request.QueryString["ss"], System.Environment.GetEnvironmentVariable("INVENTORY_STATE_KEY")));
        } catch {}
    }

    public void SetPrintFile(string archivo, bool savestate) {

        HGuardar.NavigateUrl = HImprimir.NavigateUrl = Request.Url.AbsolutePath + Request.Url.Query;
        HImprimir.NavigateUrl += Request.Url.Query.Length == 0 ? "?p=1" : "&p=1";
        HGuardar.NavigateUrl += Request.Url.Query.Length == 0 ? "?e=1" : "&e=1";

        if(savestate) {
            HImprimir.NavigateUrl += "&ss=" + HttpUtility.UrlEncode(Crypt.EncriptarDES(Utilidades.SaveState(ContentPlaceHolder1.Controls), System.Environment.GetEnvironmentVariable("INVENTORY_STATE_KEY")));
            HGuardar.NavigateUrl += "&ss=" + HttpUtility.UrlEncode(Crypt.EncriptarDES(Utilidades.SaveState(ContentPlaceHolder1.Controls), System.Environment.GetEnvironmentVariable("INVENTORY_STATE_KEY")));
        }

        ViewState["printfile"] = archivo;
        HGuardar.Visible = HImprimir.Visible = !string.IsNullOrEmpty((string)ViewState["printfile"]) && CUsuario.GetUsuario().CanPrint();
    }
    public void SetModificacion(string idModificaciones, string id2) {
        
        MModificaciones.SetModificacion(idModificaciones, id2);
    }
    public void SetModificacion(string idModificaciones)
    {
        MModificaciones.IdModificacion = idModificaciones;
    }
    public void SetAlerta(string mensaje) {
        if (!ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            LAlerta.Text = mensaje;
        else Session["szAlerta"] = mensaje;
    }
    public void SetResultado(String titulo)
    {
        Session["szMensaje"] = titulo;
        Response.Redirect(Request.Url.AbsolutePath);
    }
    public void CheckAlmacen() {

        if (!string.IsNullOrEmpty(Request.QueryString["alm"]))
        {
            Session["almacen"] = Request.QueryString["alm"];
            string query = HttpContext.Current.Request.Url.Query
                            .Replace("&alm=" + Request.QueryString["alm"], "")
                            .Replace("?alm=" + Request.QueryString["alm"] + "&", "?")
                            .Replace("?alm=" + Request.QueryString["alm"], "");
            Response.Redirect(HttpContext.Current.Request.Url.AbsolutePath + query);
            Response.End();
        }
    }
    public DropDownList GetDropDownAlmacen() {
        return DAlmacen;
    }

    public string GetAlmacen() {

        InitAlmacen();
        if(DAlmacen.Items.Count==0) {
            if (Request.Url.AbsolutePath != Request.ApplicationPath + "/inicio.aspx")
            {
                Response.Redirect("~/inicio.aspx");
                Response.End();
                return "";
            }
            DAlmacen.Visible = false;
            LAlmacen.Visible = false;
            LPagina.Text = Page.Title = "No se ha definido un almacen consulte a su administrador";
        } else if(DAlmacen.Items.Count==1) {
            DAlmacen.Visible = false;
            LAlmacen.Visible = true;
            LAlmacen.Text= DAlmacen.Items[0].Text;
        } else {
            DAlmacen.Visible = true;
            LAlmacen.Visible = false;
        }
        return DAlmacen.SelectedValue;
    }

    public void InitAlmacen() {

        if (ViewState["almacen"] != null) // ya se inicio el almacen
            return;

        CUsuario usuario = (CUsuario)Usuario.GetUsuario();
        if (usuario["almacenes"] == "")
        {
            SAlmacen.SelectCommand = string.Format(SAlmacen.SelectCommand, "AND id IN (-1)");
            if (DAlmacen.Items.Count>0 && DAlmacen.Items[0].Value == "0")
                DAlmacen.Items.RemoveAt(0);
        }
        else if (usuario["almacenes"] == "-2")
        { // todos
            SAlmacen.SelectCommand = string.Format(SAlmacen.SelectCommand, "");
        }
        else
        {
            SAlmacen.SelectCommand = string.Format(SAlmacen.SelectCommand, "AND id IN (" + usuario["almacenes"] + ")");
            if (usuario["almacenes"].IndexOf("-1") == -1)
            {
                if (DAlmacen.Items.Count > 0 && DAlmacen.Items[0].Value == "0")
                    DAlmacen.Items.RemoveAt(0);
            }
        }

        if (Session["almacen"] != null)
        {
            Conector.SetCookie("almacen", Session["almacen"].ToString());
            if (DAlmacen.Items.Count == 0 || (DAlmacen.Items.Count == 1 && DAlmacen.Items[0].Value == "0"))
            {
                try
                {
                    DAlmacen.DataBind();
                    DAlmacen.SelectedValue = Session["almacen"].ToString();
                }
                catch { if(DAlmacen.Items.Count>0) DAlmacen.SelectedIndex = 0; }
            }
            Session["almacen"] = null;
            ViewState["almacen"] = DAlmacen.SelectedValue;
        }
        else
        {
            if (DAlmacen.Items.Count == 0 || (DAlmacen.Items.Count == 1 && DAlmacen.Items[0].Value == "0"))
            {
                try
                {
                    DAlmacen.DataBind();
                    DAlmacen.SelectedValue = CConector.GetCookie("almacen");
                }
                catch { if (DAlmacen.Items.Count > 0) DAlmacen.SelectedIndex = 0; }
            }
            ViewState["almacen"] = DAlmacen.SelectedValue;
        }
    }
    protected void OnAlerta_Click(object sender, EventArgs e) {
        
        LinkButton link = (LinkButton)sender;

        SAlertas.SelectCommand = string.Format(link.CommandName, GetAlmacen());
        ((HyperLinkField)GAlertas.Columns[0]).DataNavigateUrlFormatString = link.CommandArgument;
        GAlertas.DataBind();
        /*if (GAlertas.Rows.Count==1) {
            Response.Redirect(string.Format(link.CommandArgument, GAlertas.DataKeys[0].Value.ToString()));
        } else {
            PAlertas.Visible = true;
        }*/
        PAlertas.Visible = true;
    }
}
