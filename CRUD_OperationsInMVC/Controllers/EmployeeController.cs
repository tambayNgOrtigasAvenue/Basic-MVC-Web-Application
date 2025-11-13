using System.Collections.Generic; // Add this using directive
using CRUD_OperationsInMVC.Models;
using System.Linq;
using System.Web.Mvc;

namespace CRUD_OperationsInMVC.Controllers
{
    [Authorize] // Ensure only logged-in users can access this
    public class EmployeeController : Controller
    {
        private EmployeeDBContext db = new EmployeeDBContext();

        // GET: Employee/Index (Dashboard)
        public ActionResult Index()
        {
            EmployeeDBContext context = new EmployeeDBContext();
            List<Employee> empList = context.Employees.ToList();
            return View(empList);
        }

        // Your other CRUD actions for Employee (Create, Edit, Details, Delete) would go here.
        // GET: Employee/List
        public ActionResult List()
        {
            var employees = db.Employees.ToList();
            return View(employees);
        }
    }
}