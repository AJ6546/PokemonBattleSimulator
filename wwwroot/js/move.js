document.addEventListener('DOMContentLoaded', function () {
    assignMoveButtonIds();
    handleMoveSelection();
});

const maxNumberOfMoves = 4;

function assignMoveButtonIds() {
    var teamPokemonDivs = document.querySelectorAll('[id^="team_"][id*="_selected-pokemon_"]');

    teamPokemonDivs.forEach(teamPokemonDiv => {
        var parentId = teamPokemonDiv.id;

        var [teamId, pokemonId] = parentId.match(/team_(\d+)_selected-pokemon_(\d+)/)
            .slice(1);

        var moveButtons = teamPokemonDiv.querySelectorAll('.move-button');

        moveButtons.forEach((button, index) => {
            button.id = `team_${teamId}_pokemon_${pokemonId}_move_${index}`;
        });
    });
}

function handleMoveSelection() {
    changeColorOnSelect();
    getSelectedMove();
}

function changeColorOnSelect() {
    var moveButtons = getAllMoveButtons();

    var selectedButtons = {};

    moveButtons.forEach(button => {

        var newButton = button.cloneNode(true);

        button.parentNode.replaceChild(newButton, button);

        newButton.addEventListener('click', () => {
            var [teamId, pokemonId] = getTeamAndPokemonIds(button.id);

            var key = `${teamId}_${pokemonId}`;
            if (!selectedButtons[key]) {
                selectedButtons[key] = [];
            }

            if (newButton.classList.contains('active')) {
                newButton.classList.remove('active');
                selectedButtons[key] = selectedButtons[key].filter(b => b !== newButton);
            } else {
                newButton.classList.add('active');
                selectedButtons[key].push(newButton);
                if (selectedButtons[key].length > maxNumberOfMoves) {
                    const firstButton = selectedButtons[key].shift(); 
                    firstButton.classList.remove('active');
                }
            }
        });
    });
}

function getTeamAndPokemonIds(buttonId) {
    var match = buttonId.match(/team_(\d+)_pokemon_(\d+)_/);
    if (match) {
        return [match[1], match[2]]; 
    }
    return [null, null];
}

function getSelectedMove() {
    var moveButtons = getAllMoveButtons();

    moveButtons.forEach(button => {
        button.addEventListener('click', function () {

            if (button.classList.contains('active')) {
                var moveName = button.value;
                getMove(moveName, button);
            } 
        });
    });
}

function getMove(moveName, button) {
    fetch('/Pokemon/GetMove', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(moveName)
    })
        .then(response => response.text())
        .then(html => {
            var targetDiv = button.closest('.card-body').querySelector('#selectedMove');
            if (targetDiv) {
                targetDiv.innerHTML = html;
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

function getAllMoveButtons() {
    var moveButtons = document.querySelectorAll('[id^="team_"][id*="_pokemon_"][id*="_move_"]');
    return moveButtons;
}