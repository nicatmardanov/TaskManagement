using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;

namespace adyTask2.Classes
{
    public class FileUpdate
    {
        public async static Task Update(byte type, Microsoft.AspNetCore.Http.IFormFile attached_file, int id, adyTaskManagementContext adyContext)
        {
            string file_path = Guid.NewGuid().ToString() + id + Path.GetExtension(attached_file.FileName);

            var path = Path.Combine(
              Directory.GetCurrentDirectory(), "Content/assets/Attachment",
              file_path);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await attached_file.CopyToAsync(stream);
            }

            var attach = adyContext.Attachment.FirstOrDefault(x => x.RefId == id && x.Type == type);
            attach.Path = "/Content/assets/Attachment/" + file_path;

            await adyContext.SaveChangesAsync();
        }

    }
}
