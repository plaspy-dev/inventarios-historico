using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

public partial class administrador_almacenes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CConector.ReplaceConnectionString(Master.Controls);
        TNombre.Focus();
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)Usuario.GetUsuario();

            BAceptar.Enabled = usuario.CanInsert();
            GResultado.Columns[1].Visible = usuario.CanDelete();
            GResultado.Columns[0].Visible = usuario.CanUpdate();
#if DEBUG
            BActualizar.Visible= usuario.CanAdmin();
#endif
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("almacen");
        CConector conector = new CConector();
        string szQuery;
        string mensaje;
        bool crearalmacenes = false;

        conector.StartTransaction();

        query["nombre"] = TNombre.Text;
        query["descripcion"] = TDescripcion.Text;

        if (HId.Value.Length == 0)
        {
            crearalmacenes = true;
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

        string []consultas= SUbicaciones.GetQuerys();
        for (int i = 0; i < consultas.Length; i++)
            if (conector.QueryMod(consultas[i].Replace("@almacen", HId.Value)) == -1)
                return;

        if (crearalmacenes)
        {
            StreamReader filereader = new StreamReader(Server.MapPath("~/db/almacen.sql"));
            string fullquery="";
            while (filereader.Peek() >= 0)
            {
                string linea = filereader.ReadLine();
                fullquery += "\n" + linea;
                if(fullquery.IndexOf(";")!=-1) {
                    if (conector.QueryMod(Almacen.ReplaceAlmacenTables(fullquery, HId.Value)) == -1)
                        return;
                    fullquery = "";
                }
            }
            filereader.Close();
        }

        conector.Commit();

        MySqlDataReader reader = conector.Query("SELECT id FROM ubicacion WHERE almacen=" + HId.Value + " AND estado IS NULL");
        if (reader != null)
        {
            while (reader.Read()) // limpiamos el cache de las ubicaciones
                Almacen.RemoveUbicacion(Convert.ToInt32(reader["id"]));
        }

        ((ing)Master).SetResultado(mensaje);
    }

    protected void BCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void GResultado_SelectedIndexChanged(object sender, EventArgs e)
    {
        HId.Value = GResultado.SelectedDataKey.Value.ToString();
        TNombre.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[2].Text).Trim();
        TDescripcion.Text = HttpUtility.HtmlDecode(GResultado.SelectedRow.Cells[3].Text).Trim();

        ((ing)Master).SetModificacion(HId.Value);
    }

    protected void BActualizar_Click(object sender, EventArgs e)
    {
        CConector conector = new CConector();

        conector.QueryMod("TRUNCATE TABLE estado_global");
        MySqlDataReader reader= conector.Query("SELECT id FROM almacen");
        if(reader!=null) {

            while(reader.Read()) {

                CConector conector2= new CConector();

                HId.Value= reader["id"].ToString();
            }
        }

        HId.Value = "0";
        
        CConector conector3 = new CConector();

        ((ing)Master).SetResultado("Almacenes actualizados");
    }

    protected void BMas_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        IOrderedDictionary data = new OrderedDictionary();
        data.Add("nombre", TUbicacion.Text);

        if (GUbicaciones.SelectedIndex == -1)
        {
            SUbicaciones.GetView().Insert(data, OnDataSourceOperationComplete);
        }
        else
        {
            data.Add("id", GUbicaciones.DataKeys[GUbicaciones.SelectedIndex].Value);

            IOrderedDictionary dataold = new OrderedDictionary();
            dataold.Add("id", GUbicaciones.DataKeys[GUbicaciones.SelectedIndex].Value);
            SUbicaciones.GetView().Update(data, data, dataold, OnDataSourceOperationComplete);
        }
        TUbicacion.Text = "";
        GUbicaciones.SelectedIndex = -1;
        GUbicaciones.DataBind();
    }

    private bool OnDataSourceOperationComplete(int count, Exception e)
    {
        if (e != null)
        {
            throw e;
        }
        return false;
    }

    protected void GUbicaciones_SelectedIndexChanged(object sender, EventArgs e)
    {
        TUbicacion.Text = HttpUtility.HtmlDecode(GUbicaciones.SelectedRow.Cells[2].Text);
    }
}
