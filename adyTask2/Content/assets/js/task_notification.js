$(document).ready(function (e) {

    var page = parseInt($('#notifications_all').data('page'));

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
        $.ajax({
            type: 'get',
            url: '/Task/Notifications/' + page,
            contentType: "application/json",
            success: function (result) {
                $('#notificationPage').html($($(result).find('#notificationPage')).html());
            }
        });
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
    })


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
        page = parseInt($(this).data('page')) - 1;
        Pagination(page);
    })

    $(document).on('click', '.move_next', function (e) {
        page = parseInt($(this).data('page')) + 1;
        Pagination(page);
    });

    $(document).on('click', '.meeting_line_c', function () {
        var val = $($(this).parent()).data('id');
        $('#ml_show_partial').remove();

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

    $(document).on('click', '#show_close a', function (e) {
        $('#ml_show_partial').remove();
        var body = $("html, body");
        body.stop().animate({ scrollTop: $('#notificationPage').position().top }, 500, 'swing');
    });


});