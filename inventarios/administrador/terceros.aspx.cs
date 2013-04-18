using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class administrador_terceros : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TIdentificacion.Focus();
            CUsuario usuario = (CUsuario)Usuario.GetUsuario();

            BAceptar.Enabled = usuario.CanInsert();
            GResultado.Columns[1].Visible = usuario.CanDelete();
            GResultado.Columns[0].Visible = usuario.CanUpdate();
            CConfiguracion configuracion= CConfiguracion.GetConfiguracion();

            RDireccion.Visible = RTelefono.Visible = configuracion.bDatosComplementariosTerceroSalida || configuracion.bDatosComplementariosTerceroEntrada;

            if(!string.IsNullOrEmpty(Request.QueryString["ti"])) {

                CConector conector = new CConector();
                Query query = new Query("tercero");

                query += "id";
                query+="nombre";
                query+="descripcion";
                query+="tipo";
                query+="identificacion";
                query+="telefono";
                query+="direccion";
                query+="email";

                MySqlDataReader reader= conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["ti"])));
                if(reader!=null && reader.Read()) {

                    HId.Value = reader["id"].ToString();

                    TNombre.Text= reader["nombre"].ToString();
                    TDescripcion.Text = reader["descripcion"].ToString();
                    DTipo.SelectedValue = reader["tipo"].ToString();
                    TIdentificacion.Text = reader["identificacion"].ToString();
                    TTelefono.Text = reader["telefono"].ToString();
                    TDireccion.Text = reader["direccion"].ToString();
                    TEMail.Text = reader["email"].ToString();

                    ((ing)Master).SetModificacion(HId.Value);
                    BAceptar.Enabled = usuario.CanUpdate();
                }
            }
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("tercero");
        CConector conector = CConector.GetConector();
        string szQuery;
        string mensaje;

        conector.StartTransaction();

        query["nombre"] = TNombre.Text;
        query["descripcion"] = TDescripcion.Text;
        query["tipo"] = DTipo.SelectedValue;
        query["identificacion"] = TIdentificacion.Text;
        query["telefono"] = TTelefono.Text;
        query["direccion"] = TDireccion.Text;
        query["email"] = TEMail.Text;

        if (HId.Value.Length == 0)
        {
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

        conector.Commit();

        Tercero.RemoveTercero(HId.Value);

        ((ing)Master).SetResultado(mensaje);
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath + "?ti=" + GResultado.SelectedDataKey.Value.ToString());
    }
}
