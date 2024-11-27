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
        });
    });
}

function addPokemonDropdown(teamId) {
    var container = document.querySelector(`#team_${teamId}_pokemon-container`);

    var newPokemonDiv = document.createElement('div');
    newPokemonDiv.classList.add('col-sm-12', 'pokemon');

    newPokemonDiv.innerHTML = `
        <div class="input-group">
            <select class="form-control pokemon-dropdown" id="team_${teamId}_pokemon_${container.children.length + 1}">
                ${template.innerHTML}
            </select>
            <button type="button" class="btn btn-danger remove-pokemon-btn" id="remove_team_${teamId}_pokemon_${container.children.length + 1}">-</button>
        </div>
        <div id="selected-pokemon-${container.children.length + 1}"></div>
    `;

    container.appendChild(newPokemonDiv);

    var removeButton = newPokemonDiv.querySelector('.remove-pokemon-btn');
    attachRemoveListener(removeButton,container, teamId);
}

function attachRemoveListener(button, container, teamId) {
    button.addEventListener('click', () => {
        var pokemonDiv = button.closest('.pokemon');
        pokemonDiv.remove();
        updatePokemonIds(container, teamId);
    });
}

function updatePokemonIds(container, teamId) {
    var pokemonDivs = Array.from(container.querySelectorAll('.pokemon')); 
    pokemonDivs.slice(1).forEach((div, index) => {
        var newIndex = index + 2;

        var dropdown = div.querySelector('.pokemon-dropdown');
        dropdown.id = `team_${teamId}_pokemon_${newIndex}`;

        var removeButton = div.querySelector('.remove-pokemon-btn');
        removeButton.id = `remove_team_${teamId}_pokemon_${newIndex}`;

        var selectedPokemonDiv = div.querySelector('[id^="selected-pokemon-"]');
        selectedPokemonDiv.id = `selected-pokemon-${newIndex}`;
    });
}
