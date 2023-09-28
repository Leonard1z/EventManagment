document.addEventListener("DOMContentLoaded", function () {
    function updateCountdown() {

        for (var i = 0; i < tickets.length; i++) {
            var saleEndDate = new Date(tickets[i].saleEndDate);
            // Calculate the timeremaining for each ticket
            var currentTime = new Date();
            var timeRemaining = saleEndDate - currentTime;
            //console.log("SaleEnd Date:" + saleEndDate);
            //console.log("Time reamining:" + timeRemaining);

            var ticketId = tickets[i].ticketId;

            //console.log("Ticket ID: "+ticketId);

            var daysElement = document.getElementById("days-" + ticketId);
            var hoursElement = document.getElementById("hours-" + ticketId);
            var minutesElement = document.getElementById("minutes-" + ticketId);
            var secondsElement = document.getElementById("seconds-" + ticketId);
            var buyBtn = document.getElementById("buy-btn-" + ticketId);

            if (!daysElement || !hoursElement || !minutesElement || !secondsElement || !buyBtn) {
                continue;
            }

            // Calculate days, hours, minutes, and seconds
            var days = Math.floor(timeRemaining / (1000 * 60 * 60 * 24));
            var hours = Math.floor((timeRemaining % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            var minutes = Math.floor((timeRemaining % (1000 * 60 * 60)) / (1000 * 60));
            var seconds = Math.floor((timeRemaining % (1000 * 60)) / 1000);


            if (timeRemaining <= 0) {
                daysElement.textContent = "0";
                hoursElement.textContent = "0";
                minutesElement.textContent = "0";
                secondsElement.textContent = "0";
                buyBtn.style.display = "none";
            } else {
                daysElement.textContent = days;
                hoursElement.textContent = hours;
                minutesElement.textContent = minutes;
                secondsElement.textContent = seconds;
            }
        }
    }

    var timer = setInterval(updateCountdown, 1000);

    updateCountdown();
});