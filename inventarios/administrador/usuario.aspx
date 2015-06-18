<%@ Page Title="Usuarios" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="usuario.aspx.cs" Inherits="administrador_usuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    $(function() {
        $("#alertas").accordion({
            autoHeight: false,
            navigation: true,
            collapsible: true,
            active: false
        });
        $("#accordion").accordion({
            autoHeight: false,
            navigation: true,
            collapsible: true,
            active: false
        });
        var almacenes = $("#<%= GAlmacenes.ClientID %>").find("input");
        var todos = null;
        almacenes.each(function() {
            if (todos == null) {
                var label = $("label[for='" + $(this).attr('id') + "']");
                if (label.text() == "( Todos --Incluidos los nuevos)")
                    todos = $(this);
            }
        });
        almacenes.change(function() {
            if ($(this).attr("id") == todos.attr("id")) {
                almacenes.each(function() {
                    $(this).attr('checked', todos.is(":checked"));
                });
            } else {
                if (!$(this).is(":checked")) {
                    todos.attr("checked", false);
                }
            }
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <fieldset>
        <legend>Usuario</legend>
        <table>
        <tr>
            <td>
                Nombres<asp:HiddenField ID="HId" runat="server" />
            </td>
            <td>
                <asp:TextBox ID="TNombres" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ControlToValidate="TNombres" Display="Dynamic" 
                    ErrorMessage="Por favor ingrese un nombre." SetFocusOnError="True"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Usuario</td>
            <td>
                <asp:TextBox ID="TUsuario" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                    ControlToValidate="TUsuario" Display="Dynamic" 
                    ErrorMessage="Por favor ingrese un usuario." SetFocusOnError="True"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Grupo</td>
            <td>
                <asp:DropDownList ID="DGrupos" runat="server" DataSourceID="SGrupos" DataTextField="nombre" DataValueField="id" AppendDataBoundItems="true" AutoPostBack="true">
                    <asp:ListItem Text="Administrador" Value="0" Selected="True"></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SGrupos" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre FROM grupo" >
            </asp:SqlDataSource>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Contraseña</td>
            <td>
                <asp:TextBox ID="TContrasena" TextMode="Password" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="RContrasena" runat="server" 
                    ControlToValidate="TContrasena" Display="Dynamic" 
                    ErrorMessage="Por favor ingrese una contraseña." SetFocusOnError="True"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Confirmar contraseña</td>
            <td>
                <asp:TextBox ID="TConfirmar" TextMode="Password" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:CompareValidator ID="CompareValidator1" runat="server" 
                    ControlToCompare="TContrasena" ControlToValidate="TConfirmar" Display="Dynamic" 
                    ErrorMessage="Las contraseñas no coinciden." SetFocusOnError="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <div id="accordion">
                    <h3><a href="#">Almacenes</a></h3>
                    <div class="hide">
                        <asp:GridView ID="GAlmacenes" runat="server" DataSourceID="SAlmacenes" 
                            DataKeyNames="id" AutoGenerateColumns="false" ShowHeader="false"
                            AllowSorting="True" onrowdatabound="GAlmacenes_RowDataBound">
                            <RowStyle CssClass="GridRow"></RowStyle>
                            <FooterStyle CssClass="GridFooter"></FooterStyle>
                            <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                            <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                            <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                            <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                            <Columns>
                                <asp:TemplateField HeaderText="Permiso">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="CAlmacen" Checked="false" Text='<%# Eval("nombre").ToString() %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SAlmacenes" runat="server"
                                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                                SelectCommand="(SELECT -2 AS id,'( Todos --Incluidos los nuevos)' AS nombre) UNION (SELECT -1 AS id,'(Ajustes)' AS nombre) UNION (SELECT id,nombre FROM almacen WHERE estado IS NULL) ORDER BY nombre;">
                        </asp:SqlDataSource>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <div id="alertas">
                    <h3><a href="#">Alertas</a></h3>
                    <div class="hide">
                        <asp:GridView ID="GAlertas" runat="server" DataSourceID="SAlertas" 
                            DataKeyNames="id,alerta" AutoGenerateColumns="false" ShowHeader="false"
                            AllowSorting="True">
                            <RowStyle CssClass="GridRow"></RowStyle>
                            <FooterStyle CssClass="GridFooter"></FooterStyle>
                            <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                            <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                            <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                            <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                            <Columns>
                                <asp:TemplateField HeaderText="Permiso">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="CAlerta" Checked='<%# Eval("alerta").ToString()=="" ? (Eval("estado").ToString()!="1" && Eval("estado").ToString()!="") : Eval("estado").ToString()!="-1" %>' Text='<%# Eval("nombre").ToString() %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SAlertas" runat="server"
                                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                                SelectCommand="SELECT alertas.id,alertas.nombre
                                        ,IF(@grupo=0,1, alertas_grupo.alerta) AS alerta
                                        ,alertas_usuario.estado
                                        FROM alertas
                                        LEFT JOIN alertas_grupo ON (alertas_grupo.grupo=@grupo AND alertas_grupo.alerta=alertas.id)
                                        LEFT JOIN alertas_usuario ON (alertas_usuario.usuario=@usuario AND alertas_usuario.alerta=alertas.id)
                                        ;">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="DGrupos" PropertyName="SelectedValue" Name="grupo" DefaultValue="-1" />
                                            <asp:ControlParameter ControlID="HId" PropertyName="Value" Name="usuario" DefaultValue="0" />
                                        </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                &nbsp;</td>
            <td align="right" nowrap="nowrap">
                <asp:Button ID="BAceptar" runat="server" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button ID="BCancelar" runat="server" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
            </td>
        </tr>
    </table>
    </fieldset>
    <asp:GridView ID="GUsuarios" runat="server" DataSourceID="SUsuarios" 
        DataKeyNames="id" AutoGenerateColumns="false" Width="100%"
        onselectedindexchanged="GUsuarios_SelectedIndexChanged" 
        AllowSorting="True" onrowdeleted="GUsuarios_RowDeleted" >
        <RowStyle CssClass="GridRow"></RowStyle>
        <FooterStyle CssClass="GridFooter"></FooterStyle>
        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
        <Columns>
                    <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" />
                    <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                    <asp:BoundField DataField="nombre" HeaderText="Nombres" SortExpression="nombre" ItemStyle-Width="100%" />
                    <asp:BoundField DataField="usuario" HeaderText="Usuario" SortExpression="usuario" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="grupo" HeaderText="Grupo" SortExpression="grupo" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="ingreso" HeaderText="Último ingreso" SortExpression="ingreso" ItemStyle-Wrap="false" />
        </Columns>
        <SelectedRowStyle CssClass="selecteditem" />
    </asp:GridView>
    <asp:SqlDataSource ID="SUsuarios" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT usuarios.id AS id,usuarios.nombre AS nombre,usuarios.usuario AS usuario,IFNULL(grupo.nombre,'Administrador') AS grupo,usuarios.ingreso AS ingreso FROM usuarios LEFT JOIN grupo ON (usuarios.grupo=grupo.id) WHERE usuarios.estado IS NULL OR usuarios.estado!=-1 ORDER BY usuarios.nombre" 
                 DeleteCommand="UPDATE usuarios SET estado=-1 WHERE id=@id">
    </asp:SqlDataSource>
</asp:Content>

