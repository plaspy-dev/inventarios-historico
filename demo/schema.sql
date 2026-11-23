CREATE TABLE IF NOT EXISTS usuarios (
  id bigint(20) NOT NULL auto_increment,
  nombre varchar(255),
  usuario varchar(255),
  contrasena varchar(255),
  grupo bigint,
  estado bigint,
  almacenes TEXT,
  ingreso DATETIME,
  PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE `modificaciones` (
  `id` bigint(20) unsigned NOT NULL,
  id2 bigint unsigned,
  `fecha` datetime DEFAULT NULL,
  `url` varchar(255) DEFAULT NULL,
  `usuario` bigint(20) unsigned NOT NULL,
  KEY `name` (`id`)
) ENGINE=InnoDB;
CREATE TABLE `permisos` (
  `grupo` bigint(20) unsigned NOT NULL,
  `url` varchar(255) NOT NULL,
  `permiso` int(11) DEFAULT NULL,
  PRIMARY KEY (`grupo`,`url`)
) ENGINE=InnoDB;
CREATE TABLE `varios` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) NOT NULL,
  `descripcion` varchar(1024) DEFAULT NULL,
  `tipo` int(11) DEFAULT NULL,
  `szReservado` varchar(1024) DEFAULT NULL,
  `parametro1` bigint(20) unsigned DEFAULT NULL,
  estado INT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB;
CREATE TABLE `grupo` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) NOT NULL,
  `descripcion` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB;
CREATE TABLE tercero (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) NOT NULL,
  identificacion VARCHAR(255),
  `descripcion` varchar(1024) DEFAULT NULL,
  tipo INT,
  estado INT,
  telefono VARCHAR(255),
  direccion VARCHAR(255),
  email VARCHAR(255),
  PRIMARY KEY (`id`)
)ENGINE=InnoDB;
CREATE TABLE `producto` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) NOT NULL,
  `descripcion` varchar(1024) DEFAULT NULL,
  codigo VARCHAR(255),
  unidad VARCHAR(255),
  referencia VARCHAR(255),
  empresa VARCHAR(255),
  estado INT,
  nSerial INT,
  serial INT,
  diasvencimiento INT,
  vence INT,
  lote INT,
  entero INT,
  nEstado INT DEFAULT 0,
  costo DECIMAL(60,15),
  grupo BIGINT UNSIGNED DEFAULT 0,
  linea BIGINT UNSIGNED DEFAULT 0,
  formatolote TEXT,
  dias_alerta INT,
  maximo DOUBLE,
  minimo DOUBLE,
  maxmin_tipo INT,
  valor DECIMAL(60,15),
  PRIMARY KEY (`id`)
) ENGINE=InnoDB;
CREATE TABLE `almacen` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) NOT NULL,
  `descripcion` varchar(1024) DEFAULT NULL,
  estado INT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB;
CREATE TABLE maximo_minimo (
	producto BIGINT UNSIGNED NOT NULL,
	almacen BIGINT UNSIGNED NOT NULL,
	maximo DOUBLE,
	minimo DOUBLE,
	FOREIGN KEY (producto) REFERENCES producto(id),
	FOREIGN KEY (almacen) REFERENCES almacen(id),
	PRIMARY KEY(producto,almacen)
)ENGINE=InnoDB;
CREATE TABLE ubicacion (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  almacen BIGINT UNSIGNED NOT NULL,
  nombre VARCHAR(255) NOT NULL,
  estado INT,
  FOREIGN KEY (almacen) REFERENCES almacen(id),
  PRIMARY KEY(id)
)ENGINE=InnoDB;
CREATE TABLE transformacion (
	producto1 BIGINT UNSIGNED NOT NULL,
	producto2 BIGINT UNSIGNED NOT NULL,
	proporcion DOUBLE,
	FOREIGN KEY (producto1) REFERENCES producto(id),
	FOREIGN KEY (producto2) REFERENCES producto(id),
	PRIMARY KEY(producto1,producto2)
) ENGINE=InnoDB;
CREATE TABLE costo_proveedor (
	producto BIGINT UNSIGNED NOT NULL,
	proveedor BIGINT UNSIGNED NOT NULL,
	costo DECIMAL(60,15),
	fecha DATETIME,
	FOREIGN KEY (producto) REFERENCES producto(id),
	FOREIGN KEY (proveedor) REFERENCES tercero(id),
	PRIMARY KEY(producto,proveedor)
)ENGINE=InnoDB;
CREATE TABLE valor_cliente (
	producto BIGINT UNSIGNED NOT NULL,
	cliente BIGINT UNSIGNED NOT NULL,
	valor DECIMAL(60,15),
	FOREIGN KEY (producto) REFERENCES producto(id),
	FOREIGN KEY (cliente) REFERENCES tercero(id),
	PRIMARY KEY(producto,cliente)
)ENGINE=InnoDB;
CREATE TABLE producto_proveedor (
	producto BIGINT UNSIGNED NOT NULL,
	proveedor BIGINT UNSIGNED NOT NULL,
	codigo VARCHAR(255),
	FOREIGN KEY (producto) REFERENCES producto(id),
	FOREIGN KEY (proveedor) REFERENCES tercero(id),
	PRIMARY KEY(producto,proveedor)
)ENGINE=InnoDB;
CREATE TABLE valor_almacen (
	producto BIGINT UNSIGNED NOT NULL,
	almacen BIGINT UNSIGNED NOT NULL,
	valor DECIMAL(60,15),
	FOREIGN KEY (producto) REFERENCES producto(id),
	FOREIGN KEY (almacen) REFERENCES almacen(id),
	PRIMARY KEY(producto,almacen)
)ENGINE=InnoDB;
CREATE TABLE estado_global (
	producto BIGINT UNSIGNED NOT NULL,
	estado BIGINT UNSIGNED,
	cantidad DOUBLE UNSIGNED,
	modificacion DATETIME,
	FOREIGN KEY (producto) REFERENCES producto(id),
	PRIMARY KEY(producto,estado)
) ENGINE=InnoDB;
CREATE TABLE alertas (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	nombre VARCHAR(255) NOT NULL,
	consulta TEXT,
	url TEXT,
	consulta_contar TEXT,
	global INT,
	PRIMARY KEY(id)
)ENGINE=InnoDB;
CREATE TABLE alertas_grupo (
  alerta BIGINT UNSIGNED NOT NULL,
  grupo BIGINT UNSIGNED NOT NULL,
  FOREIGN KEY (alerta) REFERENCES alertas(id),
	FOREIGN KEY (grupo) REFERENCES grupo(id),
  PRIMARY KEY(alerta, grupo)
)ENGINE=InnoDB;
CREATE TABLE alertas_usuario (
  alerta BIGINT UNSIGNED NOT NULL,
  usuario BIGINT UNSIGNED NOT NULL,
  estado INT,
  FOREIGN KEY (alerta) REFERENCES alertas(id),
  PRIMARY KEY(alerta, usuario)
)ENGINE=InnoDB;
CREATE TABLE entrada0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	fecha DATE,	
	observaciones TEXT,
	desde BIGINT UNSIGNED,
	traslado BIGINT UNSIGNED,
	estado INT,
	documento VARCHAR(255), 
	tercero BIGINT UNSIGNED,
	FOREIGN KEY (tercero) REFERENCES tercero(id),
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE entrada_items0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	entrada BIGINT UNSIGNED NOT NULL,
	producto BIGINT UNSIGNED NOT NULL,
	cantidad DOUBLE,
	lote VARCHAR(255),
	szSerial VARCHAR(255) DEFAULT '',
	ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,
	estado BIGINT NOT NULL DEFAULT 0,
	vence DATE,
	costo DECIMAL(60,15) DEFAULT 0,
	proveedor BIGINT UNSIGNED DEFAULT 0,
	ingreso DATETIME,
	traslado_items BIGINT UNSIGNED,
	FOREIGN KEY (entrada) REFERENCES entrada0(id),
	FOREIGN KEY (producto) REFERENCES producto(id),
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE estado0 (
	producto BIGINT UNSIGNED NOT NULL,
	cantidad DOUBLE UNSIGNED,
	lote VARCHAR(255),
	vence DATE,
	szSerial VARCHAR(255) NOT NULL DEFAULT '',
	ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,
	estado BIGINT NOT NULL DEFAULT 0,
	costo DECIMAL(60,15) DEFAULT 0,
	proveedor BIGINT UNSIGNED DEFAULT 0,
	costoacumulado DECIMAL(60,15) DEFAULT 0,
	modificacion DATETIME,
	FOREIGN KEY (producto) REFERENCES producto(id),
	PRIMARY KEY(producto,lote,vence,szSerial, ubicacion, estado, proveedor)
) ENGINE=InnoDB;
CREATE TABLE traslado0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	envio DATE,	
	llegada DATE,
	observaciones TEXT,
	destino BIGINT UNSIGNED,
	entrada BIGINT UNSIGNED,
	estado INT,
	documento VARCHAR(255), 
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE traslado_items0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	traslado BIGINT UNSIGNED NOT NULL,
	producto BIGINT UNSIGNED NOT NULL,
	cantidad DOUBLE,
	lote VARCHAR(255),
	vence DATE,
	szSerial VARCHAR(255) DEFAULT '',
	ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,
	estado BIGINT NOT NULL DEFAULT 0,
	ingreso DATETIME,
	costo DECIMAL(60,15) DEFAULT 0,
	proveedor BIGINT UNSIGNED DEFAULT 0,
	entrada_items BIGINT UNSIGNED,
	FOREIGN KEY (traslado) REFERENCES traslado0(id),
	FOREIGN KEY (producto) REFERENCES producto(id),
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE salida0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	fecha DATE,
	entrega DATE,
	observaciones TEXT,
	estado INT,
	documento VARCHAR(255), 
	tercero BIGINT UNSIGNED,
	FOREIGN KEY (tercero) REFERENCES tercero(id),
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE salida_items0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	salida BIGINT UNSIGNED NOT NULL,
	producto BIGINT UNSIGNED NOT NULL,
	cantidad DOUBLE,
	lote VARCHAR(255),
	vence DATE,
	szSerial VARCHAR(255) DEFAULT '',
	ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,
	estado BIGINT NOT NULL DEFAULT 0,
	costo DECIMAL(60,15) DEFAULT 0,
	proveedor BIGINT UNSIGNED DEFAULT 0,
	valor DECIMAL(60,15) DEFAULT 0,
	ingreso DATETIME,
	FOREIGN KEY (salida) REFERENCES salida0(id),
	FOREIGN KEY (producto) REFERENCES producto(id),
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE transformacion_salida0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	producto BIGINT UNSIGNED NOT NULL,
	cantidad DOUBLE,
	lote VARCHAR(255),
	vence DATE,
	szSerial VARCHAR(255) DEFAULT '',
	ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,
	estado BIGINT NOT NULL DEFAULT 0,
	ingreso DATETIME,
	costo DECIMAL(60,15) DEFAULT 0,
	proveedor BIGINT UNSIGNED DEFAULT 0,
	observaciones TEXT,
	FOREIGN KEY (producto) REFERENCES producto(id),
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE transformacion_entrada0 (
	id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
	producto BIGINT UNSIGNED NOT NULL,
	cantidad DOUBLE,
	lote VARCHAR(255),
	vence DATE,
	szSerial VARCHAR(255) DEFAULT '',
	ubicacion BIGINT UNSIGNED NOT NULL DEFAULT 0,
	estado BIGINT NOT NULL DEFAULT 0,
	ingreso DATETIME,
	costo DECIMAL(60,15) DEFAULT 0,
	proveedor BIGINT UNSIGNED DEFAULT 0,
	FOREIGN KEY (producto) REFERENCES producto(id),
	PRIMARY KEY(id)
) ENGINE=InnoDB;
CREATE TABLE configuracion (
  configuracion LONGTEXT NOT NULL
) ENGINE=InnoDB;
