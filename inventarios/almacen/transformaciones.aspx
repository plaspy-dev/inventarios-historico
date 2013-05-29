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
    var iniciando=true;
    var proporcion=1;
    var tiponumero= 0;
    var unidad="";
    
    addFunctionToStart(function() {

        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("tri=" + $(this).val());
        });
        if($("#<%= CAutomatico.ClientID %>").length==0) {
            $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>,#<%= TSerial.ClientID %>,#<%= TEstado.ClientID %>").show();
            $("#<%= LLoteA.ClientID %>,#<%= LVencimientoA.ClientID %>,#<%= LSerialA.ClientID %>,#<%= LEstadoA.ClientID %>").hide();
            $("#<%= TVencimiento2.ClientID %>,#<%= TLote2.ClientID %>,#<%= TSerial2.ClientID %>,#<%= DEstado2.ClientID %>").show();
            $("#<%= LLoteA2.ClientID %>,#<%= LVencimientoA2.ClientID %>,#<%= LSerialA2.ClientID %>,#<%= LEstadoA2.ClientID %>").hide();
        }
        $("#<%= CAutomatico.ClientID %>").change(function() {
            if($(this).is(':checked')) {
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>,#<%= TSerial.ClientID %>,#<%= TEstado.ClientID %>").hide();
                $("#<%= LLoteA.ClientID %>,#<%= LVencimientoA.ClientID %>,#<%= LSerialA.ClientID %>,#<%= LEstadoA.ClientID %>").show();
            } else { 
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>,#<%= TSerial.ClientID %>,#<%= TEstado.ClientID %>").show();
                $("#<%= LLoteA.ClientID %>,#<%= LVencimientoA.ClientID %>,#<%= LSerialA.ClientID %>,#<%= LEstadoA.ClientID %>").hide();
            }
            $("#<%= LLote.ClientID %>,#<%= LVencimiento.ClientID %>,#<%= LSerial.ClientID %>,#<%= LEstado.ClientID %>").show();
            $("#tdescripcion").text("");
            $("#<%= TProductoID.ClientID %>").val("");
            $("#<%= TProducto.ClientID %>").val("").focus();
        });
        $("#<%= CAutomatico2.ClientID %>").change(function() {
            if($(this).is(':checked')) {
                $("#<%= TVencimiento2.ClientID %>,#<%= TLote2.ClientID %>,#<%= TSerial2.ClientID %>,#<%= DEstado2.ClientID %>").hide();
                $("#<%= LLoteA2.ClientID %>,#<%= LVencimientoA2.ClientID %>,#<%= LSerialA2.ClientID %>,#<%= LEstadoA2.ClientID %>").show();
            } else { 
                $("#<%= TVencimiento2.ClientID %>,#<%= TLote2.ClientID %>,#<%= TSerial2.ClientID %>,#<%= DEstado2.ClientID %>").show();
                $("#<%= LLoteA2.ClientID %>,#<%= LVencimientoA2.ClientID %>,#<%= LSerialA2.ClientID %>,#<%= LEstadoA2.ClientID %>").hide();
                $("#<%= TLote2.ClientID %>").focus();
            }
            $("#<%= LLote2.ClientID %>,#<%= LVencimiento2.ClientID %>,#<%= LSerial2.ClientID %>,#<%= LEstado2.ClientID %>").show();
        });
        if(iniciando) 
            $("#<%= CAutomatico.ClientID %>,#<%= CAutomatico2.ClientID %>").change();
        iniciando=false;
        
        $("#<%= TCantidad2.ClientID %>").attr("readonly", true);
        $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>,#<%= TSerial.ClientID %>,#<%= TEstado.ClientID %>").attr("readonly", true);
        
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
                            item.label= item.ProductoCompleto;
                            if(!$("#<%= CAutomatico.ClientID %>").is(':checked'))
                                item.id= item.id +";" + item.lote +";" + item.vence + ";" + item.estado + ";" + item.serial + ";" + item.ubicacion;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            minLength: 3,
                    autoFocus: true,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                var descripcion= ui.item.Empresa;
                if(ui.item.Empresa!="" && ui.item.descripcion!="")
                    descripcion+=": ";
                descripcion+= " " + ui.item.descripcion;
                $("#tdescripcion").text(descripcion);
                $("#smax").text("Máx: " + ui.item.cantidad + " " + ui.item.unidad);
                if(!$("#<%= CAutomatico.ClientID %>").is(':checked')) {
                    $("#<%= TLote.ClientID %>").val(ui.item.lote);
                    $("#<%= TVencimiento.ClientID %>").val(ui.item.vence);
                    $("#<%= TSerial.ClientID %>").val(ui.item.serial);
                    $("#<%= TEstado.ClientID %>").val(ui.item.szEstado);
                    ui.item.nVence>0 ? $("#<%= TVencimiento.ClientID %>,#<%= LVencimiento.ClientID %>").show() : $("#<%= TVencimiento.ClientID %>,#<%= LVencimiento.ClientID %>").hide();
                    ui.item.nLote>0 ? $("#<%= TLote.ClientID %>,#<%= LLote.ClientID %>").show() : $("#<%= TLote.ClientID %>,#<%= LLote.ClientID %>").hide();
                    ui.item.nSerial>0 ? $("#<%= TSerial.ClientID %>,#<%= LSerial.ClientID %>").show() : $("#<%= TSerial.ClientID %>,#<%= LSerial.ClientID %>").hide();
                    ui.item.nEstado>0 ? $("#<%= TEstado.ClientID %>,#<%= LEstado.ClientID %>").show() : $("#<%= TEstado.ClientID %>,#<%= LEstado.ClientID %>").hide();
                } else {
                    $("#<%= TLote.ClientID %>").val("");
                    $("#<%= TVencimiento.ClientID %>").val("");
                    $("#<%= TSerial.ClientID %>").val("");
                    $("#<%= TEstado.ClientID %>").val("");
                    ui.item.nLote>0 ? $("#<%= LLoteA.ClientID %>,#<%= LLote.ClientID %>").show() : $("#<%= LLoteA.ClientID %>,#<%= LLote.ClientID %>").hide();
                    ui.item.nVence>0 ? $("#<%= LVencimientoA.ClientID %>,#<%= LVencimiento.ClientID %>").show() : $("#<%= LVencimientoA.ClientID %>,#<%= LVencimiento.ClientID %>").hide();
                    ui.item.nSerial>0 ? $("#<%= LSerialA.ClientID %>,#<%= LSerial.ClientID %>").show() : $("#<%= LSerialA.ClientID %>,#<%= LSerial.ClientID %>").hide();
                    ui.item.nEstado>0 ? $("#<%= LEstadoA.ClientID %>,#<%= LEstado.ClientID %>").show() : $("#<%= LEstadoA.ClientID %>,#<%= LEstado.ClientID %>").hide();
                }
                ValidatorEnable($("#<%= REntero.ClientID %>")[0], ui.item.nNumero==1);
                ValidatorEnable($("#<%= RDecimal.ClientID %>")[0], ui.item.nNumero!=1);
                
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
                            item.label= item.ProductoCompleto;
                            return item;
                        });
                        if(mapa.length==1) {
                            select2(mapa[0]);
                        }
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + ":" + errorThrown);
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
                                item.label = item.codigo + " - " + item.nombre;
                            else item.label= item.nombre;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + ":" + errorThrown);
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
            $("#<%= TProducto2.ClientID %>").val(item.label);
            unidad= item.unidad;
            if(unidad!="")
                $("#smax2").text("(" + unidad + ")");
            else $("#smax2").text("");
            
            var descripcion= item.Empresa;
            if(item.Empresa!="" && item.descripcion!="")
                descripcion+=": ";
            descripcion+= " " + item.descripcion;
            $("#tdescripcion2").text(descripcion);
            
            if(!$("#<%= CAutomatico2.ClientID %>").is(':checked')) {
                item.nVence>0 ? $("#<%= TVencimiento2.ClientID %>,#<%= LVencimiento2.ClientID %>").show() : $("#<%= TVencimiento2.ClientID %>,#<%= LVencimiento2.ClientID %>").hide();
                item.nLote>0 ? $("#<%= TLote2.ClientID %>,#<%= LLote2.ClientID %>").show() : $("#<%= TLote2.ClientID %>,#<%= LLote2.ClientID %>").hide();
                ui.item.nSerial>0 ? $("#<%= TSerial2.ClientID %>,#<%= LSerial2.ClientID %>").show() : $("#<%= TSerial2.ClientID %>,#<%= LSerial2.ClientID %>").hide();
                ui.item.nEstado>0 ? $("#<%= DEstado2.ClientID %>,#<%= LEstado2.ClientID %>").show() : $("#<%= DEstado2.ClientID %>,#<%= LEstado2.ClientID %>").hide();
                ValidatorEnable($("#<%= RVencimiento.ClientID %>")[0], item.nVence==1);
                ValidatorEnable($("#<%= RLote.ClientID %>")[0], item.nLote==1);
                ValidatorEnable($("#<%= RSerial.ClientID %>")[0], ui.item.nSerial==1);
                ValidatorEnable($("#<%= REstado.ClientID %>")[0], ui.item.nEstado==1);
            } else {
                ValidatorEnable($("#<%= RVencimiento.ClientID %>")[0], false);
                ValidatorEnable($("#<%= RLote.ClientID %>")[0], false);
                ValidatorEnable($("#<%= RSerial.ClientID %>")[0], false);
                ValidatorEnable($("#<%= REstado.ClientID %>")[0], false);
            }
            ValidatorEnable($("#<%= REntero2.ClientID %>")[0], item.nNumero==1);
            ValidatorEnable($("#<%= RDecimal2.ClientID %>")[0], item.nNumero!=1);
            tiponumero= item.nNumero;
            
            $("#<%= TCantidad.ClientID %>").change();
        }
        
        $("#<%= TCantidad.ClientID %>").change(function() {
            if(tiponumero==1) { //entero
                var cantidad=parseFloat($("#<%= TCantidad.ClientID %>").val());
                var entero=parseInt(cantidad/proporcion);
                $("#<%= TCantidad2.ClientID %>").numeric('value',entero);
                entero=entero*proporcion;
                if(unidad!="")
                    $("#smax2").text("(" + unidad + ")");
                else $("#smax2").text("");
                if(cantidad>0 && cantidad!=entero) {
                    entero= (cantidad-entero).toFixed(2);
                    $("#smax2").text($("#smax2").text() + " sobran " + entero);
                }
            } else {
                $("#<%= TCantidad2.ClientID %>").numeric('value',parseFloat($("#<%= TCantidad.ClientID %>").val())/proporcion);
            }
        });
        // $("#<%= TCantidad.ClientID %>").change();
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
        <td class="tdl"><asp:CheckBox runat="server" ID="CAutomatico" Text="Automático" Checked="true" /></td>
        <td align="right"><asp:Label runat="server" ID="LLote" Text="Lote: "></asp:Label></td>
        <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LLoteA" Text="Automático"></asp:Label></td>
        <td align="right"><asp:Label runat="server" ID="LVencimiento" Text="Vence: "></asp:Label></td>
        <td class="tdr"><asp:TextBox ID="TVencimiento" runat="server" CssClass="tvence hide"></asp:TextBox><asp:Label runat="server" ID="LVencimientoA" Text="Automático"></asp:Label></td>
    </tr>
    <tr>
        <td class="tdl"><asp:Label runat="server" ID="LUbicacion" Text="Ubicación: "></asp:Label><asp:TextBox runat="server" ID="TUbicacion" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LUbicacionA" Text="Automática"></asp:Label></td>
        <td align="right"><asp:Label runat="server" ID="LSerial" Text="Serial:"></asp:Label></td>
        <td><asp:TextBox runat="server" ID="TSerial" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LSerialA" Text="Automático"></asp:Label></td>
        <td align="right"><asp:Label runat="server" ID="LEstado" Text="Estado:"></asp:Label></td>
        <td class="tdr"><asp:TextBox ID="TEstado" runat="server" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LEstadoA" Text="Automático"></asp:Label></td>
    </tr>
    <tr class="GridAltRow">
        <td class="tdl"></td>
        <td colspan="4" class="tdr" id="tdescripcion"></td>
    </tr>
    <tr class="GridAltRow">
        <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="4" class="tdr"><asp:TextBox ID="TProducto" Width="98%" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Cantidad<asp:RequiredFieldValidator ControlToValidate="TCantidad" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
        <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
        </td>
        <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
        <td class="tdr" colspan="3"><span id="smax"></span></td>
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
        <td class="tdl">&nbsp;</td>
        <td colspan="4" class="tdr"><span id="smax2"></span>
        </td>
    </tr>
    <tr>
        <td class="tdl">Cantidad
            <asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero2" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TCantidad2" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal2" runat="server" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidad2" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
        </td>
        <td><asp:TextBox ID="TCantidad2" runat="server" CssClass="float"></asp:TextBox></td>
        <td><asp:CheckBox runat="server" ID="CAutomatico2" Text="Automático" Checked="true" /></td>
        <td>&nbsp;</td>
        <td class="tdr">&nbsp;</td>
    </tr>
    <tr>
        <td class="tdl"></td>
        <td align="right"><asp:Label runat="server" ID="LLote2" Text="Lote"></asp:Label><asp:RequiredFieldValidator ControlToValidate="TLote2" ErrorMessage="Por favor ingrese un lote." ID="RLote" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator></td>
        <td><asp:TextBox ID="TLote2" runat="server" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LLoteA2" Text="Automático"></asp:Label></td>
        <td><asp:Label runat="server" ID="LVencimiento2" Text="Vence"></asp:Label>
        <asp:RequiredFieldValidator ControlToValidate="TVencimiento2" ErrorMessage="Por favor ingrese una fecha." ID="RVencimiento" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator>
        <asp:RangeValidator ControlToValidate="TVencimiento2" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator1" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
        <td class="tdr"><asp:TextBox ID="TVencimiento2" runat="server" CssClass="fecha hide"></asp:TextBox><asp:Label runat="server" ID="LVencimientoA2" Text="Automático"></asp:Label></td>
    </tr>
    <tr>
        <td class="tdl"></td>
        <td align="right"><asp:Label runat="server" ID="LSerial2" Text="Serial:"></asp:Label><asp:RequiredFieldValidator ControlToValidate="TSerial2" ErrorMessage="Por favor ingrese un serial." ID="RSerial" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator></td>
        <td><asp:TextBox runat="server" ID="TSerial2" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LSerialA2" Text="Automático"></asp:Label></td>
        <td align="right"><asp:Label runat="server" ID="LEstado2" Text="Estado:"></asp:Label><asp:RequiredFieldValidator InitialValue="0" ControlToValidate="DEstado2" ErrorMessage="Por favor seleccione un estado." ID="REstado" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator></td>
        <td class="tdr">
            <asp:DropDownList ID="DEstado2" runat="server" AppendDataBoundItems="true" 
                    DataSourceID="SEstado" DataTextField="nombre" DataValueField="id" CssClass="hide">
                    <asp:ListItem Text="(Ninguno)" Value="0"></asp:ListItem>
            </asp:DropDownList>
            <asp:SqlDataSource ID="SEstado" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre FROM varios WHERE tipo=2 AND estado IS NULL ORDER BY nombre">
            </asp:SqlDataSource><asp:Label runat="server" ID="LEstadoA2" Text="Automático"></asp:Label></td>
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

