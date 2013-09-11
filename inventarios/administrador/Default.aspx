<%@ Page Title="Parámetros" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="administrador_Default" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Panel ID="Panel1" runat="server" DefaultButton="BAceptar">
    <table width="100%" border="0">
  <tr>
    <td valign="top" width=1>
        <asp:TreeView ID="TTipo" runat="server"  ImageSet="Arrows" >
            <ParentNodeStyle Font-Bold="False" />
            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" 
                HorizontalPadding="0px" VerticalPadding="0px" />
            <NodeStyle/>
        </asp:TreeView>
        <br />
      </td>
    <td valign="top"><table width="100%" border="0">
      <tr>
        <td align="left" class="GridHeader">
            <asp:Label ID="LTipo" runat="server"></asp:Label>
          </td>
      </tr>
      <tr>
        <td>
<table width="100%" border="0">
          <tr>
            <td width=1>Nombre</td>
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
            <td>Descripción</td>
            <td>
                <asp:TextBox ID="TDescripcion" runat="server" EnableViewState="False"></asp:TextBox>
              </td>
            <td align="right">
                <asp:HiddenField ID="HId" runat="server" />
              </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
            <td align="left">
                <asp:Button ID="BAceptar" runat="server" Text="Aceptar" 
                    onclick="BAceptar_Click" />
                <asp:Button ID="BCancelar" runat="server" CausesValidation="False" 
                    Text="Cancelar" onclick="BCancelar_Click" />
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
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" ItemStyle-Width="100%" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre, descripcion FROM varios WHERE tipo=@tipo AND estado IS NULL" 
                DeleteCommand="UPDATE varios SET estado=-1 WHERE id=@id" >
                <SelectParameters>
                    <asp:QueryStringParameter QueryStringField="tipo" Name="tipo" Type="Int32" DefaultValue="1" />
                </SelectParameters>
            </asp:SqlDataSource>
          </td>
      </tr>
    </table></td>
  </tr>
</table>
</asp:Panel>
</asp:Content>

