<%@ Page Title="Configuración" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="configuracion.aspx.cs" Inherits="administrador_configuracion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width="100%" border="0">
    <tr>
        <td colspan="3" class="GridHeader">Global</td>
    </tr>
  <tr>
    <td>Costos</td>
    <td><asp:DropDownList ID="DCostos" runat="server">
            <asp:ListItem Text="Obligatorio" Value="1"></asp:ListItem>
            <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
            <asp:ListItem Text="No requerido" Value="0"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td></td>
    </tr>
    <tr>
        <td colspan="3" class="GridHeader">Productos</td>
    </tr>
  <tr>
      <td>
          Manejo de cantidades</td>
      <td>
          <asp:DropDownList ID="DCantidades" runat="server">
              <asp:ListItem Text="Decimal" Value="0"></asp:ListItem>
              <asp:ListItem Text="Entero" Value="1"></asp:ListItem>
          </asp:DropDownList>
      </td>
  </tr>
  <tr>
    <td>Fecha de vencimiento</td>
    <td><asp:DropDownList ID="DVence" runat="server">
            <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
            <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
            <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
    <asp:RequiredFieldValidator ControlToValidate="DVence" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
  </tr>
  <tr>
    <td>Lote</td>
    <td>
        <asp:DropDownList ID="DLote" runat="server">
            <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
            <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
            <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td><asp:RequiredFieldValidator ControlToValidate="DLote" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
  </tr>
  <tr>
    <td>Serial</td>
    <td><asp:DropDownList ID="DSerial" runat="server">
            <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
            <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
            <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td><asp:RequiredFieldValidator ControlToValidate="DVence" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator5" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
  </tr>
  <tr>
    <td>Estado</td>
    <td><asp:DropDownList ID="DEstado" runat="server">
            <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
            <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
            <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td><asp:RequiredFieldValidator ControlToValidate="DVence" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
  </tr>
  <tr>
    <td>Ubicación</td>
    <td><asp:DropDownList ID="DUbicacion" runat="server">
            <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
            <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
            <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td></td>
  </tr>
  <tr>
    <td colspan="3">
        <asp:Label runat="server" ID="LMensaje"></asp:Label>
    </td>
  </tr>
  <tr>
    <td colspan="3" align="right">
        <asp:Button runat="server" ID="BAceptar" Text="Aceptar" onclick="BAceptar_Click" />
        <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
    </td>
  </tr>
</table>
</asp:Content>

