document.addEventListener('DOMContentLoaded', function () {
    const input = document.getElementById('profileImage');
    const dropContainer = document.getElementById('imagePreview');

    function handleFile(file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            dropContainer.innerHTML = `<img src="${e.target.result}" alt="Profile picture">`;
        };

        reader.readAsDataURL(file);
    }

    function handleFileInput() {
        const file = input.files[0];
        console.log('Selected file:', file);

        if (file) {
            handleFile(file);
        }
    }

    function handleDragAndDrop(e) {
        e.preventDefault();
        dropContainer.classList.toggle('drag-over', e.type === 'dragover');

        const file = e.dataTransfer.files[0];

        if (file) {
            handleFile(file);
        }
    }

    input.addEventListener('change', handleFileInput);

    ['dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropContainer.addEventListener(eventName, handleDragAndDrop);
    });
});