<%@ Page Title="Orden de producción" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="orden.aspx.cs" Inherits="produccion_orden" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .aanalisis 
    {
        color: #FFF;
    }
    .contenedor,.lote
    {
        width: 100px;
    }
    .presentacion 
    {
        width: 100px;
        text-align: right;
    }
    .obtenida 
    {
        font-size: 20px;font-weight: bold; white-space: nowrap;
    }
</style>
<script type="text/javascript">
    $(function() {
        if (hasStorage) {
          if(<%= (bool)ViewState["clean"] ? "true" : "false" %>) {
                localStorage.orden=null;
            } else {
                $("#<%= HProductos.ClientID %>").val(localStorage.orden);
                if($("#<%= HProductos.ClientID %>").val().length) {
                    if(confirm("¿Se ha encontrado una orden sin terminar desea continuarla?")) {
                        $("#<%= BActualizar.ClientID %>").click();
                    } else {
                        localStorage.orden=null;
                        $("#<%= HProductos.ClientID %>").val("");
                    }
                }
            }
        }
    });
    
    var loteTemporal="<%= (string)ViewState["FormatoLote"] %>";
    
    function generarLote() {
    
        var fecha= $("#<%= TFechaInicio.ClientID %>").val();
        if(typeof fecha==="undefined" || fecha.length==0)
            fecha= $("#<%= TFechaProduccion.ClientID %>").val();
        var partes= fecha.split("/");
        if(partes.length<3) {
            $("#<%= TLote.ClientID %>").val("");
        } else {
            var pad = "00";
            var almacen= "<%= ((ing)Master).GetAlmacen() %>";
            $("#<%= TLote.ClientID %>").val(
                loteTemporal
                .replace("%dia%", pad.substring(0, pad.length - partes[0].length) + partes[0])
                .replace("%mes%", pad.substring(0, pad.length - partes[1].length) + partes[1])
                .replace("%ano%",  partes[2])
                .replace("%anoc%", partes[2].substring(0,2))
                .replace("%codigoalmacen%", pad.substring(0, pad.length - almacen.length) + almacen)
            );
        }
    }
    
    addFunctionToStart(function() {
        if (hasStorage) 
          localStorage.orden = $("#<%= HProductos.ClientID %>").val();
        $("#<%= TLote.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "produccion.asmx/GetOrdenP",
                    data: "{ 'q': '" + request.term + "', 'p': '' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.Nombre;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $.goURL("oi=" + ui.item.Id);
                return false;
            }
        });
        $("#<%= TCliente.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetTercero",
                    data: "{ 'q': '" + request.term + "', tipo: 1 }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.Documento + " " + item.Nombre;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + ":" + errorThrown);
                    }
                });
            },
            autoFocus: true,
            minLength: 3,
            search: function() {
                $("#<%= TClienteID.ClientID %>").val("");
            },
            select: function(event, ui) {

                $("#<%= TClienteID.ClientID %>").val(ui.item.Id);
                $("#<%= TCliente.ClientID %>").val(ui.item.Documento + " " + ui.item.Nombre);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0)
                $("#<%= TClienteID.ClientID %>").val("");
        });
        $("#<%= TIngrediente.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': '' }",
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
            search: function() {
                $("#<%= TIngredienteID.ClientID %>").val("");
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TIngredienteID.ClientID %>").val(ui.item.id);
                $("#<%= TIngrediente.ClientID %>").val(ui.item.label);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0)
                $("#<%= TIngredienteID.ClientID %>").val("");
        });

        $("#<%= TProducto.ClientID %>").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "produccion.asmx/GetProducto",
                    data: "{ 'q': '" + request.term + "', 'p': '' }",
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
            search: function() {
                $("#<%= TRecetaID.ClientID %>").val("");
                $("#<%= TProductoID.ClientID %>").val("");
                $("#<%= LEmpresa.ClientID %>").html("");
            },
            autoFocus: true,
            minLength: 3,
            select: function(event, ui) {

                $("#<%= TProductoID.ClientID %>").val(ui.item.id);
                $("#<%= TProducto.ClientID %>").val(ui.item.label);
                $("#<%= TRecetaID.ClientID %>").val(ui.item.IdTabla);
                $("#<%= LEmpresa.ClientID %>").html(ui.item.Empresa);
                loteTemporal=ui.item.FormatoLote.replace("%codigo%", ui.item.codigo);
                generarLote();
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0) {
                $("#<%= TRecetaID.ClientID %>").val("");
                $("#<%= TProductoID.ClientID %>").val("");
                $("#<%= LEmpresa.ClientID %>").html("");
            } else {
                if($("#<%= TCantidad.ClientID %>").val().length!=0 && $("#<%= TProductoID.ClientID %>").val().length!=0)
                    $("#<%= BFormular.ClientID %>").click();
            }
        });
        $("#<%= TCantidad.ClientID %>").change(function() {
            if($("#<%= TCantidad.ClientID %>").val().length!=0 && $("#<%= TProductoID.ClientID %>").val().length!=0)
                $("#<%= BFormular.ClientID %>").click();
        });
        
        $("#<%= TNumero.ClientID %>").change(function() {
            $.goURL("oi=" + $(this).val());
        });
        
        $(".presentacion").autoNumeric('init', { vMin: 0, aSep: '', mDec: 8, aPad: false, aSign: ' Kg.', pSign: 's' });
        $("#<%= TCantidadUnidad1.ClientID %>,#<%= TCantidadPresentacion1.ClientID %>,#<%= TCantidadUnidad2.ClientID %>,#<%= TCantidadPresentacion2.ClientID %>,#<%= TReproceso.ClientID %>").change(function() {
            var total = $("#<%= TCantidadUnidad1.ClientID %>").val() * $("#<%= TCantidadPresentacion1.ClientID %>").val().replace(" Kg.","");
            total += $("#<%= TCantidadUnidad2.ClientID %>").val() * $("#<%= TCantidadPresentacion2.ClientID %>").val().replace(" Kg.", "");
            $("#<%= LObtenida.ClientID %>").text(total);
            total= $("#<%= TCantidad.ClientID %>").val().replace(" Kg.", "")*1-total;
            total-=$("#<%= TReproceso.ClientID %>").val().replace(" Kg.", "")*1;
            $("#<%= LMerma.ClientID %>").text(total + " Kg.");
        }).change();
        
        $("#<%= TFechaProduccion.ClientID %>,#<%= TFechaInicio.ClientID %>").change(generarLote);
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel runat="server" ID="PImprimir" Visible="false">
    <div id="dialog-confirm" title="Imprimir" class="hide"> 
        <p><asp:Label runat="server" ID="LImprimir"></asp:Label></p>
    </div>
    <script type="text/javascript">
        $(function() {
            $("#dialog-confirm").dialog({ resizable: false, modal: true, buttons: { "Imprimir": function() { window.open('<%= ViewState["urlprint"] %>', '_blank'); $(this).dialog("close"); }, "Cancelar": function() { $(this).dialog("close"); } } });
        });
    </script>
</asp:Panel>
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
<ContentTemplate>
<asp:Panel runat="server" DefaultButton="BAceptar">
<table class="tablecollapse">
    <tr>
        <td width="1">Número</td>
        <td width="1"><asp:TextBox runat="server" ID="TNumero" CssClass="likenumero"></asp:TextBox></td>
        <td width="1"></td>
        <td></td>
    </tr>
    <tr>
        <td>Planeación
            <asp:RequiredFieldValidator ControlToValidate="TFecha" ErrorMessage="Por favor ingrese una fecha." ID="RequiredFieldValidator6" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator3" Display="Dynamic" Type="Date">*</asp:RangeValidator>
        </td>
        <td><asp:TextBox runat="server" ID="TFecha" CssClass="fecha"></asp:TextBox></td>
        <td>Programado
        <asp:RangeValidator ControlToValidate="TFecha" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator1" Display="Dynamic" Type="Date">*</asp:RangeValidator></td>
        <td><asp:TextBox runat="server" ID="TFechaProduccion" CssClass="fecha"></asp:TextBox></td>
    </tr>
    <tr>
        <td>Cliente<asp:RequiredFieldValidator ControlToValidate="TClienteID" ErrorMessage="Por favor ingrese una cliente." ID="RequiredFieldValidator1" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3"><asp:TextBox runat="server" ID="TCliente" Width="98%"></asp:TextBox>
        <asp:TextBox runat="server" ID="TClienteID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td></td>
        <td colspan="3"><asp:Label runat="server" ID="LEmpresa"></asp:Label></td>
    </tr>
    <tr>
        <td>Producto<asp:RequiredFieldValidator ControlToValidate="TProductoID" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator2" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3"><asp:TextBox runat="server" ID="TProducto" Width="98%"></asp:TextBox>
        <asp:TextBox runat="server" ID="TProductoID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td nowrap="nowrap">Cantidad programada<asp:RequiredFieldValidator ControlToValidate="TCantidad" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator7" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RegularExpressionValidator1" runat="server" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
        <td>
            <asp:Button runat="server" ID="BFormular" Text="Traer formula" CausesValidation="false" OnClick="BFormular_Click" CssClass="hide" />
            <asp:TextBox runat="server" ID="TCantidad" CssClass="float"></asp:TextBox></td>
        <td>Lote<asp:RequiredFieldValidator ControlToValidate="TLote" ErrorMessage="Por favor ingrese un lote." ID="RequiredFieldValidator5" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td><asp:TextBox runat="server" ID="TLote" CssClass="lote"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Requerimientos</td>
    </tr>
    <tr>
        <td class="tdl">Producto<asp:RequiredFieldValidator ControlToValidate="TIngredienteID" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un producto." ID="RequiredFieldValidator3" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td colspan="3" class="tdr"><asp:TextBox runat="server" ID="TIngrediente" Width="98%"></asp:TextBox><asp:TextBox runat="server" ID="TIngredienteID" CssClass="hide"></asp:TextBox><asp:TextBox runat="server" ID="TRecetaID" CssClass="hide"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Cantidad
            <asp:RequiredFieldValidator ControlToValidate="TCantidadIngrediente" ValidationGroup="agregar" ErrorMessage="Por favor ingrese una cantidad." ID="RequiredFieldValidator4" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9]+" ID="RDecimal" runat="server" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidadIngrediente" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
            </td>
        <td><asp:TextBox runat="server" ID="TCantidadIngrediente" CssClass="float"></asp:TextBox></td>
        <td><asp:HiddenField runat="server" ID="HProductos" /><asp:Button runat="server" ID="BMas" Text="Agregar" ValidationGroup="agregar" OnClick="BMas_Click" />
        <asp:Button ID="BActualizar" runat="server" Text="Actualizar" CssClass="hide" CausesValidation="false" onclick="BActualizar_Click" />
        </td>
        <td class="tdr"></td>
    </tr>
    <tr>
        <td colspan="4" class="tdl tdr tdb" align="center">
            <asp:GridView ID="GProductos" runat="server" AutoGenerateColumns="False" Width="100%"
                    DataKeyNames="IdGrid,id" 
                    EmptyDataText="No hay productos" onrowdeleting="GProductos_RowDeleting" ShowFooter="true" >
                    <RowStyle CssClass="GridRow"></RowStyle>
                    <FooterStyle CssClass="GridFooter"></FooterStyle>
                    <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                    <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                    <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                    <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                    <Columns>
                        <asp:CommandField ButtonType="Image" DeleteImageUrl="~/img/eliminar.gif" ShowDeleteButton="True" />
                        <asp:BoundField DataField="codigoExtra" HeaderText="Código" SortExpression="codigoExtra" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100%" FooterStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:0.########}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" />
                    </Columns>
                </asp:GridView>
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="PEditar" Visible="false">
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader"><asp:HyperLink ID="HProduccion" CssClass="aanalisis" runat="server" Text="Datos de proceso" NavigateUrl="~/produccion/proceso.aspx"></asp:HyperLink></td>
    </tr>
    <tr>
        <td class="tdl">Marmita</td>
        <td><asp:DropDownList ID="DMarmita" runat="server" DataSourceID="SMarmita" AppendDataBoundItems="true"
                DataTextField="nombre" DataValueField="id">
                <asp:ListItem Text="(Ninguna)" Value=""></asp:ListItem>
            </asp:DropDownList>
            <asp:SqlDataSource ID="SMarmita" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT id, nombre FROM varios WHERE tipo=1001 AND szReservado IS NULL ORDER BY nombre">
            </asp:SqlDataSource></td>
        <td>Contenedor</td>
        <td class="tdr"><asp:TextBox runat="server" ID="TContenedor" CssClass="contenedor"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Fecha<asp:RequiredFieldValidator ControlToValidate="TFechaInicio" ErrorMessage="Por favor ingrese un inicio." ID="RequiredFieldValidator8" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
        <asp:RangeValidator ControlToValidate="TFechaInicio" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator2" Display="Dynamic" Type="Date">*</asp:RangeValidator>
        </td>
        <td><asp:TextBox runat="server" ID="TFechaInicio" CssClass="fecha"></asp:TextBox></td>
        <td>Operarios<asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="REntero" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TOperariosProceso" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
        <td class="tdr"><asp:TextBox runat="server" ID="TOperariosProceso" CssClass="entero"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Inicio<asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="TInicioProceso" ErrorMessage="Por favor ingrese una hora valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
        <td><asp:TextBox runat="server" ID="TInicioProceso" CssClass="hora"></asp:TextBox></td>
        <td>Fin<asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="TFinProceso" ErrorMessage="Por favor ingrese una hora valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
        <td class="tdr"><asp:TextBox runat="server" ID="TFinProceso" CssClass="hora"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Empaque</td>
    </tr>
    <tr>
        <td class="tdl">Fecha
        <asp:RangeValidator ControlToValidate="TFechaEmpaque" runat="server" ErrorMessage="Por favor ingrese una fecha valida." MinimumValue="01/01/1930" MaximumValue="01/01/2100" ID="RangeValidator4" Display="Dynamic" Type="Date">*</asp:RangeValidator>
        </td>
        <td><asp:TextBox runat="server" ID="TFechaEmpaque" CssClass="fecha"></asp:TextBox></td>
        <td>Operarios<asp:RegularExpressionValidator ValidationExpression="[0-9]+" ID="RegularExpressionValidator5" runat="server" ErrorMessage="Por favor ingrese un numero entero." ControlToValidate="TOperariosEmpaque" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
        <td class="tdr"><asp:TextBox runat="server" ID="TOperariosEmpaque" CssClass="entero"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Inicio<asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="TInicioEmpaque" ErrorMessage="Por hora ingrese una fecha valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
        <td><asp:TextBox runat="server" ID="TInicioEmpaque" CssClass="hora"></asp:TextBox></td>
        <td>Fin<asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="TFinEmpaque" ErrorMessage="Por favor ingrese una hora valida" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^(0[0-9]|1\d|2[0-3]):([0-5]\d)$" runat="server" >*</asp:RegularExpressionValidator></td>
        <td class="tdr"><asp:TextBox runat="server" ID="TFinEmpaque" CssClass="hora"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader">Cantidad Obtenida</td>
    </tr>
    <tr>
        <td class="tdl">Unidades 1<asp:RegularExpressionValidator ControlToValidate="TCantidadUnidad1" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
        </td>
        <td><asp:TextBox runat="server" ID="TCantidadUnidad1" CssClass="float"></asp:TextBox></td>
        <td nowrap="nowrap">Presentación 1<asp:RegularExpressionValidator ControlToValidate="TCantidadPresentacion1" ValidationExpression="^[0-9]*[.]?[0-9]+ Kg.$" ID="RegularExpressionValidator6" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
        <td class="tdr"><asp:TextBox runat="server" ID="TCantidadPresentacion1" CssClass="presentacion"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Unidades 2<asp:RegularExpressionValidator ControlToValidate="TCantidadUnidad2" ValidationExpression="[0-9]*[.]?[0-9]+" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator>
        </td>
        <td><asp:TextBox runat="server" ID="TCantidadUnidad2" CssClass="float"></asp:TextBox></td>
        <td>Presentación 2<asp:RegularExpressionValidator ControlToValidate="TCantidadPresentacion2" ValidationExpression="^[0-9]*[.]?[0-9]+ Kg.$" ID="RegularExpressionValidator7" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
        <td class="tdr"><asp:TextBox runat="server" ID="TCantidadPresentacion2" CssClass="presentacion"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="tdl">Reproceso<asp:RegularExpressionValidator ControlToValidate="TReproceso" ValidationExpression="^[0-9]*[.]?[0-9]+ Kg.$" ID="RegularExpressionValidator9" runat="server" ErrorMessage="Por favor ingrese un numero decimal." Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
        <td><asp:TextBox runat="server" ID="TReproceso" CssClass="presentacion"></asp:TextBox> </td>
        <td>Merma</td>
        <td class="tdr"><asp:Label runat="server" ID="LMerma" Text="0 Kg."></asp:Label></td>
    </tr>
    <tr>
        <td colspan="3" class="tdl"></td>
        <td class="tdr" nowrap="nowrap">Cantidad obtenida: <asp:Label runat="server" ID="LObtenida" CssClass="obtenida">0</asp:Label> Kg.</td>
    </tr>
    <tr>
        <td class="tdl tdb">Observaciones</td>
        <td colspan="3" class="tdr tdb"><asp:TextBox runat="server" id="TObservacionesProceso" Width="99%"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader"><asp:HyperLink ID="HAnalisis" CssClass="aanalisis" runat="server" Text="Análisis físico químico" NavigateUrl="~/produccion/laboratorio.aspx"></asp:HyperLink></td>
    </tr>
    <tr>
        <td colspan="4" class="tdl tdr tdb" align="center">
            <asp:GridView ID="GAnalisis" runat="server" AutoGenerateColumns="False" DataSourceID="SAnalisis" Width="100%" 
                DataKeyNames="id" EmptyDataText="No se encontraron análisis" >
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="peso_lavado" HeaderText="Peso lavado" SortExpression="peso_lavado" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="peso_escurrido" HeaderText="Peso escurrido" SortExpression="peso_escurrido" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Brix" HeaderText="Brix" SortExpression="Brix" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                    <asp:BoundField DataField="Acidez" HeaderText="Acidez" SortExpression="Acidez" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                    <asp:BoundField DataField="Viscosidad" HeaderText="Viscosidad" SortExpression="Viscosidad" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                    <asp:BoundField DataField="Consistencia" HeaderText="Consistencia" SortExpression="Consistencia" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                    <asp:BoundField DataField="pH" HeaderText="pH" SortExpression="pH" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                    <asp:BoundField DataField="Densidad" HeaderText="Densidad" SortExpression="Densidad" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                    <asp:BoundField DataField="ChartColor" HeaderText="Chart-Color" SortExpression="ChartColor" ItemStyle-HorizontalAlign="Right" NullDisplayText="(NA)" />
                    <asp:BoundField DataField="observaciones" HeaderText="Observaciones" SortExpression="observaciones" ItemStyle-HorizontalAlign="left" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SAnalisis" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT 
                    analisis_fisico_quimico.id,analisis_fisico_quimico.fecha,analisis_fisico_quimico.peso_lavado,analisis_fisico_quimico.peso_escurrido,analisis_fisico_quimico.observaciones 
                    ,(SELECT valor FROM analisis_fisico_quimico_parametros LEFT JOIN varios ON (varios.id=analisis_fisico_quimico_parametros.parametro) WHERE analisis_fisico_quimico.id=analisis_fisico_quimico_parametros.analisis AND varios.nombre='Brix')  AS Brix
                    ,(SELECT valor FROM analisis_fisico_quimico_parametros LEFT JOIN varios ON (varios.id=analisis_fisico_quimico_parametros.parametro) WHERE analisis_fisico_quimico.id=analisis_fisico_quimico_parametros.analisis AND varios.nombre='Acidez')  AS Acidez
                    ,(SELECT valor FROM analisis_fisico_quimico_parametros LEFT JOIN varios ON (varios.id=analisis_fisico_quimico_parametros.parametro) WHERE analisis_fisico_quimico.id=analisis_fisico_quimico_parametros.analisis AND varios.nombre='Viscosidad')  AS Viscosidad
                    ,(SELECT valor FROM analisis_fisico_quimico_parametros LEFT JOIN varios ON (varios.id=analisis_fisico_quimico_parametros.parametro) WHERE analisis_fisico_quimico.id=analisis_fisico_quimico_parametros.analisis AND varios.nombre='Consistencia')  AS Consistencia
                    ,(SELECT valor FROM analisis_fisico_quimico_parametros LEFT JOIN varios ON (varios.id=analisis_fisico_quimico_parametros.parametro) WHERE analisis_fisico_quimico.id=analisis_fisico_quimico_parametros.analisis AND varios.nombre='pH')  AS pH
                    ,(SELECT valor FROM analisis_fisico_quimico_parametros LEFT JOIN varios ON (varios.id=analisis_fisico_quimico_parametros.parametro) WHERE analisis_fisico_quimico.id=analisis_fisico_quimico_parametros.analisis AND varios.nombre='Densidad') AS Densidad
                    ,(SELECT valor FROM analisis_fisico_quimico_parametros LEFT JOIN varios ON (varios.id=analisis_fisico_quimico_parametros.parametro) WHERE analisis_fisico_quimico.id=analisis_fisico_quimico_parametros.analisis AND varios.nombre='Chart-Color') AS ChartColor
                    FROM analisis_fisico_quimico
                    WHERE analisis_fisico_quimico.orden=@orden
                    ORDER BY analisis_fisico_quimico.fecha DESC">
                <SelectParameters>
                    <asp:Parameter Name="orden" />
                </SelectParameters>
            </asp:SqlDataSource>
        </td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    </asp:PlaceHolder>
    <tr>
        <td>Observaciones</td>
        <td colspan="3"><asp:TextBox runat="server" ID="TObservaciones" Width="98%"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;<asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label><asp:ValidationSummary runat="server" /></td>
    </tr>
    <tr>
        <td colspan="4" align="right">
            <asp:Button runat="server" ID="BAceptar" Text="Aceptar" 
                onclick="BAceptar_Click" />
            <asp:Button runat="server" ID="BCancelar" Text="Cancelar" 
                CausesValidation="false" onclick="BCancelar_Click" />
        </td>
    </tr>
</table>
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>