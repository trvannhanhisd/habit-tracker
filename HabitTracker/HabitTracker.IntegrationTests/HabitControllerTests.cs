using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HabitTracker.Application.Features.Habits.Commands.CreateHabit;
using HabitTracker.Application.Features.Habits.Commands.UpdateHabit;
using HabitTracker.API.Models;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Domain.Entity;

namespace HabitTracker.IntegrationTests
{
    public class HabitControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HabitControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private static async Task<T?> SafeReadJsonAsync<T>(HttpResponseMessage response)
        {
            try
            {
                if (response.Content.Headers.ContentLength > 0)
                    return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] JSON parse error: {ex.Message}\nRaw content: {raw}");
            }
            return default;
        }

        [Fact]
        public async Task CreateHabit_ShouldReturn201()
        {
            var command = new CreateHabitCommand
            {
                Title = "Drink water",
                Description = "Drink 8 glasses per day",
                Frequency = Habit.HabitFrequency.Daily,
            };

            var response = await _client.PostAsJsonAsync("/api/habit", command);
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] CreateHabit response: {raw}");

            response.StatusCode.Should().Be(HttpStatusCode.Created, raw);

            var apiResponse = await SafeReadJsonAsync<ApiResponse<HabitViewModel>>(response);
            apiResponse.Should().NotBeNull();
            apiResponse!.Data.Title.Should().Be("Drink water");
        }

        [Fact]
        public async Task GetHabitById_ShouldReturnHabit()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/habit", new CreateHabitCommand
            {
                Title = "Run 5km",
                Frequency = Habit.HabitFrequency.Daily,
            });

            var created = await SafeReadJsonAsync<ApiResponse<HabitViewModel>>(createResponse);
            created.Should().NotBeNull();
            var id = created!.Data.Id;

            var response = await _client.GetAsync($"/api/habit/{id}");
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] GetHabitById response: {raw}");

            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);

            var result = await SafeReadJsonAsync<ApiResponse<HabitViewModel>>(response);
            result.Should().NotBeNull();
            result!.Data.Title.Should().Be("Run 5km");
        }

        [Fact]
        public async Task UpdateHabit_ShouldReturn200()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/habit", new CreateHabitCommand
            {
                Title = "Meditate",
                Frequency = Habit.HabitFrequency.Daily,
            });

            var created = await SafeReadJsonAsync<ApiResponse<HabitViewModel>>(createResponse);
            created.Should().NotBeNull();
            var id = created!.Data.Id;

            var updateCommand = new UpdateHabitCommand
            {
                Id = id,
                Title = "Meditate deeply",
                Description = "15 minutes daily",
                Frequency = Habit.HabitFrequency.Daily,
            };

            var response = await _client.PutAsJsonAsync($"/api/habit/{id}", updateCommand);
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] UpdateHabit response: {raw}");

            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);
        }

        [Fact]
        public async Task MarkHabitDone_ShouldReturn201()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/habit", new CreateHabitCommand
            {
                Title = "Read book",
                Frequency = Habit.HabitFrequency.Daily,
            });
            var created = await SafeReadJsonAsync<ApiResponse<HabitViewModel>>(createResponse);
            created.Should().NotBeNull();
            var id = created!.Data.Id;

            var response = await _client.PostAsync($"/api/habit/{id}/done", null);
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] MarkHabitDone response: {raw}");

            response.StatusCode.Should().Be(HttpStatusCode.Created, raw);

            var result = await SafeReadJsonAsync<ApiResponse<HabitLogViewModel>>(response);
            result.Should().NotBeNull();
            result!.Data.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteHabit_ShouldReturn200()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/habit", new CreateHabitCommand
            {
                Title = "No sugar",
                Frequency = Habit.HabitFrequency.Daily,
            });

            var created = await SafeReadJsonAsync<ApiResponse<HabitViewModel>>(createResponse);
            created.Should().NotBeNull();
            var id = created!.Data.Id;

            var response = await _client.DeleteAsync($"/api/habit/{id}");
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] DeleteHabit response: {raw}");

            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);

            var check = await _client.GetAsync($"/api/habit/{id}");
            check.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
