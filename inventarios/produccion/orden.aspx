<%@ Page Title="Orden de producción" Language="C#" MasterPageFile="~/ing.master" AutoEventWireup="true" CodeFile="orden.aspx.cs" Inherits="produccion_orden" %>
<%@ Register Src="~/almacen/imprimir.ascx" TagName="Imprimir" TagPrefix="uc" %>
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
    #<%= GAnalisis.ClientID %> .GridHeader,#<%= GMicro.ClientID %> .GridHeader 
    {
        font-size: 10px;
    }
    .esterilidad tr td:nth-child(odd),.sensorial tr td:nth-child(odd) { width: 146px;}
    .esterilidad tr td:nth-child(even),.sensorial tr td:nth-child(even) { width: 129px;}
    .esterilidad tr td { font-size: 12px;}
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
          
        $("#<%= BAnular.ClientID %>").click(function() { 
            addDialog("#<%= PAnular.ClientID %>", "#dAnular", true);
            return false; 
        });
        $("#<%= BAnularReal.ClientID %>").click(function() { if(typeof Page_IsValid === 'undefined' || Page_IsValid || typeof this.ejecutar === 'undefined') return confirm('¿Esta seguro de anular este documento?. Esta acción no se puede deshacer.'); return false; });
        $("#<%= BCancelarAnular.ClientID %>").click(function() {
            $("#dAnular").dialog("close");
            return false;
        });
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
                if($("#<%= TCantidad.ClientID %>").val().replace(" Kg.","").length!=0 && $("#<%= TProductoID.ClientID %>").val().length!=0)
                    $("#<%= BFormular.ClientID %>").click();
            }
        });
        $("#<%= TCantidad.ClientID %>").change(function() {
            if($("#<%= TCantidad.ClientID %>").val().replace(" Kg.","").length!=0 && $("#<%= TProductoID.ClientID %>").val().length!=0)
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
            if(total>0) {
                total= $("#<%= TCantidad.ClientID %>").val().replace(" Kg.", "")*1-total;
                total-=$("#<%= TReproceso.ClientID %>").val().replace(" Kg.", "")*1;
                $("#<%= LMerma.ClientID %>").text(total + " Kg.");
            } else $("#<%= LMerma.ClientID %>").text("--");
        }).change();
        
        $("#<%= TFechaProduccion.ClientID %>,#<%= TFechaInicio.ClientID %>").change(generarLote);
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<uc:Imprimir runat="server" ID="UCImprimir" />
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
<ContentTemplate>
<asp:Panel runat="server" ID="PAnulado" Visible="false" CssClass="anulado"><asp:Label runat="server" ID="LAnulado" Text=""></asp:Label></asp:Panel>
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
        <asp:RegularExpressionValidator ValidationExpression="[0-9]*[.]?[0-9] Kg.+" ID="RegularExpressionValidator1" runat="server" ErrorMessage="Por favor ingrese un numero decimal." ControlToValidate="TCantidad" Display="Dynamic" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
        <td>
            <asp:Button runat="server" ID="BFormular" Text="Traer formula" CausesValidation="false" OnClick="BFormular_Click" CssClass="hide" />
            <asp:TextBox runat="server" ID="TCantidad" CssClass="presentacion"></asp:TextBox></td>
        <td>Lote<asp:RequiredFieldValidator ControlToValidate="TLote" Enabled="false" ErrorMessage="Por favor ingrese un lote." ID="RequiredFieldValidator5" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator></td>
        <td><asp:TextBox runat="server" ID="TLote" CssClass="lote"></asp:TextBox></td>
    </tr>
    <tr>
        <td><asp:Label runat="server" Text="Marmita" ID="LMarmita"></asp:Label></td>
        <td><asp:DropDownList ID="DMarmitaI" runat="server" DataSourceID="SMarmita" AppendDataBoundItems="true"
                DataTextField="nombre" DataValueField="id">
                <asp:ListItem Text="(Ninguna)" Value=""></asp:ListItem>
            </asp:DropDownList></td>
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
        <td>Lote<asp:RequiredFieldValidator ControlToValidate="TLoteIngrediente" ValidationGroup="agregar" ErrorMessage="Por favor ingrese un lote." ID="RLoteIngrediente" runat="server" Display="Dynamic" SetFocusOnError="true">*</asp:RequiredFieldValidator>
        <asp:HiddenField runat="server" ID="HProductos" />
        </td>
        <td class="tdr">
        <asp:TextBox runat="server" ID="TLoteIngrediente" CssClass="lote"></asp:TextBox>
        <asp:Button runat="server" ID="BMas" Text="Agregar" ValidationGroup="agregar" OnClick="BMas_Click" />
        <asp:Button ID="BActualizar" runat="server" Text="Actualizar" CssClass="hide" CausesValidation="false" onclick="BActualizar_Click" />
        </td>
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
                        <asp:BoundField DataField="codigoExtra" HeaderText="Código" SortExpression="codigoExtra" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100%" FooterStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad" DataFormatString="{0:0.########}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="unidad" HeaderText="Un." SortExpression="unidad" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="lote" HeaderText="Lote" SortExpression="lote" ItemStyle-HorizontalAlign="Left" />
                        <asp:TemplateField HeaderText="Lote " SortExpression="lote">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="TLote" Text='<%# Eval("lote") %>' CssClass="lote"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
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
                SelectCommand="SELECT id, nombre FROM varios WHERE tipo=1001 AND estado IS NULL ORDER BY nombre">
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
            <asp:GridView ID="GAnalisis" runat="server" AutoGenerateColumns="False" Width="100%" 
                DataKeyNames="IdGrid" EmptyDataText="No se encontraron análisis" OnRowDataBound="GAnalisis_RowDataBound">
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" ItemStyle-HorizontalAlign="Right"  DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="observaciones" HeaderText="Observaciones" SortExpression="observaciones" ItemStyle-HorizontalAlign="left" ItemStyle-Width="100%" />
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5" class="GridHeader">Análisis sensorial</td>
    </tr>
    <tr>
        <td class="tdl">Chart-Color</td>
        <td><asp:TextBox runat="server" ID="TChartColor" CssClass="chart-color"></asp:TextBox></td>
        <td colspan="3" class="tdr"></td>
    </tr>
    <tr>
        <td colspan="5" class="tdl tdr tdb">
            <asp:CheckBoxList RepeatColumns="4" runat="server" ID="CSensorial" DataSourceID="SSensorial" DataTextField="nombre" DataValueField="id" CssClass="sensorial">
            </asp:CheckBoxList>
            <asp:SqlDataSource ID="SSensorial" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT varios.id, varios.nombre
                        FROM varios
                        WHERE varios.tipo=1005 AND varios.estado IS NULL
                        ORDER BY varios.nombre;">
            </asp:SqlDataSource>
        </td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4" class="GridHeader"><asp:HyperLink ID="HMicro" CssClass="aanalisis" runat="server" Text="Análisis microbiológico" NavigateUrl="~/produccion/laboratorio.aspx"></asp:HyperLink></td>
    </tr>
    <tr>
        <td colspan="4" class="tdl tdr tdb" align="center">
            <asp:GridView ID="GMicro" runat="server" AutoGenerateColumns="False" Width="100%" 
                DataKeyNames="IdGrid" EmptyDataText="No se encontraron análisis" OnRowDataBound="GMicro_RowDataBound" >
                <RowStyle CssClass="GridRow"></RowStyle>
                <FooterStyle CssClass="GridFooter"></FooterStyle>
                <PagerStyle CssClass="GridPager" ForeColor="White"></PagerStyle>
                <SelectedRowStyle CssClass="GridSelRow"></SelectedRowStyle>
                <HeaderStyle CssClass="GridHeader" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle CssClass="GridAltRow"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" ItemStyle-HorizontalAlign="Right"  DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="observaciones" HeaderText="Observaciones" SortExpression="observaciones" ItemStyle-HorizontalAlign="left" />
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5" class="GridHeader">Prueba de esterilidad comercial</td>
    </tr>
    <tr>
        <td colspan="5" class="tdl tdr tdb">
            <asp:CheckBoxList RepeatColumns="4" runat="server" ID="CEsterilidad" DataSourceID="SEsterilidad" DataTextField="nombre" DataValueField="id" CssClass="esterilidad">
            </asp:CheckBoxList>
            <asp:SqlDataSource ID="SEsterilidad" runat="server" 
                ConnectionString="<%$ ConnectionStrings:MySQLConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:MySQLConnectionString.ProviderName %>" 
                SelectCommand="SELECT varios.id, varios.nombre
                        FROM varios
                        WHERE varios.tipo=1006 AND varios.estado IS NULL
                        ORDER BY varios.nombre;">
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
        <td><asp:Label runat="server" ID="LMotivoAnulacion" Visible="false" Text="Motivo anulación"></asp:Label></td>
        <td colspan="3"><asp:Label runat="server" ID="LAnulacion"></asp:Label></td>
    </tr>
    <tr>
        <td colspan="4">&nbsp;<asp:Label runat="server" ID="LMensaje" ForeColor="Red"></asp:Label><asp:ValidationSummary runat="server" /></td>
    </tr>
    <tr>
        <td><asp:Button runat="server" ID="BAnular" Text="Anular" Visible="false" CausesValidation="false" /></td>
        <td colspan="3" align="right">
            <asp:Button runat="server" ID="BAceptar" Text="Aceptar" onclick="BAceptar_Click" />
            <asp:Button runat="server" ID="BCancelar" Text="Cancelar" CausesValidation="false" onclick="BCancelar_Click" />
        </td>
    </tr>
</table>
<asp:Panel runat="server" ID="PAnular" Visible="false">
<div title="Motivo anulación" id="dAnular" class="hide">
    <table>
        <tr>
            <td><asp:TextBox runat="server" ID="TMotivoAnulacion" TextMode="MultiLine" Width="300px" Height="50px"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:RequiredFieldValidator ValidationGroup="anulacion" ControlToValidate="TMotivoAnulacion" ErrorMessage="Por favor ingrese un motivo." ID="RequiredFieldValidator9" runat="server" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td align="right">
                <asp:Button runat="server" ValidationGroup="anulacion" ID="BAnularReal" Text="Anular" onclick="BAnular_Click"/>
                <asp:Button runat="server" ID="BCancelarAnular" CausesValidation="false" Text="Cancelar" />
            </td>
        </tr>
    </table>
</div>
</asp:Panel>
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>