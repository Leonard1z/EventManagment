document.addEventListener('DOMContentLoaded', function () {
    var plusButtons = document.querySelectorAll('.plus-button');
    var minusButtons = document.querySelectorAll('.minus-button');
    var buyButtons = document.querySelectorAll('.buy-button');



    buyButtons.forEach(function (buyButton) {
        buyButton.addEventListener('click', function () {
            var ticketId = this.getAttribute('data-ticket-id');
            var quantityInput = document.querySelector('.quantity-input[data-ticket-id="' + ticketId + '"]');
            var currentQuantity = parseInt(quantityInput.value);

            sendReservationRequest(ticketId, currentQuantity);

        });
    });


    function sendReservationRequest(ticketId, quantity) {
        fetch('/Reservation/Reserve', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ ticketId, quantity }),
            
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }

    plusButtons.forEach(function (plusButton) {
        plusButton.addEventListener('click', function () {
            var ticketId = this.getAttribute('data-ticket-id');
            var quantityInput = document.querySelector('.quantity-input[data-ticket-id="' + ticketId + '"]');
            var currentQuantity = parseInt(quantityInput.value);
            var maxQuantity = parseInt(quantityInput.getAttribute('max'));

            if (currentQuantity < maxQuantity) {
                currentQuantity++;
                quantityInput.value = currentQuantity;
                updateTotalPrice(ticketId, currentQuantity)
                updateQuantityDisplay(ticketId, currentQuantity);
                //console.log('ticketId  ' + ticketId + ' quantityInput  ' + quantityInput + '  current Quantity  ' + currentQuantity + '  maxQuantity ' +
                //    maxQuantity);
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
        // Find the ticket in the tickets array by its ID
        var ticket = tickets.find(function (t) {
            return t.ticketId === parseInt(ticketId);
        });

        if (ticket) {
            var price = ticket.price;
            //console.log(ticket);

            var totalPrice = (price * quantity).toFixed(2);

            var totalPriceElement = document.querySelector('.total-price[data-ticket-id="' + ticketId + '"]');
            totalPriceElement.textContent = '$' + totalPrice;
        }
    }

    function updateQuantityDisplay(ticketId, quantity) {

        var quantityParagraph = document.getElementById('quantity-' + ticketId).textContent;
        var available = document.getElementById('available-' + ticketId);

        //console.log(quantityParagraph);

        var quantityValue = parseInt(quantityParagraph.match(/\d+/)[0], 10);

        var newQuantity = quantityValue - quantity;

        //console.log('new quantity ' + newQuantity + ' quanityValue ' + quantityValue);

        available.textContent = 'Available: ' + newQuantity;
    }

});