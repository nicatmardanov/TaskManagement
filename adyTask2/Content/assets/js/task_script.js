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

    $('#meeting_line_department').select2(departments);

    $mltElement = $('#meeting_line_type').select2({
        placeholder: "İclas sətirinin növü",
        allowClear: true
    });



    var page = 1, dep = 0, mltype = 0, not_completed = false, myML = false;

    function ajax_mlTable() {
        var fullUrl = 'page=' + page + '&ml=' + mltype + '&nc=' + not_completed + '&mm=' + myML + '&dep=' + dep + '&type=' + $('#taskPage').data('type') + '&mtype=' + $('#taskPage').data('mtype');
        $('#modal-1').modal('show', { backdrop: 'static' });
        setTimeout(function () {
            $.ajax({
                type: 'get',
                url: '/Task/MyTasks?' + fullUrl,
                contentType: "application/json",
                success: function (result) {
                    $('#ml_type_all .meeting_operation_line_table .panel-body').html($($(result).find('.meeting_operation_line_table .panel-body')).html());
                    if ($('#ml_type_all .task_pages').length > 0 && $($(result).find('.task_pages')).length > 0)
                        $('#ml_type_all .task_pages').html($($(result).find('.task_pages')).html());

                    else if ($('#ml_type_all .task_pages').length > 0 || $($(result).find('.task_pages')).length == 0)
                        $('#ml_type_all .task_pages').remove()

                    else if ($('#ml_type_all .task_pages').length == 0 || $($(result).find('.task_pages')).length > 0)
                        $('#ml_type_all').append($($(result).find('.task_pages')));

                    setTimeout(function (e) {
                        $('#modal-1').modal('hide');
                        var body = $("html, body");
                        body.stop().animate({ scrollTop: $('#ml_type_all').position().top }, 500, 'swing');
                    }, 500)

                }

            });
        }, 500)
    }

    function Pagination(parameter_page) {
        page = parameter_page
        $('#ml_show_partial').remove();
        ajax_mlTable();
    }


    $(document).on('change', '#meeting_line_department', function (e) {
        if ($(this).val() != "")
            dep = $(this).val();

        page = 1;
        ajax_mlTable();

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('change', '#meeting_line_type', function (e) {
        if ($(this).val() != "")
            mltype = $(this).val();

        page = 1;
        ajax_mlTable();

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('click', '.notCompletedButton', function (e) {

        if ($($($(this).parent()).find('input')).prop('checked'))
            not_completed = true;
        else {
            not_completed = false;
        }

        page = 1;
        ajax_mlTable();

        //e.preventDefault();
        //e.stopImmediatePropagation();
        //return false;
    });

    $(document).on('click', '.myMeetingLinesButton', function (e) {
        if ($($($(this).parent()).find('input')).prop('checked'))
            myML = true;
        else
            myML = false;

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



    $(document).on('click', '.meeting_line_c', function () {
        var val = $(this).data('id');
        $('#ml_show_partial').remove();

        $.ajax({
            type: 'get',
            url: '/MeetingLine/Show/' + val,
            contentType: "application/json",
            success: function (result) {
                $('#taskPage').append(result);
                //window.scrollTo(0, $('#ml_show_partial').position().top);
                var body = $("html, body");
                body.stop().animate({ scrollTop: $('#ml_show_partial').position().top }, 500, 'swing');
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
        page = parseInt($(this).data('page')) - 1;
        Pagination(page);
    })

    $(document).on('click', '.move_next', function (e) {
        page = parseInt($(this).data('page')) + 1;
        Pagination(page);
    });

    $(document).on('click', '#show_close a', function (e) {
        $('#ml_show_partial').remove();
        var body = $("html, body");
        body.stop().animate({ scrollTop: $('#ml_type_all').position().top }, 500, 'swing');
    });

    $(document).on('click', '.publish_ml_task', function (e) {
        var data = {
            Description: "Yayınlandı",
            RefId: $('.publish_ml_task').data('id')
        };

        $.ajax({
            url: '/MeetingLine/Status',
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(data),
            cache: false,
            success: function () {
                ajax_mlTable();
            }
        })
    });

    $(document).on('click', '.revision_option>a', function (e) {
        var revision = [];

        for (var i = 0; i < $('.ml_check:checked').length; i++) {
            revision[i] = parseInt($($('.ml_check:checked')[i]).data('id'));
        }

        if (revision.length > 0) {
            var data = new FormData();
            $.each(revision, function (index, item) {
                data.append('ids', item);
            });

            $.ajax({
                url: '/MeetingLine/RevisionMulti',
                method: 'post',
                contentType: false,
                processData: false,
                data: data,
                cache: false,
                success: function () {
                    window.location.reload();
                }
            })
        }

    });


    $(document).on('click', '.conf_ml_option>a', function (e) {
        var status = [];

        for (var i = 0; i < $('.ml_check:checked').length; i++) {
            status[i] = parseInt($($('.ml_check:checked')[i]).data('id'));
        }

        if (status.length > 0) {
            var data = new FormData();
            $.each(status, function (index, item) {
                data.append('ids', item);
            });

            $.ajax({
                url: '/MeetingLine/StatusMulti',
                method: 'post',
                contentType: false,
                processData: false,
                data: data,
                cache: false,
                success: function () {
                    ajax_mlTable();
                }
            })
        }

    });

    ////
});