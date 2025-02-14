using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dyanamic_Time_Table_Generator.Models
{
    public class TimeTableInput
    {
        public int WorkingDays {  get; set; }
        public int SubjectsPerDay { get; set; }
    }
}