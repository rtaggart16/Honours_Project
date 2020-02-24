const requestBuilderHelpContent = $('#request-builder-help-content');
const requestHelpSelector = $('#request-help-selector-content');
const allRequestResultContent = $('#all-request-results-help-content');

const requestHelpModalDefinitions = $('#request-builder-help-content').add($('#request-help-selector-content')).add($('#all-request-results-help-content'));

function Display_Request_Help_Modal_Section(section) {
    $('.request-help-modal-content').fadeOut(300).promise().done(function () {
        console.log('Fading in view: ', section);
        section.fadeIn(300);
    }).promise().done(function () {
        $('#request-help-modal').modal('show');
    });
}