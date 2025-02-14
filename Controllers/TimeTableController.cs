using Dyanamic_Time_Table_Generator.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Dyanamic_Time_Table_Generator.Controllers
{
    public class TimeTableController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int workingDays, int subjectsPerDay, List<TimeTable> subjects)
        {
            int totalHours = workingDays * subjectsPerDay;

            if (subjects.Sum(s => s.TotalHours) != totalHours)
            {
                ViewBag.Error = "Total subject hours must be equal to total working hours.";
                return View();
            }

            try
            {
                Connection.getConnection();
                Connection.con.Open();

                foreach (var subject in subjects)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Subjects (SubjectName, TotalHours) VALUES (@Name, @Hours)", Connection.con);
                    cmd.Parameters.AddWithValue("@Name", subject.SubjectName);
                    cmd.Parameters.AddWithValue("@Hours", subject.TotalHours);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            finally
            {
                Connection.con.Close();
            }

            return RedirectToAction("Results", new { workingDays, subjectsPerDay });
        }

        public ActionResult Results(int workingDays, int subjectsPerDay)
        {
            List<TimeTable> subjects = new List<TimeTable>();

            try
            {
                Connection.getConnection();
                Connection.con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Subjects", Connection.con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    subjects.Add(new TimeTable
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        SubjectName = dr["SubjectName"].ToString(),
                        TotalHours = Convert.ToInt32(dr["TotalHours"])
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            finally
            {
                Connection.con.Close();
            }

            string[,] timetable = new string[subjectsPerDay, workingDays];
            int subjectIndex = 0;
            int subjectCount = subjects.Count;

            for (int i = 0; i < workingDays; i++)
            {
                for (int j = 0; j < subjectsPerDay; j++)
                {
                    while (subjects[subjectIndex].TotalHours == 0 && subjectIndex < subjectCount - 1)
                    {
                        subjectIndex++;
                    }

                    timetable[j, i] = subjects[subjectIndex].SubjectName;
                    subjects[subjectIndex].TotalHours--;

                    if (subjects[subjectIndex].TotalHours == 0 && subjectIndex < subjectCount - 1)
                    {
                        subjectIndex++;
                    }
                }
            }

            List<List<string>> timetableList = new List<List<string>>();
            for (int i = 0; i < subjectsPerDay; i++)
            {
                List<string> row = new List<string>();
                for (int j = 0; j < workingDays; j++)
                {
                    row.Add(timetable[i, j]);
                }
                timetableList.Add(row);
            }

            ViewBag.Timetable = timetableList;
            return View();
        }
    }
}