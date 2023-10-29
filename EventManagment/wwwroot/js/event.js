var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/GetAllEvents",
            "type": "GET",
            "dataType": "json",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "description", "width": "30%" },
            { "data": "state", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "category.name", "width": "10%" },
            {
                "data": "encryptedId",
                "render": function (data) {
                    return `
            <a href="/Event/Edit?encryptedId=${data}" class="btn btn-primary"> <i class="bi bi-pencil-square"></i>Edit</a> 
            <a href="/EventDetails?encryptedId=${data}" class="btn btn-info"><i class="bi bi-eye"></i> Details</a>
            <a onclick="getTickets('${data}')" class="btn btn-success"><i class="bi bi-ticket"></i> Tickets</a>
            <a onclick="DeleteEvent('${data}')" class="btn btn-danger"><i class="bi bi-trash-fill"></i>Delete</a>
            `;
                },
                "width": "25%"
            }
        ]
    });
}

function getTickets(encryptedId) {
    var url = '/Event/GetTickets?encryptedId=' + encryptedId;

    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.data.length > 0) {
                var ticketHtml = '';

                response.data.forEach(function (ticket) {

                    var saleStartDate = new Date(ticket.saleStartDate).toLocaleDateString('en-Ca');
                    var saleEndDate = new Date(ticket.saleEndDate).toLocaleDateString('en-Ca');

                    ticketHtml += `
                      <div class="col-6 border ticket-info">
                        <p><strong>Name:</strong> ${ticket.name}</p>
                        <p><strong>Quantity:</strong> ${ticket.description}</p>
                        <p><strong>Price:</strong> ${ticket.price}</p>
                        <p><strong>Quantity:</strong> ${ticket.quantity}</p>
                        <p><strong>Available:</strong> ${ticket.isAvailable ? 'Yes' : 'No'}</p>
                        <p><strong>Sale Start Date:</strong> ${saleStartDate}</p>
                        <p><strong>Sale End Date:</strong> ${saleEndDate}</p>
                      </div>
                    `;
                });

                $('#ticketDetails').html(ticketHtml);

                $('#ticketModal').modal('show');
                //console.log(ticketHtml)

            } else {
                var noTicketMessageHtml = `
                <h1 style="text-align: center; font-size: 20px;">No tickets found for this event.</h1>
                `;
                $('#ticketDetails').html(noTicketMessageHtml);
                $('#ticketModal').modal('show');

            }
        },
        error: function () {
            toastr.error("Error occurred while retrieving tickets.");
        }
    });
}




function DeleteEvent(encryptedId) {
    var url = '/Event/Delete?encryptedId=' + encryptedId;

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        dataTable.ajax.reload();
                        toastr.success(response.message);
                    }
                    else {
                        toastr.error(response.message);
                    }
                },
                error: function () {
                    toastr.error("Error occurred during the delete operation.");
                }
            })
        }
    });
}

