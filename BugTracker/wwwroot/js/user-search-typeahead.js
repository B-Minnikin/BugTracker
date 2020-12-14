
$(document).ready(function () {

    var userSuggestions = new Bloodhound({

        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: 'ActionMethod?query=%QUERY%',
            wildcard: "%QUERY%",
            filter: function (userSuggestions) {
                return $.map(userSuggestions, function (suggestion) {
                    return {
                        user_email: suggestion.email,
                        user_name: suggestion.name
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