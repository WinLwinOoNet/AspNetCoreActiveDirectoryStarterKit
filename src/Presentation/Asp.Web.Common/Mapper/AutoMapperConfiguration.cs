using System.Collections.Generic;
using System.Linq;
using Asp.Core.Domains;
using Asp.Web.Common.Models.EmailTemplateViewModels;
using Asp.Web.Common.Models.LogViewModels;
using Asp.Web.Common.Models.SettingViewModels;
using Asp.Web.Common.Models.UserViewModels;

namespace Asp.Web.Common.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // ----- EmailTemplate -----
            CreateMap<EmailTemplate, EmailTemplateViewModel>();
            CreateMap<EmailTemplateViewModel, EmailTemplate>();

            // ----- Log -----
            CreateMap<Log, LogViewModel>();

            // ----- Setting -----
            CreateMap<Setting, SettingViewModel>();
            CreateMap<SettingViewModel, Setting>();

            // ----- User -----
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.AuthorizedRoleIds,
                    mo => mo.MapFrom(src =>
                        src.UserRoles != null ? src.UserRoles.Select(r => r.RoleId).ToList() : new List<int>()));

            CreateMap<User, UserCreateUpdateViewModel>();
            CreateMap<UserCreateUpdateViewModel, User>()
                .ForMember(dest => dest.UserName, mo => mo.MapFrom(src => src.UserName.ToLowerInvariant()))
                .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedOn, opt => opt.Ignore());
        }
    }
}