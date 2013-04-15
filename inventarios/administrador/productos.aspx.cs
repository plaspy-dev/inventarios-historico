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
    [Serializable]
    class PrecioCliente {

        public int cliente { get; set; }
        public string NombreCliente { get; set; }
        public Decimal valor { get; set; }

        public PrecioCliente(int nCliente, string nombrecliente, decimal dValor) {
            cliente = nCliente;
            valor = dValor;
            NombreCliente = nombrecliente;
        }
    }

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
            TDAlertaV.Text = configuracion.nDiasVencimiento.ToString();

            Utilidades.GetGridColumn(GResultado, "Último costo").Visible = configuracion.nCosto != 0;

            Utilidades.GetGridColumn(GResultado, "Precio").Visible =
            lprecios.Visible=
            UPrecios.Visible = configuracion.nValor != 0;

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
                query += "formatolote";
                query += "dias_alerta";
                query += "maximo";
                query += "minimo";
                query += "valor_temporal";
                query += "valor";
                query += "maxmin_tipo";

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
                    TDAlertaV.Text = reader["dias_alerta"].ToString();
                    CMinMaxEstado.Checked = reader["maxmin_tipo"].ToString() == "1";
                    try
                    {
                        TMaximo.Text = Convert.ToDouble(reader["maximo"]).ToString(CultureInfo.InvariantCulture);
                    } catch {}
                    try
                    {
                        TMinimo.Text = Convert.ToDouble(reader["minimo"]).ToString(CultureInfo.InvariantCulture);
                    }
                    catch { }
                    try
                    {
                        TCosto.Text = Convert.ToDecimal(reader["costo"]).ToString(CultureInfo.InvariantCulture);
                    } catch {}
                    try {
                        TUltimoPrecio.Text = Convert.ToDecimal(reader["valor_temporal"]).ToString(CultureInfo.InvariantCulture);
                    } catch {}
                    try
                    {
                        TPrecio.Text = Convert.ToDecimal(reader["valor"]).ToString(CultureInfo.InvariantCulture);
                    }
                    catch { }
                    TDiasVencimiento.Text = reader["diasvencimiento"].ToString();
                    TFormatoLote.Text = reader["formatolote"].ToString();
                    CConector.SelectInDropDown(DGrupo, reader["grupo"]);

                    ViewState["codigo"] = TCodigo.Text;

                    ((ing)Master).SetModificacion(HId.Value);
                    BAceptar.Enabled = usuario.CanUpdate();
                    GCostos.DataBind();

                    List<PrecioCliente> preciocliente= new List<PrecioCliente>();

                    reader = conector.Query(string.Format("SELECT tercero.id,tercero.nombre,valor_cliente.valor"
                                        + " FROM valor_cliente"
                                        + " LEFT JOIN tercero ON (tercero.id=valor_cliente.cliente)"
                                        + " WHERE valor_cliente.producto={0}"
                                        + " ORDER BY tercero.nombre", HId.Value));
                    if(reader!=null) {
                        while(reader.Read()) {
                            preciocliente.Add(new PrecioCliente(
                                Convert.ToInt32(reader["id"]),
                                reader["nombre"].ToString(),
                                Convert.ToDecimal(reader["valor"])));
                        }
                    }
                    GPreciosC.DataSource = preciocliente;
                    GPreciosC.DataBind();
                    ViewState["cliente"] = preciocliente;

                    lcostos.Visible= 
                    PCostos.Visible = CConfiguracion.GetConfiguracion().nCosto!=0;
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
        query["dias_alerta"] = TDAlertaV.Text;
        query["minimo"] = TMinimo.Text;
        query["maximo"] = TMaximo.Text;
        query["maxmin_tipo"] = CMinMaxEstado.Checked ? "1" : "";
        query["costo"] = TCosto.Text.Length == 0 ? "" :
            decimal.Parse(TCosto.Text, NumberStyles.Currency, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        query["valor"] = TPrecio.Text.Length == 0 ? "" :
            decimal.Parse(TPrecio.Text, NumberStyles.Currency, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        query["valor_temporal"] = TUltimoPrecio.Text.Length == 0 ? "" :
            decimal.Parse(TUltimoPrecio.Text, NumberStyles.Currency, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        query["diasvencimiento"]= TDiasVencimiento.Text;
        query["formatolote"] = TFormatoLote.Text;

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

        if (conector.QueryMod("DELETE FROM valor_almacen WHERE producto=" + HId.Value) == -1)
            return;

        query = new Query("valor_almacen");
        query["producto"] = HId.Value;

        // agregamos los precios por almacen
        for (int i = 0; i < GPreciosA.Rows.Count; i++)
        {
            TextBox valor = (TextBox)GPreciosA.Rows[i].FindControl("TValor");

            if (valor.Text.Length>0)
            {
                query["almacen"] = GPreciosA.DataKeys[i].Value.ToString();
                query["valor"] = decimal.Parse(valor.Text, NumberStyles.Currency, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

                if (conector.QueryMod(query.GetInsert()) == -1)
                    return;
            }
        }

        // agregamos los precios por cliente
        if (conector.QueryMod("DELETE FROM valor_cliente WHERE producto=" + HId.Value) == -1)
            return;

        query = new Query("valor_cliente");
        query["producto"] = HId.Value;

        List<PrecioCliente> preciocliente = (List<PrecioCliente>)ViewState["cliente"];

        // agregamos los precios por almacen
        if(preciocliente!=null) {

            for (int i = 0; i < preciocliente.Count; i++)
            {
                query["cliente"] = preciocliente[i].cliente.ToString();
                query["valor"] = preciocliente[i].valor.ToString(CultureInfo.InvariantCulture);

                if (conector.QueryMod(query.GetInsert()) == -1)
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

    protected void BCAgregar_Click(object sender, EventArgs e) {
        
        TTercero.Text = TTerceroID.Text = TValor.Text = "";
        TTercero.Focus();
    }

    protected void BAgregar_Click(object sender, EventArgs e)
    {
        if(!IsValid)
            return;

        List<PrecioCliente> preciocliente = (List<PrecioCliente>)ViewState["cliente"];
        if(preciocliente==null)
            preciocliente = new List<PrecioCliente>();

        List<PrecioCliente> listado= (from p in preciocliente
                        where p.cliente.ToString() == TTerceroID.Text
                         select p).ToList();

        PrecioCliente info;
        if (listado.Count == 0)
        {
            info = new PrecioCliente(Convert.ToInt32(TTerceroID.Text)
                        , TTercero.Text
                        , 0);
            preciocliente.Add(info);
        } else {
            info = listado[0];
        }
        info.valor = Decimal.Parse(TValor.Text, NumberStyles.Currency, CultureInfo.InvariantCulture); 

        ViewState["cliente"] = preciocliente;
        GPreciosC.DataSource = preciocliente;
        GPreciosC.DataBind();

        BCAgregar_Click(sender, e);
    }

    protected void GPreciosC_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        List<PrecioCliente> preciocliente = (List<PrecioCliente>)ViewState["cliente"];
        if (preciocliente == null)
        {
            e.Cancel = true; 
            return;
        }
        List<PrecioCliente> listado = (from p in preciocliente
                                       where p.cliente.ToString() == GPreciosC.DataKeys[e.RowIndex].Value.ToString()
                                       select p).ToList();
        if (listado.Count != 0) {
            PrecioCliente info = listado[0];

            TTercero.Text = info.NombreCliente;
            TTerceroID.Text = info.cliente.ToString();
            TValor.Text = info.valor.ToString(CultureInfo.InvariantCulture);

            preciocliente.Remove(info);
            ViewState["cliente"] = preciocliente;
            GPreciosC.DataSource = preciocliente;
            GPreciosC.DataBind();
        }
        e.Cancel = true;
    }
}
