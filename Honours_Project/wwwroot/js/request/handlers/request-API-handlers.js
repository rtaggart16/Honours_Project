let stats = [];

let scores = [];

let restrictedCommits = [];

function Get_User_Repo_Request_Handler(result) {
    if (result.status.status_Code == 200) {
        if (result.repos.length > 0) {
            $('#request-repository-select').empty();

            $.each(result.repos, function (key, val) {
                $('#request-repository-select').append('<option value="' + val.name + '">' + val.name + '</option>');
            })

            $('#repos-loading-icon').fadeOut(300).promise().done(function () {
                $('#repos-fetched-icon').fadeIn(300);
                $('#repos-fetched-icon').addClass('visible');
            })
        }
    }
}

function Get_Repo_Bias_Request_Handler(result) {
    $('#request-loader-container').slideUp();

    if (result.gitHub_Commits.length == 0 && result.mass_Addition_Commits.length == 0 && result.mass_Deletion_Commits.length == 0 && result.has_Commits) {
        let noBiasBasicSwalOptions = {
            use: true,
            title: 'No Bias Found',
            text: 'The selected repository does not appear to have any bias commits',
            type: 'success'
        };

        Display_Sweet_Alert('success', noBiasBasicSwalOptions, null);

        $('#request-loader-text').text('Loading repository stats...');
        $('#request-loader-container').slideDown();

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
            Restricted_Commits: []
        };

        Submit_AJAX_POST_Request(requestEndpointContainer.getRepoStats, filteredRequestObj, Get_Repo_Stats_Handler);
    }
    else if (result.gitHub_Commits.length > 0 || result.mass_Addition_Commits.length > 0 || result.mass_Deletion_Commits.length > 0 && result.has_Commits) {
        $('#bias-github-commit-table').bootstrapTable('destroy');
        $('#bias-addition-commit-table').bootstrapTable('destroy');
        $('#bias-deletion-commit-table').bootstrapTable('destroy');

        $('#bias-github-commit-display').fadeOut(300);
        $('#bias-github-commit-display').removeClass('visible');

        $('#bias-addition-commit-display').fadeOut(300);
        $('#bias-addition-commit-display').removeClass('visible');

        $('#bias-deletion-commit-display').fadeOut(300);
        $('#bias-deletion-commit-display').removeClass('visible');

        if (result.gitHub_Commits.length > 0) {
            let githubTableData = [];

            $.each(result.gitHub_Commits, function (key, val) {
                githubTableData.push({
                    id: val.sha,
                    author: val.author.login,
                    message: val.commit.message,
                    additions: val.stats.additions,
                    deletions: val.stats.deletions
                });
            })

            $('#bias-github-commit-table').bootstrapTable({
                data: githubTableData
            });

            $('#bias-github-commit-table').bootstrapTable('hideColumn', 'id');

            $('#bias-github-commit-display').fadeIn(300);
            $('#bias-github-commit-display').addClass('visible');
        }

        if (result.mass_Addition_Commits.length > 0) {
            let additionTableData = [];

            $.each(result.mass_Addition_Commits, function (key, val) {
                additionTableData.push({
                    id: val.sha,
                    author: val.author.login,
                    message: val.commit.message,
                    additions: val.stats.additions,
                    deletions: val.stats.deletions
                });
            })

            console.log(additionTableData);

            $('#bias-addition-commit-table').bootstrapTable({
                data: additionTableData
            });

            $('#bias-addition-commit-table').bootstrapTable('hideColumn', 'id');

            $('#bias-addition-commit-display').fadeIn(300);
            $('#bias-addition-commit-display').addClass('visible');
        }

        if (result.mass_Deletion_Commits.length > 0) {
            let deletionTableData = [];

            $.each(result.mass_Deletion_Commits, function (key, val) {
                deletionTableData.push({
                    id: val.sha,
                    author: val.author.login,
                    message: val.commit.message,
                    additions: val.stats.additions,
                    deletions: val.stats.deletions
                });
            })

            $('#bias-deletion-commit-table').bootstrapTable({
                data: deletionTableData
            });

            $('#bias-deletion-commit-table').bootstrapTable('hideColumn', 'id');

            $('#bias-deletion-commit-display').fadeIn(300);
            $('#bias-deletion-commit-display').addClass('visible');
        }

        $('#repo-bias-modal').modal('show');
    }
    else if (result.has_Commits == false) {
        let basicNoCommitSwalOptions = {
            use: true,
            type: 'warning',
            title: 'No commits found in range',
            text: 'No commits found in the specified date range. Analysis can\'t be performed'
        };

        Display_Sweet_Alert('warning', basicNoCommitSwalOptions, null);
    }
}

function Get_Initial_Commit_Handler(result) {
    $('#request-loader-container').slideUp();

    $('#initial-commit-table').bootstrapTable('destroy');

    if (result.length > 0) {
        let initCommitData = [];

        $.each(result, function (key, val) {
            initCommitData.push({
                id: val.sha,
                author: val.author.login,
                message: val.commit.message,
                additions: val.stats.additions,
                deletions: val.stats.deletions
            });
        });

        $('#initial-commit-table').bootstrapTable({
            data: initCommitData
        });

        $('#initial-commit-table').bootstrapTable('hideColumn', 'id');

        $('#initial-commit-modal').modal('show');
    }
    else {
        let noCommitSwalOptions = {
            use: true,
            type: 'warning',
            title: 'No commits found',
            text: 'No commits could be found for the target repository. Statistics are unavailable'
        };

        Display_Sweet_Alert('warning', noCommitSwalOptions, null);
    }
}

function Get_Repo_Stats_Handler(result) {
    stats = result.stats;

    let chartData = [];

    $('#all-request-results-container').fadeOut(300);

    $('#all-request-results').empty(300);

    let repoInfo = {
        Commit_Total: 0,
        Addition_Total: 0,
        Deletion_Total: 0
    };

    $.each(result.stats, function (key, val) {
        repoInfo.Commit_Total += val.total;
        repoInfo.Addition_Total += val.additions;
        repoInfo.Deletion_Total += val.deletions;
    });

    $.each(result.stats, function (key, val) {
        let borderClass = 'no-border';

        if (val.below_Threshold) {
            borderClass = 'warn-border';
        }

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

        $('#all-request-results').append(authorHTML);

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

        Submit_AJAX_POST_Request(requestEndpointContainer.getContributionScore, contributionRequestObj, Get_Contribution_Score_Handler);
    });

    let orderedStats = stats.sort((a, b) => (a.total < b.total) ? 1 : ((b.total < a.total) ? -1 : 0));

    let commitChartData = [];
    let commitTableData = '';

    $.each(orderedStats, function (key, val) {
        commitChartData.push([val.author.login, val.total, val.author.login]);

        commitTableData += '<tr><td>' + (key + 1) + '</td><td>' + val.author.login + '</td><td>' + val.total + '</td></tr>';
    });

    console.log(commitChartData);

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

    $('#result-commit-stat-table').empty().promise().done(function () {
        $('#result-commit-stat-table').append(commitTableData);
    })

    $('#all-request-results-container').fadeIn(300).promise().done(function () {
        $('#request-loader-container').slideUp();
    });
}

function Get_Contribution_Score_Handler(result) {
    scores.push(result);
    $('#' + result.author_Id + '-score').text(result.score.contribution_Score.toFixed(2));
}