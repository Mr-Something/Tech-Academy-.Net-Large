using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Car_Ins_Quote_WebApp.Models
{
    public class InsuranceQuote
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string DateOfBirth { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public string DUI { get; set; }
        public int SpeedingTickets { get; set; }
        public string FullCoverage { get; set; }
        public string FinalQuote { get; set; }
    }
}