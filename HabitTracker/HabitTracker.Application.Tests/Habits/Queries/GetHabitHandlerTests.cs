using AutoMapper;
using FluentAssertions;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HabitTracker.Application.Tests.Habits.Queries
{
    public class GetHabitHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<GetHabitHandler>> _loggerMock;
        private readonly GetHabitHandler _handler;

        public GetHabitHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<GetHabitHandler>>();

            _handler = new GetHabitHandler(
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnAllHabits_WhenHabitsExist()
        {
            // Arrange
            var habits = new List<Habit>
            {
                new Habit { Id = 1, Title = "Workout" },
                new Habit { Id = 2, Title = "Read Books" }
            };

            var vmList = new List<HabitViewModel>
            {
                new HabitViewModel { Id = 1, Title = "Workout" },
                new HabitViewModel { Id = 2, Title = "Read Books" }
            };

            _habitRepositoryMock.Setup(x => x.GetAllHabitAsync())
                .ReturnsAsync(habits);

            _mapperMock.Setup(x => x.Map<List<HabitViewModel>>(habits))
                .Returns(vmList);

            // Act
            var result = await _handler.Handle(new GetHabitQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Workout");
            result[1].Title.Should().Be("Read Books");

            _habitRepositoryMock.Verify(x => x.GetAllHabitAsync(), Times.Once);
            _mapperMock.Verify(x => x.Map<List<HabitViewModel>>(habits), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoHabitsExist()
        {
            // Arrange
            var emptyHabits = new List<Habit>();
            _habitRepositoryMock.Setup(x => x.GetAllHabitAsync())
                .ReturnsAsync(emptyHabits);

            _mapperMock.Setup(x => x.Map<List<HabitViewModel>>(It.IsAny<List<Habit>>()))
                .Returns(new List<HabitViewModel>());

            // Act
            var result = await _handler.Handle(new GetHabitQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _habitRepositoryMock.Verify(x => x.GetAllHabitAsync(), Times.Once);
            _mapperMock.Verify(x => x.Map<List<HabitViewModel>>(It.IsAny<List<Habit>>()), Times.Never);
            // we expect map NOT called because handler returns early for empty
        }

        [Fact]
        public async Task Handle_ShouldLogAndRethrow_WhenRepositoryThrows()
        {
            // Arrange
            _habitRepositoryMock.Setup(x => x.GetAllHabitAsync())
                .ThrowsAsync(new Exception("DB fail"));

            // Act
            Func<Task> act = async () => await _handler.Handle(new GetHabitQuery(), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("DB fail");

            // verify that logger.LogError was invoked at least once
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }
    }
}
