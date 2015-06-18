<%@ Page Title="Terceros" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="terceros.aspx.cs" Inherits="administrador_terceros" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    $(function() {
        $("#<%= TIdentificacion.ClientID %>,#<%= TNombre.ClientID %>").autocomplete({
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
                            item.label = item.Documento + " " + item.Nombre;
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

                $(this).val(ui.item.label);
                $.goURL("ti=" + ui.item.Id);
                return false;
            }
        });
        addDialog("#<%= PArchivos.ClientID %>", "#dImportar");
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
<asp:Panel ID="Panel1" runat="server" DefaultButton="BAceptar">
    <table width="100%" border="0">
  <tr>
    <td valign="top"><table width="100%" border="0">
      <tr>
        <td>
        <table width="100%" border="0"  class="tablecollapse">
          <tr>
            <td width=1>Identificación</td>
            <td width=1>
                <asp:TextBox ID="TIdentificacion" runat="server"></asp:TextBox>
              </td>
              <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TIdentificacion" Display="Dynamic" ErrorMessage="Por favor ingrese una identificación." SetFocusOnError="True"></asp:RequiredFieldValidator>
              </td>
          </tr>
          <tr>
            <td>Nombre</td>
            <td>
                <asp:TextBox ID="TNombre" runat="server"></asp:TextBox>
              </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TNombre" Display="Dynamic" ErrorMessage="Por favor ingrese un nombre." SetFocusOnError="True"></asp:RequiredFieldValidator>
              </td>
          </tr>
          <tr>
            <td>Descripción</td>
            <td>
                <asp:TextBox ID="TDescripcion" runat="server"></asp:TextBox>
              </td>
            <td align="right">
                <asp:HiddenField ID="HId" runat="server" />
              </td>
          </tr>
          <tr>
            <td>Dirección</td>
            <td><asp:TextBox runat="server" ID="TDireccion"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="RDireccion" runat="server" ControlToValidate="TDireccion" Display="Dynamic" ErrorMessage="Por favor ingrese una dirección." SetFocusOnError="True"></asp:RequiredFieldValidator>
            </td>
          </tr>
          <tr>
            <td>Teléfono</td>
            <td><asp:TextBox runat="server" ID="TTelefono"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="RTelefono" runat="server" ControlToValidate="TTelefono" Display="Dynamic" ErrorMessage="Por favor ingrese un teléfono." SetFocusOnError="True"></asp:RequiredFieldValidator>
            </td>
          </tr>
          <tr>
            <td>E-Mail</td>
            <td><asp:TextBox runat="server" ID="TEMail"></asp:TextBox></td>
            <td></td>
          </tr>
          <tr>
            <td>Tipo</td>
            <td>
                <asp:DropDownList runat="server" ID="DTipo">
                    <asp:ListItem Text="Cliente" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Proveedor" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Proveedor y cliente" Value="2" Selected="True"></asp:ListItem>
                </asp:DropDownList>
              </td>
              <td></td>
          </tr>
          <tr>
            <td colspan="3"><asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label></td>
          </tr>
          <tr>
            <td></td>
            <td></td>
            <td>
                <asp:Button ID="BAceptar" runat="server" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button ID="BCancelar" runat="server" CausesValidation="False" Text="Cancelar" onclick="BCancelar_Click" />
                <asp:Button runat="server" ID="BArchivos" Text="Importar y exportar" CausesValidation="false" OnClick="BArchivos_Click" />
              </td>
          </tr>
        </table></td>
      </tr>
      <tr>
        <td>
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" Width="100%"
                DataKeyNames="id" DataSourceID="SResultados" AllowPaging="true" AllowSorting="true"
                onselectedindexchanged="GResultado_SelectedIndexChanged" >
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" />
                    <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                    <asp:BoundField DataField="identificacion" HeaderText="Identificación" SortExpression="identificacion" />
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" ItemStyle-Width="100%" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" />
                    <asp:BoundField DataField="direccion" HeaderText="Dirección" SortExpression="direccion" />
                    <asp:BoundField DataField="telefono" HeaderText="Teléfono" SortExpression="telefono" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="email" HeaderText="E-Mail" SortExpression="email" />
                    <asp:BoundField DataField="tipo" HeaderText="Tipo" SortExpression="tipo" ItemStyle-Wrap="false" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre, descripcion,identificacion,telefono,direccion,email
                ,CASE tipo WHEN 1 THEN 'Proveedor' WHEN 2 THEN 'Proveedor y cliente' ELSE 'Cliente' END AS tipo
                 FROM tercero WHERE estado IS NULL ORDER BY nombre" 
                DeleteCommand="UPDATE tercero SET estado=-1 WHERE id=@id" >
            </asp:SqlDataSource>
          </td>
      </tr>
    </table></td>
  </tr>
</table>
</asp:Panel>
</asp:Content>