﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core;
using Asp.Core.Data;
using Asp.Core.Domains;
using Asp.Repositories.Messages;
using Asp.Repositories.Roles;
using Asp.Repositories.Users;
using Asp.Web.Common;
using Asp.Web.Common.Models.UserViewModels;
using Asp.Web.Common.Mvc.Alerts;
using AutoMapper;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.Web.Areas.Administration.Controllers
{
    [Area(Constants.Areas.Administration)]
    [Authorize(Policy = Constants.RoleNames.Administrator)]
    public class UsersController : Controller
    {
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserSession _userSession;

        public UsersController(
            IDateTime dateTime,
            IMapper mapper,
            IMessageService messageService,
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IUserSession userSession)
        {
            _dateTime = dateTime;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _messageService = messageService;
            _userRepository = userRepository;
            _userSession = userSession;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            var roleNames = await GetAvailableRoles();
            roleNames.Insert(0, new SelectListItem {Text = "All", Value = ""});
            var model = new UserSearchViewModel {AvailableRoles = roleNames, Status = ""};
            return View("List", model);
        }

        [HttpPost]
        public async Task<IActionResult> List([DataSourceRequest] DataSourceRequest request, UserSearchViewModel model)
        {
            var dataRequest = ParsePagedDataRequest(request, model);
            var entities = await _userRepository.GetUsersAsync(dataRequest);
            var roles = await _roleRepository.GetAllRolesAsync();
            var models = entities.Select(e => _mapper.Map<User, UserViewModel>(e)).ToList();
            // Get authorized role names
            foreach (var m in models)
            {
                if (m.AuthorizedRoleIds.Any())
                {
                    m.AuthorizedRoleNames = string.Join(",",
                        roles.Where(r => m.AuthorizedRoleIds.Contains(r.Id)).Select(r => r.Name).OrderBy(r => r)
                            .ToArray());
                }
            }
            var result = new DataSourceResult {Data = models, Total = entities.TotalCount};
            return Json(result);
        }

        public async Task<IActionResult> Create()
        {
            var model = new UserCreateUpdateViewModel
            {
                AvailableRoles = await GetAvailableRoles(),
                IsActive = true
            };
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkingUser = await _userRepository.GetUserByUserNameAsync(model.UserName);
                if (checkingUser != null)
                {
                    return RedirectToAction("List")
                        .WithError($"User with same username {model.UserName} alredy exists.");
                }

                var dateTimeNow = _dateTime.Now;
                var username = _userSession.UserName;
                var user = _mapper.Map<UserCreateUpdateViewModel, User>(model);
                user.LastLoginDate = dateTimeNow;
                user.CreatedBy = username;
                user.CreatedOn = dateTimeNow;
                user.ModifiedBy = username;
                user.ModifiedOn = dateTimeNow;

                var allRoles = await _roleRepository.GetAllRolesAsync();
                foreach (var role in allRoles)
                {
                    if (model.SelectedRoleIds.Any(r => r == role.Id))
                        user.UserRoles.Add(new UserRole {User = user, Role = role});
                }

                await _userRepository.AddUserAsync(user);
                return RedirectToAction("List").WithSuccess($"{user.FirstName}'s account was created successfully.");
            }

            // If we got this far, something failed, redisplay form
            model.AvailableRoles = await GetAvailableRoles();
            return View("Create", model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            var user = await _userRepository.GetUserByIdAsync(id.Value);
            if (user == null)
                return RedirectToAction("List");

            var model = _mapper.Map<User, UserCreateUpdateViewModel>(user);
            model.SelectedRoleIds = (await _roleRepository.GetUserRolesForUserAsync(user.Id)).Select(r => r.RoleId).ToList();
            model.AvailableRoles = await GetAvailableRoles();
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserCreateUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return RedirectToAction("List").WithError("Please select a user.");
                }

                var dateTimeNow = _dateTime.Now;
                var username = _userSession.UserName;
                user = _mapper.Map(model, user);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.IsActive = model.IsActive;
                user.LastLoginDate = dateTimeNow;
                user.ModifiedOn = dateTimeNow;
                user.ModifiedBy = username;

                var allRoles = await _roleRepository.GetAllRolesAsync();
                var allUserRoles = await _roleRepository.GetUserRolesForUserAsync(model.Id);
                foreach (var role in allRoles)
                {
                    if (model.SelectedRoleIds.Any(r => r == role.Id))
                    {
                        if (allUserRoles.All(r => r.RoleId != role.Id))
                            user.UserRoles.Add(new UserRole {User = user, Role = role});
                    }
                    else if (allUserRoles.Any(r => r.RoleId == role.Id))
                    {
                        var removingUserRole = allUserRoles.FirstOrDefault(r => r.RoleId == role.Id);
                        user.UserRoles.Remove(removingUserRole);
                    }
                }
                await _userRepository.UpdateUserAsync(user);
                return RedirectToAction("List").WithSuccess($"{user.FirstName}'s account was updated successfully.");
            }

            // If we got this far, something failed, redisplay form
            model.AvailableRoles = await GetAvailableRoles();
            return View(model);
        }

        private UserPagedDataRequest ParsePagedDataRequest(DataSourceRequest request, UserSearchViewModel model)
        {
            var dataRequest = new UserPagedDataRequest
            {
                LastName = model.LastName,
                PageIndex = request.Page - 1,
                PageSize = request.PageSize
            };

            switch (model.Status)
            {
                case "1":
                    dataRequest.IsActive = true;
                    break;
                case "0":
                    dataRequest.IsActive = false;
                    break;
            }

            SortDescriptor sort = request.Sorts.FirstOrDefault();
            if (sort != null)
            {
                UserSortField sortField;
                Enum.TryParse(sort.Member, out sortField);
                dataRequest.SortField = sortField;

                dataRequest.SortOrder = sort.SortDirection == ListSortDirection.Ascending
                    ? SortOrder.Ascending
                    : SortOrder.Descending;
            }

            return dataRequest;
        }

        private async Task<IList<SelectListItem>> GetAvailableRoles()
        {
            return (await _roleRepository.GetAllRolesAsync())
                .Select(role => new SelectListItem {Text = role.Name, Value = role.Id.ToString()})
                .ToList();
        }
    }
}