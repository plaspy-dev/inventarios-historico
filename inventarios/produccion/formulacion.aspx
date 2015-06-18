<%@ Page Title="Formulación" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="formulacion.aspx.cs" Inherits="produccion_formulacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    addFunctionToStart(function() {

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

        $("#<%= TProducto.ClientID %>").autocomplete({
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
                $("#<%= TProductoID.ClientID %>").val("");
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                $("#<%= LUnidad.ClientID %>").html(ui.item.unidad);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0)
                $("#<%= TProductoID.ClientID %>").val("");
        });
        addDialog("#<%= PArchivos.ClientID %>", "#dImportar");
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:UpdatePanel runat="server" ID="UProducto" UpdateMode="Conditional">
<Triggers>
    <asp:PostBackTrigger ControlID="BExportar" />
    <asp:PostBackTrigger ControlID="BImportar" />
</Triggers>
<ContentTemplate>
<asp:Panel ID="PArchivos" runat="server" Visible="false">
<div title="Importar y exportar" id="dImportar" class="hide">
<table>
    <tr>
        <td colspan="3">
        Consideraciones para importar la formulación
        <ul>
            <li>Es importante tener una copia de seguridad de la formulación. Puede realizarla con el boton Exportar.</li>
            <li>El archivo para importar tiene el mismo formato del exportado.</li>
            <li>Los archivos deben tener en la primera línea las columnas que se van a modificar. </li>
            <li>Sí la columna Id se encuenta registrada se omitira el nombre y código del producto.</li>
            <li>Sí la columna Id se encuenta en blanco se buscara por nombre y/o código del producto.</li>
            <li>Si no se quiere modificar una columna se puede eliminar la columna. </li>
            <li>Despues de una columna con el título en blanco el resto son omitidas</li>
            <li>Las unicas columnas obligatorias es el 'id' y 'CantidadIngrediente'.</li>
            <li>El archivo debe estar codificado en UTF-7.</li>
        </ul>
        </td>
    </tr>
    <tr>
        <td>Exportar en archivo separador por comas</td>
        <td><asp:Button runat="server" ID="BExportar" Text="Exportar" CausesValidation="false" onclick="BExportar_Click" /></td>
        <td></td>
    </tr>
    <tr>
        <td>Importar archivo separador por comas (Max: 20MB)</td>
        <td><asp:FileUpload runat="server" ID="FImportar" /></td>
        <td><asp:Button runat="server" ID="BImportar" Text="Importar" ValidationGroup="archivo" onclick="BImportar_Click" /></td>
    </tr>
    <tr>
        <td colspan="3">
            <asp:RegularExpressionValidator ValidationGroup="archivo" ID="RFoto1" SetFocusOnError="true" ValidationExpression="^.+\.((csv)|(CSV))$" Display="Dynamic" ControlToValidate="FImportar" runat="server" ErrorMessage="Por favor seleccione un archivo csv valido."></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ValidationGroup="archivo" ControlToValidate="FImportar" ErrorMessage="Por favor seleccione una archivo." ID="RFFoto1" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td colspan="3" align="right">
            <hr />
            <asp:Button runat="server" ID="BCancelarArchivo" Text="Cancelar" CausesValidation="false" OnClick="BArchivos_Click" />
        </td>
    </tr>
</table>
</div>
</asp:Panel>
<table class="tablecollapse">
    <tr>
        <td width="1">Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3"><asp:TextBox runat="server" ID="TProducto" Width="98%"></asp:TextBox>
        <asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Análisis físico químico</td>
    </tr>
    <tr>
        <td colspan="4" class="tda">
            <asp:GridView ID="GAnalisis" runat="server" AutoGenerateColumns="False" DataSourceID="SAnalisis" DataKeyNames="id">
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:TemplateField HeaderText="Propiedad" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# Eval("nombre") %>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TMinimo" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="TMaximo" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Mínimo">
                        <ItemTemplate><asp:TextBox runat="server" ID="TMinimo" CssClass="float" Text='<%# Eval("minimo")==DBNull.Value ? "" : Convert.ToDouble(Eval("minimo")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'></asp:TextBox></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Máximo">
                        <ItemTemplate><asp:TextBox runat="server" ID="TMaximo" CssClass="float" Text='<%# Eval("maximo")==DBNull.Value ? "" : Convert.ToDouble(Eval("maximo")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'></asp:TextBox></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="" DataField="descripcion" ItemStyle-Wrap="false" ItemStyle-Width="100%" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SAnalisis" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT varios.id, varios.nombre,formulacion_parametro.minimo,formulacion_parametro.maximo, varios.descripcion
                        FROM varios 
                        LEFT JOIN formulacion_parametro ON (formulacion_parametro.formulacion=@formulacion AND formulacion_parametro.parametro=varios.id)
                        WHERE varios.tipo=1003 AND varios.estado IS NULL ORDER BY varios.nombre;">
                        <SelectParameters>
                            <asp:Parameter Name="formulacion" DefaultValue="0" />
                        </SelectParameters>
            </asp:SqlDataSource>
        </td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Análisis microbiológicos</td>
    </tr>
    <tr>
        <td colspan="4" class="tda">
            <asp:GridView ID="GMicro" runat="server" AutoGenerateColumns="False" DataSourceID="SMicro" DataKeyNames="id">
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:TemplateField HeaderText="Propiedad" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# Eval("nombre") %>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TMinimo" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="TMaximo" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un número decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Mínimo">
                        <ItemTemplate><asp:TextBox runat="server" ID="TMinimo" CssClass="float" Text='<%# Eval("minimo")==DBNull.Value ? "" : Convert.ToDouble(Eval("minimo")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'></asp:TextBox></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Máximo">
                        <ItemTemplate><asp:TextBox runat="server" ID="TMaximo" CssClass="float" Text='<%# Eval("maximo")==DBNull.Value ? "" : Convert.ToDouble(Eval("maximo")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'></asp:TextBox></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="" DataField="descripcion" ItemStyle-Wrap="false" ItemStyle-Width="100%" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SMicro" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT varios.id, varios.nombre,formulacion_parametro.minimo,formulacion_parametro.maximo, varios.descripcion
                        FROM varios 
                        LEFT JOIN formulacion_parametro ON (formulacion_parametro.formulacion=@formulacion AND formulacion_parametro.parametro=varios.id)
                        WHERE varios.tipo=1004 AND varios.estado IS NULL ORDER BY varios.nombre;">
                        <SelectParameters>
                            <asp:Parameter Name="formulacion" DefaultValue="0" />
                        </SelectParameters>
            </asp:SqlDataSource>
        </td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Requerimientos (Para producir 1 <asp:Label runat="server" ID="LUnidad"></asp:Label>)</td>
    </tr>
    <tr>
        <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TIngredienteID" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3" class="tdr"><asp:TextBox runat="server" ID="TIngrediente" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TIngredienteID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Cantidad
            <asp:RequiredFieldValidator ControlToValidate="TCantidadIngrediente" ValidationGroup="agregar" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidadIngrediente" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
        <td width="1"><asp:TextBox runat="server" ID="TCantidadIngrediente" CssClass="float"></asp:TextBox></td>
        <td>
        <asp:HiddenField runat="server" ID="HProductos" />
        <asp:Button runat="server" ID="BMas" Text="Agregar" ValidationGroup="agregar" onclick="BMas_Click" /></td>
        <td class="tdr"></td>
    </tr>
    <tr>
        <td colspan="4" class="tdl tdr tdb" align="center">
            <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" Width="100%"
                    DataKeyNames="IdGrid,id" EmptyDataText="Por favor agregue productos" 
                    onrowdeleting="GProductos_RowDeleting" ShowFooter="true" >
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
                        <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                        <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100%" FooterStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:0.########}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                    </Columns>
                </asp:GridView>
        </td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;<asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label><asp:ValidationSummary ID="ValidationSummary1" runat="server" /></td>
    </tr>
    <tr>
        <td colspan="4" align="right">
            <asp:Button runat="server" ID="BAceptar" Text="Aceptar" onclick="BAceptar_Click" />
            <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
            <asp:Button runat="server" ID="BEliminar" Text="Eliminar" Visible="false" OnClientClick="confirm('¿Esta seguro de eliminar esta formula?');" CausesValidation="false" onclick="BEliminar_Click" />
            <asp:Button runat="server" ID="BArchivos" Text="Importar y exportar" CausesValidation="false" OnClick="BArchivos_Click" />
        </td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4"><hr /></td>
    </tr>
    <tr>
        <td colspan="4">
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" Width="100%"
                DataKeyNames="id" DataSourceID="SResultados" AllowSorting="true" AllowPaging="true" PageSize="50"
                onselectedindexchanged="GResultado_SelectedIndexChanged" >
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" />
                    <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" />
                    <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-Width="70%" />
                    <asp:BoundField DataField="unidad" HeaderText="Unidad" SortExpression="unidad" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="empresa" HeaderText="Empresa" SortExpression="empresa" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="ingredientes" HeaderText="Ingredientes" SortExpression="ingredientes" ItemStyle-HorizontalAlign="Right" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT formulacion.producto AS id,formulacion.producto 
                    ,producto.nombre,producto.descripcion,producto.codigo,producto.unidad,producto.empresa
                    ,COUNT(formulacion_ingrediente.formulacion) AS ingredientes
                    FROM formulacion
                    LEFT JOIN producto ON (producto.id=formulacion.producto)
                    LEFT JOIN formulacion_ingrediente ON (formulacion_ingrediente.formulacion=formulacion.producto)
                    GROUP BY formulacion.producto
                    ORDER BY producto.nombre
                    ">
            </asp:SqlDataSource>
        </td>
    </tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>