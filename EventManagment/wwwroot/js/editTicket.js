let ticketDataObj = {};
document.addEventListener('DOMContentLoaded', function () {
	const baseUrl = window.location.origin;
	const urlParams = new URLSearchParams(window.location.search);
	const encryptedId = urlParams.get('encryptedId');
	const url = `${baseUrl}/api/ticket/UpdateTicket?encryptedId=${encryptedId}`;

	fetch(url)
		.then(response => response.json())
		.then(data => {
			if (data.success) {
				ticketDataObj=data.data;
				var ticketPriceInput = document.getElementById('Price');
				//console.log(data);
				document.getElementById('Name').value = data.data.name;
				document.getElementById('Description').value = data.data.description;
				if (data.data.isFree) {
					ticketPriceInput.disabled = true;
					ticketPriceInput.type = "text";
					ticketPriceInput.value = "Free";
				} else {
					ticketPriceInput.value = data.data.price;
				}
				document.getElementById('Quantity').value = data.data.quantity;
				document.getElementById('SaleStartDate').value = data.data.saleStartDate;
				document.getElementById('SaleEndDate').value = data.data.saleEndDate;

			} else {
				console.error('Error fetching available quantity:', data.message);
			}
		})
		.catch(error => console.error('Error fetching ticket data:', error));
});

document.getElementById('updateButton').addEventListener('click', function (event) {
	event.preventDefault();
	//console.log(ticketDataObj);
	//console.log(ticketDataObj.name);
	const form = document.getElementById('updateTicketForm');
	const formData = new FormData(form);
	const plainFormData = {};
	plainFormData['EncryptedId'] = encodeURIComponent(ticketDataObj.encryptedId);
	plainFormData['EncryptedEventId'] = encodeURIComponent(ticketDataObj.encryptedEventId);
	for (const [key, value] of formData.entries()) {
		plainFormData[key] = value;
	}

	if (ticketDataObj.isFree) {
		plainFormData['Price'] = 0;
		plainFormData['IsFree'] = true;
	}

	//console.log(plainFormData);

	const saleStartDateFormData = new Date(plainFormData['SaleStartDate']);
	const saleStartDateObj = new Date(ticketDataObj.saleStartDate);
	const saleEndDateFormData = new Date(plainFormData['SaleEndDate']);
	const currentDate = new Date();
	//console.log(saleStartDateFormData);
	//console.log(saleStartDateObj);


	if (saleStartDateFormData < saleStartDateObj) {
		Swal.fire({
			icon: 'error',
			title: 'Oops...',
			text: 'Sale start date cannot be earlier than the original start date. Please select a valid date.',
		});
		return;
	}
	if (saleEndDateFormData <= saleStartDateFormData) {
		Swal.fire({
			icon: 'error',
			title: 'Oops...',
			text: 'Sale end date cannot be earlier than the sale start date. Please select a valid date.',
		});
		return;
	}
	if (saleStartDateFormData <= currentDate && saleEndDateFormData >= currentDate) {
		plainFormData['IsAvailable'] = true;
		//console.log(plainFormData['IsAvailable'])
	} else {
		plainFormData['IsAvailable'] = false;
		//console.log(plainFormData['IsAvailable'])
	}
	const errorMessages = document.querySelectorAll('.error-message');
	errorMessages.forEach(errorMessage => errorMessage.textContent = '');
	let isValid = true;
	const inputFields = form.querySelectorAll('input, textarea');
	inputFields.forEach(input => {
		const fieldName = input.getAttribute('name');
		const value = input.value;

		if (value === '') {
			isValid = false;
			const errorElement = document.getElementById(`${fieldName.toLowerCase()}Error`);
			errorElement.textContent = `${fieldName} is required.`;
		} else if (fieldName === 'Quantity') {
			if (parseInt(value) <= 0 || isNaN(parseInt(value))) {
				isValid = false;
				const errorElement = document.getElementById(`${fieldName.toLowerCase()}Error`);
				errorElement.textContent = `${fieldName} must be a positive number greater than zero.`
			}
		} else if (fieldName === 'Price' && !input.disabled) {
			if (parseFloat(value) <= 0 || isNaN(parseFloat(value))) {
				isValid = false;
				const errorElement = document.getElementById(`${fieldName.toLowerCase()}Error`);
				errorElement.textContent = `${fieldName} must be a positive number greater than zero.`;
			}
		}

	});

	if (isValid) {
		//console.log(plainFormData);
		fetch('/api/ticket/EditTicket', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify(plainFormData),
		})
			.then(response => {
				if (!response.ok) {
					return response.json().then(errorData => {
						toastr.error(errorData.message);
						throw new Error('Network response was not ok');
					});
				}
				return response.json();
			})
			.then(data => {
				window.location.href = '/Event/ViewTickets?encryptedId=' + plainFormData.EncryptedEventId;
				toastr.success(data.message);
			})
			.catch(error => {
				console.log('Error', error);
			});

	} else {
		console.log('Form validation failed. Please fill in all required fields.');
	}
});