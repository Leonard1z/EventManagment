document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    var body = document.body;

    function goToNextInput(e) {
        var t = e.target,
            sib = t.nextElementSibling;

        if (t.value.length === t.maxLength) {
            if (sib) {
                sib.focus();
            }
        }
    }

    function onKeyDown(e) {
        var key = e.key;
        //console.log(key);
        if (key === "Tab" || (key >= "0" && key <= "9")) {
            return true;
        }
        e.preventDefault();
        return false;
    }

    function onFocus(e) {
        e.target.select();
    }

    function handleVerification() {
        var verificationCode = Array.from(document.querySelectorAll('#form input')).map(input => input.value).join('');
        var phoneNumber = document.getElementById('phoneNumber').innerText;

        var data = {
            PhoneNumber:phoneNumber,
            VerificationCode:verificationCode
        };

        fetch('/api/verification/verify-code', {
            method: 'POST',
            headers: {
                'Content-type':'application/json',
            },
            body: JSON.stringify(data),
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                console.log('Verification successful:', data);
            } else {
                console.error('Invalid verification code:', data);
            }
        })
        .catch(error => {
            console.error('Error:', error);
        })
    }

    body.addEventListener('input', function (e) {
        if (e.target.tagName === 'INPUT') {
            goToNextInput(e);
        }
    });

    body.addEventListener('keydown', function (e) {
        if (e.target.tagName === 'INPUT') {
            onKeyDown(e);
        }
    });

    body.addEventListener('click', function (e) {
        if (e.target.tagName === 'INPUT') {
            onFocus(e);
        }
    });

    const verifyBtn = document.getElementById("verifyBtn")
    verifyBtn.addEventListener('click', () => {
        handleVerification();
    })
});