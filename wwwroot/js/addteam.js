let teamCount = 2;
var errorDiv = document.getElementById("error-text");

const pokemonLimitError = `The number of pokemon in each team must be between ${minimumNumberOfPokemonInATeam} 
and ${maximumNumberOfPokemonInATeam}.`;
const teamLimitError = `The number of teams must be ${minimumNumberOfTeams} 
and ${maximumNumberOfTeams}.`;

document.getElementById("add-team-btn").addEventListener("click", function () {

    errorDiv.textContent = '';

    var allCards = document.querySelectorAll(".team-card");
    teamCount = allCards.length + 1;

    if (teamCount > maximumNumberOfTeams) {
        showError(errorDiv, teamLimitError);
        return;
    }

    var teamCard = document.createElement("div");
    teamCard.className = "team-card";
    teamCard.setAttribute("data-team-number", teamCount);
    teamCard.style.backgroundColor = getRandomColor();

    var teamLabel = document.createElement("div");
    teamLabel.id = `team-${teamCount}`;
    teamLabel.className = "col-sm-12 team-label";
    teamLabel.textContent = `Team ${teamCount}`;
    teamCard.appendChild(teamLabel);

    var pokemonInputDiv = document.createElement("div");
    pokemonInputDiv.className = "col-sm-12";

    fetch(`/Pokemon/RenderPokemonInput?teamId=${teamCount}`)
        .then(response => response.text())
        .then(html => {
            pokemonInputDiv.innerHTML = html;
            teamCard.appendChild(pokemonInputDiv);

            var removeButton = document.createElement("button");
            removeButton.className = "remove-team-btn";
            removeButton.textContent = "-";
            removeButton.addEventListener("click", function () {

                teamCard.remove();

                updateTeamNumbers();
            });
            pokemonInputDiv.appendChild(removeButton);

            document.getElementById("teams-container").appendChild(teamCard);
            initializeAddPokemon();
            handleDropdownSelect();
        })
        .catch(error => console.error("Error fetching partial view:", error));
});

function updateTeamNumbers() {

    const teamCards = document.querySelectorAll("#teams-container .team-card");
    teamCards.forEach((card, index) => {
        const label = card.querySelector(".team-label");
        label.textContent = `Team ${index + 1}`;
    });
    teamCount = teamCards.length;
}

function getRandomColor() {
    const randomColor = `rgb(${Math.floor(Math.random() * 151) + 50}, 
                             ${Math.floor(Math.random() * 151) + 50}, 
                             ${Math.floor(Math.random() * 151) + 50})`;
    return randomColor;
}

function showError(errorDiv, message) {
    errorDiv.textContent = message;
}
