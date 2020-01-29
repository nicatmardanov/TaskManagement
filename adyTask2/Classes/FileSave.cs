using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;

namespace adyTask2.Classes
{
    public class FileSave
    {
        public async static Task Save(Microsoft.AspNetCore.Http.IFormFile attached_file, int id, byte type, adyTaskManagementContext adyContext)
        {
            string file_path = Guid.NewGuid().ToString() + id + Path.GetExtension(attached_file.FileName);

            var path = Path.Combine(
              Directory.GetCurrentDirectory(), "Content/assets/Attachment",
              file_path);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await attached_file.CopyToAsync(stream);
            }

            Attachment attach = new Attachment
            {
                Type = type,
                Path = "/Content/assets/Attachment/" + file_path,
                RefId = id
            };

            adyContext.Attachment.Add(attach);
            await adyContext.SaveChangesAsync();
        }
    }
}
