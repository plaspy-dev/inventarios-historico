<%@ Page Title="Transformaciones" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="transformaciones.aspx.cs" Inherits="administrador_transformaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    addFunctionToStart(function() {

        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': null }",
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
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            minLength: 3,
            autoFocus: true,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                if (ui.item.unidad && ui.item.unidad != "")
                    $("#sunidad").text(ui.item.unidad);
                else $("#sunidad").text("");
                return false;
            }
        });

        $("#<%= TProducto2.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': null }",
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
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            minLength: 3,
            autoFocus: true,
            select: function(event, ui) {

                $("#<%= TProductoID2.ClientID %>").val(ui.item.id);
                $("#<%= TProducto2.ClientID %>").val(ui.item.label);
                if (ui.item.unidad && ui.item.unidad != "")
                    $("#sunidad2").text(ui.item.unidad);
                else $("#sunidad2").text("");
                return false;
            }
        });

        $("#<%= TProporcion.ClientID %>").change(function() {
            $("#cantidad").text($(this).val());
            $("#cantidad2").text("1");
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
            <td>Producto 1</td>
            <td>
                <asp:HiddenField ID="HId" runat="server" />
                <asp:HiddenField ID="HId2" runat="server" />
                <asp:TextBox ID="TProducto" Width="98%" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox>
              </td>
            <td><span id="cantidad"></span> <span id="sunidad"></span>
                <asp:RequiredFieldValidator ControlToValidate="TProductoID" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
              </td>
          </tr>
          <tr>
            <td>Producto 2</td>
            <td>
                <asp:TextBox ID="TProducto2" Width="98%" runat="server"></asp:TextBox><asp:TextBox runat="server" ID="TProductoID2" CssClass="hide"></asp:TextBox>
              </td>
              <td align="left">
                <span id="cantidad2"></span> <span id="sunidad2"></span>
                <asp:RequiredFieldValidator ControlToValidate="TProductoID2" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
              </td>
          </tr>
          <tr>
            <td>Proporción</td>
            <td>
                <asp:TextBox runat="server" CssClass="float" ID="TProporcion"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="TProporcion" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
              </td>
              <td align="right">
              </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
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
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" 
                DataKeyNames="producto1,producto2" DataSourceID="SResultados" Width="100%" 
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
                    <asp:BoundField DataField="producto1nombre" HeaderText="Producto 1" SortExpression="producto1nombre" ItemStyle-Width="50%" />
                    <asp:BoundField DataField="producto2nombre" HeaderText="Producto 2" SortExpression="producto2nombre" ItemStyle-Width="50%" />
                    <asp:BoundField DataField="proporcion" HeaderText="Proporción" SortExpression="proporcion" ItemStyle-HorizontalAlign="Right" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT transformacion.producto1,transformacion.producto2
                            ,producto1.nombre AS producto1nombre,producto2.nombre AS producto2nombre,transformacion.proporcion
                            FROM transformacion
                            LEFT JOIN producto AS producto1 ON (producto1.id=transformacion.producto1)
                            LEFT JOIN producto AS producto2 ON (producto2.id=transformacion.producto2);"
                DeleteCommand="DELETE FROM transformacion WHERE producto1=@producto1 AND producto2=@producto2" >
                <SelectParameters>
                </SelectParameters>
            </asp:SqlDataSource>
          </td>
      </tr>
    </table></td>
  </tr>
</table>
</asp:Panel>
</asp:Content>