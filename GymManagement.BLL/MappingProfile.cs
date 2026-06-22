using AutoMapper;
using AutoMapper;
using AutoMapper.Execution;
using GymManagement.BLL;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Member = GymManagement.DAL.Data.Models.Member;
namespace GymManagement.BLL
{
    public class MappingProfile :Profile //function [createmap]  ضامن ان عندي    
    {
        public MappingProfile()
        {
            MapMember();
            MapSession();
        }
        private void MapMember()
        {
            CreateMap<Member, MemberViewModel>()//sourse and destination  // create new obj from sourse , destination and assign data and return it
                .ForMember(d => d.Address, o => o.MapFrom(s => $"{s.Address.BuildingNumber} - {s.Address.Street} - {s.Address.City}"))
                .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.DateofBirth.ToShortDateString()));//ركزي بتاخد ايه


            CreateMap<HealthRecord, HealthRecordViewModel>();


            CreateMap<Member, MemberToUpdateViewModel>()
             .ForMember(d => d.BuildingNumber, o => o.MapFrom(s => s.Address.BuildingNumber))
             .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City))
             .ForMember(d => d.Street, o => o.MapFrom(s => s.Address.Street));

            CreateMap<MemberToUpdateViewModel, Member>()
                 .ForMember(dest => dest.Name, opt => opt.Ignore())
                 .ForMember(dest => dest.Photo, opt => opt.Ignore())
                 .AfterMap((src, dest) =>
                 {
                     dest.Address.BuildingNumber = src.BuildingNumber;
                     dest.Address.Street = src.Street;
                     dest.Address.City = src.City;
                 });
            CreateMap<CreateMemberViewModel, Member>()
            .ForMember(dest => dest.Address,
                opt => opt.MapFrom(src => new Address()
                {
                    BuildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }))
            .ForMember(dest => dest.HealthRecord,
                opt => opt.MapFrom(src => src.HealthRecordViewModel));
        }
        private void MapSession()
        {
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Trainer,TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Session, SessionViewModel>().
                ForMember(d => d.TrainerName, opt => opt.MapFrom(s=>s.Trainer.Name))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.CategoryName));

            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
            CreateMap<UpdateSessionViewModel, Session>().ReverseMap(); 

        }
    }
}


