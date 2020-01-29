$(document).on('click', '.custom_switch_button', function (e) {
    if ($($($(this).parent()).find('div')).hasClass('custom_switch_passive')) {
        $($($(this).parent()).find('div')).addClass('custom_switch_active');
        $($($(this).parent()).find('div')).removeClass('custom_switch_passive');
        $($($(this).parent()).find('input')).prop('checked', true);
        $(this).attr('data-active', 1);
    }
    else {
        $($($(this).parent()).find('div')).addClass('custom_switch_passive');
        $($($(this).parent()).find('div')).removeClass('custom_switch_active');
        $($($(this).parent()).find('input')).prop('checked', false);
        $(this).attr('data-active', 0);
    }
    //e.preventDefault();
    //e.stopImmediatePropagation();
    //return false;
});