<%@ Page Title="Estado" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="estado.aspx.cs" Inherits="almacen_estado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="Stylesheet" type="text/css" href="../css/jquery.multiselect.css" />
    <script type="text/javascript" src="<%=ResolveUrl("~/")%>js/jquery.multiselect.min.js"></script>
    <script type="text/javascript">

        function CheckAlmacenes() {
            if ($("#<%= DAlmacenes.ClientID %>").val() == null || $("#<%= DAlmacenes.ClientID %>").val().toString().indexOf(",") == -1)
                $("#<%= CAlmacen.ClientID %>").parent().hide();
            else $("#<%= CAlmacen.ClientID %>").parent().show();
        }
        addFunctionToStart(function() {
            addDialog("#<%= PUbicacion.ClientID %>", "#dUbicacion");
            addDialog("#<%= PEstado.ClientID %>", "#dEstado");

            CheckAlmacenes();
            
            $("#<%= DAlmacenes.ClientID %>").multiselect({
                noneSelectedText: "Seleccione un almacen",
                checkAllText: "Todos",
                uncheckAllText: "Ninguno",
                selectedText: "# seleccionados",
                close: CheckAlmacenes
            });
            $("#<%= TUbicacion.ClientID %>,#<%= TNUbicacion.ClientID %>").autocomplete({
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

            $("#<%= DProductoUbicaciones.ClientID %>").change(function() {
                var cantidad = $(this).val().split(";");
                $("#tmaxima").text(cantidad[4]);
                $("#<%= TCantidad.ClientID %>").val(cantidad[4]);
            }).change();
            $("#<%= DProductoEstado.ClientID %>").change(function() {
                var cantidad = $(this).val().split(";");
                $("#tmaximaestado").text(cantidad[4]);
                $("#<%= TEstadoCantidad.ClientID %>").val(cantidad[4]);
            }).change();

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
        });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Panel runat="server" ID="PUbicacion" Visible="false">
        <div id="dUbicacion" title='<%= ViewState["producto"].ToString() %>' class="hide">
            <table>
                <tr>
                    <td>Actual</td>
                    <td>
                        <asp:DropDownList ID="DProductoUbicaciones" runat="server" DataTextField="nombre" DataValueField="producto">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Máxima</td>
                    <td id="tmaxima" align="right"></td>
                </tr>
                <tr>
                    <td>Cantidad</td>
                    <td><asp:TextBox runat="server" ID="TCantidad" CssClass="float"></asp:TextBox>
                    <asp:RequiredFieldValidator ValidationGroup="ubicacion" ControlToValidate="TCantidad" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Nueva ubicación</td>
                    <td>
                        <asp:TextBox runat="server" ID="TNUbicacion" CssClass="tlote tubicacion"></asp:TextBox>
                        <asp:TextBox runat="server" ID="TNUbicacionID" CssClass="hide"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button runat="server" ValidationGroup="ubicacion" ID="BAceptar" onclick="BAceptar_Click" Text="Aceptar" />
                        <asp:Button runat="server" ID="BCancelar" CausesValidation="false" Text="Cancelar" OnClick="BCancelar_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="PEstado" Visible="false">
        <div id="dEstado" title='<%= ViewState["producto"].ToString() %>' class="hide">
            <table>
                <tr>
                    <td>Actual</td>
                    <td>
                        <asp:DropDownList ID="DProductoEstado" runat="server" DataTextField="nombre" DataValueField="producto">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Máxima</td>
                    <td id="tmaximaestado" align="right"></td>
                </tr>
                <tr>
                    <td>Cantidad</td>
                    <td><asp:TextBox runat="server" ID="TEstadoCantidad" CssClass="float"></asp:TextBox>
                    <asp:RequiredFieldValidator ValidationGroup="estado" ControlToValidate="TEstadoCantidad" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Nuevo estado</td>
                    <td>
                        <asp:DropDownList ID="DNuevoEstado" runat="server" AppendDataBoundItems="true" 
                            DataSourceID="SEstado" DataTextField="nombre" DataValueField="id">
                            <asp:ListItem Text="(Ninguna)" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button runat="server" ValidationGroup="estado" ID="BAceptarEstado" onclick="BAceptarEstado_Click" Text="Aceptar" />
                        <asp:Button runat="server" ID="BCancelarEstado" CausesValidation="false" Text="Cancelar" OnClick="BCancelarEstado_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
<asp:Panel runat="server" ID="PBuscar" DefaultButton="BBuscar">
    <table>
        <tr>
            <td><asp:Label runat="server" ID="LVence" Text="Vence"></asp:Label></td>
            <td>Nombre</td>
            <td><asp:Label runat="server" ID="LUbicacion" Text="Ubicación"></asp:Label></td>
            <td>Grupo</td>
            <td><asp:Label runat="server" ID="LEstado" Text="Estado"></asp:Label></td>
            <td><asp:Label runat="server" ID="LAlmacenes" Text="Almacenes"></asp:Label></td>
            <td><asp:CheckBox runat="server" ID="CAlmacen" Text="Ver almacén" CssClass="hide" /></td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="TVence" runat="server" CssClass="fecha" Text=""></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator1" runat="server" Type="Date" ErrorMessage="Por favor ingrese una fecha valida." Text="*" MinimumValue="01/01/1900" MaximumValue="01/01/3000" SetFocusOnError="True" Display="Dynamic" ControlToValidate="TVence"></asp:RangeValidator>
            </td>
            <td>
                <asp:TextBox runat="server" ID="TProducto"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox>
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
                <asp:ListBox ID="DAlmacenes" runat="server" SelectionMode="Multiple" CssClass="hide">
                </asp:ListBox>
                </td>
            <td nowrap=nowrap>
                <asp:CheckBox ID="CDetallado" runat="server" Text="Detallado" />
            </td>
            <td nowrap=nowrap>
                <asp:CheckBox ID="CTodos" runat="server" Text="Productos sin existencias" />
            </td>
            <td>
                <asp:Button ID="BBuscar" runat="server" Text="Actualizar" onclick="BBuscar_Click" />
            </td>
        </tr>
        </table>
    <br />
    <asp:GridView ID="GResultados" runat="server" DataSourceID="SResultados" 
        AutoGenerateColumns="False" EmptyDataText="No se encontraron productos" 
        AllowPaging="True" PageSize="50" AllowSorting="True" 
        DataKeyNames="producto,lote,vence,ubicacion,estado" 
        onselectedindexchanged="GResultados_SelectedIndexChanged" 
        onrowcommand="GResultados_RowCommand" >
        <RowStyle CssClass="GridRow"></RowStyle>
        <FooterStyle CssClass="GridFooter"></FooterStyle>
        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
        <Columns>
            <asp:HyperLinkField HeaderText="Código" DataNavigateUrlFormatString="~/almacen/estado.aspx?pid={3}&n={0}&l={1}&v={2:d}" DataNavigateUrlFields="nombre,lote,vence,producto" DataTextField="codigo" SortExpression="codigo" ItemStyle-HorizontalAlign="Right" />
            <asp:HyperLinkField HeaderText="Producto" DataNavigateUrlFormatString="~/almacen/estado.aspx?pid={3}&n={0}&l={1}&v={2:d}" DataNavigateUrlFields="nombre,lote,vence,producto" DataTextField="nombre" SortExpression="nombre" />
            <asp:HyperLinkField HeaderText="Empresa" DataNavigateUrlFormatString="~/almacen/estado.aspx?pid={3}&n={0}&l={1}&v={2:d}" DataNavigateUrlFields="nombre,lote,vence,producto" DataTextField="empresa" SortExpression="empresa" />
            <asp:HyperLinkField HeaderText="Descripción" DataNavigateUrlFormatString="~/almacen/estado.aspx?pid={3}&n={0}&l={1}&v={2:d}" DataNavigateUrlFields="nombre,lote,vence,producto" DataTextField="descripcion" SortExpression="descripcion" />
            <asp:BoundField HeaderText="Lote" DataField="lote" SortExpression="lote" />
            <asp:BoundField HeaderText="Vence" DataField="vence" SortExpression="vence" DataFormatString="{0:d}" />
            <asp:BoundField HeaderText="Cantidad" DataField="cantidad" SortExpression="cantidad" ItemStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Costo" DataField="costo" SortExpression="costo" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C}" />
            <asp:BoundField HeaderText="Un." DataField="unidad" SortExpression="unidad" />
            <asp:TemplateField HeaderText="Ubicación" SortExpression="ubicacion">
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="LUbicacion" CommandName="Select" Text='<%# Eval("ubicacion") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Estado" SortExpression="estado">
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="LEstado" CommandName="Estado" Text='<%# Eval("estado") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Almacén" DataField="almacen" SortExpression="almacen" />
        </Columns>
    </asp:GridView>
    <div style="text-align: right;">
        <asp:Label ID="LTotal" runat="server" Text=""></asp:Label>
    </div>
    <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad,producto.nombre,producto.empresa
                        ,producto.unidad,producto.codigo,producto.descripcion
                        ,IFNULL(almacen.nombre,'(Ajustes)') AS almacen
                        ,'' AS lote,'' AS vence,'' AS szSerial,estado{0}.costo AS costo
                        ,IF(COUNT(DISTINCT estado{0}.ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ubicacion
                        ,IF(COUNT(DISTINCT estado{0}.estado)=1,IFNULL(estado.nombre,'(Ninguno)'), '(Multiple)') AS estado
                        FROM estado{0} 
                        LEFT JOIN producto ON (producto.id=estado{0}.producto) 
                        LEFT JOIN ubicacion ON (ubicacion.id=estado{0}.ubicacion)
                        LEFT JOIN varios AS estado ON (estado.id=estado{0}.estado)
                        LEFT JOIN almacen ON (almacen.id={1})
                        WHERE 
                        (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=estado{0}.vence)
                        AND (@todos=1 OR estado{0}.cantidad>0)
                        AND (@ubicacion=-1 OR estado{0}.ubicacion=@ubicacion)
                        AND (@estado=-1 OR estado{0}.estado=@estado)
                        AND (@grupo=-1 OR producto.grupo=@grupo OR (@grupo=0 AND producto IS NULL))
                        AND (@producto=0 OR producto.id=@producto)
                        AND (@producto!=0 OR @nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY producto.id HAVING (@todos=1 OR SUM(estado{0}.cantidad)>0)
                        ORDER BY producto.codigo,producto.nombre">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TVence" PropertyName="Text" Name="vence" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TProducto" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TProductoID" PropertyName="Text" Name="producto" DefaultValue="0" />
                    <asp:ControlParameter ControlID="CTodos" PropertyName="Checked" Name="todos" />
                    <asp:ControlParameter ControlID="TUbicacionID" PropertyName="Text" Name="ubicacion" DefaultValue="-1" />
                    <asp:ControlParameter ControlID="DGrupo" PropertyName="SelectedValue" Name="grupo" />
                    <asp:ControlParameter ControlID="DEstado" PropertyName="SelectedValue" Name="estado" />
                 </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SResultados1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT estado{0}.producto,estado{0}.lote
                        ,IF(estado{0}.vence='0000-00-00',NULL,estado{0}.vence) AS vence
                        ,SUM(estado{0}.cantidad) AS cantidad,producto.nombre,producto.empresa,estado{0}.szSerial,estado{0}.costo AS costo
                        ,producto.unidad,producto.codigo,producto.descripcion                        
                        ,IFNULL(almacen.nombre,'(Ajustes)') AS almacen
                        ,IF(COUNT(DISTINCT estado{0}.ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ubicacion
                        ,IF(COUNT(DISTINCT estado{0}.estado)=1,IFNULL(estado.nombre,'(Ninguno)'), '(Multiple)') AS estado
                        FROM estado{0} 
                        LEFT JOIN producto ON (producto.id=estado{0}.producto) 
                        LEFT JOIN ubicacion ON (ubicacion.id=estado{0}.ubicacion)
                        LEFT JOIN varios AS estado ON (estado.id=estado{0}.estado)
                        LEFT JOIN almacen ON (almacen.id={1})
                        WHERE 
                        (@todos=1 OR estado{0}.cantidad>0)
                        AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=estado{0}.vence)
                        AND (@ubicacion=-1 OR estado{0}.ubicacion=@ubicacion)
                        AND (@estado=-1 OR estado{0}.estado=@estado)
                        AND (@grupo=-1 OR producto.grupo=@grupo OR (@grupo=0 AND producto IS NULL))
                        AND (@producto=0 OR producto.id=@producto)
                        AND (@producto!=0 OR @nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY estado{0}.producto,estado{0}.lote,estado{0}.vence,estado{0}.ubicacion,estado{0}.estado
                        ORDER BY producto.codigo,producto.nombre,estado{0}.lote,estado{0}.vence">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TVence" PropertyName="Text" Name="vence" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TProducto" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TProductoID" PropertyName="Text" Name="producto" DefaultValue="0" />
                    <asp:ControlParameter ControlID="CTodos" PropertyName="Checked" Name="todos" />
                    <asp:ControlParameter ControlID="TUbicacionID" PropertyName="Text" Name="ubicacion" DefaultValue="-1" />
                    <asp:ControlParameter ControlID="DGrupo" PropertyName="SelectedValue" Name="grupo" />
                    <asp:ControlParameter ControlID="DEstado" PropertyName="SelectedValue" Name="estado" />
                 </SelectParameters>
    </asp:SqlDataSource>
    </asp:Panel>
<asp:Panel runat="server" ID="PKardex" DefaultButton="BActualizar">
    <a href="estado.aspx"><< Estado</a><br /><br />
    <table>
        <tr>
            <td>Desde
            <asp:RequiredFieldValidator ControlToValidate="TDesde" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TDesde" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator>
            </td>
            <td><asp:TextBox runat="server" ID="TDesde" CssClass="fecha"></asp:TextBox></td>
            <td>Hasta
            <asp:RequiredFieldValidator ControlToValidate="TDesde" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TDesde" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator2" Display="Dynamic" Type="Date">*</asp:RangeValidator>
            </td>
            <td><asp:TextBox runat="server" ID="THasta" CssClass="fecha"></asp:TextBox></td>
            <td><asp:Button runat="server" ID="BActualizar" Text="Actualizar" 
                    onclick="BActualizar_Click" /></td>
        </tr>
    </table>
    <asp:Label runat="server" ID="LRotacion"></asp:Label><asp:Label runat="server" ID="LDias"></asp:Label>
    <div id="accordion">
        <h3><a href="#">Gráficas</a></h3>
        <div>
            <div id="chart_entradas" style="width: 1000px; height: 300px;"></div>
            <div id="chart_salidas" style="width: 1000px; height: 300px;"></div>
            <div id="chart_saldos" style="width: 1000px; height: 300px;"></div>
            <div runat="server" id="chart_costos" style="width: 1000px; height: 300px;"></div>
        </div>
    </div>
    <script type="text/javascript" src="https://service.example.invalid/jsapi"></script>
    <script type="text/javascript">
        google.load("visualization", "1", { packages: ["corechart"] });
        google.setOnLoadCallback(drawChart);
        function drawChart() {
            var data = google.visualization.arrayToDataTable([['Fecha', 'Costo']<asp:Literal runat="server" ID="LCostos"></asp:Literal>]);
            var options = { title: 'Costos' };
            var chart;
            try {
                chart = new google.visualization.AreaChart(document.getElementById('<%= chart_costos.ClientID %>'));
                chart.draw(data, options);
            } catch (e) {}
            
            data = google.visualization.arrayToDataTable([['Fecha', 'Cantidad']<asp:Literal runat="server" ID="LEntradas"></asp:Literal>]);
            options = { title: 'Entradas' };
            chart = new google.visualization.AreaChart(document.getElementById('chart_entradas'));
            chart.draw(data, options);
            
            data = google.visualization.arrayToDataTable([['Fecha', 'Cantidad']<asp:Literal runat="server" ID="LSalidas"></asp:Literal>]);
            options = { title: 'Salidas' };
            chart = new google.visualization.AreaChart(document.getElementById('chart_salidas'));
            chart.draw(data, options);
            
            data = google.visualization.arrayToDataTable([['Fecha', 'Cantidad']<asp:Literal runat="server" ID="LSaldos"></asp:Literal>]);
            options = { title: 'Saldos' };
            chart = new google.visualization.AreaChart(document.getElementById('chart_saldos'));
            chart.draw(data, options);
            $("#accordion").accordion({
                autoHeight: false,
                navigation: true,
                collapsible: true,
                active: false
            });
        }
    </script>
    <asp:GridView ID="GKardex" runat="server" DataSourceID="SKardex" 
        AutoGenerateColumns="False" EmptyDataText="No se encontraron productos" 
        AllowPaging="True" PageSize="50" AllowSorting="False" ShowFooter="true"
        onrowdatabound="GKardex_RowDataBound" >
        <RowStyle CssClass="GridRow"></RowStyle>
        <FooterStyle CssClass="GridFooter"></FooterStyle>
        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
        <Columns>
            <asp:BoundField HeaderText="Fecha" DataField="fecha" SortExpression="fecha" DataFormatString="{0:d}" />
            <asp:BoundField HeaderText="Concepto" DataField="concepto" SortExpression="concepto" ItemStyle-Wrap="false" />
            <asp:BoundField HeaderText="Documento" DataField="documento" SortExpression="documento" />
            <asp:BoundField HeaderText="Entrada" DataField="entrada" SortExpression="entrada" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Costo U.E." DataField="costo" SortExpression="costo" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C}" ItemStyle-Wrap="false" />
            <asp:BoundField HeaderText="Costo T.E." DataField="costot" SortExpression="costot" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:C}" FooterStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" FooterStyle-Wrap="false" />
            <asp:BoundField HeaderText="Salida" DataField="salida" SortExpression="salida" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Costo U.S." DataField="valor" SortExpression="valor" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" />
            <asp:BoundField HeaderText="Costo T.S." DataField="valor" SortExpression="valor" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" FooterStyle-Wrap="false" />
            <asp:BoundField HeaderText="Saldo" DataField="saldo" SortExpression="saldo" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" FooterStyle-Wrap="false"  />
            <asp:BoundField HeaderText="Costo U." DataField="valor" SortExpression="valor" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" FooterStyle-HorizontalAlign="Right" FooterStyle-Wrap="false" />
            <asp:BoundField HeaderText="Costo T." DataField="valor" SortExpression="valor" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" FooterStyle-HorizontalAlign="Right" FooterStyle-Wrap="false"/>
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SKardex" runat="server" 
        ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
        SelectCommand="(
                    SELECT 1 AS tipo,entrada{0}.fecha,entrada_items{0}.cantidad AS entrada,NULL AS salida,NULL AS saldo,entrada{0}.id AS concepto 
                    ,entrada_items{0}.ingreso,entrada{0}.documento
                    ,entrada_items{0}.costo AS costo
                    ,entrada_items{0}.costo*entrada_items{0}.cantidad AS costot
                    ,NULL AS valor
                    FROM entrada_items{0}
                    LEFT JOIN entrada{0} ON (entrada{0}.id=entrada_items{0}.entrada)
                    WHERE producto=@producto
                    AND DATE(ingreso) BETWEEN STR_TO_DATE(@desde, '%d/%m/%Y') AND STR_TO_DATE(@hasta, '%d/%m/%Y')
                ) UNION ALL (
                    SELECT 2 AS tipo,traslado{0}.envio AS fecha,NULL AS entrada,traslado_items{0}.cantidad AS salida,NULL AS saldo,traslado{0}.id AS concepto  
                    ,traslado_items{0}.ingreso,traslado{0}.documento
                    ,NULL AS costo
                    ,NULL AS costot
                    ,NULL AS valor
                    FROM traslado_items{0}
                    LEFT JOIN traslado{0} ON (traslado{0}.id=traslado_items{0}.traslado)
                    WHERE 
                    producto=@producto
                    AND DATE(ingreso) BETWEEN STR_TO_DATE(@desde, '%d/%m/%Y') AND STR_TO_DATE(@hasta, '%d/%m/%Y')
                ) UNION ALL (
                    SELECT 3 AS tipo,salida{0}.fecha AS fecha,NULL AS entrada,salida_items{0}.cantidad AS salida,NULL AS saldo,salida{0}.id AS concepto  
                    ,salida_items{0}.ingreso,salida{0}.documento
                    ,NULL AS costo
                    ,NULL AS costot
                    ,NULL AS valor
                    FROM salida_items{0}
                    LEFT JOIN salida{0} ON (salida{0}.id=salida_items{0}.salida)
                    WHERE 
                    producto=@producto
                    AND DATE(ingreso) BETWEEN STR_TO_DATE(@desde, '%d/%m/%Y') AND STR_TO_DATE(@hasta, '%d/%m/%Y')
                ) UNION ALL (
                    SELECT 4 AS tipo,ingreso AS fecha,NULL AS entrada,cantidad AS salida,NULL AS saldo,id AS concepto  
                    ,ingreso,'' AS documento
                    ,NULL AS costo
                    ,NULL AS costot
                    ,NULL AS valor
                    FROM transformacion_salida{0}
                    WHERE 
                    producto=@producto
                    AND DATE(ingreso) BETWEEN STR_TO_DATE(@desde, '%d/%m/%Y') AND STR_TO_DATE(@hasta, '%d/%m/%Y')
                ) UNION ALL (
                    SELECT 5 AS tipo,ingreso AS fecha,cantidad AS entrada,NULL AS salida,NULL AS saldo,id AS concepto  
                    ,ingreso,'' AS documento
                    ,costo AS costo
                    ,costo*cantidad AS costot
                    ,NULL AS valor
                    FROM transformacion_entrada{0}
                    WHERE 
                    producto=@producto
                    AND DATE(ingreso) BETWEEN STR_TO_DATE(@desde, '%d/%m/%Y') AND STR_TO_DATE(@hasta, '%d/%m/%Y')
                ) UNION ALL (
                    SELECT -1 AS tipo, NULL AS fecha, NULL AS cantidad, NULL AS salida, NULL AS saldo, NULL AS concepto
                    ,'0000-00-00 00:00:00' AS ingreso,'' AS documento
                    ,NULL AS costo
                    ,NULL AS costot
                    ,NULL AS valor
                )
                ORDER BY ingreso DESC;">
         <SelectParameters>
            <asp:QueryStringParameter Name="producto" QueryStringField="pid" />
            <asp:ControlParameter ControlID="TDesde" Name="desde" PropertyName="Text" />
            <asp:ControlParameter ControlID="THasta" Name="hasta" PropertyName="Text" />
         </SelectParameters>
    </asp:SqlDataSource>
</asp:Panel>
</asp:Content>