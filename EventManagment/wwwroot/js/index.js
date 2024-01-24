document.addEventListener('DOMContentLoaded', function () {
    var paginationLinks = document.querySelectorAll('.pagination a');

    paginationLinks.forEach(function (link) {
        link.addEventListener('click', function () {
            paginationLinks.forEach(function (link) {
                link.classList.remove('current');
            });
            this.classList.add('current');
        });
    });
});



function confirmDelete(encryptedId,controller,action) {
    event.preventDefault();
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
            //console.log('Controller:' + controller, 'Action:' + action);
            window.location.href = "/" + controller + "/" + action + "?encryptedId=" + encodeURIComponent(encryptedId);
        }
    });
}
