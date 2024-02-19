
function createSignalRConnection(url, onConnectedCallback) {
    //Connection Setup using the provided URL
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .build();
    //Starts the SignalR connection
    connection.start().then(() => {
        console.log("SignalR connected!");
        // Call the callback function if provided
        if (onConnectedCallback && typeof onConnectedCallback === 'function') {
            onConnectedCallback(connection);
        }
    }).catch(err => console.error(err));

    return connection;
}

window.signalRHelper = {
    createSignalRConnection
};