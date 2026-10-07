using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class EmployeeDetailsModel : PageModel
    {
        /// <summary>
        /// The employee service used to perform CRUD operations on employees.
        /// </summary>
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// The manager service used to perform CRUD operations on managers.
        /// </summary>
        private readonly ManagerService _managerService;

        /// <summary>
        /// Gets or sets the employee whose details are to be displayed on the page.
        /// </summary>
        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeDetailsModel"/> class with the specified employee service.
        /// </summary>
        /// <param name="employeeService">The employee service to use.</param>
        /// <param name="managerService">The manager service to use.</param>
        public EmployeeDetailsModel(EmployeeService employeeService, ManagerService managerService)
        {
            _employeeService = employeeService;
            _managerService = managerService;
        }

        public IActionResult OnGet(int id)
        {
            Employee? employee = _employeeService.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound();
            }

            if (_managerService.isManager(employee.EmployeeId))
            {
                Manager manager = new Manager(employee.EmployeeId, employee.Name, employee.Email, employee.PhoneNumber);
                Employee = manager;
            }
            else
            {
                Employee = employee;
            }
            
            return Page();
        }

        public IActionResult OnPost()
        {
            _employeeService.DeleteEmployee(Employee.EmployeeId);

            return RedirectToPage("/Employees");
        }
    }
}
