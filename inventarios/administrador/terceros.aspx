<%@ Page Title="Terceros" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="terceros.aspx.cs" Inherits="administrador_terceros" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
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
            <td>Nombre</td>
            <td>
                <asp:TextBox ID="TNombre" runat="server"></asp:TextBox>
              </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ControlToValidate="TNombre" Display="Dynamic" 
                    ErrorMessage="Por favor ingrese un nombre." SetFocusOnError="True"></asp:RequiredFieldValidator>
              </td>
          </tr>
          <tr>
            <td>Identificación</td>
            <td>
                <asp:TextBox ID="TIdentificacion" runat="server"></asp:TextBox>
              </td>
              <td></td>
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
            <td>Tipo</td>
            <td>
                <asp:DropDownList runat="server" ID="DTipo">
                    <asp:ListItem Text="Cliente" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Proveedor" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Proveedor y cliente" Value="2"></asp:ListItem>
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
                DataKeyNames="id" DataSourceID="SResultados" 
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
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" />
                    <asp:BoundField DataField="identificacion" HeaderText="Identificación" SortExpression="identificacion" />
                    <asp:BoundField DataField="tipo" HeaderText="Tipo" SortExpression="tipo" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre, descripcion,identificacion
                ,CASE tipo WHEN 1 THEN 'Proveedor' WHEN 2 THEN 'Proveedor y cliente' ELSE 'Cliente' END AS tipo
                 FROM tercero WHERE estado IS NULL" 
                DeleteCommand="UPDATE tercero SET estado=-1 WHERE id=@id" >
            </asp:SqlDataSource>
          </td>
      </tr>
    </table></td>
  </tr>
</table>
</asp:Panel>
</asp:Content>