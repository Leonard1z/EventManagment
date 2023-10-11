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


//Sets up the eventHandler for the UpdateNotificationCountAndData eventfrom the server
connection.on("UpdateNotificationCountAndData", () => {
    fetchNotificationCount();
});

function fetchNotificationCount() {
    fetch('/Notification/GetNotifications')
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
        if (notification.isRead) {
            listItem.classList.add('read-notification');
        } else {
            listItem.classList.add('unread-notification');
        }

        listItem.addEventListener('click', function () {
            openNotificationDetailsModal(notification)
            markNotificationAsRead(notification);
        })
        list.insertBefore(listItem, list.firstChild);
    });
}


function openNotificationDetailsModal(notification) {
    const modal = new bootstrap.Modal(document.getElementById('notificationDetailsModal'));
    const modalBody = document.getElementById('notificationDetailsBody');
    modalBody.innerHTML = `
        <div class="alert alert-info" role="alert">
            <strong>Message:</strong> ${notification.message}
        </div>
        <p><strong>Created at:</strong> ${notification.createdAt}</p>
        <div style="text-align:center;">
            <a href="${notification.paymentLink}" style="display: inline-block; padding: 10px 20px; background-color: #3498db; color: #ffffff; text-decoration: none; border-radius: 5px;">Complete Payment</a>
        </div>
    `;
    modal.show();
}

function markNotificationAsRead(notification) {
    //invokes the MarkNotificationAsRead in the HUB
    connection.invoke("MarkNotificationAsRead", notification.id)
        .then(() => {
        }).catch(error => console.error('Error marking the notification',error));
}

//Sets up the eventHandler for the MarkNotificationAsRead eventfrom the server
connection.on("MarkNotificationAsRead", (modifiedNotification) => {
    //console.log('Notification marked as read', modifiedNotification);
    updateUIForReadNotification(modifiedNotification);

});

function updateUIForReadNotification(modifiedNotification) {
    const itemId = `notification-${modifiedNotification.id}`;
    const selectedItembasedId = document.getElementById(itemId);
    //console.log(selectedItembasedId);
    if (selectedItembasedId) {
        selectedItembasedId.classList.remove('unread-notification');
        selectedItembasedId.classList.add('read-notification');
    } 
    fetchNotificationCount();
}