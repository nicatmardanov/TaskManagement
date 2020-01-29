$(document).ready(function (e) {
    $('.select2c').select2();

    var userObject = new Object({
        minimumInputLength: 1,
        language: {
            inputTooShort: function () {
                return 'Sorğunu daxil edin';
            },
            noResults: function () {
                return "Bu sorğuya uyğun nəticə tapılmadı";
            },
            searching: function () {
                return "Axtarış gedir..."
            }
        },
        placeholder: "Daxil edin",
        ajax: {
            url: '/Users/GetUsers',
            dataType: 'json',
            type: "GET",
            data: function (term) {
                return term;
            },
            processResults: function (data) {
                var myResults = [];
                $.each(data, function (index, item) {
                    myResults.push({
                        'id': item.email,
                        'text': item.full_name
                    });
                });
                return {
                    results: myResults
                };
            }

        }
    });
    var departments = new Object({
        minimumInputLength: 1,
        language: {
            inputTooShort: function () {
                return 'Sorğunu daxil edin';
            },
            noResults: function () {
                return "Bu sorğuya uyğun nəticə tapılmadı";
            },
            searching: function () {
                return "Axtarış gedir..."
            }
        },
        placeholder: "Daxil edin",
        ajax: {
            url: '/Department/GetDepartments',
            dataType: 'json',
            type: "GET",
            data: function (term) {
                return term;
            },
            processResults: function (data) {
                var myResults = [];
                $.each(data, function (index, item) {
                    myResults.push({
                        'id': item.id,
                        'text': item.full_name
                    });
                });
                return {
                    results: myResults
                };
            }

        }
    });

    $('#rvDepartment').select2(departments);
    $('#responsibleUser').select2(userObject);
    $('#identifierUser').select2(userObject);
    $('#rvFollowerUser').select2(userObject);
    $('#informedUserRv').select2(userObject);

    $(document).on('click', '.saveRevision', function (e) {
        var form_data = new FormData();

        form_data.append("meetingLineId", $('#mline_id').val());
        form_data.append("rv_type", $("#rv_type").val());

        $.each($("rvDepartment").val(), function (index, item) {
            form_data.append('department', item);
        });

        form_data.append("tags", $("#rvTags").val());
        form_data.append("description", $("#rv_description").val());
        form_data.append("responsibleUser", $("#responsibleUser").val());
        form_data.append("identifierUser", $("#identifierUser").val());
        form_data.append("rvFollowerUser", $("#rvFollowerUser").val());
        form_data.append("informedUserRv", $("#informedUserRv").val());
        form_data.append("rev_start_date", $("#rev_start_date").val());
        form_data.append("rev_finish_date", $("#rev_finish_date").val());

        if ($("#rvFile")[0].files[0] != undefined)
            form_data.append("rvFile", $("#rvFile")[0].files[0]);


        $.ajax({
            url: '/MeetingLine/Revision',
            method: 'post',
            contentType: false,
            processData: false,
            data: form_data,
            cache: false,
            success: function () {
                window.location.href = "/";
            }
        });


    e.preventDefault();
    e.stopImmediatePropagation();
    return false;
    })


});