<%@ Page Title="Grupos" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="grupos.aspx.cs" Inherits="administrador_grupos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">    
    function total(elemento, rep) {
        c = ['_a', '_e', '_d', '_i', '_m', '_l'];
        for (e in c) {
            if ($("#" + elemento.id.replace(rep, c[e])).is(":checked") == false) {
                $("#" + elemento.id.replace(rep, '_t')).attr('checked', false);
                return;
            }
        }
        $("#" + elemento.id.replace(rep, '_t')).attr('checked', true);
    }
    $(document).ready(function() {

        $("#alertas").accordion({
            autoHeight: false,
            navigation: true,
            collapsible: true,
            active: false
        });
        $("#accordion").accordion({
            autoHeight: false,
            navigation: true
        });

        for (j = 1; j < 100; j++) {
            nombre = "#ctl00_ContentPlaceHolder1_R_ctl";
            if (j < 10)
                nombre += "0";
            nombre += j;
            nombre += "_P_ctl";
            if ($(nombre + "02_t").length == 0)
                break;

            for (i = 2; ; i++) {
                temp = nombre;
                if (i < 10)
                    temp += "0";
                temp += i;
                if ($(temp + "_t").length == 0)
                    break;
                $(temp + "_t").change(function() { // todos
                    c = ['_a', '_e', '_d', '_i', '_m', '_l'];
                    for (e in c)
                        $("#" + this.id.replace("_t", c[e])).attr('checked', $(this).is(":checked"));
                });
                $(temp + '_a').change(function() { total(this, '_a'); });
                $(temp + '_e').change(function() { total(this, '_e'); });
                $(temp + '_d').change(function() { total(this, '_d'); });
                $(temp + '_i').change(function() { total(this, '_i'); });
                $(temp + '_m').change(function() { total(this, '_m'); });
                $(temp + '_l').change(function() { total(this, '_l'); });
            }
        }
    });
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table>
        <tr>
            <td valign="top">
                <asp:ListBox ID="LGrupos" runat="server" AutoPostBack="True"  AppendDataBoundItems="true"
                    DataSourceID="SGrupos" DataTextField="nombre" DataValueField="id" 
                    onselectedindexchanged="LGrupos_SelectedIndexChanged" Height="200px">
                <asp:ListItem Text="Administrador" Value="0"></asp:ListItem>
                </asp:ListBox>
                <asp:SqlDataSource ID="SGrupos" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT id, nombre FROM grupo" >
                </asp:SqlDataSource>
            </td>
            <td>
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <table>
                    <tr>
                        <td>
                            Nombre
                        </td>
                        <td>
                            <asp:TextBox ID="TNombre" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ControlToValidate="TNombre" Display="Dynamic" 
                                ErrorMessage="Por favor ingrese un nombre."></asp:RequiredFieldValidator>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Descripción</td>
                        <td>
                            <asp:TextBox ID="TDescripcion" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="alertas">
                                <h3><a href="#">Alertas</a></h3>
                                <div class="hide">
                                    <asp:GridView ID="GAlertas" runat="server" DataSourceID="SAlertas" Width="100%"
                                        DataKeyNames="id" AutoGenerateColumns="false" ShowHeader="false"
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
                                                    <asp:CheckBox runat="server" ID="CAlerta" Checked='<%# Eval("alerta").ToString()=="" ? false : true %>' Text='<%# Eval("nombre").ToString() %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="SAlertas" runat="server"
                                            ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                                            ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                                            SelectCommand="SELECT alertas.id,alertas.nombre,IF(@grupo=0,1, alertas_grupo.alerta) AS alerta
                                                    FROM alertas
                                                    LEFT JOIN alertas_grupo ON (alertas_grupo.grupo=@grupo AND alertas_grupo.alerta=alertas.id);">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="LGrupos" PropertyName="SelectedValue" Name="grupo" DefaultValue="-1" />
                                                    </SelectParameters>
                                    </asp:SqlDataSource>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Repeater ID="R" runat="server" DataMember="parent" 
                                onitemcreated="R_ItemCreated">
                                <HeaderTemplate>
                                    <div id="accordion">
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <h3><a href="#"><%# Eval("parent") %></a></h3>
                                    <div class="hide">
                                    <asp:GridView ID="P" runat="server" AutoGenerateColumns="false" DataKeyNames="pagina" Width="100%">
                                        <RowStyle CssClass="GridRow"></RowStyle>
                                        <FooterStyle CssClass="GridFooter"></FooterStyle>
                                        <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                                        <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                                        <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                                        <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                                        <Columns>
                                            <asp:TemplateField HeaderText="Todos">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="t" runat="server" Checked='<%# Bind("todos") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Página" ItemStyle-Width="100%">
                                                <ItemTemplate>
                                                    <%# Eval("titulo") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Administrador">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="a" runat="server" Checked='<%# Bind("administrador") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Eliminar">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="e" runat="server" Checked='<%# Bind("eliminar") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Editar">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="d" runat="server" Checked='<%# Bind("edicion") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Insertar">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="i" runat="server" Checked='<%# Bind("insercion") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Imprimir">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="m" runat="server" Checked='<%# Bind("imprimir") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Leer">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="l" runat="server" Checked='<%# Bind("lectura") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Button ID="BAceptar" runat="server" onclick="BAceptar_Click" Text="Aceptar" />
                            <asp:Button ID="BCancelar" runat="server" CausesValidation="False" onclick="BCancelar_Click" Text="Cancelar" />
                            <asp:Button ID="BEliminar" runat="server" CausesValidation="False" onclick="BEliminar_Click" Text="Eliminar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        </table>
</asp:Content>

