function Register_Request_DOM_Handlers() {
    $('#submit-request-btn').on('click', function () {
        Handle_Request_Button_Click();
    });

    $('#request-username-input').on('blur', function () {
        Handle_Request_Username_Blur();
    });

    $('#submit-filtered-contribution-btn').on('click', function () {
        Handle_Filtered_Contribution_Submit_Click();
    });

    Configure_Request_Date_Ranges();
}

let requestDateRange = {
    start: null,
    end: null
}

function Request_Date_Range_Callback(start, end) {
    requestDateRange.start = moment(start).toDate();
    requestDateRange.end = moment(end).toDate();

    console.log(requestDateRange);
}

function Configure_Request_Date_Ranges() {
    // Request Range Picker
    let currentDate = new Date();
    let lastMonth = new Date();
    
    lastMonth.setDate(lastMonth.getDate() - 31);

    requestDateRange.start = currentDate;
    requestDateRange.end = lastMonth;

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

function Handle_Request_Button_Click() {
    console.log('In Click');

    let fetchedReposVisible = $('#repos-fetched-icon').hasClass('visible');

    if (fetchedReposVisible) {
        // Get all applicable values
        let username = $('#request-username-input').val();
        let repositoryName = $('#request-repository-select').val();
        let addThreshold = $('#request-add-threshold-input').val();
        let delThreshold = $('#request-del-threshold-input').val();

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
                validationTypes: Validation_Types.Greater_Than_Zero
            },
            {
                name: 'Deletion Threshold',
                value: delThreshold,
                validationTypes: Validation_Types.Greater_Than_Zero
            }
        ];

        let requestDataValid = Basic_Content_Validator(requestValueKeyVal);

        if (requestDataValid) {
            $('#request-loader-text').text('Loading repository bias...');
            $('#request-loader-container').slideDown();

            let filteredRequestObj = {
                User_Name: username,
                Repo_Name: repositoryName,
                Addition_Threshold: addThreshold,
                Deletion_Threshold: delThreshold,
                Start: requestDateRange.start,
                End: requestDateRange.end
            };
            Submit_AJAX_POST_Request(requestEndpointContainer.getRepoBias, filteredRequestObj, Get_Repo_Bias_Request_Handler);
        }
    }
    else {
        let requestClickBasicSwalOptions = {
            use: true,
            title: 'Repositories are not loaded',
            text: 'Please wait for the selected user\'s repositories to be loaded before submitting',
            type: 'warning'
        };

        Display_Sweet_Alert('warning', requestClickBasicSwalOptions, null);
    }
}

function Handle_Request_Username_Blur() {
    // Handles fetching repositories for the defined username

    /*
     * Fetch value of username
     * Validate value of username
     * Submit username to the server
    */

    $('#repos-not-fetched-icon').fadeOut(300).promise().done(function () {
        $('#repos-loading-icon').fadeIn(300).promise().done(function () {
            let username = $('#request-username-input').val();

            let usernameValidatorKeyVal = [{ name: 'Username', value: username, validationType: Validation_Types.Emptiness }];

            let usernameValid = Basic_Content_Validator(usernameValidatorKeyVal, Validation_Types.Emptiness, 'string');

            if (usernameValid.success) {
                Submit_GET_Request(requestEndpointContainer.getUserRepos, usernameValidatorKeyVal, Get_User_Repo_Request_Handler);
            }
            else {
                let advancedSwalOptions = {
                    type: 'error',
                    title: 'Username is invalid',
                    html: Generate_Validation_Error_HTML(usernameValid.errors)
                };

                Display_Sweet_Alert('error', { use: false }, advancedSwalOptions);

                $('#repos-loading-icon').fadeOut(300).promise().done(function () {
                    $('#repos-not-fetched-icon').fadeIn(300);
                    $('#repos-not-fetched-icon').addClass('visible');
                })
            }
        });
    })
}

function Handle_Filtered_Contribution_Submit_Click() {
    // Check to see what tables are visible

    let githubVisible = $('#bias-github-commit-display').hasClass('visible');
    let additionVisible = $('#bias-addition-commit-display').hasClass('visible');
    let deletionVisible = $('#bias-deletion-commit-display').hasClass('visible');

    let restrictedShas = [];

    if (githubVisible) {
        let githubSelections = $('#bias-github-commit-table').bootstrapTable('getSelections');

        $.each(githubSelections, function (key, val) { restrictedShas.push(val.id) });
    }
    if (additionVisible) {
        let additionSelections = $('#bias-addition-commit-table').bootstrapTable('getSelections');

        $.each(additionSelections, function (key, val) { restrictedShas.push(val.id) });
    }
    if (deletionVisible) {
        let deletionSelections = $('#bias-deletion-commit-table').bootstrapTable('getSelections');

        $.each(deletionSelections, function (key, val) { restrictedShas.push(val.id) });
    }

    console.log(restrictedShas);

    let username = $('#request-username-input').val();
    let repositoryName = $('#request-repository-select').val();
    let addThreshold = $('#request-add-threshold-input').val();
    let delThreshold = $('#request-del-threshold-input').val();

    let filteredRequestObj = {
        User_Name: username,
        Repo_Name: repositoryName,
        Addition_Threshold: addThreshold,
        Deletion_Threshold: delThreshold,
        Start: requestDateRange.start,
        End: requestDateRange.end,
        Restricted_Commits: restrictedShas
    };

    Submit_AJAX_POST_Request(requestEndpointContainer.getRepoStats, filteredRequestObj, Get_Repo_Stats_Handler);
}

function Display_Overall_Repo_Pie(chartItem, render) {

    pieData.push(chartItem);

    /*let chart = Highcharts.chart('overall-repo-pie', {
        chart: {
            type: 'pie',
            
        },
        title: {
            text: 'Collaborator Contribution Overview'
        },
        subtitle: {
            text: ''
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Contribution Score',
            colorByPoint: true,
            data: [
                chartData
            ]
        }]
    });

    console.log(chart);*/

    if (render) {
        Highcharts.chart('overall-repo-pie', {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: 'Browser market shares in January, 2018'
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            accessibility: {
                point: {
                    valueSuffix: '%'
                }
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: true
                }
            },
            series: [{
                name: 'Brands',
                colorByPoint: true,
                data: pieData
            }]
        });

        $('#overall-repo-pie-container').fadeIn(300);
    }
    
}

function Load_Collaborator_Overview(author) {
    console.log(author);

    let repoInfo = {
        Commit_Total: 0,
        Addition_Total: 0,
        Deletion_Total: 0
    };

    $.each(stats, function (key, val) {
        repoInfo.Commit_Total += val.total;
        repoInfo.Addition_Total += val.additions;
        repoInfo.Deletion_Total += val.deletions;
    });

    let collaborator = stats.find(x => x.author.id == author);

    console.log(collaborator);

    $('#single-request-result-author-name').text(collaborator.author.login + '\'s Contribution Breakdown');

    $('#author-icon').empty().promise().done(function () {
        $('#author-icon').append('<div class="row">' +
            '<div class="col-12 text-center">' +
            '<img src="' + collaborator.author.avatar_Url + '" style="width:150px; height: 150px"></img>' +
            '</div>' +
            '<div class="col-12 text-center">' +
            '<span class="contribution-text">' + collaborator.author.login + '</span>' +
            '</div>');
    });

    $('#author-top-level-stats').append('<div class="col-4 text-center">' +
        '<h4>Contribution Stats</h4>' +
        '<h6>Commit Count: ' + collaborator.total + '</h6>' +
        '<h6>Total Additions: ' + collaborator.additions + '</h6>' +
        '<h6>Total Deletions: ' + collaborator.deletions + '</h6>' +
        '</div>' +
        '<div class="col-4 text-center">' +
        '<h4>Repository Stats</h4>' +
        '<h6>Name: ' + $('#request-repository-select').val() + '</h6>' +
        '<h6>Total Commits: ' + repoInfo.Commit_Total + '</h6>' +
        '<h6>Total Additions: ' + repoInfo.Addition_Total + '</h6>' +
        '<h6>Total Deletions: ' + repoInfo.Deletion_Total + '</h6>' +
        '</div>' +
        '<div class="col-4 text-center">' +
        '<h4>Contribution(%)</h4>' +
        '<div style="height:150px" id="author-contribution-chart"></div>');

    $('#main-request').fadeOut(300).promise().done(function () {
        $('#single-request-result-container').fadeIn(300);
    })
}
