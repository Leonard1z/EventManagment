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
            { "data": "name", "width": "10%" },
            { "data": "description", "width": "25%" },
            { "data": "state", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "category.name", "width": "10%" },
            { "data": "countRegistration", "width": "10%" },
            {
                "data": "encryptedId",
                "render": function (data) {
                    return `
            <a href="/Event/Edit?encryptedId=${data}" class="btn btn-primary"> <i class="bi bi-pencil-square"></i>Edit</a> 
            <a href="/EventDetails?encryptedId=${data}" class="btn btn-info"><i class="bi bi-eye"></i> Details</a> 
            <a onclick="DeleteEvent('${data}')" class="btn btn-danger"><i class="bi bi-trash-fill"></i>Delete</a>
            `;
                },
                "width": "25%"
            }
        ]
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

