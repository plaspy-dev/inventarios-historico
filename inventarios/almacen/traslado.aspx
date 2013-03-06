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
    var iniciando=true;
    $(function() {
        if (hasStorage) {
          if(<%= (bool)ViewState["clean"] ? "true" : "false" %>) {
            localStorage.traslado=null;
            } else {
            $("#<%= HProductos.ClientID %>").val(localStorage.traslado);
              if($("#<%= HProductos.ClientID %>").val().length) {
                if(confirm("¿Se ha encontrado un traslado sin terminar desea continuarla?")) {
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
          localStorage.traslado = $("#<%= HProductos.ClientID %>").val();
        $("#<%= BAceptar.ClientID %>").click(function() { window.onbeforeunload = null; });
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
            $("#<%= LLote.ClientID %>,#<%= LVencimiento.ClientID %>").show();
            $("#<%= TProductoID.ClientID %>").val("");
            $("#tdescripcion").text("");
            $("#<%= TProducto.ClientID %>").val("").focus();
        });
        if(iniciando)
            $("#<%= CAutomatico.ClientID %>").change();
        iniciando=false;
        $("#<%= TLote.ClientID %>,#<%= TVencimiento.ClientID %>").attr("readonly", true);
        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProductoDisponible",
                    data: "{ 'almacen': '" + <%= ViewState["almacen"] %> + "', 'auto': " + $("#<%= CAutomatico.ClientID %>").is(':checked') + ",'q': '" + request.term + "','productos': '" + $("#<%= HProductos.ClientID %>").val() + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label= item.ProductoCompleto;
                            if(!$("#<%= CAutomatico.ClientID %>").is(':checked'))
                                item.id= item.id +";" + item.lote +";" + item.vence;
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
                if(!$("#<%= CAutomatico.ClientID %>").is(':checked')) {
                    $("#<%= TLote.ClientID %>").val(ui.item.lote);
                    $("#<%= TVencimiento.ClientID %>").val(ui.item.vence);
                    ui.item.nVence>0 ? $("#<%= TVencimiento.ClientID %>,#<%= LVencimiento.ClientID %>").show() : $("#<%= TVencimiento.ClientID %>,#<%= LVencimiento.ClientID %>").hide();
                    ui.item.nLote>0 ? $("#<%= TLote.ClientID %>,#<%= LLote.ClientID %>").show() : $("#<%= TLote.ClientID %>,#<%= LLote.ClientID %>").hide();
                } else {
                    $("#<%= TLote.ClientID %>").val("");
                    $("#<%= TVencimiento.ClientID %>").val("");
                    ui.item.nLote>0 ? $("#automatico1,#<%= LLote.ClientID %>").show() : $("#automatico1,#<%= LLote.ClientID %>").hide();
                    ui.item.nVence>0 ? $("#automatico2,#<%= LVencimiento.ClientID %>").show() : $("#automatico2,#<%= LVencimiento.ClientID %>").hide();
                }
                ValidatorEnable($("#<%= REntero.ClientID %>")[0], ui.item.nNumero==1);
                ValidatorEnable($("#<%= RDecimal.ClientID %>")[0], ui.item.nNumero!=1);
                return false;
            }
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
            <td colspan="5" align="right"><asp:HyperLink runat="server" ID="HEntrada"></asp:HyperLink></td>
        </tr>
        <tr>
            <td>Número</td>
            <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
            <td>Destino
            <asp:RequiredFieldValidator ControlToValidate="DAlmacen" ErrorMessage="Por favor seleccione un destino." InitialValue="-1" ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            </td>
            <td>
                <asp:DropDownList ID="DAlmacen" runat="server" AppendDataBoundItems="true" 
                    DataSourceID="SAlmacen" DataTextField="nombre" DataValueField="id">
                    <asp:ListItem Text="(Ninguno)" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="(Ajustes)" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SAlmacen" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT id, nombre FROM almacen WHERE id!=@almacen AND estado IS NULL ORDER BY nombre">
                </asp:SqlDataSource>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>Envío<asp:RequiredFieldValidator ControlToValidate="TFecha" ErrorMessage="Por favor ingrese un fecha." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox>
            </td>
            <td>Llegada<asp:RangeValidator ControlToValidate="TLlegada" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator1" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
            <td align="right"><asp:TextBox runat="server" ID="TLlegada" CssClass="fecha"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">Documento equivalente</td>
            <td colspan="2"><asp:TextBox runat="server" ID="TDocumento" Width="98%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="tdb">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader">Productos</td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr"><asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td class="tdl"><asp:CheckBox runat="server" ID="CAutomatico" Text="Automático" Checked="true" /></td>
            <td align="right"><asp:Label runat="server" ID="LLote" Text="Lote: "></asp:Label></td>
            <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote hide"></asp:TextBox><span id="automatico1">Automático</span></td>
            <td align="right"><asp:Label runat="server" ID="LVencimiento" Text="Vence: "></asp:Label></td>
            <td class="tdr"><asp:TextBox ID="TVencimiento" runat="server" CssClass="tvence hide"></asp:TextBox><span id="automatico2">Automático</span>
            </td>
        </tr>
        <tr class="GridAltRow">
            <td class="tdl"><asp:HiddenField runat="server" ID="HProductos" EnableViewState="false" /></td>
            <td colspan="4" class="tdr" id="tdescripcion"></td>
        </tr>
        <tr class="GridAltRow">
            <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ValidationGroup="producto" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="4" class="tdr"><asp:TextBox Width="98%" ID="TProducto" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Cantidad<asp:RequiredFieldValidator ControlToValidate="TCantidad" ValidationGroup="producto" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="producto" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
            <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
            <td colspan="2"><span id="smax"></span></td>
            <td class="tdr">
                <asp:Button ID="BAgregar" runat="server" Text="Agregar" ValidationGroup="producto" onclick="BAgregar_Click" />
                <asp:Button ID="BActualizar" runat="server" Text="Actualizar" CssClass="hide" CausesValidation="false" onclick="BActualizar_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="5" align="center" class="tdl tdb tdr">
                <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="id,producto,ubicacion" DataSourceID="SProductos" 
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
                        <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:F2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                        <asp:BoundField DataField="vence" HeaderText="Vence" SortExpression="vence" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="nubicacion" HeaderText="Ubicación" SortExpression="nubicacion" />
                    </Columns>
                </asp:GridView>
                <sdg:customdatasource ID="SProductos" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    SelectCommand="SELECT traslado_items{0}.id,traslado_items{0}.producto,traslado_items{0}.cantidad,traslado_items{0}.lote
                            ,IF(traslado_items{0}.vence='0000-00-00',NULL,traslado_items{0}.vence) AS vence
                            ,producto.nombre,producto.unidad,producto.codigo,traslado_items{0}.ubicacion,IFNULL(ubicacion.nombre,'(Ninguna)') AS nubicacion
                            FROM traslado_items{0}
                            LEFT JOIN producto ON (producto.id=traslado_items{0}.producto)
                            LEFT JOIN ubicacion ON (ubicacion.id=traslado_items{0}.ubicacion)
                            WHERE traslado_items{0}.traslado=@traslado;" 
                    InsertCommand="INSERT INTO traslado_items{0} (traslado,producto,cantidad,lote,vence,ingreso,ubicacion) VALUES (@rtraslado, @producto,@cantidad,@lote,@vence,SYSDATE(),@ubicacion)"
                    UpdateCommand="UPDATE traslado_items{0} SET producto=@producto,cantidad=@cantidad,lote=@lote,vence=@vence,ubicacion=@ubicacion WHERE id=@id"
                    DeleteCommand="DELETE FROM traslado_items{0} WHERE id=@id" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="TNumero" Name="traslado" PropertyName="Text" />
                    </SelectParameters>
                </sdg:customdatasource>
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
            <td colspan="5"><asp:ValidationSummary runat="server" ID="ValidationSumary1" /><asp:ValidationSummary ValidationGroup="producto" runat="server" ID="ValidationSummary2" />
            </td>
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

