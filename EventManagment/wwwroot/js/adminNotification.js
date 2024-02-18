
document.addEventListener('DOMContentLoaded', function () {

    document.getElementById('admin-notification-icon').addEventListener('click', function () {
        const notificationdropDown = document.getElementById('admin-notification-dropdown');

        if (notificationdropDown.classList.contains('hide-dropdown')) {
            notificationdropDown.classList.remove('hide-dropdown');
            notificationdropDown.classList.add('show-dropdown');

            markAllNotificationsAsRead();

        } else {
            notificationdropDown.classList.remove('show-dropdown');
            notificationdropDown.classList.add('hide-dropdown');
        }

    });
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start().then(() => {
    console.log("SignalR connected!");
    fetchAdminNotificationCount();
}).catch(err => console.error(err));

connection.on("ReceiveNotification", () => {
    fetchAdminNotificationCount();
});

function fetchAdminNotificationCount() {
    fetch('/Notification/GetAdminNotifications')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            updateAdminNotificationCountAndData(data);
        })
        .catch(error => console.error('Error fetching notification count:', error));
}

function updateAdminNotificationCountAndData(notificationData) {
    const notificationCountElement = document.getElementById('admin-notification-count');
    const list = document.getElementById('admin-notification-list');

    const { data, count } = notificationData;

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
        listItem.innerText = `${notification.message}`;
        listItem.id = `notification-${notification.id}`;
        list.insertBefore(listItem, list.firstChild);
    });
}

function markAllNotificationsAsRead() {
    connection.invoke("MarkAllNotificationsAsRead")
        .then(() => {
        }).catch(error => console.log("Error while marking all notification", error));
}

connection.on("MarkAllNotificationsAsRead", () => {
    fetchAdminNotificationCount();
    console.log("done");
});