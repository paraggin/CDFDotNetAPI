using Microsoft.AspNetCore.Mvc;


namespace CDF_Services.IServices.IHolidayCalendarServices
{
    public interface IITDepartmentService
    {
        Task<IActionResult> ListITDepartment();
    }
}
