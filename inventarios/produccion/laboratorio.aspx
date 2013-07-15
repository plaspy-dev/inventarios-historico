<%@ Page Title="Análisis físico químico" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="laboratorio.aspx.cs" Inherits="produccion_laboratorio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .lote,.porcentaje 
    {
        width: 100px;
        text-align: right;
    }
</style>
<script type="text/javascript">
    $(function() {
        $(".porcentaje").autoNumeric('init', { vMin: 0, aSep: '', mDec: 2, aPad: false, aSign: '%', pSign: 's' });
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
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
        <asp:Panel runat="server" ID="PBordes">
        <tr>
            <td colspan="4" class="tdt"></td>
        </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="PAnalisis" Visible="false">
        <tr>
            <td class="tdl">Planeación</td>
            <td align="right"><asp:Label runat="server" ID="LPlaneacion"></asp:Label></td>
            <td>Producción</td>
            <td align="right" class="tdr"><asp:Label runat="server" ID="LProduccion"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="4" class="tdb tdl tdr"><asp:Label runat="server" ID="LProducto"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="4">&nbsp;</td>
        </tr>
        </asp:Panel>
    </table>
    <table style="border-collapse: collapse;" runat="server" id="TAnalisis" visible="false">
            <tr>
                <td colspan="4" class="GridHeader">Análisis físico químico</td>
            </tr>
            <tr>
                <td class="tdl">Fecha<asp:RequiredFieldValidator ValidationGroup="analisis" ControlToValidate="TFecha" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                <asp:RangeValidator ValidationGroup="analisis" ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
                <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox></td>
                <td></td>
                <td class="tdr"></td>
            </tr>
            <tr>
                <td class="tdl">Peso Lavado<asp:RegularExpressionValidator ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+%" ID="RegularExpressionValidator1" runat="server" ErrorMessage="Por favor ingrese un porcentaje." ControlToValidate="TPesoLavado" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                </td>
                <td><asp:TextBox ID="TPesoLavado" runat="server" CssClass="porcentaje"></asp:TextBox></td>
                <td>Peso Escurrido<asp:RegularExpressionValidator ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+%" ID="RegularExpressionValidator2" runat="server" ErrorMessage="Por favor ingrese un porcentaje." ControlToValidate="TPesoEscurrido" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
                <td class="tdr"><asp:TextBox ID="TPesoEscurrido" runat="server" CssClass="porcentaje"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="tdl">Brix<asp:RequiredFieldValidator ControlToValidate="TBrix" ErrorMessage="Por favor ingrese un brix." ValidationGroup="analisis" ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="TBrix" ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
                <td><asp:TextBox ID="TBrix" runat="server" CssClass="float"></asp:TextBox></td>
                <td>Acidez<asp:RequiredFieldValidator ControlToValidate="TAcidez" ErrorMessage="Por favor ingrese una acidez." ValidationGroup="analisis" ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+%" ID="RegularExpressionValidator3" runat="server" ErrorMessage="Por favor ingrese un porcentaje." ControlToValidate="TAcidez" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
                <td class="tdr"><asp:TextBox ID="TAcidez" runat="server" CssClass="porcentaje"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="tdl">Viscosidad<asp:RequiredFieldValidator ControlToValidate="TViscosidad" ErrorMessage="Por favor ingrese una viscosidad." ValidationGroup="analisis" ID="RequiredFieldValidator5" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ControlToValidate="TViscosidad" ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
                <td><asp:TextBox ID="TViscosidad" runat="server" CssClass="float"></asp:TextBox></td>
                <td>Consistencia
                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="TConsistencia" ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
                <td class="tdr"><asp:TextBox ID="TConsistencia" runat="server" CssClass="float"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="tdl">PH<asp:RequiredFieldValidator ControlToValidate="TPH" ErrorMessage="Por favor ingrese un PH." ValidationGroup="analisis" ID="RequiredFieldValidator8" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator7" ControlToValidate="TPH" ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
                <td><asp:TextBox ID="TPH" runat="server" CssClass="float"></asp:TextBox></td>
                <td>Densidad
                <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="TDensidad" ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
                <td class="tdr"><asp:TextBox ID="TDensidad" runat="server" CssClass="float"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="tdl">Chart-Color</td>
                <td><asp:TextBox ID="TChartColor" runat="server" CssClass="lote"></asp:TextBox></td>
                <td></td>
                <td class="tdr"></td>
            </tr>
            <tr>
                <td class="tdl">Observaciones</td>
                <td colspan="3" class="tdr"><asp:TextBox runat="server" ID="TObservaciones" Width="99%"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="tdl tdr" colspan="4">
                    <asp:ValidationSummary runat="server" ValidationGroup="analisis" />
                </td>
            </tr>
            <tr>
                <td colspan="4" align="right" class="tdl tdr">
                    <asp:Button runat="server" ID="BAgregar" Text="Agregar" OnClick="BAgregar_Clicked" ValidationGroup="analisis" />
                    <asp:Button runat="server" ID="BCancelarAgregar" Text="Cancelar" CausesValidation="false" Visible="false" OnClick="BCancelarAgregar_Clicked" />
                </td>
            </tr>
            <tr>
                <td colspan="4" class="tdl tdr tdb" align="center">
                    <asp:HiddenField runat="server" ID="HAnalisis" />
                    <asp:GridView ID="GAnalisis" runat="server" AutoGenerateColumns="False" 
                        DataKeyNames="IdGrid" OnRowDeleting="GAnalisis_RowDeleting" OnSelectedIndexChanged="GAnalisis_SelectedIndexChanged"
                        EmptyDataText="No hay análisis" >
                        <RowStyle CssClass="GridRow"></RowStyle>
                        <FooterStyle CssClass="GridFooter"></FooterStyle>
                        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                        <Columns>
                            <asp:CommandField ButtonType="Image" SelectImageUrl="~/img/edit.gif" ShowSelectButton="True" />
                            <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                            <asp:BoundField DataField="Fecha" HeaderText="Fecha" SortExpression="Fecha" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="PesoLavado" HeaderText="Peso lavado" SortExpression="PesoLavado" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="PesoEscurrido" HeaderText="Peso escurrido" SortExpression="PesoLavado" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="Brix" HeaderText="Brix" SortExpression="Brix" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="Acidez" HeaderText="Acidez" SortExpression="Acidez" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="Viscosidad" HeaderText="Viscosidad" SortExpression="Viscosidad" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="Consistencia" HeaderText="Consistencia" SortExpression="Consistencia" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="PH" HeaderText="PH" SortExpression="PH" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="Densidad" HeaderText="Densidad" SortExpression="Densidad" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="ChartColor" HeaderText="Chart-Color" SortExpression="ChartColor" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" SortExpression="Observaciones" />
                        </Columns>
                    </asp:GridView>
                </td>
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
            </table>
</asp:Content>

