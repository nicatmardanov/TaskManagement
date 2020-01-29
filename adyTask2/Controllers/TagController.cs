using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Models;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class TagController : Controller
    {
        public async Task MeetingTagAdd(string name, int id)
        {
            using(adyTaskManagementContext adyContext=new adyTaskManagementContext())
            {
                Tags _tag = new Tags
                {
                    Name = name,
                    Type = 1,
                    RefId = id
                };

                adyContext.Add(_tag);
                await adyContext.SaveChangesAsync();
            }
        }

        public async Task MeetingTagRemove(string name, int id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                Tags _tag = adyContext.Tags.FirstOrDefault(x => x.Name == name && x.RefId == id && x.Type == 1);
                adyContext.Remove(_tag);
                await adyContext.SaveChangesAsync();
            }

        }
    }
}