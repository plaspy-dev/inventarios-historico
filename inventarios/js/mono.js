// arreglos para el mono
var Page_IsValid = true;
var webForm = window;

ValidatorEnable = function(elemento, enable) { try { elemento._enabled = enable; } catch (e) { } };
webForm.WebForm_DoPostBackWithOptions = function(options) {
    Page_IsValid = true;
    if (options.validation && typeof (webForm.Page_ClientValidate) == "function")
        Page_IsValid = webForm.Page_ClientValidate(options.validationGroup);

    if (Page_IsValid) {
        if ((typeof (options.actionUrl) != "undefined") && (options.actionUrl != null) && (options.actionUrl.length > 0))
            webForm._form.action = options.actionUrl;
        if (options.trackFocus) {
            var lastFocus = webForm._form.elements["__LASTFOCUS"];
            if ((typeof (lastFocus) != "undefined") && (lastFocus != null))
                lastFocus.value = options.eventTarget;
        }
    }
    if (options.clientSubmit)
        webForm.__doPostBack(options.eventTarget, options.eventArgument);
}

Sys.Browser.WebKit = {};
if (navigator.userAgent.indexOf('WebKit/') > -1) {
    Sys.Browser.agent = Sys.Browser.WebKit;
    Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
    Sys.Browser.name = 'WebKit';
}
