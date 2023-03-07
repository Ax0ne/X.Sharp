using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using X.Sharp.Web.Models;

namespace X.Sharp.Web.Controllers
{
    public class ExampleController : Controller
    {
        readonly IHostEnvironment _hostEnvironment;

        public ExampleController(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public ActionResult<string> ProblemDetails()
        {
            try
            {
                throw new ApplicationException("抛出异常");
            }
            catch (Exception ex)
            {
                return new ObjectResult(
                    new ProblemDetails
                    {
                        Title = "我是标题",
                        Detail = "................",
                        Status = StatusCodes.Status500InternalServerError,
                        Extensions = { ["A"] = 1, ["B"] = 2 }
                    });
            }
        }

        public IActionResult EncryptPwd(string password)
        {
            password = string.IsNullOrWhiteSpace(password) ? "123456" : password;
            var user = new User();
            var hasher = new PasswordHasher<User>();
            var hashPassword = hasher.HashPassword(user, password);
            var hashPassword1 = hasher.HashPassword(user, "123456");
            var isVerify = hasher.VerifyHashedPassword(user, hashPassword, password);
            return Json(isVerify);
        }


        [HttpPost("upload")]
        public async Task<JsonResult> Upload()
        {
            var form = await Request.ReadFormAsync();
            foreach (var formFile in form.Files)
            {
                var fileName = Path.Combine(_hostEnvironment.ContentRootPath, "obj", Guid.NewGuid().ToString());
                fileName += Path.GetExtension(formFile.FileName);
                await using var fileStream = new FileStream(fileName, FileMode.Create);
                await formFile.CopyToAsync(fileStream);
            }

            return new JsonResult("OK");
        }
    }
}