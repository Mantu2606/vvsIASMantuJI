// Modern reCAPTCHA Configuration
window.RecaptchaConfig = {
    // Site key (you should replace this with your actual reCAPTCHA site key)
    siteKey: '6LcXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX', // Replace with your actual site key
    
    // Configuration options
    options: {
        theme: 'light',
        size: 'normal',
        badge: 'bottomright',
        callback: 'onRecaptchaSuccess',
        'expired-callback': 'onRecaptchaExpired',
        'error-callback': 'onRecaptchaError'
    },
    
    // Custom callbacks
    callbacks: {
        onVerificationComplete: function() {
            console.log('reCAPTCHA verification completed successfully!');
        },
        
        onFormSubmit: function(isValid) {
            if (isValid) {
                console.log('Form submission allowed - reCAPTCHA verified');
            } else {
                console.log('Form submission blocked - reCAPTCHA not verified');
            }
        },
        
        onError: function(error) {
            console.error('reCAPTCHA error:', error);
        }
    },
    
    // Validation settings
    validation: {
        requireVerification: true,
        showErrorMessages: true,
        autoVerify: false,
        timeout: 30000 // 30 seconds timeout
    },
    
    // UI settings
    ui: {
        animationDuration: 300,
        showProgressRing: true,
        showSuccessAnimation: true,
        showErrorAnimation: true,
        enableHoverEffects: true,
        enableTouchEffects: true
    }
};

// Initialize reCAPTCHA when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    if (typeof window.RecaptchaConfig !== 'undefined') {
        console.log('reCAPTCHA configuration loaded');
    }
});
