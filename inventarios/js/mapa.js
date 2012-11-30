// funciones
// obfuscate exclude identifier a,ml,p,d,centerLast,centerPos,checkPath,aplicarFiltro,setPolyLine,iniciarZona,z,r,getInfo,saveZone,u,editZone,j,centerByName,x,sMul,l
// variables
// obfuscate exclude identifier gusuarios,dusuarios,tabladetallado,uactualizacion,map,ma,bDr,tot,ml,lactualizacion,bFiltrando

$("#loading").ajaxStart(function() { $(this).show(); });
$("#loading").ajaxStop(function() { $(this).hide(); });

var map = null;
var gmap = null;
var ma = new Array();
var alimites = new Array();
var ml = null;
var init = false;
var rutas = [];
var uactualizacion = new Date(2000, 01, 01);
var gusuarios = "";
var dusuarios = "";
var zo = new Array();
var poly = null;
var markers = null;
var path = null;
var dragged = false;
var polys = new Array();
var tabladetallado = null;
var bDr = false;
var fDe = undefined;
var fHa = undefined;
var fDeM = null;
var fHaM = null;
var bMu = false;
var bGeoCo = false;
var tot = 10;
var dto = 100;
var lactualizacion = null;
var bFiltrando = null;
var colorMultiple = 14876642; // verde
// blanco -> 16777215;

function l(usuario, velocidad, inactividad, enviarcorreo) {
    alimites.push(new Limite(usuario, velocidad, inactividad, enviarcorreo));
}

function getLimiteUsuario(usuario) {

    for (var i in alimites) {
        if (alimites[i].usuario==usuario) {
            return alimites[i];
        }
    }
    return null;
}

// retorna la fecha con formato
function getFormatDate(fecha) {
    return checkZero(fecha.getDate()) + "/" + checkZero(fecha.getMonth() + 1) + "/" + checkZero(fecha.getFullYear());
}
// retorna la hora con formato
function getFormatTime(fecha) {
    return checkZero(fecha.getHours()) + ":" + checkZero(fecha.getMinutes()) + ":" + checkZero(fecha.getSeconds());
}
// verfica que los numeros contengan 0 en caso de ser menores a 10
function checkZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}

// stringToDate
function stringToDate(fecha) {
    var reggie = /(\d{2})\/(\d{2})\/(\d{4}) (\d{2}):(\d{2}):(\d{2})/g;
    var dateArray = reggie.exec(fecha);
    if (fecha.indexOf("p", 0) != -1) {
        if (dateArray[4] != "12") // se omiten las 12 p.m.
            dateArray[4] = 12 + (+dateArray[4]);
    } else if (dateArray[4] == "12") // se cambia las 12 a.m.
        dateArray[4] = "0";
    return new Date(
        (+dateArray[3]),
        (+dateArray[2]) - 1, // Careful, month starts at 0!
        (+dateArray[1]),
        (+dateArray[4]),
        (+dateArray[5]),
        (+dateArray[6])
    );
}

function sMul(multiple) {

    if (typeof multiple == 'undefined')
        return bMu;
    bMu = multiple;
    colorMultiple = 14876642;
    return bMu;
}

function padding_left(s, c, n) {
    if (!s || !c || s.length >= n) {
        return s;
    }

    var max = (n - s.length) / c.length;
    for (var i = 0; i < max; i++) {
        s = c + s;
    }

    return s;
}

function decimalToHexString(number) {

    return padding_left(number.toString(16).toUpperCase(), '0', 6);
}

// esta funcion es la que realmete agrega
function realAddPoint(lat, lon, num, col, info, fecha, bat, nom, vel, posicionnombre, zona, alerta, inicio) {

    setTimeout(function() {

        for (var i in ma) {
            if (ma[i].igualSinConfirmar(lat, lon, num)) {
                ma[i].confirmado = true;
                checkZone(ma[i]);
                tot -= dto;
                return false;
            }
        }
        var dfecha = stringToDate(fecha);

        if (uactualizacion < dfecha) {
            uactualizacion = dfecha;
            lactualizacion.html(nom + " (" + getFormatDate(dfecha) + " " + getFormatTime(dfecha) + ")").unbind('click').unbind('mouseover').click(function() {
                tabladetallado.fnFilter(nom);
                $("#odetallado").val("Ocultar detalles");
                centerByName(nom);
            }).mouseover(function() {
                $(this).css({ cursor: "hand", cursor: "pointer" });
            });
        }

        var dfecha = stringToDate(fecha);
        var nuevocolor = col;
        if (bMu) {

            if (colorMultiple > 65535) //restar en todos
            {
                colorMultiple -= 2;
                colorMultiple -= (2 << 16);
            } else { // restar solo en verde
                colorMultiple -= (2 << 8);
            }
            if (colorMultiple < 0)
                colorMultiple = 0;
            nuevocolor = decimalToHexString(colorMultiple);
        }
        var punto = new Punto(lat, lon, num, nuevocolor, info, dfecha, bat, nom, vel, posicionnombre, zona, alerta);
        ma.push(punto);
        checkZone(punto);
        makePath(num);
        var direccion = "";
        if (posicionnombre.substr(0, 2) != "_$") // es un codigo
            direccion = posicionnombre;

        var visible = punto.verificaFiltro();
        tabladetallado.fnAddData([
				        nom,
				        getFormatTime(dfecha),
				        vel,
				        bat,
				        lat,
				        lon,
				        visible,
				        direccion,
				        zona,
				        alerta
				        ]);
        if (visible && inicio)
            punto.animate();
        tot -= dto;
    }, tot);
    tot += dto;
}

// agrega un nuevo punto addPunto
function a(lat, lon, num, col, info, fecha, bat, nom, vel, posicionnombre, zona, alerta) {
    realAddPoint(lat, lon, num, col, info, fecha, bat, nom, vel, posicionnombre, zona, alerta, init);
}

// Brinca el ultimo punto de la ruta. para las rutas largas
function centerLast() {
    if (tot > 10) {
        setTimeout(centerLast, tot);
        return;
    }
    if (ma[ma.length - 1]) { // es el ultimo o el primero que se quedo de la consulta anterior
        var ultimo = ma[ma.length - 1].fecha > ma[0].fecha ? ma[ma.length - 1] : ma[0];
        ultimo.animate(false, 5000);
    }
}

// centrar el punto basados en el nombre de la persona
function centerByName(nombre) {
    for (var i = ma.length - 1; i >= 0; i--) { // buscamos al reves
        if (ma[i] && ma[i].nombre == nombre && ma[i].marker.getVisible()) {
            map.setCenter(ma[i].latLng);
            ma[i].animate();
            break;
        }
    }
}

// checkpath
function checkPath() {

    for (var i in ma) {
        if (ma[i]) {
            makePath(ma[i].numeroPunto);
            break;
        }
    }
}

// retorna la informacion del punto con la bateria y la hora.
function getInfo(nombre) {
    for (var i = ma.length - 1; i >= 0; i--) { // buscamos al reves
        if (ma[i] && ma[i].nombre == nombre) {
            return ma[i].zona + " - " + ma[i].battery + "% (" + getFormatTime(ma[i].fecha) + ")";
        }
    }
    return "";
}

// esta funcion devuelve si la fecha es mayor a la descrita
function mayorQue(hora, minuto, fecha) {
    return fecha.getHours() > hora || (fecha.getHours() == hora && fecha.getMinutes() >= minuto);
}

// esta funcion dispara el timeout de la animacion
function stopPoint(punto, timeout) {
    setTimeout(function() { punto.stopAnimation(); }, timeout);
}

// Clase Punto
function Punto(lat, lon, num, col, info, fecha, bat, nombre, vel, posicionnombre, zona, alerta) {
    // esta es la animacion
    this.animate = function(indefinido, timeout) {
        if (this.alarma)
            return;
        if (this.marker) {
            this.marker.setAnimation(google.maps.Animation.BOUNCE);
        }
        this.alarma = indefinido;
        if (typeof timeout == 'undefined')
            timeout = 2000;
        if (!indefinido)
            stopPoint(this, timeout);
    };
    this.alerta = alerta;
    this.battery = bat;
    this.color = col;
    this.alarma = false;
    this.confirmado = true;
    this.fecha = fecha;
    this.verificaFiltro = function() {
        // si hay un filtro aplicado
        if (typeof fDe !== "undefined" && typeof fHa !== "undefined") {
            // tiene que ser mayor al inicio y menor al final
            var resultado = mayorQue(fDe, fDeM, this.fecha) && !mayorQue(fHa, fHaM, this.fecha);
            this.marker.setVisible(resultado);
            return resultado;
        }
        // no hay filtro aplicado
        this.marker.setVisible(true);
        return true;
    }
    // igual
    this.igualSinConfirmar = function(lat, lon, num) { return this.confirmado == false && this.latitude == lat && this.longitud == lon && this.numeroPunto == num; };
    // igual a
    this.igual = function(lat, lon, nombre) { return this.latitude == lat && this.longitud == lon && this.nombre == nombre; };
    this.codigo = ""; // codigo que va a enviar con la direccion
    this.direccion = ""; // nombre de la calle o direccion

    if (posicionnombre.substr(0, 2) == "_$") {// es un codigo
        this.codigo = posicionnombre.substr(2);
    } else this.direccion = posicionnombre;

    this.latitude = lat;
    this.latLng = new google.maps.LatLng(lat, lon);
    this.numeroPunto = num;
    this.longitud = lon;
    this.marker = new google.maps.Marker({
        position: this.latLng,
        map: map,
        icon: 'http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=' + num + '|' + col + '|000000',
        title: info
    });
    // guardamos el punto en el mismo punto
    this.marker.p = this;
    google.maps.event.addListener(this.marker, 'click', function() {
        this.p.createInfoWindow().open(map, this);
    });
    this.stopAnimation = function() { this.alarma = false; if (this.marker) this.marker.setAnimation(null); };
    this.stopAlarma= function() { if (!this.alarma) return; this.alarma = false; if (this.marker) this.marker.setAnimation(null); };
    this.nombre = nombre;
    this.velocidad = vel;
    this.createInfoWindow = function() {
        if (!this.marker.i) {
            this.marker.i = new google.maps.InfoWindow();
        }
        var img = 5;
        if (this.battery > 79) {
            img = 1;
        } else if (this.battery > 50) {
            img = 2;
        } else if (this.battery > 30) {
            img = 3;
        } else if (this.battery > 10) {
            img = 4;
        }
        this.marker.i.setContent(
            '<table width="100%" border="0"><tr><td colspan="2" align="center"><b>'
            + this.nombre + '</b></td></tr><tr><td colspan="2" align="center" nowrap=nowrap><b>'
            + this.direccion + '</b></td></tr><tr><td>'
            + getFormatTime(this.fecha) + '</td><td align="right"><img src="img/b'
            + img + '.jpg" /></td></tr><tr><td>'
            + this.velocidad + ' Km/h</td><td align="right">'
            + this.battery + '%</td></tr></table>'
        );
        return this.marker.i;
    }
    // enviamos la descripcion a nuestro servidor y ademas actualizamos la direccion y la zona
    this.onUpdatePosition = function(update_server) {
        // actualizamos la tabla
        var oTable = tabladetallado.fnGetData();
        for (var j = 0; j < oTable.length; j++) {
            if (this.igual(oTable[j][4], oTable[j][5], oTable[j][0])) {
                tabladetallado.fnUpdate(this.direccion, j, 7, false);
                tabladetallado.fnUpdate(this.zona, j, 8, false);
                break;
            }
        }
        // actualizamos la tabla
        tabladetallado.fnDraw();
        // no me importa la respuesta
        if (update_server)
            $.get("d.ashx?l=" + this.latitude + "&o=" + this.longitud + "&k=" + this.codigo + "&d=" + encodeURIComponent(this.direccion) + "&z=" + encodeURIComponent(this.zona));
    }
    // esta es la descripcion de la direccion
    this.zona = zona;
}

// esta funcion trunca a 4 digitos significativos
function truncateFour(valor) {
    valor = parseInt(valor * 10000);
    valor = valor / 10000;
    return valor;
}
// esta funcion copia el valor de la coordenada obtenida por el geocoder
function copyAddress(punto_fuente, punto_destino) {

    if (punto_destino.direccion == "" && punto_destino.codigo.length > 0) { // verifiamos que tenga las coordenas limpias
        if (truncateFour(punto_fuente.latitude) == truncateFour(punto_destino.latitude) && truncateFour(punto_fuente.longitud) == truncateFour(punto_destino.longitud)) { // son las mismas coordenadas
            // copiamos la zona y la direccion
            punto_destino.direccion = punto_fuente.direccion;
            punto_destino.zona = punto_fuente.zona;
            punto_destino.onUpdatePosition(false);
        }
    }
}

// esta funcion inicializa el geocoder
function getAddressPoint(punto) {

    if (bGeoCo)
        return;
    bGeoCo = true;
    gmap.geocode({ 'latLng': punto.latLng }, function(results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[0]) {
                punto.direccion = results[0].formatted_address.split(",")[0]; // direccion, sin la descripcion
            }
            if (results[1]) {
                punto.zona = results[1].formatted_address; // dsecripcion de la direccion
            }
            punto.onUpdatePosition(true);
            // actualizamos los puntos que tengan las mismas coordenadas
            for (var i in ma) {
                copyAddress(punto, ma[i]);
            }
        } else {
            bGeoCo = false;
            return false; // cancelamos todas las busquedas, para reanudarlas mas adelante.
            // alert("Geocoder failed due to: " + status);
        }
        bGeoCo = false;
        for (var i in ma) { // buscamos los puntos que falta por ubicar
            if (ma[i].direccion == "" && ma[i].codigo.length > 0) {
                getAddressPoint(ma[i]);
                return true;
            }
        }
    });
}

// centerpos
function centerPos(pos) {
    var oTable = tabladetallado.fnGetData();
    for (var i in ma) {
        if (ma[i].igual(oTable[pos][4], oTable[pos][5], oTable[pos][0])) {
            map.setCenter(ma[i].latLng);
            ma[i].animate();
            break;
        }
    }
}
// marca todos los puntos como borrables -- prepareClean
function p() {

    for (var i in ma) {
        ma[i].confirmado = false;
    }
}

function aplicarFiltroReal(oTable, punto, ultimo) {

    setTimeout(function() {

        var pos = -1;
        for (var j = 0; j < oTable.length; j++) {
            if (punto.igual(oTable[j][4], oTable[j][5], oTable[j][0])) {
                pos = j;
                break;
            }
        }
        if (pos == -1) // no se encontro la posicion
            punto.verificaFiltro();
        else tabladetallado.fnUpdate(punto.verificaFiltro(), pos, 6, false);

        tot -= dto / 5;
        if (ultimo) {
            tabladetallado.fnDraw();
            bFiltrando.button("option", "label", "Filtrado");
        } else {
            bFiltrando.button("option", "label", "Filtrando...");
        }
    }, tot);
    tot += dto/5;
}

// en esta funcion aplicamos los filtros
function aplicarFiltro(desde, hasta) {

    if (desde) {
        var reggie = /(\d{2}):(\d{2})/g;
        var horaArray = reggie.exec(desde);
        fDe = (+horaArray[1]);
        fDeM = (+horaArray[2]);
    } else fDe = undefined;
    if (hasta) {
        var reggie2 = /(\d{2}):(\d{2})/g;
        var horaArray2 = reggie2.exec(hasta);
        fHa = (+horaArray2[1]);
        fHaM = (+horaArray2[2]);
    } else fHa = undefined;

    var oTable = tabladetallado.fnGetData();
    
    if (typeof fDe === "undefined" && typeof fHa === "undefined") {
        bFiltrando.button("option", "label", "Quitando...");
        for (var i in ma) {
            ma[i].verificaFiltro();
        }
        for (var j = 0; j < oTable.length; j++) {
            tabladetallado.fnUpdate(true, j, 6, false);
        }
        tabladetallado.fnDraw();
        bFiltrando.button("option", "label", "Filtro");
    } else {
        bFiltrando.button("option", "label", "Filtrando...");
        for (var i in ma) {
            aplicarFiltroReal(oTable, ma[i], i==ma.length-1);
        }
    }
}

// borra el resto de puntos
function d() {

    if (tot > 10) { // estamos con un contador activado
        setTimeout(d, tot);
        return;
    }
        
    for (var i = 0; i < ma.length; i++) {
        if (ma[i].confirmado == false) {
            ma[i].marker.setMap(null);
            var numero = ma[i].numeroPunto;
            var oTable = tabladetallado.fnGetData();
            for (var j = 0; j < oTable.length; j++) {
                if (ma[i].igual(oTable[j][4], oTable[j][5], oTable[j][0])) {
                    tabladetallado.fnDeleteRow(j, null, false);
                    break;
                }
            }
            delete ma[i];
            ma.splice(i, 1);
            i--;
            makePath(numero);
        }
    }
    // buscamos la posicion en el geocoder
    if (!bGeoCo) { // sino intentamos seguir con la cola
        for (var i in ma) { // buscamos los puntos que falta por ubicar
            if (ma[i].direccion == "" && ma[i].codigo.length > 0) {
                getAddressPoint(ma[i]);
                break;
            }
        }
    }
    tabladetallado.fnDraw();
}
// makepath
function makePath(numero) {

    if (bDr == false) {
        if (rutas[numero]) {
            rutas[numero].setMap(null);
        }
        return;
    }
    var flightPlanCoordinates = [];
    var flightPath = rutas[numero];
    var color;
    if (flightPath)
        flightPath.setMap(null);
    for (var i in ma) {
        if (ma[i].numeroPunto == numero) {
            flightPlanCoordinates.push(ma[i].latLng);
            color = ma[i].color;
        }
    }
    if (flightPlanCoordinates.length > 1) {
        flightPath = new google.maps.Polyline({
            path: flightPlanCoordinates,
            strokeColor: "#" + color,
            strokeOpacity: 1.0,
            strokeWeight: 4
        });
        flightPath.setMap(map);
        rutas[numero] = flightPath;
    }
}
// initialize
function j() {

    if (typeof i == "function")
        i();
    if (ml == null)
        ml = new google.maps.LatLng(4.643498, -74.092827);

    var tipo_mapa = $.cookie("map_type");
    if (tipo_mapa == null || tipo_mapa == "")
        tipo_mapa = google.maps.MapTypeId.ROADMAP;
    var myOptions = {
        zoom: 12,
        center: ml,
        mapTypeId: tipo_mapa
    }
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    google.maps.event.addListener(map, 'maptypeid_changed', function() {
        $.cookie("map_type", map.getMapTypeId());
    });
    // iniciamos el geocoder
    gmap = new google.maps.Geocoder();
    if (typeof publicidad == "function")
        publicidad();
        
    $.getScript('js/maps.google.polygon.containsLatLng.js', function() {
        if (typeof puntos == "function") {
            puntos();
            init = true;
            d();
        }
    });
}
// updateTableZona
function updateTableZone() {
    $("#tbzona tr").unbind('click').unbind('mouseover').each(function(i) {
        $(this).click(function() { setPolyLine(polys[i]); if (polys[i].path.getLength()) map.setCenter(polys[i].path.getAt(0)); });
    }).mouseover(function() {
        $(this).css({ cursor: "hand", cursor: "pointer" });
    });
    $("#tbzona tr td a").unbind('click').each(function(i) {
        $(this).click(function() { if (confirm('¿Esta seguro de eliminar esta zona?')) { deleteZone(polys[i], true); setPolyLine(); } return false; });
    });
}

// addFeatureEntry
function addTableRowZone(name, color) {

    currentRow_ = document.createElement("tr");
    var colorCell = document.createElement("td");
    currentRow_.appendChild(colorCell);
    colorCell.style.backgroundColor = color;
    colorCell.style.width = "1em";
    var nameCell = document.createElement("td");
    currentRow_.appendChild(nameCell);
    nameCell.style.whiteSpace = "nowrap";
    nameCell.innerHTML = name;

    var closeCell = document.createElement("td");
    closeCell.innerHTML = "<a>x</a>";
    closeCell.style.textAlign = "right";
    closeCell.style.verticalAlign = "middle";
    closeCell.style.color = "#666666";
    closeCell.style.width = "60px";
    currentRow_.appendChild(closeCell);

    $("#tbzona").get(0).appendChild(currentRow_);
    return currentRow_;
}

// actualizar
function saveZone() {
    var resultado = "";
    setPolyLine();

    for (i in polys) {
        resultado += polys[i].inclusion ? "i" : "e";
        resultado += ";";
        for (var j = 0; j < polys[i].path.getLength(); j++) {
            resultado += polys[i].path.getAt(j).lat();
            resultado += ";";
            resultado += polys[i].path.getAt(j).lng();
            resultado += ";";
        }
    }
    // limpiamos todo
    while (polys.length > 0) {
        deleteZone(polys[0], false);
    }
    editZone(false);
    return resultado;
}

// cleanPoly
function deleteZone(p, update) {
    p.setMap(null);
    for (i in p.markers) {
        p.markers[i].setMap(null);
    }
    $("#tbzona").get(0).removeChild(p.row);
    for (i in polys) {
        if (polys[i] == p) {
            polys.splice(i, 1);
            break;
        }
    }
    if (update) {
        for (i in polys) {
            polys[i].row.cells[1].innerHTML = "Zona " + (+i + 1);
        }
        updateTableZone();
        if (poly == p) {
            poly = null;
            path = null;
            markers = null;
        }
    }
}
// setPoly
function setPolyLine(p) {

    google.maps.event.clearListeners(map, 'click');
    if (poly) {
        poly.row.cells[1].style.fontWeight = '';
        if (p != poly) {
            if (path.length < 3) {  //solo tiene una o dos lineas, la borramos
                deleteZone(poly, true);
            }
        }
    }
    poly = p;
    if (p) {
        path = poly.path;
        markers = poly.markers;
        p.row.cells[1].style.fontWeight = 'bold';

        $(poly.inclusion ? "#rInclusion" : "#rExclusion").attr('checked', true);
        google.maps.event.addListener(map, 'click', addPointToZoneEvent);
    } else {
        $("#rCursor").attr('checked', true);
    }
    $("#radio").buttonset();
}

// addPointL
function addPointToZone(latLng) {
    path.insertAt(path.length, latLng);
    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        draggable: true
    });
    markers.push(marker);
    marker.setTitle("#" + path.length);
    marker.poly = poly;

    google.maps.event.addListener(marker, 'click', function() {

        setPolyLine(marker.poly);

        marker.setMap(null);
        for (var i = 0, I = markers.length; i < I && markers[i] != marker; ++i);
        markers.splice(i, 1);
        path.removeAt(i);
    });

    google.maps.event.addListener(marker, 'dragend', function() {
        dragged = true;

        setPolyLine(marker.poly);

        for (var i = 0, I = markers.length; i < I && markers[i] != marker; ++i);
        path.setAt(i, marker.getPosition());
        setTimeout(function() { dragged = false; }, 200);
    });
}

// addPoint
function addPointToZoneEvent(event) {
    if (dragged)
        return;
    addPointToZone(event.latLng);
}

// startShape
function iniciarZona(inclusion, seleccionar) {

    var color = inclusion ? "#000080" : "#ff0000";
    var polytemp = new google.maps.Polygon({
        strokeWeight: 1,
        fillColor: color,
        strokeColor: color
    });
    polys.push(polytemp);
    polytemp.setMap(map);
    polytemp.path = new google.maps.MVCArray;
    polytemp.setPaths(new google.maps.MVCArray([polytemp.path]));
    polytemp.inclusion = inclusion;
    polytemp.markers = new Array();

    polytemp.row = addTableRowZone("Zona " + polys.length, color);
    updateTableZone();
    google.maps.event.addListener(polytemp, 'click', function() {
        setPolyLine(this);
    });

    if (seleccionar) {
        setPolyLine(polytemp);
    } else {
        poly = polytemp;
        path = poly.path;
        markers = poly.markers;
    }
    return polytemp;
}







// addZonaU
function u(usuario, zona) {

    for (var i in zo) {
        if (zo[i].numzona == zona) {
            zo[i].clientes.push(usuario);
            break;
        }
    }
}

// removeZonas
function r() {

// removemos los limites actuales de los usuarios
    alimites = new Array();
    
    while (zo.length) {
        zo[0].setMap(null);
        delete zo[0];
        zo.splice(0, 1);
    }
}

// addZona --> z
function z(tipo, lat, lon, numzona) {

    for (var i in zo) {
        if (zo[i].numzona == numzona) {
            zo[i].path.insertAt(zo[i].path.length, new google.maps.LatLng(lat, lon));
            return;
        }
    }
    var color = tipo == 1 ? "#000080" : "#ff0000";
    var polytemp = new google.maps.Polygon({
        strokeWeight: 1,
        fillColor: color,
        strokeColor: color
    });
    polytemp.tipo = tipo;
    polytemp.clientes = new Array();
    polytemp.numzona = numzona;
    polytemp.path = new google.maps.MVCArray;
    polytemp.setPaths(new google.maps.MVCArray([polytemp.path]));
    polytemp.path.insertAt(polytemp.path.length, new google.maps.LatLng(lat, lon));
    polytemp.setMap(map);
    zo.push(polytemp);
}

function Limite(usuario, velocidad, inactividad, enviarcorreo) {

    this.velocidad = velocidad;
    this.inactividad = inactividad;
    this.enviarcorreo = enviarcorreo;
    this.usuario = usuario;
    return this;
}

// editZonas
function editZone(editar) {

    var checkbox = editar ? $('input[name*="CVerUsuario"]') : null;
    if (checkbox && $("#" + dusuarios).is(':visible')) {  // solo esta seleccionado un usuario
        var seleccionado = $('option:selected', "#" + dusuarios).index();
        checkbox.each(function(index) {
            $(this).attr("checked", seleccionado == 0);
            seleccionado--;
        });
    }

    // editamos los limites
    if (editar) {
        var indice = 0;
        checkbox.each(function(index) {
            indice++;
            if ($(this).is(':checked')) {  // lo tenemos en cuenta

                var limiteusuario = getLimiteUsuario(indice);
                if (limiteusuario) {
                    if (limiteusuario.velocidad)
                        $('input[name*="TVelocidad"]').val(limiteusuario.velocidad);
                    if (limiteusuario.inactividad)
                        $('input[name*="TInactivo"]').val(limiteusuario.inactividad);
                    $('input[name*="CEnviarMail"]').attr("checked", limiteusuario.enviarcorreo);
                }
            }
        });
    }
    
    for (var i in zo) {
        zo[i].setVisible(!editar);
        if (editar) {
            // verificamos que la zona este chuliada
            var zonaagregada = false;        
            checkbox.each(function(index) {
                if (zonaagregada)
                    return;
                if ($(this).is(':checked')) {  // lo tenemos en cuenta
                    for (var c in zo[i].clientes) {
                        if (zo[i].clientes[c] == index + 1) {
                            iniciarZona(zo[i].tipo == 1, false);
                            for (var j = 0; j < zo[i].path.getLength(); j++) {
                                addPointToZone(zo[i].path.getAt(j));
                            }
                            zonaagregada = true;
                            break;
                        }
                    }
                }
            });
        }
    }
}

// checkZona
function checkZone(punto) {

    if (punto.alerta == null || punto.alerta == '') {
        punto.stopAlarma();
        return;
    }
    
    $("#" + gusuarios + " tr:eq(" + (punto.numeroPunto - 1) + ")").css("background", "#000080").css("color", "#FFF").effect("pulsate", {}, 1000);
    punto.animate(true);
    
        /*
    // verificamos que este en la zona permitida
    var fuera = false;    
    
    for (var i in zo) {
        if (zo[i].tipo == 1) {
            for (var c in zo[i].clientes) {
                if (zo[i].clientes[c] == punto.numeroPunto) {
                    if (zo[i].containsLatLng(punto.latLng)) {
                        punto.stopAlarma();
                        return;
                    }
                    fuera = true;
                }
            }
        }
    }
    if (!fuera) {
        // verificamos que no tenga restricciones
        var limiteusuario = getLimiteUsuario(punto.numeroPunto);
        if (limiteusuario) {
            // verificamos la velocidad
            if (limiteusuario.velocidad && limiteusuario.velocidad < punto.velocidad)
                fuera = true;
        }
    }
    
    // esta fuera de las zonas permitidas
    if (fuera) {
        $("#" + gusuarios + " tr:eq(" + (punto.numeroPunto - 1) + ")").css("background", "#000080").css("color", "#FFF").effect("pulsate", {}, 1000);
        punto.animate(true);
    }

    // verficamos que no este una zona prohibida
    for (var i in zo) {
        if (zo[i].tipo != 1) {
            for (var c in zo[i].clientes) {
                if (zo[i].clientes[c] == punto.numeroPunto) {
                    if (zo[i].containsLatLng(punto.latLng)) {
                        $("#" + gusuarios + " tr:eq(" + (punto.numeroPunto - 1) + ")").css("background", "#ff0000").css("color", "#FFF").effect("pulsate", {}, 1000);
                        punto.animate(true);
                        return;
                    }
                }
            }
        }
    }
    
    if (!fuera) // no esta infringiendo nada
        punto.stopAlarma();
        */
}

// LZW-compress a string
function lzw_encode(s) {
    var dict = {};
    var data = (s + "").split("");
    var out = [];
    var currChar;
    var phrase = data[0];
    var code = 256;
    for (var i = 1; i < data.length; i++) {
        currChar = data[i];
        if (dict[phrase + currChar] != null) {
            phrase += currChar;
        }
        else {
            out.push(phrase.length > 1 ? dict[phrase] : phrase.charCodeAt(0));
            dict[phrase + currChar] = code;
            code++;
            phrase = currChar;
        }
    }
    out.push(phrase.length > 1 ? dict[phrase] : phrase.charCodeAt(0));
    for (var i = 0; i < out.length; i++) {
        out[i] = String.fromCharCode(out[i]);
    }
    return out.join("");
}

// Decompress an LZW-encoded string lzw_decode
function x(s) {
    var dict = {};
    var data = (s + "").split("");
    var currChar = data[0];
    var oldPhrase = currChar;
    var out = [currChar];
    var code = 256;
    var phrase;
    for (var i = 1; i < data.length; i++) {
        var currCode = data[i].charCodeAt(0);
        if (currCode < 256) {
            phrase = data[i];
        }
        else {
            phrase = dict[currCode] ? dict[currCode] : (oldPhrase + currChar);
        }
        out.push(phrase);
        currChar = phrase.charAt(0);
        dict[code] = oldPhrase + currChar;
        code++;
        oldPhrase = phrase;
    }
    return out.join("");
}
