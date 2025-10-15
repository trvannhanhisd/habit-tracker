using AutoMapper;
using FluentAssertions;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Application.Features.Habits.Queries.GetHabitById;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Application.Tests.Habits.Queries
{
    public class GetHabitByIdQueryHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<GetHabitByIdQueryHandler>> _loggerMock;
        private readonly GetHabitByIdQueryHandler _handler;

        public GetHabitByIdQueryHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<GetHabitByIdQueryHandler>>();
            _handler = new GetHabitByIdQueryHandler(
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnHabit_WhenHabitExists()
        {
            // Arrange
            var habit = new Habit { Id = 1, Title = "Drink Water" };
            var viewModel = new HabitViewModel { Id = 1, Title = "Drink Water" };

            _habitRepositoryMock
                .Setup(x => x.GetHabitByIdAsync(1))
                .ReturnsAsync(habit);
            _mapperMock
                .Setup(x => x.Map<HabitViewModel>(habit))
                .Returns(viewModel);

            var query = new GetHabitByIdQuery { HabitId = 1 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Drink Water");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenHabitNotFound()
        {
            // Arrange
            _habitRepositoryMock
                .Setup(x => x.GetHabitByIdAsync(99))
                .ReturnsAsync((Habit?)null);

            var query = new GetHabitByIdQuery { HabitId = 99 };

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Habit with ID 99 not found.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenHabitIdInvalid()
        {
            // Arrange
            var query = new GetHabitByIdQuery { HabitId = 0 };

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("HabitId must be greater than 0.");
        }
    }
}
