using Moq;
using FluentAssertions;
using HabitTracker.Domain.Repository;
using HabitTracker.Application.Features.Habits.Commands.CreateHabit;
using HabitTracker.Domain.Entity;
using AutoMapper;
using HabitTracker.Domain.Services;
using Microsoft.Extensions.Logging;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Application.Tests.Helpers;
using HabitTracker.Application.Common.ViewModels;

namespace HabitTracker.Application.Tests.Habits.Commands
{
    public class CreateHabitCommandHandlerTests
    {
        private readonly Mock<IHabitRepository> _habitRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateHabitCommandHandler>> _loggerMock;
        private readonly CreateHabitCommandHandler _handler;

        public CreateHabitCommandHandlerTests()
        {
            _habitRepositoryMock = new Mock<IHabitRepository>();
            _mapperMock = new Mock<IMapper>();
            _userContextMock = new Mock<IUserContext>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateHabitCommandHandler>>();

            _habitRepositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

            _userContextMock.Setup(u => u.GetUserId()).Returns(1);

            _handler = new CreateHabitCommandHandler(
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _userContextMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateHabitSuccessfully()
        {
            // ARRANGE
            var command = new CreateHabitCommand
            {
                Title = "Read a book",
                Description = "Read for 30 minutes",
                Frequency = "Daily"
            };

            var fakeHabit = new Habit
            {
                Id = 1,
                Title = command.Title,
                Description = command.Description,
                Frequency = command.Frequency,
                UserId = 1
            };

            _habitRepositoryMock.Setup(r => r.CreateHabitAsync(It.IsAny<Habit>()))
                .ReturnsAsync(fakeHabit);

            _mapperMock.Setup(m => m.Map<HabitViewModel>(It.IsAny<Habit>()))
                .Returns((Habit h) => new HabitViewModel
                {
                    Title = h.Title,
                    Description = h.Description,
                    Frequency = h.Frequency
                });

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Should().NotBeNull();
            result.Title.Should().Be("Read a book");
            _habitRepositoryMock.Verify(r => r.CreateHabitAsync(It.IsAny<Habit>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTitleIsEmpty()
        {
            // ARRANGE
            var command = new CreateHabitCommand { Title = "", Description = "test", Frequency = "Daily" };

            // ACT
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Title*");
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidTokenException_WhenUserIdIsInvalid()
        {
            // ARRANGE
            _userContextMock.Setup(u => u.GetUserId()).Returns(0);

            var handler = new CreateHabitCommandHandler(
                _habitRepositoryMock.Object,
                _mapperMock.Object,
                _userContextMock.Object,
                _loggerMock.Object
            );

            var command = new CreateHabitCommand
            {
                Title = "Workout",
                Description = "Pushups",
                Frequency = "Daily"
            };

            // ACT
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<InvalidTokenException>();
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenRepositoryThrows()
        {
            // ARRANGE
            var command = new CreateHabitCommand
            {
                Title = "Meditate",
                Description = "Morning session",
                Frequency = "Daily"
            };

            _habitRepositoryMock.Setup(r => r.CreateHabitAsync(It.IsAny<Habit>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Database error");

            _loggerMock.VerifyLogError("Database error", Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedViewModelCorrectly()
        {
            // ARRANGE
            var command = new CreateHabitCommand
            {
                Title = "Sleep early",
                Description = "Go to bed before 11PM",
                Frequency = "Daily"
            };

            var fakeHabit = new Habit
            {
                Title = command.Title,
                Description = command.Description,
                Frequency = command.Frequency,
                UserId = 1
            };

            _habitRepositoryMock.Setup(r => r.CreateHabitAsync(It.IsAny<Habit>()))
                .ReturnsAsync(fakeHabit);

            _mapperMock.Setup(m => m.Map<HabitViewModel>(fakeHabit))
                .Returns(new HabitViewModel
                {
                    Title = "Sleep early",
                    Description = "Go to bed before 11PM",
                    Frequency = "Daily"
                });

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Title.Should().Be("Sleep early");
            result.Description.Should().Be("Go to bed before 11PM");
            result.Frequency.Should().Be("Daily");
        }
    }
}
