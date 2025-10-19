using AutoMapper;
using FluentAssertions;
using HabitTracker.Application.Features.Habits.Commands.UpdateHabit;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Application.Tests.Habits.Commands
{
    public class UpdateHabitCommandHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateHabitCommandHandler>> _loggerMock;
        private readonly UpdateHabitCommandHandler _handler;

        public UpdateHabitCommandHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _mapperMock = new Mock<IMapper>();
            _userContextMock = new Mock<IUserContext>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateHabitCommandHandler>>();

            _habitRepositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
            _userContextMock.Setup(u => u.GetUserId()).Returns(1);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _handler = new UpdateHabitCommandHandler(
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _userContextMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldUpdateHabitSuccessfully()
        {
            // ARRANGE
            var command = new UpdateHabitCommand
            {
                Id = 1,
                Title = "Updated Habit",
                Description = "Updated desc",
                Frequency = Habit.HabitFrequency.Weekly,
            };

            var existingHabit = new Habit
            {
                Id = 1,
                Title = "Old Habit",
                Description = "Old desc",
                Frequency = Habit.HabitFrequency.Daily,
                UserId = 1
            };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(1))
                .ReturnsAsync(existingHabit);

            _habitRepositoryMock.Setup(r => r.UpdateHabitAsync(It.IsAny<Habit>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Should().Be(1);
            _habitRepositoryMock.Verify(r => r.UpdateHabitAsync(It.IsAny<Habit>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenHabitNotFound()
        {
            // ARRANGE
            var command = new UpdateHabitCommand
            {
                Id = 99,
                Title = "Nonexistent Habit",
                Frequency = Habit.HabitFrequency.Daily,
            };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(99))
                .ReturnsAsync((Habit?)null);

            // ACT
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Habit not found*");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTitleIsEmpty()
        {
            // ARRANGE
            var command = new UpdateHabitCommand
            {
                Id = 1,
                Title = "",
                Frequency = Habit.HabitFrequency.Daily,
            };

            // ACT
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Title cannot be empty*");
        }

        

        [Fact]
        public async Task Handle_ShouldThrowInvalidTokenException_WhenUserIdIsInvalid()
        {
            // ARRANGE
            _userContextMock.Setup(u => u.GetUserId()).Returns(0);

            var handler = new UpdateHabitCommandHandler(
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _userContextMock.Object,
                _loggerMock.Object
            );

            var command = new UpdateHabitCommand
            {
                Id = 1,
                Title = "Test Habit",
                Frequency = Habit.HabitFrequency.Weekly,
            };

            // ACT
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<InvalidTokenException>()
                .WithMessage("*invalid or expired*");
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccess_WhenHabitNotOwnedByUser()
        {
            // ARRANGE
            var command = new UpdateHabitCommand
            {
                Id = 2,
                Title = "Hack Attempt",
                Frequency = Habit.HabitFrequency.Monthly,
            };

            var habit = new Habit { Id = 2, UserId = 999 };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(2))
                .ReturnsAsync(habit);

            // ACT
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*permission*");
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenRepositoryThrows()
        {
            // ARRANGE
            var command = new UpdateHabitCommand
            {
                Id = 1,
                Title = "Throw me",
                Frequency = Habit.HabitFrequency.Daily,
            };

            var habit = new Habit { Id = 1, UserId = 1 };

            _habitRepositoryMock.Setup(r => r.GetHabitByIdAsync(1))
                .ReturnsAsync(habit);

            _habitRepositoryMock.Setup(r => r.UpdateHabitAsync(It.IsAny<Habit>()))
                .ThrowsAsync(new Exception("Database update error"));

            // ACT
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<Exception>().WithMessage("Database update error");
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
                ),
                Times.AtLeastOnce
            );
        }
    }
}
