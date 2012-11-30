<%@ Page Title="Estado" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="estado.aspx.cs" Inherits="almacen_estado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel runat="server" ID="PBuscar" DefaultButton="BBuscar">
    <table>
        <tr>
            <td>
                Vence</td>
            <td>
                <asp:TextBox ID="TVence" runat="server" CssClass="fecha" Text=""></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator1" runat="server" Type="Date" ErrorMessage="Por favor ingrese una fecha valida." Text="*" MinimumValue="01/01/1900" MaximumValue="01/01/3000" SetFocusOnError="True" Display="Dynamic" ControlToValidate="TVence"></asp:RangeValidator>
            </td>
            <td>
                <asp:CheckBox ID="CDetallado" runat="server" Text="Detallado" />
            </td>
            <td>
                <asp:CheckBox ID="CTodos" runat="server" Text="Todos" />
            </td>
            <td>
                <asp:Button ID="BBuscar" runat="server" Text="Actualizar" 
                    onclick="BBuscar_Click" />
            </td>
        </tr>
        </table>
    <br />
    <asp:GridView ID="GResultados" runat="server" DataSourceID="SResultados" 
        AutoGenerateColumns="False" EmptyDataText="No se encontraron productos" 
        AllowPaging="True" PageSize="50" AllowSorting="True" DataKeyNames="producto" >
        <RowStyle CssClass="GridRow"></RowStyle>
        <FooterStyle CssClass="GridFooter"></FooterStyle>
        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
        <Columns>
            <asp:HyperLinkField HeaderText="Producto" DataNavigateUrlFormatString="~/almacen/estado.aspx?pid={3}&n={0}&l={1}&v={2:d}" DataNavigateUrlFields="nombre,lote,vence,producto" DataTextField="nombre" SortExpression="nombre" />
            <asp:BoundField HeaderText="Lote" DataField="lote" SortExpression="lote" />
            <asp:BoundField HeaderText="Vence" DataField="vence" SortExpression="vence" DataFormatString="{0:d}" />
            <asp:BoundField HeaderText="Cantidad" DataField="cantidad" SortExpression="cantidad" ItemStyle-HorizontalAlign="Right" />
        </Columns>
    </asp:GridView>
    <div style="text-align: right;">
        <asp:Label ID="LTotal" runat="server" Text=""></asp:Label>
    </div>
    <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT estado{0}.producto,SUM(estado{0}.cantidad) AS cantidad,producto.nombre
                        ,'' AS lote,'' AS vence
                        FROM estado{0} 
                        LEFT JOIN producto ON (producto.id=estado{0}.producto) 
                        WHERE 
                        (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>estado{0}.vence)
                        GROUP BY producto.id HAVING (@todos=1 OR SUM(estado{0}.cantidad)>0)
                        ORDER BY producto.nombre">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TVence" PropertyName="Text" Name="vence" DefaultValue=" " />
                    <asp:ControlParameter ControlID="CTodos" PropertyName="Checked" Name="todos" />
                 </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SResultados1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT estado{0}.producto,estado{0}.lote,estado{0}.vence,estado{0}.cantidad,producto.nombre FROM estado{0} 
                        LEFT JOIN producto ON (producto.id=estado{0}.producto) 
                        WHERE 
                        (@todos=1 OR estado{0}.cantidad>0)
                        AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')>estado{0}.vence)
                        ORDER BY producto.nombre,estado{0}.lote,estado{0}.vence">
                 <SelectParameters>
                    <asp:ControlParameter ControlID="TVence" PropertyName="Text" Name="vence" DefaultValue=" " />
                    <asp:ControlParameter ControlID="CTodos" PropertyName="Checked" Name="todos" />
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
                    SELECT 1 AS tipo,entrada{0}.fecha,entrada_items{0}.cantidad AS entrada,NULL AS salida,NULL AS saldo,entrada{0}.id AS concepto FROM entrada_items{0}
                    LEFT JOIN entrada{0} ON (entrada{0}.id=entrada_items{0}.entrada)
                    WHERE producto=@producto
                    AND (@lote=' ' OR @lote=entrada_items{0}.lote)
                    AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')=entrada_items{0}.vence)
                ) UNION (
                    SELECT 2 AS tipo,traslado{0}.envio AS fecha,NULL AS entrada,traslado_items{0}.cantidad AS salida,NULL AS saldo,traslado{0}.id AS concepto  FROM traslado_items{0}
                    LEFT JOIN traslado{0} ON (traslado{0}.id=traslado_items{0}.traslado)
                    WHERE 
                    producto=@producto
                    AND (@lote=' ' OR @lote=traslado_items{0}.lote)
                    AND (@vence=' ' OR STR_TO_DATE(@vence, '%d/%m/%Y')=traslado_items{0}.vence)
                ) ORDER BY fecha DESC;">
         <SelectParameters>
            <asp:QueryStringParameter Name="producto" QueryStringField="pid" />
            <asp:QueryStringParameter Name="lote" QueryStringField="l" DefaultValue=" " />
            <asp:QueryStringParameter Name="vence" QueryStringField="v" DefaultValue=" " />
         </SelectParameters>
    </asp:SqlDataSource>
</asp:Panel>
</asp:Content>