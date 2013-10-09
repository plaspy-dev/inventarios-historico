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
        using(CConector conector = new CConector()) {

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

    protected static void UpdateTable(CConector conector, string almacen) {
        
        conector.QueryMod(string.Format("ALTER TABLE entrada_items{0} ADD proveedor BIGINT UNSIGNED DEFAULT 0;", almacen));
        conector.QueryMod(string.Format("ALTER TABLE estado{0} ADD proveedor BIGINT UNSIGNED DEFAULT 0;", almacen));
        conector.QueryMod(string.Format("ALTER TABLE traslado_items{0} ADD proveedor BIGINT UNSIGNED DEFAULT 0;", almacen));
        conector.QueryMod(string.Format("ALTER TABLE salida_items{0} ADD proveedor BIGINT UNSIGNED DEFAULT 0;", almacen));
        conector.QueryMod(string.Format("ALTER TABLE transformacion_salida{0} ADD proveedor BIGINT UNSIGNED DEFAULT 0;", almacen));
        conector.QueryMod(string.Format("ALTER TABLE transformacion_entrada{0} ADD proveedor BIGINT UNSIGNED DEFAULT 0;", almacen));


        conector.QueryMod(string.Format("UPDATE estado{0} SET proveedor=0;", almacen));
        conector.QueryMod(string.Format("DROP TABLE IF EXISTS estadotemp;", almacen));
        conector.QueryMod(string.Format("CREATE TABLE estadotemp LIKE estado{0};", almacen));
        conector.QueryMod(string.Format("ALTER TABLE estadotemp DROP PRIMARY KEY;", almacen));
        conector.QueryMod(string.Format("ALTER TABLE estadotemp ADD PRIMARY KEY (producto,lote,vence,szSerial, ubicacion, estado, proveedor);", almacen));
        conector.QueryMod(string.Format("INSERT INTO estadotemp SELECT * FROM estado{0};", almacen));
        conector.QueryMod(string.Format("DROP TABLE estado{0};", almacen));
        conector.QueryMod(string.Format("RENAME TABLE estadotemp TO estado{0};", almacen));
    }

    protected void BActualizar_Click(object sender, EventArgs e)
    {
        using (CConector conector2 = new CConector())
        using (CConector conector = new CConector())
        {
            // conector.QueryMod("TRUNCATE TABLE estado_global");
            MySqlDataReader reader = conector.Query("SELECT id FROM almacen");
            if (reader != null)
            {
                while (reader.Read())
                {
                    UpdateTable(conector2, reader["id"].ToString());
                }
            }
            UpdateTable(conector2, "0");
        }

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
