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

    var ratingButton = document.getElementById('open-rating');
    var ratingDialog = document.getElementById('ratingDialog');
    var closeRating = document.getElementById('close-rating');
    if (ratingButton && ratingDialog) {
        ratingButton.addEventListener('click', function () {
            ratingDialog.classList.remove('hidden');
        });
    }
    if (closeRating && ratingDialog) {
        closeRating.addEventListener('click', function () {
            ratingDialog.classList.add('hidden');
        });
    }
});