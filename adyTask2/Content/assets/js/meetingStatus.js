$(document).ready(function (e) {

    function checkValid(className) {
        var checkVal = true;

        if (!$('.' + className + ' textarea')[0].checkValidity()) {
            checkVal = false;
            if ($($($($('.' + className + ' textarea').parent().parent())).find('.invalid-feedback')).length == 0)
                $($($('.' + className + ' textarea').parent())).append('<div class="invalid-feedback">Zəhmət olmazsa bu sahəni doldurun.</div>');
        }

        return checkVal;
    }

    $(document).on('click', '.saveStatus', function (e) {
        var checkVal = checkValid('meeting_status_page');
        var data = {
            Description: $('#status_note').val(),
            RefId: $('.meeting_status_page').data('id')
        };

        var url = ""; 

        if (checkVal && $('#status').val() == 1) {
            url = '/MeetingLine/Status';
        }
        else {
            url = '/MeetingLine/Revision';
        }

        $.ajax({
            url: url,
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(data),
            cache: false,
            success: function () {
                window.location.href = "/Task/AllTasks";
            }
        })

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    })
})