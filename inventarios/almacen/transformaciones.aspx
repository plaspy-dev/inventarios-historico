<%@ Page Title="Transformaciones" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="transformaciones.aspx.cs" Inherits="almacen_transformaciones" %>

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
    var proporcion=1;
    addFunctionToStart(function() {

        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("tri=" + $(this).val());
        });
        $("#<%= CAutomatico.ClientID %>").change(function() {
            if($(this).is(':checked')) {
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>").hide();
                $("#automatico1,#automatico2").show();
            } else { 
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>").show();
                $("#automatico1,#automatico2").hide();
            }
            $("#<%= TProductoID.ClientID %>").val("");
            $("#<%= TProducto.ClientID %>").val("").focus();
        }).change();
        $("#<%= CAutomatico2.ClientID %>").change(function() {
            if($(this).is(':checked')) {
                $("#<%= TVencimiento2.ClientID %>,#<%= TLote2.ClientID %>").hide();
                $("#automatico3,#automatico4").show();
            } else { 
                $("#<%= TVencimiento2.ClientID %>,#<%= TLote2.ClientID %>").show();
                $("#automatico3,#automatico4").hide();
            }
            $("#<%= TProductoID2.ClientID %>").val("");
            $("#<%= TProducto2.ClientID %>").val("").focus();
        }).change();
        
        $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>").attr("readonly", true);
        
        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProductoTransferible",
                    data: "{ 'almacen': '" + <%= ViewState["almacen"] %> + "', 'auto': " + $("#<%= CAutomatico.ClientID %>").is(':checked') + ",'q': '" + request.term + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            if (item.codigo && item.codigo != "")
                                item.nombre = item.codigo + " - " + item.nombre;
                            if($("#<%= CAutomatico.ClientID %>").is(':checked'))
                                return {
                                    id: item.id, value: item.nombre, label: item.nombre, cantidad: item.cantidad, producto: item.nombre, unidad: item.unidad, codigo: item.codigo, descripcion: item.descripcion
                                };
                            return {
                                value: item.nombre,id: item.id +";" + item.lote +";" + item.vence, label: item.ProductoCompleto, cantidad: item.cantidad, lote: item.lote, vence: item.vence, producto: item.nombre, unidad: item.unidad, codigo: item.codigo, descripcion: item.descripcion
                            }; 
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            minLength: 3,
                    autoFocus: true,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.value);
                $("#<%= TUnidad.ClientID %>").val(ui.item.unidad);
                $("#tdescripcion").text(ui.item.descripcion);
                $("#smax").text("Máx: " + ui.item.cantidad + " " + ui.item.unidad);
                if(!$("#<%= CAutomatico.ClientID %>").is(':checked')) {
                    $("#<%= TLote.ClientID %>").val(ui.item.lote);
                    $("#<%= TVencimiento.ClientID %>").val(ui.item.vence);
                } else {
                    $("#<%= TLote.ClientID %>").val(" ");
                    $("#<%= TVencimiento.ClientID %>").val(" ");
                }
                
                /*
                    Buscamos si solo hay una transformacion posible
                */
                
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProductoTransferibleDestino",
                    data: "{ 'producto': " + $("#<%= TProductoID.ClientID %>").val().split(';')[0] + ",'q': '' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        var mapa= $.map(data.d, function(item) {
                            if (item.codigo && item.codigo != "")
                                item.nombre = item.codigo + " - " + item.nombre;
                            return {
                                value: item.nombre,id: item.id, label: item.nombre, cantidad: item.cantidad, lote: item.lote, vence: item.vence, producto: item.nombre, unidad: item.unidad, codigo: item.codigo, descripcion: item.descripcion
                            }; 
                        });
                        if(mapa.length==1) {
                            select2(mapa[0]);
                        }
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
                
                return false;
            }
        });
        
        
        $("#<%= TProducto2.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProductoTransferibleDestino",
                    data: "{ 'producto': " + $("#<%= TProductoID.ClientID %>").val().split(';')[0] + ",'q': '" + request.term + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            if (item.codigo && item.codigo != "")
                                item.nombre = item.codigo + " - " + item.nombre;
                            return {
                                value: item.nombre,id: item.id, label: item.nombre, cantidad: item.cantidad, lote: item.lote, vence: item.vence, producto: item.nombre, unidad: item.unidad, codigo: item.codigo
                            }; 
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            minLength: 3,
                    autoFocus: true,
            select: function(event, ui) {

                select2(ui.item);
                return false;
            }
        });
        
        function select2(item) {
        
            proporcion=item.cantidad;
            $("#<%= TProductoID2.ClientID %>").val(item.id);
            $("#<%= TProducto2.ClientID %>").val(item.value);
            $("#<%= TUnidad2.ClientID %>").val(item.unidad);        
            $("#smax2").text("(" + item.unidad + ")");
            $("#<%= TCantidad.ClientID %>").change();
            $("#tdescripcion2").text(item.descripcion);
        }
        
        $("#<%= TCantidad.ClientID %>").change(function() {
            $("#<%= TCantidad2.ClientID %>").val(
                floatFormatted(proporcion*parseFloat($("#<%= TCantidad.ClientID %>").val())
            ));
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:UpdatePanel runat="server" ID="UProducto" UpdateMode="Conditional">
    <ContentTemplate>
<asp:HiddenField runat="server" ID="HId" />
<table class="tablecollapse">
    <tr>
        <td>Numero</td>
        <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5" class="GridHeader">Producto original</td>
    </tr>
    <tr>
        <td class="tdl"></td>
        <td colspan="4" class="tdr" id="tdescripcion"></td>
    </tr>
    <tr>
        <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="4" class="tdr"><asp:TextBox ID="TProducto" Width="98%" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr class="GridAltRow">
        <td class="tdl"></td>
        <td><span id="smax"></span><asp:RequiredFieldValidator ControlToValidate="TCantidad" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
        </td>
        <td>Lote
        </td>
        <td>Vence
        </td>
        <td class="tdr">
        </td>
    </tr>
    <tr>
        <td class="tdl">Cantidad</td>
        <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
        <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote hide"></asp:TextBox><span id="automatico1">Automático</span></td>
        <td><asp:TextBox ID="TVencimiento" runat="server" CssClass="tvence hide"></asp:TextBox><span id="automatico2">Automático</span>
        <asp:TextBox ID="TUnidad" runat="server" CssClass="hide"></asp:TextBox></td>
        <td class="tdr"><asp:CheckBox runat="server" ID="CAutomatico" Text="Automático" Checked="true" /></td>
    </tr>
    <tr>
        <td colspan="5" class="GridHeader">Producto destino</td>
    </tr>
    <tr>
        <td class="tdl"></td>
        <td colspan="4" class="tdr" id="tdescripcion1"></td>
    </tr>
    <tr>
        <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID2" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="4" class="tdr"><asp:TextBox ID="TProducto2" Width="98%" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID2" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr class="GridAltRow">
        <td class="tdl"></td>
        <td><span id="smax2"></span>
        </td>
        <td>Lote<asp:RequiredFieldValidator ControlToValidate="TLote2" ErrorMessage="Por favor ingrese un lote." ID="RLote" Enabled="false" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
        </td>
        <td>Vence<asp:RequiredFieldValidator ControlToValidate="TVencimiento2" ErrorMessage="Por favor ingrese una fecha." ID="RVencimiento" Enabled="false" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
        </td>
        <td class="tdr">
        </td>
    </tr>
    <tr>
        <td class="tdl">Cantidad</td>
        <td><asp:TextBox ID="TCantidad2" ReadOnly="true" runat="server" CssClass="float"></asp:TextBox></td>
        <td><asp:TextBox ID="TLote2" runat="server" CssClass="tlote hide"></asp:TextBox><span id="automatico3">Automático</span></td>
        <td><asp:TextBox ID="TVencimiento2" runat="server" CssClass="tvence hide"></asp:TextBox><span id="automatico4">Automático</span>
        <asp:TextBox ID="TUnidad2" runat="server" CssClass="hide"></asp:TextBox></td>
        <td class="tdr"><asp:CheckBox runat="server" ID="CAutomatico2" Text="Automático" Checked="true" /></td>
    </tr>
    <tr>
        <td colspan="5" class="tdt">&nbsp;</td>
    </tr>
        <tr>
            <td>Observaciones</td>
            <td colspan="4"><asp:TextBox runat="server" ID="TObservaciones" TextMode="MultiLine" Width="99%"></asp:TextBox></td>
        </tr>
    <tr>
        <td colspan="5"><asp:Label runat="server" ID="LMensaje"></asp:Label></td>
    </tr>
        <tr>
            <td colspan="5"><asp:ValidationSummary runat="server" ID="ValidationSumary1" /><asp:ValidationSummary ValidationGroup="producto" runat="server" ID="ValidationSummary2" /></td>
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

