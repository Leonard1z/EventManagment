document.addEventListener('DOMContentLoaded', function () {
	const ticketOption = document.getElementById('ticketOption').textContent;
	const ticketPriceInput = document.getElementById('Price');

	if (ticketOption === 'free') {
		ticketPriceInput.disabled = true;
		ticketPriceInput.type = "text";
		ticketPriceInput.value = "Free";
	}
})

const textarea = document.getElementById('Description');
textarea.addEventListener('input', function () {
	this.style.height = 'auto';
	this.style.height = this.scrollHeight + 'px';
});

document.getElementById('createButton').addEventListener('click', function (event) {
	event.preventDefault();

	const form = document.getElementById('addTicketForm');
	const formData = new FormData(form);
	const encryptedEventId = document.getElementById('encryptedId').textContent;
	const ticketOption = document.getElementById('ticketOption').textContent;
	formData.append('EncryptedEventId', encodeURIComponent(encryptedEventId));
	//console.log(...formData.entries());
	const plainFormData = {};
	for (const [key, value] of formData.entries()) {
		plainFormData[key] = value;
	}
	if (ticketOption === 'free') {
		plainFormData['Price'] = 0;
		plainFormData['IsFree'] = true;
	}
	//console.log(plainFormData);
	const saleStartDate = new Date(document.getElementById('SaleStartDate').value);
	const saleEndDate = new Date(document.getElementById('SaleEndDate').value);
	const currentDate = new Date();

	//console.log(saleEndDate);
	//console.log(saleStartDate);
	//console.log(currentDate);

	if (saleStartDate <= currentDate) {
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

	if (saleStartDate <= currentDate && saleEndDate >= currentDate) {
		plainFormData['IsAvailable'] = true;
	} else {
		plainFormData['IsAvailable'] = false;
	}
	let isValid = true;

	const errorMessages = document.querySelectorAll('.error-message');
	errorMessages.forEach(errorMessage => errorMessage.textContent = '');

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
		//console.log('Form is valid.');
		//console.log(plainFormData);
		//form.submit();
		fetch('/Event/AddTicket', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify(plainFormData),
		})
			.then(response => {
				if (!response.ok) {
					throw new Error('Network response was not ok');
				}
				return response.json();
			})
			.then(data => {
				//console.log('Server response:', data);
				window.location.href = '/Event/ViewTickets?encryptedId=' + encodeURIComponent(encryptedEventId);
			})
			.catch(error => {
				console.log('Error', error);
			});

	} else {
		console.log('Form validation failed. Please fill in all required fields.');
	}

});