$('#completion').on("input", function (e) {
    var value = $(this).val();
    if (value > 100) {
        var old_val = value.substring(0, value.length - 1);
        $(this).val(old_val);
        alert('Tamamlanma faizi 100-dən böyük ola bilməz!');
    }
});

$(document).ready(function (e) {

    var moDescp = [];
    var moComp = [];
    var moFile = [];
    var moFilePos = [];
    var iter = parseInt($('.meeting_operation_table').data('count'))+1;





    function checkValid(className) {
        var checkVal = true;

        if (!$('.' + className + ' input:required')[0].checkValidity()) {
            checkVal = false;
            if ($($($($($('.' + className + ' input:required')).parent().parent())).find('.invalid-feedback')).length == 0)
                $($($($('.' + className + ' input:required')).parent())).append('<div class="invalid-feedback">Zəhmət olmazsa bu sahəni doldurun.</div>');
        }

        if (!$('.' + className + ' textarea')[0].checkValidity()) {
            checkVal = false;
            if ($($($($('.' + className + ' textarea').parent().parent())).find('.invalid-feedback')).length == 0)
                $($($('.' + className + ' textarea').parent())).append('<div class="invalid-feedback">Zəhmət olmazsa bu sahəni doldurun.</div>');
        }

        return checkVal;
    }



    $(document).on('click', '.meeting_operation_add .saveOperation', function (e) {

        var checkVal = checkValid('meeting_operation_add');

        if (checkVal) {
            $('.meeting_operation_table tbody').append('<tr><td>' + iter + '</td><td>' + $('.m_description').val() + '</td><td>' + $('#completion').val() + '</td><td>' + $('.us').val() + '</td><td>Əlavə edildi</td></tr>')

            moDescp[moDescp.length] = $('.m_description').val();
            moComp[moComp.length] = $('#completion').val();
            moFile[moComp.length] = $("#meetingOperationFile")[0].files[0];

            if ($("#meetingOperationFile")[0].files[0] == undefined)
                moFilePos[moFilePos.length] = false;
            else
                moFilePos[moFilePos.length] = true;


            $('.meeting_operation_form .panel-options>a').click();

            setTimeout(function () {
                $('.meeting_operation_form').remove();
                $('.publish_meeting').show();
            }, 150);


            iter++;
        }


        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });


    $(document).on('click', '.meeting_operation_table .new_meeting_operation', function (e) {
        if ($('.meeting_operation_form').length < 1)
            $.ajax({
                method: 'get',
                url: '/MeetingOperation/MeetingOperationForm',
                success: function (result) {
                    $('.publish_meeting').hide();
                    $('.meeting_operation_add').append(result);
                }
            })


        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('click', '.close_operation', function (e) {
        window.location.href = "/Task/AllTasks";
    });

    $(document).on('click', '.publish_meeting .submit_operation', function (e) {

        var fd = new FormData();

        if (moDescp.length > 0) {
            $.each(moDescp, function (index, item) {
                fd.append('moDescp', item);
            });

            $.each(moComp, function (index, item) {
                fd.append('moComp', item);
            });

            $.each(moFile, function (index, item) {
                fd.append('moFile', item);
            });

            $.each(moFilePos, function (index, item) {
                fd.append('moFilePos', item);
            });

            fd.append('mlId', $('.meeting_operation_table').data('id'));

            $('#modal-7').modal('show', { backdrop: 'static' });
            setTimeout(function () {
                $.ajax({
                    url: '/MeetingOperation/Add',
                    method: 'post',
                    contentType: false,
                    processData: false,
                    data: fd,
                    cache: false,
                    xhr: function () {
                        var xhr = new window.XMLHttpRequest();
                        xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                var prcComplete = evt.loaded / evt.total;
                                prcComplete = parseInt(prcComplete * 100);
                                $('#modal-7 .modal_percent').html(prcComplete + '%');
                            }
                        }, false);
                        return xhr;
                    },
                    success: function () {
                        window.location.href = "/Task/AllTasks";
                    }
                })
            }, 500);
        }

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('click', '.close_button', function (e) {
        setTimeout(function () {
            $('.meeting_operation_form .panel-options>a').click();

            $('.meeting_operation_form').remove();
            $('.publish_meeting').show();
        }, 150);


        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });



    $(document).on('change', '.fileinput input', function (e) {
        $($($($($(this).parent()).parent()).parent()).find('.remove_file')).remove();
        $($($($(this).parent()).parent()).parent()).append('<div class="remove_file"><div><span>' + e.target.files[0].name + '</span><a href="javascript:void(0)"><i class="fa fa-trash-o"></i></a></div></div>');

    });

    $(document).on('click', '.remove_file a', function (e) {
        $($($($($(this).parent()).parent()).parent()).find('input')).val('');
        $($($(this).parent()).parent()).remove();


        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

})