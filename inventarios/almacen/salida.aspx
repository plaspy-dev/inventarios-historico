<%@ Page Title="Salida" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="salida.aspx.cs" Inherits="almacen_salida" %>

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
    $(function() {
        if (hasStorage) {
          if(<%= (bool)ViewState["clean"] ? "true" : "false" %>) {
                localStorage.salida=null;
            } else {
            $("#<%= HProductos.ClientID %>").val(localStorage.salida);
              if($("#<%= HProductos.ClientID %>").val().length) {
                if(confirm("¿Se ha encontrado una salida sin terminar desea continuarla?")) {
                    $("#<%= BActualizar.ClientID %>").click();
                } else {
                    localStorage.salida=null;
                    $("#<%= HProductos.ClientID %>").val("");
                }
              }
            }
        }
    });
    addFunctionToStart(function() {

        if (hasStorage) 
          localStorage.salida = $("#<%= HProductos.ClientID %>").val();
        
        $("#<%= BAnular.ClientID %>").click(function() { return confirm('¿Esta seguro de anular este documento?. Esta acción no se puede deshacer.'); });
        $("#<%= BAceptar.ClientID %>").click(function() { window.onbeforeunload = null; });
        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("si=" + $(this).val());
        });
        $("#<%= CAutomatico.ClientID %>").change(function() {
            if($(this).is(':checked')) {    
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>,#<%= TSerial.ClientID %>,#<%= TEstado.ClientID %>").hide();
                $("#<%= LLoteA.ClientID %>,#<%= LVencimientoA.ClientID %>,#<%= LSerialA.ClientID %>,#<%= LEstadoA.ClientID %>").show();
            } else { 
                $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>,#<%= TSerial.ClientID %>,#<%= TEstado.ClientID %>").show();
                $("#<%= LLoteA.ClientID %>,#<%= LVencimientoA.ClientID %>,#<%= LSerialA.ClientID %>,#<%= LEstadoA.ClientID %>").hide();
            }
            $("#<%= LLote.ClientID %>,#<%= LVencimiento.ClientID %>,#<%= LSerial.ClientID %>,#<%= LEstado.ClientID %>").show();
            $("#<%= TProductoID.ClientID %>").val("");
            $("#tdescripcion").text("");
            $("#<%= TProducto.ClientID %>").val("").focus();
        });
        if(iniciando)
            $("#<%= CAutomatico.ClientID %>").change();
        iniciando=false;
        $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>,#<%= TSerial.ClientID %>,#<%= TEstado.ClientID %>").attr("readonly", true);
        $("#<%= TTercero.ClientID %>").autocomplete({
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
                            item.label= item.value;
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

                $("#<%= TTerceroID.ClientID %>").val(ui.item.item);
                $("#<%= TTercero.ClientID %>").val(ui.item.value);
                return false;
            }
        });
        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProductoSalida",
                    data: "{ 'almacen': '" + <%= ViewState["almacen"] %> + "', 'auto': " + $("#<%= CAutomatico.ClientID %>").is(':checked') + ",'q': '" + request.term + "','productos': '" + $("#<%= HProductos.ClientID %>").val() + "', 't': '" + $("#<%= TTerceroID.ClientID %>").val() + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label= item.ProductoCompleto;
                            if(!$("#<%= CAutomatico.ClientID %>").is(':checked'))
                                item.id= item.id +";" + item.lote +";" + item.vence + ";" + item.estado + ";" + item.serial;
                            return item;
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
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                $("#smax").text("Máx: " + ui.item.cantidad + " " + ui.item.unidad);
                var descripcion= ui.item.Empresa;
                if(ui.item.Empresa!="" && ui.item.descripcion!="")
                    descripcion+=": ";
                descripcion+= " " + ui.item.descripcion;
                $("#tdescripcion").text(descripcion);
                $("#<%= TValor.ClientID %>").numeric("value",ui.item.Valor);
                
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
                
                try { ValidatorEnable($("#<%= REntero.ClientID %>")[0], ui.item.nNumero==1); } catch (e) {}
                try { ValidatorEnable($("#<%= RDecimal.ClientID %>")[0], ui.item.nNumero!=1); } catch (e) {}
                return false;
            }
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
<asp:UpdatePanel runat="server" ID="UProducto" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Panel runat="server" ID="PAnulado" Visible="false" CssClass="anulado">Anulada</asp:Panel>
    <asp:HiddenField runat="server" ID="HId" />
    <table class="tablecollapse">
        <tr>
            <td colspan="5" align="right"><asp:HyperLink runat="server" ID="HEntrada"></asp:HyperLink></td>
        </tr>
        <tr>
            <td>Número</td>
            <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
            <td></td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>Fecha<asp:RequiredFieldValidator ControlToValidate="TFecha" ErrorMessage="Por favor ingrese un fecha." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox>
            </td>
            <td><asp:Label runat="server" ID="LEntrega" Text="Entrega"></asp:Label><asp:RangeValidator ControlToValidate="TEntrega" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="REntrega" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
            <td><asp:TextBox runat="server" ID="TEntrega" CssClass="fecha"></asp:TextBox>
            <td></td>
        </tr>
        <tr>
            <td>Cliente</td>
            <td colspan="3"><asp:TextBox runat="server" ID="TTercero" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TTerceroID" CssClass="hide"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="2">Documento equivalente<asp:RequiredFieldValidator ControlToValidate="TDocumento" ErrorMessage="Por favor ingrese un documento." ID="RequiredFieldValidator5" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="2"><asp:TextBox runat="server" ID="TDocumento" Width="98%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="tdb">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader"><asp:HiddenField runat="server" ID="HProductos" EnableViewState="false" /><asp:HiddenField runat="server" ID="HUsado" />Productos</td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr"><asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td class="tdl"><asp:CheckBox runat="server" ID="CAutomatico" Text="Automático" Checked="true" /></td>
            <td align="right"><asp:Label runat="server" ID="LLote" Text="Lote: "></asp:Label></td>
            <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LLoteA" Text="Automático"></asp:Label></td>
            <td align="right"><asp:Label runat="server" ID="LVencimiento" Text="Vence: "></asp:Label></td>
            <td class="tdr"><asp:TextBox ID="TVencimiento" runat="server" CssClass="tvence hide"></asp:TextBox><asp:Label runat="server" ID="LVencimientoA" Text="Automático"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tdl"></td>
            <td align="right"><asp:Label runat="server" ID="LSerial" Text="Serial:"></asp:Label></td>
            <td><asp:TextBox runat="server" ID="TSerial" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LSerialA" Text="Automático"></asp:Label></td>
            <td align="right"><asp:Label runat="server" ID="LEstado" Text="Estado:"></asp:Label></td>
            <td class="tdr"><asp:TextBox ID="TEstado" runat="server" CssClass="tlote hide"></asp:TextBox><asp:Label runat="server" ID="LEstadoA" Text="Automático"></asp:Label></td>
        </tr>
        <tr class="GridAltRow">
            <td class="tdl"><span id="smax"></span></td>
            <td colspan="4" class="tdr" id="tdescripcion"></td>
        </tr>
        <tr class="GridAltRow">
            <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ValidationGroup="producto" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="4" class="tdr"><asp:TextBox ID="TProducto" runat="server" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Cantidad<asp:RequiredFieldValidator ControlToValidate="TCantidad" ValidationGroup="producto" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
            <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
            <td><asp:Label runat="server" ID="LValor" Text="Precio U."></asp:Label>
            <asp:RequiredFieldValidator ControlToValidate="TValor" ValidationGroup="producto" ErrorMessage="Por favor ingrese un precio unitario." ID="RValor" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ValidationExpression="^[+-]?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]+)?$" ID="RValorF" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TValor" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
            <td><asp:TextBox runat="server" ID="TValor" CssClass="dinero"></asp:TextBox></td>
            <td class="tdr">
                <asp:Button ID="BAgregar" runat="server" Text="Agregar" ValidationGroup="producto" onclick="BAgregar_Click" />
                <asp:Button ID="BActualizar" runat="server" Text="Actualizar" CssClass="hide" CausesValidation="false" onclick="BActualizar_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdb tdr">
                <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="IdGrid,id,ubicacion,estado" 
                    onselectedindexchanged="GProductos_SelectedIndexChanged" ShowFooter="true"
                    EmptyDataText="No hay productos" onrowdeleting="GProductos_RowDeleting" >
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
                        <asp:BoundField DataField="Valor" HeaderText="Precio U." SortExpression="Valor" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="ValorTotal" HeaderText="Precio T." SortExpression="ValorTotal" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                        <asp:BoundField DataField="vence" HeaderText="Vence" SortExpression="vence" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="serial" HeaderText="Serial" SortExpression="serial" />
                        <asp:BoundField DataField="NombreUbicacion" HeaderText="Ubicación" SortExpression="NombreUbicacion" />
                        <asp:BoundField DataField="szEstado" HeaderText="Estado" SortExpression="szEstado" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="5">&nbsp;</td>
        </tr>
        <tr>
            <td>Observaciones</td>
            <td colspan="4"><asp:TextBox runat="server" ID="TObservaciones" TextMode="MultiLine" Width="99%"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="5"><asp:ValidationSummary runat="server" ID="ValidationSumary1" /><asp:ValidationSummary ValidationGroup="producto" runat="server" ID="ValidationSummary2" /></td>
        </tr>
        <tr>
            <td><asp:Button runat="server" ID="BAnular" Text="Anular" Visible="false" onclick="BAceptar_Click"/></td>
            <td colspan="4" align="right">
                <asp:Button runat="server" ID="BAceptar" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
            </td>
        </tr>
    </table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>

