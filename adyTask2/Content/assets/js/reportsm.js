$(document).ready(function (e) {

    $('.reports_side_bar').click();



    $('.select2c').select2({
        //allowClear: true,
        placeholder: "Seçim edin"
    });


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

    var places = new Object({
        minimumInputLength: 2,
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
            url: '/Place/GetPlaces',
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

    var userDepartments = new Object({
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
            url: '/Department/GetUsersDepartments',
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
            url: '/Reports/GetMReports',
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

    $('#informedUser').select2(userObject);
    $('#ownerUser').select2(userObject);
    $('#followerUser').select2(userObject);
    $('#meeting_place').select2(places);
    $('#meetingDepartment').select2(userDepartments);
    $('#select_filter').select2(reports);


    var page = 1;
    var fd = new FormData();


    function compareDate(selector1, selector2, type) {
        if (type == 0) {
            var date1_string = $("#" + selector1).val().split("/");
            var date2_string = $("#" + selector2).val().split("/");
        }
        else {
            var date1_string = $("." + selector1).val().split("/");
            var date2_string = $("." + selector2).val().split("/");
        }

        var date1 = new Date(date1_string[1] + "/" + date1_string[0] + "/" + date1_string[2]);
        var date2 = new Date(date2_string[1] + "/" + date2_string[0] + "/" + date2_string[2]);

        if (date2 > date1 || date2 == "Invalid Date")
            return true;

        return false;
    }

    function ajaxReportMl() {
        var isValid = checkValid('reportForm');
        if (isValid) {
            $('#modal-1').modal('show', { backdrop: 'static' });
            setTimeout(function (e) {
                $.ajax({
                    url: '/Reports/MReport',
                    method: 'post',
                    contentType: false,
                    processData: false,
                    data: fd,
                    cache: false,
                    success: function (result) {
                        $('.meeting_operation_line_table').remove();
                        $('#meeting_show_partial').remove();
                        $('#ml_show_partial').remove();


                        $('#report_page').append(result);

                        setTimeout(function () {
                            $('#modal-1').modal('hide');
                        }, 500);
                    }
                });

            }, 500);
        }
    }

    function Pagination(parameter_page) {
        page = parameter_page;
        fd.set('page', parseInt(page));
        ajaxReportMl();
    }

    $(document).on('change', '#meeting_start_date', function (e) {
        $($($($('#meeting_start_date').parent()).parent()).find('.invalid-feedback')).remove();
        $($($($('#meeting_finish_date').parent()).parent()).find('.invalid-feedback')).remove();
        var valid = compareDate('meeting_start_date', 'meeting_finish_date', 0);

        if (!valid && $('#meeting_finish_date').val() != "")
            $($($('#meeting_start_date').parent()).parent()).append('<div class="invalid-feedback">Bitmə vaxtı başlama vaxtından kiçik və ya ona bərabər ola bilməz. Zəhmət olmazsa, seçiminizi dəyişdirin!</div>');

    })

    $(document).on('change', '#meeting_finish_date', function (e) {
        $($($($('#meeting_start_date').parent()).parent()).find('.invalid-feedback')).remove();
        $($($($('#meeting_finish_date').parent()).parent()).find('.invalid-feedback')).remove();
        var valid = compareDate('meeting_start_date', 'meeting_finish_date', 0);

        if (!valid && $('#meeting_finish_date').val() != "")
            $($($('#meeting_finish_date').parent()).parent()).append('<div class="invalid-feedback">Bitmə vaxtı başlama vaxtından kiçik və ya ona bərabər ola bilməz. Zəhmət olmazsa, seçiminizi dəyişdirin!</div>');

    })

    $(document).on('click', '.move_page', function (e) {
        page = $(this).data('page');
        Pagination(page)
    });

    $(document).on('click', '.move_back', function (e) {
        page--;
        Pagination(page);
    })

    $(document).on('click', '.move_next', function (e) {
        page++;
        Pagination(page);
    });

    $(document).on('click', '#show_close a', function () {
        $('#meeting_show_partial').remove();
        $('#ml_show_partial').remove();
    });

    $(document).on('click', '#meeting_show_close a', function (e) {
        //var body = $("html, body");
        //body.stop().animate({ scrollTop: $('#report_partial').position().top }, 500, 'swing');
        //$('#ml_show_partial').remove();

        $('#meeting_show_partial').remove();
        $('#ml_show_partial').remove();
    })

    $(document).on('click', '.meeting_c', function () {
        var val = $(this).data('id');
        $('#meeting_show_partial').remove();
        $('#ml_show_partial').remove();

        $.ajax({
            type: 'get',
            url: '/Meeting/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $('#report_page').append(result);
                //window.scrollTo(0, $('.meeting_show').position().top);
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('.meeting_operation_line_table').position().top }, 500, 'swing');
            }
        });
    });


    $(document).on('click', '.meeting_line_c', function () {
        var val = $(this).data('id');
        $('#meeting_show_partial').remove();
        $('#ml_show_partial').remove();

        $.ajax({
            type: 'get',
            url: '/MeetingLine/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $('#report_page').append(result);

                $(result).insertBefore('#meeting_show_close');
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('#ml_show_partial').position().top }, 500, 'swing');
            }
        });
    });


    function fdataAppend() {
        fd = new FormData();

        fd.set('page', parseInt(page));

        if ($('#meeting_type').val() != null) {
            $.each($('#meeting_type').val(), function (index, item) {
                fd.append('meetingType', parseInt(item));
            });
        }



        if ($('#title').val() != null) {
            fd.append('title', $('#title').val());
        }


        if ($('#meetingDepartment').val() != null) {
            $.each($('#meetingDepartment').val(), function (index, item) {
                fd.append('department', parseInt(item));
            });
        }



        if ($('#meeting_place').val() != null) {
            $.each($('#meeting_place').val(), function (index, item) {
                fd.append('place', parseInt(item));
            });
        }


        if ($('#ownerUser').val() != null) {
            $.each($('#ownerUser').val(), function (index, item) {
                fd.append('ownerUser', parseInt(item));
            });
        }


        if ($('#followerUser').val() != null) {
            $.each($('#followerUser').val(), function (index, item) {
                fd.append('followerUser', parseInt(item));
            });
        }

        if ($('#meetingTags').val() != null) {
            fd.append('tags', $('#meetingTags').val());
        }


        fd.set('STime', $('#meeting_start_date').val());
        fd.set('FTime', $('#meeting_finish_date').val());


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
                url: '/Reports/MeetingFull/' + fr,
                method: 'get',
                contentType: false,
                processData: false,
                cache: false,
                success: function (res) {
                    $('#report_page').html('');
                    $('#report_page').append(res);

                    $('.select2c').select2({
                        //allowClear: true,
                        placeholder: "Seçim edin"
                    });
                    $('#informedUser').select2(userObject);
                    $('#ownerUser').select2(userObject);
                    $('#followerUser').select2(userObject);
                    $('#participants').select2(userObject);
                    $('#meetingDepartment').select2(userDepartments);
                    $('#meetingTags').tagsinput();
                    $('#meeting_start_date').datepicker({
                        format: 'dd/mm/yyyy'
                    });
                    $('#meeting_finish_date').datepicker({
                        format: 'dd/mm/yyyy'
                    });
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
        var m_start = $('#meeting_start_date').val().split('/');
        m_start = m_start[1] + "/" + m_start[0] + "/" + m_start[2];

        var m_finish = $('#meeting_finish_date').val().length > 0 ? $('#meeting_finish_date').val().split('/') : "";
        m_finish = $('#meeting_finish_date').val().length > 0 ? m_finish[1] + "/" + m_finish[0] + "/" + m_finish[2] : "";

        val += $('#meeting_type').val().join('+') + "-";
        val += $('#title').val().length > 0 ? $('#title').val() + "-" : "-";
        val += $('#meeting_place').val() != null ? $('#meeting_place').val().join('+') + "-" : "-";
        val += $('#ownerUser').val() != null ? $('#ownerUser').val().join('+') + "-" : "-";
        val += $('#followerUser').val() != null ? $('#followerUser').val().join('+') + "-" : "-";
        val += $('#meetingTags').val().length > 0 ? $('#meetingTags').val().split(',').join("+") + "-" : "-";
        val += m_start + "-";
        val += m_finish + "-";
        val += $('#departmentId').val() != null ? $('#departmentId').val().join('+') + "-" : "-";

        $('#modal-1').modal('show', { backdrop: 'static' });
        $('#modal-12').modal('hide');
        setTimeout(function (e) {
            var form_data = new FormData();
            form_data.append('info', val);
            form_data.append('name', $('#filter_name_save').val());

            $.ajax({
                url: '/Reports/ReportMSave',
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
        //var checkVal = false;

        //for (var i = 0; i < $('.' + className + ' .select2c:required').length; i++) {
        //    if ($('.' + className + ' .select2c:required')[i].checkValidity()) {
        //        checkVal = true;
        //        break;
        //    }
        //}



        return $('#meeting_start_date').val().length > 0;
    }



});