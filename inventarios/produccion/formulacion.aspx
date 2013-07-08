<%@ Page Title="Formulación" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="formulacion.aspx.cs" Inherits="produccion_formulacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    addFunctionToStart(function() {

        $("#<%= TIngrediente.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': '' }",
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
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            search: function() {
                $("#<%= TIngredienteID.ClientID %>").val("");
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TIngredienteID.ClientID %>").val(ui.item.id);
                $("#<%= TIngrediente.ClientID %>").val(ui.item.label);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0)
                $("#<%= TIngredienteID.ClientID %>").val("");
        });

        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': '' }",
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
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            search: function() {
                $("#<%= TProductoID.ClientID %>").val("");
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                $("#<%= LUnidad.ClientID %>").html(ui.item.unidad);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0)
                $("#<%= TProductoID.ClientID %>").val("");
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:UpdatePanel runat="server" ID="UProducto" UpdateMode="Conditional">
<ContentTemplate>
<table class="tablecollapse">
    <tr>
        <td>Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3"><asp:TextBox runat="server" ID="TProducto" Width="98%"></asp:TextBox>
        <asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Requerimientos (Para producir 1 <asp:Label runat="server" ID="LUnidad"></asp:Label>)</td>
    </tr>
    <tr>
        <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TIngredienteID" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3" class="tdr"><asp:TextBox runat="server" ID="TIngrediente" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TIngredienteID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Cantidad
            <asp:RequiredFieldValidator ControlToValidate="TCantidadIngrediente" ValidationGroup="agregar" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidadIngrediente" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
        <td><asp:TextBox runat="server" ID="TCantidadIngrediente" CssClass="float"></asp:TextBox></td>
        <td>
        <asp:HiddenField runat="server" ID="HProductos" />
        <asp:Button runat="server" ID="BMas" Text="Agregar" ValidationGroup="agregar" onclick="BMas_Click" /></td>
        <td class="tdr"></td>
    </tr>
    <tr>
        <td colspan="4" class="tdl tdr tdb" align="center">
            <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="IdGrid,id" EmptyDataText="Por favor agregue productos" 
                    onrowdeleting="GProductos_RowDeleting" ShowFooter="true" >
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
                        <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                        <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:0.00}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                    </Columns>
                </asp:GridView>
        </td>
    </tr>
    <tr>
        <td colspan="4"><asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label><asp:ValidationSummary ID="ValidationSummary1" runat="server" /></td>
    </tr>
    <tr>
        <td colspan="4" align="right">
            <asp:Button runat="server" ID="BAceptar" Text="Aceptar" onclick="BAceptar_Click" />
            <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
            <asp:Button runat="server" ID="BEliminar" Text="Eliminar" Visible="false" OnClientClick="confirm('¿Esta seguro de eliminar esta formula?');" CausesValidation="false" onclick="BEliminar_Click" />
        </td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4"><hr /></td>
    </tr>
    <tr>
        <td colspan="4">
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" 
                DataKeyNames="id" DataSourceID="SResultados" AllowSorting="true" AllowPaging="true" PageSize="50"
                onselectedindexchanged="GResultado_SelectedIndexChanged" >
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" />
                    <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" />
                    <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" />
                    <asp:BoundField DataField="unidad" HeaderText="Unidad" SortExpression="unidad" />
                    <asp:BoundField DataField="empresa" HeaderText="Empresa" SortExpression="empresa" />
                    <asp:BoundField DataField="ingredientes" HeaderText="Ingredientes" SortExpression="ingredientes" ItemStyle-HorizontalAlign="Right" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT formulacion.id,formulacion.producto 
                    ,producto.nombre,producto.descripcion,producto.codigo,producto.unidad,producto.empresa
                    ,COUNT(formulacion_ingrediente.formulacion) AS ingredientes
                    FROM formulacion
                    LEFT JOIN producto ON (producto.id=formulacion.producto)
                    LEFT JOIN formulacion_ingrediente ON (formulacion_ingrediente.formulacion=formulacion.id)
                    GROUP BY formulacion.id
                    ORDER BY producto.nombre
                    ">
            </asp:SqlDataSource>
        </td>
    </tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>