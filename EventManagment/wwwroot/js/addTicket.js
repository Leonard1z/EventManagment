document.addEventListener("DOMContentLoaded", function () {
    var freeOption = document.getElementById("freeOption");
    var paidOption = document.getElementById("paidOption");
    var ticketPriceInput = document.getElementById("Price");

    freeOption.addEventListener('click', function () {
        ticketPriceInput.disabled = true;
        ticketPriceInput.type = "text";
        ticketPriceInput.value = "Free";
        $('#selectTicketTypeModal').modal('hide');
        $('#addTicketModal').modal('show');
    });

    paidOption.addEventListener('click', function () {
        ticketPriceInput.type = "number";
        ticketPriceInput.disabled = false;
        ticketPriceInput.value = "";
        $('#selectTicketTypeModal').modal('hide');
        $('#addTicketModal').modal('show');
    });
});

function saveTicketType() {
    const name = document.getElementById('TicketName').value;
    const description = document.getElementById('TicketDescription').value;
    const priceInput = document.getElementById('Price');
    const quantity = parseInt(document.getElementById('Quantity').value);
    const saleStartDate = document.getElementById('SaleStartDate').value;
    const saleEndDate = document.getElementById('SaleEndDate').value;
    const imageInput = document.getElementById('ticketImage');

    let price;
    let isFree;

    if (priceInput.value.toLowerCase() === "free") {
        price = 0;
        isFree = true;
        //console.log('isfree');
    } else {
        price = parseFloat(priceInput.value);
        isFree = false;
/*        console.log('paid');*/
    }


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
        IsAvailable: isAvailable,
        IsFree: isFree 
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
        const isTicketFree = ticketType.IsFree ? 'Free' : 'Paid'; 
        const ticketRow = `
                    <tr>
                        <td>${ticketType.Name}</td>
                        <td>${ticketType.Description}</td>
                        <td>${ticketType.Price}</td>
                        <td>${ticketType.Quantity}</td>
                        <td>${isAvailable}</td>
                        <td>${isTicketFree}</td>
                        <td>${ticketType.SaleStartDate}</td>
                        <td>${ticketType.SaleEndDate}</td>
                    </tr>
                `;

        ticketListContainer.insertAdjacentHTML('beforeend', ticketRow);
    });
}

