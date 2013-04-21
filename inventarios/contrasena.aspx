<%@ Page Title="Contraseña" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="contrasena.aspx.cs" Inherits="contrasena" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
#pswd_info {
	padding:5px;
	background:#fefefe;
	border-radius:5px;
	box-shadow:0 1px 3px #ccc;
	border:1px solid #ddd;
}
#pswd_info h4 {
	margin:0 0 10px 0;
	padding:0;
	font-weight:normal;
	font-family: Verdana, Arial, Helvetica, sans-serif;
	font-size: 12px;
}
.invalid {
	color:#ec3f41;
}
.valid {
	color:#3a7d34;
}
</style>
<script type="text/javascript">
    $(function() {
        $("#<%= m_EContrasena.ClientID %>").keyup(function() {
            var pswd = $(this).val();
            //validate the length
            if (pswd.length < 8) {
                $('#length').removeClass('valid').addClass('invalid');
            } else {
                $('#length').removeClass('invalid').addClass('valid');
            }
            //validate letter
            if (pswd.match(/[a-z]/)) {
                $('#letter').removeClass('invalid').addClass('valid');
            } else {
                $('#letter').removeClass('valid').addClass('invalid');
            }

            //validate capital letter
            if (pswd.match(/[A-Z]/)) {
                $('#capital').removeClass('invalid').addClass('valid');
            } else {
                $('#capital').removeClass('valid').addClass('invalid');
            }
            //validate number
            if (pswd.match(/[0-9]/)) {
                $('#number').removeClass('invalid').addClass('valid');
            } else {
                $('#number').removeClass('valid').addClass('invalid');
            }
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Label runat="server" id="LMensaje"></asp:Label>
<table>
    <tr>
        <td>
            Actual</td>
        <td>
            <asp:TextBox ID="m_EActual" runat="server" TextMode="Password"></asp:TextBox></td>
        <td rowspan="8">
             <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="m_EContrasena"
                    Display="Dynamic" ErrorMessage="Por favor ingrese una contraseña" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                    ErrorMessage="Por favor ingrese una contraseña valida." 
                    ControlToValidate="m_EContrasena" Display="Dynamic" SetFocusOnError="True" 
                    ValidationExpression="^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$"></asp:RegularExpressionValidator>
                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="m_EContrasena"
                    ControlToValidate="m_EConfirmar" Display="Dynamic" ErrorMessage="Las contraseñas no coinciden"
                    SetFocusOnError="True"></asp:CompareValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="m_EConfirmar"
                        Display="Dynamic" ErrorMessage="Por favor confirme la contraseña" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <div id="pswd_info">
	                    <h4 style="white-space:nowrap;">La contraseña debe cumplir los siguientes requerimientos:</h4>
	                    <ul>
		                    <li id="letter" class="invalid">Al menos <strong>1 letra minuscula</strong></li>
		                    <li id="capital" class="invalid">Al menos <strong>1 letra mayuscula</strong></li>
		                    <li id="number" class="invalid">Al menos <strong>1 numero</strong></li>
		                    <li id="length" class="invalid">Al menos <strong>8 caracteres</strong></li>
	                    </ul>
                    </div>
        </td>
    </tr>
        <tr>
            <td>
                Nueva</td>
            <td>
                <asp:TextBox ID="m_EContrasena" runat="server" TextMode="Password"></asp:TextBox></td>
        </tr>
        <tr>
            <td>
                Confirmar</td>
            <td>
                <asp:TextBox ID="m_EConfirmar" runat="server" TextMode="Password"></asp:TextBox></td>
        </tr>
    <tr>
        <td>
            &nbsp;</td>
        <td align="right">
            <asp:Button ID="m_BCambiar" runat="server" OnClick="m_BCambiar_Click" Text="Cambiar" />
            <asp:Button ID="BCancelar" runat="server" CausesValidation="False" onclick="Button1_Click" Text="Cancelar" />
        </td>
    </tr>
    <tr>
        <td></td><td></td>
    </tr>
    <tr>
        <td></td><td></td>
    </tr>
    <tr>
        <td></td><td></td>
    </tr>
    <tr>
        <td></td><td></td>
    </tr>
    </table>
    </asp:Content>

