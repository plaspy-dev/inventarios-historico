<%@ Page Title="Datos de proceso" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="proceso.aspx.cs" Inherits="produccion_proceso" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .lote,.porcentaje 
    {
        width: 100px;
        text-align: right;
    }
    .contenedor 
    {
        width: 100px;
    }
    .presentacion 
    {
        width: 100px;
        text-align: right;
    }
</style>
<script type="text/javascript">
    
    var loteTemporal="<%= (string)ViewState["FormatoLote"] %>";
    function generarLote() {

        var fecha = $("#<%= TFechaInicio.ClientID %>").val();
        var partes = fecha.split("/");
        if (partes.length < 3) {
            $("#<%= TLote.ClientID %>").val("");
        } else {
            var pad = "00";
            var almacen = "<%= ((ing)Master).GetAlmacen() %>";
            $("#<%= TLote.ClientID %>").val(
                loteTemporal
                .replace("%dia%", pad.substring(0, pad.length - partes[0].length) + partes[0])
                .replace("%mes%", pad.substring(0, pad.length - partes[1].length) + partes[1])
                .replace("%ano%", partes[2])
                .replace("%anoc%", partes[2].substring(0, 2))
                .replace("%codigoalmacen%", pad.substring(0, pad.length - almacen.length) + almacen)
            );
        }
    }
    
    addFunctionToStart(function() { });
    $(function() {
        $(".presentacion").autoNumeric('init', { vMin: 0, aSep: '', mDec: 8, aPad: false, aSign: ' Kg.', pSign: 's' });
        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("oi=" + $(this).val());
        });
        $("#<%= TLote.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "produccion.asmx/GetOrdenP",
                    data: "{ 'q': '" + request.term + "', 'p': '' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.Nombre;
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
            select: function(event, ui) {

                $.goURL("oi=" + ui.item.Id);
                return false;
            }
        });
        $("#<%= TCantidadUnidad1.ClientID %>,#<%= TCantidadPresentacion1.ClientID %>,#<%= TCantidadUnidad2.ClientID %>,#<%= TCantidadPresentacion2.ClientID %>,#<%= TReproceso.ClientID %>").change(function() {
            var total = $("#<%= TCantidadUnidad1.ClientID %>").val() * $("#<%= TCantidadPresentacion1.ClientID %>").val().replace(" Kg.","");
            total += $("#<%= TCantidadUnidad2.ClientID %>").val() * $("#<%= TCantidadPresentacion2.ClientID %>").val().replace(" Kg.", "");
            $("#tCantidadObtenida").text(total);
            total= $("#<%= LCantidad.ClientID %>").text().replace(" Kg.", "")*1-total;
            total-=$("#<%= TReproceso.ClientID %>").val().replace(" Kg.", "")*1;
            $("#<%= LMerma.ClientID %>").text(total + " Kg.");
        }).change();

        $("#<%= TFechaInicio.ClientID %>").change(generarLote);
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Panel runat="server" ID="PAnulado" Visible="false" CssClass="anulado"><asp:Label runat="server" ID="LAnulado"></asp:Label></asp:Panel>
    <table style="border-collapse: collapse;">
        <tr>
            <td colspan="4" class="GridHeader">Orden de producción</td>
        </tr>
        <tr>
            <td class="tdl">Número</td>
            <td><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
            <td>Lote</td>
            <td class="tdr" align="right"><asp:TextBox runat="server" ID="TLote" CssClass="lote"></asp:TextBox></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="PBordes">
        <tr>
            <td colspan="4" class="tdt"></td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="PAnalisis" Visible="false">
        <tr>
            <td class="tdl">Planeación</td>
            <td align="right"><asp:Label runat="server" ID="LPlaneacion" Font-Bold="true"></asp:Label></td>
            <td>Producción</td>
            <td align="right" class="tdr"><asp:Label runat="server" ID="LProduccion" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr>
            <td class="tdl" nowrap="nowrap">Cantidad programada</td>
            <td align="right"><asp:Label runat="server" ID="LCantidad" Font-Bold="true"></asp:Label></td>
            <td></td>
            <td class="tdr"></td>
        </tr>
        <tr>
            <td colspan="4" class="tda"><asp:Label runat="server" ID="LProducto" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="4">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4" class="GridHeader">Proceso</td>
        </tr>
        <tr>
            <td class="tdl">Marmita</td>
            <td><asp:DropDownList ID="DMarmita" runat="server" DataSourceID="SMarmita" AppendDataBoundItems="true"
                    DataTextField="nombre" DataValueField="id">
                    <asp:ListItem Text="(Ninguna)" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SMarmita" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT id, nombre FROM varios WHERE tipo=1001 AND estado IS NULL ORDER BY nombre">
                </asp:SqlDataSource></td>
            <td>Contenedor</td>
            <td class="tdr"><asp:TextBox runat="server" ID="TContenedor" CssClass="contenedor"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Fecha<asp:RequiredFieldValidator ControlToValidate="TFechaInicio" ErrorMessage="Por favor ingrese un inicio." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TFechaInicio" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator>
            </td>
            <td><asp:TextBox runat="server" ID="TFechaInicio" CssClass="fecha"></asp:TextBox></td>
            <td>Operarios<asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TOperariosProceso" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
            <td class="tdr"><asp:TextBox runat="server" ID="TOperariosProceso" CssClass="entero"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Inicio<asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="TInicioProceso" ErrorMessage="Por favor ingrese una hora valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
            <td><asp:TextBox runat="server" ID="TInicioProceso" CssClass="hora"></asp:TextBox></td>
            <td>Fin<asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TFinProceso" ErrorMessage="Por favor ingrese una hora valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
            <td class="tdr"><asp:TextBox runat="server" ID="TFinProceso" CssClass="hora"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="4" class="GridHeader">Empaque</td>
        </tr>
        <tr>
            <td class="tdl">Fecha
            <asp:RangeValidator ControlToValidate="TFechaEmpaque" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator1" Display="Dynamic" Type="Date">*</asp:RangeValidator>
            </td>
            <td><asp:TextBox runat="server" ID="TFechaEmpaque" CssClass="fecha"></asp:TextBox></td>
            <td>Operarios<asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="RegularExpressionValidator5" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TOperariosEmpaque" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
            <td class="tdr"><asp:TextBox runat="server" ID="TOperariosEmpaque" CssClass="entero"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Inicio<asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="TInicioEmpaque" ErrorMessage="Por hora ingrese una fecha valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
            <td><asp:TextBox runat="server" ID="TInicioEmpaque" CssClass="hora"></asp:TextBox></td>
            <td>Fin<asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="TFinEmpaque" ErrorMessage="Por favor ingrese una hora valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
            <td class="tdr"><asp:TextBox runat="server" ID="TFinEmpaque" CssClass="hora"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="4" class="GridHeader">Cantidad Obtenida</td>
        </tr>
        <tr>
            <td class="tdl">Unidades 1<asp:RegularExpressionValidator ControlToValidate="TCantidadUnidad1" ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
            <td><asp:TextBox runat="server" ID="TCantidadUnidad1" CssClass="float"></asp:TextBox></td>
            <td>Presentación 1<asp:RegularExpressionValidator ControlToValidate="TCantidadPresentacion1" ValidationExpression="^[0-9]*[.]?[0-9]+ Kg.$" ID="RegularExpressionValidator6" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
            <td class="tdr"><asp:TextBox runat="server" ID="TCantidadPresentacion1" CssClass="presentacion"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Unidades 2<asp:RegularExpressionValidator ControlToValidate="TCantidadUnidad2" ValidationExpression="[0-9]*[.]?[0-9]+" ID="RegularExpressionValidator4" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
            <td><asp:TextBox runat="server" ID="TCantidadUnidad2" CssClass="float"></asp:TextBox></td>
            <td>Presentación 2<asp:RegularExpressionValidator ControlToValidate="TCantidadPresentacion2" ValidationExpression="^[0-9]*[.]?[0-9]+ Kg.$" ID="RegularExpressionValidator7" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
            <td class="tdr"><asp:TextBox runat="server" ID="TCantidadPresentacion2" CssClass="presentacion"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Reproceso<asp:RegularExpressionValidator ControlToValidate="TReproceso" ValidationExpression="^[0-9]*[.]?[0-9]+ Kg.$" ID="RegularExpressionValidator9" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
            <td><asp:TextBox runat="server" ID="TReproceso" CssClass="presentacion"></asp:TextBox> </td>
            <td>Merma</td>
            <td class="tdr"><asp:Label runat="server" ID="LMerma" Text="0 Kg."></asp:Label></td>
        </tr>
        <tr>
            <td colspan="4" class="tdl tdr" align="right">Cantidad obtenida: <span id="tCantidadObtenida" style="font-size: 20px;font-weight: bold; white-space: nowrap">0</span> Kg.</td>
        </tr>
        <tr>
            <td class="tdl tdb">Observaciones</td>
            <td colspan="3" class="tdr tdb"><asp:TextBox runat="server" id="TObservaciones" Width="99%"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="4">&nbsp;
                <asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="4" align="right">
                <asp:Button runat="server" ID="BAceptar" Text="Aceptar" OnClick="BAceptar_Clicked" />
                <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" OnClick="BCancelar_Clicked" />
            </td>
        </tr>
        </asp:PlaceHolder>
    </table>
</asp:Content>