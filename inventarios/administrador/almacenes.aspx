<%@ Page Title="Almacenes" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="almacenes.aspx.cs" Inherits="administrador_almacenes" %>

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
            <td width=1>Nombre</td>
            <td width=1>
                <asp:TextBox ID="TNombre" runat="server"></asp:TextBox>
              </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ControlToValidate="TNombre" Display="Dynamic" 
                    ErrorMessage="Por favor ingrese un nombre." SetFocusOnError="True"></asp:RequiredFieldValidator>
              </td>
          </tr>
          <tr>
            <td>Descripción</td>
            <td>
                <asp:TextBox ID="TDescripcion" runat="server" EnableViewState="False"></asp:TextBox>
              </td>
            <td align="right">
                <asp:HiddenField ID="HId" runat="server" />
              </td>
          </tr>
          <tr>
              <td colspan="3" class="GridHeader">Ubicaciones</td>
          </tr>
          <tr>
              <td colspan="3" class="tdl tdr">
              <asp:GridView ID="GUbicaciones" runat="server" AutoGenerateColumns="False" Width="100%"
                DataKeyNames="id" DataSourceID="SUbicaciones" ShowHeader="false"
                onselectedindexchanged="GUbicaciones_SelectedIndexChanged" AllowPaging="true" >
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" ItemStyle-Width="1" />
                    <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" ItemStyle-Width="1" />
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" ItemStyle-Width="100%" />
                </Columns>
                </asp:GridView>
                <sdg:CustomDataSource ID="SUbicaciones" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    SelectCommand="SELECT id, nombre FROM ubicacion WHERE almacen=@almacen AND estado IS NULL" 
                    UpdateCommand="UPDATE ubicacion SET nombre=@nombre WHERE id=@id"
                    InsertCommand="INSERT INTO ubicacion (nombre,almacen) VALUES (@nombre,@almacen)"
                    DeleteCommand="UPDATE ubicacion SET estado=-1 WHERE id=@id" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="HId" PropertyName="Value" Name="almacen" />
                    </SelectParameters>
                </sdg:CustomDataSource>
              </td>
          </tr>
          <tr>
            <td class="tdl tdb">Ubicación<asp:RequiredFieldValidator ControlToValidate="TUbicacion" ValidationGroup="ubicacion" ErrorMessage="Por favor ingrese una ubicación." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            </td>
            <td class="tdb"><asp:TextBox runat="server" ID="TUbicacion"></asp:TextBox></td>
            <td align="left" class="tdr tdb"><asp:Button runat="server" ID="BMas" ValidationGroup="ubicacion" Text="+" onclick="BMas_Click" /></td>
          </tr>
          <tr>
            <td>
                <asp:Button ID="BActualizar" runat="server" Text="Actualizar" Visible="false" 
                    onclick="BActualizar_Click" CausesValidation="false" />
              </td>
            <td align="right">
                <asp:Button ID="BAceptar" runat="server" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button ID="BCancelar" runat="server" CausesValidation="False" Text="Cancelar" onclick="BCancelar_Click" />
              </td>
            <td>
                <asp:HiddenField ID="HTipo" runat="server" />
              </td>
          </tr>
        </table></td>
      </tr>
      <tr>
        <td>
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" Width="100%"
                DataKeyNames="id" DataSourceID="SResultados" AllowSorting="true"
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
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" ItemStyle-Width="100%" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre, descripcion FROM almacen WHERE estado IS NULL ORDER BY nombre" 
                DeleteCommand="UPDATE almacen SET estado=-1 WHERE id=@id" >
            </asp:SqlDataSource>
          </td>
      </tr>
    </table></td>
  </tr>
</table>
</asp:Panel>
</asp:Content>

