using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult getAllStudents()
        {
            string[] studentNames = { "A", "B", "C", "D" };
            return Ok(studentNames);
        }
    }
}
