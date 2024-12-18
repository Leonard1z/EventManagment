﻿document.addEventListener('DOMContentLoaded', function () {
    var plusButtons = document.querySelectorAll('.plus-button');
    var minusButtons = document.querySelectorAll('.minus-button');
    var buyButtons = document.querySelectorAll('.buy-button');



    buyButtons.forEach(function (buyButton) {
        buyButton.addEventListener('click', function () {
            var ticketId = this.getAttribute('data-ticket-id');
            var quantityInput = document.querySelector('.quantity-input[data-ticket-id="' + ticketId + '"]');
            var currentQuantity = parseInt(quantityInput.value);
            var quantityErrorMessage = document.querySelector('.quantityErrorMessage[data-ticket-id="' + ticketId + '"]');
            var overlay = document.getElementById('overlay');
            overlay.style.display = 'flex';
            var ticket = getTicketById(ticketId);

            if (currentQuantity > 0) {
                if (!ticket.isFree) {
                    quantityErrorMessage.textContent = '';
                    var totalPrice = document.querySelector('.total-price[data-ticket-id="' + ticketId + '"]')
                    var ticketTotalPrice = parseFloat(totalPrice.textContent.replace('$', ''));
                    //console.log(currentTotalPrice);
                    sendReservationRequest(ticketId, currentQuantity, ticketTotalPrice, eventId);
                } else {
                    registerUserForFreeTickets(ticketId, currentQuantity, eventId);
                }
              
            } else {
                quantityErrorMessage.textContent = 'Quantity cannot be 0. Please select a valid value.';
                overlay.style.display = 'none';
            }

        });
    });

    async function registerUserForFreeTickets(ticketId, quantity, eventId) {
        try {
            const response = await fetch('/Registration/RegisterForFreeTickets', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ ticketId, quantity, eventId }),
            });
            if (!response.ok) {
                throw new Error('User registration failed: ' + response.statusText);
            }
            const data = await response.json()
            //console.log('After fetch', data);

            if (data && data.success) {
                updateDOMAfterReservation(ticketId);
                toastr.success(data.message);
            } else {
                //console.error('Reservation failed:', data.message);
                toastr.error(data.message);
                updateDOMAfterReservation(ticketId);
            }
        } catch (error) {
            console.error('Error:', error);
        }
    }

    async function sendReservationRequest(ticketId, quantity, ticketTotalPrice,eventId) {
        try {
            const response = await fetch('/Reservation/Reserve', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ ticketId, quantity, ticketTotalPrice, eventId }),

            });
            if (!response.ok) {
                throw new Error('Reservation failed: ' + response.statusText);
            }
            const data = await response.json()
            //console.log('After fetch', data);

            if (data && data.success) {
                updateDOMAfterReservation(ticketId);
                toastr.success(data.message);
            } else {
                //console.error('Reservation failed:', data.message);
                toastr.error(data.message);
                updateDOMAfterReservation(ticketId);
            }
        } catch (error) {
            console.error('Error:', error);
        }
    }

    function updateDOMAfterReservation(ticketId) {
        var quantityElement = document.getElementById('quantity-' + ticketId);
        var availableElement = document.getElementById('available-' + ticketId);

        if (quantityElement && availableElement) {
            fetch('/Home/GetAvailableQuantity/?ticketId=' + ticketId)
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        //console.log(data.availableQuantity)
                        var newQuantity = data.availableQuantity;
                        quantityElement.textContent = 'Available: ' + newQuantity;

                        var quantityInput = document.querySelector('.quantity-input[data-ticket-id="' + ticketId + '"]');
                        var totalPrice = document.querySelector('.total-price[data-ticket-id="' + ticketId + '"]')
                        if (quantityInput || totalPrice) {
                            quantityInput.value = 0;
                            totalPrice.value = 0;
                        }
                    } else {
                        console.error('Error fetching available quantity:', data.message);
                    }
                })
                .catch(error => {
                    console.error('Error fetching available quantity:', error);
                })
                .finally(() => {
                    overlay.style.display = 'none';
                })
        }
    }

    plusButtons.forEach(function (plusButton) {
        plusButton.addEventListener('click', function () {
            var ticketId = this.getAttribute('data-ticket-id');
            var quantityInput = document.querySelector('.quantity-input[data-ticket-id="' + ticketId + '"]');
            var quantityElement = document.getElementById('quantity-' + ticketId);
            var currentQuantity = parseInt(quantityInput.value);
            var maxQuantity = parseInt(quantityInput.getAttribute('max'));
            //console.log(quantityElement.textContent);
            var availableQuantity = parseInt(quantityElement.textContent.replace('Available: ', ''));
            //console.log(availableQuantity);

            if (currentQuantity < maxQuantity && currentQuantity < availableQuantity) {
                currentQuantity++;
                quantityInput.value = currentQuantity;
                updateTotalPrice(ticketId, currentQuantity)
                updateQuantityDisplay(ticketId, currentQuantity);
                //console.log('ticketId  ' + ticketId + ' quantityInput  ' + quantityInput + '  current Quantity  ' + currentQuantity + '  maxQuantity ' +
                //    maxQuantity);
            } else if (currentQuantity >= availableQuantity) {
                var quantityErrorMessage = document.querySelector('.quantityErrorMessage[data-ticket-id="' + ticketId + '"]');
                quantityErrorMessage.textContent = 'You cannot exceed the available tickets limit.';
            } else {

                var quantityErrorMessage = document.querySelector('.quantityErrorMessage[data-ticket-id="' + ticketId + '"]');
                quantityErrorMessage.textContent = 'You cannot exceed the limit of ' + maxQuantity + ' tickets.';
            }
        });
    });

    minusButtons.forEach(function (minusButton) {
        minusButton.addEventListener('click', function () {
            var ticketId = this.getAttribute('data-ticket-id');
            var quantityInput = document.querySelector('.quantity-input[data-ticket-id="' + ticketId + '"]');
            var currentQuantity = parseInt(quantityInput.value);

            if (currentQuantity > 0) {
                currentQuantity--;
                quantityInput.value = currentQuantity;
                updateTotalPrice(ticketId, currentQuantity)
                updateQuantityDisplay(ticketId, currentQuantity);
                //console.log(' ticketId  ' + ticketId + ' quantityInput  ' + quantityInput + '  current Quantity  ' + currentQuantity);
            }
        });
    });

    function updateTotalPrice(ticketId, quantity) {
        var ticket = getTicketById(ticketId);

        if (ticket) {
            if (!ticket.isFree) {
                var price = ticket.price;
                //console.log(ticket);

                var totalPrice = (price * quantity).toFixed(2);

                var totalPriceElement = document.querySelector('.total-price[data-ticket-id="' + ticketId + '"]');
                totalPriceElement.textContent = '$' + totalPrice;
            }
        }
    }

    function updateQuantityDisplay(ticketId, quantity) {

        var quantityParagraph = document.getElementById('quantity-' + ticketId);
        var available = document.getElementById('available-' + ticketId);

        //console.log(quantityParagraph);

        var quantityValue = parseInt((quantityParagraph.textContent || '').match(/\d+/)[0], 10);
        var newQuantity = quantityValue - quantity;

        //console.log('new quantity ' + newQuantity + ' quanityValue ' + quantityValue);
        available.textContent = 'Available: ' + newQuantity;
        if (quantity >= 0) {
            quantityParagraph.style.display = 'none';
        } else {
            quantityParagraph.style.display = 'block';
        }
    }

     // Find the ticket in the tickets array by its ID
    function getTicketById(ticketId) {
        var ticket = tickets.find(function (t) {
            return t.ticketId === parseInt(ticketId);
        });
        return ticket;
    }

});