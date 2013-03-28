<%@ Page Title="Productos" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="productos.aspx.cs" Inherits="administrador_productos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

    <script type="text/javascript">
        addFunctionToStart(function() {
            $("#accordion").accordion({
                autoHeight: false,
                navigation: true,
                collapsible: true,
                active: false
            });
            $("#<%= TNombre.ClientID %>").autocomplete({
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
            $("#<%= DVence.ClientID %>").change(function() {
                $("#<%= TDiasVencimiento.ClientID %>").attr("disabled", $(this).val() == "0");
            }).change();
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
            <td>Grupo</td>
            <td>
            <asp:DropDownList ID="DGrupo" runat="server" AppendDataBoundItems="true" 
                    DataSourceID="SGrupo" DataTextField="nombre" DataValueField="id">
                    <asp:ListItem Text="(Ninguno)" Value="0"></asp:ListItem>
            </asp:DropDownList>
            <asp:SqlDataSource ID="SGrupo" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre FROM varios WHERE tipo=1 AND estado IS NULL ORDER BY nombre">
            </asp:SqlDataSource>
            </td>
            <td>&nbsp;</td>
          </tr>
          <tr>
              <td>
                  Manejo de cantidades</td>
              <td>
                  <asp:DropDownList ID="DCantidades" runat="server">
                      <asp:ListItem Text="(Ninguna)" Value="-1"></asp:ListItem>
                      <asp:ListItem Text="Decimal" Value="0"></asp:ListItem>
                      <asp:ListItem Text="Entero" Value="1"></asp:ListItem>
                  </asp:DropDownList>
              </td>
              <td>
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                      ControlToValidate="DCantidades" Display="Dynamic" 
                      ErrorMessage="Por favor seleccione un valor." InitialValue="-1" 
                      SetFocusOnError="true"></asp:RequiredFieldValidator>
              </td>
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
            <td>
            <asp:RequiredFieldValidator ControlToValidate="DVence" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
          </tr>
          <tr>
              <td>Días para vencimiento</td>
              <td><asp:TextBox runat="server" ID="TDiasVencimiento" CssClass="entero"></asp:TextBox></td>
              <td><asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TDiasVencimiento" Display="Dynamic" SetFocusOnError="True"></asp:RegularExpressionValidator></td>
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
            <td>Serial</td>
            <td><asp:DropDownList ID="DSerial" runat="server">
                    <asp:ListItem Text="(Ninguna)" Value="-1"></asp:ListItem>
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
                    <asp:ListItem Text="(Ninguna)" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="Obligatoria" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
                    <asp:ListItem Text="No requerida" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td><asp:RequiredFieldValidator ControlToValidate="DVence" InitialValue="-1" ErrorMessage="Por favor seleccione un valor." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
          </tr>
          <tr>
            <td>Último Costo</td>
            <td><asp:TextBox runat="server" ID="TCosto" CssClass="float"></asp:TextBox></td>
            <td></td>
          </tr>
          <tr>
            <td colspan="3">
            <asp:Panel runat="server" ID="PCostos" Visible="false">
            <div id="accordion">
                    <h3><a href="#">Costos por proveedor</a></h3>
                    <div class="hide">
                        <asp:GridView ID="GCostos" runat="server" DataSourceID="SCostos" 
                            AutoGenerateColumns="false" AllowSorting="True" AllowPaging="true" EmptyDataText="No hay proveedores">
                            <RowStyle CssClass="GridRow"></RowStyle>
                            <FooterStyle CssClass="GridFooter"></FooterStyle>
                            <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                            <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                            <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                            <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                            <Columns>
                                <asp:BoundField HeaderText="Proveedor" DataField="nombre" SortExpression="nombre" />
                                <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="costo" HeaderText="Costo" SortExpression="costo" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:F2}" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SCostos" runat="server"
                                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                                SelectCommand="select tercero.nombre,costo_proveedor.costo,costo_proveedor.fecha
                                        FROM costo_proveedor
                                        LEFT JOIN tercero ON (tercero.id=costo_proveedor.proveedor)
                                        WHERE costo_proveedor.producto=@producto
                                        ORDER BY costo_proveedor.fecha DESC
                                        ;">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="HId" PropertyName="Value" Name="producto" />
                                </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
                </asp:Panel>
            </td>
          </tr>
          <tr>
              <td>&nbsp;</td>
              <td align="right">
                  <asp:Button ID="BAceptar" runat="server" onclick="BAceptar_Click" 
                      Text="Aceptar" />
                  <asp:Button ID="BCancelar" runat="server" CausesValidation="False" 
                      onclick="BCancelar_Click" Text="Cancelar" />
                  <asp:Button ID="BBuscar" runat="server" CausesValidation="false" 
                      onclick="BBuscar_Click" Text="Filtrar" />
              </td>
              <td>
                  <asp:CheckBox ID="CFiltrado" runat="server" CssClass="hide" />
                  <asp:HiddenField ID="HTipo" runat="server" />
              </td>
          </tr>
        </table></td>
      </tr>
      <tr>
        <td>
            <asp:GridView ID="GResultado" runat="server" AutoGenerateColumns="False" EmptyDataText="No se encontraron productos"
                DataKeyNames="id" DataSourceID="SResultados" AllowSorting="true"
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
                    <asp:BoundField DataField="grupo" HeaderText="Grupo" SortExpression="grupo" />
                    <asp:BoundField DataField="entero" HeaderText="Manejo de cantidades" SortExpression="entero" />
                    <asp:BoundField DataField="vence" HeaderText="Fecha de vencimiento" SortExpression="vence" />
                    <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                    <asp:BoundField DataField="serial" HeaderText="Serial" SortExpression="serial" />
                    <asp:BoundField DataField="estado" HeaderText="Estado" SortExpression="estado" />
                    <asp:BoundField DataField="costo" HeaderText="Costo" SortExpression="costo" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:F2}" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SResultados" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT producto.id,producto.nombre,producto.descripcion,producto.codigo,producto.unidad,producto.empresa 
                        ,CASE producto.vence WHEN 1 THEN 'Obligatoria' WHEN 2 THEN 'Opcional' ELSE 'Ninguna' END AS vence
                        ,CASE producto.lote WHEN 1 THEN 'Obligatoria' WHEN 2 THEN 'Opcional' ELSE 'Ninguna' END AS lote
                        ,CASE producto.nEstado WHEN 1 THEN 'Obligatoria' WHEN 2 THEN 'Opcional' ELSE 'Ninguna' END AS estado
                        ,CASE producto.serial WHEN 1 THEN 'Obligatoria' WHEN 2 THEN 'Opcional' ELSE 'Ninguna' END AS serial
                        ,CASE producto.entero WHEN 1 THEN 'Entero' ELSE 'Decimal' END AS entero
                        ,varios.nombre AS grupo
                        ,producto.costo
                        FROM producto 
                        LEFT JOIN varios ON (producto.grupo=varios.id)
                        WHERE producto.estado IS NULL
                        AND (@filtrado=0 
                        OR (
                            (@vence=-1 OR producto.vence=@vence)
                            AND (@lote=-1 OR producto.lote=@lote)
                            AND (@cantidades=-1 OR producto.entero=@cantidades)
                            AND (@serial=-1 OR producto.serial=@serial)
                            AND (@estado=-1 OR producto.nEstado=@estado)
                            AND (@nombre='_' OR producto.nombre LIKE CONCAT('%', @nombre,'%'))
                            AND (@unidad='_' OR producto.unidad LIKE CONCAT('%', @unidad,'%'))
                            AND (@codigo='_' OR producto.codigo LIKE CONCAT('%', @codigo,'%'))
                            AND (@descripcion='_' OR producto.descripcion LIKE CONCAT('%', @descripcion,'%'))
                        ))
                        ORDER BY codigo,nombre
                        " 
                DeleteCommand="UPDATE producto SET estado=-1 WHERE id=@id" >
                <SelectParameters>
                    <asp:ControlParameter ControlID="CFiltrado" PropertyName="Checked" Name="filtrado" />
                    <asp:ControlParameter ControlID="DVence" PropertyName="SelectedValue" Name="vence" />
                    <asp:ControlParameter ControlID="DLote" PropertyName="SelectedValue" Name="lote" />
                    <asp:ControlParameter ControlID="DCantidades" PropertyName="SelectedValue" Name="cantidades" />
                    <asp:ControlParameter ControlID="DEstado" PropertyName="SelectedValue" Name="estado" />
                    <asp:ControlParameter ControlID="DSerial" PropertyName="SelectedValue" Name="serial" />
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

