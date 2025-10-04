/*=====================================================
Template Name   : Farnio
Description     : Furniture Store HTML5 Template
Author          : Themesland
Version         : 1.0
=======================================================*/


(function ($) {
    
    "use strict";

    // preloader
    $(window).on('load', function () {
        $(".preloader").fadeOut("slow");
    });


    // navbar fixed top
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.navbar').addClass("fixed-top");
        } else {
            $('.navbar').removeClass("fixed-top");
        }
    });


    // category all 
    $('.category-btn').on('click',function(){
        $(".main-category").toggle();
    });


    // header Search
    if ($('.search-box-outer').length) {
        $('.search-box-outer').on('click', function (e) {
            e.preventDefault();
            $('body').addClass('search-active');
            // Focus on search input after animation
            setTimeout(function() {
                $('#header-search-input').focus();
            }, 300);
        });
        $('.close-search').on('click', function () {
            $('body').removeClass('search-active');
            // Clear search input when closing
            $('#header-search-input').val('');
            $('#search-suggestions').addClass('d-none');
        });
        
        // Close search on escape key
        $(document).on('keydown', function(e) {
            if (e.key === 'Escape' && $('body').hasClass('search-active')) {
                $('body').removeClass('search-active');
                $('#header-search-input').val('');
                $('#search-suggestions').addClass('d-none');
            }
        });
        
        // Close search when clicking outside
        $(document).on('click', function(e) {
            if ($('body').hasClass('search-active') && 
                !$(e.target).closest('.search-popup, .search-box-outer, #search-suggestions').length) {
                $('body').removeClass('search-active');
                $('#header-search-input').val('');
                $('#search-suggestions').addClass('d-none');
            }
        });
    }


    // multi level dropdown menu
    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass('show');
        }
        var $subMenu = $(this).next('.dropdown-menu');
        $subMenu.toggleClass('show');

        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
            $('.dropdown-submenu .show').removeClass('show');
        });
        return false;
    });

    // navbar dropdown-right
    $(window).resize(function() {
        let ndr = $('.navbar-nav .nav-item.dropdown').slice(-2);
        if ($(window).width() > 991 && $(window).width() < 1199){
            ndr.addClass("dropdown-right");
        }else{
            ndr.removeClass("dropdown-right");
        }
    });
    


    // data-background    
    $(document).on('ready', function () {
        $("[data-background]").each(function () {
            $(this).css("background-image", "url(" + $(this).attr("data-background") + ")");
        });
    });


    // wow init
    new WOW().init();


    // hero slider
    $('.hero-slider').owlCarousel({
        loop: true,
        nav: true,
        dots: true,
        margin: 0,
        autoplay: true,
        autoplayHoverPause: true,
        autoplayTimeout: 5000,
        items: 1,
        navText: [
            "<i class='far fa-angle-left'></i>",
            "<i class='far fa-angle-right'></i>"
        ],

        onInitialized: function(event) {
        var $firstAnimatingElements = $('.hero-slider .owl-item').eq(event.item.index).find("[data-animation]");
        doAnimations($firstAnimatingElements);
        },

        onChanged: function(event){
        var $firstAnimatingElements = $('.hero-slider .owl-item').eq(event.item.index).find("[data-animation]");
        doAnimations($firstAnimatingElements);
        }
    });

    //hero slider do animations
    function doAnimations(elements) {
		var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
		elements.each(function () {
			var $this = $(this);
			var $animationDelay = $this.data('delay');
			var $animationDuration = $this.data('duration');
			var $animationType = 'animated ' + $this.data('animation');
			$this.css({
				'animation-delay': $animationDelay,
				'-webkit-animation-delay': $animationDelay,
                'animation-duration': $animationDuration,
                '-webkit-animation-duration': $animationDuration,
			});
			$this.addClass($animationType).one(animationEndEvents, function () {
				$this.removeClass($animationType);
			});
		});
	}


    // product slider
    $('.product-slider').owlCarousel({
        items: 5,
        loop: false,
        margin: 15,
        smartSpeed: 400,
        nav: true,
        autoplay: false,
        autoplayHoverPause: true,
        dots: false,
        navText: [
            "<i class='far fa-angle-left'></i>",
            "<i class='far fa-angle-right'></i>"
        ],
        responsive:{
            0:{
                items: 1
            },
            600:{
                items: 2
            },
            1000:{
                items: 3
            },
            1200:{
                items: 5
            }
        }
    });


    // product slider 2
    $('.product-slider2').owlCarousel({
        items: 3,
        loop: true,
        margin: 20,
        smartSpeed: 400,
        nav: true,
        autoplay: false,
        autoplayHoverPause: true,
        dots: false,
        navText: [
            "<i class='far fa-angle-left'></i>",
            "<i class='far fa-angle-right'></i>"
        ],
        responsive:{
            0:{
                items: 1
            },
            600:{
                items: 2
            },
            1000:{
                items: 3
            },
            1200:{
                items: 3
            }
        }
    });


    // deal slider
    $('.deal-slider').owlCarousel({
        items: 1,
        loop: true,
        margin: 15,
        smartSpeed: 400,
        nav: false,
        dots: true,
        autoplayHoverPause: true,
        autoplay: false,
        navText: [
            "<i class='far fa-angle-left'></i>",
            "<i class='far fa-angle-right'></i>"
        ],
        responsive:{
            0:{
                items: 1
            },
            600:{
                items: 1
            },
            1000:{
                items: 1
            }
        }
    });


    // testimonial slider
    $('.testimonial-slider').owlCarousel({
        loop: true,
        margin: 20,
        nav: false,
        dots: true,
        autoplay: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 2
            },
            1000: {
                items: 3
            },
            1400: {
                items: 4
            }
        }
    });


    // brand slider
    $('.brand-slider').owlCarousel({
        loop: true,
        margin: 40,
        nav: false,
        dots: false,
        autoplay: false,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 2
            },
            600: {
                items: 3
            },
            1000: {
                items: 6
            }
        }
    });


    // category slider
    $('.category-slider').owlCarousel({
        loop: false,
        margin: 20,
        nav: false,
        dots: false,
        autoplay: false,
        responsive: {
            0: {
                items: 2
            },
            600: {
                items: 3
            },
            1000: {
                items: 4
            },
            1200: {
                items: 4
            },
            1400: {
                items: 4
            }
        }
    });


    // instagram-slider
    $('.instagram-slider').owlCarousel({
        loop: true,
        margin: 20,
        nav: false,
        dots: false,
        autoplay: true,
        responsive: {
            0: {
                items: 2
            },
            600: {
                items: 3
            },
            1000: {
                items: 5
            }
        }
    });
    


    // fun fact counter
    $('.counter').countTo();
    $('.counter-box').appear(function () {
        $('.counter').countTo();
    }, {
        accY: -100
    });


    // magnific popup init
    $(".popup-gallery").magnificPopup({
        delegate: '.popup-img',
        type: 'image',
        gallery: {
            enabled: true
        },
    });

    $(".popup-youtube, .popup-vimeo, .popup-gmaps").magnificPopup({
        type: "iframe",
        mainClass: "mfp-fade",
        removalDelay: 160,
        preloader: false,
        fixedContentPos: false
    });


    // scroll to top
    $(window).scroll(function () {
        if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
            $("#scroll-top").addClass('active');
        } else {
            $("#scroll-top").removeClass('active');
        }
    });

    $("#scroll-top").on('click', function () {
        $("html, body").animate({ scrollTop: 0 }, 1500);
        return false;
    });


    // countdown
    $('[data-countdown]').each(function() {
        let finalDate = $(this).data('countdown');
        $(this).countdown(finalDate, function(event) {
            $(this).html(event.strftime(
                '<div class="row"><div class="col countdown-item"><h2>%-D</h2><h5>Day%!d</h5></div><div class="col countdown-item"><h2>%H</h2><h5>Hours</h5></div><div class="col countdown-item"><h2>%M</h2><h5>Minutes</h5></div><div class="col countdown-item"><h2>%S</h2><h5>Seconds</h5></div></div>'
            ));
        });
    });


    // copywrite date
    let date = new Date().getFullYear();
    $("#date").html(date);

    // live product search suggestions
    (function initLiveSearch(){
        var $input = $('#header-search-input');
        var $box = $('#search-suggestions');
        if (!$input.length || !$box.length) return;

        var pendingRq = null;
        var activeIndex = -1;
        var items = [];

        function clearSuggestions(){
            items = [];
            activeIndex = -1;
            $box.empty().addClass('d-none');
            $input.attr('aria-expanded', 'false');
        }

        function renderSuggestions(results, keyword){
            if (!results || !results.length){
                clearSuggestions();
                return;
            }
            var html = results.map(function(p, idx){
                var slug = p.slug;
                var url = slug ? ('/Products/' + slug) : ('/Products/' + p.id);
                return (
                    '<a href="'+ url +'" class="search-suggestion-item" role="option" data-index="'+idx+'" data-product-id="'+ p.id +'" data-product-slug="'+ slug +'">'
                    +   '<img class="search-suggestion-thumb" src="'+ (p.image || '/assets/img/placeholder.png') +'" alt="">'
                    +   '<div class="search-suggestion-meta">'
                    +       '<div class="search-suggestion-title">'+ highlightText(p.name, keyword) +'</div>'
                    +       '<div class="search-suggestion-desc">'+ highlightText(p.category || '', keyword) +'</div>'
                    +   '</div>'
                    +   '<div class="search-suggestion-price">'+ formatCurrency(p.finalPrice) +'</div>'
                    +'</a>'
                );
            }).join('');
            html += '<div class="search-suggestions-footer">'
                 +   '<a class="btn btn-sm btn-primary" href="/Products?query='+ encodeURIComponent(keyword) +'">See all results</a>'
                 + '</div>';
            $box.html(html).removeClass('d-none');
            $input.attr('aria-expanded', 'true');
            items = $box.find('.search-suggestion-item');
            activeIndex = -1;
        }

        function escapeHtml(text){
            return (text || '').replace(/[&<>"']/g, function(m){
                return ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;','\'':'&#39;'}[m]);
            });
        }

        function escapeRegex(text){
            return (text || '').replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
        }

        function highlightText(text, keyword){
            var safe = escapeHtml(text || '');
            if (!keyword) return safe;
            try {
                var re = new RegExp(escapeRegex(keyword), 'ig');
                return safe.replace(re, function(m){ return '<mark>' + m + '</mark>'; });
            } catch(e){
                return safe;
            }
        }

        function formatCurrency(val){
            if (val === null || val === undefined) return '';
            try { return new Intl.NumberFormat(undefined, { style:'currency', currency: 'INR' }).format(val); } catch(e){ return val; }
        }

        var debouncedSearch = debounce(function(keyword){
            if (!keyword || keyword.length < 2){
                clearSuggestions();
                return;
            }
            
            // Add loading state
            $('.search-popup .form-group').addClass('loading');
            
            if (pendingRq && pendingRq.abort) pendingRq.abort();
            pendingRq = $.ajax({
                url: '/ProductSearch/Search',
                method: 'GET',
                data: { keyword: keyword }
            }).done(function(res){
                var results = (res && res.services) ? res.services : [];
                renderSuggestions(results, keyword);
            }).fail(function(){
                clearSuggestions();
            }).always(function(){
                // Remove loading state
                $('.search-popup .form-group').removeClass('loading');
            });
        }, 200);

        $input.on('input', function(){
            var v = $(this).val();
            debouncedSearch(v);
        });

        $input.on('keydown', function(e){
            if ($box.hasClass('d-none')) return;
            if (e.key === 'ArrowDown'){
                e.preventDefault();
                activeIndex = Math.min(activeIndex + 1, items.length - 1);
                updateActive();
            } else if (e.key === 'ArrowUp'){
                e.preventDefault();
                activeIndex = Math.max(activeIndex - 1, 0);
                updateActive();
            } else if (e.key === 'Enter'){
                if (activeIndex >= 0 && items[activeIndex]){
                    e.preventDefault();
                    window.location.href = $(items[activeIndex]).attr('href');
                }
            } else if (e.key === 'Escape'){
                clearSuggestions();
            }
        });

        function updateActive(){
            items.removeClass('active');
            if (activeIndex >= 0 && items[activeIndex]){
                $(items[activeIndex]).addClass('active');
            }
        }

        $(document).on('click', function(e){
            if (!$(e.target).closest('#header-search-input, #search-suggestions').length){
                clearSuggestions();
            }
        });

        // Force id-only navigation to avoid legacy /Products/Details/... links
        $(document).on('click touchstart', '.search-suggestion-item', function(e){
            e.preventDefault();
            var slug = $(this).data('product-slug');
            var id = $(this).data('product-id');
            var href = $(this).attr('href') || '';

            // If slug is provided via data attribute, prefer it
            if (slug) {
                window.location.href = '/Products/' + slug;
                return;
            }

            // Try to extract slug or id from href
            var m = href.match(/\/Products\/(?:Details\/)?([^/?#]+)/i);
            if (m && m[1]) {
                var token = m[1];
                // If token is not purely digits, treat as slug
                if (/\D/.test(token)) {
                    window.location.href = '/Products/' + token;
                    return;
                }
                // If token is numeric, fall back to id route
                window.location.href = '/Products/' + token;
                return;
            }

            // Last fallback: if only id is present
            if (id) {
                window.location.href = '/Products/' + id;
                return;
            }

            // Fallback to original href if nothing else
            window.location.href = href;
        });
        
        // Add touch feedback for mobile
        $(document).on('touchstart', '.search-suggestion-item', function(){
            $(this).addClass('active');
        });
        
        $(document).on('touchend touchcancel', '.search-suggestion-item', function(){
            $(this).removeClass('active');
        });

        function debounce(fn, wait){
            var t; return function(){
                var args = arguments, ctx = this;
                clearTimeout(t);
                t = setTimeout(function(){ fn.apply(ctx, args); }, wait);
            };
        }
    })();


    // nice select
    $('.select').niceSelect();


    // price range slider - handled in individual views for better customization



    // flexslider
    if ($('.flexslider-thumbnails').length) {
        $('.flexslider-thumbnails').flexslider({
            animation: "slide",
            controlNav: "thumbnails",
        });
    }


    // bootstrap tooltip enable
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-tooltip="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })


    // profile image btn
    $(".profile-img-btn").on('click', function () {
        $(".profile-img-file").click(); 
    });
    
    
    // message bottom scroll
    if ($('.message-content-info').length) {
        $(function () {
            var chatbox = $('.message-content-info');
            var chatheight = chatbox[0].scrollHeight;
            chatbox.scrollTop(chatheight);
        });
    }


    // modal popup banner
    $(window).on('load', function () {
        setTimeout(function () {
            $("#popup-banner").modal("show");
        }, 3000)
    });


    // invoice print
    $('.invoice-print-btn').click(function(){
        $('.invoice-print-btn').hide();
        $('.invoice-container').removeClass('not-print');
        window.print();
        $('.invoice-container').addClass('not-print');
        $('.invoice-print-btn').show();
   });


})(jQuery);

// Description tables: add data-labels from thead ths for better mobile UX
(function(){
    try {
        var containers = document.querySelectorAll('.description-body');
        if (!containers || !containers.length) return;

        containers.forEach(function(container){
            container.querySelectorAll('table').forEach(function(table){
                var headerTexts = [];
                var thead = table.querySelector('thead');
                if (thead) {
                    headerTexts = Array.prototype.map.call(
                        thead.querySelectorAll('th'),
                        function(th){ return (th.textContent || '').trim(); }
                    );
                }

                table.querySelectorAll('tbody tr').forEach(function(tr){
                    var cells = tr.querySelectorAll('td');
                    cells.forEach(function(td, idx){
                        if (!td.hasAttribute('data-label')){
                            var label = headerTexts[idx] || '';
                            if (label) td.setAttribute('data-label', label);
                        }
                    });
                });
            });
        });
    } catch(e) { /* no-op */ }
})();










