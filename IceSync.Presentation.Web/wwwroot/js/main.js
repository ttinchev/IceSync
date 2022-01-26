
(function ($) {
    "use strict";
    $.ajax({
        type: "GET",
        url: "https://localhost:44390/api/v1/Workflows",
        dataType: "json",
        success: function (response) {
            for (var i = 0; i < response.data.length; i++) {
                $(".table").append(
                    "<div class='row'><div class='cell' data-title='Full Name'>" +
                    response.data[i].workflowId + "</div><div class='cell' data-title='Age'>" +
                    response.data[i].workflowName + "</div><div class='cell' data-title='Job Title'>" +
                    response.data[i].isActive + "</div><div class='cell' data-title='Location'>" +
                    response.data[i].isRunning + "</div><div class='cell' data-title='Location'>" +
                    response.data[i].multiExecBehavior + "</div><div class='cell' data-title='Location'>" +
                    "<button type='button' class='button' value='" + response.data[i].workflowId + "'>Run</button></div></div> ");
            };

            $(".button").on("click", function () {
                $.ajax({
                    type: "GET",
                    url: "https://localhost:44390/api/v1/Workflows/" + $(this).val() + "/run",
                    success: function (result) {
                        alert('success');
                    },
                    error: function (result) {
                        alert(result.responseJSON.messages[0]);
                    }
                });
            });
        }
    });
})(jQuery);

