/*
    Name: Ross Taggart
    ID: S1828840
*/

//! Section: Contents

    /*
     * - Variables
     *      - requestDateRange
     * - Functions
     *      - Register_Request_DOM_Handlers
     *      - Request_Date_Range_Callback
     *      - Configure_Request_Date_Ranges
     *      - Handle_Initial_Commit_Button_Click
     *      - Handle_Request_Button_Click
     *      - Handle_Request_Username_Blur
     *      - Handle_Filtered_Contribution_Submit_Click
     *      - Handle_Return_To_Overview_Button_Click
     *      - Load_Collaborator_Overview
     *      - Handle_Request_Builder_Help_FAQ_Toggle
     *      - Handle_Request_Builder_Help_Tutorial_Toggle
     *      - Handle_All_Request_Results_Help_FAQ_Toggle
     *      - Handle_Individual_Request_Results_Help_FAQ_Toggle
     *      - Handle_Request_Builder_Help_Icon_Click
    */

//! END Section: Contents

//! Section: Variables

/**
 * requestDateRange - Object to track the date range a user select
*/
let requestDateRange = {
    start: null,
    end: null
}

//! END Section: Variables

//! Section Functions

/**
 * Name: Register_Request_DOM_Handlers
 * Description: Method to attach handlers to DOM elements
 */
function Register_Request_DOM_Handlers() {
    // Attach the Handle_Request_Button_Click method to the submit request button
    $('#submit-request-btn').on('click', function () {
        Handle_Request_Button_Click();
    });

    // Attach the Handle_Request_Username_Blur method to the username input
    $('#request-username-input').on('blur', function () {
        Handle_Request_Username_Blur();
    });

    // Attach the Handle_Filtered_Contribution_Submit_Click method to the submit filtered contribution submit button
    $('#submit-filtered-contribution-btn').on('click', function () {
        Handle_Filtered_Contribution_Submit_Click();
    });

    // Attach the Handle_Return_To_Overview_Button_Click method to back to the overview button
    $('#back-to-overview-btn').on('click', function () {
        Handle_Return_To_Overview_Button_Click();
    });

    // Attach the Handle_Initial_Commit_Button_Click method to back to the initial commit button
    $('#submit-initial-commit-btn').on('click', function () {
        Handle_Initial_Commit_Button_Click();
    });

    // Attach the Handle_Request_Builder_Help_FAQ_Toggle method to the request builder FAQ button
    $('#request-builder-help-FAQ-selector').on('click', function () {
        Handle_Request_Builder_Help_FAQ_Toggle();
    });

    // Attach the Handle_Request_Builder_Help_Tutorial_Toggle method to the request builder tutorial button
    $('#request-builder-help-tutorial-selector').on('click', function () {
        Handle_Request_Builder_Help_Tutorial_Toggle();
    });

    // Attach the Handle_All_Request_Results_Help_FAQ_Toggle method to the request results help button
    $('#all-request-results-help-FAQ-selector').on('click', function () {
        Handle_All_Request_Results_Help_FAQ_Toggle();
    });

    // Attach the Handle_Individual_Request_Results_Help_FAQ_Toggle method to the request results FAQ button
    $('#individual-request-results-help-FAQ-selector').on('click', function () {
        Handle_Individual_Request_Results_Help_FAQ_Toggle();
    });

    // Attach the Handle_Request_Builder_Help_Icon_Click method to the request builder help button
    $('#request-builder-help-icon').on('click', function () {
        Handle_Request_Builder_Help_Icon_Click();
    });

    // Attach the Display_Request_Help_Modal_Section method to the request builder help button
    $('#display-request-builder-help-btn').on('click', function () {
        Display_Request_Help_Modal_Section($('#request-builder-help-content'));
    });

    // Attach the Display_Request_Help_Modal_Section method to the request results help button
    $('#display-all-request-result-help-btn').on('click', function () {
        Display_Request_Help_Modal_Section($('#all-request-results-help-content'));
    });

    // Attach the Display_Request_Help_Modal_Section method to the individual results help button
    $('#display-individual-result-help-btn').on('click', function () {
        Display_Request_Help_Modal_Section($('#individual-request-results-help-content'));
    })

    // Configure the valid range of the date range picker
    Configure_Request_Date_Ranges();
}

/**
 * Name: Request_Date_Range_Callback
 * Description: Method to overwrite the value of the global date range with the values from the date picker
 * @param {any} start The start of the date range selected
 * @param {any} end The end of the date range selected
 */
function Request_Date_Range_Callback(start, end) {
    // Overwrite the object's parameters by formatting the parameters as moment objects
    requestDateRange.start = moment(start).toDate();
    requestDateRange.end = moment(end).toDate();
}

/**
 * Name: Configure_Request_Date_Ranges
 * Description: Method to configure the date range picker
 */
function Configure_Request_Date_Ranges() {
    // Initialise two new dates
    let currentDate = new Date();
    let lastMonth = new Date();

    // Specify the last month by subtracting 31 days from the current date
    lastMonth.setDate(lastMonth.getDate() - 31);

    // Populate the attributes of the global variables
    requestDateRange.start = currentDate;
    requestDateRange.end = lastMonth;

    // Initialise the date picker, specifyin the method to call when dates are selected
    $('#request-range-picker').daterangepicker({
        "locale": {
            "format": "DD/MM/YYYY",
            "separator": " - ",
            "applyLabel": "Apply",
            "cancelLabel": "Cancel",
            "fromLabel": "From",
            "toLabel": "To",
            "customRangeLabel": "Custom",
            "weekLabel": "W",
            "daysOfWeek": [
                "Su",
                "Mo",
                "Tu",
                "We",
                "Th",
                "Fr",
                "Sa"
            ],
            "monthNames": [
                "January",
                "February",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December"
            ],
            "firstDay": 1
        },
        "startDate": lastMonth,
        "endDate": currentDate,
        "maxDate": currentDate,
        "opens": "center"
    }, Request_Date_Range_Callback);
}

/**
 * Name: Handle_Initial_Commit_Button_Click
 * Description: Method to handle an initial commit bias submit
*/
function Handle_Initial_Commit_Button_Click() {
    // Create a variable to store a list of restricted commit identifiers
    restrictedCommits = [];

    // Get the selected restricted commits
    let initCommitSelections = $('#initial-commit-table').bootstrapTable('getSelections');

    // Iterate through each restricted commit and add to the restricted list
    $.each(initCommitSelections, function (key, val) {
        restrictedCommits.push(val.id);
    });

    // Get all applicable values
    let username = $('#request-username-input').val();
    let repositoryName = $('#request-repository-select').val();
    let addThreshold = $('#request-add-threshold-input').val();
    let delThreshold = $('#request-del-threshold-input').val();
    let commitThreshold = $('#request-commit-threshold-input').val();

    // Create an object used to validate parameters
    let requestValueKeyVal = [
        {
            name: 'Username',
            value: username,
            validationType: Validation_Types.Emptiness
        },
        {
            name: 'Repository',
            value: repositoryName,
            validationType: Validation_Types.Emptiness
        },
        {
            name: 'Addition Threshold',
            value: addThreshold,
            validationType: Validation_Types.Greater_Than_Zero
        },
        {
            name: 'Deletion Threshold',
            value: delThreshold,
            validationType: Validation_Types.Greater_Than_Zero
        },
        {
            name: 'Commit Threshold',
            value: commitThreshold,
            validationType: Validation_Types.Greater_Than_Zero
        }
    ];

    // Check if options are valid
    let requestDataValid = Basic_Content_Validator(requestValueKeyVal);

    // If validation is successful
    if (requestDataValid.success) {
        $('#initial-commit-modal').modal('hide');
        $('#request-loader-text').text('Loading repository bias...');
        $('#request-loader-container').slideDown();

        // Create an object to store parameters for a bias request
        let filteredRequestObj = {
            User_Name: username,
            Repo_Name: repositoryName,
            Addition_Threshold: addThreshold,
            Deletion_Threshold: delThreshold,
            Commit_Threshold: commitThreshold,
            Start: requestDateRange.start,
            End: requestDateRange.end,
            Restricted_Commits: restrictedCommits
        };

        // SUbmit bias request
        Submit_AJAX_POST_Request(requestEndpointContainer.getRepoBias, filteredRequestObj, Get_Repo_Bias_Request_Handler);
    }
}

/**
 * Name: Handle_Request_Button_Click
 * Description: Method for handling a request button click
*/
function Handle_Request_Button_Click() {
    let fetchedReposVisible = $('#repos-fetched-icon').hasClass('visible');

    // Check if the repositories have been fetched
    if (fetchedReposVisible) {
        // Get all applicable values
        let username = $('#request-username-input').val();
        let repositoryName = $('#request-repository-select').val();
        let addThreshold = $('#request-add-threshold-input').val();
        let delThreshold = $('#request-del-threshold-input').val();
        let commitThreshold = $('#request-commit-threshold-input').val();

        // Create object used to validate the parameters
        let requestValueKeyVal = [
            {
                name: 'Username',
                value: username,
                validationType: Validation_Types.Emptiness
            },
            {
                name: 'Repository',
                value: repositoryName,
                validationType: Validation_Types.Emptiness
            },
            {
                name: 'Addition Threshold',
                value: addThreshold,
                validationType: Validation_Types.Greater_Than_Zero
            },
            {
                name: 'Deletion Threshold',
                value: delThreshold,
                validationType: Validation_Types.Greater_Than_Zero
            },
            {
                name: 'Commit Threshold',
                value: commitThreshold,
                validationType: Validation_Types.Greater_Than_Zero
            }
        ];

        // Validate the parameters
        let requestDataValid = Basic_Content_Validator(requestValueKeyVal);

        // if the parameters are valid
        if (requestDataValid.success) {
            $('#request-loader-text').text('Loading initial commits...');
            $('#request-loader-container').slideDown();

            // Submit the initial repo fetch
            Submit_GET_Request(requestEndpointContainer.getRepoInitCommits, [{ name: 'userName', value: username }, { name: 'repoName', value: repositoryName }], Get_Initial_Commit_Handler);
        }
        // Validation failed
        else {
            // Create Swal object
            let advancedSwalOptions = {
                type: 'error',
                title: 'Request data is invalid',
                html: Generate_Validation_Error_HTML(requestDataValid.errors)
            };

            // Inform the user that they have entered invalid data
            Display_Sweet_Alert('error', { use: false }, advancedSwalOptions);
        }
    }
    // If the repos have not been fetched
    else {
        // Create a Swal object
        let requestClickBasicSwalOptions = {
            use: true,
            title: 'Repositories are not loaded',
            text: 'Please wait for the selected user\'s repositories to be loaded before submitting',
            type: 'warning'
        };

        // Warn the user that no repos have been fetched
        Display_Sweet_Alert('warning', requestClickBasicSwalOptions, null);
    }
}

/**
 * Name: Handle_Request_Username_Blur
 * Description: Method to trigger when a user toggles off the username input
*/
function Handle_Request_Username_Blur() {
    $('#repos-not-fetched-icon').fadeOut(300).promise().done(function () {
        $('#repos-loading-icon').fadeIn(300).promise().done(function () {
            // Get the username the user entered
            let username = $('#request-username-input').val();

            // Create an object to validate the username
            let usernameValidatorKeyVal = [{ name: 'Username', value: username, validationType: Validation_Types.Emptiness }];

            // Validate the username
            let usernameValid = Basic_Content_Validator(usernameValidatorKeyVal, Validation_Types.Emptiness, 'string');

            // If the username is valid
            if (usernameValid.success) {
                // Get the user's repos
                Submit_GET_Request(requestEndpointContainer.getUserRepos, usernameValidatorKeyVal, Get_User_Repo_Request_Handler);
            }
            // The username is not valid
            else {
                // Create a Swal object
                let advancedSwalOptions = {
                    type: 'error',
                    title: 'Username is invalid',
                    html: Generate_Validation_Error_HTML(usernameValid.errors)
                };

                // Warn the user that the username is not valid
                Display_Sweet_Alert('error', { use: false }, advancedSwalOptions);

                $('#repos-loading-icon').fadeOut(300).promise().done(function () {
                    $('#repos-not-fetched-icon').fadeIn(300);
                    $('#repos-not-fetched-icon').addClass('visible');
                })
            }
        });
    })
}

/**
 * Name: Handle_Filtered_Contribution_Submit_Click
 * Description: Method to handle submitting filtered options
*/
function Handle_Filtered_Contribution_Submit_Click() {
    // Hide the bias modal
    $('#repo-bias-modal').modal('hide').promise().done(function () {
        $('#request-loader-text').text('Loading filtered repository stats...');
        $('#request-loader-container').slideDown();
    })

    // Check what bias displays are available
    let githubVisible = $('#bias-github-commit-display').hasClass('visible');
    let additionVisible = $('#bias-addition-commit-display').hasClass('visible');
    let deletionVisible = $('#bias-deletion-commit-display').hasClass('visible');

    // Create a variable to store restricted characters
    let restrictedShas = [];

    // If the bias GitHub display is visible
    if (githubVisible) {
        // Fetch the restricted GitHub commits
        let githubSelections = $('#bias-github-commit-table').bootstrapTable('getSelections');

        $.each(githubSelections, function (key, val) { restrictedShas.push(val.id) });
    }
    // If the bias addition display is visible
    if (additionVisible) {
        // Fetch the restricted addition commits
        let additionSelections = $('#bias-addition-commit-table').bootstrapTable('getSelections');

        $.each(additionSelections, function (key, val) { restrictedShas.push(val.id) });
    }
    // If the bias deletion display is visible
    if (deletionVisible) {
        // Fetch the restricted deletion commits
        let deletionSelections = $('#bias-deletion-commit-table').bootstrapTable('getSelections');

        $.each(deletionSelections, function (key, val) { restrictedShas.push(val.id) });
    }

    // Add all bias restriction to the global restrictions
    $.each(restrictedCommits, function (key, val) { restrictedShas.push(val); })

    // Get the options
    let username = $('#request-username-input').val();
    let repositoryName = $('#request-repository-select').val();
    let addThreshold = $('#request-add-threshold-input').val();
    let delThreshold = $('#request-del-threshold-input').val();
    let commitThreshold = $('#request-commit-threshold-input').val();

    // Create a request object for the repository stats
    let filteredRequestObj = {
        User_Name: username,
        Repo_Name: repositoryName,
        Addition_Threshold: addThreshold,
        Deletion_Threshold: delThreshold,
        Commit_Threshold: commitThreshold,
        Start: requestDateRange.start,
        End: requestDateRange.end,
        Restricted_Commits: restrictedShas
    };

    // Fetch the repository stats
    Submit_AJAX_POST_Request(requestEndpointContainer.getRepoStats, filteredRequestObj, Get_Repo_Stats_Handler);
}

/**
 * Name: Handle_Return_To_Overview_Button_Click
 * Description: Method handle individual stats buck button 
*/
function Handle_Return_To_Overview_Button_Click() {
    // Fade individual stats out, re-fade overall view
    $('#single-request-result-container').fadeOut(300).promise().done(function () {
        $('#main-request').fadeIn(300);
    })
}

/**
 * Name: Load_Collaborator_Overview
 * Description: Method to populate and display individual stats for a collaborator
 * @param {any} author The ID of the target collaborator
 */
function Load_Collaborator_Overview(author) {
    // Object to track repository totals
    let repoInfo = {
        Commit_Total: 0,
        Addition_Total: 0,
        Deletion_Total: 0
    };

    // Iterate through all stats and populate the repository totals
    $.each(stats, function (key, val) {
        repoInfo.Commit_Total += val.total;
        repoInfo.Addition_Total += val.additions;
        repoInfo.Deletion_Total += val.deletions;
    });

    // Get the target collcaborator's stats and score
    let collaborator = stats.find(x => x.author.id == author);
    let score = scores.find(x => x.author_Id == author);

    $('#single-request-result-author-name').text(collaborator.author.login + '\'s Contribution Breakdown');

    // Displays the user's icon
    $('#author-icon').empty().promise().done(function () {
        $('#author-icon').append('<div class="row">' +
            '<div class="col-12 text-center">' +
            '<img src="' + collaborator.author.avatar_Url + '" style="width:150px; height: 150px"></img>' +
            '</div>' +
            '<div class="col-12 text-center">' +
            '<span class="contribution-text">' + collaborator.author.login + '</span>' +
            '</div>');
    });

    // Display's the collaborator's top-level stats
    $('#author-top-level-stats').empty().promise().done(function () {
        $('#author-top-level-stats').append('<div class="col-4 text-center">' +
            '<h4>Contribution Stats</h4>' +
            '<h6>Commit Count: <span class="primary-text">' + collaborator.total + '</span></h6>' +
            '<h6>Total Additions: <span class="secondary-text">' + collaborator.additions + '</span></h6>' +
            '<h6>Total Deletions: <span class="warn-text">' + collaborator.deletions + '</span></h6>' +
            '</div>' +
            '<div class="col-4 text-center">' +
            '<h4>Repository Stats</h4>' +
            '<h6>Name: ' + $('#request-repository-select').val() + '</h6>' +
            '<h6>Total Commits: <span class="primary-text">' + repoInfo.Commit_Total + '</span></h6>' +
            '<h6>Total Additions: <span class="secondary-text">' + repoInfo.Addition_Total + '</span></h6>' +
            '<h6>Total Deletions: <span class="warn-text">' + repoInfo.Deletion_Total + '</span></h6>' +
            '</div>' +
            '<div class="col-4 text-center">' +
            '<h4>Contribution(%)</h4>' +
            '<div style="height:150px" id="author-contribution-chart"></div>');
    })

    // Create a generic object to contain basic gauge options
    var gaugeOptions = {

        chart: {
            type: 'solidgauge'
        },

        title: null,

        pane: {
            center: ['50%', '50%'],
            size: '70%',
            startAngle: -90,
            endAngle: 90,
            background: {
                backgroundColor:
                    Highcharts.defaultOptions.legend.backgroundColor || '#EEE',
                innerRadius: '60%',
                outerRadius: '100%',
                shape: 'arc'
            }
        },

        tooltip: {
            enabled: false
        },

        // the value axis
        yAxis: {
            stops: [
                [0.1, '#DF5353'], // red
                [0.5, '#DDDF0D'], // yellow
                [0.9, '#55BF3B'], // green
            ],
            lineWidth: 0,
            minorTickInterval: null,
            //tickAmount: 2,
            title: {
                y: -70
            },
            labels: {
                y: 16
            }
        },

        plotOptions: {
            solidgauge: {
                dataLabels: {
                    y: 5,
                    borderWidth: 0,
                    useHTML: true
                }
            }
        },

        exporting: {
            enabled: false
        }
    };

    // Add a gauge chart for the user's overall contribution chart
    Highcharts.chart('author-contribution-chart', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 100,
            title: {
                text: 'Contribution'
            }
        },

        credits: {
            enabled: false
        },

        series: [{
            name: 'Contribution',
            data: [score.score.contribution_Score],
            dataLabels: {
                format:
                    '<div style="text-align:center; padding-top: 5px;">' +
                    '<span style="font-size:12px">{y:.1f}</span><br/>' +
                    '<span style="font-size:12px;opacity:0.4">%</span>' +
                    '</div>'
            },
            tooltip: {
                valueSuffix: '%'
            }
        }]

    }));

    // Create an array to store timeline data
    let timelineData = [];

    $.each(collaborator.commits.reverse(), function (key, val) {
        try {
            // Add commit data to the array
            timelineData.push({
                name: val.commit.committer.date,
                label: val.commit.message,
                description: '<b>Author: </b>' + val.author.login + '<br/>' + '<b>Message: </b>' + val.commit.message
            });
        }
        catch (ex) {
            console.log(ex.message);
        }
        
    });

    // Create a timeline chart with the options
    Highcharts.chart('collaborator-commit-timeline-chart', {
        chart: {
            type: 'timeline',
            zoomType: 'x'
        },
        xAxis: {
            visible: false
        },
        yAxis: {
            visible: false
        },
        title: {
            text: collaborator.author.login + ' commit history'
        },
        plotOptions: {
            series: {
                dataLabels: {
                    enabled: false
                }
            },
            turboThreshold: 2
        },
        series: [{
            data: timelineData
        }]
    });

    // Add a gauge chart for the user's commit score contribution chart
    Highcharts.chart('commit-score-gauge', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 50,
            title: {
                text: 'Commit'
            }
        },

        credits: {
            enabled: false
        },

        series: [{
            name: 'Commit',
            data: [score.score.commit_Score],
            dataLabels: {
                format:
                    '<div style="text-align:center; padding-top: 5px;">' +
                    '<span style="font-size:12px">{y:.1f}</span><br/>' +
                    '<span style="font-size:12px;opacity:0.4">%</span>' +
                    '</div>'
            },
            tooltip: {
                valueSuffix: '%'
            }
        }]

    }));

    // Add a gauge chart for the user's addition score contribution chart
    Highcharts.chart('add-score-gauge', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 25,
            title: {
                text: 'Addition'
            }
        },

        credits: {
            enabled: false
        },

        series: [{
            name: 'Addition',
            data: [score.score.addition_Score],
            dataLabels: {
                format:
                    '<div style="text-align:center; padding-top: 5px;">' +
                    '<span style="font-size:12px">{y:.1f}</span><br/>' +
                    '<span style="font-size:12px;opacity:0.4">%</span>' +
                    '</div>'
            },
            tooltip: {
                valueSuffix: '%'
            }
        }]

    }));

    // Add a gauge chart for the user's deletion score contribution chart
    Highcharts.chart('del-score-gauge', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 25,
            title: {
                text: 'Deletion'
            }
        },

        credits: {
            enabled: false
        },

        series: [{
            name: 'Commit',
            data: [score.score.deletion_Score],
            dataLabels: {
                format:
                    '<div style="text-align:center; padding-top: 5px;">' +
                    '<span style="font-size:12px">{y:.1f}</span><br/>' +
                    '<span style="font-size:12px;opacity:0.4">%</span>' +
                    '</div>'
            },
            tooltip: {
                valueSuffix: '%'
            }
        }]

    }));

    // Display the commit score equation
    $('#commit-score-equation-text').text('(' + collaborator.total + ' / ' + repoInfo.Commit_Total + ') * 50');
    $('#commit-score-equation-result-text').text(score.score.commit_Score.toFixed(2));

    // Display the additon score equation
    $('#addition-score-equation-text').text('(' + collaborator.additions + ' / ' + repoInfo.Addition_Total + ') * 25');
    $('#addition-score-equation-result-text').text(score.score.addition_Score.toFixed(2));

    // Display the deletion score equation
    $('#deletion-score-equation-text').text('(' + collaborator.deletions + ' / ' + repoInfo.Deletion_Total + ') * 25');
    $('#deletion-score-equation-result-text').text(score.score.deletion_Score.toFixed(2));

    $('#main-request').fadeOut(300).promise().done(function () {
        $('#single-request-result-container').fadeIn(300);
    })
}

/**
 * Name: Handle_Request_Builder_Help_FAQ_Toggle
 * Description: Method to display FAQs for the request builder
*/
function Handle_Request_Builder_Help_FAQ_Toggle() {
    // Display the Request Builder FAQ section
    $('#request-builder-help-FAQ-section').slideToggle().promise().done(function () {
        if ($('#request-builder-help-FAQ-section').is(':visible')) {
            $('#request-builder-help-FAQ-down-icon').fadeOut(300).promise().done(function () {
                $('#request-builder-help-FAQ-up-icon').fadeIn(300);
            })
        }
        else {
            $('#request-builder-help-FAQ-up-icon').fadeOut(300).promise().done(function () {
                $('#request-builder-help-FAQ-down-icon').fadeIn(300);
            })
        }
    })
}

/**
 * Name: Handle_Request_Builder_Help_Tutorial_Toggle
 * Description: Method to display the tutorial for the request builder
*/
function Handle_Request_Builder_Help_Tutorial_Toggle() {
    // Display the Request Builder tutorial section
    $('#request-builder-help-tutorial-section').slideToggle().promise().done(function () {
        if ($('#request-builder-help-tutorial-section').is(':visible')) {
            $('#request-builder-help-tutorial-down-icon').fadeOut(300).promise().done(function () {
                $('#request-builder-help-tutorial-up-icon').fadeIn(300);
            })
        }
        else {
            $('#request-builder-help-tutorial-up-icon').fadeOut(300).promise().done(function () {
                $('#request-builder-help-tutorial-down-icon').fadeIn(300);
            })
        }
    })
}

/**
 * Name: Handle_All_Request_Results_Help_FAQ_Toggle
 * Description: Method to display the FAQs for the request results
*/
function Handle_All_Request_Results_Help_FAQ_Toggle() {
    // Display the Request Results FAQ sections
    $('#all-request-results-help-FAQ-section').slideToggle().promise().done(function () {
        if ($('#all-request-results-help-FAQ-section').is(':visible')) {
            $('#all-request-results-help-FAQ-down-icon').fadeOut(300).promise().done(function () {
                $('#all-request-results-help-FAQ-up-icon').fadeIn(300);
            })
        }
        else {
            $('#all-request-results-help-FAQ-up-icon').fadeOut(300).promise().done(function () {
                $('#all-request-results-help-FAQ-down-icon').fadeIn(300);
            })
        }
    })
}

/**
 * Name: Handle_Individual_Request_Results_Help_FAQ_Toggle
 * Description: Method to display the FAQs for the individual request results
*/
function Handle_Individual_Request_Results_Help_FAQ_Toggle() {
    // Display the individual request results FAQ section
    $('#individual-request-results-help-FAQ-section').slideToggle().promise().done(function () {
        if ($('#individual-request-results-help-FAQ-section').is(':visible')) {
            $('#individual-request-results-help-FAQ-down-icon').fadeOut(300).promise().done(function () {
                $('#individual-request-results-help-FAQ-up-icon').fadeIn(300);
            })
        }
        else {
            $('#individual-request-results-help-FAQ-up-icon').fadeOut(300).promise().done(function () {
                $('#individual-request-results-help-FAQ-down-icon').fadeIn(300);
            })
        }
    })
}

/**
 * Name: Handle_Request_Builder_Help_Icon_Click
 * Description: Method to handle the help icon click
*/
function Handle_Request_Builder_Help_Icon_Click() {
    // Display the modal section
    Display_Request_Help_Modal_Section($('#request-help-selector-content'));
}
