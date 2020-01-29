$(document).ready(function (e) {
    var userObject = new Object({
        minimumInputLength: 3,
        allowClear: true,
        language: {
            inputTooShort: function () {
                return 'Minimum 3 simvol daxil edin';
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


    var dep_array = [];

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

    var reports = new Object({
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
            url: '/Reports/GetReports',
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
                        'text': item.name,
                    });

                    $('#select_filter').attr('data-info', item.id);

                });
                return {
                    results: myResults
                };
            }

        }
    });

    $('.select2c').select2({
        allowClear: true,
        placeholder: "Seçim edin"
    });


    $('#mlDepartment').select2(departments);
    $('#responsibleUser').select2(userObject);
    $('#identifierUser').select2(userObject);
    $('#mlFollowerUser').select2(userObject);
    $('#select_filter').select2(reports);

    var page = 1;
    var fd = new FormData();


    function ajaxReportMl() {
        //var isValid = checkValid('reportForm');
        //if (isValid) {
            $('#modal-1').modal('show', { backdrop: 'static' });
            setTimeout(function (e) {
                $.ajax({
                    url: '/Reports/MLReport',
                    method: 'post',
                    contentType: false,
                    processData: false,
                    data: fd,
                    cache: false,
                    success: function (result) {
                        $('#report_partial').remove();
                        $('#ml_show_partial').remove();

                        $('#report_page').append(result);

                        setTimeout(function () {
                            $('#modal-1').modal('hide');
                        }, 500)
                    }
                });

            }, 500);
        //}
    }

    function Pagination(parameter_page) {
        page = parameter_page;
        fd.set('page', parseInt(page));
        ajaxReportMl();
    }

    $(document).on('click', '.move_page', function (e) {
        page = $(this).data('page');
        Pagination(page)
    });

    $(document).on('click', '.move_back', function (e) {
        page = parseInt($(this).data('page')) - 1;
        Pagination(page);
    })

    $(document).on('click', '.move_next', function (e) {
        page = parseInt($(this).data('page')) + 1;
        Pagination(page);
    });

    $(document).on('click', '.meeting_line_c', function () {
        var val = $(this).data('id');
        $('#ml_show_partial').remove();

        $.ajax({
            type: 'get',
            url: '/MeetingLine/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $('#report_page').append(result);
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('#ml_show_partial').position().top }, 500, 'swing');
            }
        });
    });

    $(document).on('click', '#show_close a', function (e) {
        var body = $("html, body");
        body.stop().animate({ scrollTop: $('#report_partial').position().top }, 500, 'swing');
        $('#ml_show_partial').remove();
    });

    function fdataAppend() {
        fd = new FormData();

        fd.set('page', parseInt(page));
        fd.set('meetingType', parseInt($('#meeting_type').val()));
        fd.set('mlType', parseInt($('#ml_type').val()));
        if ($('#mlDepartment').val() != null) {
            $.each($('#mlDepartment').val(), function (index, item) {
                fd.append('department', parseInt(item));
            });
            dep_array = $('#mlDepartment').val();
        }
        else
            fd.set('department', '');


        if ($('#responsibleUser').val() != null)
            fd.set('responsibleEmail', $('#responsibleUser').val());
        else
            fd.set('responsibleEmail', "");

        if ($('#mlFollowerUser').val() != null)
            fd.set('followerEmail', $('#mlFollowerUser').val());
        else
            fd.set('followerEmail', "");

        if ($('#identifierUser').val() != null)
            fd.set('confirmedEmail', $('#identifierUser').val());
        else
            fd.set('confirmedEmail', "");


        fd.set('status', parseInt($('#mlStatus').val()));
        fd.set('cStatus', parseInt($('#mlStatusSec').val()));

    }

    $(document).on('click', '.report_search', function (e) {
        page = 1;

        fdataAppend();
        ajaxReportMl();
    });

    $(document).on('click', '.select_filter', function (e) {
        $('#modal-11').modal('show', { backdrop: 'static' });
    });

    $(document).on('click', '#modal-11 .btn-info', function (e) {
        var fr = $('#select_filter').data('info');
        $('#modal-1').modal('show', { backdrop: 'static' });
        $('#modal-11').modal('hide');
        setTimeout(function (e) {
            $.ajax({
                url: '/Reports/MeetingLineFull/' + fr,
                method: 'get',
                contentType: false,
                processData: false,
                cache: false,
                success: function (res) {
                    $('#report_page').html('');
                    $('#report_page').append(res);

                    $('.select2c').select2({
                        allowClear: true,
                        placeholder: "Seçim edin"
                    });
                    $('#mlDepartment').select2(departments);
                    $('#responsibleUser').select2(userObject);
                    $('#identifierUser').select2(userObject);
                    $('#mlFollowerUser').select2(userObject);
                    $('#select_filter').select2(reports);
                    $('#modal-1').modal('hide');
                }
            });
        }, 500);
    });


    $(document).on('click', '.save_filter', function (e) {

        $('#modal-12').modal('show', { backdrop: 'static' });


    });

    $(document).on('click', '#modal-12 .btn-info', function (e) {
        fdataAppend();

        var val = "";

        val += $('#meeting_type').val() + "-";
        val += $('#ml_type').val() + "-";
        val += $('#mlDepartment').val() != null ? $('#mlDepartment').val().join('+') + "-" : "-";
        val += $('#responsibleUser').val() != null ? $('#responsibleUser').val() + "-" : "-";
        val += $('#mlFollowerUser').val() != null ? $('#mlFollowerUser').val() + "-" : "-";
        val += $('#identifierUser').val() != null ? $('#identifierUser').val() + "-" : "-";
        val += $('#mlStatus').val() + "-";
        val += $('#mlStatusSec').val();

        $('#modal-1').modal('show', { backdrop: 'static' });
        $('#modal-12').modal('hide');
        setTimeout(function (e) {
            var form_data = new FormData();
            form_data.append('info', val);
            form_data.append('name', $('#filter_name_save').val());

            $.ajax({
                url: '/Reports/ReportSave',
                method: 'post',
                contentType: false,
                processData: false,
                data: form_data,
                cache: false,
                success: function () {
                    $('#filter_name_save').val('');
                    $('#modal-1').modal('hide');
                }
            });

        }, 500);
    });

    function getCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    function checkValid(className) {
        var checkVal = false;

        for (var i = 0; i < $('.' + className + ' .select2c:required').length; i++) {
            if ($('.' + className + ' .select2c:required')[i].checkValidity()) {
                checkVal = true;
                break;
            }
        }

        return checkVal;
    }



});