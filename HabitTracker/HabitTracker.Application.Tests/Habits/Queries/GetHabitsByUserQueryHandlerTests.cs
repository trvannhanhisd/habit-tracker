using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Services;
using Microsoft.Extensions.Logging;
using HabitTracker.Application.Common.ViewModels;

namespace HabitTracker.Application.Tests.Habits.Queries
{
    public class GetHabitsByUserQueryHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<GetHabitsByUserQueryHandler>> _loggerMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly GetHabitsByUserQueryHandler _handler;

        public GetHabitsByUserQueryHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<GetHabitsByUserQueryHandler>>();
            _userContextMock = new Mock<IUserContext>();

            _userContextMock.Setup(u => u.GetUserId()).Returns(1);

            _handler = new GetHabitsByUserQueryHandler(
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _userContextMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnHabits_WhenUserHasHabits()
        {
            // Arrange
            var query = new GetHabitsByUserQuery { UserId = 1 };
            var habits = new List<Habit>
            {
                new Habit { Id = 1, Title = "Drink Water", Description = "8 glasses a day" },
                new Habit { Id = 2, Title = "Workout", Description = "Gym 3 times a week" }
            };

            _habitRepositoryMock.Setup(r => r.GetAllHabitsByUserIdAsync(1))
                .ReturnsAsync(habits);

            _mapperMock.Setup(m => m.Map<List<HabitViewModel>>(habits))
                .Returns(new List<HabitViewModel>
                {
                    new HabitViewModel { Title = "Drink Water" },
                    new HabitViewModel { Title = "Workout" }
                });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Drink Water");
            result[1].Title.Should().Be("Workout");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoHabits()
        {
            // Arrange
            var query = new GetHabitsByUserQuery { UserId = 1 };
            _habitRepositoryMock.Setup(r => r.GetAllHabitsByUserIdAsync(1))
                .ReturnsAsync(new List<Habit>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserIdInvalid()
        {
            // Arrange
            var query = new GetHabitsByUserQuery { UserId = -10 };

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid user ID.");
        }

        [Fact]
        public async Task Handle_ShouldUseUserContext_WhenUserIdIsZero()
        {
            // Arrange
            var query = new GetHabitsByUserQuery { UserId = 0 };
            var habits = new List<Habit>
            {
                new Habit { Id = 1, Title = "Read Book" }
            };

            _habitRepositoryMock.Setup(r => r.GetAllHabitsByUserIdAsync(1))
                .ReturnsAsync(habits);

            _mapperMock.Setup(m => m.Map<List<HabitViewModel>>(habits))
                .Returns(new List<HabitViewModel>
                {
                    new HabitViewModel { Title = "Read Book" }
                });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Read Book");
        }
    }
}
