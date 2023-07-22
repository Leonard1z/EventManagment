function saveTicketType() {
    const name = document.getElementById('TicketName').value;
    const description = document.getElementById('TicketDescription').value;
    const price = parseFloat(document.getElementById('Price').value);
    const quantity = parseInt(document.getElementById('Quantity').value);
    const isAvailable = document.getElementById('IsAvailable').value === 'true';
    const saleStartDateInput = document.getElementById('SaleStartDate').value;
    const saleEndDateInput = document.getElementById('SaleEndDate').value;

    if (!name || !description || isNaN(price) || price < 0 || isNaN(quantity) || quantity < 0 || !saleStartDateInput || !saleEndDateInput) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill out all required fields and ensure Price and Quantity are non-negative numbers!',
        });
        return;
    }

    const saleStartDate = new Date(saleStartDateInput)
    const saleEndDate = new Date(saleEndDateInput)


    const currentDate = new Date();
    currentDate.setHours(0, 0, 0, 0);

    if (saleStartDate.getTime() < currentDate.getTime()) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Sale start date cannot be earlier than the current date. Please select a valid date.',
        });
        return;
    }

    if (saleEndDate.getTime() < saleStartDate.getTime()) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Sale end date cannot be earlier than the sale start date. Please select a valid date.',
        });
        return;
    }

    const newTicketType = {
        Name: name,
        Description: description,
        Price: price,
        Quantity: quantity,
        IsAvailable: isAvailable,
        SaleStartDate: saleStartDate,
        SaleEndDate: saleEndDate
    };

    ticketTypes.push(newTicketType);
    document.getElementById('ticketData').value = JSON.stringify(ticketTypes);
    updateTicketList();
    $('#addTicketModal').modal('hide');
    //console.log(ticketTypes);
}

function updateTicketList() {
    const ticketListContainer = document.getElementById('ticketListContainer');
    ticketListContainer.innerHTML = '';

    ticketTypes.forEach(ticketType => {
        const isAvailableText = ticketType.IsAvailable ? 'Yes' : 'No';
        const saleStartDate = new Date(ticketType.SaleStartDate).toLocaleDateString('en-CA');
        const saleEndDate = new Date(ticketType.SaleEndDate).toLocaleDateString('en-CA');
        const ticketRow = `
                    <tr>
                        <td>${ticketType.Name}</td>
                        <td>${ticketType.Description}</td>
                        <td>${ticketType.Price}</td>
                        <td>${ticketType.Quantity}</td>
                        <td>${isAvailableText}</td>
                        <td>${saleStartDate}</td>
                        <td>${saleEndDate}</td>
                    </tr>
                `;

        ticketListContainer.insertAdjacentHTML('beforeend', ticketRow);
    });
}

