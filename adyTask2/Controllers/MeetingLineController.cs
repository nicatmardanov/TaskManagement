using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class MeetingLineController : Controller
    {
        private string IpAdress { get; }
        private string AInformation { get; }

        public MeetingLineController(IHttpContextAccessor accessor)
        {
            IpAdress = !string.IsNullOrEmpty(accessor.HttpContext.Connection.RemoteIpAddress.ToString()) ? accessor.HttpContext.Connection.RemoteIpAddress.ToString() : "";
            AInformation = accessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }

        //get
        [Classes.Attributes.Permission(2)]
        public IActionResult Add()
        {
            Classes.Permissions permission = new Classes.Permissions(HttpContext);
            ViewBag.MLPermission = permission.IsPermitted(2) ? 1 : 0;
            return View();
        }

        public IActionResult Redirect(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _user = adyContext.User.FirstOrDefault(x => x.EmailAddress == User.Identity.Name);
                var _meetingLine = adyContext.MeetingLine.Where(x => x.Id == id && ((x.ResponsibleEmail == _user.EmailAddress && x.IsDirected == 0) || (x.Direct.FirstOrDefault(y => y.ToUserId == _user.Id) != null && x.IsDirected == 1)));

                ViewBag.Id = id;
                return View(_meetingLine.Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).ToList());
            }
        }
        public async Task<IActionResult> Status(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _meetingLine = await adyContext.MeetingLine.FirstOrDefaultAsync(x => x.Id == id);
                var _user = User.Identity.Name;
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                if ((_meetingLine.StatusId == 1 && _meetingLine.CreatorId == user_id) || (_meetingLine.StatusId == 3 && _meetingLine.ResponsibleEmail == _user) || (_meetingLine.StatusId == 5 && _meetingLine.IdentifierEmail == _user) || (_meetingLine.StatusId == 6 && _meetingLine.FollowerEmail == _user))
                {
                    ViewBag.Id = id;
                    //return View(await adyContext.MeetingLine.FirstOrDefaultAsync(x => x.Id == id)/*.Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).ToList()*/);
                    return View(await adyContext.MeetingLine.Where(x => x.Id == id).Include(x => x.MlTypeNavigation).FirstOrDefaultAsync());
                }
                else
                {
                    return Redirect("/Task/AllTasks");
                }


            }
        }
        public IActionResult Cancel(int id)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var mLine = adyContext.MeetingLine.Where(x => x.Id == id && x.CreatorId == user_id && x.StatusId == 1);
            if (mLine != null)
            {
                ViewBag.MLId = id;
                return View(mLine);
            }
            return View();

        }
        public async Task Remove(int id)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            var ML = await adyContext.MeetingLine.FirstOrDefaultAsync(x => x.Id == id);
            adyContext.MeetingLine.Remove(ML);
            await adyContext.SaveChangesAsync();
        }

        public async Task<IActionResult> Show(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                return View(await adyContext.MeetingLine.Where(x => x.Id == id).Include(x => x.MlTypeNavigation).FirstOrDefaultAsync());
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _meetingLine = await adyContext.MeetingLine.FirstOrDefaultAsync(x => x.Id == id);
                if (_meetingLine.StatusId == 1)
                {
                    if (await adyContext.Attachment.FirstOrDefaultAsync(x => x.RefId == id && x.Type == 2) != null)
                        ViewData["Attachment"] = adyContext.Attachment.FirstOrDefaultAsync(x => x.RefId == id && x.Type == 2).Result.Path.Split('/')[4];
                    ViewData["Department"] = await adyContext.MDepartment.Where(x => x.Type == 2 && x.RefId == id).Select(x => x.DepartmentId).ToArrayAsync();
                    string tags = string.Join(',', adyContext.Tags.Where(x => x.Type == 2 && x.RefId == id).Select(x => x.Name));
                    ViewData["Name"] = tags;
                    ViewBag.Id = id;

                    return View(adyContext.MeetingLine.FirstOrDefault(x => x.Id == id));
                }

                return RedirectToAction("AllTasks", "Task");

            }
        }


        //public IActionResult Revision(int id)
        //{
        //    return View(id);
        //}


        //Post
        [Classes.Attributes.Permission(2)]
        [HttpPost]
        public async Task<int> AddMeetingLine(MeetingLine _meetingline, string StartTime, string FinishTime, string Tags, int[] Departments)
        {
            Tags _tags; MDepartment _mDepartment;

            string[] sTime = StartTime.Split('/');
            string[] fTime = !string.IsNullOrEmpty(FinishTime) ? FinishTime.Split('/') : new string[0];
            int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            _meetingline.StartTime = new DateTime(int.Parse(sTime[2]), int.Parse(sTime[1]), int.Parse(sTime[0]));
            if (!string.IsNullOrEmpty(FinishTime))
                _meetingline.FinishTime = new DateTime(int.Parse(fTime[2]), int.Parse(fTime[1]), int.Parse(fTime[0]));
            _meetingline.CreateDate = DateTime.UtcNow.AddHours(4);
            _meetingline.CreatorId = user_id;
            _meetingline.StatusId = 1;
            _meetingline.IsDirected = 0;
            _meetingline.IsRevised = 0;
            _meetingline.IsPublished = 0;

            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                adyContext.MeetingLine.Add(_meetingline);
                await adyContext.SaveChangesAsync();

                if (!string.IsNullOrEmpty(Tags))
                    foreach (var item in Tags.Split(','))
                    {
                        _tags = new Tags
                        {
                            Name = item,
                            RefId = _meetingline.Id,
                            Type = 2
                        };

                        adyContext.Tags.Add(_tags);
                        await adyContext.SaveChangesAsync();
                    }
                if (Departments != null)
                    foreach (var item in Departments)
                    {
                        _mDepartment = new MDepartment
                        {
                            Type = 2,
                            DepartmentId = item,
                            RefId = _meetingline.Id
                        };

                        adyContext.MDepartment.Add(_mDepartment);
                        await adyContext.SaveChangesAsync();
                    }

                if (Request.Form.Files.Count > 0)
                {
                    var ml_file = Request.Form.Files[0];
                    await Classes.FileSave.Save(Request.Form.Files[0], _meetingline.Id, 2, adyContext);
                }
            }



            Classes.Log _log = new Classes.Log();
            await _log.LogAdd(2, "", _meetingline.Id, 6, user_id, IpAdress, AInformation);


            return _meetingline.Id;
        }


        [HttpPost]
        public async Task Status([FromBody] MLog data)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                Classes.Log _log = new Classes.Log();

                if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 1)
                {
                    var meetingLine = adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId);
                    Classes.ValidMeeting_Line _valid = new Classes.ValidMeeting_Line();

                    if (_valid.ValidMLine(meetingLine))
                    {
                        meetingLine.StatusId = 3;
                        meetingLine.IsPublished = 1;

                        if (meetingLine.IsRevised == 1)
                            await _log.LogAdd(2, "", data.RefId, 19, user_id, IpAdress, AInformation);
                        else
                            await _log.LogAdd(2, "", data.RefId, 15, user_id, IpAdress, AInformation);

                        meetingLine.IsRevised = 0;
                    }
                    else
                    {
                        Response.StatusCode = 406;
                        await Response.WriteAsync("İclas sətiri təsdiqlənmək üçün uyğun formada deyil!");
                    }

                }

                else if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 3)
                {
                    adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId = 5;
                    if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).IsRevised == 1)
                        await _log.LogAdd(2, data.Description, data.RefId, 19, user_id, IpAdress, AInformation);
                    else
                        await _log.LogAdd(2, data.Description, data.RefId, 11, user_id, IpAdress, AInformation);

                    if (await adyContext.Revision.CountAsync() > 0)
                    {
                        if (await adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.MlId == data.RefId) != null)
                        {
                            adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefault(x => x.MlId == data.RefId).Active = 0;
                            adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).IsRevised = 0;

                        }
                    }
                }
                else if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 5)
                {
                    adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId = 6;

                    if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).IsRevised == 1)
                        await _log.LogAdd(2, data.Description, data.RefId, 19, user_id, IpAdress, AInformation);
                    else
                        await _log.LogAdd(2, data.Description, data.RefId, 12, user_id, IpAdress, AInformation);

                    if (await adyContext.Revision.CountAsync() > 0)
                        if (await adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.MlId == data.RefId) != null)
                        {
                            adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefault(x => x.MlId == data.RefId).Active = 0;
                            adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).IsRevised = 0;
                        }
                }
                else if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 6)
                {
                    adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId = 7;
                    if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).IsRevised == 1)
                        await _log.LogAdd(2, data.Description, data.RefId, 19, user_id, IpAdress, AInformation);
                    else
                        await _log.LogAdd(2, data.Description, data.RefId, 13, user_id, IpAdress, AInformation);

                    if (await adyContext.Revision.CountAsync() > 0)
                        if (await adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.MlId == data.RefId) != null)
                        {
                            adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefault(x => x.MlId == data.RefId).Active = 0;
                            adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).IsRevised = 0;
                        }
                }
                await adyContext.SaveChangesAsync();
            }
        }

        [HttpPost]
        public async Task StatusMulti(int[] ids)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                Classes.Log _log;
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                MeetingLine meetingLine;

                for (int i = 0; i < ids.Length; i++)
                {
                    _log = new Classes.Log();
                    meetingLine = adyContext.MeetingLine.FirstOrDefault(x => x.Id == ids[i]);

                    if (meetingLine.StatusId == 1)
                    {
                        meetingLine.StatusId = 3;
                        meetingLine.IsPublished = 1;
                        if (meetingLine.IsRevised == 1)
                            await _log.LogAdd(2, "", ids[i], 19, user_id, IpAdress, AInformation);
                        else
                            await _log.LogAdd(2, "", ids[i], 15, user_id, IpAdress, AInformation);


                        meetingLine.IsRevised = 0;

                    }
                    else if (meetingLine.StatusId == 6)
                    {
                        meetingLine.StatusId = 7;
                        await _log.LogAdd(2, meetingLine.Description, ids[i], 13, user_id, IpAdress, AInformation);
                        if (await adyContext.Revision.CountAsync() > 0)
                            if (await adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.MlId == ids[i]) != null)
                            {
                                adyContext.Revision.OrderByDescending(x => x.Id).FirstOrDefault(x => x.MlId == ids[i]).Active = 0;
                                if (meetingLine.IsRevised == 1)
                                    await _log.LogAdd(2, meetingLine.Description, ids[i], 19, user_id, IpAdress, AInformation);
                                else
                                    await _log.LogAdd(2, meetingLine.Description, ids[i], 13, user_id, IpAdress, AInformation);


                                adyContext.MeetingLine.FirstOrDefault(x => x.Id == ids[i]).IsRevised = 0;

                            }
                    }
                    await adyContext.SaveChangesAsync();
                }
            }
        }

        [HttpPost]
        public async Task Revision([FromBody] MLog data)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                Revision _rev = new Revision
                {
                    Description = data.Description,
                    CreateDate = DateTime.UtcNow.AddHours(4),
                    UserId = user_id,
                    MlId = data.RefId,
                    Active = 1
                };
                if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 3)
                    adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId = 1;
                else if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 5)
                    adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId = 3;
                else if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 6)
                    adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId = 5;
                else if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId == 7)
                    adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).StatusId = 6;

                adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.RefId).IsRevised = 1;

                adyContext.Revision.Add(_rev);
                await adyContext.SaveChangesAsync();

                Classes.Log _log = new Classes.Log();
                await _log.LogAdd(2, data.Description, data.RefId, 10, user_id, IpAdress, AInformation);
            }
        }

        [HttpPost]
        public async Task RevisionMulti(int[] ids)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                MeetingLine meetingLine;
                for (int i = 0; i < ids.Length; i++)
                {
                    meetingLine = adyContext.MeetingLine.FirstOrDefault(x => x.Id == ids[i]);
                    if (meetingLine.StatusId == 5)
                        meetingLine.StatusId = 3;
                    else if (meetingLine.StatusId == 6)
                        meetingLine.StatusId = 5;
                    else if (meetingLine.StatusId == 7)
                        meetingLine.StatusId = 6;

                    meetingLine.IsRevised = 1;

                    await adyContext.SaveChangesAsync();

                    Classes.Log _log = new Classes.Log();
                    await _log.LogAdd(2, meetingLine.Description, ids[i], 10, user_id, IpAdress, AInformation);
                }


            }
        }

        [HttpPost]
        public async Task Redirect(Direct data, string FinishDate, string ToUserEmail)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var oldDirects = adyContext.Direct.Where(x => x.MlId == data.MlId);
                await oldDirects.ForEachAsync(x => x.IsActive = 0);

                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);


                data.CreateDate = DateTime.UtcNow.AddHours(4);
                data.FromUserId = user_id;
                data.ToUserId = adyContext.User.FirstOrDefault(x => x.EmailAddress == ToUserEmail).PersonId;
                data.IsActive = 1;

                var fTime = FinishDate.Split('/');
                data.FinishDate = new DateTime(int.Parse(fTime[2]), int.Parse(fTime[1]), int.Parse(fTime[0]));

                var mLine = adyContext.MeetingLine.FirstOrDefault(x => x.Id == data.MlId);
                mLine.IsDirected = 1;


                adyContext.Direct.Add(data);
                await adyContext.SaveChangesAsync();

                Classes.Log _log = new Classes.Log();
                string log_description = data.Description;
                int mlId = data.MlId ?? default;

                await _log.LogAdd(2, log_description, mlId, 4, user_id, IpAdress, AInformation);
            }
        }

        [HttpPost]
        public async Task Cancel(int MlId, string Description)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                adyContext.MeetingLine.FirstOrDefault(x => x.Id == MlId).StatusId = 8;
                await adyContext.SaveChangesAsync();
                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                Classes.Log _log = new Classes.Log();
                await _log.LogAdd(2, Description, MlId, 17, user_id, IpAdress, AInformation);
            }
        }

        [HttpPost]
        public async Task<int> Edit(MeetingLine mLine, bool FileEmpty, bool FileNotChanged, string STime, string FTime, int[] addedDepartment, int[] deletedDepartment, string[] addedTags, string[] deletedTags, byte isEdit)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                MeetingLine meetingLine = adyContext.MeetingLine.FirstOrDefault(x => x.Id == mLine.Id);

                meetingLine.MlType = mLine.MlType;
                meetingLine.Description = mLine.Description;
                meetingLine.ResponsibleEmail = mLine.ResponsibleEmail;
                meetingLine.FollowerEmail = mLine.FollowerEmail;
                meetingLine.IdentifierEmail = mLine.IdentifierEmail;
                meetingLine.InformedUserEmail = mLine.InformedUserEmail;

                var sTime = STime.Split('/');
                var fTime = !string.IsNullOrEmpty(FTime) ? FTime.Split('/') : new string[0];

                meetingLine.StartTime = new DateTime(int.Parse(sTime[2]), int.Parse(sTime[1]), int.Parse(sTime[0]));

                if (FTime != null && FTime.Length > 0)
                    meetingLine.FinishTime = new DateTime(int.Parse(fTime[2]), int.Parse(fTime[1]), int.Parse(fTime[0]));

                //if (await TryUpdateModelAsync(meetingLine))
                //{
                //    meetingLine = mLine;
                //}

                var errors = ModelState.Values.SelectMany(x => x.Errors);

                await adyContext.SaveChangesAsync();

                if (addedDepartment.Length > 0 || deletedDepartment.Length > 0)
                {
                    var departments = adyContext.MDepartment.Where(x => x.Type == 2 && x.RefId == mLine.Id);
                    if (deletedDepartment.Length >= addedDepartment.Length)
                    {
                        for (int i = 0; i < deletedDepartment.Length; i++)
                        {
                            adyContext.MDepartment.Remove(departments.FirstOrDefault(x => x.DepartmentId == deletedDepartment[i]));

                            if (i < addedDepartment.Length)
                            {
                                MDepartment mDepartment = new MDepartment
                                {
                                    Type = 2,
                                    RefId = mLine.Id,
                                    DepartmentId = addedDepartment[i]
                                };

                                adyContext.MDepartment.Add(mDepartment);
                            }

                            await adyContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < addedDepartment.Length; i++)
                        {

                            if (i < deletedDepartment.Length)
                            {
                                adyContext.MDepartment.Remove(departments.FirstOrDefault(x => x.DepartmentId == deletedDepartment[i]));
                            }

                            MDepartment mDepartment = new MDepartment
                            {
                                Type = 2,
                                RefId = mLine.Id,
                                DepartmentId = addedDepartment[i]
                            };
                            adyContext.MDepartment.Add(mDepartment);

                            await adyContext.SaveChangesAsync();
                        }
                    }
                }

                if (deletedTags.Length > 0 || addedTags.Length > 0)
                {
                    Tags a_tag;
                    var d_tag = adyContext.Tags.Where(x => x.RefId == mLine.Id && x.Type == 2);

                    if (deletedTags.Length >= addedTags.Length)
                        for (int i = 0; i < deletedTags.Length; i++)
                        {
                            adyContext.Tags.Remove(d_tag.FirstOrDefault(x => x.Name == deletedTags[i]));

                            if (i < addedTags.Length)
                            {
                                a_tag = new Tags
                                {
                                    Name = addedTags[i],
                                    RefId = mLine.Id,
                                    Type = 2
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
                                RefId = mLine.Id,
                                Type = 2
                            };

                            adyContext.Tags.Add(a_tag);

                            if (i < deletedTags.Length)
                            {
                                adyContext.Tags.Remove(d_tag.FirstOrDefault(x => x.Name == deletedTags[i]));
                            }

                            await adyContext.SaveChangesAsync();
                        }
                }


                Attachment _attach = adyContext.Attachment.FirstOrDefault(x => x.RefId == mLine.Id && x.Type == 2);

                if (FileEmpty && _attach != null)
                {
                    System.IO.File.Delete(_attach.Path.Substring(1, _attach.Path.Length - 1));
                    adyContext.Attachment.Remove(_attach);
                    await adyContext.SaveChangesAsync();
                }
                else if (!FileEmpty && !FileNotChanged && Request.Form.Files.Count > 0)
                {
                    var meeting_file = Request.Form.Files[0];

                    if (_attach != null && !Classes.FileCompare.FCompare(meeting_file, _attach.Path))
                    {
                        System.IO.File.Delete(_attach.Path.Substring(1, _attach.Path.Length - 1));
                        await Classes.FileUpdate.Update(2, meeting_file, mLine.Id, adyContext);
                    }
                    else
                    {
                        await Classes.FileSave.Save(meeting_file, mLine.Id, 2, adyContext);
                    }
                }



                Classes.Log _log = new Classes.Log();
                string log_description = mLine.Description;
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                if (isEdit == 1)
                    await _log.LogAdd(2, "", mLine.Id, 9, user_id, IpAdress, AInformation);

                return mLine.Id;

            }
        }




        //Partial
        [Classes.Attributes.Permission(2)]
        public PartialViewResult MeetingLineTable() => PartialView();

        [Classes.Attributes.Permission(2)]
        public PartialViewResult MeetingLineAddForm() => PartialView();

        public PartialViewResult MeetingDirectForm() => PartialView();

        public PartialViewResult MeetingStatusForm() => PartialView();

        public PartialViewResult RevisionForm() => PartialView();

        public PartialViewResult CancelForm() => PartialView();


        //Partial Post

        [HttpPost]
        public PartialViewResult MeetingLineEditForm(MeetingLine meetingLine, string Department, string InformedUser, string StartTime, string FinishTime, string Name, string Path)
        {
            ViewData["Attachment"] = Path;
            ViewData["Department"] = Department.Split(';').Select(x => int.Parse(x)).ToArray();

            if (!string.IsNullOrEmpty(InformedUser))
                ViewData["InformedUser"] = InformedUser.Split(';');
            ViewData["STime"] = StartTime;
            ViewData["FTime"] = FinishTime;
            ViewData["Name"] = Name;

            return PartialView(meetingLine);
        }
    }
}