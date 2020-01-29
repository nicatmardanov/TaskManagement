$(document).ready(function (e) {
    $(document).on('click', '.cancelML', function (e) {

        var fd = new FormData();
        fd.set('MId', parseInt($('#meeting_cancel').data('id')));
        fd.set('Description', $('#cancel_note').val());


        $('#modal-1').modal('show', { backdrop: 'static' });
        setTimeout(function () {
            $.ajax({
                url: '/Meeting/Cancel',
                method: 'post',
                contentType: false,
                processData: false,
                data: fd,
                cache: false,
                success: function () {
                    setTimeout(function () {
                        $('#modal-1').modal('hide');
                        window.location.href = "/Meeting/AllMeetings";
                    }, 500);
                }
            })
        }, 500);
    });
});