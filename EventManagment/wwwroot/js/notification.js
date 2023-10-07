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
        if (count == 0) {
            notificationCountElement.style.display = 'none';
        } else {
            notificationCountElement.innerText = count;
            notificationCountElement.style.display = 'block';
        }
}