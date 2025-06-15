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
    var starInputs = ratingDialog ? ratingDialog.querySelectorAll('input[name="score"]') : [];
    var updateStars = function (value) {
        if (!ratingDialog) return;
        var svgs = ratingDialog.querySelectorAll('svg.rating-star');
        svgs.forEach(function (svg, index) {
            if (index < value) {
                svg.classList.add('fill-yellow-400', 'text-yellow-400', 'stroke-yellow-400');
                svg.classList.remove('text-gray-300', 'stroke-gray-300');
            } else {
                svg.classList.remove('fill-yellow-400', 'text-yellow-400', 'stroke-yellow-400');
                svg.classList.add('text-gray-300', 'stroke-gray-300');
            }
        });
    };
    starInputs.forEach(function (input) {
        input.addEventListener('change', function () {
            updateStars(parseInt(this.value));
        });
    });
});