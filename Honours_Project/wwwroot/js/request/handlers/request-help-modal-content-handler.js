const requestBuilderHelpContent = $('#request-builder-help-content');

const requestHelpModalDefinitions = requestBuilderHelpContent;

function Display_Request_Help_Modal_Section(section) {
    requestHelpModalDefinitions.fadeOut(300).promise().done(function () {
        console.log('Fading in view: ', section);
        section.fadeIn(300);
    }).promise().done(function () {
        $('#request-help-modal').modal('show');
    });
}