// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#show_hide_password a").on('click', function (event) {
        event.preventDefault();
        if ($('#show_hide_password input').attr("type") == "text") {
            $('#show_hide_password input').attr('type', 'password');
            $('#show_hide_password i').addClass("fa-eye-slash");
            $('#show_hide_password i').removeClass("fa-eye");
        } else if ($('#show_hide_password input').attr("type") == "password") {
            $('#show_hide_password input').attr('type', 'text');
            $('#show_hide_password i').removeClass("fa-eye-slash");
            $('#show_hide_password i').addClass("fa-eye");
        }
    });
});

$(function () {

    //// Get the current date
    //var currentDate = new Date();

    //// Calculate the date one month ago
    //currentDate.setMonth(currentDate.getMonth() - 1);

    //// Format the date as yyyy-mm-dd
    //var formattedDate = currentDate.toISOString().slice(0, 10);

    //// Set the value of the input field
    //document.getElementById("beginDate").value = formattedDate;


    //// Get the current date
    //var currentDate = new Date();

    //// Calculate the date one month ago
    //currentDate.setMonth(currentDate.getMonth() + 1);

    //// Format the date as yyyy-mm-dd
    //var formattedDate = currentDate.toISOString().slice(0, 10);

    //// Set the value of the input field
    //document.getElementById("endDate").value = formattedDate;

    const svgIcon = $("#svg-icon");
    const openSetting = $("#open-setting");

    const dialog = $("#dialog");
    const closeDialogBtn = $("#close-dialog");

    const mailDialog = $("#settingMail");
    const closeMailSettingBtn = $("#close-mailsetting");

    loadSetting();
    $("#save-address").on("click", function () {
        var newData = {
            geolocation: $("#geolocationCheck").prop("checked"), // Get the checkbox value
            scheduleList: []
        };

        // Loop through each list item to gather input values
        $("#scheduleList li").each(function () {
            var address = $(this).find(".form-control").eq(0).val();
            var timelist = $(this).find(".form-control.timepicker").val();
            newData.scheduleList.push({ addresslist: address, timelist: timelist });
        });


        console.log("Data saved:", newData);

        // Make an AJAX POST request to save the data
        $.ajax({
            url: "api/worktime/SaveSchedule", // Replace with your save API endpoint
            type: "POST",
            data: JSON.stringify(newData),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                console.log("Data saved:", response);
            },
            error: function (xhr, status, error) {
                console.error("Error saving data:", error);
            }
        });

        loadSetting();
        mailDialog.addClass("hidden");
    });

    function loadSetting() {
        $.get('api/worktime/geolocation', function (data) {

            var showGeoloc = data; // Assuming the API returns true or false

            $('#geolocationCheck').prop('checked', showGeoloc);
            // Show or hide the navigation item based on the API response
            if (showGeoloc) {
                $('#geolocNavItem').show();
            } else {
                $('#geolocNavItem').hide();
            }
        });

        $.ajax({
            url: "api/worktime/Setting", // Replace with your actual API endpoint URL
            type: "GET",
            dataType: "json",
            success: function (data) {

                // Clear the previous list content
                $("#scheduleList").empty();

                var count = 0;
                // Loop through the data and add each item to the list

                function createListItem() {
                    // Create new input fields
                    var newSettingInput = $("<input>").attr({
                        class: "form-control",
                        placeholder: "Destinataire(s), mail séparés par “,”",
                        style: "width:800%"
                    });

                    var newTimePickerInput = $("<input>").attr({
                        class: "form-control timepicker",
                    });

                    newTimePickerInput.timepicker({
                        timeFormat: 'H:mm',
                        interval: 15,
                        minTime: '00:00',
                        maxTime: '23:59',
                        startTime: '00:00',
                        dynamic: false,
                        dropdown: true,
                        scrollbar: true
                    });

                    var newAddButton = $("<button>").attr({
                        type: 'button',
                        style: 'color:red;background-color:transparent;border-color:transparent'
                    }).text("+");

                    var newSubButton = $("<button>").attr({
                        type: 'button',
                        style: 'color:red;background-color:transparent;border-color:transparent'
                    }).text("-");

                    var newListItem = $("<li style='display:flex;align-items:center'>");
                    newListItem.append("&nbsp;Address:&nbsp;").append(newSettingInput);
                    newListItem.append("&nbsp;Time:&nbsp;").append(newTimePickerInput);

                    newListItem.append(newAddButton);
                    newListItem.append(newSubButton);

                    // Append the new list item to the list
                    $("#scheduleList").append(newListItem);

                    newSubButton.on("click", function () {
                        newListItem.remove(); // Remove the current listItem
                    });

                    newAddButton.on("click", function () {
                        createListItem(); // Create a new list item when the "+" button is clicked
                    });
                }

                if (data.scheduleList.length == 0) {
                    createListItem();
                }
                else {
                    data.scheduleList.forEach(function (schedule) {

                        ++count;
                        // Create elements for settingInput and timePickerInput
                        var settingInput = $("<input>").attr({
                            class: "form-control",
                            value: schedule.addresslist,
                            placeholder: "Destinataire(s), mail séparés par “,”",
                            style: "width:800%"
                        });

                        var timePickerInput = $("<input>").attr({
                            class: "form-control timepicker",
                            value: schedule.timelist
                        });

                        var addButton = $("<button>").attr({
                            type: 'button',
                            style: 'color:red;background-color:transparent;border-color:transparent'
                        }).text("+");

                        var subButton = $("<button>").attr({
                            type: 'button',
                            style: 'color:red;background-color:transparent;border-color:transparent'
                        }).text("-");

                        subButton.on("click", function () {
                            listItem.remove(); // Remove the current listItem
                        });

                        // Initialize the timepicker for the dynamically created input
                        timePickerInput.timepicker({
                            timeFormat: 'H:mm',
                            interval: 15,
                            minTime: '00:00',
                            maxTime: '23:59',
                            startTime: '00:00',
                            defaultTime: schedule.timelist, // Set the default time
                            dynamic: false,
                            dropdown: true,
                            scrollbar: true
                        });

                        var listItem = $("<li style='display:flex;align-items:center'>");
                        listItem.append("&nbsp;Address:&nbsp;").append(settingInput);
                        listItem.append("&nbsp;Time:&nbsp;").append(timePickerInput);

                        listItem.append(addButton);
                        listItem.append(subButton);

                        // Append the list item to the list
                        $("#scheduleList").append(listItem);

                        addButton.on("click", createListItem);

                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }

    const settingInput = $('#settingInput');
    const closeMailSettingButton = $('#close-mailsetting');

    closeMailSettingButton.on('click', function () {
        // Close the setting dialog
        loadSetting();
        $('#settingMail').addClass('hidden');
    });


    const saveAddressButton = $('#save-address');

    setInterval(updateTime, 1000); // update every second

    $("input").attr("autocomplete", "off");
    $("select").attr("autocomplete", "off");
    $('[data-toggle="tooltip"]').tooltip({
        html: true,
        template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner" style="background-color: white;"></div></div>'
    });

    $("#Code").on("input", function () {
        var input = $(this).val();
        if (input.length > 5) {
            $(this).val(input.slice(0, 5));
        }
    });

    function updateTime() {
        var now = new Date();
        var formattedTime = ("0" + now.getDate()).slice(-2) + '/' + ("0" + (now.getMonth() + 1)).slice(-2) + ' : ' + ("0" + now.getHours()).slice(-2) + ':' + ("0" + now.getMinutes()).slice(-2) + ':' + ("0" + now.getSeconds()).slice(-2);
        var element = document.getElementById('liveTime');

        if (element !== null) {
            document.getElementById('liveTime').innerText = formattedTime;
            element.innerText = formattedTime;
        } else {
            console.log('Element does not exist');
        }
    }

    $('[data-toggle="copytooltip"]').click(function () {
        var tooltip = $(this);
        // Create tooltip
        tooltip.tooltip({
            html: true,
            template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner" style="background-color: white;"></div></div>',
            trigger: 'manual'
        });
        // Toggle tooltip
        tooltip.tooltip('toggle');
        // Remove tooltip after 2 seconds
        setTimeout(function () {
            tooltip.tooltip('hide');
        }, 1600);
    });

    $('.copy').on('click', function () {
        var value = $(this).attr('value');

        var content = $('#' + value).text().trim();

        navigator.clipboard.writeText(content)
            .then(function () {
                console.log('Text copied to clipboard successfully.');
            })
            .catch(function (error) {
                console.error('Unable to copy text to clipboard:', error);
            });
    });

    var currentTime = new Date();
    var hours = currentTime.getHours();
    var minutes = currentTime.getMinutes();

    // Format the time as HH:mm
    var formattedTime = (hours < 10 ? '0' : '') + hours + ':' + (minutes < 10 ? '0' : '') + minutes;

    // Set the value of the input field
    $('.timepicker').val(formattedTime);

    // Initialize the timepicker
    $('.timepicker').timepicker({
        timeFormat: 'H:mm',
        interval: 15,
        minTime: '00:00',
        maxTime: '23:59',
        startTime: '00:00',
        defaultTime: new Date(),
        dynamic: false,
        dropdown: true,
        scrollbar: true
    });

    svgIcon.on("click", function () {
        dialog.removeClass("hidden");
    });

    openSetting.on("click", function () {
        loadSetting();
        mailDialog.removeClass("hidden");
    })

    closeMailSettingBtn.on("click", function () {
        mailDialog.addClass("hidden");
    });

    closeDialogBtn.on("click", function () {
        dialog.addClass("hidden");
    });
});

