# Gestión de inventarios histórica / Historical inventory management

[Español](#español) · [English](#english)

## Español

Aplicación web para saber qué artículos hay en cada almacén y de dónde viene cada cambio de existencias. Registra las entradas, las salidas y los traslados.

### Qué puedes hacer

- Administra fichas de productos y datos de almacenes.
- Registra la entrada y salida de artículos con formularios separados.
- Registra traslados entre almacenes y consulta los formatos asociados.

### Ejemplo

Prueba el recorrido documentado en `demos/ejemplo.json`. Entran diez herramientas al primer almacén y se trasladan dos al segundo. El ejemplo usa datos ficticios.

### Cómo empezar

El sitio usa ASP.NET de su época. Revisa las páginas y su código en un entorno Windows compatible con .NET Framework. Para una ejecución local, identifica las conexiones que usa la aplicación y prepara una base vacía con datos de prueba. Las conexiones, recursos y servicios originales se gestionan por separado.

Puntos de entrada en el código. [`inventarios.sln`](inventarios.sln).

Las fechas del historial se apoyan en Git o en la evidencia disponible de los respaldos. Las fechas reconstruidas a partir de archivos sin zona horaria usan las 12:00 UTC como referencia.

## English

A web application for looking up stock at each warehouse and tracing where changes came from. It records receipts, issues, and transfers.

### What you can do

- Manage product records and warehouse details.
- Record goods coming in and going out using separate forms.
- Record warehouse transfers and review the associated forms.

### Example

Follow the walkthrough in `demos/ejemplo.json`. Ten tools come into the first warehouse, then two move to the second. The example uses fictional data.

### Getting started

The site uses historical ASP.NET. Review its pages and code in a Windows environment compatible with .NET Framework. To run locally, identify the connections the application uses and prepare an empty database with test data. Original connections, resources, and services are managed separately.

Entry points in the source. [`inventarios.sln`](inventarios.sln).

History dates come from Git or available backup evidence. Reconstructed dates from files with no time zone use 12:00 UTC as a reference.
