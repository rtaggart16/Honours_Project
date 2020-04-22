/*
    Name: Ross Taggart
    ID: S1828840
*/

//! Section: Contents

    /*
     * - Variables
     *      - requestBuilderHelpContent
     *      - requestHelpSelector
     *      - allRequestResultContent
     *      - requestHelpModalDefinitions
     * - Functions
     *      - Display_Request_Help_Modal_Section
    */

//! END Section: Contents

//! Section: Variables

// requestBuilderHelpContent - The request builder help section
const requestBuilderHelpContent = $('#request-builder-help-content');

// requestHelpSelector - The request help section
const requestHelpSelector = $('#request-help-selector-content');

// allRequestResultContent - All requests help section
const allRequestResultContent = $('#all-request-results-help-content');

// requestHelpModalDefinitions - Array of modal sections
const requestHelpModalDefinitions = $('#request-builder-help-content').add($('#request-help-selector-content')).add($('#all-request-results-help-content'));

//! END Section: Variables

//! Section: Functions

/**
 * Name: Display_Request_Help_Modal_Section
 * Description: Method to display a specific modal section
 * @param {any} section The target modal section
 */
function Display_Request_Help_Modal_Section(section) {
    // Fade all displayed sections out
    $('.request-help-modal-content').fadeOut(300).promise().done(function () {
        // Fade in the target section
        section.fadeIn(300);
    }).promise().done(function () {
        // Display the modal if it is not displayed
        $('#request-help-modal').modal('show');
    });
}

//! END Section: Functions