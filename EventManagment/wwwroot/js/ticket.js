document.addEventListener('DOMContentLoaded', function () {
    var encryptedId = document.getElementById('encryptedId').innerText;
    getTickets(encryptedId);
});

function getTickets(encryptedId) {
    const baseUrl = window.location.origin;
    const ticketsUrl = `${baseUrl}/Event/GetTickets?encryptedId=${encryptedId}`;

    $.ajax({
        url: ticketsUrl,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.data.length > 0) {
                console.log(response.data);
            } else {
                console.log('No tickets found for this event.');
            }
        },
        error: function () {
            toastr.error("Error occurred while retrieving tickets.");
        }
    });
}