<%@ Page Title="Entrada" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="entrada.aspx.cs" Inherits="almacen_entrada" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .tlote 
    {
        width: 100px;
    }
</style>
<script type="text/javascript">
    addFunctionToStart(function() {

        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            return {
                                value: item.value, label: item.item
                            }
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.value);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                return false;
            }
        });
        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("ei=" + $(this).val());
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:UpdatePanel runat="server" ID="UProducto" UpdateMode="Conditional">
<ContentTemplate>
    <asp:HiddenField runat="server" ID="HId" />
    <table>
        <tr>
            <td colspan="5" align="right"><asp:HyperLink runat="server" ID="HTraslado"></asp:HyperLink></td>
        </tr>
        <tr>
            <td>Número</td>
            <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
            <td>Fecha</td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                &nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ValidationGroup="producto" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td>Cantidad<asp:RequiredFieldValidator ControlToValidate="TCantidad" ValidationGroup="producto" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td>Lote<asp:RequiredFieldValidator ControlToValidate="TLote" ValidationGroup="producto" ErrorMessage="Por favor ingrese un lote." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td>Vence<asp:RequiredFieldValidator ControlToValidate="TVencimiento" ValidationGroup="producto" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="TProducto" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
            <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
            <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote"></asp:TextBox></td>
            <td><asp:TextBox ID="TVencimiento" runat="server" CssClass="fecha"></asp:TextBox></td>
            <td>
                <asp:Button ID="BAgregar" runat="server" Text="Agregar" 
                    ValidationGroup="producto" onclick="BAgregar_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="5" align="center">
                <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="id" DataSourceID="SProductos" 
                    onselectedindexchanged="GProductos_SelectedIndexChanged" EmptyDataText="No hay productos" >
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
                        <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" />
                        <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:F2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                        <asp:BoundField DataField="vence" HeaderText="Vence" SortExpression="vence" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="producto" HeaderText="IdProducto" SortExpression="producto" ItemStyle-CssClass="hide" HeaderStyle-CssClass="hide" />
                    </Columns>
                </asp:GridView>
                <sdg:customdatasource ID="SProductos" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    SelectCommand="SELECT entrada_items{0}.id,entrada_items{0}.producto,entrada_items{0}.cantidad,entrada_items{0}.lote,entrada_items{0}.vence
                            ,producto.nombre
                            FROM entrada_items{0}
                            LEFT JOIN producto ON (producto.id=entrada_items{0}.producto)
                            WHERE entrada_items{0}.entrada=@entrada;" 
                    InsertCommand="INSERT INTO entrada_items{0} (entrada,producto,cantidad,lote,vence) VALUES (@rentrada, @producto,@cantidad,@lote,@vence)"
                    UpdateCommand="UPDATE entrada_items{0} SET producto=@producto,cantidad=@cantidad,lote=@lote,vence=@vence WHERE id=@id"
                    DeleteCommand="DELETE FROM entrada_items{0} WHERE id=@id" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="TNumero" Name="entrada" PropertyName="Text" />
                    </SelectParameters>
                </sdg:customdatasource>
            </td>
        </tr>        
        <tr>
            <td>Observaciones</td>
            <td colspan="4"><asp:TextBox runat="server" ID="TObservaciones" TextMode="MultiLine" Width="99%"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="5" align="right">
                <asp:Button runat="server" ID="BAceptar" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
            </td>
        </tr>
    </table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>