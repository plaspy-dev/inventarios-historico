var hiddenButton = null;
var funcionesInicio = new Array();
var funcionesRegistradas = new Array();
var g_OpenDialog = null;

function addDialog(szPanel, szDialog, showclosebutton) {

    if ($(szDialog + " :submit").length || $(szDialog + " :button").length) {

        $(szDialog).show().removeClass("hide");
        g_OpenDialog = szDialog; // guardamos el dialogo abierto para validacion.
        $(szDialog).dialog({
            modal: true,
            autoOpen: true,
            width: 'auto',
            closeOnEscape: showclosebutton,
            open: function(event, ui) { if (!showclosebutton) $(".ui-dialog-titlebar-close").hide(); setTimeout("$('" + szDialog + " :input:first').focus();", 100); }
        });
        if (szPanel != null)
            $(szDialog).parent().appendTo(jQuery(szPanel));
        $(szDialog + " :submit").button();
        $(szDialog + " :button").button();
        return true;
    }
    return false;
}

function FullRowSelect(grilla) {

    var tr = $(grilla + " tr").each(function(index) {
        var sel = $(this).children("td").children('input[src*="edit.gif"]');
        if (sel.length > 0) {
            $(this).css({ cursor: "hand", cursor: "pointer" }).hover(function() {
                $(this).addClass("GridSelRow");
            }, function() {
                $(this).removeClass("GridSelRow");
            });
            $(this).children("td").each(function(ind) {
                if ($(this).children("input").length == 0) {
                    if (sel.attr("id") == undefined) {
                        $(this).click(function() {
                            eval(" " + sel.attr("onclick"));
                        });
                    } else {
                        $(this).click(function() {
                            $("#" + sel.attr("id")).click();
                        });
                    }
                }
            });
        }
    });
}

// Cuando envia el boton para deshabilitarlo
function obs() {
    var retorno = true;
    if (hiddenButton == this)
        return false;
    if (typeof this.ejecutar !== 'undefined') {
        if (typeof this.ejecutar === 'function')
            retorno = this.ejecutar();
        else retorno = eval(this.ejecutar);
    }
    if ((typeof retorno === 'undefined' || retorno) && (typeof Page_IsValid === 'undefined' || Page_IsValid || typeof this.ejecutar === 'undefined')) {
        hiddenButton = this;
        $(this).hide();
        $(this).after('<input type="button" id="dummybutton" disabled="disabled" value="' + $(this).val() + '" class="' + $(this).attr('class') + '" />');
    }
    return retorno;
}

function inicio() {

    // todos los campos hora
    /*$(".hora").mask("99:99");
    $(".hora").timeEntry({ show24Hours: true });
    $(".hora").timepicker({ hourText: 'Horas',
        minuteText: 'Minutos',
        amPmText: ['AM', 'PM']
    });
    // todos los campos fecha
    $(".fecha").mask("99/99/9999");*/
    // $(".fecha").datepicker({ onSelect: function() { } });
    $(".fecha").datepicker($.datepicker.regional["es"]);

    // todos los campos entero
    $('.entero').autoNumeric('init', { vMin: 0, mDec: 0, aSep: '' });

    // todos los reales
    $('.float').autoNumeric('init', { vMin: 0, aSep: '', mDec:8,aPad:false });

    // dinero
    $('.dinero').autoNumeric('init', { vMin: 0, aSign: '$', aPad: false });

    $('img[src*="menubg.jpg"]').hover(function() {
        $(this).attr("src", $(this).attr("src").replace("menubg", "menuover"));
    }, function() {
        $(this).attr("src", $(this).attr("src").replace("menuover", "menubg"));
    });

    $(".linkMenu").hover(function() {
        if (typeof this.rep == "undefined") {
            var img = $(this).prev().find("img");
            if (img.length) {
                this.rep = img;
            } else this.rep = false;
        }
        if (this.rep)
            this.rep.attr("src", this.rep.attr("src").replace("menubg", "menuover"));
    }, function() {
        if (this.rep)
            this.rep.attr("src", this.rep.attr("src").replace("menuover", "menubg"));
    });

    // todos los eliminar
    $('input[src*="eliminar.gif"]').each(function(i) {
        if (this.eliminar == undefined) {
            this.eliminar = $(this).attr("onclick");
            $(this).unbind('click').attr("onclick", "").click(function() { if (confirm('¿Esta seguro de eliminar este elemento?')) { if (this.eliminar != undefined) { if (this.eliminar.toString().indexOf("function") == -1) eval(this.eliminar); else this.eliminar(); } return true; } return false; });
        }
    });

    $(":submit,:button").each(function(i) {
        if (this.ejecutar == undefined) {
            this.ejecutar = $(this).attr("onclick");
            //            if ($(this).data("events")) {
            //                $.each($(this).data("events"), function(evento, event) {
            //                    alert(evento);
            //                    $.each(event, function(j, h) {
            //                        alert(h.handler);
            //                    });
            //                });
            //            }
            $(this)
            // .unbind('click')
            .attr("onclick", "").click(obs);
        }
    });

    for (var funcionInicio in funcionesInicio) {
        funcionesInicio[funcionInicio]();
    }
    
    // funcion propia de la pagina
    /*if (typeof iniciopagina == "function") {
    iniciopagina();
    }*/

    // capturamos el enter
    $('input,select').keydown(function(event) { //event==Keyevent
        if (event.which == 13) {
            var inputs = $(this).closest('form').find(':input:visible');
            if ($(this).attr('type') != 'button' &&
                $(this).attr('type') != 'submit') {
                inputs.eq(inputs.index(this) + 1).focus();
                $(this).change();
                event.preventDefault();
            }
        }
    });
}

function addFunctionToStart(funcion) {
    funcionesInicio.push(funcion);
}

$.extend({
    getUrlVars: function() {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    getUrlVar: function(name) {
        return $.getUrlVars()[name];
    },
    goURL: function(params) {
        pos = window.location.href.indexOf('?');
        url = pos == -1 ? location.href : location.href.slice(0, window.location.href.indexOf('?'));
        if (params == null)
            location.href = url;
        else location.href = url + "?" + params;
    }
});

function commaFormatted(amount) {

    var delimiter = ","; // replace comma if desired
    var a = amount.split('.', 2)
    var d = a[1];
    var i = parseInt(a[0]);
    if (isNaN(i)) { return ''; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    var n = new String(i);
    var a = [];
    while (n.length > 3) {
        var nn = n.substr(n.length - 3);
        a.unshift(nn);
        n = n.substr(0, n.length - 3);
    }
    if (n.length > 0) { a.unshift(n); }
    n = a.join(delimiter);
    if (d.length < 1) { amount = n; }
    else { amount = n + '.' + d; }
    amount = minus + amount;
    return amount;
}

function floatFormatted(amount) {
    var i = parseFloat(amount);
    if (isNaN(i)) { i = 0.00; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    i = parseInt((i + .005) * 100);
    i = i / 100;
    s = new String(i);
    if (s.indexOf('.') < 0) { s += '.00'; }
    if (s.indexOf('.') == (s.length - 2)) { s += '0'; }
    s = minus + s;
    return commaFormatted(s);
}

function currencyFormatted(amount) {
    return '$' + floatFormatted(s);
}

var hasStorage = (function() {
    try {
        var mod = "test_hasStorage";
        localStorage.setItem(mod, mod);
        localStorage.removeItem(mod);
        return true;
    } catch (e) {
        return false;
    }
} ());