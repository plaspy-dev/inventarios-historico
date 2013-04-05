# Gestión de inventarios histórica / Historical inventory management

[English](#english) · [Español](#español)

## English

Archived Web Forms site for recording stock receipts, issues and transfers across warehouses

> [!WARNING]
> **Status: Archived.** The website compiles today with the documented Windows toolchain.

This 2013-04-05 source snapshot tracks stock and movements separately for each warehouse.

### Overview

Workshop and warehouse staff use the inventory screens to track items and current stock. This dated edition describes the screens and local database setup below.

### Features

- Open stock transformations in `almacen/transformaciones.aspx`.
- Sign in with a local inventory account through `inventarios/Default.aspx`.
- Record incoming stock at `inventarios/almacen/entrada.aspx`.
- Record stock withdrawals at `inventarios/almacen/salida.aspx`.
- Move stock between warehouse locations at `inventarios/almacen/traslado.aspx`.
- Maintain product records at `inventarios/administrador/productos.aspx`.
- Configure the inventory options at `inventarios/administrador/configuracion.aspx`.
- Maintain business contacts at `inventarios/administrador/terceros.aspx`.

### Build

Install x64 Windows with .NET Framework 4.8 ASP.NET, its Developer Pack, .NET SDK 11.0.100, and Python 3.12.4. The command builds the own library and precompiles the pages in a temporary copy. Choose a new output directory.

```powershell
python demo/compilar.py --output "$env:TEMP/inventarios-compilado"
```

To run the site, host it in IIS Express and configure an isolated MySQL database through `MySql.Data.MySqlClient`. The build template omits deployment connections.

See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for the notices and SHA-256 values of recovered dependencies.

### Requirements and dependencies

The archived installation SQL in `inventarios/db/` and the deployed configuration contain installation accounts and settings. The reconstruction leaves those files in the source archive. The `demo/` directory supplies a disposable database schema and fictional records. The C# generated from the archived assembly in 2026 builds the matching component library.

The 2026 reconstruction decompiled the period SDG.Componentes assembly into src/SDG.Componentes/. The build compiles these reconstructed C# sources. The archived DES helper uses fictitious material.

MySQL Connector/NET 6.3.5.0 is included. 

### Project structure

The recovery includes the archived menu definition `inventarios/directorio.xml` dated 2013-04-05, and 22 interface graphics from the same source.

`inventarios/` has the pages and stock workflows. `src/SDG.Componentes/` builds the C# reconstruction of the period assembly. `demo/` contains the build script and Web Forms configuration template.

### Usage and limits

Generate hexadecimal `INVENTARIO_DEMO_ROOT` and `INVENTARIO_DEMO_LOGIN` in the current shell. Run `docker compose -f demo/compose.yaml up -d`. From this checkout, run `python demo/preparar-sitio-local.py <new-folder>` and open the generated `inventarios/` folder in IIS Express. The script only changes the settings in that disposable copy. Sign in as `alicia_demo` with the generated login password and confirm that Inicio shows Deposito Norte. Remove the service after the test with `docker compose -f demo/compose.yaml down --volumes`.

Open the local site and sign in with the generated account. The verification records the protected home page and fictional data service.

### Verification

The later 2026 reconstruction trial runs the exact revision with ASP.NET Compiler 4.8.10001.0 and a disposable MySQL server. The local test covers sign-in and the protected home page. Results for other workflows depend on the files and permissions of each edition.

### History

This source snapshot comes from a dated RAR archive. The original date is 2013-04-05. The 2026 reconstruction supplies the build, bilingual guide and local database fixture.

### Keys for IIS

Generate an eight-character ASCII key for each setting and set it in the IIS worker process. Each installation uses its own keys.

- `INVENTORY_PRODUCT_KEY` encodes and reads the product entries.

### License

The application uses [MIT](LICENSE).

### Support

Include steps to reproduce the problem when reporting it.

### Contributing

Use fictional records in examples and screenshots.

## Español

Archivado — Sitio Web Forms para registrar entradas, salidas y traslados de existencias entre almacenes

> [!WARNING]
> **Estado: archivado.** El sitio compila hoy con las herramientas de Windows indicadas.

Este respaldo de fuentes del 2013-04-05 controla existencias y movimientos por almacén.

### Descripción general

El personal de taller y almacén usa las pantallas de inventario para seguir los artículos y las existencias. Esta edición fechada describe sus pantallas y la preparación de la base local.

### Funciones

- Abre las transformaciones de existencias en `almacen/transformaciones.aspx`.
- Inicia sesión con una cuenta local de inventarios desde `inventarios/Default.aspx`.
- Registra entradas de existencias en `inventarios/almacen/entrada.aspx`.
- Registra salidas de existencias en `inventarios/almacen/salida.aspx`.
- Registra traslados entre ubicaciones en `inventarios/almacen/traslado.aspx`.
- Mantén las fichas de productos en `inventarios/administrador/productos.aspx`.
- Configura las opciones de inventario en `inventarios/administrador/configuracion.aspx`.
- Mantén los contactos comerciales en `inventarios/administrador/terceros.aspx`.

### Cómo compilar

Necesitas Windows x64 con ASP.NET de .NET Framework 4.8, su Developer Pack, el SDK .NET 11.0.100 y Python 3.12.4. El comando recompila la biblioteca propia y precompila las páginas desde una copia temporal. Usa una carpeta de salida nueva.

```powershell
python demo/compilar.py --output "$env:TEMP/inventarios-compilado"
```

Para ejecutar el sitio, hospédalo en IIS Express y configura una base MySQL aislada con el proveedor `MySql.Data.MySqlClient`. La plantilla de compilación deja fuera las conexiones de despliegue.

Los avisos y SHA-256 de las dependencias recuperadas están en [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

### Requisitos y dependencias

El SQL de instalación archivado en `inventarios/db/` y la configuración desplegada contienen cuentas y ajustes de instalación. La reconstrucción conserva esos archivos en el respaldo de fuentes. `demo/` trae un esquema desechable y registros ficticios. El C# generado a partir del ensamblado archivado en 2026 compila la biblioteca correspondiente.

La reconstrucción de 2026 descompiló el ensamblado de época SDG.Componentes en src/SDG.Componentes/. La compilación usa esos fuentes C# reconstruidos. El auxiliar DES archivado usa material ficticio.

El árbol incluye MySQL Connector/NET 6.3.5.0. 

### Estructura

La recuperación incluye la definición del menú archivado `inventarios/directorio.xml` del 2013-04-05 y 22 gráficos de interfaz del mismo respaldo.

`inventarios/` contiene las páginas y los movimientos. `src/SDG.Componentes/` compila la reconstrucción C# del ensamblado de época. `demo/` contiene el lanzador de compilación y la plantilla Web Forms.

### Uso y alcance

Genera `INVENTARIO_DEMO_ROOT` e `INVENTARIO_DEMO_LOGIN` en hexadecimal para la sesión actual. Ejecuta `docker compose -f demo/compose.yaml up -d`. Desde esta copia, ejecuta `python demo/preparar-sitio-local.py <carpeta-nueva>` y abre la carpeta `inventarios/` generada en IIS Express. El script configura únicamente esa copia desechable. Ingresa como `alicia_demo` con la clave generada y comprueba que Inicio muestra Deposito Norte. Retira el servicio con `docker compose -f demo/compose.yaml down --volumes` al terminar.

Abre el sitio local e inicia sesión con la cuenta generada. La verificación registra la portada protegida y el servicio de datos ficticios.

### Verificación

En la prueba posterior de reconstrucción de 2026 se usa esta revisión exacta con ASP.NET Compiler 4.8.10001.0 y MySQL desechable. La prueba local cubre el ingreso y la portada protegida. El alcance de otros recorridos depende de los archivos y permisos de cada edición.

### Historial

Este corte proviene de un respaldo RAR con fecha original del 2013-04-05. La reconstrucción de 2026 aporta compilación, guía bilingüe y base de datos local ficticia.

### Claves de IIS

Genera una clave ASCII de ocho caracteres para cada variable y defínela en el proceso de IIS. Cada instalación usa sus propias claves.

- `INVENTORY_PRODUCT_KEY` cifra y lee las entradas de producto.

### Licencia

La aplicación usa [MIT](LICENSE).

### Soporte

Al informar de un problema, incluye los pasos para reproducirlo.

### Contribuciones

Usa registros ficticios en ejemplos y capturas.
