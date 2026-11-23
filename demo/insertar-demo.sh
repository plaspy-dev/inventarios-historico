#!/bin/sh
case "$MYSQL_PASSWORD" in
    ''|*[!a-f0-9]*) echo 'Expected a hexadecimal local test password' >&2; exit 1 ;;
esac
MYSQL_PWD="$MYSQL_ROOT_PASSWORD" mysql -uroot "$MYSQL_DATABASE" <<DEMO
INSERT INTO grupo(id,nombre,descripcion) VALUES(501,'Operadores demo','Grupo ficticio');
INSERT INTO usuarios(id,nombre,usuario,contrasena,grupo,estado,almacenes)
VALUES(501,'Alicia Ejemplo','alicia_demo','$MYSQL_PASSWORD',501,NULL,'1');
INSERT INTO almacen(id,nombre,descripcion,estado) VALUES(1,'Deposito Norte','Lugar ficticio',NULL);
INSERT INTO producto(id,nombre,descripcion,codigo,unidad,estado)
VALUES(501,'Llave de ensayo','Articulo ficticio','H-501','unidad',NULL);
INSERT INTO permisos(grupo,url,permiso) VALUES(501,'/Default.aspx',1),(501,'~/administrador/productos.aspx',1);
DEMO
