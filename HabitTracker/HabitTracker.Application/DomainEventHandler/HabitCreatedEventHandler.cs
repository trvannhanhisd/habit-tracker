using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Events;
using HabitTracker.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace HabitTracker.Application.DomainEventHandler
{
    public class HabitCreatedEventHandler : INotificationHandler<HabitCreatedEvent>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HabitCreatedEventHandler> _logger;

        public HabitCreatedEventHandler(
            IHabitRepository habitRepository,
            IHttpClientFactory httpClientFactory,
            ILogger<HabitCreatedEventHandler> logger)
        {
            _habitRepository = habitRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Handle(HabitCreatedEvent notification, CancellationToken cancellationToken)
        {
            var pokemonType = MapCategoryToPokemonType(notification.Category);
            var petName = await GetRandomPokemonName(pokemonType, cancellationToken);

            var habit = await _habitRepository.GetHabitByIdAsync(notification.HabitId, cancellationToken);
            if (habit == null)
            {
                _logger.LogWarning("Habit {HabitId} not found when handling HabitCreatedEvent", notification.HabitId);
                return;
            }

            var canEvolve = await CanPokemonEvolve(petName, cancellationToken);
            habit.PetName = petName;
            habit.CanEvolve = canEvolve;

            await _habitRepository.UpdateHabitAsync(habit, cancellationToken);
            // Note: SaveChangesAsync is handled by HabitDbContext
            _logger.LogInformation("Assigned petName {PetName} with CanEvolve {CanEvolve} to habit {HabitId}", petName, canEvolve, notification.HabitId);
        }

        private string MapCategoryToPokemonType(Habit.HabitCategory category)
        {
            return category switch
            {
                Habit.HabitCategory.General => "normal",
                Habit.HabitCategory.Health => "grass",
                Habit.HabitCategory.Fitness => "fighting",
                Habit.HabitCategory.Study => "psychic",
                Habit.HabitCategory.Work => "steel",
                Habit.HabitCategory.Finance => "rock",
                Habit.HabitCategory.SelfGrowth => "fairy",
                Habit.HabitCategory.Social => "flying",
                Habit.HabitCategory.Creative => "ghost",
                Habit.HabitCategory.Environment => "bug",
                _ => "normal"
            };
        }

        private async Task<string> GetRandomPokemonName(string pokemonType, CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient("PokeAPI");
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    var response = await httpClient.GetAsync($"type/{pokemonType.ToLower()}", cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    var pokemonTypeData = JsonSerializer.Deserialize<PokemonTypeResponse>(json);

                    if (pokemonTypeData?.Pokemon == null || pokemonTypeData.Pokemon.Length == 0)
                        return "Pikachu";

                    // 🎯 Lọc ra chỉ base form
                    var filtered = pokemonTypeData.Pokemon
                        .Select(p => p.Pokemon.Name)
                        .Where(name => !IsAlternateForm(name))
                        .ToList();

                    if (filtered.Count == 0)
                        filtered = pokemonTypeData.Pokemon.Select(p => p.Pokemon.Name).ToList();

                    var random = new Random();
                    return filtered[random.Next(filtered.Count)];
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Attempt {Attempt} failed to fetch Pokémon for type {PokemonType}", attempt + 1, pokemonType);
                    if (attempt == 2) return "Pikachu";
                    await Task.Delay(1000, cancellationToken);
                }
            }
            return "Pikachu";
        }

        private bool IsAlternateForm(string name)
        {
            string[] bannedKeywords = new[]
            {
                "mega", "alola", "galar", "hisui", "paldea",
                "totem", "gmax", "starter", "ash", "cap",
                "cosplay", "busted", "zen", "school",
                "original", "belle", "blade", "attack",
                "defense", "speed", "wash", "fan", "frost", "heat", "mow"
            };

            return bannedKeywords.Any(k => name.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> CanPokemonEvolve(string pokemonName, CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient("PokeAPI");
            try
            {
                var response = await httpClient.GetAsync($"pokemon-species/{pokemonName.ToLower()}", cancellationToken);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var speciesData = JsonSerializer.Deserialize<PokemonSpeciesResponse>(json);
                return speciesData?.EvolutionChain != null && speciesData.EvolutionChain.Url != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check evolution for Pokémon {PokemonName}", pokemonName);
                return false; // Nếu không lấy được dữ liệu, coi như không tiến hóa
            }
        }

        private class PokemonTypeResponse
        {
            [JsonPropertyName("pokemon")]
            public PokemonEntry[] Pokemon { get; set; }
        }

        private class PokemonEntry
        {
            [JsonPropertyName("pokemon")]
            public PokemonData Pokemon { get; set; }
        }

        private class PokemonData
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        private class PokemonSpeciesResponse
        {
            [JsonPropertyName("evolution_chain")]
            public EvolutionChain EvolutionChain { get; set; }
        }

        private class EvolutionChain
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }
    }
}
