$(document).ready(function(e) {

    var userObject = new Object({
        minimumInputLength: 3,
        language: {
            inputTooShort: function() {
                return 'Minimum 3 simvol daxil edin';
            },
            noResults: function() {
                return "Bu sorğuya uyğun nəticə tapılmadı";
            }
        },
        placeholder: "Daxil edin",
        ajax: {
            url: '/Users/GetUsers',
            dataType: 'json',
            type: "GET",
            data: function(term) {
                return term;
            },
            processResults: function(data) {
                var myResults = [];
                $.each(data, function(index, item) {
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

    $('#responsibleUser').select2(userObject);


    $(document).on('click', '.saveDirect', function (e) {
        var fd = new FormData();

        fd.append('ToUserEmail', $('#responsibleUser').val());
        fd.append('Description', $('#direct_note').val());
        fd.append('FinishDate', $('#meeting_d_finish_date').val());
        fd.append('MlId', $('.direct_page').data('id'));

        $.ajax({
            url: '/MeetingLine/Redirect',
            type: 'POST',
            data: fd,
            contentType: false,
            processData: false,
            cache: false,
            success: function () {
                window.location.href = "/Task/AllTasks";
            }
        })

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });
});