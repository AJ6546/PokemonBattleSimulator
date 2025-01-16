document.addEventListener('DOMContentLoaded', () => {

    initializeAddPokemon();
});

const template = document.querySelector('#pokemon-options-template');

function initializeAddPokemon() {
    var buttons = document.querySelectorAll('[id^="team_"][id$="_add-pokemon-btn"]');

    buttons.forEach(button => {
        var newButton = button.cloneNode(true);

        button.parentNode.replaceChild(newButton, button);

        newButton.addEventListener('click', (event) => {
            var buttonId = event.target.id;
            var teamId = buttonId.match(/team_(\d+)_add-pokemon-btn/)[1];

            addPokemonDropdown(teamId);
            handleDropdownSelect();
        });
    });
}

function addPokemonDropdown(teamId) {

    errorDiv.textContent = '';

    var container = document.querySelector(`#team_${teamId}_pokemon-container`);

    var pokemonRow = document.createElement('div');
    pokemonRow.classList.add('row');
    polemonCount = container.children.length + 1;

    if (polemonCount > maximumNumberOfPokemonInATeam) {
        showError(errorDiv, pokemonLimitError);
        return;
    }

    pokemonRow.id = `team_${teamId}_pokemon_${polemonCount}_row`;

    var newPokemonDiv = document.createElement('div');
    newPokemonDiv.classList.add('col-sm-2', 'pokemon');

    newPokemonDiv.innerHTML = `
        <div class="input-group">
            <select class="form-control pokemon-dropdown" id="team_${teamId}_pokemon_${container.children.length + 1}">
                ${template.innerHTML}
            </select>
            <button type="button" class="btn btn-danger remove-pokemon-btn" id="remove_team_${teamId}_pokemon_${container.children.length + 1}">-</button>
        </div>
    `;

    var selectedPokemonDiv = document.createElement('div');
    selectedPokemonDiv.classList.add('col-sm-10','selected-pokemon');

    selectedPokemonDiv.innerHTML = `
         <div id="team_${teamId}_selected-pokemon_${container.children.length + 1}"></div>
        </div>
    `;

    pokemonRow.appendChild(newPokemonDiv);
    pokemonRow.appendChild(selectedPokemonDiv);

    container.appendChild(pokemonRow);

    var removeButton = newPokemonDiv.querySelector('.remove-pokemon-btn');
    attachRemoveListener(removeButton,container, teamId);
}

function attachRemoveListener(button, container, teamId) {
    button.addEventListener('click', () => {
        var removedIndex = button.id.split('_').pop();
        var pokemonRowId = `team_${teamId}_pokemon_${removedIndex}_row`;
        var pokemonRow = document.getElementById(pokemonRowId); 
        if (pokemonRow) {
            pokemonRow.remove(); 
        }
        updatePokemonIds(container, teamId, removedIndex);
    });
}

function updatePokemonIds(container, teamId, removedIndex) {

    var pokemonRows = Array.from(container.querySelectorAll(`[id^="team_${teamId}_pokemon_"][id$="_row"]`));

    pokemonRows.forEach(row => {

        var currentIndex = parseInt(row.id.match(/pokemon_(\d+)_row/)[1], 10);

        if (currentIndex > removedIndex) {
            var newIndex = currentIndex - 1;
            row.id = `team_${teamId}_pokemon_${newIndex}_row`;

            var dropdown = row.querySelector(`[id^="team_${teamId}_pokemon_"]`);
            if (dropdown) {
                dropdown.id = `team_${teamId}_pokemon_${newIndex}`;
            }

            var selectedPokemonDiv = row.querySelector(`[id^="team_${teamId}_selected-pokemon_"]`);
            if (selectedPokemonDiv) {
                selectedPokemonDiv.id = `team_${teamId}_selected-pokemon_${newIndex}`;
            }

            var removeButton = row.querySelector(`[id^="remove_team_${teamId}_pokemon_"]`);
            if (removeButton) {
                removeButton.id = `remove_team_${teamId}_pokemon_${newIndex}`;
            }
        }
    });
}
