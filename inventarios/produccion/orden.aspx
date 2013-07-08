<%@ Page Title="Orden de producción" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="orden.aspx.cs" Inherits="produccion_orden" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    $(function() {
        $("#<%= TCliente.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetTercero",
                    data: "{ 'q': '" + request.term + "', tipo: 1 }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.Documento + " " + item.Nombre;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            autoFocus: true,
            minLength: 3,
            search: function() {
                $("#<%= TClienteID.ClientID %>").val("");
            },
            select: function(event, ui) {

                $("#<%= TClienteID.ClientID %>").val(ui.item.Id);
                $("#<%= TCliente.ClientID %>").val(ui.item.Documento + " " + ui.item.Nombre);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0)
                $("#<%= TClienteID.ClientID %>").val("");
        });
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
                    url: "produccion.asmx/GetProducto",
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
                $("#<%= TRecetaID.ClientID %>").val("");
                $("#<%= TProductoID.ClientID %>").val("");
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                $("#<%= TRecetaID.ClientID %>").val(ui.item.IdTabla);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0) {
                $("#<%= TRecetaID.ClientID %>").val("");
                $("#<%= TProductoID.ClientID %>").val("");
            }
        });
        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("oi=" + $(this).val());
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel runat="server" ID="PImprimir" Visible="false">
    <div id="dialog-confirm" title="Imprimir" class="hide"> 
        <p><asp:Label runat="server" ID="LImprimir"></asp:Label></p>
    </div>
    <script type="text/javascript">
        $(function() {
            $("#dialog-confirm").dialog({ resizable: false, modal: true, buttons: { "Imprimir": function() { window.open('<%= ViewState["urlprint"] %>', '_blank'); $(this).dialog("close"); }, "Cancelar": function() { $(this).dialog("close"); } } });
        });
    </script>
</asp:Panel>
<asp:Panel runat="server" DefaultButton="BAceptar">
<table class="tablecollapse">
    <tr>
        <td>Número</td>
        <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td>Fecha Planeación
            <asp:RequiredFieldValidator ControlToValidate="TFecha" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator>
        </td>
        <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox></td>
        <td>Fecha producción
        <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator1" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
        <td><asp:TextBox runat="server" ID="TFechaProduccion" CssClass="fecha"></asp:TextBox></td>
    </tr>
    <tr>
        <td>Cliente<asp:RequiredFieldValidator ControlToValidate="TClienteID" ErrorMessage="Por favor ingrese una cliente." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3"><asp:TextBox runat="server" ID="TCliente" Width="98%"></asp:TextBox>
        <asp:TextBox runat="server" ID="TClienteID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td>Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3"><asp:TextBox runat="server" ID="TProducto" Width="98%"></asp:TextBox>
        <asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td>Lote</td>
        <td><asp:TextBox runat="server" ID="TLote"></asp:TextBox></td>
        <td>Cantidad programada</td>
        <td><asp:TextBox runat="server" ID="TCantidad" CssClass="float"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4" align="right"><asp:Button runat="server" ID="BFormular" Text="Traer formula" CausesValidation="false" OnClick="BFormular_Click" /></td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Requerimientos</td>
    </tr>
    <tr>
        <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TIngredienteID" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3" class="tdr"><asp:TextBox runat="server" ID="TIngrediente" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TIngredienteID" CssClass="hide"></asp:TextBox><asp:TextBox runat="server" ID="TRecetaID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Cantidad
            <asp:RequiredFieldValidator ControlToValidate="TCantidadIngrediente" ValidationGroup="agregar" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidadIngrediente" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
        <td><asp:TextBox runat="server" ID="TCantidadIngrediente" CssClass="float"></asp:TextBox></td>
        <td><asp:HiddenField runat="server" ID="HProductos" /><asp:Button runat="server" ID="BMas" Text="Agregar" ValidationGroup="agregar" OnClick="BMas_Click" /></td>
        <td class="tdr"></td>
    </tr>
    <tr>
        <td colspan="4" class="tdl tdr tdb">
            <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="IdGrid,id,ubicacion,estado" 
                    EmptyDataText="No hay productos" onrowdeleting="GProductos_RowDeleting" ShowFooter="true" >
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
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:F2}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                    </Columns>
                </asp:GridView>
        </td>
    </tr>
    <tr>
        <td>Observaciones</td>
        <td colspan="3"><asp:TextBox runat="server" ID="TObservaciones" Width="98%"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4"><asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label><asp:ValidationSummary runat="server" /></td>
    </tr>
    <tr>
        <td colspan="4" align="right">
            <asp:Button runat="server" ID="BAceptar" Text="Aceptar" 
                onclick="BAceptar_Click" />
            <asp:Button runat="server" ID="BCancelar" Text="Cancelar" 
                CausesValidation="false" onclick="BCancelar_Click" />
        </td>
    </tr>
</table>
</asp:Panel>
</asp:Content>