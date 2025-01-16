document.addEventListener('DOMContentLoaded', function () {
    handleDropdownSelect();
});

function handleDropdownSelect() {
    var dropdowns = document.querySelectorAll('.pokemon-dropdown');
    dropdowns.forEach(function(dropdown) {
        dropdown.addEventListener('change', function(event) {
            var selectedDropdownId = event.target.id;
            var selectedValue = event.target.value;

            getPokemon(selectedValue, selectedDropdownId);
        });
    });
}

function getPokemon(pokemonId, dropdownId) {
    fetch('/Pokemon/GetPokemon', {
        method: 'POST',  
        headers: {
            'Content-Type': 'application/json'  
        },
        body: JSON.stringify(pokemonId)  
    })
        .then(response => response.text())  
        .then(html => {
            var targetDivId = dropdownId.replace("pokemon", "selected-pokemon");
            var targetDiv = document.getElementById(targetDivId);
            if (targetDiv) {
                targetDiv.innerHTML = html;
                assignMoveButtonIds();
                handleMoveSelection();
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
}
