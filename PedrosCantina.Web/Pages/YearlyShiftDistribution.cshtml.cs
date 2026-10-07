using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class YearlyShiftDistributionModel : PageModel
    {
        /// <summary>
        /// Gets the shift service used to retrieve employee shift distribution data for the yearly shift distribution page.
        /// </summary>
        private readonly ShiftService _shiftService;

        /// <summary>
        /// Gets or sets the list of employee shift summaries for the specified year.
        /// </summary>
        public List<EmployeeShiftSummary> EmployeeShiftSummaries { get; set; } = [];

        /// <summary>
        /// Gets or sets the year for which the employee shift distribution is being displayed.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the year to search for employee shift distribution data.
        /// </summary>
        [BindProperty]
        public int SearchYear { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="YearlyShiftDistributionModel"/> class with the specified shift service.
        /// </summary>
        /// <param name="shiftService">The shift service to use for retrieving employee shift distribution data.</param>
        public YearlyShiftDistributionModel(ShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        public void OnGet(int year)
        {
            Year = year;

            DateOnly parsedYear = new DateOnly(year, 1, 1);

            EmployeeShiftSummaries = _shiftService.GetEmployeeShiftDistributionByYear(parsedYear);
        }

        public IActionResult OnPostSearch()
        {
            return RedirectToPage("/YearlyShiftDistribution", new { year = SearchYear });
        }
    }
}