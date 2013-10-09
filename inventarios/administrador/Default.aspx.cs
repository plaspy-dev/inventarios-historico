using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using SDG.Componentes;

public partial class administrador_Default : System.Web.UI.Page
{
    protected TreeNode Add(string titulo, int tipo, TreeNode parent)
    {
        TreeNode hijo = new TreeNode(titulo);
        hijo.NavigateUrl = Request.Url.AbsolutePath + "?tipo=" + tipo.ToString();
        hijo.Selected = tipo.ToString() == Request.QueryString["tipo"];
        parent.ChildNodes.Add(hijo);
        if (hijo.Selected)
        {
            HTipo.Value = tipo.ToString();
            LTipo.Text = titulo;
            parent.Expand();
        }
        return hijo;
    }

    protected TreeNode Add(string titulo)
    {
        TreeNode parent= new TreeNode(titulo);
        parent.Collapse();
        parent.SelectAction = TreeNodeSelectAction.Expand;
        TTipo.Nodes.Add(parent);
        return parent;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        TNombre.Focus();
        if (!IsPostBack)
        {
            if (Request.QueryString["tipo"] == null)
            {
                Response.Redirect(Request.Url.AbsolutePath + "?tipo=1");
                return;
            }

            CUsuario usuario = (CUsuario) Usuario.GetUsuario();

            BAceptar.Enabled= usuario.CanInsert();
            GResultado.Columns[1].Visible = usuario.CanDelete();
            GResultado.Columns[0].Visible = usuario.CanUpdate();

            TreeNode parent = Add("Productos");

            Add("Grupos", 1, parent);
            // Add("Estados", 2, parent);
            Add("Línea", 3, parent);


            // pulpafruit
            parent = Add("Orden de producción");
            Add("Marmitas", 1001, parent);
            Add("Estado", 1002, parent);
            Add("Análisis físico químico", 1003, parent);
            Add("Microbiológicas", 1004, parent);
            Add("Análisis sensorial", 1005, parent);
            Add("Prueba de esterilidad", 1006, parent);

            if (HTipo.Value.Length == 0)
                Response.Redirect(Request.Url.AbsolutePath + "?tipo=1");
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("varios");
        using (CConector conector = new CConector())
        {
            string szQuery;
            string mensaje;

            query["nombre"] = TNombre.Text;
            query["descripcion"] = TDescripcion.Text;

            if (HId.Value.Length == 0)
            {
                query["tipo"] = HTipo.Value;
                szQuery = query.GetInsert();
                mensaje = TNombre.Text + " ingresado";
            }
            else
            {
                szQuery = query.GetUpdate("id='" + HId.Value + "'");
                mensaje = TNombre.Text + " editado";
            }
            int res = conector.QueryMod(szQuery, true);
            if (res == -1)
                return;
            if (HId.Value.Length == 0)
                HId.Value = res.ToString();
            if (conector.InsertModificacion(HId.Value) == -1)
                return;

            Almacen.RemoveVarios(HId.Value);
            Session["szMensaje"] = mensaje;
            Response.Redirect(Request.Url.AbsolutePath + "?tipo=" + HTipo.Value);
        }
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath + "?tipo=" + HTipo.Value);
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        HId.Value= GResultado.SelectedDataKey.Value.ToString();
        TNombre.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[2].Text).Trim();
        TDescripcion.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[3].Text).Trim();

        ((ing)Master).SetModificacion(HId.Value);
    }
}
