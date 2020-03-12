function Basic_Content_Validator(values) {
    let validResult = {
        errors: [],
        success: true,
        errorType: 'system'
    };

    $.each(values, function (key, val) {
        switch (val.validationType) {
            case Validation_Types.Emptiness:
                if (typeof (val.value) == 'string') {
                    if (val.value == '' || val.value == ' ') {
                        validResult.errors.push(val.name + ' can\'t be empty or only a space');
                        validResult.success = false;
                        validResult.errorType = 'user';
                    }
                }
                else {
                    validResult.errors.push('Attempted to validate a non-string against string measures');
                    validResult.success = false;
                }
                break;

            case Validation_Types.Greater_Than_Zero:
                console.log('Validating: ', val);
                if (val.value <= 0) {
                    validResult.errors.push(val.name + ' can\'t be less than or equal to 0');
                    validResult.success = false;
                    validResult.errorType = 'user';
                }
                break;
        }
    })

    return validResult;
}

const Validation_Types = Object.freeze({
    Emptiness: { name: "empty", usedOn: "string" },
    Greater_Than_Zero: { name: ">0", usedOn: "number" }
});

function Generate_Validation_Error_HTML(errors) {
    let htmlString = '<h4>Your request has the following errors: </h4><ul class="list-group text-left">';
    let numberOfErrors = errors.length;

    $.each(errors, function (key, val) {
        // Add each error as a list item
        if (numberOfErrors == key) {
            htmlString += '<li class="list-group-item">' + val + '</li></ul>';
        }
        else {
            htmlString += '<li class="list-group-item">' + val + '</li>';
        }
    });

    return htmlString;
}