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
                        alert(textStatus);
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
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel ID="Panel1" runat="server" DefaultButton="BAceptar">
    <table width="100%" border="0">
  <tr>
    <td valign="top"><table width="100%" border="0">
      <tr>
        <td>
        <table width="100%" border="0"  class="tablecollapse">
          <tr>
            <td>Identificación</td>
            <td>
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
            <td></td>
            <td align="right">
                <asp:Button ID="BAceptar" runat="server" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button ID="BCancelar" runat="server" CausesValidation="False" Text="Cancelar" onclick="BCancelar_Click" />
              </td>
            <td></td>
          </tr>
        </table></td>
      </tr>
      <tr>
        <td>
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" 
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
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" />
                    <asp:BoundField DataField="direccion" HeaderText="Dirección" SortExpression="direccion" />
                    <asp:BoundField DataField="telefono" HeaderText="Teléfono" SortExpression="telefono" />
                    <asp:BoundField DataField="email" HeaderText="E-Mail" SortExpression="email" />
                    <asp:BoundField DataField="tipo" HeaderText="Tipo" SortExpression="tipo" />
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