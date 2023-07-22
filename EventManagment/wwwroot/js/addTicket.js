function saveTicketType() {
    const name = document.getElementById('TicketName').value;
    const description = document.getElementById('TicketDescription').value;
    const price = parseFloat(document.getElementById('Price').value);
    const quantity = parseInt(document.getElementById('Quantity').value);
    const isAvailable = document.getElementById('IsAvailable').value === 'true';
    const saleStartDate = document.getElementById('SaleStartDate').value;
    const saleEndDate = document.getElementById('SaleEndDate').value;

    console.log("Name:", name);

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
    updateTicketList();
    $('#addTicketModal').modal('hide');
    console.log(ticketTypes);
}

function updateTicketList() {
    const ticketListContainer = document.getElementById('ticketListContainer');
    ticketListContainer.innerHTML = '';

    ticketTypes.forEach(ticketType => {
        const isAvailableText = ticketType.IsAvailable ? 'Yes' : 'No';
        const ticketRow = `
                    <tr>
                        <td>${ticketType.Name}</td>
                        <td>${ticketType.Description}</td>
                        <td>${ticketType.Price}</td>
                        <td>${ticketType.Quantity}</td>
                        <td>${isAvailableText}</td>
                        <td>${ticketType.SaleStartDate}</td>
                        <td>${ticketType.SaleEndDate}</td>
                    </tr>
                `;

        console.log(ticketRow);
        ticketListContainer.insertAdjacentHTML('beforeend', ticketRow);
    });
}