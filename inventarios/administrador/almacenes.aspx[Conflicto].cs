using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SDG.Componentes;
using MySql.Data.MySqlClient;

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
        CConector conector = CConector.GetConector();
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

        conector.Commit();

        MySqlDataReader reader= conector.Query("SELECT id FROM ubicacion WHERE almacen=" + HId.Value + " AND estado IS NULL");
        if(reader!=null) {
            while(reader.Read()) // limpiamos el cache de las ubicaciones
                Almacen.RemoveUbicacion(Convert.ToInt32(reader["id"]));
        }

        if (crearalmacenes)
        {
            // creammos las tablas del almacen
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE entrada0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	fecha DATE,		observaciones TEXT,	desde BIGINT UNSIGNED,	traslado BIGINT UNSIGNED,	estado INT,	documento VARCHAR(255), tercero BIGINT UNSIGNED, FOREIGN KEY (tercero) REFERENCES tercero(id), PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE entrada_items0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	entrada BIGINT UNSIGNED NOT NULL, producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE,	lote VARCHAR(255),	vence DATE,szSerial VARCHAR(255) DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,estado BIGINT NOT NULL DEFAULT 0,	ingreso DATETIME,   costo DECIMAL DEFAULT 0, traslado_items BIGINT UNSIGNED,	FOREIGN KEY (entrada) REFERENCES entrada0(id),	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE estado0 (	producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE UNSIGNED,	lote VARCHAR(255),	vence DATE,szSerial VARCHAR(255) NOT NULL DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,estado BIGINT NOT NULL DEFAULT 0,	modificacion DATETIME, costo DECIMAL DEFAULT 0,	cantidadacumulada DOUBLE DEFAULT 0, FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(producto,lote,vence,szSerial, ubicacion,estado)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE traslado0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	envio DATE,		llegada DATE,	observaciones TEXT,	destino BIGINT UNSIGNED,	entrada BIGINT UNSIGNED,	estado INT,	documento VARCHAR(255), PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE traslado_items0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	traslado BIGINT UNSIGNED NOT NULL,	producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE,	lote VARCHAR(255),	vence DATE,szSerial VARCHAR(255) DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,estado BIGINT NOT NULL DEFAULT 0,	ingreso DATETIME,   costo DECIMAL DEFAULT 0,	entrada_items BIGINT UNSIGNED,	FOREIGN KEY (traslado) REFERENCES traslado0(id),	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE salida0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	fecha DATE,		observaciones TEXT,	estado INT,	documento VARCHAR(255), tercero BIGINT UNSIGNED, FOREIGN KEY (tercero) REFERENCES tercero(id), PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE salida_items0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	salida BIGINT UNSIGNED NOT NULL,	producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE,	lote VARCHAR(255),	vence DATE,szSerial VARCHAR(255) DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,estado BIGINT NOT NULL DEFAULT 0,	ingreso DATETIME,   costo DECIMAL DEFAULT 0,	FOREIGN KEY (salida) REFERENCES salida0(id),	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE transformacion_salida0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE,	lote VARCHAR(255),	vence DATE,szSerial VARCHAR(255) DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,estado BIGINT NOT NULL DEFAULT 0,	ingreso DATETIME,   costo DECIMAL DEFAULT 0,	observaciones TEXT,	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE transformacion_entrada0 (	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,	producto BIGINT UNSIGNED NOT NULL,	cantidad DOUBLE,	lote VARCHAR(255),	vence DATE,szSerial VARCHAR(255) DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,estado BIGINT NOT NULL DEFAULT 0,	ingreso DATETIME,   costo DECIMAL DEFAULT 0,	FOREIGN KEY (producto) REFERENCES producto(id),	PRIMARY KEY(id)) ENGINE=InnoDB;", HId.Value)) == -1)
                return;
            /*if (conector.QueryMod(Almacen.ReplaceAlmacenTables("", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("", HId.Value)) == -1)
                return;*/

            // creamos los disparadores
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_entrada_items0 BEFORE INSERT ON entrada_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;	IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	    INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial, NEW.ubicacion, NEW.estado,NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_entrada_items0 BEFORE UPDATE ON entrada_items0 FOR EACH ROW BEGIN 	IF NEW.lote IS NULL THEN 		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;	IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR NEW.estado!=OLD.estado THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;			INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion, estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad,NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad,szSerial,ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_entrada_items0 BEFORE DELETE ON entrada_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN      UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_traslado_items0 BEFORE INSERT ON traslado_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_traslado_items0 BEFORE UPDATE ON traslado_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_traslado_items0 BEFORE DELETE ON traslado_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_salida_items0 BEFORE INSERT ON salida_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_salida_items0 BEFORE UPDATE ON salida_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR  OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_salida_items0 BEFORE DELETE ON salida_items0 FOR EACH ROW BEGIN 	IF OLD.ingreso IS NOT NULL THEN 		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado; 	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_transformacion_salida0 BEFORE INSERT ON transformacion_salida0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_transformacion_salida0 BEFORE UPDATE ON transformacion_salida0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_transformacion_salida0 BEFORE DELETE ON transformacion_salida0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_transformacion_entrada0 BEFORE INSERT ON transformacion_entrada0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	    INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial,ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial, NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_transformacion_entrada0 BEFORE UPDATE ON transformacion_entrada0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;			INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad,NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad,szSerial, ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_transformacion_entrada0 BEFORE DELETE ON transformacion_entrada0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN      UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER ai_estado0 AFTER INSERT ON estado0 FOR EACH ROW BEGIN 	UPDATE estado0 SET costo=(SELECT SUM(cantidad*costo)/SUM(cantidad) FROM entrada_items0 WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado AND ingreso IS NOT NULL) WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado; END;", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER au_estado0 AFTER UPDATE ON estado0 FOR EACH ROW BEGIN 	UPDATE estado0 SET costo=(SELECT SUM(cantidad*costo)/SUM(cantidad) FROM entrada_items0 WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado AND ingreso IS NOT NULL) WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado; END;", HId.Value)) == -1)
                return;
            
            /*if (conector.QueryMod(Almacen.ReplaceAlmacenTables("", HId.Value)) == -1)
                return;
            if (conector.QueryMod(Almacen.ReplaceAlmacenTables("", HId.Value)) == -1)
                return;*/
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
        MySqlDataReader reader= conector.Query("SELECT id FROM almacen");
        if(reader!=null) {

            while(reader.Read()) {

                CConector conector2= new CConector();

                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada_items0 ADD estado BIGINT NOT NULL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE estado0 ADD estado BIGINT NOT NULL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE traslado_items0 ADD estado BIGINT NOT NULL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida_items0 ADD estado BIGINT NOT NULL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_salida0 ADD estado BIGINT NOT NULL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_entrada0 ADD estado BIGINT NOT NULL DEFAULT 0;", reader["id"].ToString()));

                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada_items0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE estado0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE traslado_items0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida_items0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_salida0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_entrada0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", reader["id"].ToString()));

                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada_items0 ADD costo DECIMAL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE estado0 ADD costo DECIMAL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE traslado_items0 ADD costo DECIMAL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida_items0 ADD costo DECIMAL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_salida0 ADD costo DECIMAL DEFAULT 0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_entrada0 ADD costo DECIMAL DEFAULT 0;", reader["id"].ToString()));

                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada0 ADD tercero BIGINT UNSIGNED;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada0 ADD FOREIGN KEY (tercero) REFERENCES tercero(id);", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida0 ADD tercero BIGINT UNSIGNED;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida0 ADD FOREIGN KEY (tercero) REFERENCES tercero(id);", reader["id"].ToString()));

                conector2.QueryMod(Almacen.ReplaceAlmacenTables("RENAME TABLE estado0 TO estado0_temp;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TABLE estado0 ( producto BIGINT UNSIGNED NOT NULL, cantidad DOUBLE UNSIGNED, lote VARCHAR(255), vence DATE, szSerial VARCHAR(255) NOT NULL DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,estado BIGINT NOT NULL DEFAULT 0, modificacion DATETIME, FOREIGN KEY (producto) REFERENCES producto(id),PRIMARY KEY(producto,lote,vence,szSerial, ubicacion,estado)) ENGINE=InnoDB;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("INSERT INTO estado0 SELECT producto,cantidad,lote,vence,szSerial,ubicacion,estado,modificacion FROM estado0_temp;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TABLE estado0_temp;", reader["id"].ToString()));

                conector2.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE estado0 ADD cantidadacumulada DOUBLE DEFAULT 0;", reader["id"].ToString()));

                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bi_entrada_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bu_entrada_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bd_entrada_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bi_traslado_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bu_traslado_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bd_traslado_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bi_salida_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bu_salida_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bd_salida_items0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bi_transformacion_salida0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bu_transformacion_salida0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bd_transformacion_salida0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bi_transformacion_entrada0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bu_transformacion_entrada0;", reader["id"].ToString()));
                conector2.QueryMod(Almacen.ReplaceAlmacenTables("DROP TRIGGER bd_transformacion_entrada0;", reader["id"].ToString()));

                HId.Value= reader["id"].ToString();

                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_entrada_items0 BEFORE INSERT ON entrada_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;	IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	    INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion,estado, costo, cantidadacumulada, modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial, NEW.ubicacion, NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_entrada_items0 BEFORE UPDATE ON entrada_items0 FOR EACH ROW BEGIN 	IF NEW.lote IS NULL THEN 		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;	IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR NEW.estado!=OLD.estado THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;			INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion, estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad,NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad,szSerial,ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_entrada_items0 BEFORE DELETE ON entrada_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN      UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_traslado_items0 BEFORE INSERT ON traslado_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_traslado_items0 BEFORE UPDATE ON traslado_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_traslado_items0 BEFORE DELETE ON traslado_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_salida_items0 BEFORE INSERT ON salida_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_salida_items0 BEFORE UPDATE ON salida_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR  OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_salida_items0 BEFORE DELETE ON salida_items0 FOR EACH ROW BEGIN 	IF OLD.ingreso IS NOT NULL THEN 		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado; 	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_transformacion_salida0 BEFORE INSERT ON transformacion_salida0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_transformacion_salida0 BEFORE UPDATE ON transformacion_salida0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_transformacion_salida0 BEFORE DELETE ON transformacion_salida0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_transformacion_entrada0 BEFORE INSERT ON transformacion_entrada0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	    INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial,ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial, NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_transformacion_entrada0 BEFORE UPDATE ON transformacion_entrada0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;			INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad,NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad,szSerial, ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_transformacion_entrada0 BEFORE DELETE ON transformacion_entrada0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN      UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER ai_estado0 AFTER INSERT ON estado0 FOR EACH ROW BEGIN 	UPDATE estado0 SET costo=(SELECT SUM(cantidad*costo)/SUM(cantidad) FROM entrada_items0 WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado AND ingreso IS NOT NULL) WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado; END;", HId.Value)) == -1)
                    return;
                if (conector2.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER au_estado0 AFTER UPDATE ON estado0 FOR EACH ROW BEGIN 	UPDATE estado0 SET costo=(SELECT SUM(cantidad*costo)/SUM(cantidad) FROM entrada_items0 WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado AND ingreso IS NOT NULL) WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado; END;", HId.Value)) == -1)
                    return;
            }
        }
        
        CConector conector3 = new CConector();

        conector3.QueryMod("ALTER TABLE entrada_items0 ADD estado BIGINT NOT NULL DEFAULT 0;");
        conector3.QueryMod("ALTER TABLE estado0 ADD estado BIGINT NOT NULL DEFAULT 0;");
        conector3.QueryMod("ALTER TABLE traslado_items0 ADD estado BIGINT NOT NULL DEFAULT 0;");
        conector3.QueryMod("ALTER TABLE salida_items0 ADD estado BIGINT NOT NULL DEFAULT 0;");
        conector3.QueryMod("ALTER TABLE transformacion_salida0 ADD estado BIGINT NOT NULL DEFAULT 0;");
        conector3.QueryMod("ALTER TABLE transformacion_entrada0 ADD estado BIGINT NOT NULL DEFAULT 0;");

        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada_items0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE estado0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE traslado_items0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida_items0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_salida0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_entrada0 CHANGE serial szSerial VARCHAR(255) DEFAULT '';", "0"));

        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada_items0 ADD costo DECIMAL DEFAULT 0;", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE estado0 ADD costo DECIMAL DEFAULT 0;", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE traslado_items0 ADD costo DECIMAL DEFAULT 0;", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida_items0 ADD costo DECIMAL DEFAULT 0;", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_salida0 ADD costo DECIMAL DEFAULT 0;", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE transformacion_entrada0 ADD costo DECIMAL DEFAULT 0;", "0"));

        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada0 ADD tercero BIGINT UNSIGNED;", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE entrada0 ADD FOREIGN KEY (tercero) REFERENCES tercero(id);", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida0 ADD tercero BIGINT UNSIGNED;", "0"));
        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE salida0 ADD FOREIGN KEY (tercero) REFERENCES tercero(id);", "0"));

        conector3.QueryMod("RENAME TABLE estado0 TO estado0_temp;");
        conector3.QueryMod("CREATE TABLE estado0 ( producto BIGINT UNSIGNED NOT NULL, cantidad DOUBLE UNSIGNED, lote VARCHAR(255), vence DATE, szSerial VARCHAR(255) NOT NULL DEFAULT '', ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0, estado BIGINT NOT NULL DEFAULT 0, modificacion DATETIME, FOREIGN KEY (producto) REFERENCES producto(id),PRIMARY KEY(producto,lote,vence,szSerial, ubicacion,estado)) ENGINE=InnoDB;");
        conector3.QueryMod("INSERT INTO estado0 SELECT producto,cantidad,lote,vence,szSerial,ubicacion,estado,modificacion FROM estado0_temp;");
        conector3.QueryMod("DROP TABLE estado0_temp;");

        conector3.QueryMod(Almacen.ReplaceAlmacenTables("ALTER TABLE estado0 ADD cantidadacumulada DOUBLE DEFAULT 0;", reader["id"].ToString()));

        conector3.QueryMod("DROP TRIGGER bi_entrada_items0;");
        conector3.QueryMod("DROP TRIGGER bu_entrada_items0;");
        conector3.QueryMod("DROP TRIGGER bd_entrada_items0;");
        conector3.QueryMod("DROP TRIGGER bi_traslado_items0;");
        conector3.QueryMod("DROP TRIGGER bu_traslado_items0;");
        conector3.QueryMod("DROP TRIGGER bd_traslado_items0;");
        conector3.QueryMod("DROP TRIGGER bi_salida_items0;");
        conector3.QueryMod("DROP TRIGGER bu_salida_items0;");
        conector3.QueryMod("DROP TRIGGER bd_salida_items0;");
        conector3.QueryMod("DROP TRIGGER bi_transformacion_salida0;");
        conector3.QueryMod("DROP TRIGGER bu_transformacion_salida0;");
        conector3.QueryMod("DROP TRIGGER bd_transformacion_salida0;");
        conector3.QueryMod("DROP TRIGGER bi_transformacion_entrada0;");
        conector3.QueryMod("DROP TRIGGER bu_transformacion_entrada0;");
        conector3.QueryMod("DROP TRIGGER bd_transformacion_entrada0;");

        HId.Value = "0";

        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_entrada_items0 BEFORE INSERT ON entrada_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;	IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	    INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion,estado, costo, cantidadacumulada, modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial, NEW.ubicacion, NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_entrada_items0 BEFORE UPDATE ON entrada_items0 FOR EACH ROW BEGIN 	IF NEW.lote IS NULL THEN 		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;	IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR NEW.estado!=OLD.estado THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;			INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion, estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad,NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad,szSerial,ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_entrada_items0 BEFORE DELETE ON entrada_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN      UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_traslado_items0 BEFORE INSERT ON traslado_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_traslado_items0 BEFORE UPDATE ON traslado_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_traslado_items0 BEFORE DELETE ON traslado_items0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_salida_items0 BEFORE INSERT ON salida_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_salida_items0 BEFORE UPDATE ON salida_items0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR  OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_salida_items0 BEFORE DELETE ON salida_items0 FOR EACH ROW BEGIN 	IF OLD.ingreso IS NOT NULL THEN 		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado; 	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_transformacion_salida0 BEFORE INSERT ON transformacion_salida0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_transformacion_salida0 BEFORE UPDATE ON transformacion_salida0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad+OLD.cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad-NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_transformacion_salida0 BEFORE DELETE ON transformacion_salida0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bi_transformacion_entrada0 BEFORE INSERT ON transformacion_entrada0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF NEW.ingreso IS NOT NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;	    INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial,ubicacion,estado,costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial, NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bu_transformacion_entrada0 BEFORE UPDATE ON transformacion_entrada0 FOR EACH ROW BEGIN 	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.lote IS NULL THEN		SET NEW.lote='';	END IF;	IF NEW.estado IS NULL THEN		SET NEW.estado=0;	END IF;	IF NEW.ubicacion IS NULL THEN		SET NEW.ubicacion=0;	END IF;	IF NEW.szSerial IS NULL THEN		SET NEW.szSerial='';	END IF;	IF NEW.vence IS NULL THEN		SET NEW.vence='0000-00-00';	END IF;		IF OLD.ingreso IS NOT NULL THEN		IF NEW.producto!=OLD.producto OR NEW.lote!=OLD.lote OR NEW.vence!=OLD.vence OR NEW.szSerial!=OLD.szSerial OR NEW.ubicacion!=OLD.ubicacion OR OLD.estado!=NEW.estado THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;			UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;			INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad, szSerial, ubicacion,estado, costo, cantidadacumulada,modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad,NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());		ELSEIF NEW.cantidad!=OLD.cantidad THEN			UPDATE estado0 SET cantidad=cantidad-OLD.cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;		END IF;	ELSEIF NEW.ingreso IS NOT NULL AND OLD.ingreso IS NULL THEN		UPDATE estado0 SET cantidad=cantidad+NEW.cantidad,modificacion=SYSDATE() WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND ubicacion=NEW.ubicacion AND estado=NEW.estado;		INSERT IGNORE INTO estado0 (producto, lote, vence, cantidad,szSerial, ubicacion,estado, costo, cantidadacumulada, modificacion) VALUES (NEW.producto, NEW.lote, NEW.vence, NEW.cantidad, NEW.szSerial,NEW.ubicacion,NEW.estado, NEW.costo, NEW.cantidad, SYSDATE());	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER bd_transformacion_entrada0 BEFORE DELETE ON transformacion_entrada0 FOR EACH ROW BEGIN	IF OLD.ingreso IS NOT NULL THEN      UPDATE estado0 SET cantidad=cantidad-OLD.cantidad,modificacion=SYSDATE() WHERE producto=OLD.producto AND lote=OLD.lote AND vence=OLD.vence AND szSerial=OLD.szSerial AND ubicacion=OLD.ubicacion AND estado=OLD.estado;	END IF; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER ai_estado0 AFTER INSERT ON estado0 FOR EACH ROW BEGIN 	UPDATE estado0 SET costo=(SELECT SUM(cantidad*costo)/SUM(cantidad) FROM entrada_items0 WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado AND ingreso IS NOT NULL) WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado; END;", HId.Value)) == -1)
            return;
        if (conector3.QueryMod(Almacen.ReplaceAlmacenTables("CREATE TRIGGER au_estado0 AFTER UPDATE ON estado0 FOR EACH ROW BEGIN 	UPDATE estado0 SET costo=(SELECT SUM(cantidad*costo)/SUM(cantidad) FROM entrada_items0 WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado AND ingreso IS NOT NULL) WHERE producto=NEW.producto AND lote=NEW.lote AND vence=NEW.vence AND szSerial=NEW.szSerial AND estado=NEW.estado; END;", HId.Value)) == -1)
            return;

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
