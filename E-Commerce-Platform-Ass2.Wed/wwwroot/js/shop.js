    // Quantity increase/decrease functionality
$(document).ready(function() {
    // Increase quantity
    $('.btn-plus').on('click', function(e) {
        e.preventDefault();
        var $input = $(this).closest('.quantity').find('input');
        var val = parseInt($input.val());
        $input.val(val + 1);
    });

    // Decrease quantity
    $('.btn-minus').on('click', function(e) {
        e.preventDefault();
        var $input = $(this).closest('.quantity').find('input');
        var val = parseInt($input.val());
        if (val > 1) {
            $input.val(val - 1);
        }
    });

    // Price filter
    $('input[type="checkbox"][id^="price-"]').on('change', function() {
        if (this.id === 'price-all' && this.checked) {
            $('input[type="checkbox"][id^="price-"]').not(this).prop('checked', false);
        } else if (this.id !== 'price-all' && this.checked) {
            $('#price-all').prop('checked', false);
        }
        // AJAX call can be added here to filter products
        filterProducts();
    });

    // Color filter
    $('input[type="checkbox"][id^="color-"]').on('change', function() {
        if (this.id === 'color-all' && this.checked) {
            $('input[type="checkbox"][id^="color-"]').not(this).prop('checked', false);
        } else if (this.id !== 'color-all' && this.checked) {
            $('#color-all').prop('checked', false);
        }
        // AJAX call can be added here to filter products
        filterProducts();
    });

    // Size filter
    $('input[type="checkbox"][id^="size-"]').on('change', function() {
        if (this.id === 'size-all' && this.checked) {
            $('input[type="checkbox"][id^="size-"]').not(this).prop('checked', false);
        } else if (this.id !== 'size-all' && this.checked) {
            $('#size-all').prop('checked', false);
        }
        // AJAX call can be added here to filter products
        filterProducts();
    });

    // Rating stars functionality
    $('.text-primary i').on('click', function() {
        var index = $(this).index();
        $(this).parent().find('i').each(function(i) {
            if (i <= index) {
                $(this).removeClass('far fa-star').addClass('fas fa-star');
            } else {
                $(this).removeClass('fas fa-star').addClass('far fa-star');
            }
        });
    });
});

// Product filter function
function filterProducts() {
    // Collect all selected filter conditions
    var priceFilters = [];
    $('input[type="checkbox"][id^="price-"]:checked').not('#price-all').each(function() {
        priceFilters.push($(this).attr('id'));
    });

    var colorFilters = [];
    $('input[type="checkbox"][id^="color-"]:checked').not('#color-all').each(function() {
        colorFilters.push($(this).attr('id'));
    });

    var sizeFilters = [];
    $('input[type="checkbox"][id^="size-"]:checked').not('#size-all').each(function() {
        sizeFilters.push($(this).attr('id'));
    });

    // AJAX call can be added here to send filter request to server
    console.log('Price filters:', priceFilters);
    console.log('Color filters:', colorFilters);
    console.log('Size filters:', sizeFilters);

    // Example: Use AJAX to call backend API
    /*
    $.ajax({
        url: '/Shop/FilterProducts',
        type: 'POST',
        data: {
            priceFilters: priceFilters,
            colorFilters: colorFilters,
            sizeFilters: sizeFilters
        },
        success: function(response) {
            // Update product list
            $('.row.pb-3').html(response);
        },
        error: function(error) {
            console.error('Filter failed:', error);
        }
    });
    */
}
