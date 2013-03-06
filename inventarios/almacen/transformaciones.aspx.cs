using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Globalization;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class almacen_transformaciones : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((ing)Master).CheckAlmacen();

        /*SProductos.SelectCommand = string.Format(SProductos.SelectCommand, ((ing)Master).GetAlmacen());
        SProductos.UpdateCommand = string.Format(SProductos.UpdateCommand, ((ing)Master).GetAlmacen());
        SProductos.DeleteCommand = string.Format(SProductos.DeleteCommand, ((ing)Master).GetAlmacen());*/

        if (!IsPostBack)
        {
            TNumero.Focus();
            ViewState["almacen"] = ((ing)Master).GetAlmacen();
            CUsuario usuario = (CUsuario)CUsuario.GetUsuario();
            if (!string.IsNullOrEmpty(Request.QueryString["tri"]))
            {
                CConector conector = new CConector();

                Query query = new Query("transformacion_salida" + ((ing)Master).GetAlmacen());

                query += "id";
                query += "observaciones";
                query += "producto";
                query += "cantidad";
                query += "lote";
                query += "vence";

                MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["tri"])));
                if (reader != null && reader.Read())
                {
                    Producto producto = new Producto(reader["producto"].ToString());

                    TNumero.Text = reader["id"].ToString();
                    ((ing)Master).SetModificacion(TNumero.Text, ((ing)Master).GetAlmacen());

                    TObservaciones.Text = reader["observaciones"].ToString();
                    TProductoID.Text = producto.id;
                    TProducto.Text = producto.NombreCompleto;
                    TCantidad.Text = Convert.ToDecimal(reader["cantidad"]).ToString(CultureInfo.InvariantCulture);
                    TLote.Text = reader["lote"].ToString();
                    
                    TLote.Visible= producto.nLote > 0;
                    TVencimiento.Visible = producto.nVence > 0;

                    object vence=null;
                    try {
                        vence=reader["vence"];
                    } catch{}
                    TVencimiento.Text= CConector.GetFromMySQLDate(vence);

                    query = new Query("transformacion_entrada" + ((ing)Master).GetAlmacen());
                    query += "producto";
                    query += "cantidad";
                    query += "lote";
                    query += "vence";

                    reader = conector.Query(query.GetSelect("id=" + Query.GetValor(TNumero.Text)));
                    if(reader!=null && reader.Read()) {

                        producto = new Producto(reader["producto"].ToString());

                        TProductoID2.Text = producto.id;
                        TProducto2.Text = producto.NombreCompleto;
                        TCantidad2.Text = Convert.ToDecimal(reader["cantidad"]).ToString(CultureInfo.InvariantCulture);
                        TLote2.Text = reader["lote"].ToString();
                        vence=null;
                        try
                        {
                            vence = reader["vence"];
                        } catch { }
                        TVencimiento2.Text = CConector.GetFromMySQLDate(vence);

                        TLote2.Visible = producto.nLote > 0;
                        TVencimiento2.Visible = producto.nVence > 0;
                    }
                    CAutomatico.Visible = CAutomatico2.Visible = false;
                    /*CAutomatico2.Checked = CAutomatico.Checked = false;
                    CAutomatico2.CssClass = CAutomatico.CssClass = "hide";*/
                    BAceptar.Enabled = false;
                    return;
                }
            }
            BAceptar.Enabled = usuario.CanInsert();
            TNumero.Text = new CConector().QueryString("SELECT IF(MAX(id),MAX(id)+1,1) FROM transformacion_salida" + ((ing)Master).GetAlmacen());
            TNumero.Focus();
        }
    }
    protected void BAceptar_Click(object sender, EventArgs e)
    {
        string[] producto = TProductoID.Text.Split(';');
        Producto pProducto = new Producto(producto[0]);

        LVencimiento.CssClass = TVencimiento.CssClass = pProducto.nVence > 0 ? "" : "hide";
        LLote.CssClass = TLote.CssClass = pProducto.nLote > 0 ? "" : "hide";
        TVencimiento.CssClass += " fecha";

        REntero.Enabled = pProducto.nNumero == 1;
        RDecimal.Enabled = pProducto.nNumero != 1;

        pProducto = new Producto(TProductoID2.Text);

        LVencimiento2.CssClass = TVencimiento2.CssClass = pProducto.nVence > 0 ? "" : "hide";
        LLote2.CssClass = TLote2.CssClass = pProducto.nLote > 0 ? "" : "hide";
        TVencimiento2.CssClass += " fecha";

        if (CAutomatico2.Checked) {
            RVencimiento.Enabled = RLote.Enabled = false;
        } else {
            RLote.Enabled = pProducto.nLote == 1;
            RVencimiento.Enabled = pProducto.nVence == 1;
        }
        REntero2.Enabled = pProducto.nNumero == 1;
        RDecimal2.Enabled = pProducto.nNumero != 1;

        Validate();

        if (!IsValid)
            return;

        string id = "";
        Query salida = new Query("transformacion_salida" + ((ing)Master).GetAlmacen());
        Query entrada = new Query("transformacion_entrada" + ((ing)Master).GetAlmacen());
        CConector conector = new CConector();
        conector.StartTransaction();

        ArrayList productos;
        ProductoUsado productousado = new ProductoUsado();

        if (producto.Length == 1)
        { // automatico
            productos = productousado.GetProductos(producto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen());
        }
        else
        { // manual
            productos = productousado.GetProductos(producto[0], Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture), ((ing)Master).GetAlmacen(), producto[1], producto[2]);
        }
        if (productos == null)
        {
            LMensaje.Text = "No se encontraron las cantidades disponibles.";
            return;
        }
        Decimal proporcion = 1;
        // buscamos los nuevo
        if (conector.QueryString("SELECT COUNT(*) FROM transformacion") != "0") { // se definieron patrones de trasformacion
            proporcion=-1;
            try {
                proporcion = Convert.ToDecimal(conector.QueryString("SELECT proporcion FROM transformacion WHERE producto1=" + Query.GetValor(producto[0]) + " AND producto2=" + Query.GetValor(TProductoID2.Text)));
                proporcion = 1 / proporcion;
            } catch {}
            if(proporcion==-1) {
                try
                {
                    proporcion = Convert.ToDecimal(conector.QueryString("SELECT proporcion FROM transformacion WHERE producto2=" + Query.GetValor(producto[0]) + " AND producto1=" + Query.GetValor(TProductoID2.Text)));
                }
                catch { }
            }
            if(proporcion==-1) {
                LMensaje.Text = "No se encontro ninguna transformación valida";
                return;
            }
        }


        for (int i = 0; i < productos.Count; i++)
        {
            salida["observaciones"] = TObservaciones.Text;
            salida["producto"] = ((Producto)productos[i]).id;
            salida["cantidad"] = ((Producto)productos[i]).cantidad.ToString(CultureInfo.InvariantCulture);
            salida["lote"] = ((Producto)productos[i]).lote;
            salida["vence"] = CConector.GetMySQLDateFormat(((Producto)productos[i]).vence);
            salida["ingreso"] = "SYSDATE()";

            int res = conector.QueryMod(salida.GetInsert(), true);
            if (res == -1)
                return;
            id = res.ToString();

            entrada["id"] = id;
            entrada["producto"] = TProductoID2.Text;
            entrada["cantidad"] = (((Producto)productos[i]).cantidad * proporcion).ToString(CultureInfo.InvariantCulture);
            entrada["ingreso"] = "SYSDATE()";

            if (!CAutomatico2.Checked)
            {
                entrada["lote"] = TLote2.Text;
                entrada["vence"] = CConector.GetMySQLDateFormat(TVencimiento2.Text);
            }
            else
            {
                entrada["lote"] = ((Producto)productos[i]).lote;
                entrada["vence"] = CConector.GetMySQLDateFormat(((Producto)productos[i]).vence);
            }

            if (conector.QueryMod(entrada.GetInsert()) == -1)
                return;

            if (conector.InsertModificacion(id, ((ing)Master).GetAlmacen()) == -1)
                return;
        }

        conector.Commit();
        ((ing)Master).SetAlerta("Transformación " + HId.Value + " editada.");
        ((ing)Master).SetResultado("Transformación " + id + " editada.");
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
}
