<%@ Page Title="Bienvenido a Plaspy" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width="100%" border="0">
  <tr>
    <td>&nbsp;</td>
    <td><hr class="hrv" /></td>
<td><table>
            <tr>
                <td>
                <asp:Panel runat="server" ID="PLogin" DefaultButton="m_BIngresar">
     <table>
        <tr>
            <td>
                Email</td>
            <td>
                <asp:TextBox ID="m_EUsuario" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic"
                    ErrorMessage="Por favor ingrese un nombre de usuario" SetFocusOnError="True" ControlToValidate="m_EUsuario"></asp:RequiredFieldValidator>&nbsp;</td>
        </tr>
        <tr>
            <td>
                Contrase&ntilde;a</td>
            <td style="text-align: right">
                <asp:TextBox ID="m_EContrasena" runat="server" TextMode="Password"></asp:TextBox></td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td style="text-align: right">
                <asp:Button ID="m_BIngresar" runat="server" OnClick="m_BIngresar_Click" Text="Ingresar" /></td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td style="text-align: right">
                &nbsp;</td>
            <td>
            </td>
        </tr>
    </table></asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
    <asp:Label ID="m_SMensaje" runat="server" ForeColor="Red"></asp:Label></td>
            </tr>
        </table></td>
  </tr>
  <tr>
    <td colspan="3"><hr /></td>
    </tr>
  </table>
</asp:Content>

