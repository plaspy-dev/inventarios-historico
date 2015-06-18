function initTercero(tercero, terceroid, tipo) {
    addFunctionToStart(function() {
        $("#" + tercero).autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: "../webservices/servicios.asmx/GetTercero",
                    data: "{ 'q': '" + request.term + "', tipo: " + tipo + " }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function(data) { return data; },
                    success: function(data) {
                        response($.map(data.d, function(item) {
                            item.label = item.NombreCompleto;
                            return item;
                        }))
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            minLength: 3,
            autoFocus: true,
            search: function() {
                $("#" + terceroid).val("");
            },
            select: function(event, ui) {
                $("#" + terceroid).val(ui.item.Id);
                $("#" + tercero).val(ui.item.NombreCompleto);
                return false;
            }
        }).change(function() {
            if ($(this).val().length == 0)
                $("#" + terceroid).val("");
        });
    });
}