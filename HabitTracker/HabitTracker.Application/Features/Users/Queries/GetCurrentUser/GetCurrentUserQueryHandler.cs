using AutoMapper;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Services;
using HabitTracker.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HabitTracker.Application.Features.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserViewModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCurrentUserQueryHandler> _logger;

        public GetCurrentUserQueryHandler(IUserRepository userRepository, IUserContext userContext, IMapper mapper, ILogger<GetCurrentUserQueryHandler> logger)
        {
            _userRepository = userRepository;
            _userContext = userContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserViewModel> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (userId == 0)
            {
                _logger.LogWarning("User not authenticated for creating habit");
                throw new InvalidTokenException("User token is invalid or expired.");
            }


            var user = await _userRepository.GetUserByIdAsync(userId);
            return _mapper.Map<UserViewModel>(user);
        }
    }
}
