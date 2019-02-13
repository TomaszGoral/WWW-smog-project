using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aplikacja2017.Models
{
    public class PointPlusUser
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string user { get; set; }
        public string password { get; set; }
    }
    public class _address
    {
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Points { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
    public class _ad
    {
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }

    }
}