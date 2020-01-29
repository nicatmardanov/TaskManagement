using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.AspNetCore.Http;

namespace adyTask2.Classes
{
    public class Log : Microsoft.AspNetCore.Mvc.Controller
    {
        public async Task LogAdd(byte type, string descp, int refId, int operation, int user_id, string IpAdress, string additionalInformation)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                MLog _log = new MLog
                {
                    Type = type,
                    IpAdress = IpAdress,
                    Description = descp,
                    RefId = refId,
                    UserId = user_id,
                    OperationId = operation,
                    CreateDate = DateTime.UtcNow.AddHours(4),
                    AdditionalInformation=additionalInformation
                };

                adyContext.MLog.Add(_log);
                await adyContext.SaveChangesAsync();
            }

        }
    }
}
