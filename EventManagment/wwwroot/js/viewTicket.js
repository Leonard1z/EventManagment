﻿document.addEventListener('DOMContentLoaded', function () {
	var encryptedId = document.getElementById('encryptedId').innerText;
	var freeOption = document.getElementById("freeOption");
	var paidOption = document.getElementById("paidOption");
	const baseUrl = window.location.origin;
	getTickets(encryptedId);

	freeOption.addEventListener('click', function () {
		window.location.href = `${baseUrl}/Event/AddTicket?encryptedEventId=${encryptedId}&option=free`;
		$('#selectTicketTypeModal').modal('hide');
	});

	paidOption.addEventListener('click', function () {
		window.location.href = `${baseUrl}/Event/AddTicket?encryptedEventId=${encryptedId}&option=paid`;
		$('#selectTicketTypeModal').modal('hide');
	});
});

let allTicketsData = [];

function getTickets(encryptedId) {
	const baseUrl = window.location.origin;
	const ticketsUrl = `${baseUrl}/Event/GetTickets?encryptedId=${encryptedId}`;

	$.ajax({
		url: ticketsUrl,
		type: 'GET',
		dataType: 'json',
		success: function (response) {
			if (response.data.length > 0) {
				//console.log(response.data);
				allTicketsData = response.data;
				renderTickets(allTicketsData);
			} else {
				console.log('No tickets found for this event.');
				allTicketsData = response.data;
				renderTickets(allTicketsData);
			}
		},
		error: function () {
			toastr.error("Error occurred while retrieving tickets.");
		}
	});
}

function formatDate(dateString) {
	const options = {
		weekday: 'long',
		year: 'numeric',
		month: 'long',
		day: 'numeric',
		hour: 'numeric',
		minute: 'numeric',
		hour12: true,
	};

	const formattedDate = new Date(dateString).toLocaleDateString('en-US', options);
	return formattedDate;
}

function renderTickets(tickets) {
	var ticketBodyDataItemsContainer = document.querySelector('.ticketBodyDataItems');
	var ticketBodyDataDetailsContainer = document.querySelector('.ticketBodyDataDetails');
	var ticketActionsContainer = document.querySelector('.ticketBodyActions');

	ticketBodyDataItemsContainer.innerHTML = '';
	ticketBodyDataDetailsContainer.innerHTML = '';
	ticketActionsContainer.innerHTML = '';

	//console.log(tickets);

	tickets.forEach(function (ticket) {
		const ticketBodyData = document.createElement('div');
		ticketBodyData.classList.add('ticketBodyData');

		const ticketBodyTitle = document.createElement('div');
		ticketBodyTitle.classList.add('ticketBodyTitle');
		ticketBodyTitle.innerHTML = 
			`
				<div class="ticketName">
					<h1>${ticket.name}</h1>
				</div>
				<div class="ticketDate">
					<span>Start: ${formatDate(ticket.saleStartDate)}</span >
				</div>
				<div class="ticketDate">
					<span>End: ${formatDate(ticket.saleEndDate)}</span >
				</div>
			`;
		const ticketBodyTotalQuantity = document.createElement('div');
		ticketBodyTotalQuantity.classList.add('ticketBodyTotalQuantity');
		ticketBodyTotalQuantity.innerHTML =
			`
					<div class="totalTickets">
						<h1>${ticket.totalTickets}</h1>
					</div>
			`

		ticketBodyData.appendChild(ticketBodyTitle);
		ticketBodyData.appendChild(ticketBodyTotalQuantity);
		ticketBodyDataItemsContainer.appendChild(ticketBodyData);

		const ticketAvailablity = ticket.isAvailable ? 'Available' : 'Not Available';

		const ticketDataDetails = document.createElement('div');
		ticketDataDetails.classList.add('ticketDataDetails');
		ticketDataDetails.innerHTML = 
			`

				<div class="ticketQuantity">
					<h1>${ticket.quantity}</h1>
				</div>
				<div class="ticketPrice">
					<h1>${(ticket.price == 0 ? "Free" : ticket.price)}</h1>
				</div>
				<div class="ticketStatus">
					<h1>${ticketAvailablity}</h1>
				</div>
			`
		ticketBodyDataDetailsContainer.appendChild(ticketDataDetails);


		const ticketItemActions = document.createElement('div');
		ticketItemActions.classList.add('ticketActions');

		const ticketActionsDropdown = document.createElement('div');
		ticketActionsDropdown.classList.add('ticketActionsDropdown', `ticketId-${ticket.encryptedId}`, 'hidden');
		const actions = [
			{ label: 'Edit', icon: 'fa-edit', action: () => editTicket(ticket.encryptedId) },
			{ label: 'Delete', icon: 'fa-trash', action: () => deleteTicket(ticket.encryptedId) },

		];

		actions.forEach(action => {
			const actionItem = document.createElement('div');
			actionItem.classList.add('actionItem');
			actionItem.innerHTML = `<i class="fa-solid ${action.icon}"></i> ${action.label}`;
			actionItem.addEventListener('click', action.action);
			ticketActionsDropdown.appendChild(actionItem);
		});

		const ticketActions = document.createElement('div');
		ticketActions.classList.add('actions');
		ticketActions.innerHTML = `
			<i class="fa-solid fa-ellipsis-vertical" onclick="toggleDropdown(event, '${ticket.encryptedId}')"></i>
		`;

		ticketItemActions.appendChild(ticketActionsDropdown);
		ticketItemActions.appendChild(ticketActions);
		ticketActionsContainer.appendChild(ticketItemActions);

	});
}

function toggleDropdown(event, encryptedTicketId) {
	const dropdown = document.querySelector(`.ticketActionsDropdown.ticketId-${encryptedTicketId}`);
	dropdown.classList.toggle('visible');
	//console.log(dropdown)

	const allDropdowns = document.querySelectorAll('.ticketActionsDropdown');
	allDropdowns.forEach(otherDropdown => {
		if (otherDropdown !== dropdown && otherDropdown.classList.contains('visible')) {
			otherDropdown.classList.remove('visible')
		}
	});

	event.stopPropagation();
}

document.addEventListener('click', () => {
	// Close all dropdowns when clicking outside
	const allDropdowns = document.querySelectorAll('.ticketActionsDropdown');
	allDropdowns.forEach(dropdown => dropdown.classList.remove('visible'));
});

const searchInput = document.getElementById('searchInput');
searchInput.addEventListener('input', function () {
	filterTickets(searchInput.value.toLowerCase());
})

function filterTickets(searchTerm) {
	const filteredTickets = allTicketsData.filter(ticket => {
		return ticket.name.toLowerCase().includes(searchTerm)
	});

	renderTickets(filteredTickets);
}
function editTicket(ticketId) {
	const baseUrl = window.location.origin;	
	window.location.href = `${baseUrl}/Ticket/Edit?encryptedId=${ticketId}`;
}
function deleteTicket(encryptedId) {
	const baseUrl = window.location.origin;
	var url = `${baseUrl}/api/ticket/DeleteTicket?encryptedId=${encryptedId}`;
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
			fetch(url, {
				method: 'DELETE',
			})
				.then(response => response.json())
				.then(data => {
					if (data.success) {
						toastr.success(data.message);
						var encryptedEventId = document.getElementById('encryptedId').innerText;
						getTickets(encryptedEventId);

					} else {
						toastr.error(data.message);
					}
				})
				.catch(error => {
					console.error('Error occurred during the delete operation:', error);
					toastr.error("Error occurred during the delete operation.");
				});
		}
	});
}