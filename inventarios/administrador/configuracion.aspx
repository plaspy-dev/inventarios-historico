<%@ Page Title="Configuración" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="configuracion.aspx.cs" Inherits="administrador_configuracion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    $(function() {
        $("#tpr").tabs();
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="tpr">
    <ul>
        <li><a href="#dConfiguracion">Configuración</a></li>
        <li><a href="#dConsecutivos">Consecutivos</a></li>
    </ul>
    <div id="dConfiguracion">
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
                <td>Precios</td>
                <td><asp:DropDownList ID="DValores" runat="server">
                    <asp:ListItem Text="Obligatorio" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Opcional" Value="2"></asp:ListItem>
                    <asp:ListItem Text="No requerido" Value="0"></asp:ListItem>
                </asp:DropDownList>
                </td>
                <td></td>    
            </tr>
            <tr>
                <td>Tipo precios</td>
                <td><asp:DropDownList ID="DTipoValores" runat="server">
                    <asp:ListItem Text="Cerrado" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Abierto" Value="0"></asp:ListItem>
                </asp:DropDownList>
                </td>
                <td></td>    
            </tr>
            <tr>
                <td>Como salen los productos</td>
                <td>
                    <asp:DropDownList runat="server" ID="DSeleccionProducto">
                        <asp:ListItem Text="Automático (El sistema decide que producto sale, el mas antiguo o por vencer)" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Semi-Automático (El sistema decide el producto pero el usuario puede seleccionar manual)" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Manual (El usuario decide que productos salen)" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td colspan="3" class="GridHeader">Productos</td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CVencimiento" Text="Los productos manejan fecha de vencimiento" /></td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CLote" Text="Los productos manejan lote" /></td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CSerial" Text="Los productos manejan serial" /></td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CEstado" Text="Los productos manejan estado" /></td>
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
                <td colspan="3" class="GridHeader">Configuración inicial de productos</td>
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
              <td></td>
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
            <td>Días para alerta de vencimiento</td>
            <td><asp:TextBox runat="server" ID="TDiasVencimiento" CssClass="entero"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ControlToValidate="TDiasVencimiento" ErrorMessage="Por favor ingrese unos días." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TDiasVencimiento" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
          </tr>
          <tr>
            <td colspan="3">
                <asp:Label runat="server" ID="LMensaje"></asp:Label>
            </td>
          </tr>
            <tr>
                <td colspan="3" class="GridHeader">Entradas</td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CTerceroEntrada" Text="El proveedor de la entrada es obligatorio" /></td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CDocumentoEntrada" Text="El documento equivalente de la entrada es obligatorio" /></td>
            </tr>
            <tr>
                <td colspan="3" class="GridHeader">Salidas</td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CTerceroSalida" Text="El cliente en la salida es obligatorio" /></td>
            </tr>
            <tr>
                <td colspan="3"><asp:CheckBox runat="server" ID="CDocumentoSalida" Text="El documento equivalente de la salida es obligatorio" /></td>
            </tr>
            <tr>
                <td>Fecha de entrega</td>
                <td><asp:DropDownList ID="DEntrega" runat="server">
                    <asp:ListItem Text="Los productos salen de inventario solo con la fecha de entrega" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Los productos salen de inventario inmediatamente" Value="1"></asp:ListItem>
                </asp:DropDownList></td>
                <td></td>
            </tr>
          <tr>
            <td colspan="3" align="right">
                <asp:Button runat="server" ID="BAceptar" Text="Aceptar" onclick="BAceptar_Click" />
                <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
            </td>
          </tr>
        </table>
    </div>
    <div id="dConsecutivos">
        <asp:UpdatePanel runat="server" ID="UConsecutivos" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="GAlmacenes" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="id" DataSourceID="SAlmaceness" AllowSorting="true" onselectedindexchanged="GAlmacenes_SelectedIndexChanged">
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
                        <asp:CommandField ButtonType="Image" ShowSelectButton="True" SelectImageUrl="~/img/edit.gif" />
                        <asp:BoundField DataField="nombre" HeaderText="Almacén" SortExpression="nombre" />
                        <asp:BoundField DataField="entrada" HeaderText="Entrada" SortExpression="entrada" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="traslado" HeaderText="Traslado" SortExpression="traslado" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="salida" HeaderText="Salida" SortExpression="salida" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SAlmaceness" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                    ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                    SelectCommand="SELECT almacen.id,almacen.nombre
                            ,entrada.AUTO_INCREMENT AS entrada
                            ,salida.AUTO_INCREMENT AS salida
                            ,traslado.AUTO_INCREMENT AS traslado
                            FROM almacen
                            LEFT JOIN INFORMATION_SCHEMA.TABLES AS entrada ON (entrada.TABLE_SCHEMA='pulpafruit' AND entrada.TABLE_NAME LIKE 'entrada%' AND entrada.TABLE_NAME NOT LIKE 'entrada_items%' AND almacen.id=CAST(REPLACE(entrada.TABLE_NAME,'entrada','') AS UNSIGNED))
                            LEFT JOIN INFORMATION_SCHEMA.TABLES AS salida ON (salida.TABLE_SCHEMA='pulpafruit' AND salida.TABLE_NAME LIKE 'salida%' AND salida.TABLE_NAME NOT LIKE 'salida_items%' AND almacen.id=CAST(REPLACE(salida.TABLE_NAME,'salida','') AS UNSIGNED))
                            LEFT JOIN INFORMATION_SCHEMA.TABLES AS traslado ON (traslado.TABLE_SCHEMA='pulpafruit' AND traslado.TABLE_NAME LIKE 'traslado%' AND traslado.TABLE_NAME NOT LIKE 'traslado_items%' AND almacen.id=CAST(REPLACE(traslado.TABLE_NAME,'traslado','') AS UNSIGNED))
                            WHERE estado IS NULL
                            ;">
                </asp:SqlDataSource>
                <asp:Panel runat="server" ID="PAlmacen" Visible="false">
                    <table>
                        <tr>
                            <td>Entrada</td>
                            <td>Traslado</td>
                            <td>Salida</td>
                        </tr>
                        <tr>
                            <td><asp:TextBox runat="server" ID="TEntrada" CssClass="entero"></asp:TextBox></td>
                            <td><asp:TextBox runat="server" ID="TTraslado" CssClass="entero"></asp:TextBox></td>
                            <td><asp:TextBox runat="server" ID="TSalida" CssClass="entero"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td colspan="3" align="right">
                                <asp:Button runat="server" ID="BEditar" Text="Editar" ValidationGroup="almacen" OnClick="BEditar_Click" />
                                <asp:Button runat="server" ID="BCancelarEdicion" Text="Cancelar" CausesValidation="false" OnClick="BCancelarEdicion_Click"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
</asp:Content>

