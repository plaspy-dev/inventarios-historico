using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using MySql.Data.MySqlClient;
using SDG.Componentes;

public partial class administrador_productos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TNombre.Focus();
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)Usuario.GetUsuario();

            CConfiguracion configuracion = CConfiguracion.GetConfiguracion();

            DCantidades.SelectedValue = configuracion.nCantidad.ToString();
            DVence.SelectedValue = configuracion.nProductoVence.ToString();
            DLote.SelectedValue = configuracion.nProductoLote.ToString();
            DSerial.SelectedValue = configuracion.nProductoSerial.ToString();
            DEstado.SelectedValue = configuracion.nProductoEstado.ToString();

            BAceptar.Enabled = usuario.CanInsert();
            GResultado.Columns[1].Visible = usuario.CanDelete();
            GResultado.Columns[0].Visible = usuario.CanUpdate();

            if(!string.IsNullOrEmpty(Request.QueryString["pid"])) {

                Query query = new Query("producto");
                CConector conector = CConector.GetConector();

                query+="nombre";
                query+="descripcion";
                query+="codigo";
                query+="unidad";
                query+="vence";
                query += "lote";
                query += "empresa";
                query += "entero";
                query += "nEstado";
                query += "serial";
                query += "grupo";
                query += "costo";
                query += "id";
                query += "diasvencimiento";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["pid"])));
                if(reader!=null && reader.Read()) {

                    HId.Value= reader["id"].ToString();
                    TNombre.Text= reader["nombre"].ToString();
                    TDescripcion.Text = reader["descripcion"].ToString();
                    TEmpresa.Text = reader["empresa"].ToString();
                    TCodigo.Text = reader["codigo"].ToString();
                    TUnidad.Text = reader["unidad"].ToString();
                    DVence.SelectedValue = reader["vence"].ToString();
                    DLote.SelectedValue = reader["lote"].ToString();
                    DCantidades.SelectedValue = reader["entero"].ToString();
                    DSerial.SelectedValue = reader["serial"].ToString();
                    DEstado.SelectedValue = reader["nEstado"].ToString();
                    TCosto.Text = Convert.ToDecimal(reader["costo"]).ToString(CultureInfo.InvariantCulture);
                    TDiasVencimiento.Text = reader["diasvencimiento"].ToString();
                    CConector.SelectInDropDown(DGrupo, reader["grupo"]);

                    ViewState["codigo"] = TCodigo.Text;

                    ((ing)Master).SetModificacion(HId.Value);
                    BAceptar.Enabled = usuario.CanUpdate();
                    GCostos.DataBind();
                    PCostos.Visible = CConfiguracion.GetConfiguracion().nCosto!=0 && GCostos.Rows.Count > 0;
                }
            }
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        
        Query query = new Query("producto");
        CConector conector = new CConector();
        string szQuery;
        string mensaje;

        conector.StartTransaction();

        string codigo = "";
        if (!string.IsNullOrEmpty((string)ViewState["codigo"]))
            codigo= ViewState["codigo"].ToString();

        if(TCodigo.Text !=codigo
            && conector.QueryString("SELECT id FROM producto WHERE codigo=" + Query.GetValor(TCodigo.Text) + " AND estado IS NULL").Length>0) {
            ((ing)Master).SetAlerta("Ya existe un producto con código " + TCodigo.Text + ". Presione aceptar para continuar con este código.");
            ViewState["codigo"] = TCodigo.Text;
            return;
        }

        query["nombre"] = TNombre.Text;
        query["descripcion"] = TDescripcion.Text;
        query["codigo"] = TCodigo.Text;
        query["unidad"] = TUnidad.Text;
        query["vence"] = DVence.SelectedValue;
        query["lote"] = DLote.SelectedValue;
        query["empresa"] = TEmpresa.Text;
        query["entero"] = DCantidades.SelectedValue;
        query["grupo"] = DGrupo.SelectedValue;
        query["nEstado"] = DEstado.SelectedValue;
        query["serial"] = DSerial.SelectedValue;
        query["costo"] = TCosto.Text.Length == 0 ? "" :
            decimal.Parse(TCosto.Text, NumberStyles.Currency, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        query["diasvencimiento"]= TDiasVencimiento.Text;

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

        if (conector.QueryMod("DELETE FROM maximo_minimo WHERE producto=" + HId.Value) == -1)
            return;

        query = new Query("maximo_minimo");
        query["producto"]= HId.Value;

        for (int i = 0; i < GAlmacenes.Rows.Count; i++ ) {

            Alertas.RemoveAlertas(GAlmacenes.DataKeys[i].Value.ToString());

            TextBox maximo= (TextBox)GAlmacenes.Rows[i].FindControl("TMaximo");
            TextBox minimo = (TextBox)GAlmacenes.Rows[i].FindControl("TMinimo");

            if(maximo.Text.Length>0 || minimo.Text.Length>0) {

                query["almacen"] = GAlmacenes.DataKeys[i].Value.ToString();
                query["maximo"] = maximo.Text;
                query["minimo"] = minimo.Text;

                if(conector.QueryMod(query.GetInsert())==-1)
                    return;
            }
        }

        conector.Commit();
        Producto.RemoveProducto(HId.Value);
        ((ing)Master).SetResultado(mensaje);
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath + "?pid=" + GResultado.SelectedDataKey.Value.ToString());
    }
    protected void BBuscar_Click(object sender, EventArgs e)
    {
        CFiltrado.Checked = !CFiltrado.Checked;
        BBuscar.Text = CFiltrado.Checked ? "Cancelar filtrado" : "Filtrar";
    }
}
