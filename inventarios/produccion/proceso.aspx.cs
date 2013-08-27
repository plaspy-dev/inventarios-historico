using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class produccion_proceso : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            BAceptar.Enabled = usuario.CanInsert();
            if (!string.IsNullOrEmpty(Request.QueryString["oi"]))
            {
                using (CConector conector = new CConector())
                {
                    Query query = new Query("orden_produccion");

                    query += "id";
                    query += "lote";
                    query += "planeacion";
                    query += "produccion";
                    query += "producto";
                    query += "marmita";
                    query += "cantidad";
                    query += "estado";

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["oi"])));
                    if (reader != null && reader.Read())
                    {
                        PAnulado.Visible = reader["estado"].ToString() != "";
                        if (PAnulado.Visible)
                        {
                            LAnulado.Text = "Anulada";
                            BAceptar.Enabled = false;
                        }

                        Session["parametros"] = Request.Url.Query;
                        PBordes.Visible = false;
                        PAnalisis.Visible = true;
                        ViewState["id"] = TNumero.Text = reader["id"].ToString();
                        Conector.SelectInDropDown(DMarmita, reader["marmita"]);
                        LCantidad.Text = Convert.ToDecimal(reader["cantidad"]).ToString("0.#########", CultureInfo.InvariantCulture) + " Kg.";

                        BAceptar.Enabled = usuario.CanUpdate();

                        ((ing)Master).SetModificacion(TNumero.Text);
                        // ((ing)Master).SetPrintFile("orden.html");

                        TLote.Text = reader["lote"].ToString();
                        LPlaneacion.Text = Conector.GetFromMySQLDate(reader["planeacion"]);
                        LProduccion.Text = Conector.GetFromMySQLDate(reader["produccion"]);
                        Producto producto = new Producto(reader["producto"].ToString());
                        LProducto.Text = producto.NombreCompleto
                            + "<br/>"
                            + producto.Empresa;
                        ViewState["FormatoLote"] = producto.FormatoLote.Replace("%codigo%", producto.codigo);

                        query = new Query("proceso_orden_produccion");

                        query += "produccion_fecha";
                        query += "produccion_inicio";
                        query += "produccion_fin";
                        query += "produccion_operararios";
                        query += "empaque_fecha";
                        query += "empaque_inicio";
                        query += "empaque_fin";
                        query += "empaque_operarios";
                        query += "unidad1";
                        query += "unidad2";
                        query += "presentacion1";
                        query += "presentacion2";
                        query += "observaciones";
                        query += "contenedor";
                        query += "reproceso";

                        reader= conector.Query(query.GetSelect("orden=" + Query.GetValor(TNumero.Text)));
                        if (reader!=null && reader.Read()) {

                            ViewState["orden"] = TNumero.Text;
                            TFechaInicio.Text = Conector.GetFromMySQLDate(reader["produccion_fecha"]);
                            TInicioProceso.Text = GetHora(reader["produccion_inicio"]);
                            TFinProceso.Text = GetHora(reader["produccion_fin"]);
                            TFechaEmpaque.Text = Conector.GetFromMySQLDate(reader["empaque_fecha"]);
                            TInicioEmpaque.Text = GetHora(reader["empaque_inicio"]);
                            TFinEmpaque.Text = GetHora(reader["empaque_fin"]);
                            TOperariosProceso.Text = reader["produccion_operararios"].ToString();
                            TOperariosEmpaque.Text = reader["empaque_operarios"].ToString();
                            TCantidadUnidad1.Text = reader["unidad1"] == DBNull.Value ? "" : Convert.ToDouble(reader["unidad1"]).ToString(CultureInfo.InvariantCulture);
                            TCantidadUnidad2.Text = reader["unidad2"] == DBNull.Value ? "" : Convert.ToDouble(reader["unidad2"]).ToString(CultureInfo.InvariantCulture);
                            TReproceso.Text = reader["reproceso"] == DBNull.Value ? "" : Convert.ToDouble(reader["reproceso"]).ToString(CultureInfo.InvariantCulture);
                            TCantidadPresentacion1.Text = reader["presentacion1"] == DBNull.Value ? "" : Convert.ToDouble(reader["presentacion1"]).ToString(CultureInfo.InvariantCulture);
                            TCantidadPresentacion2.Text = reader["presentacion2"] == DBNull.Value ? "" : Convert.ToDouble(reader["presentacion2"]).ToString(CultureInfo.InvariantCulture);
                            TObservaciones.Text = reader["observaciones"].ToString();
                            TContenedor.Text = reader["contenedor"].ToString();
                        }
                    }
                }
            }
        }
    }

    protected string GetHora(object hora) {

        try
        {
            return hora.ToString().Substring(0, 5);
        } catch { }
        return "";
    }

    protected void BAceptar_Clicked(object sender, EventArgs e) {
        
        if (!IsValid)
            return;

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();

            Query query = new Query("proceso_orden_produccion");
            
            string orden= (string)ViewState["orden"];
            query["produccion_fecha"] = Conector.GetMySQLDateFormat(TFechaInicio.Text);
            query["produccion_inicio"] = TInicioProceso.Text;
            query["produccion_fin"] = TFinProceso.Text;
            query["empaque_fecha"] = Conector.GetMySQLDateFormat(TFinEmpaque.Text);
            query["empaque_inicio"]= TInicioEmpaque.Text;
            query["empaque_fin"]= TFinEmpaque.Text;
            query["produccion_operararios"] = TOperariosProceso.Text;
            query["empaque_operarios"] = TOperariosEmpaque.Text;
            query["unidad1"]= TCantidadUnidad1.Text;
            query["unidad2"]= TCantidadUnidad2.Text;
            query["presentacion1"]= TCantidadPresentacion1.Text.Replace(" Kg.","");
            query["presentacion2"] = TCantidadPresentacion2.Text.Replace(" Kg.", "");
            query["reproceso"] = TReproceso.Text.Replace(" Kg.", "");
            query["observaciones"] = TObservaciones.Text;
            query["contenedor"] = TContenedor.Text;

            if (string.IsNullOrEmpty(orden))
            {
                orden= query["orden"] = ViewState["id"].ToString();
                if(conector.QueryMod(query.GetInsert())==-1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }
            else { 
                if(conector.QueryMod(query.GetUpdate("orden=" + Query.GetValor(orden)))==-1) {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }

            query = new Query("orden_produccion");

            query["marmita"] = DMarmita.SelectedValue;
            query["lote"] = TLote.Text;

            if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(orden))) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            if (conector.InsertModificacion(orden) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            conector.Commit();
            Session["parametros"] = "?oi=" + TNumero.Text;
            ((ing)Master).SetResultado("Orden de producción " + TNumero.Text + " Editada");
        }
    }

    protected void BCancelar_Clicked(object sender, EventArgs e) {
        
        Response.Redirect(Request.Url.AbsolutePath);
    }
}
