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
        public async Task<IActionResult> MyTasks(int page, byte ml, byte nc, byte mm, int dep, byte type, byte mtype)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                //var _meetingLine = adyContext.MeetingLine.Where(x => x.IsRevised==0 && ((x.ResponsibleEmail == User.Identity.Name && x.IsDirected==0) || ((x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive==1)!=null))));

                IQueryable<MeetingLine> _meetingLine;
                if (mtype == 0)
                {
                    _meetingLine = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && ((x.StatusId == 1 && x.IsRevised == 1) || (x.StatusId > 1 && x.StatusId != 8)))) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null);
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
                    _meetingLine = adyContext.MeetingLine.Where(x => x.IdentifierEmail == User.Identity.Name && x.StatusId == 5);
                    ViewBag.HasCheck = (byte)0;
                    ViewBag.SType = 0;
                    ViewBag.MType = (byte)2;
                }
                else if (mtype == 3)
                    _meetingLine = adyContext.MeetingLine.Where(x => x.FollowerEmail == User.Identity.Name && x.StatusId == 6);
                else if (mtype == 4)
                    _meetingLine = adyContext.MeetingLine.Where(x => x.InformedUserEmail.Contains(User.Identity.Name) && x.StatusId > 1 && x.StatusId != 8);
                else if (mtype == 5)
                    _meetingLine = adyContext.MeetingLine.Where(x => x.StatusId == 1 && x.CreatorId == user_id);
                else if (mtype == 6)
                    _meetingLine = adyContext.MeetingLine.Where(x => ((x.StatusId > 1) || x.StatusId == 1 && x.IsRevised == 1) && (x.ResponsibleEmail == User.Identity.Name) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || x.FollowerEmail.Contains(User.Identity.Name) || x.IdentifierEmail.Contains(User.Identity.Name));
                else if (mtype == 7)
                    _meetingLine = adyContext.MeetingLine.Where(x => ((x.StatusId > 1) || x.StatusId == 1 && x.IsRevised == 1) && x.CreatorId == user_id);
                else if (mtype == 8)
                {
                    _meetingLine = adyContext.MeetingLine.Where(x => x.CreatorId==user_id);
                    await _meetingLine.Where(x => x.ReadDate == null && x.ResponsibleEmail == User.Identity.Name).ForEachAsync(x => x.ReadDate = DateTime.UtcNow.AddHours(4));
                    await adyContext.SaveChangesAsync();
                }
                else
                    _meetingLine = Enumerable.Empty<MeetingLine>().AsQueryable();




                if (ml == 1)
                    _meetingLine = _meetingLine.Where(x => x.MeetingId == null);
                else if (ml == 2)
                    _meetingLine = _meetingLine.Where(x => x.MeetingId > 0);

                if (nc == 1)
                    _meetingLine = _meetingLine.Where(x => x.StatusId != 7);
                else if (nc == 2)
                    _meetingLine = _meetingLine.Where(x => x.StatusId == 7);


                if (mm == 1)
                    _meetingLine = _meetingLine.Where(x => x.CreatorId == user_id);
                else if (mm == 2)
                    _meetingLine = _meetingLine.Where(x => x.CreatorId != user_id);

                //if (dep > 0)
                //{
                //    var _department = adyContext.Department.Where(x => x.Id == dep && x.Active == 1);
                //    var mlDepartment = adyContext.MDepartment.Where(x => _department.Select(x => x.Id).Contains(x.DepartmentId) && x.Type == 2);
                //    var id = mlDepartment.Select(x => x.RefId);
                //    _meetingLine = _meetingLine.Where(x => id.Contains(x.Id));

                //    ViewBag.DepName = _department.FirstOrDefault().Name;
                //}


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

        public ActionResult MyTasks2(int page, byte ml, byte nc, byte mm, int dep, byte type, byte mtype)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();

            var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            //var _meetingLine = adyContext.MeetingLine.Where(x => x.IsRevised==0 && ((x.ResponsibleEmail == User.Identity.Name && x.IsDirected==0) || ((x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive==1)!=null))));

            IQueryable<Classes.Meeting_Line> _meetingLine;
            IQueryable<Classes.Meeting_Line> _meeting;
            IQueryable<MeetingLine> MLines;
            IQueryable<Meeting> Meetings;

            if (mtype == 3)
            {
                MLines = adyContext.MeetingLine.Where(x => x.FollowerEmail.Contains(User.Identity.Name) && x.StatusId == 6);
                Meetings = adyContext.Meeting.Where(x => x.FollowerUser.Contains(User.Identity.Name) && x.StatusId == 2);
            }
            else
            {
                MLines = Enumerable.Empty<MeetingLine>().AsQueryable();
                Meetings = Enumerable.Empty<Meeting>().AsQueryable();
            }




            if (ml == 1)
            {
                MLines = MLines.Where(x => x.MeetingId == null);
                //Meetings = Enumerable.Empty<Meeting>().AsQueryable();
            }
            else if (ml == 2)
            {
                MLines = MLines.Where(x => x.MeetingId > 0);
                //Meetings = Enumerable.Empty<Meeting>().AsQueryable();
            }

            if (nc == 1)
                MLines = MLines.Where(x => x.StatusId != 7);
            else if (nc == 2)
                MLines = MLines.Where(x => x.StatusId == 7);

            if (mm == 1)
            {
                MLines = MLines.Where(x => x.CreatorId == user_id);
                Meetings = Meetings.Where(x => x.CreatorId == user_id);
            }
            else if (mm == 2)
            {
                MLines = MLines.Where(x => x.CreatorId != user_id);
                Meetings = Meetings.Where(x => x.CreatorId != user_id);
            }

            //if (dep > 0)
            //{
            //    var _department = adyContext.Department.Where(x => x.Id == dep && x.Active == 1);
            //    var mlDepartment = adyContext.MDepartment.Where(x => _department.Select(x => x.Id).Contains(x.DepartmentId) && x.Type == 2);
            //    var id = mlDepartment.Select(x => x.RefId);
            //    _meetingLine = _meetingLine.Where(x => id.Contains(x.Id));

            //    ViewBag.DepName = _department.FirstOrDefault().Name;
            //}


            _meetingLine = MLines
       .Select(x => new Classes.Meeting_Line { Type = 2, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;

            _meeting = Meetings
                .Select(x => new Classes.Meeting_Line { Type = 1, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;

            if ((_meetingLine != null || _meeting != null) && !((ml >= 1 && ml <= 2) || (nc >= 1 && nc <= 2)))
                _meetingLine = _meetingLine.Union(_meeting);

            else if (!(_meetingLine != null || _meeting != null))
                _meetingLine = Enumerable.Empty<Classes.Meeting_Line>().AsQueryable();


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

            return View(_meetingLine.OrderByDescending(x => x.CreateDate).Skip((page - 1) * 10).Take(10));

        }

        public async Task<IActionResult> AllTasks()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var _meetingLine = adyContext.MeetingLine.Where(x => (x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && ((x.StatusId == 1 && x.IsRevised == 1) || (x.StatusId > 1 && x.StatusId != 8))) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null);

                await _meetingLine.Where(x => x.ReadDate == null && x.ResponsibleEmail == User.Identity.Name).ForEachAsync(x => x.ReadDate = DateTime.UtcNow.AddHours(4));
                await adyContext.SaveChangesAsync();

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = 0;
                ViewBag.MyMeetingLines = 0;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)0;
                ViewBag.Title = "Tapşırıqlarım";

                return View("MyTasks", _meetingLine.OrderByDescending(x => x.Id).Include(x => x.Direct).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
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
                ViewBag.NotCompleted = 0;
                ViewBag.MyMeetingLines = 0;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)1;
                ViewBag.Title = "Təkliflərim";

                return View("MyTasks", _meetingLine.OrderByDescending(x => x.Id).Include(x => x.Direct).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }
        }

        public async Task<IActionResult> MyLines()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var _meetingLine = adyContext.MeetingLine.Where(x => x.CreatorId==user_id);

                await _meetingLine.Where(x => x.ReadDate == null && x.ResponsibleEmail == User.Identity.Name).ForEachAsync(x => x.ReadDate = DateTime.UtcNow.AddHours(4));
                await adyContext.SaveChangesAsync();

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = 0;
                ViewBag.MyMeetingLines = 0;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)8;
                ViewBag.Title = "Daxil edilmiş tapşırıqlarım";

                return View("MyTasks", _meetingLine.OrderByDescending(x => x.Id).Include(x => x.Direct).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }

        }

        public IActionResult Confirmation()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _meetingLine = adyContext.MeetingLine.Where(x => x.IdentifierEmail == User.Identity.Name && x.StatusId == 5);
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = 0;
                ViewBag.MyMeetingLines = 0;
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
            //using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            //{
            //    var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            //    var _meetingLine = adyContext.MeetingLine.Where(x => x.FollowerEmail == User.Identity.Name && x.StatusId == 6);

            //    double page_count = _meetingLine.Count() / 10.0;

            //    if (page_count % 1 > 0)
            //        page_count++;


            //    ViewBag.MaxPage = (int)page_count;
            //    ViewBag.PageNumber = 1;

            //    ViewBag.MeetingLine = 0;
            //    ViewBag.NotCompleted = 0;
            //    ViewBag.MyMeetingLines = 0;
            //    ViewBag.Department = 0;
            //    ViewBag.Type = (byte)0;
            //    ViewBag.MType = (byte)3;
            //    ViewBag.Title = "Təqiblərim";

            //    return View("MyTasks", _meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            //}


            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            var _meetingLine = adyContext.MeetingLine.Where(x => x.FollowerEmail.Contains(User.Identity.Name) && x.StatusId == 6)
                .Select(x => new Classes.Meeting_Line { Type = 2, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;



            var _meeting = adyContext.Meeting.Where(x => x.FollowerUser.Contains(User.Identity.Name) && x.StatusId == 2)
                .Select(x => new Classes.Meeting_Line { Type = 1, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;

            if (_meetingLine != null || _meeting != null)
                _meetingLine = _meetingLine.Union(_meeting);
            else
                _meetingLine = Enumerable.Empty<Classes.Meeting_Line>().AsQueryable();


            double page_count = _meetingLine.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.MaxPage = (int)page_count;
            ViewBag.PageNumber = 1;

            ViewBag.MeetingLine = 0;
            ViewBag.NotCompleted = 0;
            ViewBag.MyMeetingLines = 0;
            ViewBag.Department = 0;
            ViewBag.Type = (byte)0;
            ViewBag.MType = (byte)3;
            ViewBag.Title = "Təqiblərim";

            return View("MyTasks2", _meetingLine.OrderByDescending(x => x.CreateDate).Take(10));


        }

        public IActionResult Informed()
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            var _meetingLine = adyContext.MeetingLine.Where(x => x.InformedUserEmail.Contains(User.Identity.Name) && x.StatusId > 1 && x.StatusId != 8)
                .Select(x => new Classes.Meeting_Line { Type = 2, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;



            var _meeting = adyContext.Meeting.Where(x => x.InformedUser.Contains(User.Identity.Name) && x.StatusId == 2)
                .Select(x => new Classes.Meeting_Line { Type = 1, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;

            if (_meetingLine != null || _meeting != null)
                _meetingLine = _meetingLine.Union(_meeting);
            else
                _meetingLine = Enumerable.Empty<Classes.Meeting_Line>().AsQueryable();


            double page_count = _meetingLine.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.MaxPage = (int)page_count;
            ViewBag.PageNumber = 1;

            ViewBag.MeetingLine = 0;
            ViewBag.NotCompleted = 0;
            ViewBag.MyMeetingLines = 0;
            ViewBag.Department = 0;
            ViewBag.Type = 1;
            ViewBag.MType = (byte)4;
            ViewBag.Title = "Məlumatlandırılmalar";

            return View("MyTasks2", _meetingLine.OrderByDescending(x => x.CreateDate).Take(10));
        }

        public IActionResult InMLines()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var _meetingLine = adyContext.MeetingLine.Where(x => ((x.StatusId > 1) || x.StatusId==1 && x.IsRevised==1) && (x.ResponsibleEmail == User.Identity.Name) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || x.FollowerEmail.Contains(User.Identity.Name) || x.IdentifierEmail.Contains(User.Identity.Name));

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = 0;
                ViewBag.MyMeetingLines = 0;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)6;
                ViewBag.Title = "Gələn iclas sətirləri";

                return View("MyTasks", _meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }
        }

        public IActionResult OutMLines()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var _meetingLine = adyContext.MeetingLine.Where(x => ((x.StatusId > 1) || x.StatusId == 1 && x.IsRevised == 1) && x.CreatorId == user_id);

                double page_count = _meetingLine.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingLine = 0;
                ViewBag.NotCompleted = 0;
                ViewBag.MyMeetingLines = 0;
                ViewBag.Department = 0;
                ViewBag.Type = (byte)0;
                ViewBag.MType = (byte)7;
                ViewBag.Title = "Göndərilən iclas sətirləri";

                return View("MyTasks", _meetingLine.Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }
        }

        public IActionResult AllNotifications()
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var meeting_lines = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.IsPublished == 1) || (x.CreatorId == user_id && x.StatusId > 1)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || (x.FollowerEmail == User.Identity.Name && x.StatusId > 1) || (x.IdentifierEmail == User.Identity.Name && x.StatusId > 1)).Select(x => x.Id);
            var meetings = adyContext.Meeting.Where(x => x.IsPublished == 1 && (x.CreatorId == user_id || x.FollowerUser.Contains(User.Identity.Name) || x.OwnerUser == User.Identity.Name || x.InformedUser.Contains(User.Identity.Name) || x.Participiants.Contains(User.Identity.Name))).Select(x => x.Id);    ////////////////////////////

            var ml_logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 2 /*&& x.OperationId != 6 */ && meeting_lines.Contains(x.RefId));
            var m_logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 1 && meetings.Contains(x.RefId));
            var logs = ml_logs.Union(m_logs);

            double page_count = logs.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.PageNumber = 1;
            ViewBag.MaxPage = (int)page_count;
            ViewBag.UserId = user_id;

            ViewBag.MeetingLine = 0;
            ViewBag.NotCompleted = 0;
            ViewBag.MyMeetingLines = 0;

            return View("Notifications", logs);
        }

        public IActionResult Notifications(int page, byte mtype, byte nc, byte mm)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var meeting_lines = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.IsPublished == 1) || (x.CreatorId == user_id && x.StatusId > 1)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || (x.FollowerEmail == User.Identity.Name && x.StatusId > 1) || (x.IdentifierEmail == User.Identity.Name && x.StatusId > 1));
            var meetings = adyContext.Meeting.Where(x => x.IsPublished == 1 && (x.CreatorId == user_id || x.FollowerUser.Contains(User.Identity.Name) || x.OwnerUser == User.Identity.Name || x.InformedUser.Contains(User.Identity.Name) || x.Participiants.Contains(User.Identity.Name)));    ////////////////////////////

            if (mtype == 1)
            {
                meeting_lines = meeting_lines.Where(x => x.MeetingId == null);
                //Meetings = Enumerable.Empty<Meeting>().AsQueryable();
            }
            else if (mtype == 2)
            {
                meeting_lines = meeting_lines.Where(x => x.MeetingId > 0);
                //Meetings = Enumerable.Empty<Meeting>().AsQueryable();
            }

            if (nc == 1)
                meeting_lines = meeting_lines.Where(x => x.StatusId != 7);
            else if (nc == 2)
                meeting_lines = meeting_lines.Where(x => x.StatusId == 7);

            if (mm == 1)
            {
                meeting_lines = meeting_lines.Where(x => x.CreatorId == user_id);
                meeting_lines = meeting_lines.Where(x => x.CreatorId == user_id);
            }
            else if (mm == 2)
            {
                meeting_lines = meeting_lines.Where(x => x.CreatorId != user_id);
                meeting_lines = meeting_lines.Where(x => x.CreatorId != user_id);
            }

            //if ((_meetingLine != null || _meeting != null) && !((ml >= 1 && ml <= 2) || (nc >= 1 && nc <= 2)))
            //    _meetingLine = _meetingLine.Union(_meeting);

            //else if (!(_meetingLine != null || _meeting != null))
            //    _meetingLine = Enumerable.Empty<Classes.Meeting_Line>().AsQueryable();

            var m_lines = meeting_lines.Select(x => x.Id);
            var mtngs = meetings.Select(x => x.Id);

            var ml_logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 2 /*&& x.OperationId != 6 */ && m_lines.Contains(x.RefId));
            var m_logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 1 && mtngs.Contains(x.RefId));
            IQueryable<MLog> logs = ml_logs;


            if ((m_logs != null || ml_logs != null) && !((mtype >= 1 && mtype <= 2) || (nc >= 1 && nc <= 2)))
                logs = ml_logs.Union(m_logs);


            double page_count = logs.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.PageNumber = page;
            ViewBag.MaxPage = (int)page_count;
            ViewBag.UserId = user_id;

            ViewBag.MeetingLine = mtype;
            ViewBag.NotCompleted = nc;
            ViewBag.MyMeetingLines = mm;

            return View(logs);

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
                ViewBag.NotCompleted = 0;
                ViewBag.MyMeetingLines = 0;
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

                // notifications
                var meeting_line_nlogs = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.IsPublished == 1) || (x.CreatorId == user_id && x.StatusId > 1)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || (x.FollowerEmail == User.Identity.Name && x.StatusId > 1) || (x.IdentifierEmail == User.Identity.Name && x.StatusId > 1)).Select(x => x.Id);
                var meeting_nlogs = adyContext.Meeting.Where(x => x.IsPublished == 1 && (x.CreatorId == user_id || x.FollowerUser.Contains(User.Identity.Name) || x.OwnerUser == User.Identity.Name || x.InformedUser.Contains(User.Identity.Name) || x.Participiants.Contains(User.Identity.Name))).Select(x => x.Id);    ////////////////////////////

                var ml_logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 2 /*&& x.OperationId != 6 */ && meeting_line_nlogs.Contains(x.RefId));
                var m_logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 1 && meeting_nlogs.Contains(x.RefId));
                var notification_mlines = ml_logs.Union(m_logs);
                int notification_count = await notification_mlines.CountAsync();

                //

                int all_tasks_count = await adyContext.MeetingLine.Where(x => (x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && ((x.StatusId == 1 && x.IsRevised == 1) || (x.StatusId > 1 && x.StatusId != 8))) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null).CountAsync();
                int all_offers_count = await adyContext.MeetingLine.Where(x => x.MlType == 2 && (((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.StatusId != 8) || (x.CreatorId == user_id && x.StatusId > 1 && x.StatusId != 8)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null)).CountAsync();
                int cofirmed_count = await adyContext.MeetingLine.Where(x => x.IdentifierEmail == User.Identity.Name && x.StatusId == 5).CountAsync();


                // followed
                var _meetingLine_follow = adyContext.MeetingLine.Where(x => x.FollowerEmail.Contains(User.Identity.Name) && x.StatusId == 6)
                .Select(x => new Classes.Meeting_Line { Type = 2, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;

                var _meeting_follow = adyContext.Meeting.Where(x => x.FollowerUser.Contains(User.Identity.Name) && x.StatusId == 2)
                    .Select(x => new Classes.Meeting_Line { Type = 1, Id = x.Id, CreateDate = x.CreateDate.Value }) as IQueryable<Classes.Meeting_Line>;

                if (_meetingLine_follow != null || _meeting_follow != null)
                    _meetingLine_follow = _meetingLine_follow.Union(_meeting_follow);
                else
                    _meetingLine_follow = Enumerable.Empty<Classes.Meeting_Line>().AsQueryable();

                int followed_count = await _meetingLine_follow.CountAsync();
                // followed


                int informed_count = 0; //await adyContext.MeetingLine.Where(x => x.InformedUserEmail.Contains(User.Identity.Name) && x.IsPublished == 1 && x.StatusId != 8).CountAsync();


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
                    adyContext.MLog.FirstOrDefault(x => x.Id == item).ReadDate = DateTime.UtcNow.AddHours(4);
                    await adyContext.SaveChangesAsync();
                }
            }
        }
    }
}