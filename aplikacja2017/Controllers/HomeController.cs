using aplikacja2017.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aplikacja2017.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            User responseuser = new User { user = "", password = "" };
            return View((object)responseuser);
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Account()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Addaccount(string auser, string apassword, string arepassword)
        {
            User responseuser = new User { user = "", password = "" };
            if (apassword != arepassword) return Content("not identical passwords");
            bool usertaken = false;
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {
                conn.Open();
                string loginQuery = "(SELECT \"user\", pass, latestactivity FROM users)";
                var command = new NpgsqlCommand(loginQuery, conn);
                using (NpgsqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        if (dr[0].ToString() == auser)
                        {
                            usertaken = true;
                        }
                    }
                }
                if (usertaken) return Content("Username taken");
                string insertQuery = "INSERT INTO users(\"user\", pass, latestactivity) VALUES('" + auser + "', '" + apassword + "', '1970-01-01 08:00:00')";
                var insertcommand = new NpgsqlCommand(insertQuery, conn);
                Int32 save = insertcommand.ExecuteNonQuery();
                responseuser = new User { user = auser, password = apassword };
                return View("Index", (object)responseuser);

            }
            
            return RedirectToAction("Index",responseuser);
        }
        [HttpPost]
        public ActionResult Index(string suser, string spassword)
        {
            User responseuser = new User { user = "", password = "" };
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {
                conn.Open();
                string loginQuery = "(SELECT \"user\", pass, latestactivity FROM users)";
                var command = new NpgsqlCommand(loginQuery, conn);
                bool isinbase = false;

                using (NpgsqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        
                        if (dr[0].ToString() == suser && dr[1].ToString() == spassword )
                        {
                            isinbase = true;
                            responseuser = new User { user = suser, password = spassword };
                        }
                    }
                }
                
            }
            return View((object)responseuser);
        }
        // GET: Home/AddPoint
        /*public ActionResult AddPoint(PointPlusUser point)
        {
            List<string> addresslist = new List<string>();
            bool logged = false;
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {
                conn.Open();
                string loginQuery = "(SELECT \"user\", pass, latestactivity FROM users)";
                var command = new NpgsqlCommand(loginQuery, conn);
                using (NpgsqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        
                        if (dr[0].ToString() == point.user && dr[1].ToString() == point.password && DateTime.Compare(((DateTime)dr[2]).Date, DateTime.Now.Date) < 0)
                        {
                            logged = true;
                        }
                    }
                }

            }
            if (logged)
            {
                string name = "unknown";
                Int32 saveResult;
                

                using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                                "User Id=postgres;Password=goralt;Database=postgis;"))
                {

                    string punkt = "ST_Transform(ST_GeomFromText('POINT(" +
                        point.Lon.ToString(CultureInfo.InvariantCulture) + " " + point.Lat.ToString(CultureInfo.InvariantCulture) + ")',3857),2180)";

                    conn.Open();

                    string areaQuery = "(SELECT geom FROM warszawa WHERE ST_Contains(warszawa.geom," + punkt + " ))";
                    string addressQuery = "SELECT adres FROM adresy WHERE ST_Contains(" + areaQuery + ",ST_Transform(adresy.geom,2180))";
                    var command = new NpgsqlCommand(addressQuery, conn);
                    int i = 0;

                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            addresslist.Add(dr.GetValue(0).ToString());
                        }
                    }

                    if (name != "undefined")
                    {

                        //int w = 1;
                        //string Query = "(SELECT address FROM points WHERE address='" + name + "')";
                        //var com = new NpgsqlCommand(Query, conn);
                        //string firstrecord="";
                        //using (NpgsqlDataReader dr = com.ExecuteReader())
                        //{
                        //    if(dr.Read())
                        //    {
                        //        while(dr.GetValue(i++)!=null)
                        //        string s;
                        //        addresslist.Add(dr.GetValue(0).ToString());
                        //    }
                        //}
                        //if (firstrecord == "")
                        //{
                        //    string insertQuery = "insert into points(address,geom,p) values (\'" + name + "\'," + punkt + "," + w.ToString() + ")";
                        //    var insertComm = new NpgsqlCommand(insertQuery, conn);
                        //    saveResult = insertComm.ExecuteNonQuery();
                        //}
                        //else
                        //{
                        //    string insertQuery = "update points set p=p+" + w.ToString() + " where address='" + name + "'";
                        //    var insertComm = new NpgsqlCommand(insertQuery, conn);
                        //    saveResult = insertComm.ExecuteNonQuery();
                        //}
                    }
                    

                        string loginUpdateQuery = "UPDATE users SET latestactivity = NOW() WHERE \"user\" ='" + point.user + "' AND pass='" + point.password + "'";
                        var upcommand = new NpgsqlCommand(loginUpdateQuery, conn);
                        Int32 save = upcommand.ExecuteNonQuery();
                    
                }

                var commune = new
                {
                    Name = name,
                    //Saved = saveResult
                };
            }
            return Json(addresslist);
        }*/
        public ActionResult AddAddress(_address name)
        {
            bool logged = false;
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {
                conn.Open();
                string loginQuery = "(SELECT \"user\", pass, latestactivity FROM users)";
                var command = new NpgsqlCommand(loginQuery, conn);
                using (NpgsqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        if (dr[0].ToString() == name.User && dr[1].ToString() == name.Password && DateTime.Compare(((DateTime)dr[2]).Date, DateTime.Now.Date) < 0)
                        {
                            logged = true;
                        }
                    }
                }

            }

            _address response = new _address { Address = "", Lat = 0, Lon = 0 ,Points=0, User="", Password="" };
            if (logged)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                                "User Id=postgres;Password=goralt;Database=postgis;"))
                {

                    if (name.Address != "")
                    {
                        conn.Open();
                        double w = name.Points;
                        
                        /*string areaQuery = "(SELECT geom FROM warszawa WHERE ST_Contains(warszawa.geom," + point + " ))";
                        string addressQuery = "SELECT adres FROM adresy WHERE ST_Contains(" + areaQuery + ",ST_Transform(adresy.geom,2180))";
                        string Query = "(SELECT address FROM points WHERE address='" + name.Address + "' and ST_Contains(" + areaQuery + ",points.geom))";
                        string Querygeom = "(SELECT geom FROM adresy WHERE adres='" + name.Address + "' and ST_Contains(" + areaQuery + ",ST_Transform(adresy.geom,2180)))";*/
                        string Query = "SELECT address from points where address ='" + name.Address +"'";
                        var com = new NpgsqlCommand(Query, conn);
                        string firstrecord = "";
                        NpgsqlTypes.PostgisPoint geometry = null;
                        Int32 saveResult;
                        using (NpgsqlDataReader dr = com.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                firstrecord = dr.GetValue(0).ToString();

                            }
                        }
                        /*com = new NpgsqlCommand(Querygeom, conn);
                        using (NpgsqlDataReader dr = com.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                geometry = (NpgsqlTypes.PostgisPoint)dr.GetValue(0);
                            }
                        }*/
                        if (firstrecord == "")
                        {
                            /*string punkt = "ST_Transform(ST_GeomFromText('POINT(" +
                               geometry.X.ToString(CultureInfo.InvariantCulture) + " " + geometry.Y.ToString(CultureInfo.InvariantCulture) + ")',2178),2180)";*/
                            string point = "ST_Transform(ST_GeomFromText('POINT(" +
                            name.Lon.ToString(CultureInfo.InvariantCulture) + " " + name.Lat.ToString(CultureInfo.InvariantCulture) + ")',3857),2180)";
                            string insertQuery = "insert into points(address,geom,p,count) values ('" + name.Address + "'," + point + "," + w.ToString() + ",1)";
                            var insertComm = new NpgsqlCommand(insertQuery, conn);
                            response.Address = name.Address;
                            response.Lat = name.Lat;
                            response.Lon = name.Lon;
                            saveResult = insertComm.ExecuteNonQuery();
                        }
                        else
                        {
                            string insertQuery = "update points set p=p+" + w.ToString() + ", count=count+1 where address='" + name.Address + "'";
                            var insertComm = new NpgsqlCommand(insertQuery, conn);
                            saveResult = insertComm.ExecuteNonQuery();
                            response.Address = "Update";
                        }
                        string loginUpdateQuery = "UPDATE users SET latestactivity = NOW() WHERE \"user\" ='" + name.User + "' AND pass='" + name.Password + "'";
                        var upcommand = new NpgsqlCommand(loginUpdateQuery, conn);
                        Int32 save = upcommand.ExecuteNonQuery();
                    }
                }
                
            }
            return Json(response); 
        }
        public ActionResult GetAllPoints()
        {
            List<_address> points = new List<_address>();
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {

                conn.Open();
                string query = "select st_x(ST_Transform(geom,3857)), st_y(ST_Transform(geom,3857)), address from points";
                var command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    float xVal = 0, yVal = 0;
                    bool s = true;
                    s = s && float.TryParse(dr.GetValue(0).ToString(), out xVal);
                    s = s && float.TryParse(dr.GetValue(1).ToString(), out yVal);
                    string address = dr.GetValue(2).ToString();
                    if (s)
                    {
                        points.Add(new _address() { Address=address, Lon = xVal, Lat = yVal });
                    }
                }
            }

            return Json(points);
        }
        public ActionResult Getp(_address adres)
        {
            string p = "";
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {

                conn.Open();
                string query = "select p from points where address='" + adres.Address +"'";
                var command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    p= dr.GetValue(0).ToString();

                }

            }
            _address ad = new _address() { Address = p, Lat = 0, Lon = 0 };
            return Json(ad);
        }
        public ActionResult Getlist()
        {
            List<_ad> lista = new List<_ad>();
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {
                int i = 0;
                conn.Open();
                string query = "select address,p,count from points order by p desc";
                var command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read() && i< 20)
                {
                    _ad p = new _ad() { Address = dr.GetValue(0).ToString(), Lat = dr.GetValue(1).ToString(), Lon = dr.GetValue(2).ToString() };
                    lista.Add(p);

                }

            }
            
            return Json(lista);
        }
    }
}