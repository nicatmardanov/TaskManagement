using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace adyTask2.Classes
{
    public class FileCompare
    {
        private static string FHash(string fileName)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private static string FHash(Microsoft.AspNetCore.Http.IFormFile formFile)
        {
            MemoryStream stream = new MemoryStream();
            formFile.OpenReadStream().CopyTo(stream);

            byte[] bytes = System.Security.Cryptography.MD5.Create().ComputeHash(stream.ToArray());
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static bool FCompare(Microsoft.AspNetCore.Http.IFormFile File1, string File2)
        {
            string hash_1 = FHash(File1);
            string hash_2 = FHash(Path.Combine(
                                Directory.GetCurrentDirectory(), File2.Substring(1, File2.Length - 1)));
                              


            if (hash_1 == hash_2)
                return true;

            return false;
        }
    }
}
