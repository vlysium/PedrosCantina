using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PedrosCantina.Library.Models;
using PedrosCantina.Library.Services;

namespace PedrosCantina.Web.Pages
{
    public class ShiftDetailsModel : PageModel
    {
        /// <summary>
        /// Gets the shift service used to retrieve shifts for the shift details page.
        /// </summary>
        private readonly ShiftService _shiftService;

        /// <summary>
        /// Gets the shift period service used to retrieve shift periods for the shift details page.
        /// </summary>
        private readonly ShiftPeriodService _shiftPeriodService;

        /// <summary>
        /// Gets or sets the shift to be displayed on the shift details page.
        /// </summary>
        [BindProperty]
        public Shift Shift { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftDetailsModel"/> class with the specified shift service.
        /// </summary>
        /// <param name="shiftService">The shift service to use.</param>
        /// <param name="shiftPeriodService">The shift period service to use.</param>
        public ShiftDetailsModel(ShiftService shiftService, ShiftPeriodService shiftPeriodService)
        {
            _shiftService = shiftService;
            _shiftPeriodService = shiftPeriodService;
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

            if (shift != null)
            {
                Shift = shift;
                return Page();
            }

            return RedirectToPage("/MonthlyPlan", new { year = DateTime.Now.Year, month = DateTime.Now.Month });
        }
    }
}
