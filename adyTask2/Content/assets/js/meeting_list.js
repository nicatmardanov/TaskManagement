$(document).ready(function (e) {

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

    $('#meeting_department').select2(departments);

    $mltElement = $('#meeting_type').select2({
        placeholder: "İclasın növü",
        allowClear: true
    });



    var page = 1, dep = 0, meetingType = 0, myMeeting = false;

    function ajax_mTable() {
        var fullUrl = 'page=' + page + '&mt=' + meetingType + '&mm=' + myMeeting + '&dep=' + dep + '&type=' + $('.meeting_list_page').data('type');
        $('#modal-1').modal('show', { backdrop: 'static' });
        setTimeout(function () {
            $.ajax({
                type: 'get',
                url: '/Meeting/MeetingList?' + fullUrl,
                contentType: "application/json",
                success: function (result) {
                    $('#meeting_all_lists .meeting_operation_line_table .panel-body').html($($(result).find('.meeting_operation_line_table .panel-body')).html());
                    if ($('#meeting_all_lists .meeting_pages').length > 0)
                        $('#meeting_all_lists .meeting_pages').html($($(result).find('.meeting_pages')).html());
                    $('#meeting_show_partial').remove();

                    setTimeout(function (e) {
                        $('#modal-1').modal('hide');
                        var body = $("html, body");
                        body.stop().animate({ scrollTop: $('#meeting_all_lists').position().top }, 500, 'swing');
                    }, 500)
                }
            });
        }, 500)
    }

    function Pagination(parameter_page) {
        page = parameter_page;
        ajax_mTable();
    }


    $(document).on('click', '.publish_meeting', function (e) {
        var id = parseInt($(this).data('id'));
        $.ajax({
            url: '/Meeting/Status/' + id,
            type: 'get',
            success: function () {
                ajax_mTable();
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        })
    });


    $(document).on('change', '#meeting_department', function (e) {
        if ($(this).val() != "")
            dep = $(this).val();

        page = 1;

        ajax_mTable();
    });

    $(document).on('change', '#meeting_type', function (e) {
        if ($(this).val() != "")
            meetingType = $(this).val();

        page = 1;


        ajax_mTable();
    });

    $(document).on('click', '.myMeetingsButton', function (e) {
        if ($($($(this).parent()).find('div')).hasClass('custom_switch_passive'))
            myMeeting = false;
        else
            myMeeting = true;

        page = 1;

        ajax_mTable();
    });

    $('#meeting_type').on('select2:unselecting', function (e) {
        meetingType = 0;
        page = 1;
        ajax_mTable();
    });

    $('#meeting_department').on('select2:unselecting', function (e) {
        dep = 0;
        page = 1;
        ajax_mTable();
    });

    $(document).on('click', '#meeting_show_close a', function () {
        $('#meeting_show_partial').remove();
    })

    $(document).on('click', '.meeting_c', function () {
        var val = $(this).data('id');
        $('#meeting_show_partial').remove();

        $.ajax({
            type: 'get',
            url: '/Meeting/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $('#meeting_all_lists').append(result);
                //window.scrollTo(0, $('.meeting_show').position().top);
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('.meeting_show').position().top }, 500, 'swing');
            }
        });
    });

    $(document).on('change', '.first-check', function () {
        if ($(this).prop('checked'))
            $('.icheck-11').prop('checked', true);
        else
            $('.icheck-11').prop('checked', false);
    });

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

    $(document).on('click', '#meeting_show_close a', function (e) {
        $('#ml_show_partial').remove();
    });


    $(document).on('click', '.meeting_line_c', function () {
        var val = $(this).data('id');
        $('.meeting_operation_table').remove();

        $.ajax({
            type: 'get',
            url: '/MeetingLine/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $(result).insertBefore('#meeting_show_partial>.row');
                $('#show_close').remove();
                //window.scrollTo(0, $('#ml_show_partial').position().top);
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('.meeting_operation_table').position().top }, 500, 'swing');
            }
        });
    });


    ////
});