document.addEventListener('DOMContentLoaded', function () {

    document.getElementById('notification-icon').addEventListener('click', function () {
        const notificationdropDown = document.getElementById('notification-dropdown');

        if (notificationdropDown.classList.contains('hide-dropdown')) {
            notificationdropDown.classList.remove('hide-dropdown');
            notificationdropDown.classList.add('show-dropdown');
        } else {
            notificationdropDown.classList.remove('show-dropdown');
            notificationdropDown.classList.add('hide-dropdown');
        }

    });
});


//Connection Setup using the /notificatiohub Url
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

//Starts the signalR if connected calls the fetchMehtod
connection.start().then(() => {
    console.log("SignalR connected!");
    fetchNotificationCount();
}).catch(err => console.error(err));


//Sets up the eventHandler for the UpdateNotificationCount eventfrom the server
connection.on("UpdateNotificationCountAndData", (count, notificationsData) => {
        //console.log("Updated Notification Count:", count);
    updateNotificationCountAndData({ count, notificationsData });
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
            updateNotificationCountAndData(data);
        })
        .catch(error => console.error('Error fetching notification count:', error));
}

function updateNotificationCountAndData(notificationData) {
    const notificationCountElement = document.getElementById('notification-count');
    const list = document.getElementById('notification-list');

    const { count, data } = notificationData;

    if (count == 0) {
        notificationCountElement.style.display = 'none';
    } else {
        notificationCountElement.innerText = count;
        notificationCountElement.style.display = 'block';
    }

    list.innerHTML = ''; 
    //console.log(notificationData)
    data.forEach(notification => {
        const listItem = document.createElement('li');
        listItem.innerText = `${notification.message} - ${notification.createdAt}`;
        list.appendChild(listItem);
    });
}