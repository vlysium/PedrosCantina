using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class CreateShiftModel : PageModel
    {
        /// <summary>
        /// Gets the shift service used to create shifts for the create shift page.
        /// </summary>
        private readonly ShiftService _shiftService;

        /// <summary>
        /// Gets the shift period service used to retrieve shift periods for the create shift page.
        /// </summary>
        private readonly ShiftPeriodService _shiftPeriodService;

        /// <summary>
        /// Gets the employee service used to retrieve employees for the create shift page.
        /// </summary>
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Gets the manager service used to retrieve managers for the create shift page.
        /// </summary>
        private readonly ManagerService _managerService;

        /// <summary>
        /// Gets or sets the list of employees to be displayed in the employee selection dropdown on the create shift page.
        /// </summary>
        public List<Employee> Employees { get; set; } = new List<Employee>();

        /// <summary>
        /// Gets or sets the SelectList of employees to be used in the employee selection dropdown on the create shift page.
        /// </summary>
        public SelectList EmployeeSelectList = new SelectList(new List<Employee>(), "EmployeeId", "Name");

        /// <summary>
        /// Gets or sets the list of managers to be displayed in the manager selection dropdown on the create shift page.
        /// </summary>
        public List<Manager> Managers { get; set; } = new List<Manager>();

        /// <summary>
        /// Gets or sets the SelectList of managers to be used in the manager selection dropdown on the create shift page.
        /// </summary>
        public SelectList ManagerSelectList = new SelectList(new List<Manager>(), "EmployeeId", "Name");

        /// <summary>
        /// Gets or sets the date of the shift to be created on the create shift page.
        /// </summary>
        [BindProperty]
        public DateOnly Date { get; set; }

        /// <summary>
        /// Gets or sets the shift period of the shift to be created on the create shift page.
        /// </summary>
        [BindProperty]
        public string Period { get; set; }

        /// <summary>
        /// Gets or sets the manager ID of the shift to be created on the create shift page.
        /// </summary>
        [BindProperty]
        public int ManagerId { get; set; }

        /// <summary>
        /// Gets or sets the first employee to be added to the shift on the create shift page.
        /// </summary>
        [BindProperty]
        public int EmployeeIdToAdd1 { get; set; }
        
        /// <summary>
        /// Gets or sets the second employee to be added to the shift on the create shift page.
        /// </summary>
        [BindProperty]
        public int EmployeeIdToAdd2 { get; set; }
        

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateShiftModel"/> class with the specified shift service.
        /// </summary>
        /// <param name="shiftService">The shift service to use.</param>
        /// <param name="shiftPeriodService">The shift period service to use.</param>
        /// <param name="employeeService">The employee service to use.</param>
        /// <param name="managerService">The manager service to use.</param>
        public CreateShiftModel(ShiftService shiftService, ShiftPeriodService shiftPeriodService, EmployeeService employeeService, ManagerService managerService)
        {
            _shiftService = shiftService;
            _shiftPeriodService = shiftPeriodService;
            _employeeService = employeeService;
            _managerService = managerService;
        }

        public IActionResult OnGet(string date, string period)
        {
            ShiftPeriod? shiftPeriod = _shiftPeriodService.GetShiftPeriodById(period);

            if (!DateOnly.TryParse(date, out DateOnly parsedDate) || shiftPeriod == null)
            {
                return RedirectToPage("/MonthlyPlan", new { year = DateTime.Now.Year, month = DateTime.Now.Month });
            }

            Date = parsedDate;
            Period = shiftPeriod.GetCapitalizedCode();

            ManagerSelectList = new SelectList(_managerService.GetAllManagers(), "EmployeeId", "Name");
            EmployeeSelectList = new SelectList(_employeeService.GetAllEmployees(), "EmployeeId", "Name");

            return Page();
        }

        public IActionResult OnPost()
        {
            DateOnly parsedDate = Date;
            ShiftPeriod shiftPeriod = _shiftPeriodService.GetShiftPeriodById(Period)!;
            Manager manager = _managerService.GetManagerById(ManagerId)!;
            Employee? employeeToAdd1 = _employeeService.GetEmployeeById(EmployeeIdToAdd1);
            Employee? employeeToAdd2 = _employeeService.GetEmployeeById(EmployeeIdToAdd2);

            Shift newShift = new Shift(parsedDate, shiftPeriod, manager);

            // Add employees to the shift if they are not null and not the same employee
            if (employeeToAdd1 != null)
            {
                newShift.AddEmployee(employeeToAdd1);
            }

            // Add the second employee only if they are not null and not the same as the first employee
            if (employeeToAdd2 != null && employeeToAdd2.EmployeeId != employeeToAdd1?.EmployeeId)
            {
                newShift.AddEmployee(employeeToAdd2);
            }

            _shiftService.AddShift(newShift);

            return RedirectToPage("/ShiftDetails", new { date = parsedDate.ToString("yyyy-MM-dd"), period = shiftPeriod.Code });
        }
    }
}
