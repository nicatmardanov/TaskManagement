$(document).ready(function (e) {
    $('#ml_update .select2c').select2();

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
    var userObject = new Object({
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
            url: '/Users/GetUsers',
            dataType: 'json',
            type: "GET",
            data: function (term) {
                return term;
            },
            processResults: function (data) {
                var myResults = [];
                $.each(data, function (index, item) {
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

    $('.ml_up_department').select2(departments);
    $('.ml_up_resp_user').select2(userObject);
    $('.ml_up_ident_user').select2(userObject);
    $('.ml_up_follower_user').select2(userObject);
    $('.ml_up_inf_user').select2(userObject);

    $('#ml_update .datepicker').datepicker({
        format: 'dd/mm/yyyy'
    });

    var deletedTags = [];
    var addedTags = [];

    var deletedDepartment = [];
    var addedDepartment = [];

    $(document).on('click', '.remove_file a', function (e) {

        $($($($($(this).parent()).parent()).parent()).find('input')).val('');
        $($($(this).parent()).parent()).remove();

        //fileChange = true;

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    function checkValid(className) {
        var checkVal = true;

        for (var i = 0; i < $('.' + className + ' input:required').length; i++) {
            if (!$('.' + className + ' input:required')[i].checkValidity()) {
                checkVal = false;
                if ($($($($($('.' + className + ' input:required')[i]).parent().parent())).find('.invalid-feedback')).length == 0)
                    $($($($('.' + className + ' input:required')[i]).parent())).append('<div class="invalid-feedback">Zəhmət olmazsa bu sahəni doldurun.</div>');
            }
        }

        for (var i = 0; i < $('.' + className + ' .select2c:required').length; i++) {
            if (!$('.' + className + ' .select2c:required')[i].checkValidity()) {
                checkVal = false;
                if ($($($($($('.' + className + ' .select2c:required')[i]).parent().parent())).find('.invalid-feedback')).length == 0)
                    $($($($('.' + className + ' .select2c:required')[i]).parent())).append('<div class="invalid-feedback">Zəhmət olmazsa bu sahəni doldurun.</div>');
            }
        }

        if (!$('.' + className + ' textarea')[0].checkValidity()) {
            checkVal = false;
            if ($($($($('.' + className + ' textarea').parent().parent())).find('.invalid-feedback')).length == 0)
                $($($('.' + className + ' textarea').parent())).append('<div class="invalid-feedback">Zəhmət olmazsa bu sahəni doldurun.</div>');
        }

        return checkVal;
    }

    $(document).on('change', '.fileinput input', function (e) {
        $($($($($(this).parent()).parent()).parent()).find('.remove_file')).remove();

        $($($($(this).parent()).parent()).parent()).append('<div class="remove_file"><div><span>' + e.target.files[0].name + '</span><a href="javascript:void(0)"><i class="fa fa-trash-o"></i></a></div></div>');

        $('#ml_edit_file').addClass('fileChange');
    });

    //$(".ml_up_tags").on('itemAdded', function (event) {
    //    addedTags[addedTags.length] = event.item;
    //});
    //$(".ml_up_tags").on('itemRemoved', function (event) {
    //    deletedTags[deletedTags.length] = event.item;
    //});

    //$('.ml_up_department').on('select2:unselecting', function (e) {
    //    deletedDepartment[deletedDepartment.length] = parseInt(e.params.args.data.id);
    //});

    //$('.ml_up_department').on('select2:select', function (e) {
    //    addedDepartment[addedDepartment.length] = parseInt(e.params.data.id);
    //});

    var addedTagsLength = $('.ml_up_tags').val().length > 0 ? $('.ml_up_tags').val().split(',').length : 0;;
    $(document).on('itemAdded', ".ml_up_tags", function (event) {
        addedTags.push(event.item);
    })

    $(document).on('itemRemoved', ".ml_up_tags", function (event) {
        if (jQuery.inArray(event.item, addedTags.slice(addedTagsLength)) == 0) {
            addedTags = jQuery.grep(addedTags, function (value) {
                return value != event.item;
            });
        }
        else
            deletedTags[deletedTags.length] = event.item;
    });

    $(document).on('select2:unselecting', '.ml_up_department', function (e) {
        if (addedDepartment.includes(parseInt(e.params.args.data.id))) {
            addedDepartment = jQuery.grep(addedDepartment, function (value) {
                return value != e.params.args.data.id;
            });
        }
        else {
            deletedDepartment[deletedDepartment.length] = parseInt(e.params.args.data.id);
        }

    });


    $(document).on('select2:select', '.ml_up_department', function (e) {
        addedDepartment[addedDepartment.length] = parseInt(e.params.data.id);
    });

    function compareDate(selector1, selector2, type) {
        if (type == 0) {
            var date1_string = $("#" + selector1).val().split("/");
            var date2_string = $("#" + selector2).val().split("/");
        }
        else {
            var date1_string = $("." + selector1).val().split("/");
            var date2_string = $("." + selector2).val().split("/");
        }

        var date1 = new Date(date1_string[1] + "/" + date1_string[0] + "/" + date1_string[2]);
        var date2 = new Date(date2_string[1] + "/" + date2_string[0] + "/" + date2_string[2]);

        if (date2 > date1)
            return true;

        return false;
    }



    $(document).on('click', '.editMeetingLine', function () {
        var fd = new FormData();

        if ($("#ml_edit_file")[0].files[0] != undefined && $('#ml_edit_file').hasClass('fileChange')) {
            fd.append("meetingLFile", $("#ml_edit_file")[0].files[0]);
        }
        else if (!$('#ml_edit_file').hasClass('fileChange') && $('.remove_file').length == 1) {
            fd.append("FileNotChanged", true);
        }
        else if ($('.remove_file').length == 0) {
            fd.append("FileEmpty", true);
        }

        $.each(deletedTags, function (index, item) {
            fd.append('deletedTags', item);
        });

        $.each(addedTags.slice(addedTagsLength), function (index, item) {
            fd.append('addedTags', item);
        });

        $.each(deletedDepartment, function (index, item) {
            fd.append('deletedDepartment', item);
        });

        $.each(addedDepartment, function (index, item) {
            fd.append('addedDepartment', item);
        });

        fd.append("Id", $('#ml_update').data('id'));

        var checkVal = checkValid('ml_add');
        var isValiDate = compareDate($('.ml_up_s_date').val(), $('.ml_up_f_date').val(), 1);

        if (checkVal && isValiDate) {
            fd.append('MlType', $('.ml_up_type').val());
            fd.append('Description', $('.ml_up_description').val());
            fd.append('ResponsibleEmail', $('.ml_up_resp_user').val());
            fd.append('IdentifierEmail', $('.ml_up_ident_user').val());
            fd.append('FollowerEmail', $('.ml_up_follower_user').val());

            if ($('.ml_up_inf_user').val() != null) {
                fd.append('InformedUserEmail', $('.ml_up_inf_user').val().join(';'));
            }
            else
                fd.append('InformedUserEmail', "");

            fd.append("STime", $('.ml_up_s_date').val());
            fd.append("FTime", $('.ml_up_f_date').val());


            $('#modal-7').modal('show', { backdrop: 'static' });
            setTimeout(function (e) {
                $.ajax({
                    url: '/MeetingLine/Edit',
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
                        deletedTags = [];
                        addedTags = [];

                        deletedDepartment = [];
                        addedDepartment = [];
                    }
                });
            }, 500);

        }


    });


    $(document).on('click', '.add_ml_option>a', function (e) {
        var checkVal = checkValid('meeting_add');
        $('.meeting_add .invalid-feedback').remove();

        if (checkVal && !compareMeetingTime() && $('.meeting_line_add_table').length == 0 && !$('.meeting_add .updtSave>a').hasClass('saveMeeting')) {
            meeting_line_add_ajax(e);
        }
        else {
            alert('İclas sətri daxil etmək üçün əvvəlcə iclas yaratmağınız tələb olunur!');
        }

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    })


});