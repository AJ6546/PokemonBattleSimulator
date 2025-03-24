document.addEventListener('DOMContentLoaded', function () {
    battle();
});

function battle() {
    const battleButton = document.getElementById("battle-btn");
    
    errorDiv.textContent = '';

    battleButton.addEventListener("click", function () {

        var teams = [];
        document.querySelectorAll('[id^="team_"][id$="_pokemon-container"]').forEach(teamContainer => {
            var teamId = teamContainer.id.split("_")[1];
            var pokemon  = [];

            teamContainer.querySelectorAll('[id="pokemon-details"]').forEach(pokemonContainer => {
                var pokemonId = parseInt(pokemonContainer.querySelector('[id="pokemon-id"]').textContent, 10);

                var selectedMoves = getSelectedMoves(pokemonContainer);

                var pokemonModel = {
                    Id: parseInt(pokemonId, 10),
                    Moves: selectedMoves
                };

                pokemon.push(pokemonModel);

            });

            if (pokemon.length < minimumNumberOfPokemonInATeam ||
                pokemon.length > maximumNumberOfPokemonInATeam) {
                showError(errorDiv, pokemonLimitError);
                return;
            }

            var team = {
                TeamId: parseInt(teamId, 10),
                Pokemon: pokemon 
            };

            teams.push(team);
        });


        if (teams.length < minimumNumberOfTeams ||
            teams.length > maximumNumberOfTeams) {
            showError(errorDiv, teamLimitError);
            return;
        }

        fetch('/Pokemon/Battle',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(teams)
        }).then(response => response.json())
            .then(data => {
                console.log(data.message);
                document.getElementById('battle-log').style.display = "block";
                document.getElementById('battle-log-details').innerText = data.log;
            }).catch(error => console.error('Error:', error));
    });
}

function getSelectedMoves(pokemonContainer) {
    var selectedMoves = [];

    pokemonContainer.querySelectorAll('.move-button.active').forEach(moveButton => {
        selectedMoves.push(moveButton.value); 
    });

    return selectedMoves;
}