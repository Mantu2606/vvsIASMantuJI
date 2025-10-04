// Modern reCAPTCHA Implementation
class ModernRecaptcha {
    constructor() {
        this.isVerified = false;
        this.isVerifying = false;
        this.verificationTimeout = null;
        this.init();
    }

    init() {
        this.bindEvents();
        this.setupAccessibility();
        this.initializeUI();
    }

    bindEvents() {
        // reCAPTCHA checkbox click
        const checkbox = document.getElementById('recaptchaCheckbox');
        if (checkbox) {
            checkbox.addEventListener('click', (e) => this.handleCheckboxClick(e));
            checkbox.addEventListener('keydown', (e) => this.handleKeydown(e));
        }

        // Form submission
        const form = document.getElementById('contactForm');
        if (form) {
            form.addEventListener('submit', (e) => this.handleFormSubmit(e));
        }

        // Privacy and Terms links
        const privacyLink = document.querySelector('.recaptcha-privacy');
        const termsLink = document.querySelector('.recaptcha-terms');
        
        if (privacyLink) {
            privacyLink.addEventListener('click', (e) => this.handlePrivacyClick(e));
        }
        
        if (termsLink) {
            termsLink.addEventListener('click', (e) => this.handleTermsClick(e));
        }

        // Touch events for mobile
        if ('ontouchstart' in window) {
            checkbox?.addEventListener('touchstart', (e) => this.handleTouchStart(e));
            checkbox?.addEventListener('touchend', (e) => this.handleTouchEnd(e));
        }
    }

    setupAccessibility() {
        const wrapper = document.getElementById('recaptchaWrapper');
        const checkbox = document.getElementById('recaptchaCheckbox');
        
        if (wrapper && checkbox) {
            wrapper.setAttribute('role', 'checkbox');
            wrapper.setAttribute('aria-checked', 'false');
            wrapper.setAttribute('tabindex', '0');
            wrapper.setAttribute('aria-label', 'I\'m not a robot verification');
            
            // Add screen reader text
            const srText = document.createElement('span');
            srText.className = 'sr-only';
            srText.textContent = 'Click to verify you are not a robot';
            wrapper.appendChild(srText);
        }
    }

    initializeUI() {
        this.updateUI();
        this.setupProgressRing();
    }

    setupProgressRing() {
        const circle = document.querySelector('.progress-ring-circle');
        if (circle) {
            const radius = circle.r.baseVal.value;
            const circumference = radius * 2 * Math.PI;
            
            circle.style.strokeDasharray = `${circumference} ${circumference}`;
            circle.style.strokeDashoffset = circumference;
            
            this.setProgress = (percent) => {
                const offset = circumference - (percent / 100 * circumference);
                circle.style.strokeDashoffset = offset;
            };
        }
    }

    handleCheckboxClick(e) {
        e.preventDefault();
        e.stopPropagation();
        
        if (this.isVerifying || this.isVerified) {
            return;
        }

        this.startVerification();
    }

    handleKeydown(e) {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            this.handleCheckboxClick(e);
        }
    }

    handleTouchStart(e) {
        const checkbox = document.getElementById('recaptchaCheckbox');
        checkbox?.classList.add('touch-active');
    }

    handleTouchEnd(e) {
        const checkbox = document.getElementById('recaptchaCheckbox');
        setTimeout(() => {
            checkbox?.classList.remove('touch-active');
        }, 100);
    }

    handleFormSubmit(e) {
        if (!this.isVerified && window.RecaptchaConfig?.validation?.requireVerification) {
            e.preventDefault();
            this.showError('Please complete the reCAPTCHA verification before submitting.');
            this.shakeElement();
            return false;
        }
        
        // Show loading state
        const submitBtn = document.getElementById('submitBtn');
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.querySelector('.btn-text').style.display = 'none';
            submitBtn.querySelector('.btn-loading').style.display = 'inline';
        }
        
        return true;
    }

    handlePrivacyClick(e) {
        e.preventDefault();
        window.open('https://policies.google.com/privacy', '_blank');
    }

    handleTermsClick(e) {
        e.preventDefault();
        window.open('https://policies.google.com/terms', '_blank');
    }

    startVerification() {
        this.isVerifying = true;
        this.updateUI();
        
        // Simulate verification process
        this.simulateVerification();
        
        // Set timeout
        this.verificationTimeout = setTimeout(() => {
            if (this.isVerifying) {
                this.handleVerificationTimeout();
            }
        }, window.RecaptchaConfig?.validation?.timeout || 30000);
    }

    simulateVerification() {
        const progressSteps = [0, 25, 50, 75, 100];
        let currentStep = 0;
        
        const progressInterval = setInterval(() => {
            if (currentStep < progressSteps.length) {
                this.setProgress(progressSteps[currentStep]);
                currentStep++;
            } else {
                clearInterval(progressInterval);
                this.completeVerification();
            }
        }, 200);
    }

    completeVerification() {
        clearTimeout(this.verificationTimeout);
        
        this.isVerifying = false;
        this.isVerified = true;
        
        // Generate a fake reCAPTCHA response token
        const fakeToken = this.generateFakeToken();
        document.getElementById('recaptchaResponse').value = fakeToken;
        
        this.updateUI();
        this.showSuccess();
        
        // Call custom callback
        if (window.RecaptchaConfig?.callbacks?.onVerificationComplete) {
            window.RecaptchaConfig.callbacks.onVerificationComplete();
        }
    }

    handleVerificationTimeout() {
        this.isVerifying = false;
        this.updateUI();
        this.showError('Verification timed out. Please try again.');
        this.shakeElement();
        
        if (window.RecaptchaConfig?.callbacks?.onError) {
            window.RecaptchaConfig.callbacks.onError('timeout');
        }
    }

    generateFakeToken() {
        // Generate a fake reCAPTCHA response token for demo purposes
        // In production, this would be replaced with actual Google reCAPTCHA
        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        let result = '';
        for (let i = 0; i < 100; i++) {
            result += chars.charAt(Math.floor(Math.random() * chars.length));
        }
        return result;
    }

    updateUI() {
        const wrapper = document.getElementById('recaptchaWrapper');
        const checkbox = document.getElementById('recaptchaCheckbox');
        const statusText = document.querySelector('.status-text');
        const statusIndicator = document.querySelector('.status-indicator');
        
        if (!wrapper || !checkbox) return;
        
        // Remove all states
        wrapper.classList.remove('verified', 'verifying', 'error');
        checkbox.classList.remove('checked', 'verifying', 'error');
        
        if (this.isVerified) {
            wrapper.classList.add('verified');
            checkbox.classList.add('checked');
            if (statusText) statusText.textContent = 'Verified';
            if (statusIndicator) statusIndicator.style.background = '#28a745';
        } else if (this.isVerifying) {
            wrapper.classList.add('verifying');
            checkbox.classList.add('verifying');
            if (statusText) statusText.textContent = 'Verifying...';
            if (statusIndicator) statusIndicator.style.background = '#ffc107';
        } else {
            if (statusText) statusText.textContent = 'Click to verify';
            if (statusIndicator) statusIndicator.style.background = '#ddd';
        }
        
        // Update ARIA attributes
        wrapper.setAttribute('aria-checked', this.isVerified.toString());
    }

    showSuccess() {
        const checkbox = document.getElementById('recaptchaCheckbox');
        if (checkbox) {
            checkbox.classList.add('recaptcha-success');
            setTimeout(() => {
                checkbox.classList.remove('recaptcha-success');
            }, 800);
        }
    }

    showError(message) {
        const errorDiv = document.getElementById('recaptchaError');
        if (errorDiv) {
            errorDiv.querySelector('span').textContent = message;
            errorDiv.style.display = 'flex';
            
            setTimeout(() => {
                errorDiv.style.display = 'none';
            }, 5000);
        }
        
        if (window.RecaptchaConfig?.callbacks?.onError) {
            window.RecaptchaConfig.callbacks.onError(message);
        }
    }

    shakeElement() {
        const wrapper = document.getElementById('recaptchaWrapper');
        if (wrapper) {
            wrapper.classList.add('shake');
            setTimeout(() => {
                wrapper.classList.remove('shake');
            }, 600);
        }
    }

    reset() {
        this.isVerified = false;
        this.isVerifying = false;
        clearTimeout(this.verificationTimeout);
        document.getElementById('recaptchaResponse').value = '';
        this.updateUI();
    }

    // Public methods
    getVerificationStatus() {
        return this.isVerified;
    }

    getResponseToken() {
        return document.getElementById('recaptchaResponse').value;
    }
}

// Global callbacks for reCAPTCHA
window.onRecaptchaSuccess = function() {
    console.log('reCAPTCHA success callback');
};

window.onRecaptchaExpired = function() {
    console.log('reCAPTCHA expired callback');
    if (window.modernRecaptcha) {
        window.modernRecaptcha.reset();
    }
};

window.onRecaptchaError = function() {
    console.log('reCAPTCHA error callback');
    if (window.modernRecaptcha) {
        window.modernRecaptcha.showError('Verification failed. Please try again.');
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.modernRecaptcha = new ModernRecaptcha();
    console.log('Modern reCAPTCHA initialized');
});
