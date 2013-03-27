using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using SDG.Componentes;

public partial class ing : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // CUsuario.CheckVariables();
        CUsuario usuario = (CUsuario)Usuario.GetUsuario();

        if (!IsPostBack)
        {
            InitAlmacen();
            CheckAlmacen();
            LUsuario.Text = usuario.GetNombre();

            CConector conector= new CConector();
            string cantidad= conector.QueryString(string.Format("SELECT COUNT(*) FROM entrada{0} WHERE fecha IS NULL;", GetAlmacen()));
            if(cantidad=="0") {
                HAlertas.Visible = false;
            } else {
                HAlertas.Visible = true;
                HAlertas.Text = cantidad;
                if (cantidad == "1")
                {
                    HAlertas.Text += " Traslado pendiente";
                }
                else
                {
                    HAlertas.Text += " Traslados pendientes";
                }
            }
        }
        if (Session["szMensaje"] != null)
        {
            LPagina.Text += " " + Session["szMensaje"].ToString();
            Session["szMensaje"] = null;
        }
        else
        {
            LPagina.Text = Page.Title;
        }
        if (Session["szAlerta"]!=null) {
            LAlerta.Text = Session["szAlerta"].ToString();
            Session["szAlerta"] = null;
        } else {
            LAlerta.Text = "";
        }
        usuario.CreateMenu(TMenu, Session["parametros"] == null ? "" : Session["parametros"].ToString());
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
        /*if (ViewState["almacen"]!=null && ViewState["almacen"].ToString()!=GetAlmacen()) {
            Response.Redirect(HttpContext.Current.Request.Url.AbsolutePath + HttpContext.Current.Request.Url.Query);
            Response.End();
        }*/
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

    protected void InitAlmacen() {

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
    protected void HAlertas_Click(object sender, EventArgs e)
    {
        CConector conector = new CConector();
        string cantidad = conector.QueryString(string.Format("SELECT COUNT(*) FROM entrada{0} WHERE fecha IS NULL;", GetAlmacen()));
        if(cantidad=="1") {
            cantidad= conector.QueryString(string.Format("SELECT id FROM entrada{0} WHERE fecha IS NULL;", GetAlmacen()));
            Response.Redirect("~/almacen/entrada.aspx?ei=" + cantidad);
            Response.End();
        }
        else
        {
            SAlertas.SelectCommand = string.Format(SAlertas.SelectCommand, GetAlmacen());
            PAlertas.Visible = true;
        }
    }
}
