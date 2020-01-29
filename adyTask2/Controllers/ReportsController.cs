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
        public IActionResult Meetings()
        {
            return View();
        }

        public IActionResult MeetingLines()
        {
            return View();
        }

        public IActionResult MeetingLineFull(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                ViewBag.Report = adyContext.Reports.FirstOrDefault(x => x.Id == id).ReportString;
                return View();
            }
        }

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
                    CreateDate = DateTime.UtcNow.AddHours(4)
                };

                await adyContext.AddAsync(_report);
                await adyContext.SaveChangesAsync();

            }
        }






        //post
        [HttpPost]
        public PartialViewResult MLReport(int page,
                                          byte meetingType,
                                          byte mlType,
                                          int[] department,
                                          string responsibleEmail,
                                          string followerEmail,
                                          string confirmedEmail,
                                          byte status,
                                          byte cStatus)
        {

            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            bool meetingCheck = meetingType > 0, mlTypeCheck = mlType > 0, depCheck = department != null && department.Length > 0,
                responsibleCheck = !string.IsNullOrEmpty(responsibleEmail), followerCheck = !string.IsNullOrEmpty(followerEmail),
                confirmedCheck = !string.IsNullOrEmpty(confirmedEmail), statusCheck = status > 1, cStatusCheck = cStatus > 0;

            var meetingLine = adyContext.MeetingLine.AsQueryable();

            if (meetingCheck || mlTypeCheck || depCheck || responsibleCheck || followerCheck || confirmedCheck || statusCheck || cStatusCheck)
            {

                if (meetingCheck && meetingLine != null)
                {
                    meetingLine = meetingLine.Where(x => x.Meeting != null ? x.Meeting.MeetingType == meetingType : x == null); //////////////////
                }

                if (mlTypeCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => x.MlType == mlType);

                if (depCheck && meetingLine != null)
                {
                    var mlDeps = adyContext.MDepartment.Where(x => x.Type == 2 && department.Contains(x.DepartmentId)).Select(x => x.RefId);
                    if (mlDeps != null)
                        meetingLine = meetingLine.Where(x => mlDeps.Contains(x.Id));
                }

                if (responsibleCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => x.ResponsibleEmail == responsibleEmail);

                if (followerCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => x.FollowerEmail == followerEmail || x.Direct.FirstOrDefault(y => y.ToUser.EmailAddress == User.Identity.Name && y.IsActive == 1) != null);

                if (confirmedCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => x.IdentifierEmail == confirmedEmail);

                if (cStatusCheck && statusCheck && meetingLine != null)
                {
                    if (cStatus == 1)
                        status = 7;
                }
                else if (cStatusCheck && !statusCheck && meetingLine != null)
                {
                    if (cStatus == 1)
                        meetingLine = meetingLine.Where(x => x.StatusId == 7);
                    else if (cStatus == 2)
                        meetingLine = meetingLine.Where(x => x.StatusId > 1 && x.StatusId != 7 && x.IsPublished == 1);
                }

                if (statusCheck && meetingLine != null)
                    meetingLine = meetingLine.Where(x => x.StatusId == status && x.IsPublished == 1);


            }
            double page_count = meetingLine.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.MaxPage = (int)page_count;
            ViewBag.PageNumber = page;



            return PartialView(meetingLine.OrderByDescending(x => x.Id).Include(x => x.Direct).OrderByDescending(x => x.Id).Include(x => x.MlTypeNavigation).Include(x => x.Status).Skip((page - 1) * 10).Take(10));

        }






        //json
        public async Task<JsonResult> GetReports(string term)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            if (await adyContext.Reports.CountAsync() > 0)
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var reports = adyContext.Reports.Where(x => x.ReportName.Contains(term) && x.UserId == user_id);
                var reports_info = reports.Select(x => new { id = x.Id, name = x.ReportName, info = x.ReportString });
                return Json(reports_info);
            }

            return Json(string.Empty);
        }



    }
}