using AutoMapper;
using FluentAssertions;
using HabitTracker.Application.Features.Habits.Commands.CreateMissedHabitLogs;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MediatR;

namespace HabitTracker.Application.Tests.Habits.Commands
{
    public class CreateMissedHabitLogsCommandHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IHabitLogRepository> _habitLogRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<ILogger<CreateMissedHabitLogsCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateMissedHabitLogsCommandHandler _handler;

        public CreateMissedHabitLogsCommandHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _habitLogRepositoryMock = new Mock<IHabitLogRepository>();
            _mapperMock = new Mock<IMapper>();
            _userContextMock = new Mock<IUserContext>();
            _loggerMock = new Mock<ILogger<CreateMissedHabitLogsCommandHandler>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _habitRepositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

            _handler = new CreateMissedHabitLogsCommandHandler(
                _habitRepositoryMock.Object,
                _habitLogRepositoryMock.Object,
                _mapperMock.Object,
                _userContextMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateMissedLogs_WhenHabitsExist()
        {
            // Arrange
            var today = DateTime.Today;
            var habits = new List<Habit>
            {
                new Habit { Id = 1, Title = "Run" },
                new Habit { Id = 2, Title = "Read" }
            };

            _habitRepositoryMock.Setup(r => r.GetHabitsWithoutLogForDateAsync(today))
                .ReturnsAsync(habits);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new CreateMissedHabitLogsCommand(), CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
            _habitRepositoryMock.Verify(r => r.GetHabitsWithoutLogForDateAsync(today), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturn_WhenNoHabitsExist()
        {
            // Arrange
            var today = DateTime.Today;
            _habitRepositoryMock.Setup(r => r.GetHabitsWithoutLogForDateAsync(today))
                .ReturnsAsync(new List<Habit>());

            // Act
            var result = await _handler.Handle(new CreateMissedHabitLogsCommand(), CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRethrow_WhenRepositoryThrows()
        {
            // Arrange
            var today = DateTime.Today;
            _habitRepositoryMock.Setup(r => r.GetHabitsWithoutLogForDateAsync(today))
                .ThrowsAsync(new Exception("Database crash"));

            // Act
            Func<Task> act = async () => await _handler.Handle(new CreateMissedHabitLogsCommand(), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database crash");
        }
    }
}
