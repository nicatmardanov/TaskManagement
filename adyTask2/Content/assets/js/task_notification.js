$(document).ready(function (e) {

    var page = parseInt($('#notifications_all').data('page'));

    $mltElement = $('#meeting_line_type').select2({
        placeholder: "İclas sətirinin növü",
        allowClear: true
    });

    $ncElement = $('#notCompletedButton').select2({
        placeholder: "Açıqda qalanlar",
        allowClear: true
    });

    $myElement = $('#myMeetingLinesButton').select2({
        placeholder: "Mənim yaratdıqlarım",
        allowClear: true
    });


    var page = 1, mltype = 0, not_completed = 0, myML = 0;

    function ajax_remove(ids) {
        if (ids != null && ids.length > 0) {
            var fd = new FormData();

            $.each(ids, function (index, item) {
                fd.append('ids', item);
            });

            $('#modal-1').modal('show', { backdrop: 'static' });
            setTimeout(function (e) {
                $.ajax({
                    url: '/Task/Notifications',
                    method: 'post',
                    contentType: false,
                    processData: false,
                    data: fd,
                    cache: false,
                    success: function () {
                        ajax_mlTable();
                        setTimeout(function (e) {
                            $('#modal-1').modal('hide');
                            var body = $("html, body");
                            body.stop().animate({ scrollTop: $('#notificationPage').position().top }, 500, 'swing');
                        }, 500)
                    }
                })
            }, 500);
        }
    }


    function ajax_mlTable() {
        var fullUrl = '/Task/Notifications?page=' + page + '&mtype=' + mltype + '&nc=' + not_completed + '&mm=' + myML;
        $('#modal-1').modal('show', { backdrop: 'static' });
        setTimeout(function () {

            $.ajax({
                type: 'get',
                url: fullUrl,
                contentType: "application/json",
                success: function (result) {
                    $('#notificationPage').html($($(result).find('#notificationPage')).html());
                    $mltElement = $('#meeting_line_type').select2({
                        placeholder: "İclas sətirinin növü",
                        allowClear: true
                    });

                    $ncElement = $('#notCompletedButton').select2({
                        placeholder: "Açıqda qalanlar",
                        allowClear: true
                    });

                    $myElement = $('#myMeetingLinesButton').select2({
                        placeholder: "Mənim yaratdıqlarım",
                        allowClear: true
                    });

                    if ($('#notificationPage .task_pages').length > 0 && $($(result).find('.task_pages')).length > 0)
                        $('#notificationPage .task_pages').html($($(result).find('.task_pages')).html());

                    else if ($('#notificationPage .task_pages').length > 0 || $($(result).find('.task_pages')).length == 0)
                        $('#notificationPage .task_pages').remove()

                    else if ($('#notificationPage .task_pages').length == 0 || $($(result).find('.task_pages')).length > 0)
                        $('#notificationPage').append($($(result).find('.task_pages')));

                    setTimeout(function (e) {
                        $('#modal-1').modal('hide');
                        var body = $("html, body");
                        body.stop().animate({ scrollTop: $('#ml_type_all').position().top }, 500, 'swing');
                    }, 500)
                }
            });
        }, 500);
    }

    function Pagination(parameter_page) {
        page = parameter_page
        ajax_mlTable();
    }

    $(document).on('click', '.remove_notification', function (e) {
        var ids = [];
        ids[0] = parseInt($($($(this).parent()).parent()).data('id'));
        ajax_remove(ids);
    });

    $(document).on('click', '.all_remove_notifications', function (e) {
        var ids = [];
        for (var i = 0; i < $('.ml_check:checked').length; i++) {
            ids[i] = parseInt($($('.ml_check:checked')[i]).data('id'));
        }
        ajax_remove(ids);
    });

    $(document).on('change', '#meeting_line_type', function (e) {
        if ($(this).val() != "") {
            mltype = $(this).val();
        }

        page = 1;
        ajax_mlTable();

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('change', '#notCompletedButton', function (e) {
        if ($(this).val() != "")
            not_completed = $(this).val();


        page = 1;
        ajax_mlTable();

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('change', '#myMeetingLinesButton', function (e) {
        if ($(this).val() != "")
            myML = $(this).val();

        page = 1;
        ajax_mlTable();

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $('#meeting_line_type').on('select2:unselecting', function (e) {
        mltype = 0;
        page = 1;
        ajax_mlTable();
    });



    $('#meeting_line_department').on('select2:unselecting', function (e) {
        dep = 0;
        page = 1;
        ajax_mlTable();
    });

    $('#notCompletedButton').on('select2:unselecting', function (e) {
        not_completed = 0;
        page = 1;
        ajax_mlTable();
    });

    $('#myMeetingLinesButton').on('select2:unselecting', function (e) {
        myML = 0;
        page = 1;
        ajax_mlTable();
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

    $(document).on('click', '.meeting_line_c', function () {
        var val = $($(this).parent()).data('id');
        $('#ml_show_partial').remove();
        $('#meeting_show_partial').remove();

        $.ajax({
            type: 'get',
            url: '/MeetingLine/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $('#notificationPage').append(result);
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('#ml_show_partial').position().top }, 500, 'swing');
            }
        });
    });

    $(document).on('click', '.meeting_c', function () {
        var val = $($(this).parent()).data('id');
        $('#ml_show_partial').remove();
        $('#meeting_show_partial').remove();

        $.ajax({
            type: 'get',
            url: '/Meeting/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $('#notificationPage').append(result);
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('#meeting_show_partial').position().top }, 500, 'swing');
            }
        });
    });

    $(document).on('click', '#meeting_show_close a', function () {
        $('#ml_show_partial').remove();
        $('#meeting_show_partial').remove();
        var body = $("html, body");
        body.stop().animate({ scrollTop: $('#notificationPage').position().top }, 500, 'swing');
    })

    $(document).on('click', '#show_close a', function (e) {
        $('#ml_show_partial').remove();
        $('#meeting_show_partial').remove();
        var body = $("html, body");
        body.stop().animate({ scrollTop: $('#notificationPage').position().top }, 500, 'swing');
    });


});