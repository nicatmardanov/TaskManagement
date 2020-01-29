$(document).ready(function (e) {

    var userObject = new Object({
        minimumInputLength: 3,
        language: {
            inputTooShort: function () {
                return 'Minimum 3 simvol daxil edin';
            },
            noResults: function () {
                return "Bu sorğuya uyğun nəticə tapılmadı";
            },
            searching: function () {
                return "Axtarış gedir..."
            }
        },
        placeholder: "İstifadəçinin adını daxil edin",
        ajax: {
            url: '/Users/GetNonAdmins',
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

    $('#user').select2(userObject);


    $('#user').on('select2:select', function (e) {

        $('#all_permissions').remove();

        $.ajax({
            method: 'get',
            url: '/Users/PermissionListBox/' + $(this).val(),
            success: function (result) {
                $('#permission_page').append(result);

                var listBox = {
                    nonSelectedListLabel: 'Bütün səlahiyyətlər',
                    selectedListLabel: 'İstifadəçinin səlahiyyətləri',
                    preserveSelectionOnMove: 'moved',
                    moveOnSelect: false,
                    infoText: 'Siyahıda var: {0}',
                    infoTextEmpty: 'Siyahı boşdur',
                    filterPlaceHolder: 'Axtar',
                    infoTextFiltered: '<span class="label label-warning">Göstərilir</span> {0} / {1}',
                    filterTextClear: 'Hamısını göstər',
                    moveSelectedLabel: 'Seçilmiş bəndi göndər',
                    moveAllLabel: 'Hamısını göndər',
                    removeSelectedLabel: 'Seçilmiş bəndi sil',
                    removeAllLabel: 'Hamısını sil',
                }

                $('.permission_pages').bootstrapDualListbox(listBox);
                $('.permission_meeting_type').bootstrapDualListbox(listBox);
                $('.permission_meeting_dep').bootstrapDualListbox(listBox);
            }
        })
    });


    $(document).on('click', '.save_permissions_button', function (e) {
        var pages = $('.permission_pages').val() != null ? $('.permission_pages').val().join(';') : "";
        var m_type = $('.permission_meeting_type').val() != null ? $('.permission_meeting_type').val().join(';') : "";
        var dep = $('.permission_meeting_dep').val() != null ? $('.permission_meeting_dep').val().join(';') : "";

        var fd = new FormData();
        fd.append('Page', pages);
        fd.append('Mtype', m_type);
        fd.append('Department', dep);
        fd.append('UserId', parseInt($('#user').val()));
        fd.append('Id', parseInt($('#lb_id').val()));

        $('#modal-1').modal('show', { backdrop: 'static' });
        setTimeout(function (e) {
            $.ajax({
                url: '/Users/Permissions',
                method: 'post',
                contentType: false,
                processData: false,
                data: fd,
                cache: false,
                success: function () {
                    window.location.reload();
                }
            })
        }, 500);

    })


});