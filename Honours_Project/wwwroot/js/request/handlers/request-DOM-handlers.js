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

    $('#back-to-overview-btn').on('click', function () {
        Handle_Return_To_Overview_Button_Click();
    });

    $('#submit-initial-commit-btn').on('click', function () {
        Handle_Initial_Commit_Button_Click();
    });

    $('#request-builder-help-FAQ-selector').on('click', function () {
        Handle_Request_Builder_Help_FAQ_Toggle();
    });

    $('#request-builder-help-tutorial-selector').on('click', function () {
        Handle_Request_Builder_Help_Tutorial_Toggle();
    });

    $('#request-builder-help-icon').on('click', function () {
        Handle_Request_Builder_Help_Icon_Click();
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

function Handle_Initial_Commit_Button_Click() {
    restrictedCommits = [];
    let initCommitSelections = $('#initial-commit-table').bootstrapTable('getSelections');

    $.each(initCommitSelections, function (key, val) {
        restrictedCommits.push(val.id);
    });

    // Get all applicable values
    let username = $('#request-username-input').val();
    let repositoryName = $('#request-repository-select').val();
    let addThreshold = $('#request-add-threshold-input').val();
    let delThreshold = $('#request-del-threshold-input').val();
    let commitThreshold = $('#request-commit-threshold-input').val();

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

    let requestDataValid = Basic_Content_Validator(requestValueKeyVal);

    if (requestDataValid.success) {
        $('#initial-commit-modal').modal('hide');
        $('#request-loader-text').text('Loading repository bias...');
        $('#request-loader-container').slideDown();

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

        Submit_AJAX_POST_Request(requestEndpointContainer.getRepoBias, filteredRequestObj, Get_Repo_Bias_Request_Handler);
    }
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
        let commitThreshold = $('#request-commit-threshold-input').val();

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

        let requestDataValid = Basic_Content_Validator(requestValueKeyVal);

        if (requestDataValid.success) {
            $('#request-loader-text').text('Loading initial commits...');
            $('#request-loader-container').slideDown();

            Submit_GET_Request(requestEndpointContainer.getRepoInitCommits, [{ name: 'userName', value: username }, { name: 'repoName', value: repositoryName }], Get_Initial_Commit_Handler);
        }
        else {
            let advancedSwalOptions = {
                type: 'error',
                title: 'Request data is invalid',
                html: Generate_Validation_Error_HTML(requestDataValid.errors)
            };

            Display_Sweet_Alert('error', { use: false }, advancedSwalOptions);
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
    $('#repo-bias-modal').modal('hide').promise().done(function () {
        $('#request-loader-text').text('Loading filtered repository stats...');
        $('#request-loader-container').slideDown();
    })
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

        let testMassDels = [];

        $.each(deletionSelections, function (key, val) { testMassDels.push(val.id) });

        console.log('Excluded Mass Deletion Commits: ', testMassDels);

        $.each(deletionSelections, function (key, val) { restrictedShas.push(val.id) });
    }

    $.each(restrictedCommits, function (key, val) { restrictedShas.push(val); })

    let username = $('#request-username-input').val();
    let repositoryName = $('#request-repository-select').val();
    let addThreshold = $('#request-add-threshold-input').val();
    let delThreshold = $('#request-del-threshold-input').val();
    let commitThreshold = $('#request-commit-threshold-input').val();

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

    Submit_AJAX_POST_Request(requestEndpointContainer.getRepoStats, filteredRequestObj, Get_Repo_Stats_Handler);
}

function Handle_Return_To_Overview_Button_Click() {
    $('#single-request-result-container').fadeOut(300).promise().done(function () {
        $('#main-request').fadeIn(300);
    })
}

function Load_Collaborator_Overview(author) {
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
    let score = scores.find(x => x.author_Id == author);

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

    // The speed gauge
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

    // Timeline

    let timelineData = [];

    $.each(collaborator.commits, function (key, val) {
        timelineData.push({
            name: val.commit.committer.date,
            label: val.commit.message,
            description: '<b>Author: </b>' + val.author.login + '<br/>' + '<b>Message: </b>' + val.commit.message
        });
    });

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
            }
        },
        series: [{
            data: timelineData
        }]
    });

    // Score Breakdown

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

    $('#commit-score-equation-text').text('(' + collaborator.total + ' / ' + repoInfo.Commit_Total + ') * 50');
    $('#commit-score-equation-result-text').text(score.score.commit_Score.toFixed(2));

    $('#addition-score-equation-text').text('(' + collaborator.additions + ' / ' + repoInfo.Addition_Total + ') * 25');
    $('#addition-score-equation-result-text').text(score.score.addition_Score.toFixed(2));

    $('#deletion-score-equation-text').text('(' + collaborator.deletions + ' / ' + repoInfo.Deletion_Total + ') * 25');
    $('#deletion-score-equation-result-text').text(score.score.deletion_Score.toFixed(2));

    $('#main-request').fadeOut(300).promise().done(function () {
        $('#single-request-result-container').fadeIn(300);
    })
}

function Handle_Request_Builder_Help_FAQ_Toggle() {
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

function Handle_Request_Builder_Help_Tutorial_Toggle() {
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

function Handle_Request_Builder_Help_Icon_Click() {
    Display_Request_Help_Modal_Section($('#request-builder-help-content'));
}
