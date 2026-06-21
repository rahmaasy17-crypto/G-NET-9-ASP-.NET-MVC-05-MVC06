using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController( ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<IActionResult> Index(CancellationToken c)
        {
            var sessions=await _sessionService.GetAllSessionsAsync(c);
            return View(sessions);
        }
        [HttpGet] 
        public async Task<IActionResult> Create() 
        {
            await PopulateDropDownListAsync();

            return View();
           }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken c)
        {
            if (!ModelState.IsValid) {
                await PopulateDropDownListAsync();//عشان تفضل معايا لو رجعت من غير مدخل الداتا عشان فيه خطا معين
                return View(model); }
            var result = await _sessionService.CreateSessionAsync(model, c);
            if (result)
             {   TempData["successMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
                TempData["ErrorMessage"] = "Failed to Create Session";
            await PopulateDropDownListAsync();
            return View(model);
        }

        private async Task PopulateDropDownListAsync() //كدا لما يرجع للصفح اللي هحددها هيبقي معاهدول مش لو باظ ف مكان خلاص كدا
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");//select listعشان هناك المفروض تبقي 
            //ايه القيمه اللي معاه وايه اللي هيظهر فلليست 
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoriesForDropDownAsync(), "Id", "CategoryName");
        }
    }
}
