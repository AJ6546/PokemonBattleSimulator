﻿using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IGetSelectedPokemonDetails
    {
        public Task ExecuteAsync(PokemonModel pokemon);
    }
}
