$(document).ready(function (e) {
    $(document).on('click', '.cancelML', function (e) {

        var fd = new FormData();
        fd.set('MlId', parseInt($('#meeting_line_cancel').data('id')));
        fd.set('Description', $('#cancel_note').val());


        $('#modal-1').modal('show', { backdrop: 'static' });
        setTimeout(function () {
            $.ajax({
                url: '/MeetingLine/Cancel',
                method: 'post',
                contentType: false,
                processData: false,
                data: fd,
                cache: false,
                success: function () {
                    setTimeout(function (e) {
                        $('#modal-1').modal('hide');
                        window.location.href = "/Task/AllTasks";
                    }, 500);
                }
            })
        }, 500);
    });
});