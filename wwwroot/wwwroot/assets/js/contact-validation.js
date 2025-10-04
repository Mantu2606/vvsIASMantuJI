// Contact Form Validation
$(document).ready(function() {
    // Form validation and submission handling
    $('#contactForm').on('submit', function(e) {
        // Check if reCAPTCHA is verified
        if (!window.modernRecaptcha || !window.modernRecaptcha.getVerificationStatus()) {
            e.preventDefault();
            window.modernRecaptcha.showError('Please complete the reCAPTCHA verification before submitting.');
            window.modernRecaptcha.shakeElement();
            return false;
        }

        // Show loading state
        const submitBtn = $('#submitBtn');
        submitBtn.prop('disabled', true);
        submitBtn.find('.btn-text').hide();
        submitBtn.find('.btn-loading').show();
    });

    // Real-time validation feedback
    $('.form-control').on('blur', function() {
        const field = $(this);
        const fieldName = field.attr('name');
        const value = field.val().trim();
        
        // Remove existing validation classes
        field.removeClass('is-valid is-invalid');
        
        // Validate based on field type
        let isValid = true;
        
        if (fieldName === 'Name') {
            isValid = /^[a-zA-Z\s]+$/.test(value) && value.length >= 2;
        } else if (fieldName === 'Email') {
            // Simple email validation
            var atIndex = value.indexOf('@');
            var dotIndex = value.indexOf('.');
            isValid = atIndex > 0 && dotIndex > atIndex + 1 && dotIndex < value.length - 1;
        } else if (fieldName === 'PhoneNumber') {
            // Simple phone validation - only digits and exactly 10 characters
            isValid = /^\d{10}$/.test(value);
        } else if (fieldName === 'Message') {
            isValid = value.length >= 5;
        }
        
        // Add validation class
        if (isValid && value) {
            field.addClass('is-valid');
        } else if (!isValid && value) {
            field.addClass('is-invalid');
        }
    });
});
