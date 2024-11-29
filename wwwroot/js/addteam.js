let teamCount = 2;
document.getElementById("add-team-btn").addEventListener("click", function () {

    const allCards = document.querySelectorAll(".team-card");
    teamCount = allCards.length + 1;

    const teamCard = document.createElement("div");
    teamCard.className = "team-card";
    teamCard.setAttribute("data-team-number", teamCount);
    teamCard.style.backgroundColor = getRandomColor();

    const teamLabel = document.createElement("div");
    teamLabel.id = `team-${teamCount}`;
    teamLabel.className = "col-sm-12 team-label";
    teamLabel.textContent = `Team ${teamCount}`;
    teamCard.appendChild(teamLabel);

    const pokemonInputDiv = document.createElement("div");
    pokemonInputDiv.className = "col-sm-12";

    fetch(`/Pokemon/RenderPokemonInput?teamId=${teamCount}`)
        .then(response => response.text())
        .then(html => {
            pokemonInputDiv.innerHTML = html;
            teamCard.appendChild(pokemonInputDiv);

            const removeButton = document.createElement("button");
            removeButton.className = "remove-team-btn";
            removeButton.textContent = "-";
            removeButton.addEventListener("click", function () {

                teamCard.remove();

                updateTeamNumbers();
            });
            teamCard.appendChild(removeButton);

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