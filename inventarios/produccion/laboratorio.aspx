<%@ Page Title="Laboratorio" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="laboratorio.aspx.cs" Inherits="produccion_laboratorio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .lote,.porcentaje 
    {
        width: 100px;
        text-align: right;
    }
</style>
<script type="text/javascript">
    function validar(textbox, minimo, maximo) {
        if ($(textbox).val() == "") {
            $(textbox).css("background-color", "").css("color", "#000");
            return;
        }
        var valor = parseFloat($(textbox).val());
        if (valor < minimo)
            $(textbox).css("background-color", "#FFFF00").css("color", "#000");
        else if (valor > maximo)
            $(textbox).css("background-color", "#FF0000").css("color", "#FFF");
        else $(textbox).css("background-color", "#00FF00").css("color", "#000");
    }
    $(function() {
        $(document).tooltip();
        $(".valores").change();
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
            <td colspan="5" class="GridHeader">Orden de producción</td>
        </tr>
        <tr>
            <td class="tdl" width="1">Número</td>
            <td width="1"><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
            <td width="146px">Lote</td>
            <td width="1"><asp:TextBox runat="server" ID="TLote" CssClass="lote"></asp:TextBox></td>
            <td class="tdr"></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="PBordes">
        <tr>
            <td colspan="5" class="tdt"></td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="PAnalisis" Visible="false">
        <tr>
            <td class="tdl">Planeación</td>
            <td align="right"><asp:Label runat="server" ID="LPlaneacion" Font-Bold="true"></asp:Label></td>
            <td>Producción</td>
            <td align="right"><asp:Label runat="server" ID="LProduccion" Font-Bold="true"></asp:Label></td>
            <td class="tdr"></td>
        </tr>
        <tr>
            <td class="tdl" nowrap="nowrap">Cantidad programada</td>
            <td align="right"><asp:Label runat="server" ID="LCantidad" Font-Bold="true"></asp:Label></td>
            <td></td>
            <td></td>
            <td class="tdr"></td>
        </tr>
        <tr>
            <td colspan="5" class="tdb tdl tdr"><asp:Label runat="server" ID="LProducto" Font-Bold="true"></asp:Label><br /><asp:Label runat="server" ID="LCliente" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="5">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader">Análisis físico químico</td>
        </tr>
        <tr>
            <td class="tdl">Fecha<asp:RequiredFieldValidator ValidationGroup="analisis" ControlToValidate="TFecha" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                            <asp:RangeValidator ValidationGroup="analisis" ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox></td>
            <td></td>
            <td></td>
            <td class="tdr"></td>
        </tr>
        <tr>
            <td class="tdl">Peso Lavado<asp:RegularExpressionValidator ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+%" ID="RegularExpressionValidator1" runat="server" ErrorMessage="Por favor ingrese un porcentaje." ControlToValidate="TPesoLavado" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
            <td><asp:TextBox ID="TPesoLavado" runat="server" CssClass="porcentaje"></asp:TextBox></td>
            <td nowrap="nowrap">Peso Escurrido<asp:RegularExpressionValidator ValidationGroup="analisis" ValidationExpression="[0-9]*[.]?[0-9]+%" ID="RegularExpressionValidator2" runat="server" ErrorMessage="Por favor ingrese un porcentaje." ControlToValidate="TPesoEscurrido" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
            <td><asp:TextBox ID="TPesoEscurrido" runat="server" CssClass="porcentaje"></asp:TextBox></td>
            <td class="tdr"></td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr">
                <asp:DataList ID="DAnalisis" runat="server" AutoGenerateColumns="False" DataSourceID="SAnalisis" RepeatColumns="2" DataKeyField="nombre">
                    <ItemTemplate>
                        <table>
                            <tr>
                                <td width="146px">
                                <asp:HiddenField runat="server" ID="HId" Value='<%# Eval("id") %>' />
                                <%# Eval("nombre") %><asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationGroup="analisis" ControlToValidate="TValor" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ValidationGroup="analisis" ControlToValidate="TValor" ErrorMessage='<%# "Por favor ingrese " + Eval("nombre") + "."%>' ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                </td>
                                <td><asp:TextBox runat="server" 
                                    title='<%# "Valor entre " + Eval("minimo")+  "-" + Eval("maximo") %>'
                                    onchange='<%# "validar(this, " + Convert.ToDouble(Eval("minimo")).ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + Convert.ToDouble(Eval("maximo")).ToString(System.Globalization.CultureInfo.InvariantCulture)+ ")"%>' 
                                    ID="TValor" CssClass="float valores" Text='<%# Eval("valor")==DBNull.Value ? "" : Convert.ToDouble(Eval("valor")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'></asp:TextBox></td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
                <asp:SqlDataSource ID="SAnalisis" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT varios.id, varios.nombre,formulacion_parametro.minimo,formulacion_parametro.maximo 
                            ,analisis_fisico_quimico_parametros.valor
                            FROM formulacion_parametro 
                            LEFT JOIN varios ON (formulacion_parametro.parametro=varios.id)
                            LEFT JOIN analisis_fisico_quimico_parametros ON (analisis_fisico_quimico_parametros.analisis=@analisis AND analisis_fisico_quimico_parametros.parametro=varios.id)
                            WHERE formulacion_parametro.formulacion=@producto
                            ORDER BY varios.nombre;">
                            <SelectParameters>
                                <asp:Parameter Name="analisis" DefaultValue="0" />
                                <asp:Parameter Name="producto" />
                            </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td class="tdl">Observaciones</td>
            <td colspan="4" class="tdr"><asp:TextBox runat="server" ID="TObservaciones" Width="99%"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl tdr" colspan="5">
                <asp:ValidationSummary runat="server" ValidationGroup="analisis" />
            </td>
        </tr>
        <tr>
            <td colspan="5" align="right" class="tdl tdr">
                <asp:Button runat="server" ID="BAgregar" Text="Agregar" OnClick="BAgregar_Clicked" ValidationGroup="analisis" />
                <asp:Button runat="server" ID="BCancelarAgregar" Text="Cancelar" CausesValidation="false" Visible="false" OnClick="BCancelarAgregar_Clicked" />
            </td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr tdb" align="center">
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
                        <asp:BoundField DataField="Brix" HeaderText="Brix" SortExpression="Brix" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                        <asp:BoundField DataField="Acidez" HeaderText="Acidez" SortExpression="Acidez" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                        <asp:BoundField DataField="Viscosidad" HeaderText="Viscosidad" SortExpression="Viscosidad" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                        <asp:BoundField DataField="Consistencia" HeaderText="Consistencia" SortExpression="Consistencia" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                        <asp:BoundField DataField="PH" HeaderText="pH" SortExpression="PH" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                        <asp:BoundField DataField="Densidad" HeaderText="Densidad" SortExpression="Densidad" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                        <asp:BoundField DataField="ChartColor" HeaderText="Chart-Color" SortExpression="ChartColor" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                        <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" SortExpression="Observaciones" ItemStyle-HorizontalAlign="left" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader">Microbiológicas</td>
        </tr>
        <tr>
            <td class="tdl">Fecha<asp:RequiredFieldValidator ValidationGroup="analisis" ControlToValidate="TFechaMicro" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                            <asp:RangeValidator ValidationGroup="analisis" ControlToValidate="TFechaMicro" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator1" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
            <td><asp:TextBox runat="server" ID="TFechaMicro" CssClass="fecha"></asp:TextBox></td>
            <td></td>
            <td></td>
            <td class="tdr"></td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr">
                <asp:DataList ID="DMicrobiologico" runat="server" AutoGenerateColumns="False" DataSourceID="SMicrobiologico" RepeatColumns="2" DataKeyField="nombre">
                    <ItemTemplate>
                        <table>
                            <tr>
                                <td width="146px" style="font-size: 10px">
                                <asp:HiddenField runat="server" ID="HId" Value='<%# Eval("id") %>' />
                                <%# Eval("nombre") %><asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationGroup="analisis" ControlToValidate="TValor" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ValidationGroup="analisis" ControlToValidate="TValor" ErrorMessage='<%# "Por favor ingrese " + Eval("nombre") + "."%>' ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                </td>
                                <td><asp:TextBox runat="server" ID="TValor" CssClass="float" Text='<%# Eval("valor")==DBNull.Value ? "" : Convert.ToDouble(Eval("valor")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'></asp:TextBox></td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
                <asp:SqlDataSource ID="SMicrobiologico" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT varios.id, varios.nombre, analisis_microbiologico_parametros.valor
                            FROM varios
                            LEFT JOIN analisis_microbiologico_parametros ON (analisis_microbiologico_parametros.analisis=@analisis AND analisis_microbiologico_parametros.parametro=varios.id)
                            WHERE varios.tipo=1004 AND varios.szReservado IS NULL
                            ORDER BY varios.id;">
                            <SelectParameters>
                                <asp:Parameter Name="analisis" DefaultValue="0" />
                            </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="5">&nbsp;
                <asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="5" align="right">
                <asp:Button runat="server" ID="BAceptar" Text="Aceptar" OnClick="BAceptar_Clicked" />
                <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" OnClick="BCancelar_Clicked" />
            </td>
        </tr>
        </asp:PlaceHolder>
            </table>
</asp:Content>

