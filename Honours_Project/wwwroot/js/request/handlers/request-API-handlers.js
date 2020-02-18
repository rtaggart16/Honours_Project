let stats = [];

let scores = [];

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

    if (result.gitHub_Commits.length == 0 && result.mass_Addition_Commits.length == 0 && result.mass_Deletion_Commits.length == 0) {
        let noBiasBasicSwalOptions = {
            use: true,
            title: 'No Bias Found',
            text: 'The selected repository does not appear to have any bias commits',
            type: 'success'
        };

        Display_Sweet_Alert('success', noBiasBasicSwalOptions, null);
    }
    else {
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
        let authorHTML = '<div class="col-4">' +
            '<div class="hover-container">' +
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
            '<button class="btn btn-info" onclick="Load_Collaborator_Overview(' + '\'' + val.author.id + '\'' + ')">View</button>' +
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
    })

    $('#all-request-results-container').fadeIn(300).promise().done(function () {
        $('#request-loader-container').slideUp();
    });
}

function Get_Contribution_Score_Handler(result) {
    scores.push(result);
    $('#' + result.author_Id + '-score').text(result.score.contribution_Score.toFixed(2));
}