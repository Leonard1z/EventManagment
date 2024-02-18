const verificationForm = document.querySelector("#verificationForm");
const phoneInputField = document.querySelector("#phone");
const errorMsg = document.querySelector("#error-msg");

const errorMap = ["Invalid number", "Invalid country code", "Too short", "Too long", "Invalid number"];

const phoneInput = window.intlTelInput(phoneInputField, {
    onlyCountries: ["xk", "al", "mk"],
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/19.2.19/js/utils.js",
});

verificationForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (phoneInputField.value.trim()) {
        if (phoneInput.isValidNumberPrecise()) {
            try {
                const response = await fetch('/api/verification/send-code', {
                    method: 'POST',
                    headers: {
                        'Content-type': 'application/json',
                    },
                    body: JSON.stringify({ phoneNumber: phoneInput.getNumber() }),

                })
                if (response.ok) {
                    const responseData = await response.json();
                    const verificationPageUrl = responseData.verificationPageUrl;
                    console.log('API call success');
                    window.location.href = verificationPageUrl;
                } else {
                    console.error('API call failed');
                }
            } catch (eror) {
                console.error('Error during API call:', error.message);
            }
        } else {
            const errorCode = phoneInput.getValidationError();
            errorMsg.innerHTML = errorMap[errorCode] || "Invalid number";
            errorMsg.classList.remove("hide");
        }
    }
});