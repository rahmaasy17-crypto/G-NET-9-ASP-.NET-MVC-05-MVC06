using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken c = default)
        {
            //var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync();//category مش هتنفعني عشان انا عايزاجيب المدربين وال
            var sessions = await _unitOfWork.SessionReposatory.GetALLSessionsWithTrainerAndCategory(c);
            if (sessions.Any() || sessions == null) return null;
            var mapedSessions = sessions.Select(s => new SessionViewModel()
            { 
            Id = s.Id,
            Capacity= s.Capacity,
            CategoryName=s.Category.CategoryName,
            TrainerName=s.Trainer.Name,
            Description= s.Description,
            EndDate= s.EndDate,
            StartDate= s.StartDate,
            
            });
            foreach(var session in mapedSessions) 
            { 
                session.AvailableSlots=await _unitOfWork.SessionReposatory.GetCountOfBookedSlotsAsync(session.Id,c);
            }
            return mapedSessions;
        }


        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken c = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End Date Must Be Greater Than Start Date");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date Must be in the Future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity Must Be Between 1 and 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(model.TrainerId);

            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIDAsync(model.CategoryId);

            if (category is null) return Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Specialties>(category.CategoryName,true,out var categorySpecialty);

            if (!isValid || trainer.Spectatty != categorySpecialty)
                return Result.Validation("Trainer and Category Must be the Same !");

            var session = _mapper.Map<CreateSessionViewModel, Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);

            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0? Result.OK():Result.Fail("Failed to Create Session");
        }

      

        public Task<SessionViewModel?> GetSessionDetailsByIdAsync(int sessionId, CancellationToken c = default)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken c = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveSessionAsync(int id, CancellationToken c = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSessionDetailsAsync(int id, UpdateSessionViewModel model, CancellationToken c = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken c = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(c:c);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken c = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(c: c);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }
    }
}
