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
                query +="szSerial";
                query += "estado";

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
                    TSerial.Text = reader["szSerial"].ToString();
                    TEstado.Text = Almacen.GetNombreEstado(Convert.ToInt32(reader["estado"]));
                    
                    TLote.Visible= producto.nLote > 0;
                    TVencimiento.Visible = producto.nVence > 0;
                    TEstado.Visible = producto.nEstado > 0;
                    TSerial.Visible = producto.nSerial > 0;

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
                    query += "szSerial";
                    query += "estado";

                    reader = conector.Query(query.GetSelect("id=" + Query.GetValor(TNumero.Text)));
                    if(reader!=null && reader.Read()) {

                        producto = new Producto(reader["producto"].ToString());

                        TProductoID2.Text = producto.id;
                        TProducto2.Text = producto.NombreCompleto;
                        TCantidad2.Text = Convert.ToDecimal(reader["cantidad"]).ToString(CultureInfo.InvariantCulture);
                        TLote2.Text = reader["lote"].ToString();
                        CConector.SelectInDropDown(DEstado2,reader["estado"]);
                        TSerial2.Text = reader["szSerial"].ToString();

                        vence=null;
                        try
                        {
                            vence = reader["vence"];
                        } catch { }
                        TVencimiento2.Text = CConector.GetFromMySQLDate(vence);

                        TLote2.Visible = producto.nLote > 0;
                        TVencimiento2.Visible = producto.nVence > 0;
                        DEstado2.Visible = producto.nEstado > 0;
                        TSerial2.Visible = producto.nSerial > 0;
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
        LEstado.CssClass = TEstado.CssClass = pProducto.nEstado > 0 ? "" : "hide";
        LSerial.CssClass = TSerial.CssClass = pProducto.nSerial > 0 ? "" : "hide";

        if (CAutomatico.Checked)
            TSerial.CssClass = TEstado.CssClass = TLote.CssClass = TVencimiento.CssClass = "hide";

        TVencimiento.CssClass += " fecha";
        TEstado.CssClass += " tlote";
        TSerial.CssClass += " tlote";
        

        REntero.Enabled = pProducto.nNumero == 1;
        RDecimal.Enabled = pProducto.nNumero != 1;

        pProducto = new Producto(TProductoID2.Text);

        LVencimiento2.CssClass = TVencimiento2.CssClass = pProducto.nVence > 0 ? "" : "hide";
        LLote2.CssClass = TLote2.CssClass = pProducto.nLote > 0 ? "" : "hide";
        LEstado2.CssClass = DEstado2.CssClass = pProducto.nEstado > 0 ? "" : "hide";
        LSerial2.CssClass = TSerial2.CssClass = pProducto.nSerial > 0 ? "" : "hide";

        if (CAutomatico2.Checked)
            TSerial2.CssClass = DEstado2.CssClass = TLote2.CssClass = TVencimiento2.CssClass = "hide";

        TVencimiento2.CssClass += " fecha";
        TSerial2.CssClass += " tlote";

        if (CAutomatico2.Checked) {
            RSerial.Enabled = REstado.Enabled = RVencimiento.Enabled = RLote.Enabled = false;
        } else {
            RLote.Enabled = pProducto.nLote == 1;
            RVencimiento.Enabled = pProducto.nVence == 1;
            REstado.Enabled = pProducto.nEstado == 1;
            RSerial.Enabled = pProducto.nSerial == 1;
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

        Decimal proporcion = 1;
        // buscamos los nuevo
        if (conector.QueryString("SELECT COUNT(*) FROM transformacion") != "0")
        { // se definieron patrones de trasformacion
            proporcion = -1;
            try
            {
                proporcion = Convert.ToDecimal(conector.QueryString("SELECT proporcion FROM transformacion WHERE producto1=" + Query.GetValor(producto[0]) + " AND producto2=" + Query.GetValor(TProductoID2.Text)));
            }
            catch { }
            if (proporcion == -1)
            {
                try
                {
                    proporcion = Convert.ToDecimal(conector.QueryString("SELECT proporcion FROM transformacion WHERE producto2=" + Query.GetValor(producto[0]) + " AND producto1=" + Query.GetValor(TProductoID2.Text)));
                    proporcion = 1 / proporcion;
                }
                catch { }
            }
            if (proporcion == -1)
            {
                LMensaje.Text = "No se encontro ninguna transformación valida";
                return;
            }
        }

        Decimal cantidad = Convert.ToDecimal(TCantidad.Text, CultureInfo.InvariantCulture);
        if (REntero2.Enabled) { // es un entero

            int entero= Convert.ToInt32(Math.Floor(cantidad/proporcion));
            Decimal cantidadreal= entero*proporcion;
            if(cantidadreal!=cantidad)
                cantidad= cantidadreal;
        }
        if (producto.Length == 1)
        { // automatico
            productos = productousado.GetProductos(producto[0], cantidad, ((ing)Master).GetAlmacen());
        }
        else
        { // manual
            productos = productousado.GetProductos(producto[0], cantidad, ((ing)Master).GetAlmacen(), producto[1], producto[2], Convert.ToInt32(producto[3]), producto[4]);
        }
        if (productos == null)
        {
            LMensaje.Text = "No se encontraron las cantidades disponibles.";
            return;
        }

        for (int i = 0; i < productos.Count; i++)
        {
            salida["observaciones"] = TObservaciones.Text;
            salida["producto"] = ((Producto)productos[i]).id;
            salida["cantidad"] = ((Producto)productos[i]).cantidad.ToString(CultureInfo.InvariantCulture);
            salida["lote"] = ((Producto)productos[i]).lote;
            salida["vence"] = CConector.GetMySQLDateFormat(((Producto)productos[i]).vence);
            salida["szSerial"] = ((Producto)productos[i]).serial;
            salida["estado"] = ((Producto)productos[i]).estado.ToString();
            salida["ingreso"] = "SYSDATE()";
            
            int res = conector.QueryMod(salida.GetInsert(), true);
            if (res == -1)
                return;
            id = res.ToString();

            entrada["id"] = id;
            entrada["producto"] = TProductoID2.Text;
            entrada["cantidad"] = (((Producto)productos[i]).cantidad/proporcion).ToString(CultureInfo.InvariantCulture);
            entrada["ingreso"] = "SYSDATE()";

            if (!CAutomatico2.Checked)
            {
                entrada["lote"] = TLote2.Text;
                entrada["vence"] = CConector.GetMySQLDateFormat(TVencimiento2.Text);
                entrada["szSerial"]=TSerial2.Text;
                entrada["estado"]=DEstado2.SelectedValue;
            }
            else
            {
                entrada["lote"] = ((Producto)productos[i]).lote;
                entrada["vence"] = CConector.GetMySQLDateFormat(((Producto)productos[i]).vence);
                entrada["szSerial"] = ((Producto)productos[i]).serial;
                entrada["estado"] = ((Producto)productos[i]).estado.ToString();
            }

            if (conector.QueryMod(entrada.GetInsert(), true) == -1)
                return;

            if (conector.InsertModificacion(id, ((ing)Master).GetAlmacen()) == -1)
                return;
            HId.Value = id;
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
