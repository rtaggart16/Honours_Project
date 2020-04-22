/*
    Name: Ross Taggart
    ID: S1828840
*/

//! Section: Contents

    /*
     * - Variables
     *      - stats
     *      - scores
     *      - restrictedCommits
     * - Functions
     *      - Get_User_Repo_Request_Handler
     *      - Get_Repo_Bias_Request_Handler
     *      - Get_Initial_Commit_Handler
     *      - Get_Repo_Stats_Handler
     *      - Get_Contribution_Score_Handler
    */

//! END Section: Contents


//! Section: Variables

/**
 * stats - Variable to hold top-level GitHub statistics
*/
let stats = [];

/**
 * scores - Variable to hold top-level scores for each collaborator
*/
let scores = [];

/**
 * restrictedCommits - Variable to hold the SHA ids of commits that the user does not want to process
*/
let restrictedCommits = [];

//! END Section: Variables


//! Section: Functions

/**
 * Name: Get_User_Repo_Request_Handler
 * Description: Method to handle the result of a repository fetch
 * @param {any} result Object that contains the fetched repositories as well as the status
 */
function Get_User_Repo_Request_Handler(result) {
    // Check the type of the status code of the result
    switch (result.status.status_Code) {
        // Fetch was successful
        case 200:
            // If any repositories were identified
            if (result.repos.length > 0) {
                // Empty the select box
                $('#request-repository-select').empty();

                // Iterate through each repository and add it to the select
                $.each(result.repos, function (key, val) {
                    $('#request-repository-select').append('<option value="' + val.name + '">' + val.name + '</option>');
                })

                // Fade out the loader
                $('#repos-loading-icon').fadeOut(300).promise().done(function () {
                    $('#repos-fetched-icon').fadeIn(300);
                    $('#repos-fetched-icon').addClass('visible');
                })
            }
            // No repositories were identified
            else {
                // Create options for the Swal
                let basicEmptySwalOptions = {
                    use: true,
                    type: 'warning',
                    title: 'No public repositories',
                    text: 'The given username is valid, however, no public repositories were found'
                };

                // Fade out the loader
                $('#repos-loading-icon').fadeOut(300).promise().done(function () {
                    $('#repos-fetched-icon').fadeOut();
                    $('#repos-not-fetched-icon').fadeIn(300);
                    $('#repos-fetched-icon').removeClass('visible');
                })

                // Warn the user that there are no repositories
                Display_Sweet_Alert('warning', basicEmptySwalOptions, null);
            }
            break;

        // No repositories identified for the given username
        case 404:
            // Create options for the Swal
            let basicSwalOptions = {
                use: true,
                type: 'error',
                title: 'Not a valid Github username',
                text: 'The username entered is not a valid Github username'
            };

            // Fade out the loader
            $('#repos-loading-icon').fadeOut(300).promise().done(function () {
                $('#repos-fetched-icon').fadeOut();
                $('#repos-not-fetched-icon').fadeIn(300);
                $('#repos-fetched-icon').removeClass('visible');
            })

            // Inform the user that the GitHub username is not valid
            Display_Sweet_Alert('error', basicSwalOptions, null);
            break
    }
}

/**
 * Name: Get_Repo_Bias_Request_Handler
 * Description: Method to display potential bias options for the user
 * @param {any} result Bias information from the API
 */
function Get_Repo_Bias_Request_Handler(result) {
    $('#request-loader-container').slideUp();

    // If no bias GitHub, Addition or Deletion commits were identified
    if (result.gitHub_Commits.length == 0 && result.mass_Addition_Commits.length == 0 && result.mass_Deletion_Commits.length == 0 && result.has_Commits) {
        // Create options for the Swal
        let noBiasBasicSwalOptions = {
            use: true,
            title: 'No Bias Found',
            text: 'The selected repository does not appear to have any bias commits',
            type: 'success'
        };

        // Inform the user that there was no bias identified
        Display_Sweet_Alert('success', noBiasBasicSwalOptions, null);

        // Inform the user stats are loading
        $('#request-loader-text').text('Loading repository stats...');
        $('#request-loader-container').slideDown();

        // Get the information required for stats request
        let username = $('#request-username-input').val();
        let repositoryName = $('#request-repository-select').val();
        let addThreshold = $('#request-add-threshold-input').val();
        let delThreshold = $('#request-del-threshold-input').val();
        let commitThreshold = $('#request-commit-threshold-input').val();

        // Create the request object
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

        // Submit the statistics request
        Submit_AJAX_POST_Request(requestEndpointContainer.getRepoStats, filteredRequestObj, Get_Repo_Stats_Handler);
    }
    // If any bias commits were found
    else if (result.gitHub_Commits.length > 0 || result.mass_Addition_Commits.length > 0 || result.mass_Deletion_Commits.length > 0 && result.has_Commits) {
        // Destroy the existing Bootstrap tables to avoid any carry over data 
        $('#bias-github-commit-table').bootstrapTable('destroy');
        $('#bias-addition-commit-table').bootstrapTable('destroy');
        $('#bias-deletion-commit-table').bootstrapTable('destroy');

        // Fade out GitHub display
        $('#bias-github-commit-display').fadeOut(300);
        $('#bias-github-commit-display').removeClass('visible');

        // Fade out addition display
        $('#bias-addition-commit-display').fadeOut(300);
        $('#bias-addition-commit-display').removeClass('visible');

        // Fade out deletion display
        $('#bias-deletion-commit-display').fadeOut(300);
        $('#bias-deletion-commit-display').removeClass('visible');

        // If any bias GitHub commits were found
        if (result.gitHub_Commits.length > 0) {
            let githubTableData = [];

            // Add the commit details to the bias GitHub object
            $.each(result.gitHub_Commits, function (key, val) {
                githubTableData.push({
                    id: val.sha,
                    author: val.author.login,
                    message: val.commit.message,
                    additions: val.stats.additions,
                    deletions: val.stats.deletions
                });
            })

            // Re-initialise the GitHub bias table
            $('#bias-github-commit-table').bootstrapTable({
                data: githubTableData
            });

            $('#bias-github-commit-table').bootstrapTable('hideColumn', 'id');

            // Show the GitHub bias table
            $('#bias-github-commit-display').fadeIn(300);
            $('#bias-github-commit-display').addClass('visible');
        }

        // If any bias addition commits were found
        if (result.mass_Addition_Commits.length > 0) {
            let additionTableData = [];

            // Add the commit details to the bias addition object
            $.each(result.mass_Addition_Commits, function (key, val) {
                additionTableData.push({
                    id: val.sha,
                    author: val.author.login,
                    message: val.commit.message,
                    additions: val.stats.additions,
                    deletions: val.stats.deletions
                });
            })

            // Re-initialise the addition bias table
            $('#bias-addition-commit-table').bootstrapTable({
                data: additionTableData
            });

            $('#bias-addition-commit-table').bootstrapTable('hideColumn', 'id');

            // Show the addition bias table
            $('#bias-addition-commit-display').fadeIn(300);
            $('#bias-addition-commit-display').addClass('visible');
        }

        // If any bias deletion commits were found
        if (result.mass_Deletion_Commits.length > 0) {
            let deletionTableData = [];

            // Add the commit details to the bias deletion object
            $.each(result.mass_Deletion_Commits, function (key, val) {
                deletionTableData.push({
                    id: val.sha,
                    author: val.author.login,
                    message: val.commit.message,
                    additions: val.stats.additions,
                    deletions: val.stats.deletions
                });
            })

            // Re-initialise the deletion bias table
            $('#bias-deletion-commit-table').bootstrapTable({
                data: deletionTableData
            });

            $('#bias-deletion-commit-table').bootstrapTable('hideColumn', 'id');

            // Show the deletion bias table
            $('#bias-deletion-commit-display').fadeIn(300);
            $('#bias-deletion-commit-display').addClass('visible');
        }

        // Display the bias options modal
        $('#repo-bias-modal').modal('show');
    }
    // If no commits were found at all
    else if (result.has_Commits == false) {
        // Create the Swal options
        let basicNoCommitSwalOptions = {
            use: true,
            type: 'warning',
            title: 'No commits found in range',
            text: 'No commits found in the specified date range. Analysis can\'t be performed'
        };

        // Warn the user that the no commits were identified
        Display_Sweet_Alert('warning', basicNoCommitSwalOptions, null);
    }
}

/**
 * Name: Get_Initial_Commit_Handler
 * Description: Method to warn the user about potential bias from initial commits
 * @param {any} result Information initial commit bias
 */
function Get_Initial_Commit_Handler(result) {
    $('#request-loader-container').slideUp();

    // Destroy the initial commit table to avoid any carry over data 
    $('#initial-commit-table').bootstrapTable('destroy');

    // If any initial commits were found
    if (result.length > 0) {
        let initCommitData = [];

        // Add the commit details to the initial commit bias object
        $.each(result, function (key, val) {
            initCommitData.push({
                id: val.sha,
                author: val.author.login,
                message: val.commit.message,
                additions: val.stats.additions,
                deletions: val.stats.deletions
            });
        });

        // Re-initialise the bootstrap table to display the initial commits
        $('#initial-commit-table').bootstrapTable({
            data: initCommitData
        });

        $('#initial-commit-table').bootstrapTable('hideColumn', 'id');

        // Display the initial commit bias modal
        $('#initial-commit-modal').modal('show');
    }
    // If no commits were found
    else {
        // Initialise the Swal options
        let noCommitSwalOptions = {
            use: true,
            type: 'warning',
            title: 'No commits found',
            text: 'No commits could be found for the target repository. Statistics are unavailable'
        };

        // Warn the user that no commits were found
        Display_Sweet_Alert('warning', noCommitSwalOptions, null);
    }
}


/**
 * Name: Get_Repo_Stats_Handler
 * Description: Method to handle the results of a stats request
 * @param {any} result Collaborator stats information
 */
function Get_Repo_Stats_Handler(result) {
    // Populate the global stats variable
    stats = result.stats;

    let chartData = [];

    // Fade out existing results
    $('#all-request-results-container').fadeOut(300);

    $('#all-request-results').empty(300);

    // Create an object to track the totals of the repository
    let repoInfo = {
        Commit_Total: 0,
        Addition_Total: 0,
        Deletion_Total: 0
    };

    // Iterate through each collaborator's stats and add the values to the overall repo object
    $.each(result.stats, function (key, val) {
        repoInfo.Commit_Total += val.total;
        repoInfo.Addition_Total += val.additions;
        repoInfo.Deletion_Total += val.deletions;
    });

    // Iterate through each collaborator statistics
    $.each(result.stats, function (key, val) {
        // Track the class that should be used for the collaborator's display
        let borderClass = 'no-border';

        // If the collaborator's commit total is below the user's threshold, they should have a red border
        if (val.below_Threshold) {
            borderClass = 'warn-border';
        }

        // Create dynamic HTML that will display each user's result
        let authorHTML = '<div class="col-4">' +
            '<div class="hover-container ' + borderClass + '">' +
            '<div class="row">' +
            '<div class="col-6 text-center"><img src="' + val.author.avatar_Url + '" style="height:50px; width:50px;"/></div>' +
            '<div class="col-6 text-center"><span class="contribution-text" id="' + val.author.id + '-score"></span></div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="col-12 text-center"><span>' + val.author.login + '</span></div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="col-12">' +
            '<table class="table-striped w-100" style="padding:10px">' +
            '<thead>' +
            '<tr>' +
            '<th>Item</th>' +
            '<th>Value</th>' +
            '</tr>' +
            '</thead>' +
            '<tbody>' +
            '<tr><td>Total Commits</td><td class="primary-text">' + val.total + '</td></tr>' +
            '<tr><td>Total Additions</td><td class="secondary-text">' + val.additions + '</td></tr>' +
            '<tr><td>Total Deletions</td><td class="warn-text">' + val.deletions + '</td></tr>' +
            '</tbody>' +
            '</table>' +
            '<div class="row">' +
            '<div class="col-12 text-center">' +
            '<button class="btn secondary-btn" onclick="Load_Collaborator_Overview(' + '\'' + val.author.id + '\'' + ')">Details</button>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';

        // Add the dynamic HTMl to the display
        $('#all-request-results').append(authorHTML);

        // Create an object for a collaborator's contribution request
        let contributionRequestObj = {
            Author_Id: val.author.id,
            Repo: repoInfo,
            User: {
                Commit_Total: val.total,
                Addition_Total: val.additions,
                Deletion_Total: val.deletions
            },
            Author_Total: result.stats.length
        };

        // Submit the collaborator's contribution request
        Submit_AJAX_POST_Request(requestEndpointContainer.getContributionScore, contributionRequestObj, Get_Contribution_Score_Handler);
    });

    // Orders the collaborators by total commits
    let orderedStats = stats.sort((a, b) => (a.total < b.total) ? 1 : ((b.total < a.total) ? -1 : 0));

    // Create variables to track commit data
    let commitChartData = [];
    let commitTableData = '';

    // Iterate through the ordered statistics
    $.each(orderedStats, function (key, val) {
        // Add the commit data to the item chart
        commitChartData.push([val.author.login, val.total, val.author.login]);

        // Add the commit data ot the table
        commitTableData += '<tr><td>' + (key + 1) + '</td><td>' + val.author.login + '</td><td>' + val.total + '</td></tr>';
    });

    // Create a Highcharts item chart with the commit data
    Highcharts.chart('result-commit-stat-item-chart', {

        chart: {
            type: 'item'
        },

        title: {
            text: 'Collaborator Commit Visualisation'
        },

        subtitle: {
            text: ''
        },

        legend: {
            labelFormat: '{name} <span style="opacity: 0.4">{y}</span>'
        },

        series: [{
            name: 'Commits',
            keys: ['name', 'y', 'label'],
            data: commitChartData,
            dataLabels: {
                enabled: true,
                format: '{point.label}'
            },

            // Circular options
            center: ['50%', '88%'],
            size: '170%',
            startAngle: -100,
            endAngle: 100
        }]
    });

    // Empty the statistics table, and add the new commit data
    $('#result-commit-stat-table').empty().promise().done(function () {
        $('#result-commit-stat-table').append(commitTableData);
    })

    // Show the results
    $('#all-request-results-container').fadeIn(300).promise().done(function () {
        $('#request-loader-container').slideUp();
    });
}

/**
 * Name: Get_Contribution_Score_Handler
 * Description: Method to display a collaborator's contribution score
 * @param {any} result Contribution score result
 */
function Get_Contribution_Score_Handler(result) {
    // Add the score to the global object
    scores.push(result);

    // Display the socre to the display
    $('#' + result.author_Id + '-score').text(result.score.contribution_Score.toFixed(2));
}

//! END Section: Functions