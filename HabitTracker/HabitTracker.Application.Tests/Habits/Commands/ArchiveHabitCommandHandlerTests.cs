using Xunit;
using Moq;
using FluentAssertions;
using HabitTracker.Application.Features.Habits.Commands.ArchiveHabit;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Services;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Tests.Habits.Commands
{
    public class ArchiveHabitCommandHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ArchiveHabitCommandHandler>> _loggerMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly ArchiveHabitCommandHandler _handler;

        public ArchiveHabitCommandHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ArchiveHabitCommandHandler>>();
            _userContextMock = new Mock<IUserContext>();

            _habitRepositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
            _userContextMock.Setup(u => u.GetUserId()).Returns(1);

            _handler = new ArchiveHabitCommandHandler(
                _habitRepositoryMock.Object,
                _loggerMock.Object,
                _userContextMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldArchiveHabitSuccessfully()
        {
            // Arrange
            var command = new ArchiveHabitCommand { HabitId = 1 };
            var habit = new Habit { Id = 1, UserId = 1, IsArchived = false };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(1))
                .ReturnsAsync(habit);
            _habitRepositoryMock.Setup(r => r.UpdateHabitAsync(It.IsAny<Habit>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            habit.IsArchived.Should().BeTrue();
            _habitRepositoryMock.Verify(r => r.UpdateHabitAsync(It.IsAny<Habit>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenHabitAlreadyArchived()
        {
            // Arrange
            var command = new ArchiveHabitCommand { HabitId = 1 };
            var habit = new Habit { Id = 1, UserId = 1, IsArchived = true };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(1))
                .ReturnsAsync(habit);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenHabitNotFound()
        {
            // Arrange
            var command = new ArchiveHabitCommand { HabitId = 99 };
            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(99))
                .ReturnsAsync((Habit?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenHabitBelongsToAnotherUser()
        {
            // Arrange
            var command = new ArchiveHabitCommand { HabitId = 2 };
            var habit = new Habit { Id = 2, UserId = 99, IsArchived = false };
            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(2))
                .ReturnsAsync(habit);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
