using AutoMapper;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Infrastructure.Repository;
using MediatR;


namespace HabitTracker.Application.Features.Users.Queries.GetUsers
{
    internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserViewModel>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserViewModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUserAsync();

            var userList = _mapper.Map<List<UserViewModel>>(users);

            return userList;
        }
    }
}
