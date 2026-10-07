using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class MonthlyPlanModel : PageModel
    {
        /// <summary>
        /// Gets the shift service used to retrieve shifts for the monthly plan.
        /// </summary>
        private readonly ShiftService _shiftService;

        /// <summary>
        /// Gets the employee service used to retrieve employees for the monthly plan.
        /// </summary>
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Gets or sets the current month being displayed.
        /// </summary>
        [BindProperty]
        public DateTime CurrentMonth { get; set; }

        /// <summary>
        /// Gets the previous month.
        /// </summary>
        public DateTime PreviousMonth => CurrentMonth.AddMonths(-1);

        /// <summary>
        /// Gets the next month.
        /// </summary>
        public DateTime NextMonth => CurrentMonth.AddMonths(1);

        /// <summary>
        /// Gets the days displayed in the calendar.
        /// Empty calendar cells have a null Date.
        /// </summary>
        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();

        /// <summary>
        /// Gets all shifts for the currently displayed month.
        /// </summary>
        public List<Shift> Shifts { get; set; } = new List<Shift>();

        /// <summary>
        /// Gets or sets the employee shift summary for the currently displayed month.
        /// </summary>
        public EmployeeShiftSummary? EmployeeShiftSummary { get; set; }

        /// <summary>
        /// Gets or sets the SelectList of employees to be used in the employee selection dropdown on the create shift page.
        /// </summary>
        public SelectList EmployeeSelectList = new SelectList(new List<Employee>(), "EmployeeId", "Name");

        /// <summary>
        /// Gets or sets the user ID to search for employee shift summary.
        /// </summary>
        [BindProperty]
        public int? SearchUserId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyPlanModel"/> class with the specified shift service.
        /// </summary>
        /// <param name="shiftService">The shift service to use.</param>
        /// <param name="employeeService">The employee service to use.</param>
        public MonthlyPlanModel(ShiftService shiftService, EmployeeService employeeService)
        {
            _shiftService = shiftService;
            _employeeService = employeeService;
        }

        public IActionResult OnGet(int? year, int? month, int? userId)
        {
            // If no month was supplied, redirect to the current month.
            if (!year.HasValue || !month.HasValue)
            {
                return RedirectToPage("/MonthlyPlan", new { year = DateTime.Now.Year, month = DateTime.Now.Month });
            }

            EmployeeSelectList = new SelectList(_employeeService.GetAllEmployees(), "EmployeeId", "Name");

            // Set the current month.
            CurrentMonth = new DateTime(year.Value, month.Value, 1);

            // Get shifts for the month.
            Shifts = _shiftService.GetMonthlyPlan(year.Value, month.Value);

            // Get the employee shift summary if a user ID is provided.
            if (userId.HasValue)
            {
                DateOnly.TryParse($"{year.Value}-{month.Value}-01", out DateOnly parsedDate);

                EmployeeShiftSummary = _shiftService.GetMonthlyShiftsByEmployeeId(parsedDate, userId.Value);
            }

            // Generate the calendar and map the shifts to their dates.
            GenerateCalendar();

            return Page();
        }

        public IActionResult OnPostSearch()
        {
            return RedirectToPage("/MonthlyPlan", new { userId = SearchUserId });
        }

        /// <summary>
        /// Generates the calendar days and maps shifts to each date.
        /// </summary>
        private void GenerateCalendar()
        {
            DateOnly firstDay = DateOnly.FromDateTime(CurrentMonth);

            int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);

            Dictionary<DateOnly, List<Shift>> shiftsByDate = Shifts.GroupBy(shift => shift.Date).ToDictionary(shift => shift.Key, shift => shift.ToList());

            // Monday = 0, Sunday = 6
            int startOffset = ((int)firstDay.DayOfWeek + 6) % 7;

            // Empty cells before the first day
            for (int i = 0; i < startOffset; i++)
            {
                Days.Add(new CalendarDay());
            }

            // Actual days
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateOnly date = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, day);

                shiftsByDate.TryGetValue(date, out List<Shift>? shifts);

                Days.Add(new CalendarDay
                {
                    Date = date,
                    Shifts = shifts ?? new List<Shift>()
                });
            }

            // Empty cells after the last day
            while (Days.Count % 7 != 0)
            {
                Days.Add(new CalendarDay());
            }
        }
    }

    /// <summary>
    /// Represents a single day in the monthly calendar.
    /// </summary>
    public class CalendarDay
    {
        /// <summary>
        /// Gets or sets the date.
        /// Null represents an empty calendar cell.
        /// </summary>
        public DateOnly? Date { get; set; }

        /// <summary>
        /// Gets or sets the shifts assigned to this date.
        /// </summary>
        public List<Shift> Shifts { get; set; } = new List<Shift>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarDay"/> class.
        /// </summary>
        public CalendarDay() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarDay"/> class with the specified date and shifts.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="shifts">The shifts.</param>
        public CalendarDay(DateOnly date, List<Shift> shifts): this()
        {
            Date = date;
            Shifts = shifts;
        }
    }
}
