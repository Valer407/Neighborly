document.addEventListener('DOMContentLoaded', function () {
    var filterButton = document.getElementById('filter-button');
    var filterPanel = document.getElementById('filter-panel');
    var closeFilter = document.getElementById('close-filter');

    if (filterButton && filterPanel) {
        filterButton.addEventListener('click', function () {
            filterPanel.classList.toggle('hidden');
            filterPanel.classList.toggle('flex');
        });
    }

    if (closeFilter && filterPanel) {
        closeFilter.addEventListener('click', function () {
            filterPanel.classList.add('hidden');
            filterPanel.classList.remove('flex');
        });
    }
});