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
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        // GET: Home/AddPoint
        public ActionResult AddPoint(Point point)
        {
            string name = "unknown";
            Int32 saveResult;
            List<string> addresslist = new List<string>();

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
            }

            var commune = new
            {
                Name = name,
                //Saved = saveResult
            };

            return Json(addresslist);
        }
        public ActionResult AddAddress(_address name)
        {
            _address response = new _address { Address = "", Lat = 0, Lon = 0 ,Points=0 };
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;" +
                            "User Id=postgres;Password=goralt;Database=postgis;"))
            {
                
                if (name.Address != "")
                {
                    conn.Open();
                    double w = name.Points;
                    string point = "ST_Transform(ST_GeomFromText('POINT(" +
                    name.Lon.ToString(CultureInfo.InvariantCulture) + " " + name.Lat.ToString(CultureInfo.InvariantCulture) + ")',3857),2180)";
                    string areaQuery = "(SELECT geom FROM warszawa WHERE ST_Contains(warszawa.geom," + point + " ))";
                    string addressQuery = "SELECT adres FROM adresy WHERE ST_Contains(" + areaQuery + ",ST_Transform(adresy.geom,2180))";
                    string Query = "(SELECT address FROM points WHERE address='" + name.Address + "' and ST_Contains(" + areaQuery + ",points.geom))";
                    string Querygeom = "(SELECT geom FROM adresy WHERE adres='" + name.Address + "' and ST_Contains(" + areaQuery + ",ST_Transform(adresy.geom,2180)))";
                    var com = new NpgsqlCommand(Query, conn);
                    string firstrecord = "";
                    NpgsqlTypes.PostgisPoint geometry =null;
                    Int32 saveResult;
                    using (NpgsqlDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            firstrecord=dr.GetValue(0).ToString();
                            
                        }
                    }
                    com = new NpgsqlCommand(Querygeom, conn);
                    using (NpgsqlDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            geometry= (NpgsqlTypes.PostgisPoint)dr.GetValue(0);
                        }
                    }
                    if (firstrecord == "")
                    {
                        string punkt = "ST_Transform(ST_GeomFromText('POINT(" +
                           geometry.X.ToString(CultureInfo.InvariantCulture) + " " + geometry.Y.ToString(CultureInfo.InvariantCulture) + ")',2178),2180)";
                        string insertQuery = "insert into points(address,geom,p) values ('" + name.Address + "'," + punkt + "," + w.ToString() + ")";
                        var insertComm = new NpgsqlCommand(insertQuery, conn);
                        response.Address = name.Address;
                        response.Lat = geometry.X;
                        response.Lon = geometry.Y;
                        saveResult = insertComm.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertQuery = "update points set p=p+" + w.ToString() + " where address='" + name.Address + "'";
                        var insertComm = new NpgsqlCommand(insertQuery, conn);
                        saveResult = insertComm.ExecuteNonQuery();
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
                string query = "select address,p from points order by p desc";
                var command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read() && i< 20)
                {
                    _ad p = new _ad() { Address = dr.GetValue(0).ToString(), Lat = dr.GetValue(1).ToString(), Lon = "" };
                    lista.Add(p);

                }

            }
            
            return Json(lista);
        }
    }
}