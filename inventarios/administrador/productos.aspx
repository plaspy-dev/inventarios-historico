<%@ Page Title="Productos" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="productos.aspx.cs" Inherits="administrador_productos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    addFunctionToStart(function() {
        $("#<%= TNombre.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.ProductoCompleto;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $.goURL("pid=" + ui.item.id);
                return false;
            }
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel ID="Panel1" runat="server" DefaultButton="BAceptar">
    <table width="100%" border="0">
  <tr>
    <td valign="top"><table width="100%" border="0">
      <tr>
        <td>
<table width="100%" border="0">
          <tr>
            <td>Nombre</td>
            <td>
                <asp:TextBox ID="TNombre" runat="server" Width="98%"></asp:TextBox>
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
                <asp:TextBox ID="TDescripcion" runat="server" Width="98%"></asp:TextBox>
              </td>
            <td align="right">
                <asp:HiddenField ID="HId" runat="server" />
              </td>
          </tr>
          <tr>
            <td>Código</td>
            <td>
                <asp:TextBox ID="TCodigo" runat="server"></asp:TextBox>
              </td>
                <td align="right">
              </td>
          </tr>
          <tr>
            <td>Empresa</td>
            <td>
                <asp:TextBox ID="TEmpresa" runat="server"></asp:TextBox>
              </td>
                <td align="right">
              </td>
          </tr>
          <tr>
            <td>Unidad</td>
            <td>
                <asp:TextBox ID="TUnidad" runat="server"></asp:TextBox>
              </td>
            <td align="right">
              </td>
          </tr>
          <tr>
            <td>Manejo de cantidades</td>
            <td><asp:DropDownList ID="DCantidades" runat="server">
                    <asp:ListItem Text="(Ninguna)" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="Decimal" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Entero" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td><asp:RequiredFieldValidator ControlToValidate="DCantidades" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
          </tr>
          <tr>
            <td>Fecha de vencimiento</td>
            <td><asp:DropDownList ID="DVence" runat="server">
                    <asp:ListItem Text="(Ninguna)" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
                    <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td><asp:RequiredFieldValidator ControlToValidate="DVence" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
          </tr>
          <tr>
            <td>Lote</td>
            <td>
                <asp:DropDownList ID="DLote" runat="server">
                    <asp:ListItem Text="(Ninguna)" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
                    <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td><asp:RequiredFieldValidator ControlToValidate="DLote" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
          </tr>
          <tr>
            <td>&nbsp;</td>
            <td align="right">
                <asp:Button ID="BAceptar" runat="server" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button ID="BCancelar" runat="server" CausesValidation="False" Text="Cancelar" onclick="BCancelar_Click" />
                <asp:Button ID="BBuscar" runat="server" CausesValidation="false" Text="Filtrar" 
                    onclick="BBuscar_Click" />
              </td>
            <td><asp:CheckBox runat="server" ID="CFiltrado" CssClass="hide" />
                <asp:HiddenField ID="HTipo" runat="server" />
              </td>
          </tr>
        </table></td>
      </tr>
      <tr>
        <td>
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" EmptyDataText="No se encontraron productos"
                DataKeyNames="id" DataSourceID="SResultados" 
                onselectedindexchanged="GResultado_SelectedIndexChanged" AllowPaging="true" PageSize="50" >
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
                    <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="codigo" />
                    <asp:BoundField DataField="unidad" HeaderText="Unidad" SortExpression="unidad" />
                    <asp:BoundField DataField="empresa" HeaderText="Empresa" SortExpression="empresa" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" />
                    <asp:BoundField DataField="entero" HeaderText="Manejo de cantidades" SortExpression="entero" />
                    <asp:BoundField DataField="vence" HeaderText="Fecha de vencimiento" SortExpression="vence" />
                    <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id,nombre,descripcion,codigo,unidad,empresa 
                        ,CASE vence WHEN 1 THEN 'Obligatoria' WHEN 2 THEN 'Opcional' ELSE 'Ninguna' END AS vence
                        ,CASE lote WHEN 1 THEN 'Obligatoria' WHEN 2 THEN 'Opcional' ELSE 'Ninguna' END AS lote
                        ,CASE entero WHEN 1 THEN 'Entero' ELSE 'Decimal' END AS entero
                        FROM producto 
                        WHERE estado IS NULL
                        AND (@filtrado=0 
                        OR (
                            (@vence=-1 OR vence=@vence)
                            AND (@lote=-1 OR lote=@lote)
                            AND (@cantidades=-1 OR entero=@cantidades)
                            AND (@nombre='_' OR nombre LIKE CONCAT('%', @nombre,'%'))
                            AND (@unidad='_' OR unidad LIKE CONCAT('%', @unidad,'%'))
                            AND (@codigo='_' OR codigo LIKE CONCAT('%', @codigo,'%'))
                            AND (@descripcion='_' OR descripcion LIKE CONCAT('%', @descripcion,'%'))
                        ))
                        " 
                DeleteCommand="UPDATE producto SET estado=-1 WHERE id=@id" >
                <SelectParameters>
                    <asp:ControlParameter ControlID="CFiltrado" PropertyName="Checked" Name="filtrado" />
                    <asp:ControlParameter ControlID="DVence" PropertyName="SelectedValue" Name="vence" />
                    <asp:ControlParameter ControlID="DLote" PropertyName="SelectedValue" Name="lote" />
                    <asp:ControlParameter ControlID="DCantidades" PropertyName="SelectedValue" Name="cantidades" />
                    <asp:ControlParameter ControlID="TNombre" PropertyName="Text" Name="nombre" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TDescripcion" PropertyName="Text" Name="descripcion" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TUnidad" PropertyName="Text" Name="unidad" DefaultValue="_" />
                    <asp:ControlParameter ControlID="TCodigo" PropertyName="Text" Name="codigo" DefaultValue="_" />
                </SelectParameters>
            </asp:SqlDataSource>
          </td>
      </tr>
    </table></td>
  </tr>
</table>
</asp:Panel>
</asp:Content>

