using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class produccion_formulacion : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        if (!IsPostBack) {

            GProductos.DataBind();
            CUsuario usuario= (CUsuario)Usuario.GetInstance();
            TProducto.Focus();

            BAceptar.Enabled = usuario.CanInsert();
            if (!string.IsNullOrEmpty(Request.QueryString["fi"])) { 

                using(CConector conector= new CConector()) {
                    Query query= new Query("formulacion");

                    query += "id";
                    query+="producto";

                    MySqlDataReader reader = conector.Query(query.GetSelect("id=" + Query.GetValor(Request.QueryString["fi"])));
                    if (reader != null && reader.Read()) {

                        BEliminar.Visible = usuario.CanDelete();
                        BAceptar.Enabled = usuario.CanUpdate();

                        ((ing)Master).SetModificacion(reader["id"].ToString());

                        ViewState["id"] = reader["id"].ToString();
                        Producto info= new Producto(reader["producto"].ToString());
                        
                        TProducto.Text = info.NombreCompleto;
                        TProductoID.Text = info.id;
                        LUnidad.Text = info.unidad;

                        query = new Query("formulacion_ingrediente");

                        query += "producto";
                        query += "cantidad";

                        List<Producto> lista = new List<Producto>();
                        reader = conector.Query(query.GetSelect("formulacion=" + Query.GetValor(Request.QueryString["fi"])));
                        if (reader != null) {
                            while (reader.Read()) {
                                info = new Producto(reader["producto"].ToString());
                                info.cantidad= Convert.ToDecimal(reader["cantidad"]);
                                info.IdGrid = lista.Count;
                                lista.Add(info);
                            }
                        }
                        HProductos.Value = Serializar.SerializeBinary(lista);
                        GProductos.DataSource = lista;
                        GProductos.DataBind();

                        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");
                    }
                }
            }
        }
    }

    protected void GProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
        try
        {
            Producto info = lista
                .Where(x => x.IdGrid.ToString() == GProductos.DataKeys[e.RowIndex].Values[0].ToString())
                .ToList().First();
            TIngredienteID.Text = info.id;
            TIngrediente.Text= info.NombreCompleto;
            TCantidadIngrediente.Text = info.cantidad.ToString(CultureInfo.InvariantCulture);
            lista.Remove(info);

            for (int i = 0; i < lista.Count; i++)
                lista[i].IdGrid = i;

            HProductos.Value = Serializar.SerializeBinary(lista);
            GProductos.DataSource = lista;
            GProductos.DataBind();
            GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");
        }
        catch { }
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect("~/produccion/formulacion.aspx?fi=" + GResultado.SelectedValue.ToString());
    }

    protected void BMas_Click(object sender, EventArgs e) {

        if (!IsValid)
            return;

        List<Producto> lista = Serializar.DesSerializeBinary <List<Producto>> (HProductos.Value);
        Producto info = new Producto(TIngredienteID.Text);
        info.cantidad = Convert.ToDecimal(TCantidadIngrediente.Text, CultureInfo.InvariantCulture);
        info.IdGrid = lista.Count;
        lista.Add(info);

        HProductos.Value = Serializar.SerializeBinary(lista);
        GProductos.DataSource = lista;
        GProductos.DataBind();
        GProductos.FooterRow.Cells[Utilidades.GetGridColumnIndex(GProductos, "Cantidad")].Text = lista.Sum(x => x.cantidad).ToString("0.00");

        TIngrediente.Focus();
        TCantidadIngrediente.Text = 
        TIngredienteID.Text= TIngrediente.Text = "";
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        if (GProductos.Rows.Count == 0)
        {
            LMensaje.Text = "Por favor ingrese por lo menos un producto";
            TIngrediente.Focus();
            return;
        }

        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            Query query = new Query("formulacion");
            string id = (string)ViewState["id"];
            query["producto"] = TProductoID.Text;

            if (string.IsNullOrEmpty(id))
            {
                int res = conector.QueryMod(query.GetInsert(), true);
                if (res == -1)
                {
                    LMensaje.Text = conector.GetLastError();
                    return;
                }
                id = res.ToString();
            }
            else
            {
                if (conector.QueryMod(query.GetUpdate("id=" + Query.GetValor(id))) == -1)
                {

                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }
            if (conector.InsertModificacion(id) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }

            if (conector.QueryMod("DELETE FROM formulacion_ingrediente WHERE formulacion=" + Query.GetValor(id)) == -1)
            {

                LMensaje.Text = conector.GetLastError();
                return;
            }

            List<Producto> lista = Serializar.DesSerializeBinary<List<Producto>>(HProductos.Value);
            query = new Query("formulacion_ingrediente");
            query["formulacion"] = id;

            for (int i = 0; i < lista.Count; i++)
            {

                query["producto"] = lista[i].id;
                query["cantidad"] = lista[i].cantidad.ToString(CultureInfo.InvariantCulture);

                if (conector.QueryMod(query.GetInsert()) == -1)
                {

                    LMensaje.Text = conector.GetLastError();
                    return;
                }
            }

            conector.Commit();
        }
        ((ing)Master).SetResultado("Producto " + TProducto.Text + " Editado");
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/produccion/formulacion.aspx");
    }

    protected void BEliminar_Click(object sender, EventArgs e)
    {
        using (CConector conector = new CConector())
        {
            conector.StartTransaction();
            string id = ViewState["id"].ToString();
            if (conector.QueryMod("DELETE FROM formulacion_ingrediente WHERE formulacion=" + Query.GetValor(id)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            if (conector.QueryMod("DELETE FROM formulacion WHERE id=" + Query.GetValor(id)) == -1)
            {
                LMensaje.Text = conector.GetLastError();
                return;
            }
            conector.Commit();
            ((ing)Master).SetResultado("Producto " + TProducto.Text + " Eliminado");
        }
    }
}
