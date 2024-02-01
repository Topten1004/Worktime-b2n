using AutoMapper;
using Worktime.Models;
using Worktime.ViewModel;

namespace Worktime.Services
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Employee, EmployeeVM>().ReverseMap();
            CreateMap<User, UserVM>().ReverseMap();
        }
    }
}
