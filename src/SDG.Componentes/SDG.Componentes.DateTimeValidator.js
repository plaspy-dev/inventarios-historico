function getDate(fecha) {
    var reggie = new RegExp('(\\d{2})/(\\d{2})/(\\d{4})', 'g');
    var dateArray = reggie.exec(fecha);
    return new Date(
        (+dateArray[3]),
        (+dateArray[2]) - 1, /*Careful, month starts at 0!*/
        (+dateArray[1]),
        0,
        0,
        0
    );
}
function getDateTime(fecha) {
    var reggie = new RegExp('(\\d{2})/(\\d{2})/(\\d{4}) (\\d{2}):(\\d{2})', 'g');
    var dateArray = reggie.exec(fecha);
    if(dateArray==null) 
        return getDate(fecha);
    return new Date(
        (+dateArray[3]),
        (+dateArray[2]) - 1, /*Careful, month starts at 0!*/
        (+dateArray[1]),
        (+dateArray[4]),
        (+dateArray[5]),
        0
    );
}
function onValidate_DateTime(sender, args) {
    args.IsValid = false;
    var fecha = getDateTime(args.Value);
    if (sender.maximunValue) {
        var maximo = getDateTime(sender.maximunValue);
        if (fecha > maximo) {
            args.IsValid = false;
            sender.errormessage = 'Por favor ingrese una fecha menor a ' + sender.maximunValue;
            return;
        }
    }
    if (sender.minimunValue) {
        var minimo = null;
        if (fecha.getHours() == 0 && fecha.getMinutes() == 0)
            minimo = getDate(sender.minimunValue); // solo validamos la fecha.
        else minimo = getDateTime(sender.minimunValue);
        if (fecha < minimo) {
            args.IsValid = false;
            sender.errormessage = 'Por favor ingrese una fecha mayor a ' + sender.minimunValue;
            return;
        }
    }
    args.IsValid = true;
}