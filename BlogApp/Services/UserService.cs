using BlogApp.Dotnet.ApplicationCore.DTOs;
using BlogApp.Dotnet.ApplicationCore.Interfaces;
using BlogApp.Dotnet.ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Dotnet.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> CreateAsync(UserDTO user)
        {
            User model = ConvertToModel(user);
            var result = await _userManager.CreateAsync(model, user.Password);
            var id = await _userManager.GetUserIdAsync(model);
            model.UserName = id;
            await _userManager.UpdateAsync(model);

            await _userManager.AddToRoleAsync(model, "User");

            return result;
        }

        public async Task<SignInResult> PasswordSignInAsync(UserDTO user, string password)
        {
            User model = await _userManager.FindByEmailAsync(user.Email);

            return await _signInManager.PasswordSignInAsync(model, password, false, lockoutOnFailure: false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateAsync(UserDTO oldUser, UserDTO newUser, bool passwordChanged = false)
        {
            var contextModel = await _userManager.FindByIdAsync(oldUser.Id);

            contextModel.FirstName = newUser.FirstName;
            contextModel.LastName = newUser.LastName;
            contextModel.Email = newUser.Email;

            if (passwordChanged)
            {
                contextModel.PasswordHash = _userManager.PasswordHasher.HashPassword(contextModel, newUser.Password);
            }

            var result = await _userManager.UpdateAsync(contextModel);

            return result;
        }

        public async Task<PaginatedDTO<UserDTO>> GetAll(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var users = _userManager.Users;

            var sortedUsers = await SortUsers(users, searchString, sortOrder);
            var sortedUserDTOList = sortedUsers.Select(user => new UserDTO(user))
                                               .Skip(((pageNumber ?? 1) - 1) * pageSize)
                                               .Take(pageSize + 1)
                                               .ToList();

            return GetPaginatedDTOs(sortedUserDTOList, pageNumber ?? 1, pageSize);
        }

        public async Task<UserDTO> FindByEmailAsync(string email)
        {
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user != null ? new UserDTO(user) : default;
            }

            return default;
        }

        public async Task<UserDTO> FindByIdAsync(string id)
        {
            if (id == null)
            {
                return default;
            }

            return new UserDTO(await _userManager.FindByIdAsync(id));
        }

        public async Task<IdentityResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);

            return result;
        }

        private User ConvertToModel(UserDTO input)
        {
            User model = new User
            {
                UserName = input.Email,
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName
            };

            return model;
        }

        public async Task<IdentityError> ValidatePassword(string password)
        {
            IdentityError erorr = default;

            foreach (var validator in _userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(_userManager, null, password);

                if (!result.Succeeded)
                {
                    erorr = result.Errors.First();

                    return erorr;
                }
            }

            return erorr;
        }

        private Task<IQueryable<User>> SortUsers(IQueryable<User> users, string searchString, string sortOrder)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString)
                                       || s.Email.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "firstName_desc":
                    users = users.OrderByDescending(u => u.FirstName);
                    break;
                case "firstName":
                    users = users.OrderBy(u => u.FirstName);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
                case "email":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "lastName_desc":
                    users = users.OrderByDescending(u => u.LastName);
                    break;
                case "lastName":
                    users = users.OrderBy(u => u.LastName);
                    break;
                default:
                    users = users.OrderBy(u => u.FirstName);
                    break;
            }

            return Task.FromResult(users);
        }

        private static PaginatedDTO<UserDTO> GetPaginatedDTOs(IEnumerable<UserDTO> userDTOs, int pageNumber, int pageSize)
        {
            bool hasNextPage = false;

            if (userDTOs.Count() > pageSize)
            {
                hasNextPage = true;
                userDTOs = userDTOs.SkipLast(1);
            }

            return new PaginatedDTO<UserDTO>(userDTOs, pageNumber, hasNextPage, pageNumber > 1, pageSize);
        }
    }
}
