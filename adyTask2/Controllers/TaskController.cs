using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class TaskController : Controller
    {
        //get
        public async Task<IActionResult> MyTasks(int page, byte ml, bool nc, bool mm, int dep, byte type, byte mtype)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                //var _meetingLine = adyContext.MeetingLine.Where(x => x.IsRevised==0 && ((x.ResponsibleEmail == User.Identity.Name && x.IsDirected==0) || ((x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive==1)!=null))));

                IQueryable<MeetingLine> _meetingLine;
                if (mtype == 0)
                {
                    _meetingLine = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && x.StatusId > 1 && x.StatusId != 8)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null);
                    await _meetingLine.Where(x => x.ReadDate == null && x.ResponsibleEmail == User.Identity.Name).ForEachAsync(x => x.ReadDate = DateTime.UtcNow.AddHours(4));
                    await adyContext.SaveChangesAsync();
                }
                else if (mtype == 1)
                {
                    _meetingLine = adyContext.MeetingLine.Where(x => x.MlType == 2 && (((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && x.StatusId > 1 && x.StatusId != 8)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null));
                    await _meetingLine.Where(x => x.ReadDate == null && x.ResponsibleEmail == User.Identity.Name).ForEachAsync(x => x.ReadDate = DateTime.UtcNow.AddHours(4));
                    await adyContext.SaveChangesAsync();
                }
                else if (mtype == 2)
                {
                    _meetingLine = adyContext.MeetingLine.Where(x => x.IdentifierEmail == User.Identity.Name && x.StatusId == 6);
                    ViewBag.HasCheck = (byte)0;
                    ViewBag.SType = 0;
                    ViewBag.MType = (byte)2;
                }
                else if (mtype == 3)
                    _meetingLine = adyContext.MeetingLine.Where(x => x.FollowerEmail == User.Identity.Name && x.StatusId == 5);
                else if (mtype == 4)
                    _meetingLine = adyContext.MeetingLine.Where(x => x.InformedUserEmail.Contains(User.Identity.Name) && x.StatusId > 1 && x.StatusId != 8);
                else if (mtype == 5)
                    _meetingLine = adyContext.MeetingLine.Where(x => x.StatusId == 1 && x.CreatorId == user_id);
                else
                    _meetingLine = Enumerable.Empty<MeetingLine>().AsQueryable();




                if (ml > 0)
                    _meetingLine = _meetingLine.Where(x => x.MlType == ml);

                if (nc)
                    _meetingLine = _meetingLine.Where(x => x.StatusId != 7);

                if (mm)
                    _meetingLine = _meetingLine.Where(x => x.CreatorId == user_id);

                if (dep > 0)
                {
                    var _department = adyContext.Department.Where(x => x.Id == dep && x.Active == 1);
                    var mlDepartment = adyContext.MDepartment.Where(x => _department.Select(x => x.Id).Contains(x.DepartmentId) && x.Type == 2);
                    var id = mlDepartment.Select(x => x.RefId);
                    _meetingLine = _meetingLine.Where(x => id.Contains(x.Id));

                    ViewBag.DepName = _department.FirstOrDefault().Name;
                }


                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = page;

                ViewBag.MeetingLine = ml;
                ViewBag.NotCompleted = nc;
                ViewBag.MyMeetingLines = mm;
                ViewBag.Department = dep;
                ViewBag.Type = type;
                ViewBag.Title = "ADY Task Management";

                return View(_meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Skip((page - 1) * 10).Take(10).ToList());
            }

        }



        public async Task<IActionResult> AllTasks()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var _meetingLine = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && x.StatusId > 1 && x.StatusId != 8)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null);

                await _meetingLine.Where(x => x.ReadDate == null && x.ResponsibleEmail == User.Identity.Name).ForEachAsync(x => x.ReadDate = DateTime.UtcNow.AddHours(4));
                await adyContext.SaveChangesAsync();

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = false;
                ViewBag.MyMeetingLines = false;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)0;
                ViewBag.Title = "Tapşırıqlarım";

                return View("MyTasks", _meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }

        }

        public async Task<IActionResult> AllOffers()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var _meetingLine = adyContext.MeetingLine.Where(x => x.MlType == 2 && (((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && x.StatusId > 1 && x.StatusId != 8)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null));

                await _meetingLine.Where(x => x.ReadDate == null && x.ResponsibleEmail == User.Identity.Name).ForEachAsync(x => x.ReadDate = DateTime.UtcNow.AddHours(4));
                await adyContext.SaveChangesAsync();

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = false;
                ViewBag.MyMeetingLines = false;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)1;
                ViewBag.Title = "Təkliflərim";

                return View("MyTasks", _meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }
        }

        public IActionResult Confirmation()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _meetingLine = adyContext.MeetingLine.Where(x => x.IdentifierEmail == User.Identity.Name && x.StatusId == 6);
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = false;
                ViewBag.MyMeetingLines = false;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.Title = "Təsdiqlərim";
                ViewBag.HasCheck = (byte)0;
                ViewBag.SType = 0;
                ViewBag.MType = (byte)2;

                return View("MyTasks", _meetingLine.OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }
        }

        public IActionResult Followed()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var _meetingLine = adyContext.MeetingLine.Where(x => x.FollowerEmail == User.Identity.Name && x.StatusId == 5);

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = false;
                ViewBag.MyMeetingLines = false;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)3;
                ViewBag.Title = "Təqiblərim";

                return View("MyTasks", _meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }
        }

        public IActionResult Informed()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _meetingLine = adyContext.MeetingLine.Where(x => x.InformedUserEmail.Contains(User.Identity.Name) && x.StatusId > 1 && x.StatusId != 8);
                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = false;
                ViewBag.MyMeetingLines = false;
                ViewBag.Department = 0;
                ViewBag.Type = 1;
                ViewBag.MType = (byte)4;
                ViewBag.Title = "Məlumatlandırılmalar";

                return View("MyTasks", _meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }
        }

        public IActionResult Notifications(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var meeting_lines = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.IsPublished == 1) || (x.CreatorId == user_id && x.StatusId > 1)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || (x.FollowerEmail == User.Identity.Name && x.StatusId > 1) || (x.IdentifierEmail == User.Identity.Name && x.StatusId > 1)).Select(x => x.Id);
                var logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 2 && x.OperationId != 6 && meeting_lines.Contains(x.RefId)).OrderByDescending(x => x.Id);

                double page_count = logs.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.PageNumber = id;
                ViewBag.MaxPage = (int)page_count;
                ViewBag.UserId = user_id;
            }

            return View();
        }


        public IActionResult MeetingLineDraft()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                var _meetingLine = adyContext.MeetingLine.Where(x => x.StatusId == 1 && x.CreatorId == user_id);

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = false;
                ViewBag.MyMeetingLines = false;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.Title = "Qaralama iclas sətirləri";
                ViewBag.SType = 1;
                ViewBag.MType = (byte)5;

                return View("MyTasks", _meetingLine.OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }

        }


        public async Task<string> NotificationsCount()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);


                IQueryable<int> notification_mlines = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.IsPublished == 1) || (x.CreatorId == user_id && x.StatusId > 1)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || (x.FollowerEmail == User.Identity.Name && x.StatusId > 1) || (x.IdentifierEmail == User.Identity.Name && x.StatusId > 1)).Select(x => x.Id);
                int notification_count = await adyContext.MLog.Where(x => x.ReadDate == null && x.OperationId != 6 && x.Type == 2 && notification_mlines.Contains(x.RefId)).CountAsync();

                int all_tasks_count = await adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && x.StatusId > 1 && x.StatusId != 8)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null).CountAsync();
                int followed_count = await adyContext.MeetingLine.Where(x => x.FollowerEmail == User.Identity.Name && x.StatusId == 5).CountAsync();
                int all_offers_count = await adyContext.MeetingLine.Where(x => x.MlType == 2 && (((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && x.StatusId > 1 && x.StatusId != 8)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null)).CountAsync();
                int cofirmed_count = await adyContext.MeetingLine.Where(x => x.IdentifierEmail == User.Identity.Name && x.StatusId == 6).CountAsync();
                int informed_count = await adyContext.MeetingLine.Where(x => x.InformedUserEmail.Contains(User.Identity.Name) && x.IsPublished == 1 && x.StatusId != 8).CountAsync();


                return $"{notification_count}-{all_tasks_count}-{followed_count}-{all_offers_count}-{cofirmed_count}-{informed_count}";


            }

        }



        //post
        [HttpPost]
        public async Task Notifications(int[] ids)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                foreach (var item in ids)
                {
                    adyContext.MLog.FirstOrDefault(x => x.Id == item && x.Type == 2).ReadDate = DateTime.UtcNow.AddHours(4);
                    await adyContext.SaveChangesAsync();
                }
            }
        }
    }
}