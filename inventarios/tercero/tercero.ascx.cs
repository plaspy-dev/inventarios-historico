using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[ValidationPropertyAttribute("IdTercero")]
public partial class tercero_tercero : System.Web.UI.UserControl
{
    public string IdTercero
    {
        set
        {
            TTercero.Text = "";
            TTerceroID.Text = "";
            if (!string.IsNullOrEmpty(value))
            {
                Tercero tercero= new Tercero(value);
                TTerceroID.Text = tercero.Id;
                TTercero.Text = tercero.NombreCompleto;
            }
        }
        get { return TTerceroID.Text; }
    }

    public string Tipo {

        set {
            ViewState["tipoTercero"] = value;
        }
        get { return (string)ViewState["tipoTercero"]; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
            if (string.IsNullOrEmpty((string)ViewState["tipoTercero"]))
                ViewState["tipoTercero"] = "2";
        }
        if (!Page.ClientScript.IsClientScriptIncludeRegistered("tercero"))
            Page.ClientScript.RegisterClientScriptInclude("tercero", ResolveUrl("~/tercero/tercero.ascx.js"));
        Page.ClientScript.RegisterClientScriptBlock(typeof(string), "terceroI", "initTercero('" + TTercero.ClientID + "', '" + TTerceroID.ClientID + "', " + ViewState["tipoTercero"].ToString() + ");", true);
    }
}
