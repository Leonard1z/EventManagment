function assignTickets() {
    var assigneeData = [];
    var registrationId = document.getElementById('registrationId').value;
    showLoadingOverlay();
    fetch('/GetRegistration?registrationId=' + registrationId, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    })
    .then(response => response.json())
        .then(data => {
            console.log(data);
            for (var i = 1; i <= data.quantity; i++) {
                var firstName = document.getElementById('assigneeFirstName' + i).value;
                var lastName = document.getElementById('assigneeLastName' + i).value;
                var email = document.getElementById('assigneeEmail' + i).value;
                var phoneNumber = document.getElementById('assigneePhoneNumber' + i).value;

                if (!firstName || !lastName || !email || !phoneNumber) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Incomplete Form...',
                        text: 'Please provide all the required information for the ticket assignment.',
                    });
                    hideLoadingOverlay();
                    return;
                }

                if (!email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Invalid Email',
                        text: 'Please provide a valid email address.',
                    });
                    hideLoadingOverlay();
                    return;
                }

                //console.log('firstname' + firstName);
                assigneeData.push({
                    registrationId: registrationId,
                    firstName: firstName,
                    lastName: lastName,
                    email: email,
                    phoneNumber: phoneNumber,
                    eventName: data.eventName,
                    eventStartDate: data.eventStartDate,
                    expireDate: data.eventEndDate,
                    venue: data.venue,
                    ticketType: data.ticketTypeName,
                    ticketPrice: data.ticketPrice,
                });

                //console.log(assigneeData);
            }


        fetch('/Assign/Ticket', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(assigneeData),
        })
        .then(response => response.json())
            .then(data => {
                if (data.success) {
                    console.log('Success', data);
                    hideLoadingOverlay();
                    $('#assignModal').modal('hide');
                    hideRegistrationCard(registrationId);
                    toastr.success(data.message);
                } else {
                    toastr.error(data.message);
                }
            })
            .catch((error) => {
                console.log('Error', error);
                hideLoadingOverlay();
            });

        })
        .catch((error) => {
            hideLoadingOverlay();
            console.log('Error while retrieving data', error);
        });

    //console.log('Assignee Data:', assigneeData);
}

function openAssignModal(quantity, registrationId) {
    var modalBody = document.getElementById('assignModalBody');
    modalBody.innerHTML = '';
    document.getElementById('registrationId').value = registrationId;

    for (var i = 1; i <= quantity; i++) {
        var div = document.createElement('div');
        div.className = 'form-group border my-2 p-3 col-md-6';

        var ticketLabel = document.createElement('label');
        ticketLabel.innerText = 'Ticket ' + i
        ticketLabel.className = 'label text-info fs-5 text-center mb-2';
        ticketLabel.style.display = 'block';

        var labelFirstName = document.createElement('label');
        labelFirstName.innerText = 'First Name:';
        var inputFirstName = document.createElement('input');
        inputFirstName.type = 'text';
        inputFirstName.className = 'form-control';
        inputFirstName.id = 'assigneeFirstName' + i;

        var labelLastName = document.createElement('label');
        labelLastName.innerText = 'Last Name:';
        var inputLastName = document.createElement('input');
        inputLastName.type = 'text';
        inputLastName.className = 'form-control';
        inputLastName.id = 'assigneeLastName' + i;

        var labelEmail = document.createElement('label');
        labelEmail.innerText = 'Email:';
        var inputEmail = document.createElement('input');
        inputEmail.type = 'email';
        inputEmail.className = 'form-control';
        inputEmail.id = 'assigneeEmail' + i;

        var labelPhoneNumber = document.createElement('label');
        labelPhoneNumber.innerText = 'Phone Number:';
        var inputPhoneNumber = document.createElement('input');
        inputPhoneNumber.type = 'text';
        inputPhoneNumber.className = 'form-control';
        inputPhoneNumber.id = 'assigneePhoneNumber' + i;

        div.appendChild(ticketLabel);
        div.appendChild(labelFirstName);
        div.appendChild(inputFirstName);
        div.appendChild(labelLastName);
        div.appendChild(inputLastName);
        div.appendChild(labelEmail);
        div.appendChild(inputEmail);
        div.appendChild(labelPhoneNumber);
        div.appendChild(inputPhoneNumber);

        modalBody.appendChild(div);
    }

    $('#assignModal').modal('show');

}

function hideRegistrationCard(registrationId) {
    var registrationCard = document.getElementById('registrationCard_' + registrationId);
    if (registrationCard) {
        registrationCard.style.display = 'none';
    }
}

function showLoadingOverlay() {
    document.getElementById('overlay').style.display = 'flex';
}

function hideLoadingOverlay() {
    document.getElementById('overlay').style.display = 'none';
}