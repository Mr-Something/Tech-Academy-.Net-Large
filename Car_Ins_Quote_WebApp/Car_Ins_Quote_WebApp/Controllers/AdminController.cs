using Car_Ins_Quote_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Car_Ins_Quote_WebApp.Controllers
{
    public class AdminController : Controller
    {
        //Connection string to connect to the database
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Insurance;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public ActionResult Index()
        {
            
            string queryString = @"SELECT Id, FirstName, LastName, EmailAddress, FinalQuote from Quotes";
            List<InsuranceQuote> insquotes = new List<InsuranceQuote>();
            
            //Similar to connection block in HomeController method GetQuote(). Connects to database and displays information.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                //Finds and sets each parameter for the cshtml page to print to the screen for the admin.
                while (reader.Read())
                {
                    var insquote = new InsuranceQuote();
                    insquote.Id = Convert.ToInt32(reader["Id"]);
                    insquote.FirstName = reader["FirstName"].ToString();
                    insquote.LastName = reader["LastName"].ToString();
                    insquote.EmailAddress = reader["EmailAddress"].ToString();
                    insquote.FinalQuote = reader["FinalQuote"].ToString();

                    insquotes.Add(insquote);
                }
            }
            return View(insquotes);
        }
    }
}