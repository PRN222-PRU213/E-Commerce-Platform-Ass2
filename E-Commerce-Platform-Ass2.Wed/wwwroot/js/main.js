(function ($) {
    "use strict";
    
    // Dropdown on mouse hover (exclude user profile dropdown - click only)
    $(document).ready(function () {
        function toggleNavbarMethod() {
            if ($(window).width() > 992) {
                // Only apply hover to navbar dropdowns, NOT user profile dropdown
                $('.navbar .dropdown:not(.user-profile-dropdown)').on('mouseover', function () {
                    $('.dropdown-toggle', this).trigger('click');
                }).on('mouseout', function () {
                    $('.dropdown-toggle', this).trigger('click').blur();
                });
            } else {
                $('.navbar .dropdown').off('mouseover').off('mouseout');
            }
        }
        toggleNavbarMethod();
        $(window).resize(toggleNavbarMethod);
        
        // User profile dropdown - click only (no hover, disable Bootstrap auto-toggle)
        $('.user-profile-dropdown .dropdown-toggle').off('click.bs.dropdown'); // Remove Bootstrap's default handler
        
        $('.user-profile-dropdown .dropdown-toggle').on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            
            var $dropdown = $(this).closest('.user-profile-dropdown');
            var $menu = $dropdown.find('.dropdown-menu');
            var isOpen = $dropdown.hasClass('show');
            
            // Close all user profile dropdowns first
            $('.user-profile-dropdown').removeClass('show');
            $('.user-profile-dropdown .dropdown-menu').removeClass('show');
            
            // Toggle current dropdown if it wasn't open
            if (!isOpen) {
                $dropdown.addClass('show');
                $menu.addClass('show');
            }
        });
        
        // Close dropdown when clicking outside
        $(document).on('click', function (e) {
            if (!$(e.target).closest('.user-profile-dropdown').length) {
                $('.user-profile-dropdown').removeClass('show');
                $('.user-profile-dropdown .dropdown-menu').removeClass('show');
            }
        });
    });
    
    
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });


    // Vendor carousel
    $('.vendor-carousel').owlCarousel({
        loop: true,
        margin: 29,
        nav: false,
        autoplay: true,
        smartSpeed: 1000,
        responsive: {
            0:{
                items:2
            },
            576:{
                items:3
            },
            768:{
                items:4
            },
            992:{
                items:5
            },
            1200:{
                items:6
            }
        }
    });


    // Related carousel
    $('.related-carousel').owlCarousel({
        loop: true,
        margin: 29,
        nav: false,
        autoplay: true,
        smartSpeed: 1000,
        responsive: {
            0:{
                items:1
            },
            576:{
                items:2
            },
            768:{
                items:3
            },
            992:{
                items:4
            }
        }
    });
    
})(jQuery);

