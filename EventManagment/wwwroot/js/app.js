function toggleFullScreen() {
	let expandIcon = document.getElementById('expand-icon');
	expandIcon.classList.toggle('fa-expand');
	expandIcon.classList.toggle('fa-compress');

	if (expandIcon.classList.contains('fa-expand')) {
		exitFullScreen();
	} else {
		enterFullScreen();
	}
}

function enterFullScreen() {
	let element = document.documentElement;

	if (element.requestFullscreen) {
		element.requestFullscreen();
	} else if (element.mozRequestFullScreen) {
		element.mozRequestFullScreen();
	} else if (element.webkitRequestFullscreen) {
		element.webkitRequestFullscreen();
	} else if (element.msRequestFullscreen) {
		element.msRequestFullscreen();
	}
}

function exitFullScreen() {
	if (document.exitFullscreen) {
		document.exitFullscreen();
	} else if (document.mozCancelFullScreen) {
		document.mozCancelFullScreen();
	} else if (document.webkitExitFullscreen) {
		document.webkitExitFullscreen();
	} else if (document.msExitFullscreen) {
		document.msExitFullscreen();
	}
}

function toggleMenu() {
	let navigation = document.getElementById('navigation-bar');
	let barsIcon = document.getElementById('toggle-sidebar');
	let mainContent = document.getElementById('main-container');
	let menuTitle = document.querySelector('.menu-title');
	let listTitles = document.querySelectorAll('.title')

	navigation.classList.toggle('active');
	barsIcon.classList.toggle('active');

	let isMenuActive = navigation.classList.contains('active');
	localStorage.setItem('menuState', isMenuActive);
	//console.log(!isMenuActive);
	//console.log(listTitles);

	if (!isMenuActive) {
		mainContent.style.marginLeft = '65px';
		menuTitle.innerHTML = '<i class="fa-solid fa-chart-line"></i>';
		menuTitle.style.fontSize = '19px';
		listTitles.forEach(title => {
			title.style.visibility = 'hidden';
		});
	} else {
		mainContent.style.marginLeft = '250px';
		menuTitle.innerHTML = 'EventManagement';
		menuTitle.style.fontSize = '14px';
		listTitles.forEach(title => {
			title.style.visibility = 'visible';
		});
	}
}

window.addEventListener('DOMContentLoaded', function () {
	let isMenuActive = localStorage.getItem('menuState');
	//console.log(isMenuActive);
	if (isMenuActive === 'false') {
		toggleMenu();
	}
});

function checkScreenSizeAndToggleMenu() {
	if (window.matchMedia("(max-width: 767px)").matches) {
		let isMenuActive = localStorage.getItem('menuState');

		if (isMenuActive === 'true') {
			toggleMenu();
		}
	}
}
// Call the function when the page loads and on window resize
window.addEventListener('DOMContentLoaded', checkScreenSizeAndToggleMenu);
window.addEventListener('resize', checkScreenSizeAndToggleMenu);

document.getElementById('toggle-dropdown').addEventListener('click', function () {
	let dropdownMenu = document.querySelector('.menu-dropdown');
	dropdownMenu.classList.toggle('show');
});

document.addEventListener('click', function (event) {
	let toggleDropdown = document.getElementById('toggle-dropdown');
	let dropdownMenu = document.querySelector('.menu-dropdown');

	if (!toggleDropdown.contains(event.target) && !dropdownMenu.contains(event.target)) {
		dropdownMenu.classList.remove('show');
	}
});