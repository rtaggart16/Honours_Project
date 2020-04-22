/*
    Name: Ross Taggart
    ID: S1828840
*/

//! Section: Contents

    /*
     * - Variables
     *      - Validation_Types
     * - Functions
     *      - Basic_Content_Validator
     *      - Generate_Validation_Error_HTML
    */

//! END Section: Contents

//! Section: Variables

/**
 * Validation_Types - Object to hold the validation options
*/
const Validation_Types = Object.freeze({
    Emptiness: { name: "empty", usedOn: "string" },
    Greater_Than_Zero: { name: ">0", usedOn: "number" }
});

//! END Section: Variables

//! Section: Functions

/**
 * Name: Basic_Content_Validator
 * Description: Method to validate nay options options passed in against the validation option types
 * @param {any} values The values to validate
 */
function Basic_Content_Validator(values) {
    // Object to hold the return value of the method
    let validResult = {
        errors: [],
        success: true,
        errorType: 'system'
    };

    // Iterate through each value passed in 
    $.each(values, function (key, val) {
        // Check the type of validation being performed
        switch (val.validationType) {
            // Checking if the value of the parameter is empty
            case Validation_Types.Emptiness:
                // Check if the type of the parameter is of string type
                if (typeof (val.value) == 'string') {
                    // Check if the value is empty or only a space
                    if (val.value == '' || val.value == ' ') {
                        // The value is not valid
                        validResult.errors.push(val.name + ' can\'t be empty or only a space');
                        validResult.success = false;
                        validResult.errorType = 'user';
                    }
                }
                else {
                    // The value is not a string
                    validResult.errors.push('Attempted to validate a non-string against string measures');
                    validResult.success = false;
                }
                break;

            // Checking if the value is greater than zero
            case Validation_Types.Greater_Than_Zero:
                // Check if the value of the parameter is greater than zero
                if (val.value <= 0) {
                    // The value is not valid
                    validResult.errors.push(val.name + ' can\'t be less than or equal to 0');
                    validResult.success = false;
                    validResult.errorType = 'user';
                }
                break;
        }
    })

    return validResult;
}

/**
 * Name: Generate_Validation_Error_HTML
 * Description: Method to generate HTML for a SweetAlert
 * @param {any} errors Errors with a request
 */
function Generate_Validation_Error_HTML(errors) {
    // Create variables to store basic error info
    let htmlString = '<h4>Your request has the following errors: </h4><ul class="list-group text-left">';
    let numberOfErrors = errors.length;

    // Iterate through each error
    $.each(errors, function (key, val) {
        // Add the error to the HTML
        if (numberOfErrors == key) {
            htmlString += '<li class="list-group-item">' + val + '</li></ul>';
        }
        else {
            htmlString += '<li class="list-group-item">' + val + '</li>';
        }
    });

    // Return the HTML
    return htmlString;
}

//! END Section: Functions