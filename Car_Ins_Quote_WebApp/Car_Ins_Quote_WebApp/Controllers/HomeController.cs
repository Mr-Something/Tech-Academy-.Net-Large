using Car_Ins_Quote_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Car_Ins_Quote_WebApp.Controllers
{
    public class HomeController : Controller
    {
        //Connection string to database
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Insurance;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public ActionResult Index()
        {
            return View();
        }

        //Primary method GetQuote(). Takes in http form answers, calculates from answers, posts answers to database, and defines estimate for insurance quote.
        [HttpPost]
        public ActionResult GetQuote(string firstName, string lastName, string emailAddress, string dateOfBirth, string carYear, 
                                     string carMake, string carModel, string dui, string speedingTickets, string fullCoverage, string finalQuote)
        {
            InsuranceQuote newQuote = new InsuranceQuote();
            //if and else if blocks to handle missing or invalid information presented by the user when filling out the form.
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(dateOfBirth) ||
                string.IsNullOrEmpty(carYear) || string.IsNullOrEmpty(carMake) || string.IsNullOrEmpty(carModel) || string.IsNullOrEmpty(speedingTickets))
            {
                return View("~/Views/Shared/InputError.cshtml");
            }
            else if (Convert.ToInt32(carYear) > Convert.ToInt32(DateTime.Now.Year) + 1 || Convert.ToInt32(carYear) < 1908)
            {
                return View("~/Views/Shared/YearError.cshtml");
            }
            //final else block to proceed with primary function of the method.
            else
            {
                //Establishing vars used in if statements for quote calculations.
                decimal insQuote = 50.00m;
                int vehicleYear = Convert.ToInt32(carYear);
                int ticketNumber = Convert.ToInt32(speedingTickets);

                //The following few lines are used to calculate the user's age for quote purposes.
                DateTime now = DateTime.Now;
                DateTime birthDate = Convert.ToDateTime(dateOfBirth);
                int age = now.Year - birthDate.Year;

                if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                        age--;
                int userAge = age;

                //If statements used to calculate quote.
                if (userAge < 25) { insQuote = insQuote + 25.0m; }
                if (userAge < 18) { insQuote = insQuote + 100.0m; }
                if (userAge > 100) { insQuote = insQuote + 25.0m; }
                if (vehicleYear < 2000) { insQuote = insQuote + 25.0m; }
                if (vehicleYear > 2015) { insQuote = insQuote + 25.0m; }
                if (carMake.ToLower() == "porsche") { insQuote = insQuote + 25.0m; }
                if (carMake.ToLower() == "porsche" && carModel.ToLower() == "911 carrera") { insQuote = insQuote + 25.0m; }
                if (ticketNumber > 0) { int ticketPrice = 10 * ticketNumber; insQuote = insQuote + ticketPrice; }
                if (dui == "Yes") { decimal duiPrice = insQuote / 4.0m; insQuote = insQuote + duiPrice; }
                if (fullCoverage == "Yes") { decimal fullCovPrice = insQuote / 2.0m; insQuote = insQuote + fullCovPrice; }
                insQuote = Math.Round(insQuote, 2);
                finalQuote = insQuote.ToString();

                //This is used as a string in the "FinalQuote" page to display what the user's estimated ins cost per month is.
                newQuote.FinalQuote = finalQuote;
                
                //This opens a connection to the database and adds the user's information to it.
                string queryString = @"INSERT INTO Quotes (FirstName, LastName, EmailAddress, DateOfBirth, CarYear, CarMake, CarModel, DUI, SpeedingTickets, FullCoverage, FinalQuote) 
                                        VALUES (@FirstName, @LastName, @EmailAddress, @DateOfBirth, @CarYear, @CarMake, @CarModel, @DUI, @SpeedingTickets, @Fullcoverage, @FinalQuote)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@FirstName", SqlDbType.VarChar);
                    command.Parameters.Add("@LastName", SqlDbType.VarChar);
                    command.Parameters.Add("@EmailAddress", SqlDbType.VarChar);
                    command.Parameters.Add("@DateOfBirth", SqlDbType.Date);
                    command.Parameters.Add("@CarYear", SqlDbType.Int);
                    command.Parameters.Add("@CarMake", SqlDbType.VarChar);
                    command.Parameters.Add("@CarModel", SqlDbType.VarChar);
                    command.Parameters.Add("@SpeedingTickets", SqlDbType.Int);
                    command.Parameters.Add("@DUI", SqlDbType.VarChar);
                    command.Parameters.Add("@FullCoverage", SqlDbType.VarChar);
                    command.Parameters.Add("@FinalQuote", SqlDbType.Money);

                    command.Parameters["@FirstName"].Value = firstName;
                    command.Parameters["@LastName"].Value = lastName;
                    command.Parameters["@EmailAddress"].Value = emailAddress;
                    command.Parameters["@DateOfBirth"].Value = dateOfBirth;
                    command.Parameters["@CarYear"].Value = carYear;
                    command.Parameters["@CarMake"].Value = carMake;
                    command.Parameters["@CarModel"].Value = carModel;
                    command.Parameters["@SpeedingTickets"].Value = speedingTickets;
                    command.Parameters["@DUI"].Value = dui;
                    command.Parameters["@FullCoverage"].Value = fullCoverage;
                    command.Parameters["@FinalQuote"].Value = finalQuote;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                //This is the string that will display in FinalQuote.cshtml.
                ViewData["MyQuote"] = newQuote;

                return View("~/Views/Home/FinalQuote.cshtml");
            }
        }
    }
}