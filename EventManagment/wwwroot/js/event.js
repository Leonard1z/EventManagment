
document.addEventListener('DOMContentLoaded', function () {
    fetchEvents(); 
});

let allEventsData = [];
const eventsPerPage = 6;
let currentPage = 1;

async function fetchEvents() {
    try {
        const response = await fetch('/GetAllEvents');

        if (!response.ok) {
            throw new Error(`Failed to fetch events. Status: ${response.status}`);
        }
        const responseData = await response.json();

        if (Array.isArray(responseData.data)) {
            allEventsData = responseData.data;
            renderEvents(allEventsData);
            //console.log(responseData.data);
        } else {
            console.error('Error: Invalid data format', data);
        }
    } catch (error) {
        console.error('Error fetching events:', error.message);
    }
}

function renderEvents(events) {
    // Check if currentPage is within valid bounds
    const maxPage = Math.ceil(allEventsData.length / eventsPerPage);
    //console.log(maxPage);
    if (currentPage < 1) {
        currentPage = 1;
    } else if (currentPage > maxPage) {
        currentPage = maxPage;
    }

    const eventContainer = document.querySelector('.event-data-body');
    const eventContainerMetric = document.querySelector('.event-body-metrics');
    const eventContainerActions = document.querySelector('.event-body-actions');

    eventContainer.innerHTML = '';
    eventContainerMetric.innerHTML = '';
    eventContainerActions.innerHTML = '';

    if (!events) {
        console.error('Error: Events data is undefined.');
        return;
    }

    // Calculate the start and end index for the current page
    const startIndex = (currentPage - 1) * eventsPerPage;
    const endIndex = startIndex + eventsPerPage;
    // Slice the events array based on the startIndex and endIndex
    const eventsToRender = events.slice(startIndex, endIndex);

    eventsToRender.forEach(event => {

        const eventItem = document.createElement('div');
        eventItem.classList.add('event-data-items');

        const eventImage = document.createElement('div');
        eventImage.classList.add('event-image');
        eventImage.innerHTML = `<img src="${event.image}" alt="Event Image">`;

        const eventDetails = document.createElement('div');
        eventDetails.classList.add('event-details');
        eventDetails.innerHTML = `
            <div class="event-details-title">
                <h1>${event.name}</h1>
            </div>
            <div class="event-details-date">
                <span>${event.startDate}</span>
            </div>
        `;

        eventItem.appendChild(eventImage);
        eventItem.appendChild(eventDetails);
        eventContainer.appendChild(eventItem);

        const eventItemMetrics = document.createElement('div');
        eventItemMetrics.classList.add('event-metrics-data');

        const eventMetricSold = document.createElement('div');
        eventMetricSold.classList.add('metrics-sold');
        eventMetricSold.innerHTML = `
        <div class="event-metrics-sold">
            <p>${event.sold}</p>
        </div>
        `
        const eventMetricGross = document.createElement('div');
        eventMetricGross.classList.add('metrics-gross');
        eventMetricGross.innerHTML = `
        <div class="event-metrics-gross">
            <p>${event.gross}</p>
        </div>
        `
        const eventStatus = document.createElement('div');
        eventStatus.classList.add('event-status');
        eventStatus.innerHTML = `
        <div class="event-metrics-status">
            <p>${event.status}</p>
        </div>
        `

        eventItemMetrics.appendChild(eventMetricSold);
        eventItemMetrics.appendChild(eventMetricGross);
        eventItemMetrics.appendChild(eventStatus);
        eventContainerMetric.appendChild(eventItemMetrics);

        const eventItemActions = document.createElement('div');
        eventItemActions.classList.add('event-actions');

        const eventActionsDropdown = document.createElement('div');
        eventActionsDropdown.classList.add('event-actions-dropdown', `event-id-${event.encryptedId}`, 'hidden');
        const actions = [
            { label: 'Edit', icon: 'fa-edit', action: () => editEvent(event.encryptedId) },
            { label: 'Delete', icon: 'fa-trash', action: () => deleteEvent(event.encryptedId) },
            { label: 'View', icon: 'fa-info-circle', action: () => viewEventDetails(event.encryptedId) }
        ];

        actions.forEach(action => {
            const actionItem = document.createElement('div');
            actionItem.classList.add('action-item');
            actionItem.innerHTML = `<i class="fa-solid ${action.icon}"></i> ${action.label}`;
            actionItem.addEventListener('click', action.action);
            eventActionsDropdown.appendChild(actionItem);
        });

        const eventActions = document.createElement('div');
        eventActions.classList.add('actions');
        eventActions.innerHTML = `
            <i class="fa-solid fa-ellipsis-vertical" onclick="toggleDropdown(event, '${event.encryptedId}')"></i>
        `;

        eventItemActions.appendChild(eventActionsDropdown);
        eventItemActions.appendChild(eventActions);
        eventContainerActions.appendChild(eventItemActions);
    });
}

function toggleDropdown(event, eventId) {
    //console.log(event);
    const dropdown = document.querySelector(`.event-actions-dropdown.event-id-${eventId}`);
    dropdown.classList.toggle('visible');
    //console.log(dropdown)

    const allDropdowns = document.querySelectorAll('.event-actions-dropdown');
    allDropdowns.forEach(otherDropdown => {
        if (otherDropdown !== dropdown && otherDropdown.classList.contains('visible')) {
            otherDropdown.classList.remove('visible')
        }
    });

    event.stopPropagation();
}

document.addEventListener('click', () => {
    // Close all dropdowns when clicking outside
    const allDropdowns = document.querySelectorAll('.event-actions-dropdown');
    allDropdowns.forEach(dropdown => dropdown.classList.remove('visible'));
});

const statusTab = document.querySelector('.status-tab');
statusTab.addEventListener('click', function () {
    const statusDropdown = document.getElementById('statusDropdown');
    statusDropdown.classList.toggle('hide');
    statusDropdown.classList.toggle('show');
});

const statusOptions = document.querySelectorAll('#statusDropdown li');
statusOptions.forEach(option => {
    option.addEventListener('click', function () {
        const selectedStatus = option.dataset.status;
        updateStatusTab(selectedStatus);
        toggleStatusDropdown();

        if (selectedStatus === 'All') {
            renderEvents(allEventsData);
        } else {
            const filteredEvents = allEventsData.filter(event => event.status === selectedStatus);
            renderEvents(filteredEvents);
        }
    });
});

function updateStatusTab(status) {
    //console.log(status);
    const statusTab = document.querySelector('.status-tab');
    const chevronIcon = '<i class="fa-solid fa-chevron-down"></i>';
    statusTab.dataset.status = status;
    statusTab.innerHTML = status === 'All' ? `All Events ${chevronIcon}` : `${status} ${chevronIcon}`;
}

function toggleStatusDropdown() {
    const dropdown = document.getElementById('statusDropdown');
    dropdown.classList.toggle('hide');
}

const searchInput = document.getElementById('searchInput');
searchInput.addEventListener('input', function () {
    filterEvents(searchInput.value.toLowerCase());
});

function filterEvents(searchTerm) {
    const filteredEvents = allEventsData.filter(event => {
        return event.name.toLowerCase().includes(searchTerm) ||
            event.startDate.toLowerCase().includes(searchTerm) ||
            event.status.toLowerCase().includes(searchTerm);
    });

    renderEvents(filteredEvents);
}

document.getElementById('prevPage').addEventListener('click', () => {
    if (currentPage > 1) {
        currentPage--;
        updatePagination();
        renderEvents(allEventsData);
    }
});

document.getElementById('nextPage').addEventListener('click', () => {
    const maxPage = Math.ceil(allEventsData.length / eventsPerPage);
    if (currentPage < maxPage) {
        currentPage++;
        updatePagination();
        renderEvents(allEventsData);
    }
});

function updatePagination() {
    const maxPage = Math.ceil(allEventsData.length / eventsPerPage);
    document.getElementById('currentPage').textContent = currentPage + '/' + maxPage;
}

function viewEventDetails(encryptedId) {
    //console.log(encryptedId);
    const baseUrl = window.location.origin;
    window.location.href = `${baseUrl}/EventDetails?encryptedId=${encryptedId}`;
}

function editEvent(encryptedId) {
    //console.log(encryptedId);
    const baseUrl = window.location.origin;
    window.location.href = `${baseUrl}/Event/Edit?encryptedId=${encryptedId}`;
}

function deleteEvent(encryptedId) {
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
            fetch(url, {
                method: 'DELETE',
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    toastr.success(data.message);
                    fetchEvents();
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