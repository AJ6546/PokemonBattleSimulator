(function ($) {
    document.getElementById("pokemonInput").addEventListener("change", handlePokemonChange);
    document.getElementById("opponentPokemonInput").addEventListener("change", handlePokemonChange);

    function handlePokemonChange() {
        const selectedPokemon = this.value;

        var formData = selectedPokemon;

        fetch(`/Pokemon/GetPokemon`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: formData,
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then(data => {
                console.log("Response from server:", data);
            })
            .catch(error => {
                console.error("There was a problem with the fetch operation:", error);
            });
    }
})();
