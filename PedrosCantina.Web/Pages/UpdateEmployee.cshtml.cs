using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class UpdateEmployeeModel : PageModel
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
        /// Gets the list of roles available for selection when updating an employee.
        /// </summary>
        public readonly SelectList Roles = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Value = "Employee", Text = "Medarbejder" },
            new SelectListItem { Value = "Manager", Text = "Leder" }
        }, "Value", "Text");
        
        /// <summary>
        /// The employee to be updated.
        /// </summary>
        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        /// <summary>
        /// The role of the employee to be updated (Employee or Manager).
        /// </summary>
        [BindProperty]
        public string EmployeeRole { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEmployeeModel"/> class with the specified employee service and manager service.
        /// </summary>
        /// <param name="employeeService">The employee service to use.</param>
        /// <param name="managerService">The manager service to use.</param>
        public UpdateEmployeeModel(EmployeeService employeeService, ManagerService managerService)
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
                EmployeeRole = "Manager";
                Employee = manager;
            }
            else
            {
                EmployeeRole = "Employee";
                Employee = employee;
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (EmployeeRole == "Manager" && !_managerService.isManager(Employee.EmployeeId))
            {
                Manager newManager = new Manager(Employee.EmployeeId, Employee.Name, Employee.Email, Employee.PhoneNumber);
                _managerService.PromoteToManager(newManager);
            }

            if (EmployeeRole == "Employee" && _managerService.isManager(Employee.EmployeeId))
            {
                _managerService.DemoteManager(Employee.EmployeeId);
            }
            
            _employeeService.UpdateEmployee(Employee);

            return RedirectToPage("/EmployeeDetails", new { id = Employee.EmployeeId });
        }
    }
}
