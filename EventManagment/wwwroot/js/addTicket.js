function saveTicketType() {
    const name = document.getElementById('TicketName').value;
    const description = document.getElementById('TicketDescription').value;
    const price = parseFloat(document.getElementById('Price').value);
    const quantity = parseInt(document.getElementById('Quantity').value);
    const saleStartDate = document.getElementById('SaleStartDate').value;
    const saleEndDate = document.getElementById('SaleEndDate').value;

    if (!name || !description || isNaN(price) || price < 0 || isNaN(quantity) || quantity < 0 || !saleStartDate || !saleEndDate) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill out all required fields and ensure Price and Quantity are non-negative numbers!',
        });
        return;
    }

    const currentDate = new Date().toISOString();

    //console.log('Current Date:', currentDate);
    //console.log('Sale Start Date:', saleStartDate);
    //console.log('Sale End Date:', saleEndDate);

    if (saleStartDate < currentDate) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Sale start date cannot be earlier than the current date. Please select a valid date.',
        });
        return;
    }

    if (saleEndDate < saleStartDate) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Sale end date cannot be earlier than the sale start date. Please select a valid date.',
        });
        return;
    }

    const isAvailable = saleStartDate <= currentDate && saleEndDate >= currentDate;
    //console.log('Is Available:', isAvailable);

    const newTicketType = {
        Name: name,
        Description: description,
        Price: price,
        Quantity: quantity,
        SaleStartDate: saleStartDate,
        SaleEndDate: saleEndDate,
        IsAvailable: isAvailable
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
        const isAvailable = ticketType.IsAvailable ? 'Yes' : 'No';
        const ticketRow = `
                    <tr>
                        <td>${ticketType.Name}</td>
                        <td>${ticketType.Description}</td>
                        <td>${ticketType.Price}</td>
                        <td>${ticketType.Quantity}</td>
                        <td>${isAvailable}</td>
                        <td>${ticketType.SaleStartDate}</td>
                        <td>${ticketType.SaleEndDate}</td>
                    </tr>
                `;

        ticketListContainer.insertAdjacentHTML('beforeend', ticketRow);
    });
}

