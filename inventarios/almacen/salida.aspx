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
    $(function() {
        if (hasStorage) {
          if(<%= (bool)ViewState["clean"] ? "true" : "false" %>) {
                localStorage.salida=null;
            } else {
            $("#<%= HProductos.ClientID %>").val(localStorage.salida);
              if($("#<%= HProductos.ClientID %>").val().length) {
                $("#<%= BActualizar.ClientID %>").click();
              }
            }
        }
    });
    addFunctionToStart(function() {

        if (hasStorage) 
          localStorage.salida = $("#<%= HProductos.ClientID %>").val();
        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("si=" + $(this).val());
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
                $("#<%= TProducto.ClientID %>").val(ui.item.producto);
                $("#<%= TUnidad.ClientID %>").val(ui.item.unidad);
                $("#smax").text("Máx: " + ui.item.cantidad + " " + ui.item.unidad);
                $("#tdescripcion").text(ui.item.descripcion);
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
            <td>Fecha<asp:RequiredFieldValidator ControlToValidate="TFecha" ErrorMessage="Por favor ingrese un fecha." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox>
            </td>
            <td></td>
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
            <td colspan="5" class="GridHeader"><asp:HiddenField runat="server" ID="HProductos" />Productos</td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr"><asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td class="tdl"></td>
            <td colspan="4" class="tdr" id="tdescripcion"></td>
        </tr>
        <tr>
            <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ValidationGroup="producto" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="4" class="tdr"><asp:TextBox ID="TProducto" runat="server" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
        </tr>
        <tr class="GridAltRow">
            <td class="tdl"></td>
            <td><span id="smax"></span><asp:RequiredFieldValidator ControlToValidate="TCantidad" ValidationGroup="producto" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td>Lote</td>
            <td>Vence</td>
            <td class="tdr"><asp:CheckBox runat="server" ID="CAutomatico" Text="Automático" Checked="true" /></td>
        </tr>
        <tr>
            <td class="tdl">Cantidad</td>
            <td><asp:TextBox ID="TCantidad" runat="server" CssClass="float"></asp:TextBox></td>
            <td><asp:TextBox ID="TLote" runat="server" CssClass="tlote hide"></asp:TextBox><span id="automatico1">Automático</span></td>
            <td><asp:TextBox ID="TVencimiento" runat="server" CssClass="tvence hide"></asp:TextBox><span id="automatico2">Automático</span>
            <asp:TextBox ID="TUnidad" runat="server" CssClass="hide"></asp:TextBox></td>
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
                    SelectCommand="SELECT salida_items{0}.id,salida_items{0}.producto,salida_items{0}.cantidad,salida_items{0}.lote
                            ,IF(salida_items{0}.vence='0000-00-00',NULL,salida_items{0}.vence) AS vence
                            ,producto.nombre,producto.unidad,producto.codigo,salida_items{0}.ubicacion,IFNULL(ubicacion.nombre,'(Ninguna)') AS nubicacion
                            FROM salida_items{0}
                            LEFT JOIN producto ON (producto.id=salida_items{0}.producto)
                            LEFT JOIN ubicacion ON (ubicacion.id=salida_items{0}.ubicacion)
                            WHERE salida_items{0}.salida=@salida;" 
                    InsertCommand="INSERT INTO salida_items{0} (salida,producto,cantidad,lote,vence,ingreso,ubicacion) VALUES (@rsalida, @producto,@cantidad,@lote,@vence,SYSDATE(),@ubicacion)"
                    UpdateCommand="UPDATE salida_items{0} SET producto=@producto,cantidad=@cantidad,lote=@lote,vence=@vence,ubicacion=@ubicacion WHERE id=@id"
                    DeleteCommand="DELETE FROM salida_items{0} WHERE id=@id" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="TNumero" Name="salida" PropertyName="Text" />
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

