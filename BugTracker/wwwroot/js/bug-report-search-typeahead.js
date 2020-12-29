
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
                        report_localId: suggestion.localBugReportId,
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
            displayKey: 'report_localId',
            templates: {
                suggestion: function (data) {
                    return `
                    <div>
                        <strong>` + data.report_localId + `</strong>
                        <p>` + data.report_title + `</p>
                    </div>
                `
                }
            }
        });

});