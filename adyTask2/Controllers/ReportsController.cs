using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Classes.Attributes;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Permission(3)]
    public class ReportsController : Controller
    {
        //get
        public IActionResult Meetings() => View();

        public IActionResult MeetingLines() => View();

        public IActionResult MeetingLineFull(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                ViewBag.Report = adyContext.Reports.FirstOrDefault(x => x.Id == id && x.Type==2).ReportString;
                return View();
            }
        }

        public IActionResult MeetingFull(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                ViewBag.Report = adyContext.Reports.FirstOrDefault(x => x.Id == id && x.Type==1).ReportString;
                return View();
            }
        }





        //post

        [HttpPost]
        public async Task ReportSave(string name, string info)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                Reports _report = new Reports
                {
                    ReportName = name,
                    ReportString = info,
                    UserId = user_id,
                    CreateDate = DateTime.UtcNow.AddHours(4),
                    Type = 2
                };

                await adyContext.AddAsync(_report);
                await adyContext.SaveChangesAsync();

            }
        }

        [HttpPost]
        public async Task ReportMSave(string name, string info)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                Reports _report = new Reports
                {
                    ReportName = name,
                    ReportString = info,
                    UserId = user_id,
                    CreateDate = DateTime.UtcNow.AddHours(4),
                    Type = 1
                };

                await adyContext.AddAsync(_report);
                await adyContext.SaveChangesAsync();

            }
        }


        [HttpPost]
        public PartialViewResult MLReport(int[] meetingType,
                                          int[] mlType,
                                          int[] department,
                                          int[] status,
                                          string[] responsibleEmail,
                                          string[] followerEmail,
                                          string[] confirmedEmail,
                                          int page,
                                          string STime,
                                          string FTime)
        {

            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            bool meetingCheck = meetingType != null && meetingType.Length > 0, mlTypeCheck = mlType != null && mlType.Length > 0,
                depCheck = department != null && department.Length > 0, responsibleCheck = responsibleEmail != null && responsibleEmail.Length > 0,
                followerCheck = followerEmail != null && followerEmail.Length > 0, confirmedCheck = confirmedEmail != null && confirmedEmail.Length > 0,
                statusCheck = status != null && status.Length > 0, STimeCheck = !string.IsNullOrEmpty(STime), FTimeCheck = !string.IsNullOrEmpty(FTime);

            var meetingLine = adyContext.MeetingLine.AsQueryable();

            if (meetingCheck || mlTypeCheck || depCheck || responsibleCheck || followerCheck || confirmedCheck || statusCheck || STimeCheck || FTimeCheck)
            {

                if (meetingCheck && meetingLine != null)
                {
                    meetingLine = meetingLine.Where(x => x.Meeting != null ? meetingType.Contains(x.Meeting.MeetingType.Value) : x == null); //////////////////
                }

                if (mlTypeCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => mlType.Contains(x.MlType.Value));

                if (depCheck && meetingLine != null)
                {
                    var mlDeps = adyContext.MDepartment.Where(x => x.Type == 2 && department.Contains(x.DepartmentId)).Select(x => x.RefId);
                    if (mlDeps != null)
                        meetingLine = meetingLine.Where(x => mlDeps.Contains(x.Id));
                }

                if (responsibleCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => responsibleEmail.Contains(x.ResponsibleEmail));

                if (followerCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => followerEmail.Contains(x.FollowerEmail));

                if (confirmedCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => confirmedEmail.Contains(x.IdentifierEmail));

                //if (cStatusCheck && statusCheck && meetingLine != null)
                //{
                //    if (cStatus == 1)
                //        status = 7;
                //}
                //else if (cStatusCheck && !statusCheck && meetingLine != null)
                //{
                //    if (cStatus == 1)
                //        meetingLine = meetingLine.Where(x => x.StatusId == 7);
                //    else if (cStatus == 2)
                //        meetingLine = meetingLine.Where(x => x.StatusId > 1 && x.StatusId != 7 && x.IsPublished == 1);
                //}

                if (statusCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => status.Contains(x.StatusId.Value) && x.IsPublished == 1);

                if (STimeCheck && meetingLine != null)
                {
                    var start_string = STime.Split('/');
                    var start = new DateTime(int.Parse(start_string[2]), int.Parse(start_string[1]), int.Parse(start_string[0]));
                    meetingLine = meetingLine.Where(x => x.StartTime >= start);
                }

                if (FTimeCheck && meetingLine != null)
                {
                    var finish_string = FTime.Split('/');
                    var finish = new DateTime(int.Parse(finish_string[2]), int.Parse(finish_string[1]), int.Parse(finish_string[0]));
                    meetingLine = meetingLine.Where(x => x.FinishTime <= finish);
                }


            }
            double page_count = meetingLine.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.MaxPage = (int)page_count;
            ViewBag.PageNumber = page;



            return PartialView(meetingLine.OrderByDescending(x => x.Id).Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.MlTypeNavigation).Include(x => x.Status).Skip((page - 1) * 10).Take(10));

        }



        [HttpPost]
        public PartialViewResult MReport(int[] meetingType,
                                         int[] department,
                                         int[] place,
                                         string[] ownerUser,
                                         string[] followerUser,
                                         int page,
                                         string title,
                                         string tags,
                                         string STime,
                                         string FTime)
        {



            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            bool meetingCheck = meetingType != null && meetingType.Length > 0, placeCheck = place != null && place.Length > 0,
                depCheck = department != null && department.Length > 0, ownerCheck = ownerUser != null && ownerUser.Length > 0,
                followerCheck = followerUser != null && followerUser.Length > 0, titleCheck = !string.IsNullOrEmpty(title),
                tagsCheck = !string.IsNullOrEmpty(tags), STimeCheck = !string.IsNullOrEmpty(STime), FTimeCheck = !string.IsNullOrEmpty(FTime);



            var meeting = adyContext.Meeting.AsQueryable();

            if (meetingCheck || placeCheck || depCheck || ownerCheck || followerCheck || titleCheck || tagsCheck || STimeCheck || FTimeCheck)
            {
                if (meetingCheck && meeting != null)
                    meeting = meeting.Where(x => meetingType.Contains(x.MeetingType.Value));

                if (placeCheck && meeting != null)
                    meeting = meeting.Where(x => place.Contains(x.Place.Value));

                if (depCheck && meeting != null)
                {
                    var mDeps = adyContext.MDepartment.Where(x => x.Type == 1 && department.Contains(x.DepartmentId)).Select(x => x.RefId);
                    if (mDeps != null)
                        meeting = meeting.Where(x => mDeps.Contains(x.Id));
                }

                if (ownerCheck && meeting != null)
                    meeting = meeting.Where(x => ownerUser.Contains(x.OwnerUser));

                if (followerCheck && meeting != null)
                    meeting = meeting.Where(x => followerUser.Contains(x.FollowerUser));

                if (titleCheck && meeting != null)
                    meeting = meeting.Where(x => x.Description.Contains(title));

                if (tagsCheck && meeting != null)
                {
                    var tags_array = tags.Split(',');
                    var tags_ref = adyContext.Tags.Where(x => x.Type == 1 && tags.Equals(x.Name)).Select(x => x.RefId);
                    if (tags_ref != null)
                        meeting = meeting.Where(x => tags_ref.Contains(x.Id));
                }

                if (STimeCheck && meeting != null)
                {
                    var start_string = STime.Split('/');
                    var start = new DateTime(int.Parse(start_string[2]), int.Parse(start_string[1]), int.Parse(start_string[0]));
                    meeting = meeting.Where(x => x.MeetingDate >= start);
                }

                if (FTimeCheck && meeting != null)
                {
                    var finish_string = FTime.Split('/');
                    var finish = new DateTime(int.Parse(finish_string[2]), int.Parse(finish_string[1]), int.Parse(finish_string[0]));
                    meeting = meeting.Where(x => x.MeetingDate <= finish);
                }
            }

            double page_count = meeting.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.MaxPage = (int)page_count;
            ViewBag.Page = page;



            return PartialView(meeting.OrderByDescending(x => x.Id).Include(x => x.MeetingTypeNavigation).Include(x => x.Status).Skip((page - 1) * 10).Take(10));

        }




        //json
        public async Task<JsonResult> GetReports(string term)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            if (await adyContext.Reports.CountAsync() > 0)
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var reports = adyContext.Reports.Where(x => x.Type == 2 && x.ReportName.Contains(term) && x.UserId == user_id);
                var reports_info = reports.Select(x => new { id = x.Id, name = x.ReportName, info = x.ReportString });
                return Json(reports_info);
            }

            return Json(string.Empty);
        }

        public async Task<JsonResult> GetMReports(string term)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            if (await adyContext.Reports.CountAsync() > 0)
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var reports = adyContext.Reports.Where(x => x.Type == 1 && x.ReportName.Contains(term) && x.UserId == user_id);
                var reports_info = reports.Select(x => new { id = x.Id, name = x.ReportName, info = x.ReportString });
                return Json(reports_info);
            }

            return Json(string.Empty);
        }



    }
}