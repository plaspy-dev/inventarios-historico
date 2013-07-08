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

            /*TreeNode parent = Add("Hoja de ingreso");

            Add("Según", 1, parent);
            Add("Antecedentes", 2, parent);
            Add("Revisión sistemas", 3, parent);
            Add("Examen físico", 4, parent);
            Add("Varios", 5, parent);
            Add("Tipo diagnóstico", 20, parent);
            // Add("Varios Inferior (Hoja de ingreso)", 6, parent); // eliminado.

            parent = Add("Indice de Barthel");
            Add("Grupos", 7, parent);
            // Add("Indice de Barthel", 8, parent);

            parent = Add("Hoja de gastos");
            Add("Suministros", 9, parent);

            parent = Add("Hoja de enfermería");
            Add("Dieta", 10, parent);
            Add("Control de líquidos", 11, parent);
            Add("Vía", 12, parent);

            parent = Add("Pacientes");
            Add("Categoría de afiliación", 13, parent);
            // Add("Copagos", 14, parent);

            parent = Add("Encuesta de satisfacción al usuario");
            Add("Grupo preguntas", 15, parent);
            // Add("Preguntas", 16, parent);

            parent = Add("Evento adverso o incidente en la IPS");
            Add("Tipo evento", 17, parent);
            Add("Consecuencia del evento", 18, parent);

            parent = Add("Formulación");
            Add("Tipo formulación", 19, parent);

            // 20
            parent = Add("Autorizaciones");
            Add("Horas", 21, parent);

            parent = Add("Formulación");
            Add("Área", 22, parent);

            parent = Add("Valoración apoyo terapeutico");
            Add("Sistema de bajo flujo", 23, parent);
            Add("Sistema de alto flujo", 24, parent);
            Add("Fuerza muscular", 25, parent);*/
            
            // disponible 26


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

            Almacen.RemoveEstado(HId.Value);
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
