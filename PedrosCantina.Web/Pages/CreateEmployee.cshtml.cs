using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class CreateEmployeeModel : PageModel
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
        /// Gets the list of roles available for selection when creating a new employee.
        /// </summary>
        public readonly SelectList Roles = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Value = "Employee", Text = "Medarbejder" },
            new SelectListItem { Value = "Manager", Text = "Leder" }
        }, "Value", "Text");
        
        /// <summary>
        /// The new employee to be created.
        /// </summary>
        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        /// <summary>
        /// The role of the new employee to be created (Employee or Manager).
        /// </summary>
        [BindProperty]
        public string EmployeeRole { get; set; } = "Employee";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEmployeeModel"/> class with the specified employee service and manager service.
        /// </summary>
        /// <param name="employeeService">The employee service to use.</param>
        /// <param name="managerService">The manager service to use.</param>
        public CreateEmployeeModel(EmployeeService employeeService, ManagerService managerService)
        {
            _employeeService = employeeService;
            _managerService = managerService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (EmployeeRole == "Manager")
            {
                Manager newManager = new Manager
                {
                    Name = Employee.Name,
                    Email = Employee.Email,
                    PhoneNumber = Employee.PhoneNumber
                };
                _managerService.AddManager(newManager);
            }
            else
            {
                _employeeService.AddEmployee(Employee);
            }

            return RedirectToPage("/Employees");
        }
    }
}
