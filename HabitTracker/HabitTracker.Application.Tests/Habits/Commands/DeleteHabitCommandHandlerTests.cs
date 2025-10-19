using HabitTracker.Application.Features.Habits.Commands.DeleteHabit;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace HabitTracker.Application.Tests.Habits.Commands
{
    public class DeleteHabitCommandHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DeleteHabitCommandHandler>> _loggerMock;
        private readonly DeleteHabitCommandHandler _handler;

        public DeleteHabitCommandHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteHabitCommandHandler>>();

            _habitRepositoryMock.Setup(r => r.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new DeleteHabitCommandHandler(
                _habitRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldDeleteHabit_WhenHabitExists()
        {
            // Arrange
            var habitId = 10;
            var habit = new Habit { Id = habitId, Title = "Morning Run" };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(habitId))
                .ReturnsAsync(habit);
            _habitRepositoryMock.Setup(r => r.DeleteHabitAsync(habitId))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new DeleteHabitCommand(habitId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _habitRepositoryMock.Verify(r => r.DeleteHabitAsync(habitId), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFound_WhenHabitDoesNotExist()
        {
            // Arrange
            var habitId = 99;
            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(habitId))
                .ReturnsAsync((Habit?)null);

            var command = new DeleteHabitCommand(habitId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"*{habitId}*");

            _habitRepositoryMock.Verify(r => r.DeleteHabitAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRethrow_WhenRepositoryThrows()
        {
            // Arrange
            var habitId = 5;
            var habit = new Habit { Id = habitId, Title = "Read Book" };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(habitId))
                .ReturnsAsync(habit);
            _habitRepositoryMock.Setup(r => r.DeleteHabitAsync(habitId))
                .ThrowsAsync(new Exception("DB error"));

            var command = new DeleteHabitCommand(habitId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
        }
    }
}
