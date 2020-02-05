using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;

namespace adyTask2.Classes
{
    public class ValidMeeting_Line
    {
        public bool ValidMeeting(Meeting meeting)
        {
            if (!meeting.MeetingType.HasValue)
                return false;

            if (string.IsNullOrEmpty(meeting.Title))
                return false;

            if (!meeting.Place.HasValue)
                return false;

            if (string.IsNullOrEmpty(meeting.OwnerUser))
                return false;

            if (string.IsNullOrEmpty(meeting.FollowerUser))
                return false;

            if (!meeting.MeetingDate.HasValue)
                return false;

            if (!meeting.FinishDate.HasValue)
                return false;

            if (string.IsNullOrEmpty(meeting.Description))
                return false;

            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                MDepartment md = adyContext.MDepartment.FirstOrDefault(x => x.Type == 1 && x.RefId == meeting.Id);
                if (md == null)
                    return false;
            }

            if (string.IsNullOrEmpty(meeting.Participiants))
                return false;


            return true;
        }
    }
}
