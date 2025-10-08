using AutoMapper;
using HabitTracker.Application.Features.Auth.Commands.Login;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserViewModel?>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public RegisterCommandHandler(IAuthRepository authRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _mapper = mapper;
        }

        public async Task<UserViewModel?> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _authRepository.UserExistsAsync(request.UserName))
                return null;
            if (await _authRepository.EmailExistsAsync(request.Email))
                return null;

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, request.Password)
            };

            var registeredUser = await _authRepository.RegisterUserAsync(user);

            return _mapper.Map<UserViewModel>(registeredUser);
        }
    }
}
