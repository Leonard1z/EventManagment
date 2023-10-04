const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start().then(() => {
    console.log("SignalR connected!");
    fetchNotificationCount();
}).catch(err => console.error(err));

connection.on("UpdateNotificationCount", (count) => {
    //console.log("Updated Notification Count:", count);
    updateNotificationCount(count);
});

function fetchNotificationCount() {
    fetch('/Notification/GetNotificationCount')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            updateNotificationCount(data.count);
        })
        .catch(error => console.error('Error fetching notification count:', error));
}

function updateNotificationCount(count) {
    const notificationCountElement = document.getElementById('notification-count');
        notificationCountElement.innerText = count;
}

function checkIfUserIsAuthenticated() {
    return isAuthenticated === 'true';
}