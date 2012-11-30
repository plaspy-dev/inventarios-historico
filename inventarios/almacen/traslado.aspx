<%@ Page Title="Traslado" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="traslado.aspx.cs" Inherits="almacen_traslado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .tvence
    {
        width: 100px;
        text-align: right;
        background: white url('../img/cal.gif') left center no-repeat;
    }
    .tlote 
    {
        width: 100px;
    }
</style>
<script type="text/javascript">
    addFunctionToStart(function() {

        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("ti=" + $(this).val());
        });
        $("#<%= CAutomatico.ClientID %>").change(function() {
            if($(this).is(':checked')) {
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>").hide();
                $("#automatico1,#automatico2").show();
            } else { 
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>").show();
                $("#automatico1,#automatico2").hide();
            }
        }).change();
        $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>").attr("readonly", true);
        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProductoDisponible",
                    data: "{ 'almacen': '" + <%= ViewState["almacen"] %> + "', 'auto': " + $("#<%= CAutomatico.ClientID %>").is(':checked') + ",'q': '" + request.term + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            if($("#<%= CAutomatico.ClientID %>").is(':checked'))
                                return {
                                    id: item.producto, value: item.nombre, label: item.nombre, cantidad: item.cantidad, producto: item.nombre
                                };
                            return {
                                value: item.nombre,id: item.producto +";" + item.lote +";" + item.vence, label: item.nombre + " ("+ item.lote + ") " + item.vence, cantidad: item.cantidad, lote: item.lote, vence: item.vence, producto: item.nombre
                            }; 
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.producto);
                $("#smax").text("Máx: " + ui.item.cantidad);
                if(!$("#<%= CAutomatico.ClientID %>").is(':checked')) {
                    $("#<%= TLote.ClientID %>").val(ui.item.lote);
                    $("#<%= TVencimiento.ClientID %>").val(ui.item.vence);
                } else {
                    $("#<%= TLote.ClientID %>").val(" ");
                    $("#<%= TVencimiento.ClientID %>").val(" ");
                }
                return false;
            }
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField runat="server" ID="HId" />
    <table>
        <tr>
            <td colspan="5" align="right"><asp:HyperLink runat="server" ID="HEntrada"></asp:HyperLink></td>
        </tr>
        <tr>
            <td>Número</td>
            <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
            <td>Destino</td>
            <td></td>
            <td><asp:DropDownList ID="DAlmacen" runat="server" DataSourceID="SAlmacen" 
                        DataTextField="nombre" DataValueField="id" AppendDataBoundItems="true">
                        <asp:ListItem Text="(Ajustes)" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SAlmacen" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                        ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                        SelectCommand="SELECT id, nombre FROM almacen WHERE id!=@almacen AND estado IS NULL ORDER BY nombre">
                    </asp:SqlDataSource></td>
        </tr>
        <tr>
            <td>Envío</td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox></td>
            <td>Llegada</td>
            <td><asp:TextBox runat="server" ID="TLlegada" CssClass="fecha"></asp:TextBox></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5"><asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td>Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ValidationGroup="producto" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td>Cantidad <span id="smax"></span><asp:RequiredFieldValidator ControlToValidate="TCantidad" ValidationGroup="producto" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td>Lote</td>
            <td>Vence</td>
            <td><asp:CheckBox runat="server" ID="CAutomatico" Text="Automático" Checked="true" /></td>
        </tr>
        <tr>
            <td><asp:TextBox ID="TProducto" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
            <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
            <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote hide"></asp:TextBox><span id="automatico1">Automático</span></td>
            <td><asp:TextBox ID="TVencimiento" runat="server" CssClass="tvence hide"></asp:TextBox><span id="automatico2">Automático</span></td>
            <td>
                <asp:Button ID="BAgregar" runat="server" Text="Agregar" ValidationGroup="producto" onclick="BAgregar_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="5" align="center">
                <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="id" DataSourceID="SProductos" 
                    onselectedindexchanged="GProductos_SelectedIndexChanged" 
                    EmptyDataText="No hay productos" onrowdeleted="GProductos_RowDeleted" >
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
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
                    SelectCommand="SELECT traslado_items{0}.id,traslado_items{0}.producto,traslado_items{0}.cantidad,traslado_items{0}.lote,traslado_items{0}.vence
                            ,producto.nombre
                            FROM traslado_items{0}
                            LEFT JOIN producto ON (producto.id=traslado_items{0}.producto)
                            WHERE traslado_items{0}.traslado=@traslado;" 
                    InsertCommand="INSERT INTO traslado_items{0} (traslado,producto,cantidad,lote,vence,ingreso) VALUES (@rtraslado, @producto,@cantidad,@lote,@vence,SYSDATE())"
                    UpdateCommand="UPDATE traslado_items{0} SET producto=@producto,cantidad=@cantidad,lote=@lote,vence=@vence WHERE id=@id"
                    DeleteCommand="DELETE FROM traslado_items{0} WHERE id=@id" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="TNumero" Name="traslado" PropertyName="Text" />
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
    <asp:UpdatePanel runat="server" ID="UProducto" UpdateMode="Conditional">
    <ContentTemplate>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>

