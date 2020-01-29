$(document).ready(function (e) {

    $('.select2c').select2();
    $('#meeting_place').select2({ tags: true });
    $('#other_country').select2({ tags: true });


    var fileChange = false;
    var eindex = 0;

    var deletedMlTags = [];
    var addedMlTags = [];

    var deletedMlDepartment = [];
    var addedMlDepartment = [];

    $('#meetingDepartment').on('select2:unselecting', function (e) {
        if (!$('.meeting_add .updtSave>a').hasClass('saveMeeting')) {
            if (addedDepartment.includes(parseInt(e.params.args.data.id))) {
                addedDepartment = jQuery.grep(addedDepartment, function (value) {
                    return value != e.params.args.data.id;
                });
            }
            else
                deletedDepartment[deletedDepartment.length] = parseInt(e.params.args.data.id);
        }
    });

    $('#meetingDepartment').on('select2:select', function (e) {
        if (!$('.meeting_add .updtSave>a').hasClass('saveMeeting')) {
            addedDepartment[addedDepartment.length] = parseInt(e.params.data.id);
        }
    });

    function meetingSend(e, type, id) {
        var fd = new FormData();

        fd.append("Title", $('#title').val());
        fd.append("MeetingType", $("#meeting_type").val());
        $.each($('#meetingDepartment').val(), function (index, item) {
            fd.append('meetingDepartments', item);
        });

        fd.append("start_date", $("#meeting_start_date").val());
        fd.append("start_time", $("#meeting_start_time").val());
        fd.append("finish_time", $("#meeting_finish_time").val());
        fd.append("OwnerUser", $("#ownerUser").val());


        if ($('#meeting_place option:selected').attr('data-select2-tag') == undefined)
            fd.append("Place", $("#meeting_place").val());
        else
            fd.append("PlaceName", $("#meeting_place").val());

        fd.append("FollowerUser", $("#followerUser").val());

        if ($("#informedUser").val() != null)
            fd.append("InformedUser", $("#informedUser").val().join(";"));
        else
            fd.append("InformedUser", "");


        fd.append("Participiants", $("#participants").val().join(";"));

        if ($("#other_participants").val() != null)
            fd.append("OtherParticipiants", $("#other_participants").val().join(";"));
        else
            fd.append("OtherParticipiants", "");


        fd.append("Description", $("#description").val());
        fd.append("tags", $("#meetingTags").val());

        if ($("#meetingFile")[0].files[0] != undefined) {

            if ($('.meeting_add').data('type') == 0 || ($('.meeting_add').data('type') == 1 && fileChange))
                fd.append("meetingFile", $("#meetingFile")[0].files[0]);
        }

        if (id > 0) {
            fd.append('meetingId', id);
        }

        var url = "";
        if (type == 0)
            url = '/Meeting/Add';
        else if (type == 1) {
            url = '/Meeting/Update';

            $.each(deletedTags, function (index, item) {
                fd.append('deletedTags', item);
            });

            $.each(addedTags, function (index, item) {
                fd.append('addedTags', item);
            });

            $.each(deletedDepartment, function (index, item) {
                fd.append('deletedDepartment', item);
            });

            $.each(addedDepartment, function (index, item) {
                fd.append('addedDepartment', item);
            });
        }

        $('#modal-7').modal('show', { backdrop: 'static' });
        setTimeout(function (e) {
            $.ajax({
                url: url,
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
                success: function (result) {

                    $('#meeting-title-section').html($('#title').val());


                    if ($('.meeting_add .updtSave>a').hasClass('saveMeeting')) {
                        $('.meeting_add').attr('data-id', result.res);

                        $('.meeting_add .updtSave>a').removeClass('saveMeeting');
                        $('.meeting_add .updtSave>a').addClass('updateMeeting');

                        $('.meeting_add').attr('data-type', 1);
                    }
                    else {

                        if ($('.meeting_add').data('edit') == 1) {
                            window.location.href = "/Meeting/AllMeetings";
                        }
                        else {
                            deletedTags = [];
                            addedTags = [];

                            deletedDepartment = [];
                            addedDepartment = [];

                            fileChange = true;
                        }
                    }


                    if (type == 0)
                        $('.publish_meeting').show();



                    if ($('.meeting_line_add_table').length == 0) {
                        meeting_line_add_ajax(e);
                    }

                    setTimeout(function () {
                        $('#modal-7').modal('hide');
                        $('#modal-7 .modal_percent').html('');
                    }, 500)
                }
            });

        }, 500);



        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    }

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

        if ($('.' + className + ' textarea').length > 0)
            if (!$('.' + className + ' textarea')[0].checkValidity()) {
                checkVal = false;
                if ($($($($('.' + className + ' textarea').parent().parent())).find('.invalid-feedback')).length == 0)
                    $($($('.' + className + ' textarea').parent())).append('<div class="invalid-feedback">Zəhmət olmazsa bu sahəni doldurun.</div>');
            }

        return checkVal;
    }

    $(document).on('click', '#save_others', function (e) {
        var checkVal = checkValid('others_modal');
        if (checkVal) {
            var fd = new FormData();
            fd.append('Name', $('#other_name').val());
            fd.append('Surname', $('#other_surname').val());
            fd.append('Email', $('#other_email').val());

            if ($.isNumeric($('#other_country').val()))
                fd.append('CountryId', parseInt($('#other_country').val())); /////////////
            else {
                var countryname = $('#other_country').val() != null ? $('#other_country').val() : "";
                fd.append('CountryName', countryname); /////////////
            }

            fd.append('Position', $('#other_position').val());
            fd.append('Company', $('#other_company').val());

            $.ajax({
                url: '/Users/AddOthers',
                method: 'post',
                contentType: false,
                processData: false,
                data: fd,
                cache: false,
                success: function (result) {
                    var data = {
                        id: result,
                        text: $('#other_name').val() + " " + $('#other_surname').val()
                    };

                    if ($('#other_participants').find("option[value='" + data.id + "']").length) {
                        $('#other_participants').val(data.id).trigger('change');
                    } else {
                        var newOption = new Option(data.text, data.id, true, true);
                        $('#other_participants').append(newOption).trigger('change');
                    }

                    $('.others_modal').modal('hide');
                    $('.others_modal input').val('');
                    $('.others_modal .select2c').val(null).trigger('change');

                }
            })

        }
    })

    $(document).on('click', '.saveMeeting', function (e) {  //save meeting

        var checkVal = checkValid('meeting_add');

        if (checkVal) {
            $('.meeting_add .panel-options a').click();
            $('.meeting_add .invalid-feedback').remove();

            meetingSend(e, 0, 0);
        }


        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('click', '.updateMeeting', function (e) {

        var checkVal = checkValid('meeting_add');

        if (checkVal) {
            $('.meeting_add .panel-options a').click();
            $('.meeting_add .invalid-feedback').remove();
            var meeting_id = $('.meeting_add').data('id');
            meetingSend(e, 1, meeting_id);
        }
    });

    var ml_type = [];
    var mlDepartment = [];
    var mlTags = [];
    var ml_description = [];
    var responsibleUser = [];
    var mlFollowerUser = [];
    var identifierUser = [];
    var informedUserMl = [];
    var meeting_l_start_date = [];
    var meeting_l_finish_date = [];
    var mlFile = [];
    var mlFilePos = [];

    var deletedTags = [];
    var addedTags = [];

    var deletedDepartment = [];
    var addedDepartment = [];


    $(document).on('click', '.saveMeetingLine', function (e) {
        var checkVal = checkValid('ml_add');

        if (checkVal) {
            var form_data = new FormData();
            form_data.append("MeetingId", $('.meeting_add').data('id'));
            form_data.append("MlType", $('#ml_type').val());
            form_data.append("Description", $('#ml_description').val());
            form_data.append("StartTime", $('#meeting_l_start_date').val());
            form_data.append("FinishTime", $('#meeting_l_finish_date').val());
            form_data.append("ResponsibleEmail", $('#responsibleUser').val());
            form_data.append("FollowerEmail", $('#mlFollowerUser').val());
            form_data.append("IdentifierEmail", $('#identifierUser').val());
            if ($('#informedUserMl').val() != null)
                form_data.append("InformedUserEmail", $('#informedUserMl').val().join(';'));
            else
                form_data.append("InformedUserEmail", "");

            form_data.append("MLFile", $("#mlFile")[0].files[0]);

            form_data.append('Tags', $('#mlTags').val());

            $.each($('#mlDepartment').val(), function (index, item) {
                form_data.append('Departments', parseInt(item));
            });

            var meeting_line_id = 0;

            $.ajax({
                url: '/MeetingLine/AddMeetingLine',
                method: 'post',
                contentType: false,
                processData: false,
                data: form_data,
                cache: false,
                async: false,
                success: function (result) {
                    meeting_line_id = result;
                }
            })


            if ($(".meeting_line_add_table").length == 0 && perm == 1) {
                $.ajax({
                    type: 'get',
                    url: '/MeetingLine/MeetingLineTable',
                    success: function (newResult) {
                        $(newResult).insertBefore('.publish_meeting');
                        var tbody = '<tr data-id=' + meeting_line_id + '><td><input tabindex="5" type="checkbox" class="ml_check icheck-11" checked></td><td><div class="dropdown"><button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown">Əməliyyatlar<span class="caret"></span></button><ul class="dropdown-menu"><li><a href="javascript:void(0)" class="edit_meeting_line" data-eindex=' + eindex + ' data-id=' + meeting_line_id + '>Görüntülə</a></li><li><a href="javascript:void(0)" data-id=' + meeting_line_id + ' class="delete_ml">Sil</a></li></ul></div></td><td>' + $('#ml_type option:selected').html() + '</td><td class="table_desc">' + $('#ml_description').val() + '</td><td>' + $('#meeting_l_start_date').val() + '</td><td>' + $('#meeting_l_finish_date').val() + '</td><td class="tableUsers">' + $('#responsibleUser option:selected').text().split(' (')[0] + '</td><td class="tableUsers">' + $('#mlFollowerUser option:selected').text().split(' (')[0] + '</td><td class="tableUsers">' + $('#identifierUser option:selected').text().split(' (')[0] + '</td><td>Qaralama</td></tr>';
                        $('.meeting_line_add_table tbody').append(tbody);
                        eindex++;

                        $('#ml_add .panel-options a').click();
                        setTimeout(function () {
                            $('#ml_add').remove();
                            //$('.publish_meeting').show();
                        }, 500);
                    }
                });
            }

            else {
                var tbody = '<tr data-id=' + meeting_line_id + '><td><input tabindex="5" type="checkbox" class="ml_check icheck-11" checked></td><td><div class="dropdown"><button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown">Əməliyyatlar<span class="caret"></span></button><ul class="dropdown-menu"><li><a href="javascript:void(0)" class="edit_meeting_line" data-eindex=' + eindex + ' data-id=' + meeting_line_id + '>Görüntülə</a></li><li><a href="javascript:void(0)" data-id=' + meeting_line_id + ' class="delete_ml">Sil</a></li></ul></div></td><td>' + $('#ml_type option:selected').html() + '</td><td class="table_desc">' + $('#ml_description').val() + '</td><td>' + $('#meeting_l_start_date').val() + '</td><td>' + $('#meeting_l_finish_date').val() + '</td><td class="tableUsers">' + $('#responsibleUser option:selected').text().split(' (')[0] + '</td><td class="tableUsers">' + $('#mlFollowerUser option:selected').text().split(' (')[0] + '</td><td class="tableUsers">' + $('#identifierUser option:selected').text().split(' (')[0] + '</td><td>Qaralama</td></tr>';
                $('.meeting_line_add_table tbody').append(tbody);
                eindex++;

                $('#ml_add .panel-options a').click();
                setTimeout(function () {
                    $('#ml_add').remove();
                    //$('.publish_meeting').show();
                }, 500);
            }

            if ($('#mlp').data('addml') == 1) {
                $('.publish_meeting').show();
            }


        }
    });

    var perm = parseInt($('#mlp').data('id'));

    function meeting_line_add_ajax(e) {
        if (perm == 1)
            $.ajax({
                method: 'get',
                url: '/MeetingLine/MeetingLineAddForm',
                success: function (result) {
                    if ($('#ml_add').length < 1) {

                        $(result).insertBefore('.publish_meeting');

                        $('#ml_add .select2c').select2();

                        $('#mlDepartment').select2(departments);
                        $('#responsibleUser').select2(userObject);
                        $('#identifierUser').select2(userObject);
                        $('#mlFollowerUser').select2(userObject);
                        $('#informedUserMl').select2(userObject);

                        $('#ml_add .datepicker').datepicker({
                            format: 'dd/mm/yyyy'
                        });

                        $('#mlTags').tagsinput();

                        //$('.publish_meeting').hide();
                    }
                }
            });
    }

    $(document).on('click', '.meeting_l_si', function () {
        $('#meeting_l_start_date').trigger('focus');
    });

    $(document).on('click', '.meeting_l_fi', function () {
        $('#meeting_l_finish_date').trigger('focus');
    });

    $(document).on('click', '.new_meeting_line_option>a', function (e) {
        if ($('#ml_add').length == 0)
            meeting_line_add_ajax(e);


        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('click', '.publish_meeting a', function (e) {


        $('#modal-7').modal('show', { backdrop: 'static' });
        $('#modal-7 .ajax_modal_7 span').html('');
        setTimeout(function (e) {
            if ($('#mlp').data('addml') != 1) {
                $.ajax({
                    url: '/Meeting/Status/' + $('.meeting_add').data('id'),
                    method: 'get'
                });
            }

            if ($('.meeting_line_add_table tbody tr').length > 0) {
                var form_data = new FormData();

                $.each($('.meeting_line_add_table tbody tr'), function (index, item) {
                    if ($($(item).find('.ml_check')).prop('checked'))
                        form_data.append('ids', $(item).data('id'));
                });

                $.ajax({
                    url: '/MeetingLine/StatusMulti',
                    method: 'post',
                    contentType: false,
                    processData: false,
                    data: form_data,
                    cache: false,
                    success: function () {
                        window.location.href = "/Task/AllTasks";
                    }
                })

            }
        }, 500);



        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('click', '.delete_ml', function (e) {

        var index = $(this).data('id');

        ml_type.splice(index, 1);
        mlDepartment.splice(index, 1);
        mlTags.splice(index, 1);
        ml_description.splice(index, 1);
        responsibleUser.splice(index, 1);
        identifierUser.splice(index, 1);
        mlFollowerUser.splice(index, 1);
        informedUserMl.splice(index, 1);
        meeting_l_start_date.splice(index, 1);
        meeting_l_finish_date.splice(index, 1);
        mlFile.splice(index, 1);
        mlFilePos.splice(index, 1);

        $($('.meeting_line_add_table tbody tr')[index]).remove();

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    $(document).on('change', '.fileinput input', function (e) {
        $($($($($(this).parent()).parent()).parent()).find('.remove_file')).remove();

        $($($($(this).parent()).parent()).parent()).append('<div class="remove_file"><div><span>' + e.target.files[0].name + '</span><a href="javascript:void(0)"><i class="fa fa-trash-o"></i></a></div></div>');

        if ($('.meeting_add').attr('data-type') == 1)
            fileChange = true;

        if ($('#ml_edit_file').length == 1)
            $('#ml_edit_file').addClass('fileChange');

    });

    $(document).on('click', '.remove_file a', function (e) {

        $($($($($(this).parent()).parent()).parent()).find('input')).val('');
        $($($(this).parent()).parent()).remove();

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });

    function closeForm(k) {
        if (k == 0) {
            $('.meeting_add .panel-options a').click();
        }
        else if (k == 1) {
            $('#ml_add').remove();
            //$('.publish_meeting').show();
        }
        else if (k == 2) {
            $('#ml_update').remove();
            //if ($('#ml_add').length == 0)
            //    $('.publish_meeting').show();
            //else
            //    $('#ml_add .panel-options>a')

            if ($('#ml_add').length != 0)
                $('#ml_add .panel-options>a')

        }
    }

    var addedMLTagsLength = 0;
    $(document).on('itemAdded', ".ml_up_tags", function (event) {
        addedMlTags.push(event.item);
    })

    $(document).on('itemRemoved', ".ml_up_tags", function (event) {
        if (jQuery.inArray(event.item, addedMlTags.slice(addedMLTagsLength)) == 0) {
            addedMlTags = jQuery.grep(addedMlTags, function (value) {
                return value != event.item;
            });
        }
        else
            deletedMlTags[deletedMlTags.length] = event.item;
    });

    $(document).on('select2:unselecting', '.ml_up_department', function (e) {
        if (addedMlDepartment.includes(parseInt(e.params.args.data.id))) {
            addedMlDepartment = jQuery.grep(addedMlDepartment, function (value) {
                return value != e.params.args.data.id;
            });
        }
        else {
            deletedMlDepartment[deletedMlDepartment.length] = parseInt(e.params.args.data.id);
        }

    });


    $(document).on('select2:select', '.ml_up_department', function (e) {
        addedMlDepartment[addedMlDepartment.length] = parseInt(e.params.data.id);
    });


    $(document).on('click', '.meeting_add .close_button', function () {
        closeForm(0);
    });

    $(document).on('click', '#ml_add .close_button', function () {
        closeForm(1)
    });

    $(document).on('click', '#ml_update .close_button', function () {
        closeForm(2)
    });

    $("#meetingTags").on('itemAdded', function (event) {
        if ($('.meeting_add').attr('data-type') == 1) {
            addedTags[addedTags.length] = event.item;
        }
    });
    $("#meetingTags").on('itemRemoved', function (event) {
        if ($('.meeting_add').attr('data-type') == 1) {
            if (addedTags.includes(event.item)) {
                addedTags = jQuery.grep(addedTags, function (value) {
                    return value != event.item;
                });
            }
            else
                deletedTags[deletedTags.length] = event.item;
        }
    });

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

    var otherParticipantsObject = new Object({
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
        placeholder: "Daxil edin",
        ajax: {
            url: '/Users/GetOtherParticipants',
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

    var countryObject = new Object({
        minimumInputLength: 3,
        tags: [],
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
        placeholder: "Daxil edin",
        ajax: {
            url: '/Country/Countries',
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
                        'text': item.name
                    });
                });
                return {
                    results: myResults
                };
            }

        }
    });

    var places = new Object({
        minimumInputLength: 3,
        tags: [],
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
        placeholder: "Daxil edin",
        ajax: {
            url: '/Place/GetPlaces',
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

    var userDepartments = new Object({
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
            url: '/Department/GetUsersDepartments',
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

    $('#informedUser').select2(userObject);
    $('#ownerUser').select2(userObject);
    $('#followerUser').select2(userObject);
    $('#participants').select2(userObject);
    $('#other_participants').select2(otherParticipantsObject);
    $('#meeting_place').select2(places);
    $('#meetingDepartment').select2(userDepartments);


    $('#ml_add .select2c').select2();

    $('#mlDepartment').select2(departments);
    $('#responsibleUser').select2(userObject);
    $('#identifierUser').select2(userObject);
    $('#mlFollowerUser').select2(userObject);
    $('#informedUserMl').select2(userObject);

    $('#ml_add .datepicker').datepicker({
        format: 'dd/mm/yyyy'
    });

    $('#other_country').select2(countryObject);



    $(document).on('click', '.edit_meeting_line', function (e) {

        $('#ml_add .panel-options>a').click();

        edit_index = $(this).data('id');

        $.ajax({
            type: 'get',
            url: '/MeetingLine/Edit/' + edit_index,
            async: false,
            success: function (result) {
                $($(result).find('#ml_update')).insertBefore('.publish_meeting');

                $('#ml_update .select2c').select2();

                $('.ml_up_department').select2(departments);
                $('.ml_up_resp_user').select2(userObject);
                $('.ml_up_ident_user').select2(userObject);
                $('.ml_up_follower_user').select2(userObject);
                $('.ml_up_inf_user').select2(userObject);

                deletedMlTags = [];
                addedMlTags = [];

                deletedMlDepartment = [];
                addedMlDepartment = [];

                $('.ml_up_tags').tagsinput();
                addedMLTagsLength = $('.ml_up_tags').val().length > 0 ? $('.ml_up_tags').val().split(',').length : 0;


                $('#ml_update .datepicker').datepicker({
                    format: 'dd/mm/yyyy'
                });

                $('#ml_add .panel-options>a').click();

                //$('.publish_meeting').hide();
            }
        })

        eindex = $(this).data('eindex');

        e.preventDefault();
        e.stopImmediatePropagation();
        return false;

    });

    $(document).on('click', '.editMeetingLine', function (e) {

        var checkVal = checkValid('ml_update');

        var edit_index = eindex;
        if (checkVal) {

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


            $.each(deletedMlTags, function (index, item) {
                fd.append('deletedTags', item);
            });

            $.each(addedMlTags.slice(addedMLTagsLength), function (index, item) {
                fd.append('addedTags', item);
            });

            $.each(deletedMlDepartment, function (index, item) {
                fd.append('deletedDepartment', item);
            });

            $.each(addedMlDepartment, function (index, item) {
                fd.append('addedDepartment', item);
            });

            addedMlDepartment = [];
            addedMlTags = [];

            deletedMlDepartment = [];
            deletedMlTags = [];
            addedMLTagsLength = 0;


            fd.append("Id", $('#ml_update').data('id'));

            var checkVal = checkValid('ml_add');

            if (checkVal) {
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
                        success: function (result) {

                            var tr = '<td><input tabindex="5" type="checkbox" class="ml_check icheck-11" checked></td><td><div class="dropdown"><button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown">Əməliyyatlar<span class="caret"></span></button><ul class="dropdown-menu"><li><a href="javascript:void(0)" class="edit_meeting_line" data-eindex=' + eindex + ' data-id=' + result + '>Görüntülə</a></li><li><a href="javascript:void(0)" data-id=' + result + ' class="delete_ml">Sil</a></li></ul></div></td><td>' + $('.ml_up_type option:selected').html() + '</td><td class="table_desc">' + $('.ml_up_description').val() + '</td><td>' + $('.ml_up_s_date').val() + '</td><td>' + $('.ml_up_f_date').val() + '</td><td class="tableUsers">' + $('.ml_up_resp_user option:selected').text().split(' (')[0] + '</td><td class="tableUsers">' + $('.ml_up_follower_user option:selected').text().split(' (')[0] + '</td><td class="tableUsers">' + $('.ml_up_ident_user option:selected').text().split(' (')[0] + '</td><td>Qaralama</td>';

                            $($('.meeting_line_add_table tbody>tr')[edit_index]).html('');

                            $($('.meeting_line_add_table tbody>tr')[edit_index]).append(tr);



                            setTimeout(function (e) {
                                $('#modal-7').modal('hide');
                                closeForm(2);
                            }, 500)
                        }
                    });



                }, 500);

            }
        }



        e.preventDefault();
        e.stopImmediatePropagation();
        return false;
    });


    $(document).on('change', '.first-check', function (e) {
        if ($(this).prop('checked'))
            $('.icheck-11').prop('checked', true);
        else
            $('.icheck-11').prop('checked', false);
    });

    ///end of document ready
});