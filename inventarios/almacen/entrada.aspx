<%@ Page Title="Entrada" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="entrada.aspx.cs" Inherits="almacen_entrada" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .tlote 
    {
        width: 100px;
    }
    .tubicacion {}
</style>
<script type="text/javascript">
    $(function() {
        if (hasStorage) {
          if(<%= (bool)ViewState["clean"] ? "true" : "false" %>) {
                localStorage.entrada=null;
            } else {
            $("#<%= HProductos.ClientID %>").val(localStorage.entrada);
              if($("#<%= HProductos.ClientID %>").val().length) {
                if(confirm("¿Se ha encontrado una entrada sin terminar desea continuarla?")) {
                    $("#<%= BActualizar.ClientID %>").click();
                } else {localStorage
                    .salida=null;
                    $("#<%= HProductos.ClientID %>").val("");
                }
              }
            }
        }
    });
    addFunctionToStart(function() {
        if (hasStorage) 
          localStorage.entrada = $("#<%= HProductos.ClientID %>").val();
          
        $("#<%= BAnular.ClientID %>").click(function() { return confirm('¿Esta seguro de anular este documento?. Esta acción no se puede deshacer.'); });
        $("#<%= BAceptar.ClientID %>").click(function() { window.onbeforeunload = null; });
                  
          $(".tubicacion").autocomplete({
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

                $("#"+ $(this).attr("id")+ "ID").val(ui.item.item);
                $(this).val(ui.item.value);
                return false;
            },
                search: function(event, ui) { $("#" + $(this).attr("id") + "ID").val("0"); }
        }).change(function() { 
            if ($(this).val().length == 0) 
                $("#" + $(this).attr("id") + "ID").val("0"); 
            else if($("#" + $(this).attr("id") + "ID").val()=="0")
                $("#" + $(this).attr("id") + "ID").val("-1");
        });
        
          $("#<%= TTercero.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetTercero",
                    data: "{ 'q': '" + request.term + "', tipo: 0 }",
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
            search: function() {
                $("#<%= TTerceroID.ClientID %>").val("");
            },
            select: function(event, ui) {

                $("#<%= TTerceroID.ClientID %>").val(ui.item.item);
                $("#<%= TTercero.ClientID %>").val(ui.item.value);
                return false;
            }
        }).change(function() { 
            if($(this).val().length==0)
                $("#<%= TTerceroID.ClientID %>").val("");
        });
        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': '" + $("#<%= TTerceroID.ClientID %>").val() + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label= item.ProductoCompleto;
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

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                var descripcion= ui.item.Empresa;
                if(ui.item.Empresa!="" && ui.item.descripcion!="")
                    descripcion+=": ";
                descripcion+= " " + ui.item.descripcion;
                $("#tdescripcion").text(descripcion);
                if (ui.item.unidad && ui.item.unidad != "")
                    $("#sunidad").text("(" + ui.item.unidad + ")");
                else $("#sunidad").text("");
                $("#<%= TCosto.ClientID %>").numeric("value",ui.item.Costo);
                
                if(ui.item.DiasV>0) {
                    var fecha = new Date();
                    fecha.setDate(fecha.getDate()+ui.item.DiasV);
                    $("#<%= TVencimiento.ClientID %>").datepicker("setDate", fecha);
                } else {
                    $("#<%= TVencimiento.ClientID %>").val("");
                }
                var pad = "00";
                var almacen= "<%= ((ing)Master).GetAlmacen() %>";
                
                $("#<%= TLote.ClientID %>").val(
                    ui.item.LoteTemporal
                    .replace("%codigoalmacen%", pad.substring(0, pad.length - almacen.length) + almacen)
                );

                ui.item.nVence>0 ? $("#<%= TVencimiento.ClientID %>,#<%= LVencimiento.ClientID %>").show() : $("#<%= TVencimiento.ClientID %>,#<%= LVencimiento.ClientID %>").hide();
                ui.item.nLote>0 ? $("#<%= TLote.ClientID %>,#<%= LLote.ClientID %>").show() : $("#<%= TLote.ClientID %>,#<%= LLote.ClientID %>").hide();
                ui.item.nSerial>0 ? $("#<%= TSerial.ClientID %>,#<%= LSerial.ClientID %>").show() : $("#<%= TSerial.ClientID %>,#<%= LSerial.ClientID %>").hide();
                ui.item.nEstado>0 ? $("#<%= DEstado.ClientID %>,#<%= LEstado.ClientID %>").show() : $("#<%= DEstado.ClientID %>,#<%= LEstado.ClientID %>").hide();
                
                try { ValidatorEnable($("#<%= RVencimiento.ClientID %>")[0], ui.item.nVence==1); } catch (e) {}
                try { ValidatorEnable($("#<%= RLote.ClientID %>")[0], ui.item.nLote==1); } catch (e) {}
                try { ValidatorEnable($("#<%= RSerial.ClientID %>")[0], ui.item.nSerial==1); } catch (e) {}
                try { ValidatorEnable($("#<%= REstado.ClientID %>")[0], ui.item.nEstado==1); } catch (e) {}
                try { ValidatorEnable($("#<%= REntero.ClientID %>")[0], ui.item.nNumero==1); } catch (e) {}
                try { ValidatorEnable($("#<%= RDecimal.ClientID %>")[0], ui.item.nNumero!=1); } catch (e) {}
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
<asp:Panel runat="server" ID="PImprimir" Visible="false">
    <div id="dialog-confirm" title="Imprimir" class="hide"> 
        <p><asp:Label runat="server" ID="LImprimir"></asp:Label></p>
    </div>
    <script type="text/javascript">
        $(function() {
            $("#dialog-confirm").dialog({resizable: false,modal: true,buttons: { "Imprimir": function() { window.open('<%= ViewState["urlprint"] %>', '_blank'); $(this).dialog("close"); }, "Cancelar" : function() {$(this).dialog("close");}}});
        });
    </script>
</asp:Panel>
<asp:UpdatePanel runat="server" ID="UProducto" UpdateMode="Conditional">
<ContentTemplate>
    <asp:Panel runat="server" ID="PAnulado" Visible="false" CssClass="anulado">Anulada</asp:Panel>
    <asp:HiddenField runat="server" ID="HId" />
    <table class="tablecollapse">
        <tr>
            <td colspan="5" align="right"><asp:HyperLink runat="server" ID="HTraslado"></asp:HyperLink></td>
        </tr>
        <tr>
            <td>Número</td>
            <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
            <td>Fecha
            <asp:RequiredFieldValidator ControlToValidate="TFecha" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator>
            </td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>Proveedor<asp:RequiredFieldValidator ControlToValidate="TTerceroID" ErrorMessage="Por favor ingrese un proveedor." ID="RTerceroID" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="3"><asp:TextBox runat="server" ID="TTercero" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TTerceroID" CssClass="hide"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="2">Documento equivalente<asp:RequiredFieldValidator ControlToValidate="TDocumento" ErrorMessage="Por favor ingrese un documento." ID="RDocumento" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="2"><asp:TextBox runat="server" ID="TDocumento" Width="98%"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="5" class="tdb">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader"><asp:HiddenField runat="server" ID="HProductos" EnableViewState="false" /><asp:HiddenField runat="server" ID="HUsado" />Productos</td>
        </tr>
        <tr>
            <td class="tdl"></td>
            <td colspan="4" class="tdr" id="tdescripcion"></td>
        </tr>
        <tr>
            <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ValidationGroup="producto" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="4" class="tdr"><asp:TextBox ID="TProducto" Width="98%" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
        </tr>
        <tr class="GridAltRow">
            <td class="tdl"></td>
            <td><span id="sunidad"></span></td>
            <td><asp:Label runat="server" ID="LCosto" Text="Costo Unitario"></asp:Label>
            <asp:RequiredFieldValidator ControlToValidate="TCosto" ValidationGroup="producto" ErrorMessage="Por favor ingrese un costo unitario." ID="RCosto" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ValidationExpression="^[+-]?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]+)?$" ID="RCostoF" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCosto" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
            <td><asp:Label runat="server" ID="LLote" Text="Lote"></asp:Label><asp:RequiredFieldValidator ControlToValidate="TLote" ValidationGroup="producto" ErrorMessage="Por favor ingrese un lote." ID="RLote" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator></td>
            <td class="tdr">
                <asp:Label runat="server" ID="LVencimiento" Text="Vence"></asp:Label>
                <asp:RequiredFieldValidator ControlToValidate="TVencimiento" ValidationGroup="producto" ErrorMessage="Por favor ingrese una fecha." ID="RVencimiento" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator>
                <asp:RangeValidator ControlToValidate="TVencimiento" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator1" Display="Dynamic" Type="Date">*</asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td class="tdl">Cantidad
                <asp:RequiredFieldValidator ControlToValidate="TCantidad" ValidationGroup="producto" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
            <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
            <td><asp:TextBox runat="server" ID="TCosto" CssClass="dinero"></asp:TextBox></td>
            <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote"></asp:TextBox></td>
            <td class="tdr"><asp:TextBox ID="TVencimiento" runat="server" CssClass="fecha"></asp:TextBox></td>
        </tr>
        <tr class="GridAltRow">
            <td class="tdl">
                &nbsp;</td>
            <td><asp:Label runat="server" ID="LSerial" Text="Serial"></asp:Label><asp:RequiredFieldValidator ControlToValidate="TSerial" ValidationGroup="producto" ErrorMessage="Por favor ingrese un serial." ID="RSerial" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator></td>
            <td><asp:Label runat="server" ID="LUbicacion" Text="Ubicación"></asp:Label><asp:RequiredFieldValidator InitialValue="-1" ControlToValidate="TUbicacionID" ValidationGroup="producto" ErrorMessage="Por favor ingrese una ubicacion valida." ID="RUbicacionO" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator>
            <asp:RequiredFieldValidator InitialValue="0" ControlToValidate="TUbicacionID" ValidationGroup="producto" ErrorMessage="Por favor ingrese una ubicacion." ID="RUbicacion" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator></td>
            <td><asp:Label runat="server" ID="LEstado" Text="Estado"></asp:Label><asp:RequiredFieldValidator InitialValue="0" ControlToValidate="DEstado" ValidationGroup="producto" ErrorMessage="Por favor seleccione un estado." ID="REstado" runat="server" Display="Dynamic">*</asp:RequiredFieldValidator></td>
            <td class="tdr">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="tdl">
                &nbsp;</td>
            <td><asp:TextBox CssClass="tlote" runat="server" ID="TSerial"></asp:TextBox></td>
            <td>
                <asp:TextBox runat="server" ID="TUbicacion" CssClass="tlote tubicacion"></asp:TextBox>
                <asp:TextBox runat="server" ID="TUbicacionID" Text="0" CssClass="hide"></asp:TextBox></td>
            <td>
                <asp:DropDownList ID="DEstado" runat="server" AppendDataBoundItems="true" 
                        DataSourceID="SEstado" DataTextField="nombre" DataValueField="id">
                        <asp:ListItem Text="(Ninguno)" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SEstado" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT id, nombre FROM varios WHERE tipo=2 AND estado IS NULL ORDER BY nombre">
                </asp:SqlDataSource>
            </td>
            <td class="tdr">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="tdl"></td>
            <td></td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td class="tdr">
                <asp:Button ID="BAgregar" runat="server" Text="Agregar" ValidationGroup="producto" onclick="BAgregar_Click" />
                <asp:Button ID="BActualizar" runat="server" Text="Actualizar" CssClass="hide" CausesValidation="false" onclick="BActualizar_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="5" align="center" class="tdl tdb tdr">
                <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="IdGrid,id,ubicacion,estado" 
                    onselectedindexchanged="GProductos_SelectedIndexChanged" 
                    EmptyDataText="No hay productos" onrowdeleting="GProductos_RowDeleting" ShowFooter="true" >
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
                        <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" />
                        <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                        <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:F2}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Costo" HeaderText="Costo U." SortExpression="Costo" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="CostoTotal" HeaderText="Costo T." SortExpression="CostoTotal" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                        <asp:BoundField DataField="vence" HeaderText="Vence" SortExpression="vence" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="serial" HeaderText="Serial" SortExpression="serial" />
                        <asp:BoundField DataField="NombreUbicacion" HeaderText="Ubicación" SortExpression="NombreUbicacion" />
                        <asp:TemplateField HeaderText="Ubicación ">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="TUbicacion" CssClass="tlote tubicacion" Text='<%# Eval("NombreUbicacion") %>'></asp:TextBox>
                                <asp:TextBox runat="server" ID="TUbicacionID" CssClass="hide" Text='<%# Eval("ubicacion") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="szEstado" HeaderText="Estado" SortExpression="szEstado" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="5" align="right"><asp:Label runat="server" ID="LTotal"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="5">&nbsp;</td>
        </tr>     
        <tr>
            <td>Observaciones</td>
            <td colspan="4"><asp:TextBox runat="server" ID="TObservaciones" TextMode="MultiLine" Width="99%"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="5"><asp:ValidationSummary runat="server" ID="ValidationSumary1" /><asp:ValidationSummary ValidationGroup="producto" runat="server" ID="ValidationSummary2" />
            <asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label>
            </td>
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