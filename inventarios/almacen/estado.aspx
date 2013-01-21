<%@ Page Title="Estado" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="estado.aspx.cs" Inherits="almacen_estado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

    <script type="text/javascript">
    addFunctionToStart(function() {
        addDialog("#<%= PUbicacion.ClientID %>", "#dUbicacion");
        $("#<%= DProductoUbicaciones.ClientID %>").change(function() {
            var cantidad = $(this).val().split(";");
            $("#tmaxima").text(cantidad[cantidad.length - 1]);
            $("#<%= TCantidad.ClientID %>").val(cantidad[cantidad.length - 1]);
        }).change();
    });
    $(function() {
        $("#<%= CTodosAlmacenes.ClientID %>").change(function() {
            if ($(this).is(':checked')) {
                $("#<%= CAlmacen.ClientID %>").parent().show();
            } else {
                $("#<%= CAlmacen.ClientID %>").parent().hide();
            }
        }).change();
        if ($("#<%= CTodosAlmacenes.ClientID %>").is(':checked'))
            $('#<%= GResultados.ClientID %> a[href*="estado.aspx"]').click(function() { return false;  });
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
                    <td>Destino</td>
                    <td>
                        <asp:DropDownList ID="DNuevaUbicacion" runat="server" AppendDataBoundItems="true" 
                            DataSourceID="SUbicacion" DataTextField="nombre" DataValueField="id">
                            <asp:ListItem Text="(Ninguna)" Value="0"></asp:ListItem>
                        </asp:DropDownList>
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
<asp:Panel runat="server" ID="PBuscar" DefaultButton="BBuscar">
    <table>
        <tr>
            <td>Vence</td>
            <td>Nombre</td>
            <td>Ubicación</td>
            <td></td>
            <td><asp:CheckBox runat="server" ID="CTodosAlmacenes" Text="Todos los almacenes" /></td>
            <td><asp:CheckBox runat="server" ID="CAlmacen" Text="Ver almacén" CssClass="hide" /></td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="TVence" runat="server" CssClass="fecha" Text=""></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator1" runat="server" Type="Date" ErrorMessage="Por favor ingrese una fecha valida." Text="*" MinimumValue="01/01/1900" MaximumValue="01/01/3000" SetFocusOnError="True" Display="Dynamic" ControlToValidate="TVence"></asp:RangeValidator>
            </td>
            <td>
                <asp:TextBox runat="server" ID="TNombre"></asp:TextBox>
            </td>
            <td>
                <asp:DropDownList ID="DUbicacion" runat="server" AppendDataBoundItems="true" 
                       DataSourceID="SUbicacion" DataTextField="nombre" DataValueField="id">
                       <asp:ListItem Text="(Todas)" Value="-1"></asp:ListItem>
                       <asp:ListItem Text="(Ninguna)" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SUbicacion" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT id, nombre FROM ubicacion WHERE almacen={0} AND estado IS NULL ORDER BY nombre">
                </asp:SqlDataSource>
            </td>
            <td>
                <asp:CheckBox ID="CDetallado" runat="server" Text="Detallado" />
            </td>
            <td>
                <asp:CheckBox ID="CTodos" runat="server" Text="Todos los productos" />
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
        DataKeyNames="producto,lote,vence,ubicacion" 
        onselectedindexchanged="GResultados_SelectedIndexChanged" >
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
            <asp:BoundField HeaderText="Un." DataField="unidad" SortExpression="unidad" />
            <asp:TemplateField HeaderText="Ubicación" SortExpression="ubicacion">
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="LUbicacion" CommandName="Select" Text='<%# Eval("ubicacion") %>'></asp:LinkButton>
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
                        ,producto.unidad,producto.codigo,producto.descripcion,IFNULL(almacen.nombre,'(Ajustes)') AS almacen
                        ,'' AS lote,'' AS vence
                        ,IF(COUNT(DISTINCT estado{0}.ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ubicacion
                        FROM estado{0} 
                        LEFT JOIN producto ON (producto.id=estado{0}.producto) 
                        LEFT JOIN ubicacion ON (ubicacion.id=estado{0}.ubicacion)
                        LEFT JOIN almacen ON (almacen.id={1})
                        WHERE 
                        (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=estado{0}.vence)
                        AND (@ubicacion=-1 OR estado{0}.ubicacion=@ubicacion)
                        AND (@nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY producto.id HAVING (@todos=1 OR SUM(estado{0}.cantidad)>0)
                        ORDER BY producto.nombre">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TVence" PropertyName="Text" Name="vence" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TNombre" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="CTodos" PropertyName="Checked" Name="todos" />
                    <asp:ControlParameter ControlID="DUbicacion" PropertyName="SelectedValue" Name="ubicacion" />
                 </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SResultados1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT estado{0}.producto,estado{0}.lote
                        ,IF(estado{0}.vence='0000-00-00',NULL,estado{0}.vence) AS vence
                        ,estado{0}.cantidad,producto.nombre,producto.empresa
                        ,producto.unidad,producto.codigo,producto.descripcion,IFNULL(almacen.nombre,'(Ajustes)') AS almacen
                        ,IF(COUNT(DISTINCT estado{0}.ubicacion)=1,IFNULL(ubicacion.nombre,'(Ninguna)'), '(Multiple)') AS ubicacion
                        FROM estado{0} 
                        LEFT JOIN producto ON (producto.id=estado{0}.producto) 
                        LEFT JOIN ubicacion ON (ubicacion.id=estado{0}.ubicacion)
                        LEFT JOIN almacen ON (almacen.id={1})
                        WHERE 
                        (@todos=1 OR estado{0}.cantidad>0)
                        AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=estado{0}.vence)
                        AND (@ubicacion=-1 OR estado{0}.ubicacion=@ubicacion)
                        AND (@nombre='_' OR producto.codigo LIKE CONCAT('%', @nombre, '%')
                            OR producto.nombre LIKE CONCAT('%', @nombre, '%')
                            OR producto.empresa LIKE CONCAT('%', @nombre, '%')
                            OR producto.descripcion LIKE CONCAT('%', @nombre, '%'))
                        GROUP BY estado{0}.producto,estado{0}.lote,estado{0}.vence
                        ORDER BY producto.nombre,estado{0}.lote,estado{0}.vence">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TVence" PropertyName="Text" Name="vence" DefaultValue=" " />
                    <asp:ControlParameter ControlID="TNombre" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="CTodos" PropertyName="Checked" Name="todos" />
                    <asp:ControlParameter ControlID="DUbicacion" PropertyName="SelectedValue" Name="ubicacion" />
                 </SelectParameters>
    </asp:SqlDataSource>
    </asp:Panel>
<asp:Panel runat="server" ID="PKardex">
    <a href="estado.aspx"><< Estado</a><br /><br />
    <asp:GridView ID="GKardex" runat="server" DataSourceID="SKardex" 
        AutoGenerateColumns="False" EmptyDataText="No se encontraron productos" 
        AllowPaging="True" PageSize="50" AllowSorting="True" 
        onrowdatabound="GKardex_RowDataBound" >
        <RowStyle CssClass="GridRow"></RowStyle>
        <FooterStyle CssClass="GridFooter"></FooterStyle>
        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
        <Columns>
            <asp:BoundField HeaderText="Fecha" DataField="fecha" SortExpression="fecha" DataFormatString="{0:d}" />
            <asp:BoundField HeaderText="Concepto" DataField="concepto" SortExpression="concepto" />
            <asp:BoundField HeaderText="Entrada" DataField="entrada" SortExpression="entrada" ItemStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Salida" DataField="salida" SortExpression="salida" ItemStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Saldo" DataField="saldo" SortExpression="saldo" ItemStyle-HorizontalAlign="Right" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SKardex" runat="server" 
        ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
        SelectCommand="(
                    SELECT 1 AS tipo,entrada{0}.fecha,entrada_items{0}.cantidad AS entrada,NULL AS salida,NULL AS saldo,entrada{0}.id AS concepto 
                    ,entrada_items{0}.ingreso
                    FROM entrada_items{0}
                    LEFT JOIN entrada{0} ON (entrada{0}.id=entrada_items{0}.entrada)
                    WHERE producto=@producto
                    AND (@lote=' ' OR @lote=entrada_items{0}.lote)
                    AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=entrada_items{0}.vence)
                ) UNION ALL (
                    SELECT 2 AS tipo,traslado{0}.envio AS fecha,NULL AS entrada,traslado_items{0}.cantidad AS salida,NULL AS saldo,traslado{0}.id AS concepto  
                    ,traslado_items{0}.ingreso
                    FROM traslado_items{0}
                    LEFT JOIN traslado{0} ON (traslado{0}.id=traslado_items{0}.traslado)
                    WHERE 
                    producto=@producto
                    AND (@lote=' ' OR @lote=traslado_items{0}.lote)
                    AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=traslado_items{0}.vence)
                ) UNION ALL (
                    SELECT 3 AS tipo,salida{0}.fecha AS fecha,NULL AS entrada,salida_items{0}.cantidad AS salida,NULL AS saldo,salida{0}.id AS concepto  
                    ,salida_items{0}.ingreso
                    FROM salida_items{0}
                    LEFT JOIN salida{0} ON (salida{0}.id=salida_items{0}.salida)
                    WHERE 
                    producto=@producto
                    AND (@lote=' ' OR @lote=salida_items{0}.lote)
                    AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=salida_items{0}.vence)
                ) UNION ALL (
                    SELECT 4 AS tipo,ingreso AS fecha,NULL AS entrada,cantidad AS salida,NULL AS saldo,id AS concepto  
                    ,ingreso
                    FROM transformacion_salida{0}
                    WHERE 
                    producto=@producto
                    AND (@lote=' ' OR @lote=lote)
                    AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=vence)
                ) UNION ALL (
                    SELECT 5 AS tipo,ingreso AS fecha,cantidad AS entrada,NULL AS salida,NULL AS saldo,id AS concepto  
                    ,ingreso
                    FROM transformacion_entrada{0}
                    WHERE 
                    producto=@producto
                    AND (@lote=' ' OR @lote=lote)
                    AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>=vence)
                ) ORDER BY ingreso DESC;">
         <SelectParameters>
            <asp:QueryStringParameter Name="producto" QueryStringField="pid" />
            <asp:QueryStringParameter Name="lote" QueryStringField="l" DefaultValue=" " />
            <asp:QueryStringParameter Name="vence" QueryStringField="v" DefaultValue=" " />
         </SelectParameters>
    </asp:SqlDataSource>
</asp:Panel>
</asp:Content>