using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using MySql.Data.MySqlClient;
using SDG.Componentes;
using System.Configuration;

public partial class administrador_grupos : System.Web.UI.Page
{
    class Parents {
        public string parent {set; get; }
        public ArrayList permisos {set; get;}
        public Parents(string p) {
            parent = p;
            permisos = new ArrayList();
        }
        public Permisos FindURL(string url) {

            for (int i = 0; i < permisos.Count; i++)
            {
                if (((Permisos)permisos[i]).pagina == url)
                    return (Permisos)permisos[i];
            }
            return null;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        BEliminar.Visible = false;
        if(!IsPostBack) {

            CUsuario usuario = (CUsuario) Usuario.GetUsuario();
            BAceptar.Enabled = usuario.CanInsert();

            TNombre.Focus();
            ArrayList parents= new ArrayList();
            ArrayList menu = Usuario.GetUsuario().GetFullMenu();
            for(int i=0; i<menu.Count; i++) {

                CURLs url = (CURLs)menu[i];
                /*if (url.m_bNativo)
                {*/
                if (url.Parent.Length==0) {
                    continue;
                }
                Parents padre=null;
                for (int j = 0; j < parents.Count; j++ ) {
                    if(((Parents) parents[j]).parent==url.Parent) {
                        padre = (Parents)parents[j];
                        break;
                    }
                }
                if(padre==null) {
                    padre = new Parents(url.Parent);
                    parents.Add(padre);
                }
                if (url.URL!="~")
                    padre.permisos.Add(new Permisos(url.Titulo, url.URL));
                //}
            }
            R.DataSource = parents;
            R.DataBind();
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query= new Query("grupo");
        using (CConector conector = new CConector())
        {
            string szQuery;
            string mensaje;
            string grupo = "";

            conector.StartTransaction();

            query["nombre"] = TNombre.Text;
            query["descripcion"] = TDescripcion.Text;

            if (LGrupos.SelectedIndex == -1)
            {

                szQuery = query.GetInsert();
                mensaje = TNombre.Text + " ingresado";
                int resultado = conector.QueryMod(szQuery, true);
                if (resultado == -1)
                    return;
                grupo = resultado.ToString();
            }
            else
            {
                szQuery = query.GetUpdate("id='" + LGrupos.SelectedValue + "'");
                mensaje = TNombre.Text + " editado";
                if (conector.QueryMod(szQuery) == -1)
                    return;
                grupo = LGrupos.SelectedValue;
            }
            if (conector.InsertModificacion(grupo) == -1)
                return;
            if (conector.QueryMod("DELETE FROM permisos WHERE grupo='" + grupo + "'") == -1)
                return;

            for (int j = 0; j < R.Items.Count; j++)
            {
                GridView grid = (GridView)R.Items[j].FindControl("P");
                Query permisos = new Query("permisos");

                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    permisos["grupo"] = grupo;
                    permisos["url"] = grid.DataKeys[i].Value.ToString();
                    int per = 0;

                    CheckBox check;
                    check = (CheckBox)grid.Rows[i].FindControl("a");
                    if (check.Checked)
                        per += 32;
                    check = (CheckBox)grid.Rows[i].FindControl("e");
                    if (check.Checked)
                        per += 16;
                    check = (CheckBox)grid.Rows[i].FindControl("d");
                    if (check.Checked)
                        per += 8;
                    check = (CheckBox)grid.Rows[i].FindControl("i");
                    if (check.Checked)
                        per += 4;
                    check = (CheckBox)grid.Rows[i].FindControl("m");
                    if (check.Checked)
                        per += 2;
                    check = (CheckBox)grid.Rows[i].FindControl("l");
                    if (check.Checked)
                        per += 1;
                    if (per == 0) // lo omitimos
                        continue;
                    if (per > 0 && (per & 1) != 1)
                        per += 1;
                    permisos["permiso"] = per.ToString();

                    if (conector.QueryMod(permisos.GetInsert()) == -1)
                        return;
                }
            }

            if (conector.QueryMod("DELETE FROM alertas_grupo WHERE grupo=" + grupo) == -1)
                return;

            query = new Query("alertas_grupo");
            query["grupo"] = grupo;

            for (int i = 0; i < GAlertas.Rows.Count; i++)
            {
                CheckBox check = (CheckBox)GAlertas.Rows[i].FindControl("CAlerta");
                if (check.Checked)
                {
                    query["alerta"] = GAlertas.DataKeys[i].Value.ToString();
                    if (conector.QueryMod(query.GetInsert()) == -1)
                        return;
                }
            }

            conector.Commit();
            Session["alertas"] = null;

            ((ing)Master).SetResultado(mensaje);
        }
    }
    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
    protected void BEliminar_Click(object sender, EventArgs e)
    {
        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            if (conector.QueryMod("DELETE FROM alertas_grupo WHERE grupo=" + LGrupos.SelectedValue) == -1)
                return;
            if (conector.QueryMod("DELETE FROM permisos WHERE grupo='" + LGrupos.SelectedValue + "'") == -1)
                return;
            if (conector.QueryMod("DELETE FROM grupo WHERE id='" + LGrupos.SelectedValue + "'") == -1)
            {
                ((ing)Master).SetAlerta("Todavia existen usuarios en este grupo.");
                return;
            }
            conector.InsertModificacion(LGrupos.SelectedValue);
            conector.Commit();
            ((ing)Master).SetResultado("Grupo " + TNombre.Text + " Eliminado.");
        }
    }

    protected void LGrupos_SelectedIndexChanged(object sender, EventArgs e)
    {
        ArrayList parents = new ArrayList();
        ArrayList menu = Usuario.GetUsuario().GetFullMenu();

        for (int i = 0; i < menu.Count; i++)
        {
            CURLs url = (CURLs)menu[i];
            /*if (url.m_bNativo)
            {*/
            if (url.Parent.Length == 0)
            {
                continue;
            }
                Parents padre = null;
                for (int j = 0; j < parents.Count; j++)
                {
                    if (((Parents)parents[j]).parent == url.Parent)
                    {
                        padre = (Parents)parents[j];
                        break;
                    }
                }
                if (padre == null)
                {
                    padre = new Parents(url.Parent);
                    parents.Add(padre);
                }
                if (url.URL != "~")
                    padre.permisos.Add(new Permisos(url.Titulo, url.URL, LGrupos.SelectedValue == "0" ? 63 : 0));
            // }
        }

        if (LGrupos.SelectedValue == "0")
        {
            BAceptar.Enabled = false;
            BEliminar.Enabled = false;

            TNombre.Text = LGrupos.SelectedItem.Text;
            TDescripcion.Text = "Grupo de administración, con todos los accesos";
        } else {
            CUsuario usuario = (CUsuario) Usuario.GetUsuario();

            BAceptar.Enabled = usuario.CanUpdate();
            BEliminar.Enabled = usuario.CanDelete();
            BEliminar.Visible = true;

            using (CConector conector = new CConector())
            {

                TNombre.Text = LGrupos.SelectedItem.Text;
                TDescripcion.Text = conector.QueryString("SELECT descripcion FROM grupo WHERE id='" + LGrupos.SelectedValue + "'");

                Query query = new Query("permisos");
                query += "url";
                query += "permiso";

                MySqlDataReader reader = conector.Query(query.GetSelect("grupo='" + LGrupos.SelectedValue + "'"));
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        string url = reader["url"].ToString();
                        int nPermisos = Convert.ToInt32(reader["permiso"]);
                        for (int i = 0; i < parents.Count; i++)
                        {
                            Parents pagina = (Parents)parents[i];
                            Permisos info = pagina.FindURL(url);
                            if (info != null)
                            {
                                info.SetPermisos(nPermisos);
                            }
                        }
                    }
                }
            }
        }
        R.DataSource = parents;
        R.DataBind();

        ((ing)Master).SetModificacion(LGrupos.SelectedValue);
    }

    protected void R_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if(e.Item.DataItem!=null) {

            GridView grid = (GridView)e.Item.FindControl("P");
            grid.DataSource= ((Parents)e.Item.DataItem).permisos;
        }
    }
}
