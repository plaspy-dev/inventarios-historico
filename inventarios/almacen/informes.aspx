<%@ Page Title="Informes" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="informes.aspx.cs" Inherits="almacen_informes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<link rel="Stylesheet" type="text/css" href="../css/jquery.multiselect.css" />
<script type="text/javascript" src="<%=ResolveUrl("~/")%>js/jquery.multiselect.min.js"></script>
<script type="text/javascript">
    addFunctionToStart(function() {

        $("#<%= DAlmacenes.ClientID %>").multiselect({
            noneSelectedText: "Seleccione un almacen",
            checkAllText: "Todos",
            uncheckAllText: "Ninguno",
            selectedText: "# seleccionados"
        });
        $("#<%= TUbicacion.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetUbicacion",
                    data: "{ 'q': '" + request.term + "', a: '<%= ((ing)Master).GetAlmacen() %>' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.value;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $("#" + $(this).attr("id") + "ID").val(ui.item.item);
                $(this).val(ui.item.value);
                return false;
            },
            search: function(event, ui) { $("#" + $(this).attr("id") + "ID").val(""); }
        }).change(function() { if ($(this).val().length == 0) $("#" + $(this).attr("id") + "ID").val(""); });
        $("#<%= TTercero.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetTercero",
                    data: "{ 'q': '" + request.term + "', tipo: 2 }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.value;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            autoFocus: true,
            minLength: 3,
            search: function() { $("#<%= TTerceroID.ClientID %>").val(""); },
            select: function(event, ui) {

                $("#<%= TTerceroID.ClientID %>").val(ui.item.item);
                $("#<%= TTercero.ClientID %>").val(ui.item.value);
                return false;
            }
        }).change(function() {
            if ($(this).val() == "")
                $("#<%= TTerceroID.ClientID %>").val("");
        });
        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': null }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.ProductoCompleto;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            autoFocus: true,
            minLength: 3,
            search: function() { $("#<%= TProductoID.ClientID %>").val(""); },
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.value);
                return false;
            }
        }).change(function() {
            if ($(this).val() == "")
                $("#<%= TProductoID.ClientID %>").val("");
        });
    })
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel runat="server" ID="PBuscar" DefaultButton="BBuscar">
    <table>
        <tr>
            <td>Ver</td>
            <td><asp:Label runat="server" ID="LAlmacenes" Text="Almacenes"></asp:Label></td>
            <td>Desde</td>
            <td>Hasta</td>
            <td>Tercero</td>
            <td>Producto</td>
            <td>Ubicación</td>
            <td>Grupo</td>
            <td>Estado producto</td>
            <td>Estado documento</td>
            <td></td>
        </tr>
        <tr>
            <td><asp:DropDownList runat="server" ID="DVer">
                <asp:ListItem Text="Entradas" Value="1"></asp:ListItem>
                <asp:ListItem Text="Traslados" Value="2"></asp:ListItem>
                <asp:ListItem Text="Salidas" Value="3"></asp:ListItem>
                <asp:ListItem Text="Transformaciones" Value="4"></asp:ListItem>
            </asp:DropDownList></td>
            <td>
                <asp:ListBox ID="DAlmacenes" runat="server" SelectionMode="Multiple" CssClass="hide">
                </asp:ListBox>
            </td>
            <td>
                <asp:TextBox ID="TDesde" runat="server" CssClass="fecha" Text=""></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator2" runat="server" Type="Date" ErrorMessage="Por favor ingrese una fecha valida." Text="*" MinimumValue="01/01/1900" MaximumValue="01/01/3000" SetFocusOnError="True" Display="Dynamic" ControlToValidate="TDesde"></asp:RangeValidator>
            </td>
            <td>
                <asp:TextBox ID="THasta" runat="server" CssClass="fecha" Text=""></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator1" runat="server" 
                    ControlToValidate="THasta" Display="Dynamic" 
                    ErrorMessage="Por favor ingrese una fecha valida." MaximumValue="01/01/3000" 
                    MinimumValue="01/01/1900" SetFocusOnError="True" Text="*" Type="Date"></asp:RangeValidator>
            </td>
            <td>
                <asp:TextBox runat="server" ID="TTercero" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TTerceroID" CssClass="hide"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox runat="server" ID="TProducto"></asp:TextBox>
                <asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox runat="server" ID="TUbicacion" CssClass="tlote tubicacion"></asp:TextBox>
                <asp:TextBox runat="server" ID="TUbicacionID" CssClass="hide"></asp:TextBox>
            </td>
            <td><asp:DropDownList ID="DGrupo" runat="server" AppendDataBoundItems="true" 
                    DataSourceID="SGrupo" DataTextField="nombre" DataValueField="id">
                    <asp:ListItem Text="(Todos)" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="(Ninguno)" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SGrupo" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT id, nombre FROM varios WHERE tipo=1 AND estado IS NULL ORDER BY nombre">
                </asp:SqlDataSource>
                </td>
                <td>
                <asp:DropDownList ID="DEstado" runat="server" AppendDataBoundItems="true" 
                        DataSourceID="SEstado" DataTextField="nombre" DataValueField="id">
                        <asp:ListItem Text="(Todos)" Value="-1"></asp:ListItem>
                        <asp:ListItem Text="(Ninguno)" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SEstado" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT id, nombre FROM varios WHERE tipo=2 AND estado IS NULL ORDER BY nombre">
                </asp:SqlDataSource>
                </td>
            <td>
                <asp:DropDownList ID="DEstadoDoc" runat="server">
                    <asp:ListItem Text="Activos" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Anulados" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Todos" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="BBuscar" runat="server" Text="Actualizar" onclick="BBuscar_Click" />
            </td>
        </tr>
        </table>
    <br />
    <asp:Panel runat="server" ID="PGraficas" Visible="false">
        <div id="accordion">
            <h3><a href="#">Gráficas</a></h3>
            <div>
                <div id="chart_productos" style="width: 1000px; height: 300px;"></div>
                <div runat="server" id="dtproductos" style="width: 1000px; height: 300px;"></div>
                <div id="chart_cantidad" style="width: 1000px; height: 300px;"></div>
                <div runat="server" id="dtcantidad" style="width: 1000px; height: 300px;"></div>
                <div runat="server" id="chart_costos" style="width: 1000px; height: 300px;"></div>
                <div runat="server" id="dtcostos" style="width: 1000px; height: 300px;"></div>
            </div>
        </div>
        <script type="text/javascript" src="https://service.example.invalid/jsapi"></script>
        <script type="text/javascript">
            google.load("visualization", "1", { packages: ["corechart"] });
            google.setOnLoadCallback(drawChart);
            function drawChart() {
                var data = google.visualization.arrayToDataTable([<asp:Literal runat="server" ID="LCostos"></asp:Literal>]);
                var options = { title: 'Valores' };
                var chart;
                try {
                    chart = new google.visualization.AreaChart(document.getElementById('<%= chart_costos.ClientID %>'));
                    chart.draw(data, options);
                } catch (e) {}
                
                data = google.visualization.arrayToDataTable([<asp:Literal runat="server" ID="LCantidad"></asp:Literal>]);
                options = { title: 'Cantidades' };
                chart = new google.visualization.AreaChart(document.getElementById('chart_cantidad'));
                chart.draw(data, options);
                
                data = google.visualization.arrayToDataTable([<asp:Literal runat="server" ID="LProductos"></asp:Literal>]);
                options = { title: 'Total productos' };
                chart = new google.visualization.AreaChart(document.getElementById('chart_productos'));
                chart.draw(data, options);
                
                try {
                    data = google.visualization.arrayToDataTable([<asp:Literal runat="server" ID="LDTProductos"></asp:Literal>]);
                    options = { title: 'Total productos por almacén' };
                    chart = new google.visualization.AreaChart(document.getElementById('<%= dtproductos.ClientID %>'));
                    chart.draw(data, options);
                } catch (e) {}
                try {
                    data = google.visualization.arrayToDataTable([<asp:Literal runat="server" ID="LDTCantidad"></asp:Literal>]);
                    options = { title: 'Cantidades por almacén' };
                    chart = new google.visualization.AreaChart(document.getElementById('<%= dtcantidad.ClientID %>'));
                    chart.draw(data, options);
                } catch (e) {}
                try {
                    data = google.visualization.arrayToDataTable([<asp:Literal runat="server" ID="LDTCostos"></asp:Literal>]);
                    options = { title: 'Valores por almacén' };
                    chart = new google.visualization.AreaChart(document.getElementById('<%= dtcostos.ClientID %>'));
                    chart.draw(data, options);
                } catch (e) {}
                
                $("#accordion").accordion({
                    autoHeight: false,
                    navigation: true,
                    collapsible: true,
                    active: false
                });
            }
        </script>
    </asp:Panel>
    <asp:GridView ID="GResultados" runat="server" DataSourceID="SResultados1" ShowFooter="true"  Visible="false"
        AutoGenerateColumns="False" EmptyDataText="No se encontraron productos" 
        AllowSorting="True" 
        DataKeyNames="id" onrowdatabound="GResultados_RowDataBound" >
        <RowStyle CssClass="GridRow"></RowStyle>
        <FooterStyle CssClass="GridFooter"></FooterStyle>
        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
        <Columns>
            <asp:BoundField HeaderText="Almacén" DataField="almacen" SortExpression="almacen" />
            <asp:BoundField HeaderText="Número" DataField="id" SortExpression="id" ItemStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Fecha" DataField="fecha" SortExpression="fecha" DataFormatString="{0:d}" />
            <asp:BoundField HeaderText="Desde/Hacia" DataField="desde" SortExpression="desde" />
            <asp:BoundField HeaderText="Documento" DataField="documento" SortExpression="documento" />
            <asp:BoundField HeaderText="Tercero" DataField="nombre" SortExpression="nombre" />
            <asp:BoundField HeaderText="Total productos" DataField="productos" SortExpression="productos" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Cantidad" DataField="total" SortExpression="total" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Costo" DataField="costo" SortExpression="costo" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C}" FooterStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Precio" DataField="valor" SortExpression="valor" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C}" FooterStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Estado" DataField="estado" SortExpression="estado" />
        </Columns>
    </asp:GridView>
    <div style="text-align: right;">
        <asp:Label ID="LTotal" runat="server" Text=""></asp:Label>
    </div>
    <asp:SqlDataSource ID="SResultados1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT entrada{0}.id,entrada{0}.fecha,almacen.nombre AS desde,entrada{0}.documento,tercero.nombre AS nombre
                            ,COUNT(*) AS productos, SUM(entrada_items{0}.cantidad) AS total
                            ,SUM(entrada_items{0}.cantidad*entrada_items{0}.costo) AS costo
                            ,NULL AS valor
                            ,IFNULL(a.nombre,'(Ajustes)') AS almacen
                            ,IF(entrada{0}.estado IS NULL, 'Activo', 'Anulado') AS estado
                            FROM entrada{0}
                            LEFT JOIN tercero ON (tercero.id=entrada{0}.tercero)
                            LEFT JOIN almacen ON (almacen.id=entrada{0}.desde)
                            LEFT JOIN almacen AS a ON (a.id={0})
                            LEFT JOIN entrada_items{0} ON (entrada{0}.id=entrada_items{0}.entrada)
                            LEFT JOIN producto ON (producto.id=entrada_items{0}.producto)
                        WHERE 
                        (@desde=' ' OR STR_TO_DATE(@desde, '%d/%m/%Y')<=entrada{0}.fecha)
                        AND (@hasta=' ' OR STR_TO_DATE(@hasta, '%d/%m/%Y')>=entrada{0}.fecha)
                        AND (@estadodoc=0 OR (@estadodoc=1 AND entrada{0}.estado IS NULL) OR (@estadodoc=2 AND entrada{0}.estado=-1))
                        AND (@tercero=0 OR entrada{0}.tercero=@tercero)
                        AND (@ubicacion=-1 OR entrada_items{0}.ubicacion=@ubicacion)
                        AND (@estado=-1 OR entrada_items{0}.estado=@estado)
                        AND (@grupo=-1 OR producto.grupo=@grupo OR (@grupo=0 AND producto IS NULL))
                        AND (@producto=0 OR producto.id=@producto)
                        AND (@producto!=0 OR @nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY entrada{0}.id
                        ORDER BY id
                        ">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TDesde" PropertyName="Text" Name="desde" DefaultValue=" " />
                    <asp:ControlParameter ControlID="THasta" PropertyName="Text" Name="hasta" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TTerceroID" PropertyName="Text" Name="tercero" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TProducto" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TProductoID" PropertyName="Text" Name="producto" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TUbicacionID" PropertyName="Text" Name="ubicacion" DefaultValue="-1" />
                    <asp:ControlParameter ControlID="DGrupo" PropertyName="SelectedValue" Name="grupo" />
                    <asp:ControlParameter ControlID="DEstado" PropertyName="SelectedValue" Name="estado" />
                    <asp:ControlParameter ControlID="DEstadoDoc" PropertyName="SelectedValue" Name="estadodoc" />
                 </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SResultados2" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT traslado{0}.id,traslado{0}.envio AS fecha,almacen.nombre AS desde,traslado{0}.documento,'' AS nombre
                            ,COUNT(*) AS productos, SUM(traslado_items{0}.cantidad) AS total
                            ,SUM(traslado_items{0}.cantidad*traslado_items{0}.costo) AS costo
                            ,NULL AS valor
                            ,IFNULL(a.nombre,'(Ajustes)') AS almacen
                            ,IF(traslado{0}.estado IS NULL, 'Activo', 'Anulado') AS estado
                            FROM traslado{0}
                            LEFT JOIN almacen ON (almacen.id=traslado{0}.destino)
                            LEFT JOIN traslado_items{0} ON (traslado{0}.id=traslado_items{0}.traslado)
                            LEFT JOIN producto ON (producto.id=traslado_items{0}.producto)
                            LEFT JOIN almacen AS a ON (a.id={0})
                        WHERE 
                        (@desde=' ' OR STR_TO_DATE(@desde, '%d/%m/%Y')<=traslado{0}.envio)
                        AND (@hasta=' ' OR STR_TO_DATE(@hasta, '%d/%m/%Y')>=traslado{0}.envio)
                        AND (@estadodoc=0 OR (@estadodoc=1 AND traslado{0}.estado IS NULL) OR (@estadodoc=2 AND traslado{0}.estado=-1))
                        AND (@ubicacion=-1 OR traslado_items{0}.ubicacion=@ubicacion)
                        AND (@estado=-1 OR traslado_items{0}.estado=@estado)
                        AND (@grupo=-1 OR producto.grupo=@grupo OR (@grupo=0 AND producto IS NULL))
                        AND (@producto=0 OR producto.id=@producto)
                        AND (@producto!=0 OR @nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY traslado{0}.id
                        ORDER BY id
                        ">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TDesde" PropertyName="Text" Name="desde" DefaultValue=" " />
                    <asp:ControlParameter ControlID="THasta" PropertyName="Text" Name="hasta" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TTerceroID" PropertyName="Text" Name="tercero" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TProducto" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TProductoID" PropertyName="Text" Name="producto" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TUbicacionID" PropertyName="Text" Name="ubicacion" DefaultValue="-1" />
                    <asp:ControlParameter ControlID="DGrupo" PropertyName="SelectedValue" Name="grupo" />
                    <asp:ControlParameter ControlID="DEstado" PropertyName="SelectedValue" Name="estado" />
                    <asp:ControlParameter ControlID="DEstadoDoc" PropertyName="SelectedValue" Name="estadodoc" />
                 </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SResultados3" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT salida{0}.id,salida{0}.fecha,NULL AS desde,salida{0}.documento,tercero.nombre AS nombre
                            ,COUNT(*) AS productos, SUM(salida_items{0}.cantidad) AS total
                            ,SUM(salida_items{0}.cantidad*salida_items{0}.costo) AS costo
                            ,SUM(salida_items{0}.cantidad*salida_items{0}.valor) AS valor
                            ,IFNULL(a.nombre,'(Ajustes)') AS almacen
                            ,IF(salida{0}.estado IS NULL, 'Activo', 'Anulado') AS estado
                            FROM salida{0}
                            LEFT JOIN tercero ON (tercero.id=salida{0}.tercero)
                            LEFT JOIN salida_items{0} ON (salida{0}.id=salida_items{0}.salida)
                            LEFT JOIN producto ON (producto.id=salida_items{0}.producto)
                            LEFT JOIN almacen AS a ON (a.id={0})
                        WHERE 
                        (@desde=' ' OR STR_TO_DATE(@desde, '%d/%m/%Y')<=salida{0}.fecha)
                        AND (@hasta=' ' OR STR_TO_DATE(@hasta, '%d/%m/%Y')>=salida{0}.fecha)
                        AND (@estadodoc=0 OR (@estadodoc=1 AND salida{0}.estado IS NULL) OR (@estadodoc=2 AND salida{0}.estado=-1))
                        AND (@tercero=0 OR salida{0}.tercero=@tercero)
                        AND (@ubicacion=-1 OR salida_items{0}.ubicacion=@ubicacion)
                        AND (@estado=-1 OR salida_items{0}.estado=@estado)
                        AND (@grupo=-1 OR producto.grupo=@grupo OR (@grupo=0 AND producto IS NULL))
                        AND (@producto=0 OR producto.id=@producto)
                        AND (@producto!=0 OR @nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY salida{0}.id
                        ORDER BY id
                        ">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TDesde" PropertyName="Text" Name="desde" DefaultValue=" " />
                    <asp:ControlParameter ControlID="THasta" PropertyName="Text" Name="hasta" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TTerceroID" PropertyName="Text" Name="tercero" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TProducto" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TProductoID" PropertyName="Text" Name="producto" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TUbicacionID" PropertyName="Text" Name="ubicacion" DefaultValue="-1" />
                    <asp:ControlParameter ControlID="DGrupo" PropertyName="SelectedValue" Name="grupo" />
                    <asp:ControlParameter ControlID="DEstado" PropertyName="SelectedValue" Name="estado" />
                    <asp:ControlParameter ControlID="DEstadoDoc" PropertyName="SelectedValue" Name="estadodoc" />
                 </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SResultados4" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT transformacion_entrada{0}.id,transformacion_entrada{0}.ingreso AS fecha,NULL AS desde,NULL AS documento,NULL AS nombre
                            ,COUNT(*) AS productos, SUM(transformacion_entrada{0}.cantidad) AS total
                            ,SUM(transformacion_entrada{0}.cantidad*transformacion_entrada{0}.costo) AS costo
                            ,NULL AS valor
                            ,IFNULL(a.nombre,'(Ajustes)') AS almacen
                            ,'Activo' AS estado
                            FROM transformacion_entrada{0}
                            LEFT JOIN producto ON (producto.id=transformacion_entrada{0}.producto)
                            LEFT JOIN almacen AS a ON (a.id={0})
                        WHERE 
                        (@desde=' ' OR STR_TO_DATE(@desde, '%d/%m/%Y')<=transformacion_entrada{0}.ingreso)
                        AND (@hasta=' ' OR STR_TO_DATE(@hasta, '%d/%m/%Y')>=transformacion_entrada{0}.ingreso)
                        AND (@ubicacion=-1 OR transformacion_entrada{0}.ubicacion=@ubicacion)
                        AND (@estado=-1 OR transformacion_entrada{0}.estado=@estado)
                        AND (@grupo=-1 OR producto.grupo=@grupo OR (@grupo=0 AND producto IS NULL))
                        AND (@producto=0 OR producto.id=@producto)
                        AND (@producto!=0 OR @nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY transformacion_entrada{0}.id
                        ORDER BY id
                        ">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TDesde" PropertyName="Text" Name="desde" DefaultValue=" " />
                    <asp:ControlParameter ControlID="THasta" PropertyName="Text" Name="hasta" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TTerceroID" PropertyName="Text" Name="tercero" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TProducto" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TProductoID" PropertyName="Text" Name="producto" DefaultValue="0" />
                    <asp:ControlParameter ControlID="TUbicacionID" PropertyName="Text" Name="ubicacion" DefaultValue="-1" />
                    <asp:ControlParameter ControlID="DGrupo" PropertyName="SelectedValue" Name="grupo" />
                    <asp:ControlParameter ControlID="DEstado" PropertyName="SelectedValue" Name="estado" />
                 </SelectParameters>
    </asp:SqlDataSource>
    </asp:Panel>
</asp:Content>