document.addEventListener("DOMContentLoaded", function () {
    fetchDashboardStatistics();
});

async function fetchDashboardStatistics() {
    try {
        const response = await fetch("/GetEventCountForDashboard");
        const data = await response.json();

        const totalEventsElement = document.getElementById("totalEvents");
        if (totalEventsElement) {
            totalEventsElement.innerText = data.eventCount;
        }


    } catch (error) {
        console.error("Error fetching dashboard statistics:", error);
    }
}