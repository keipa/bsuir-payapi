using PayAPI.Models;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace PayAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "PayAPI";
            using (var command = new SqlCommand())
            {
                try
                {
                    command.Connection = new SqlConnection(System.Configuration.ConfigurationManager.
    ConnectionStrings["PayContext"].ConnectionString);
                    command.Connection.Open();
                    ViewBag.Connected = true;
                }
                catch (Exception)
                {
                    ViewBag.Connected = false;
                }
            }

            return View();
        }
    }
}