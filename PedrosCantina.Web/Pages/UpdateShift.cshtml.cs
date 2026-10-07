using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class UpdateShiftModel : PageModel
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
        public Shift Shift { get; set; } = new Shift();

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
        /// Initializes a new instance of the <see cref="UpdateShiftModel"/> class with the specified shift service, shift period service, employee service, and manager service.
        /// </summary>
        /// <param name="shiftService">The shift service to use.</param>
        /// <param name="shiftPeriodService">The shift period service to use.</param>
        /// <param name="employeeService">The employee service to use.</param>
        /// <param name="managerService">The manager service to use.</param>
        public UpdateShiftModel(ShiftService shiftService, ShiftPeriodService shiftPeriodService, EmployeeService employeeService, ManagerService managerService)
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

            ShiftKey shiftKey = new ShiftKey(parsedDate, shiftPeriod);

            Shift? shift = _shiftService.GetShiftById(shiftKey);

            if (shift == null)
            {
                return RedirectToPage("/MonthlyPlan", new { year = DateTime.Now.Year, month = DateTime.Now.Month });
            }

            Shift = shift;

            Date = parsedDate;
            Period = shiftPeriod.GetCapitalizedCode();

            ManagerId = Shift.Manager.EmployeeId;

            List<Employee> employees = Shift.Employees.Values.ToList();

            EmployeeIdToAdd1 = employees.ElementAtOrDefault(1)?.EmployeeId ?? 0;
            EmployeeIdToAdd2 = employees.ElementAtOrDefault(2)?.EmployeeId ?? 0;

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

            Shift updatedShift = new Shift(parsedDate, shiftPeriod, manager);

            // Add employees to the shift if they are not null and not the same employee
            if (employeeToAdd1 != null && employeeToAdd1.EmployeeId != manager.EmployeeId)
            {
                updatedShift.AddEmployee(employeeToAdd1);
            }

            // Add the second employee only if they are not null and not the same as the first employee or the manager
            if (employeeToAdd2 != null && employeeToAdd2.EmployeeId != manager.EmployeeId && employeeToAdd2.EmployeeId != employeeToAdd1?.EmployeeId)
            {
                updatedShift.AddEmployee(employeeToAdd2);
            }

            _shiftService.UpdateShift(updatedShift);

            return RedirectToPage("/MonthlyPlan", new { year = parsedDate.Year, month = parsedDate.Month });
        }
    }
}
