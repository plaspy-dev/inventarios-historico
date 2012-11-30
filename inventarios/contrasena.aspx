<%@ Page Title="Contraseña" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="contrasena.aspx.cs" Inherits="contrasena" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Label runat="server" id="LMensaje"></asp:Label>
<table>
    <tr>
        <td>
            Actual</td>
        <td>
            <asp:TextBox ID="m_EActual" runat="server" TextMode="Password"></asp:TextBox></td>
        <td>
        </td>
    </tr>
        <tr>
            <td>
                Nueva</td>
            <td>
                <asp:TextBox ID="m_EContrasena" runat="server" TextMode="Password"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="m_EContrasena"
                    Display="Dynamic" ErrorMessage="Por favor ingrese una contraseña" SetFocusOnError="True"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td>
                Confirmar</td>
            <td>
                <asp:TextBox ID="m_EConfirmar" runat="server" TextMode="Password"></asp:TextBox></td>
            <td>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="m_EContrasena"
                    ControlToValidate="m_EConfirmar" Display="Dynamic" ErrorMessage="Las contraseñas no coinciden"
                    SetFocusOnError="True"></asp:CompareValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="m_EConfirmar"
                    Display="Dynamic" ErrorMessage="Por favor confirme la contraseña" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    </td>
        </tr>
    <tr>
        <td>
            &nbsp;</td>
        <td>
        </td>
        <td>
            <asp:Button ID="m_BCambiar" runat="server" OnClick="m_BCambiar_Click" Text="Cambiar" />
            <asp:Button ID="BCancelar" runat="server" CausesValidation="False" onclick="Button1_Click" Text="Cancelar" />
        </td>
    </tr>
    </table>
    </asp:Content>

