/* Write here your custom javascript codes */

function activaTab(tab) {
    $('.nav-tabs a[href="#' + tab + '"]').tab("show");
};

function makeFormValidation(formId) {
    var form = $("#"+formId),
        formData = $.data(form[0]),
        settings = formData.validator.settings,
        oldErrorPlacement = settings.errorPlacement,
        oldSuccess = settings.success;
    settings.errorPlacement = function(label, element) {
        oldErrorPlacement(label, element);
        label.parents("label").addClass("state-error");
        label.addClass("text-danger");
    };
    settings.success = function(label) {
        label.parents("label").removeClass("state-error");
        oldSuccess(label);
    };
};
function getListFromList(subcategorieId, idTarget, urlData) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#" + idTarget).html(procemessage).show();
    var url = urlData;
    $.ajax({
        url: url,
        data: { subCategorie: subcategorieId },
        cache: false,
        type: "POST",
        success: function (data) {
            var markup = "<option value='' disabled selected hidden>Seleccionar</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
            }
            $("#" + idTarget).html(markup).show();
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}