<%@ Control Language="C#" AutoEventWireup="true" CodeFile="modificaciones.ascx.cs" Inherits="modificaciones" %>
<asp:Panel runat="server" ID="PModificaciones">
<table border="1">
    <tr>
        <td nowrap="nowrap">Creador</td>
        <td nowrap="nowrap"><asp:Label ID="LCreadoPor" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td nowrap="nowrap">Fecha creación</td>
        <td nowrap="nowrap"><asp:Label ID="LFechaCreacion" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td nowrap="nowrap">Último modificador</td>
        <td nowrap="nowrap"><asp:Label ID="LModificadoPor" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td nowrap="nowrap">Última modificación</td>
        <td nowrap="nowrap"><asp:Label ID="LFechaModificacion" runat="server" Text=""></asp:Label></td>
    </tr>
</table>
</asp:Panel>
<fieldset>
    <legend>Modificaciones</legend>
    <asp:GridView GridLines="Both" ID="GModificaciones" runat="server" ShowHeader="false" AutoGenerateColumns="false" EmptyDataText="No se encontraron modificaciones" EmptyDataRowStyle-Wrap="false" EmptyDataRowStyle-BorderWidth="0">
                <Columns>
                    <asp:BoundField DataField="usuario" HeaderText="Usuario" />
                    <asp:BoundField DataField="fecha" HeaderText="Fecha" />
                </Columns>
            </asp:GridView>
</fieldset>