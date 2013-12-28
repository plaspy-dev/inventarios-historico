<%@ Page Title="Laboratorio" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="laboratorio.aspx.cs" Inherits="produccion_laboratorio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .chart-color 
    {
        width: 100px;
    }
    .lote,.porcentaje 
    {
        width: 100px;
        text-align: right;
    }
    #<%= GAnalisis.ClientID %> .GridHeader,#<%= GMicro.ClientID %> .GridHeader 
    {
        font-size: 10px;
    }
    .esterilidad tr td:nth-child(odd),.sensorial tr td:nth-child(odd) { width: 146px;}
    .esterilidad tr td:nth-child(even),.sensorial tr td:nth-child(even) { width: 129px;}
    .esterilidad tr td { font-size: 12px;}
    .rmicrosimple 
    {
        white-space: nowrap;
        font-size: 11px;
    }
</style>
<script type="text/javascript">
    function validar(textbox, minimo, maximo) {
        if ($(textbox).val() == "") {
            $(textbox).css("background-color", "").css("color", "#000");
            return;
        }
        var valor = parseFloat($(textbox).val());
        if (minimo && valor < minimo)
            $(textbox).css("background-color", "#FFFF00").css("color", "#000");
        else if (maximo && valor > maximo)
            $(textbox).css("background-color", "#FF0000").css("color", "#FFF");
        else $(textbox).css("background-color", "#00FF00").css("color", "#000");
    }
    $(function() {
        $(".rmicrosimple").each(function() {
            $(this).find("tr td:first").hide();
        });
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
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Panel runat="server" ID="PAnulado" Visible="false" CssClass="anulado"><asp:Label runat="server" ID="LAnulado"></asp:Label></asp:Panel>
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
                    <asp:RegularExpressionValidator ValidationGroup="analisis" ID="RegularExpressionValidator4" ControlToValidate="TFecha" ErrorMessage="Por favor ingrese una fecha valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))) (0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
            <td><asp:TextBox runat="server" ID="TFecha" CssClass="fechahora"></asp:TextBox></td>
            <td></td>
            <td></td>
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
                                <td width="129px"><asp:TextBox runat="server" 
                                    title='<%# "Valor entre " + Eval("minimo")+  "-" + Eval("maximo") %>'
                                    onchange='<%# "validar(this, " + (Eval("minimo")==DBNull.Value ? "null" : Convert.ToDouble(Eval("minimo")).ToString(System.Globalization.CultureInfo.InvariantCulture)) + "," + (Eval("maximo")==DBNull.Value ? "null" : Convert.ToDouble(Eval("maximo")).ToString(System.Globalization.CultureInfo.InvariantCulture)) + ")"%>' 
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
                            WHERE formulacion_parametro.formulacion=@producto AND varios.tipo=1003 AND varios.estado IS NULL
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
                <asp:GridView ID="GAnalisis" runat="server" AutoGenerateColumns="False" Width="100%"
                    DataKeyNames="IdGrid" OnRowDeleting="GAnalisis_RowDeleting" OnSelectedIndexChanged="GAnalisis_SelectedIndexChanged" OnRowDataBound="GAnalisis_RowDataBound"
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
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" SortExpression="Fecha" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" SortExpression="Observaciones" ItemStyle-HorizontalAlign="left" ItemStyle-Width="100%" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader">Análisis sensorial</td>
        </tr>
        <tr>
            <td class="tdl">Chart-Color</td>
            <td><asp:TextBox runat="server" ID="TChartColor" CssClass="chart-color"></asp:TextBox></td>
            <td colspan="3" class="tdr"></td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr tdb">
                <asp:CheckBoxList RepeatColumns="4" runat="server" ID="CSensorial" DataSourceID="SSensorial" DataTextField="nombre" DataValueField="id" CssClass="sensorial">
                </asp:CheckBoxList>
                <asp:SqlDataSource ID="SSensorial" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT varios.id, varios.nombre
                            FROM varios
                            WHERE varios.tipo=1005 AND varios.estado IS NULL
                            ORDER BY varios.nombre;">
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader">Análisis microbiológico</td>
        </tr>
        <tr>
            <td class="tdl">Fecha<asp:RequiredFieldValidator ValidationGroup="micro" ControlToValidate="TFechaMicro" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="TFechaMicro" ValidationGroup="micro" ErrorMessage="Por favor ingrese una fecha valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))) (0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
            <td><asp:TextBox runat="server" ID="TFechaMicro" CssClass="fechahora"></asp:TextBox></td>
            <td></td>
            <td></td>
            <td class="tdr"></td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr">
                <asp:DataList ID="DMicroSimple" runat="server" AutoGenerateColumns="False" DataSourceID="SMicroSimple" RepeatColumns="2" DataKeyField="nombre">
                    <ItemTemplate>
                        <table>
                            <tr>
                                <td width="146px" style="font-size: 10px; white-space: nowrap;">
                                <asp:HiddenField runat="server" ID="HId" Value='<%# Eval("id") %>' />
                                    <%# Eval("nombre") %>
                                    <asp:RequiredFieldValidator ValidationGroup="micro" ControlToValidate="RValor" ErrorMessage='<%# "Por favor seleccione un " + Eval("nombre") + "."%>' ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                </td>
                                <td width="129px">
                                <asp:RadioButtonList runat="server" ID="RValor" RepeatColumns="3" CssClass="rmicrosimple" SelectedValue='<%# Eval("valor") %>'>
                                    <asp:ListItem Text="Ninguno" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Ausente" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Presente" Value="1"></asp:ListItem>
                                </asp:RadioButtonList>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
                <asp:SqlDataSource ID="SMicroSimple" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT varios.id, varios.nombre
                            ,analisis_fisico_quimico_parametros.valor
                            FROM varios
                            LEFT JOIN analisis_fisico_quimico_parametros ON (analisis_fisico_quimico_parametros.analisis=@analisis AND analisis_fisico_quimico_parametros.parametro=varios.id)
                            WHERE varios.tipo=1007 AND varios.estado IS NULL
                            ORDER BY varios.nombre;">
                            <SelectParameters>
                                <asp:Parameter Name="analisis" DefaultValue="0" />
                            </SelectParameters>
                </asp:SqlDataSource>
                <asp:DataList ID="DMicro" runat="server" AutoGenerateColumns="False" DataSourceID="SMicro" RepeatColumns="2" DataKeyField="nombre">
                    <ItemTemplate>
                        <table>
                            <tr>
                                <td width="146px" style="font-size: 10px; white-space: nowrap;">
                                <asp:HiddenField runat="server" ID="HId" Value='<%# Eval("id") %>' />
                                <%# Eval("nombre") %><asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationGroup="micro" ControlToValidate="TValor" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ValidationGroup="micro" ControlToValidate="TValor" ErrorMessage='<%# "Por favor ingrese " + Eval("nombre") + "."%>' ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                </td>
                                <td width="129px"><asp:TextBox runat="server" 
                                    title='<%# "Valor entre " + Eval("minimo")+  "-" + Eval("maximo") %>'
                                    onchange='<%# "validar(this, " + (Eval("minimo")==DBNull.Value ? "null" : Convert.ToDouble(Eval("minimo")).ToString(System.Globalization.CultureInfo.InvariantCulture)) + "," + (Eval("maximo")==DBNull.Value ? "null" : Convert.ToDouble(Eval("maximo")).ToString(System.Globalization.CultureInfo.InvariantCulture)) + ")"%>' 
                                    ID="TValor" CssClass="float valores" Text='<%# Eval("valor")==DBNull.Value ? "" : Convert.ToDouble(Eval("valor")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'></asp:TextBox></td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
                <asp:SqlDataSource ID="SMicro" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT varios.id, varios.nombre,formulacion_parametro.minimo,formulacion_parametro.maximo 
                            ,analisis_fisico_quimico_parametros.valor
                            FROM formulacion_parametro 
                            LEFT JOIN varios ON (formulacion_parametro.parametro=varios.id)
                            LEFT JOIN analisis_fisico_quimico_parametros ON (analisis_fisico_quimico_parametros.analisis=@analisis AND analisis_fisico_quimico_parametros.parametro=varios.id)
                            WHERE formulacion_parametro.formulacion=@producto AND varios.tipo=1004 AND varios.estado IS NULL
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
            <td colspan="4" class="tdr"><asp:TextBox runat="server" ID="TObservacionesMicro" Width="99%"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl tdr" colspan="5">
                <asp:ValidationSummary runat="server" ValidationGroup="micro" />
            </td>
        </tr>
        <tr>
            <td colspan="5" align="right" class="tdl tdr">
                <asp:Button runat="server" ID="BAgregarMicro" Text="Agregar" OnClick="BAgregarMicro_Clicked" ValidationGroup="micro" />
                <asp:Button runat="server" ID="BCancelarMicro" Text="Cancelar" CausesValidation="false" Visible="false" OnClick="BCancelarMicro_Clicked" />
            </td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr tdb" align="center">
                <asp:HiddenField runat="server" ID="HMicro" />
                <asp:GridView ID="GMicro" runat="server" AutoGenerateColumns="False" Width="100%"
                    DataKeyNames="IdGrid" OnRowDeleting="GMicro_RowDeleting" OnSelectedIndexChanged="GMicro_SelectedIndexChanged" OnRowDataBound="GMicro_RowDataBound"
                    EmptyDataText="No hay análisis microbiológicos" >
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
                        <asp:CommandField ButtonType="Image" SelectImageUrl="~/img/edit.gif" ShowSelectButton="True" />
                        <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" SortExpression="Fecha" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" SortExpression="Observaciones" ItemStyle-HorizontalAlign="left" ItemStyle-Width="100%" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader">Prueba de esterilidad comercial</td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr tdb">
                <asp:CheckBoxList RepeatColumns="4" runat="server" ID="CEsterilidad" DataSourceID="SEsterilidad" DataTextField="nombre" DataValueField="id" CssClass="esterilidad">
                </asp:CheckBoxList>
                <asp:SqlDataSource ID="SEsterilidad" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT varios.id, varios.nombre
                            FROM varios
                            WHERE varios.tipo=1006 AND varios.estado IS NULL
                            ORDER BY varios.nombre;">
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" class="GridHeader">Ajustes</td>
        </tr>
        <tr>
            <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TIngredienteID" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
            <td colspan="4" class="tdr"><asp:TextBox runat="server" ID="TIngrediente" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TIngredienteID" CssClass="hide"></asp:TextBox><asp:TextBox runat="server" ID="TRecetaID" CssClass="hide"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdl">Cantidad
                <asp:RequiredFieldValidator ControlToValidate="TCantidadIngrediente" ValidationGroup="agregar" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidadIngrediente" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                </td>
            <td><asp:TextBox runat="server" ID="TCantidadIngrediente" CssClass="float"></asp:TextBox></td>
            <td>Lote<asp:RequiredFieldValidator ControlToValidate="TLoteIngrediente" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un lote." ID="RLoteIngrediente" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="HProductos" />
            </td>
            <td>
            <asp:TextBox runat="server" ID="TLoteIngrediente" CssClass="lote"></asp:TextBox>
            </td>
            <td class="tdr" align="right">
                <asp:Button runat="server" ID="BMas" Text="Agregar" ValidationGroup="agregar" OnClick="BMas_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="5" class="tdl tdr tdb" align="center">
                <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" Width="100%"
                        DataKeyNames="IdGrid,id" 
                        EmptyDataText="No hay ajustes" onrowdeleting="GProductos_RowDeleting" >
                        <RowStyle CssClass="GridRow"></RowStyle>
                        <FooterStyle CssClass="GridFooter"></FooterStyle>
                        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                        <Columns>
                            <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                            <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100%" FooterStyle-HorizontalAlign="Left" />
                            <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:0.########}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                            <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" ItemStyle-HorizontalAlign="Left" />
                        </Columns>
                    </asp:GridView>
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

