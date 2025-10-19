using AutoMapper;
using FluentAssertions;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Application.Features.HabitLogs.Commands.MarkHabitDone;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.Habit;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Application.Tests.HabitLogs.Commands
{
    public class MarkHabitDoneCommandHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IHabitLogRepository> _habitLogRepositoryMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<MarkHabitDoneCommandHandler>> _loggerMock;
        private readonly MarkHabitDoneCommandHandler _handler;

        public MarkHabitDoneCommandHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _habitLogRepositoryMock = new Mock<IHabitLogRepository>();
            _userContextMock = new Mock<IUserContext>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<MarkHabitDoneCommandHandler>>();

            _habitRepositoryMock.Setup(x => x.UnitOfWork)
                .Returns(Mock.Of<IUnitOfWork>());

            _handler = new MarkHabitDoneCommandHandler(
                _habitLogRepositoryMock.Object,
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _userContextMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldMarkHabitAsDone_Successfully()
        {
            // Arrange
            var userId = 1;
            var date = DateTime.Today;
            var habit = new Habit { Id = 1, UserId = userId };
            habit.MarkAsCompleted(date);

            _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
            _habitRepositoryMock.Setup(r => r.GetHabitByUserIdAsync(userId, 1))
                .ReturnsAsync(habit);
            _habitRepositoryMock.Setup(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var log = habit.Logs.First();
            var expectedVm = new HabitLogViewModel { Date = log.Date, IsCompleted = true };
            _mapperMock.Setup(m => m.Map<HabitLogViewModel>(log)).Returns(expectedVm);

            var command = new MarkHabitDoneCommand { HabitId = 1, Date = date };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsCompleted.Should().BeTrue();
            result.Date.Should().Be(date);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotAuthenticated()
        {
            // Arrange
            _userContextMock.Setup(u => u.GetUserId()).Returns(0);
            var command = new MarkHabitDoneCommand { HabitId = 1, Date = DateTime.Today };

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedHabitAccessException>()
                .WithMessage("User not authenticated.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenHabitNotFound()
        {
            // Arrange
            var userId = 1;
            _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
            _habitRepositoryMock.Setup(r => r.GetHabitByUserIdAsync(userId, 999))
                .ReturnsAsync((Habit?)null);

            var command = new MarkHabitDoneCommand { HabitId = 999, Date = DateTime.Today };

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<HabitNotFoundException>()
                .WithMessage($"Habit with ID 999 not found for user {userId}.");
        }

    }
}
