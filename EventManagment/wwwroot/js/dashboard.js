document.addEventListener("DOMContentLoaded", function () {
    fetchDashboardStatistics();
});

async function fetchDashboardStatistics() {
    try {
        const response = await fetch("/GetAllData");
        const data = await response.json();

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const totalEventsElement = document.getElementById("totalEvents");
        const totalTicketsSold = document.getElementById("totalTickets");
        //console.log(data);
        if (totalEventsElement && totalTicketsSold) {
            totalEventsElement.innerText = data.totalEventsCreated;
            totalTicketsSold.innerText = data.totalTicketsSold;
        }


    } catch (error) {
        console.error("Error fetching dashboard statistics:", error);
    }
}