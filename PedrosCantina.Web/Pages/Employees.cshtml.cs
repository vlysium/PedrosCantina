using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class EmployeesModel : PageModel
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
        /// Gets or sets the list of all employees to be displayed on the page.
        /// </summary>
        public List<Employee> AllEmployees { get; set; } = new List<Employee>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesModel"/> class with the specified employee service.
        /// </summary>
        /// <param name="employeeService">The employee service to use.</param>
        /// <param name="managerService">The manager service to use.</param>
        public EmployeesModel(EmployeeService employeeService, ManagerService managerService)
        {
            _employeeService = employeeService;
            _managerService = managerService;
        }

        public void OnGet()
        {
            List<Employee> employees = _employeeService.GetAllEmployees();
            List<Manager> managers = _managerService.GetAllManagers();

            // Cast managers to Employee and add them to the AllEmployees list
            foreach (Employee employee in employees)
            {
                if (_managerService.isManager(employee.EmployeeId))
                {
                    Manager? manager = managers.FirstOrDefault(m => m.EmployeeId == employee.EmployeeId);
                    if (manager != null)
                    {
                        AllEmployees.Add(manager);
                    }
                }
                else
                {
                    AllEmployees.Add(employee);
                }
            }
        }
    }
}
