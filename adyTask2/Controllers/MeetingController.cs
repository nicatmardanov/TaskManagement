using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class MeetingController : Controller
    {

        private string IpAdress { get; }
        private string AInformation { get; }

        public MeetingController(IHttpContextAccessor accessor)
        {
            IpAdress = !string.IsNullOrEmpty(accessor.HttpContext.Connection.RemoteIpAddress.ToString()) ? accessor.HttpContext.Connection.RemoteIpAddress.ToString() : "";
            AInformation = accessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }

        //get
        public IActionResult AllMeetings()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                IQueryable<Meeting> _meeting = adyContext.Meeting.Where(x => x.CreatorId == user_id || x.FollowerUser.Contains(User.Identity.Name) || x.OwnerUser == User.Identity.Name || x.InformedUser.Contains(User.Identity.Name) || x.Participiants.Contains(User.Identity.Name));    ////////////////////////////

                double page_count = _meeting.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingType = 0;
                ViewBag.MyMeetings = false;
                ViewBag.Department = 0;
                ViewBag.Title = "İclaslarım";

                return View("MeetingList", _meeting.OrderByDescending(x => x.Id).Include(x => x.MeetingTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }

        }

        public IActionResult InMeetings()
        {
            using adyTaskManagementContext adyContext = new adyTaskManagementContext();
            IQueryable<Meeting> _meeting = adyContext.Meeting.Where(x => x.IsPublished==1 && (x.FollowerUser.Contains(User.Identity.Name) || x.OwnerUser == User.Identity.Name || x.InformedUser.Contains(User.Identity.Name) || x.Participiants.Contains(User.Identity.Name)));    ////////////////////////////


            double page_count = _meeting.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.MaxPage = (int)page_count;
            ViewBag.PageNumber = 1;

            ViewBag.MeetingType = 0;
            ViewBag.MyMeetings = false;
            ViewBag.Department = 0;
            ViewBag.Type = 0;
            ViewBag.Title = "Gələn iclaslar";

            return View("MeetingList", _meeting.OrderByDescending(x => x.Id).Include(x => x.MeetingTypeNavigation).Include(x => x.Status).Take(10).ToList());
        }

        public IActionResult OutMeetings()
        {
            using adyTaskManagementContext adyContext = new adyTaskManagementContext();
            var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            IQueryable<Meeting> _meeting = adyContext.Meeting.Where(x => x.IsPublished==1 && x.CreatorId == user_id);    ////////////////////////////

            double page_count = _meeting.Count() / 10.0;

            if (page_count % 1 > 0)
                page_count++;


            ViewBag.MaxPage = (int)page_count;
            ViewBag.PageNumber = 1;

            ViewBag.MeetingType = 0;
            ViewBag.MyMeetings = false;
            ViewBag.Department = 0;
            ViewBag.Type = 1;
            ViewBag.Title = "Göndərilən iclaslar";

            return View("MeetingList", _meeting.OrderByDescending(x => x.Id).Include(x => x.MeetingTypeNavigation).Include(x => x.Status).Take(10).ToList());
        }

        public IActionResult MeetingList(int page, byte mt, bool mm, int dep, byte type)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {

                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                IQueryable<Meeting> _meeting = adyContext.Meeting.AsQueryable();    ////////////////////////////

                if (type == 0)
                    _meeting = adyContext.Meeting.Where(x => x.IsPublished == 1 && (x.FollowerUser.Contains(User.Identity.Name) || x.OwnerUser == User.Identity.Name || x.InformedUser.Contains(User.Identity.Name) || x.Participiants.Contains(User.Identity.Name)));    ////////////////////////////
                else if (type == 1)
                    _meeting = _meeting.Where(x => x.IsPublished==1 && x.CreatorId == user_id);




                if (mt > 0)
                    _meeting = _meeting.Where(x => x.MeetingType == mt);

                if (mm)
                    _meeting = _meeting.Where(x => x.CreatorId == user_id);

                if (dep > 0)
                {
                    var _department = adyContext.Department.Where(x => x.Id == dep && x.Active == 1);
                    var mlDepartment = adyContext.MDepartment.Where(x => _department.Select(x => x.Id).Contains(x.DepartmentId) && x.Type == 1);
                    var id = mlDepartment.Select(x => x.RefId);
                    _meeting = _meeting.Where(x => id.Contains(x.Id));

                    ViewBag.DepName = _department.FirstOrDefault().Name;
                }


                double page_count = _meeting.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = page;

                ViewBag.MeetingType = mt;
                ViewBag.MyMeetings = mm;
                ViewBag.Department = dep;
                ViewBag.Title = "İclaslarım";

                return View(_meeting.OrderByDescending(x => x.Id).Include(x => x.MeetingTypeNavigation).Include(x => x.Status).Skip((page - 1) * 10).Take(10).ToList());
            }
        }

        [Classes.Attributes.Permission(1)]
        public IActionResult Add()
        {
            Classes.Permissions permission = new Classes.Permissions(HttpContext);
            ViewBag.MLPermission = permission.IsPermitted(2) ? 1 : 0;
            return View();
        }

        public IActionResult Drafts()
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                IQueryable<Meeting> _meeting = adyContext.Meeting.Where(x => x.StatusId == 1 && (x.CreatorId == user_id || x.FollowerUser.Contains(User.Identity.Name) || x.OwnerUser == User.Identity.Name || x.InformedUser.Contains(User.Identity.Name) || x.Participiants.Contains(User.Identity.Name)));    ////////////////////////////

                double page_count = _meeting.Count() / 10.0;

                if (page_count % 1 > 0)
                    page_count++;


                ViewBag.MaxPage = (int)page_count;
                ViewBag.PageNumber = 1;

                ViewBag.MeetingType = 0;
                ViewBag.MyMeetings = false;
                ViewBag.Department = 0;
                ViewBag.Title = "Qaralama iclaslar";

                return View("MeetingList", _meeting.OrderByDescending(x => x.Id).Include(x => x.MeetingTypeNavigation).Include(x => x.Status).Take(10).ToList());
            }

        }

        public IActionResult Cancel(int id)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var meeting = adyContext.Meeting.FirstOrDefault(x => x.Id == id && x.CreatorId == user_id && x.StatusId == 1);
            if (meeting != null)
            {
                ViewBag.MId = id;
                return View(meeting);
            }
            return View();
        }

        public async Task<IActionResult> Show(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var meeting = await adyContext.Meeting.Where(x => x.Id == id).Include(x => x.MeetingTypeNavigation).Include(x => x.PlaceNavigation).Include(x => x.Status).FirstOrDefaultAsync();
                var meetingLines = adyContext.MeetingLine.Where(x => x.MeetingId == meeting.Id).Include(x => x.Meeting).Include(x => x.Status).Include(x => x.Revision).Include(x => x.Direct).Include(x => x.MlTypeNavigation).ToList();
                ViewData["MeetingLine"] = meetingLines;
                return View(meeting);
            }
        }

        public async Task<IActionResult> Status(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var meeting = adyContext.Meeting.FirstOrDefault(x => x.Id == id);
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                meeting.StatusId = 2;
                meeting.IsPublished = 1;

                await adyContext.SaveChangesAsync();

                Classes.Log log = new Classes.Log();
                await log.LogAdd(1, "", meeting.Id, 15, user_id, IpAdress, AInformation);


            }

            return RedirectToAction("AllMeetings");
        }

        public async Task<IActionResult> Edit(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _meeting = await adyContext.Meeting.FirstOrDefaultAsync(x => x.Id == id);
                if (_meeting.StatusId == 1)
                {
                    if (await adyContext.Attachment.FirstOrDefaultAsync(x => x.RefId == id && x.Type == 1) != null)
                        ViewData["Attachment"] = adyContext.Attachment.FirstOrDefaultAsync(x => x.RefId == id && x.Type == 1).Result.Path.Split('/')[4];

                    ViewData["Department"] = await adyContext.MDepartment.Where(x => x.Type == 1 && x.RefId == id).Select(x => x.DepartmentId).ToArrayAsync();
                    string tags = string.Join(',', adyContext.Tags.Where(x => x.Type == 1 && x.RefId == id).Select(x => x.Name));
                    ViewData["Name"] = tags;
                    ViewBag.Id = id;
                    Classes.Permissions permission = new Classes.Permissions(HttpContext);
                    ViewBag.MLPermission = permission.IsPermitted(2) ? 1 : 0;

                    return View(adyContext.Meeting.FirstOrDefault(x => x.Id == id));
                }

                return RedirectToAction("AllMeetings");

            }
        }





        //post
        [HttpPost]
        [Classes.Attributes.Permission(1)]
        public async Task<JsonResult> Add(Meeting _meeting, string MeetingType, string PlaceName, string start_date, string start_time, string finish_time, string tags, string OtherParticipiants, int[] meetingDepartments)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var startTime = start_time.Split(':');
                var finishTime = finish_time.Split(':');
                var mDate = start_date.Split('/');
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                DateTime meetingStartDate = new DateTime(int.Parse(mDate[2]), int.Parse(mDate[1]), int.Parse(mDate[0]));


                _meeting.MeetingDate = meetingStartDate.AddHours(double.Parse(startTime[0])).AddMinutes(double.Parse(startTime[1]));
                _meeting.FinishDate = meetingStartDate.AddHours(double.Parse(finishTime[0])).AddMinutes(double.Parse(finishTime[1]));
                _meeting.MeetingType = byte.Parse(MeetingType);

                _meeting.CreatorId = user_id;
                _meeting.CreateDate = DateTime.UtcNow.AddHours(4);
                _meeting.StatusId = 1;
                _meeting.IsActive = 1;
                _meeting.IsPublished = 0;
                _meeting.OtherParticipiants = OtherParticipiants;

                if (!string.IsNullOrEmpty(PlaceName))
                {
                    var _place = new Place { Name = PlaceName };
                    adyContext.Place.Add(_place);
                    await adyContext.SaveChangesAsync();

                    _meeting.Place = _place.Id;
                }

                adyContext.Meeting.Add(_meeting);
                await adyContext.SaveChangesAsync();

                if (Request.Form.Files.Count > 0)
                {
                    var meeting_file = Request.Form.Files[0];
                    await Classes.FileSave.Save(Request.Form.Files[0], _meeting.Id, 1, adyContext);

                }

                MDepartment _meetingDepartment;
                foreach (var item in meetingDepartments)
                {
                    _meetingDepartment = new MDepartment
                    {
                        Type = 1,
                        DepartmentId = item,
                        RefId = _meeting.Id
                    };

                    adyContext.MDepartment.Add(_meetingDepartment);
                    await adyContext.SaveChangesAsync();
                }

                Tags _tag;
                if (!string.IsNullOrEmpty(tags))
                    foreach (var item in tags.Split(','))
                    {
                        _tag = new Tags
                        {
                            Name = item,
                            Type = 1,
                            RefId = _meeting.Id
                        };

                        adyContext.Tags.Add(_tag);
                        await adyContext.SaveChangesAsync();
                    }

                Classes.Log _log = new Classes.Log();

                await _log.LogAdd(1, "", _meeting.Id, 6, user_id, IpAdress, AInformation);

                return Json(new { res = _meeting.Id });
            }
        }

        [HttpPost]
        [Classes.Attributes.Permission(1)]
        public async Task<JsonResult> Update(Meeting _meeting, int meetingId, string PlaceName, string MeetingType, int[] deletedDepartment, int[] addedDepartment, string start_date, string start_time, string finish_time, string[] deletedTags, string[] addedTags)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var startTime = start_time.Split(':');
                var finishTime = finish_time.Split(':');
                var mDate = start_date.Split('/');
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                DateTime meetingStartDate = new DateTime(int.Parse(mDate[2]), int.Parse(mDate[1]), int.Parse(mDate[0]));

                _meeting.MeetingDate = meetingStartDate.AddHours(double.Parse(startTime[0])).AddMinutes(double.Parse(startTime[1]));
                _meeting.FinishDate = meetingStartDate.AddHours(double.Parse(finishTime[0])).AddMinutes(double.Parse(finishTime[1]));
                _meeting.MeetingType = byte.Parse(MeetingType);

                _meeting.CreatorId = user_id;
                _meeting.CreateDate = adyContext.MLog.FirstOrDefaultAsync(x => x.Type == 1 && x.RefId == meetingId).Result.CreateDate;
                _meeting.Id = meetingId;
                _meeting.StatusId = 1;
                _meeting.IsActive = 1;
                _meeting.IsPublished = 0;

                if (!string.IsNullOrEmpty(PlaceName) && adyContext.Place.FirstOrDefault(x => x.Name == PlaceName) == null)
                {
                    var _place = new Place { Name = PlaceName };
                    adyContext.Place.Add(_place);
                    await adyContext.SaveChangesAsync();

                    _meeting.Place = _place.Id;
                }

                adyContext.Meeting.Update(_meeting);
                await adyContext.SaveChangesAsync();

                if (deletedTags.Length > 0 || addedTags.Length > 0)
                {
                    Tags a_tag;
                    var d_tag = adyContext.Tags.Where(x => x.RefId == meetingId && x.Type == 1);

                    if (deletedTags.Length >= addedTags.Length)
                        for (int i = 0; i < deletedTags.Length; i++)
                        {
                            adyContext.Tags.Remove(d_tag.FirstOrDefault(x => x.Name == deletedTags[i]));

                            if (i < addedTags.Length)
                            {
                                a_tag = new Tags
                                {
                                    Name = addedTags[i],
                                    RefId = meetingId,
                                    Type = 1
                                };

                                adyContext.Tags.Add(a_tag);
                            }

                            await adyContext.SaveChangesAsync();
                        }
                    else
                        for (int i = 0; i < addedTags.Length; i++)
                        {
                            a_tag = new Tags
                            {
                                Name = addedTags[i],
                                RefId = meetingId,
                                Type = 1
                            };

                            adyContext.Tags.Add(a_tag);

                            if (i < deletedTags.Length)
                            {
                                adyContext.Tags.Remove(d_tag.FirstOrDefault(x => x.Name == deletedTags[i]));
                            }

                            await adyContext.SaveChangesAsync();
                        }
                }

                if (deletedDepartment.Length > 0 || addedDepartment.Length > 0)
                {
                    MDepartment a_dep;
                    var d_dep = adyContext.MDepartment.Where(x => x.RefId == meetingId && x.Type == 1);

                    if (deletedDepartment.Length >= addedDepartment.Length)
                        for (int i = 0; i < deletedDepartment.Length; i++)
                        {
                            adyContext.MDepartment.Remove(d_dep.FirstOrDefault(x => x.DepartmentId == deletedDepartment[i]));

                            if (i < addedDepartment.Length)
                            {
                                a_dep = new MDepartment
                                {
                                    DepartmentId = addedDepartment[i],
                                    RefId = meetingId,
                                    Type = 1
                                };

                                adyContext.MDepartment.Add(a_dep);
                            }

                            await adyContext.SaveChangesAsync();
                        }
                    else
                        for (int i = 0; i < addedDepartment.Length; i++)
                        {
                            a_dep = new MDepartment
                            {
                                DepartmentId = addedDepartment[i],
                                RefId = meetingId,
                                Type = 1
                            };

                            adyContext.MDepartment.Add(a_dep);

                            if (i < deletedDepartment.Length)
                            {
                                adyContext.MDepartment.Remove(d_dep.FirstOrDefault(x => x.DepartmentId == deletedDepartment[i]));
                            }

                            await adyContext.SaveChangesAsync();
                        }
                }


                var existFile = adyContext.Attachment.FirstOrDefault(x => x.Type == 1 && x.RefId == _meeting.Id);
                if (Request.Form.Files.Count == 0 && existFile != null)
                {
                    System.IO.File.Delete(existFile.Path.Substring(1, existFile.Path.Length - 1));
                    adyContext.Attachment.Remove(existFile);
                    await adyContext.SaveChangesAsync();
                }
                else if (Request.Form.Files.Count > 0)
                {
                    var meeting_file = Request.Form.Files[0];
                    if (existFile != null)
                    {
                        if (!Classes.FileCompare.FCompare(meeting_file, existFile.Path))
                        {
                            System.IO.File.Delete(existFile.Path.Substring(1, existFile.Path.Length - 1));

                            await Classes.FileUpdate.Update(1, meeting_file, _meeting.Id, adyContext);

                        }
                    }
                    else
                    {
                        await Classes.FileSave.Save(meeting_file, _meeting.Id, 1, adyContext);
                    }
                }

                Classes.Log _log = new Classes.Log();
                await _log.LogAdd(1, "", _meeting.Id, 9, user_id, IpAdress, AInformation);



                return Json(new { res = _meeting.Id });
            }

        }

        [HttpPost]
        public async Task Cancel(int MId, string Description)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                adyContext.Meeting.FirstOrDefault(x => x.Id == MId).StatusId = 8;
                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                Classes.Log _log = new Classes.Log();
                await _log.LogAdd(1, Description, MId, 17, user_id, IpAdress, AInformation);
            }
        }


        //partial
        [Classes.Attributes.Permission(1)]
        public PartialViewResult AddMeetingForm() => PartialView();

        public PartialViewResult CancelForm() => PartialView();

    }
}