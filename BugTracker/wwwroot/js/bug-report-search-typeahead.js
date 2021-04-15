
$(document).ready(function () {

    // REQUIRES an element with id=project-id to hold the current project ID
    var projectId = document.getElementById("project-id").value;

    var reportSuggestions = new Bloodhound({

        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/Search/GetBugReports?query=%QUERY%&projectId=' + projectId,
            wildcard: "%QUERY%",
            filter: function (reportSuggestions) {
                return $.map(reportSuggestions, function (suggestion) {
                    return {
                        report_id: suggestion.bugReportId,
                        report_localId: suggestion.localBugReportId,
                        report_link: suggestion.url,
                        report_title: suggestion.title
                    }
                })
            },
        }
    });

    reportSuggestions.initialize();

    $('#Search').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    },
        {
            name: "reports",
            source: reportSuggestions.ttAdapter(),
            displayKey: 'report_title',
            templates: {
                suggestion: function (data) {
                    return `
                    <div>
                        <strong>#` + data.report_localId + `</strong>
                        <p>` + data.report_title + `</p>
                    </div>
                `
                }
            }
        });

    $('#Search').bind('typeahead:select', function (ev, suggestion) {

        var bugReportIdInput = document.getElementById('bug-report-id');
        if (bugReportIdInput !== null) {
            bugReportIdInput.value = suggestion.report_id;
        }

        var bugReportLink = document.getElementById('bug-report-link');
        if (bugReportLink !== null) {
            bugReportLink.value = suggestion.report_link;
        }

        var bugReportTitle = document.getElementById('bug-report-title');
        if (bugReportTitle !== null) {
            bugReportTitle.value = suggestion.report_title;
        }

        document.getElementById('local-bug-report-id').value = suggestion.report_localId;
    });

});