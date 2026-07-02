const stripe = Stripe(stripePublishableKey);
const elements = stripe.elements();

const style = {
    base: {
        fontSize: '16px',
        fontFamily: 'inherit',
        color: 'var(--text)',
        '::placeholder': { color: 'var(--placeholder)' },
        backgroundColor: 'transparent',
    },
    invalid: { color: '#dc3545' },
};

const cardNumber = elements.create('cardNumber', { style });
const cardExpiry = elements.create('cardExpiry', { style });
const cardCvc = elements.create('cardCvc', { style });

cardNumber.mount('#card-number-element');
cardExpiry.mount('#card-expiry-element');
cardCvc.mount('#card-cvc-element');

const inputs = [cardNumber, cardExpiry, cardCvc];

inputs.forEach(el => {
    el.on('focus', function () {
        const container = document.getElementById(this._retrieved?.id);
        if (container) container.classList.add('StripeElement--focus');
    });
    el.on('blur', function () {
        const container = document.getElementById(this._retrieved?.id);
        if (container) container.classList.remove('StripeElement--focus');
    });
    el.on('change', function (event) {
        const container = document.getElementById(this._retrieved?.id);
        if (container) {
            container.classList.toggle('StripeElement--invalid', !!event.error);
        }
    });
});

// Map element instances to their container IDs for focus/blur
// We use a simple approach: grab containers by known IDs
document.querySelectorAll('.stripe-input').forEach(el => {
    el.addEventListener('focusin', () => el.classList.add('StripeElement--focus'));
    el.addEventListener('focusout', () => el.classList.remove('StripeElement--focus'));
});

const cardErrors = document.getElementById('card-errors');
cardNumber.on('change', function (event) {
    if (event.error) {
        cardErrors.textContent = event.error.message;
    } else {
        cardErrors.textContent = '';
    }
});

const form = document.getElementById('checkout-form');
form.addEventListener('submit', async function (e) {
    const selectedMethod = document.querySelector('input[name="PaymentMethod"]:checked');
    if (!selectedMethod || selectedMethod.value !== 'CreditCard') {
        return;
    }

    e.preventDefault();
    setLoading(true);

    const cardholderName = document.getElementById('cardholder-name')?.value || undefined;

    const { error, paymentMethod } = await stripe.createPaymentMethod({
        type: 'card',
        card: cardNumber,
        billing_details: {
            name: cardholderName,
            email: document.getElementById('Email').value,
        },
    });

    if (error) {
        cardErrors.textContent = error.message;
        setLoading(false);
        return;
    }

    document.getElementById('PaymentMethodId').value = paymentMethod.id;
    form.submit();
});

function setLoading(isLoading) {
    const btn = form.querySelector('button[type="submit"]');
    if (isLoading) {
        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status"></span> Processing...';
    } else {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-shield-check"></i> Place Order';
    }
}
