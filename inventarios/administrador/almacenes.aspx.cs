using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;

public partial class administrador_almacenes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TNombre.Focus();
        if (!IsPostBack)
        {
            CUsuario usuario = (CUsuario)Usuario.GetUsuario();

            BAceptar.Enabled = usuario.CanInsert();
            GResultado.Columns[1].Visible = usuario.CanDelete();
            GResultado.Columns[0].Visible = usuario.CanUpdate();
        }
    }

    protected void BAceptar_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;
        Query query = new Query("almacen");
        CConector conector = CConector.GetConector();
        string szQuery;
        string mensaje;

        query["nombre"] = TNombre.Text;
        query["descripcion"] = TDescripcion.Text;

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

        // creammos las tablas del almacen
        if (conector.QueryMod("CREATE TABLE entrada0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	fecha DATE,		observaciones TEXT,	desde BIGINT UNSIGNED,	traslado BIGINT UNSIGNED,	estado INT,	PRIMARY KEY(id)) ENGINE=InnoDB;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TABLE entrada_items0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	entrada BIGINT UNSIGNED NOT NULL, producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE,	lote VARCHAR(255),	vence DATE,	ingreso DATETIME,	traslado_items BIGINT UNSIGNED,	FOREIGN KEY (entrada) REFERENCES entrada0(id),	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(id)) ENGINE=InnoDB;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TABLE estado0 (	producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE UNSIGNED,	lote VARCHAR(255),	vence DATE,	modificacion DATETIME,	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(producto,lote,vence)) ENGINE=InnoDB;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TABLE traslado0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	envio DATE,		llegada DATE,	observaciones TEXT,	destino BIGINT UNSIGNED,	entrada BIGINT UNSIGNED,	estado INT,	PRIMARY KEY(id)) ENGINE=InnoDB;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TABLE traslado_items0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	traslado BIGINT UNSIGNED NOT NULL,	producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE,	lote VARCHAR(255),	vence DATE,	ingreso DATETIME,	entrada_items BIGINT UNSIGNED,	FOREIGN KEY (traslado) REFERENCES traslado0(id),	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(id)) ENGINE=InnoDB;".Replace("0", HId.Value)) == -1)
            return;

        // creamos los disparadores
        if (conector.QueryMod("CREATE TRIGGER bi_entrada_items0 BEFORE INSERT ON entrada_items0 FOR EACH ROW BEGIN 	IF NEW.ingreso IS NOT NULL THEN 		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence; 	    INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, SYSDATE()); 	END IF; END;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TRIGGER bu_entrada_items0 BEFORE UPDATE ON entrada_items0 FOR EACH ROW BEGIN 	IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;			UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence;			INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, SYSDATE());		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence;		INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, SYSDATE());		END IF; END;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TRIGGER bd_entrada_items0 BEFORE DELETE ON entrada_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN      UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;	END IF; END; ".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TRIGGER bi_traslado_items0 BEFORE INSERT ON traslado_items0 FOR EACH ROW BEGIN 	IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence;	END IF; END;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TRIGGER bu_traslado_items0 BEFORE UPDATE ON traslado_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence;	END IF; END;".Replace("0", HId.Value)) == -1)
            return;
        if (conector.QueryMod("CREATE TRIGGER bd_traslado_items0 BEFORE DELETE ON traslado_items0 FOR EACH ROW BEGIN 	IF OLD.ingreso IS NOT NULL THEN 		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence; 	END IF; END;".Replace("0", HId.Value)) == -1)
            return;
        /*if (conector.QueryMod("CREATE TRIGGER bu_estado0 BEFORE UPDATE ON estado0 FOR EACH ROW BEGIN	IF NEW.cantidad<0 THEN    UPDATE error_cantidades_negativas SET x=1;	END IF; END;".Replace("0", HId.Value)) == -1)
            return;*/

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
}
