
$(document).ready(function () {

    // REQUIRES an element with id=project-id to hold the current project ID
    var projectId = document.getElementById("project-id").value;

    var userSuggestions = new Bloodhound({

        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/Search/GetProjectMembers?query=%QUERY%&projectId=' + projectId,
            wildcard: "%QUERY%",
            filter: function (userSuggestions) {
                return $.map(userSuggestions, function (suggestion) {
                    return {
                        user_email: suggestion.email,
                        user_name: suggestion.userName
                    }
                })
            },
        }
    });

    userSuggestions.initialize();

    $('#Search').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    },
    {
        name: "users",
        source: userSuggestions.ttAdapter(),
        displayKey: 'user_name',
        templates: {
            suggestion: function (data) {
                return `
                    <div>
                        <strong>` + data.user_name + `</strong>
                        <p>` + data.user_email + `</p>
                    </div>
                `
            }
        }
    });

});