using AutoMapper;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Helpers
{
    public class RoleResolver : IValueResolver<User, UserProfileViewModel, string>
    {
        private readonly UserManager<User> _userManager;

        public RoleResolver(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public string Resolve(User source, UserProfileViewModel destination, string destMember, ResolutionContext context)
        {
            return _userManager.GetRolesAsync(source).Result[0];
        }
    }
}
