using Dookki_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        DOOKKIEntities db = new DOOKKIEntities();
        // GET: Admin/HomeAdmin
        [HttpPost]
        public ActionResult UpdateStatus(bool IsFull)
        {
            // Store the checkbox state in the database or session
            Session["TableStatus"] = IsFull;  // Example with session
                                              // Redirect back to the same view to avoid resubmission on refresh
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Index(int? year)
        {
            // Retrieve the checkbox state from the session
            bool isFull = (Session["TableStatus"] != null) ? (bool)Session["TableStatus"] : false;

            // Pass the value to the view using ViewBag
            ViewBag.IsFull = isFull;

            // caculate year
            // Generate a list of years (e.g., from 2000 to the current year)
            int currentYear = DateTime.Now.Year;
            List<int> years = new List<int>();
            for (int i = currentYear; i >= 2018; i--) // Adjust the range as needed
            {
                years.Add(i);
            }

            // Pass the list to the view
            ViewBag.Years = new SelectList(years);

            // Default selected year
            ViewBag.Year = year ?? currentYear;

            // pie chart
            double[] chartPieData = GetChartPieData(year ?? currentYear);
            string[] chartPieLable = { "Khách không có tài khoản", "Khách có tài khoản" };


            // line chart
            double[] chartLineData = ProfitByYear(year ?? currentYear);
            string[] chartLineLable = { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
                "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
                "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12", };

            // Truyền dữ liệu qua ViewBag hoặc Model
            ViewBag.ChartPieData = chartPieData;
            ViewBag.ChartPieLable = chartPieLable;

            ViewBag.ChartLineData = chartLineData;
            ViewBag.ChartLineLable = chartLineLable;

            return View();
        }

        public ActionResult Statistical(int? year)
        {
            // caculate year
            // Generate a list of years (e.g., from 2000 to the current year)
            int currentYear = DateTime.Now.Year;
            List<int> years = new List<int>();
            for (int i = currentYear; i >= 2018; i--) // Adjust the range as needed
            {
                years.Add(i);
            }

            // Pass the list to the view
            ViewBag.Years = new SelectList(years);

            // Default selected year
            ViewBag.Year = year ?? currentYear;

            // pie chart
            double[] chartPieData = GetChartPieData(year ?? currentYear);
            string[] chartPieLable = { "Khách không có tài khoản", "Khách có tài khoản" };


            // line chart
            double[] chartLineData = ProfitByYear(year ?? currentYear);
            string[] chartLineLable = { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
                "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
                "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12", };

            // Truyền dữ liệu qua ViewBag hoặc Model
            ViewBag.ChartPieData = chartPieData;
            ViewBag.ChartPieLable = chartPieLable;

            ViewBag.ChartLineData = chartLineData;
            ViewBag.ChartLineLable = chartLineLable;

            return View();
        }
        private double[] GetChartPieData(int ? year)
        {
            int totalBill = db.Orders.Count(); 
            int customerHasAccount = 0;        

            
            foreach (var order in db.Orders)
            {
                if (order.OrderDetails.Any() && order.OrderDetails.ElementAt(0).Payment.day.Year == year)
                {
                    if (order.customerID != 0)
                    {
                        customerHasAccount++;
                    }
                }
                
            }

            int customerHasNotAccount = totalBill - customerHasAccount; 

            
            double percentHasNotAccount = Math.Round((double)customerHasNotAccount / totalBill * 100, 2);
            double percentHasAccount = Math.Round((double)customerHasAccount / totalBill * 100, 2);

            
            double[] chartData = new double[] { percentHasNotAccount, percentHasAccount };

            return chartData;
        }
        private double[] ProfitByYear(int ? year)
        {

            // Sample data — replace with real data from the database
            double []monthlyRevenues = new double[12];
            foreach (var order in db.Orders)
            {

                if (order.OrderDetails.Any() && order.OrderDetails.ElementAt(0).Payment.day.Year == year)
                {
                    decimal totalBill = 0;
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        totalBill +=  orderDetail.quantily * orderDetail.Ticket.Price;
                    }
                    // Determine the month (0-based index)
                    int month = order.OrderDetails.ElementAt(0).Payment.day.Month - 1;

                    // Accumulate revenue for the corresponding month
                    monthlyRevenues[month] += (double)totalBill;
                }
            }

            return monthlyRevenues;
        }
    }
}