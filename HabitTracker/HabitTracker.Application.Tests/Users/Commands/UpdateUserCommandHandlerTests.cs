using FluentAssertions;
using HabitTracker.Application.Features.Users.Commands.UpdateUser;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.User;
using HabitTracker.Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Application.Tests.Users.Commands
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UpdateUserCommandHandler>>();
            _handler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateUser_Successfully()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                UserName = "OldName",
                Email = "old@example.com",
                Role = UserRole.User,
                IsActive = true
            };

            var command = new UpdateUserCommand
            {
                Id = 1,
                UserName = "NewName",
                Email = "new@example.com",
                Role = UserRole.Admin,
                IsActive = false
            };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(command.Id))
                .ReturnsAsync(existingUser);

            _userRepositoryMock.Setup(r => r.UpdateUserAsync(existingUser))
                .ReturnsAsync(existingUser.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            existingUser.UserName.Should().Be("NewName");
            existingUser.Email.Should().Be("new@example.com");
            existingUser.Role.Should().Be(UserRole.Admin);
            existingUser.IsActive.Should().BeFalse();

            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                Id = 99,
                UserName = "NonExist",
                Email = "test@example.com",
                Role = UserRole.User
            };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(command.Id))
                .ReturnsAsync((User?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UserNotFoundException>()
                .WithMessage("User with ID 99 not found.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNameIsEmpty()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "Old" };

            var command = new UpdateUserCommand
            {
                Id = 1,
                UserName = "",
                Email = "test@example.com",
                Role = UserRole.User
            };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(command.Id))
                .ReturnsAsync(user);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("UserName cannot be empty.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenEmailIsEmpty()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "Old" };

            var command = new UpdateUserCommand
            {
                Id = 1,
                UserName = "NewUser",
                Email = "",
                Role = UserRole.User
            };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(command.Id))
                .ReturnsAsync(user);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Email cannot be empty.");
        }
    }
}
