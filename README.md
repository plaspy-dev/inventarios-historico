# Gestión de inventarios histórica / Historical inventory management

[Español](#español) · [English](#english)

## Español

Aplicación web para saber qué artículos hay en cada almacén y de dónde viene cada cambio de existencias. Registra las entradas, las salidas y los traslados.

### Esta revisión

**2013-04-21 · Explicar los requisitos al cambiar la contraseña.** Añade indicaciones visuales en el formulario de contraseña y ajusta la lógica de usuario y mantenimiento de almacenes. Esto ayuda a reconocer los requisitos mientras se escribe un valor de prueba.

### Cómo explorar el código

El sitio usa ASP.NET de su época. Revisa las páginas y su código en un entorno Windows compatible con .NET Framework. Para una ejecución local, identifica las conexiones que usa la aplicación y prepara una base vacía con datos de prueba. Las conexiones, recursos y servicios originales se gestionan por separado.

- [inventarios/Default.aspx](inventarios/Default.aspx)
- [inventarios/inicio.aspx](inventarios/inicio.aspx)
- [inventarios/administrador/Default.aspx](inventarios/administrador/Default.aspx)
- [inventarios/error/Default.aspx](inventarios/error/Default.aspx)

## English

A web application for looking up stock at each warehouse and tracing where changes came from. It records receipts, issues, and transfers.

### This revision

**2013-04-21 · Explain password requirements during changes.** Add visual guidance to the password form and revise user and warehouse maintenance logic. Users can recognize the requirements as they enter a test value.

### How to explore the source

The site uses historical ASP.NET. Review its pages and code in a Windows environment compatible with .NET Framework. To run locally, identify the connections the application uses and prepare an empty database with test data. Original connections, resources, and services are managed separately.

- [inventarios/Default.aspx](inventarios/Default.aspx)
- [inventarios/inicio.aspx](inventarios/inicio.aspx)
- [inventarios/administrador/Default.aspx](inventarios/administrador/Default.aspx)
- [inventarios/error/Default.aspx](inventarios/error/Default.aspx)
