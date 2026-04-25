/* ====================================
   Form Validation & Feedback
   ==================================== */

/**
 * Client-side Form Validation
 * Handles form submission and field validation
 */

class FormValidator {
    constructor(formElement) {
        this.form = formElement;
        this.fields = {};
        this.init();
    }

    init() {
        if (!this.form) return;

        // Collect all form fields
        this.form.querySelectorAll('[name]').forEach(field => {
            this.fields[field.name] = {
                element: field,
                value: field.value,
                isValid: true,
                errors: []
            };

            // Add event listeners
            field.addEventListener('blur', () => this.validateField(field.name));
            field.addEventListener('change', () => this.validateField(field.name));
        });

        // Form submission
        this.form.addEventListener('submit', (e) => {
            if (!this.validate()) {
                e.preventDefault();
                this.showErrors();
            }
        });
    }

    validateField(fieldName) {
        const field = this.fields[fieldName];
        if (!field) return true;

        const element = field.element;
        field.errors = [];

        // Required validation
        if (element.hasAttribute('required') && !element.value.trim()) {
            field.errors.push('این فیلد الزامی است');
        }

        // Email validation
        if (element.type === 'email' && element.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(element.value)) {
                field.errors.push('فرمت ایمیل نامعتبر است');
            }
        }

        // Min length validation
        if (element.hasAttribute('minlength')) {
            const minLength = parseInt(element.getAttribute('minlength'));
            if (element.value.length < minLength) {
                field.errors.push(`حداقل ${minLength} کاراکتر مورد نیاز است`);
            }
        }

        // Max length validation
        if (element.hasAttribute('maxlength')) {
            const maxLength = parseInt(element.getAttribute('maxlength'));
            if (element.value.length > maxLength) {
                field.errors.push(`حداکثر ${maxLength} کاراکتر مجاز است`);
            }
        }

        // Pattern validation
        if (element.hasAttribute('pattern')) {
            const pattern = new RegExp(element.getAttribute('pattern'));
            if (element.value && !pattern.test(element.value)) {
                field.errors.push('فرمت وارد شده نامعتبر است');
            }
        }

        // Number range
        if (element.type === 'number') {
            if (element.hasAttribute('min')) {
                const min = parseFloat(element.getAttribute('min'));
                if (parseFloat(element.value) < min) {
                    field.errors.push(`حداقل مقدار ${min} است`);
                }
            }
            if (element.hasAttribute('max')) {
                const max = parseFloat(element.getAttribute('max'));
                if (parseFloat(element.value) > max) {
                    field.errors.push(`حداکثر مقدار ${max} است`);
                }
            }
        }

        field.isValid = field.errors.length === 0;

        // Update UI
        this.updateFieldUI(fieldName);

        return field.isValid;
    }

    updateFieldUI(fieldName) {
        const field = this.fields[fieldName];
        const element = field.element;
        const wrapper = element.closest('.form-group') || element.parentElement;

        // Remove old error messages
        const oldError = wrapper.querySelector('.invalid-feedback');
        if (oldError) oldError.remove();

        // Add new error message
        if (!field.isValid) {
            element.classList.add('is-invalid');
            element.classList.remove('is-valid');

            const errorDiv = document.createElement('div');
            errorDiv.className = 'invalid-feedback';
            errorDiv.textContent = field.errors[0];
            wrapper.appendChild(errorDiv);
        } else {
            element.classList.remove('is-invalid');
            element.classList.add('is-valid');
        }
    }

    validate() {
        let isValid = true;
        for (const fieldName in this.fields) {
            if (!this.validateField(fieldName)) {
                isValid = false;
            }
        }
        return isValid;
    }

    showErrors() {
        let firstInvalidField = null;
        for (const fieldName in this.fields) {
            if (!this.fields[fieldName].isValid) {
                if (!firstInvalidField) {
                    firstInvalidField = this.fields[fieldName].element;
                }
            }
        }

        if (firstInvalidField) {
            firstInvalidField.focus();
        }
    }

    reset() {
        for (const fieldName in this.fields) {
            const field = this.fields[fieldName];
            field.element.value = '';
            field.element.classList.remove('is-invalid', 'is-valid');
            field.errors = [];
        }
    }
}

// Initialize all forms on page load
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('form[data-validate="true"]').forEach(form => {
        new FormValidator(form);
    });
});

// Export for use in other scripts
if (typeof window !== 'undefined') {
    window.FormValidator = FormValidator;
}
