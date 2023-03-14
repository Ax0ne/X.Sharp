// Copyright (c) Ax0ne.  All Rights Reserved

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using X.Sharp.Web.Models;
using X.Sharp.Web.Services;
using X.Sharp.Web.Utils;

namespace X.Sharp.Web.Controllers
{
    public class ExampleController : Controller
    {
        readonly IHostEnvironment _hostEnvironment;
        private IEmailService _emailService;

        public ExampleController(IHostEnvironment hostEnvironment, IEmailService emailService)
        {
            _hostEnvironment = hostEnvironment;
            _emailService = emailService;
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

        public IActionResult OutputXml()
        {
            // 需要配置 options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            Response.ContentType = "text/xml";
            return Ok(new User { UserName = "HHH" });
        }

        public IActionResult SendEmail()
        {
            _emailService.Send(new EmailMessage
            {
                ReceiveEmail = "407869341@qq.com",
                ReceiveName = "接收者",
                SenderEmail = "test@outlook.com",
                SenderName = "发送者",
                Subject = "主题",
                Content = "内容"
            });
            return Ok();
        }
        public IActionResult VerifyCode(string text)
        {
            return File(ImageUtils.GenerateVerifyCode(text), "image/png");
        }
    }
}