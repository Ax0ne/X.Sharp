using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using X.Sharp.Web.Models;
using X.Sharp.Web.Services;

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
                Content =
                    @"<table dir=""ltr""><tbody><tr><td id=""x_i1"" style=""padding: 0px; font-family: &quot;Segoe UI Semibold&quot;, &quot;Segoe UI Bold&quot;, &quot;Segoe UI&quot;, &quot;Helvetica Neue Medium&quot;, Arial, sans-serif; font-size: 17px; color: rgb(112, 112, 112) !important;"">Microsoft 帐户</td></tr><tr><td id=""x_i2"" style=""padding: 0px; font-family: &quot;Segoe UI Light&quot;, &quot;Segoe UI&quot;, &quot;Helvetica Neue Medium&quot;, Arial, sans-serif; font-size: 41px; color: rgb(38, 114, 236) !important;"">登录活动异常</td></tr><tr><td id=""x_i3"" style=""padding: 25px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">已检测到最近登录到 Microsoft 帐户 <a href=""mailto:Hu**e@outlook.com"" target=""_blank"" rel=""noopener noreferrer"" data-auth=""NotApplicable"" dir=""ltr"" id=""x_iAccount"" class=""x_link"" style=""color: rgb(38, 114, 236) !important; text-decoration: none;"" data-linkindex=""0"">Hu**e@outlook.com</a> 的一些异常。</td></tr><tr><td id=""x_i4"" style=""padding: 25px 0px 0px; font-family: &quot;Segoe UI Bold&quot;, &quot;Segoe UI Semibold&quot;, &quot;Segoe UI&quot;, &quot;Helvetica Neue Medium&quot;, Arial, sans-serif; font-size: 14px; font-weight: bold; color: rgb(42, 42, 42) !important;"">登录详细信息</td></tr><tr><td id=""x_i5"" style=""padding: 6px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">国家/地区: 美国</td></tr><tr><td id=""x_i6"" style=""padding: 6px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">IP 地址: 54.185.150.149</td></tr><tr><td id=""x_i7"" style=""padding: 6px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">日期: 2023/3/9 14:33 (GMT)</td></tr><tr><td id=""x_i8"" style=""padding: 6px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">平台: Windows</td></tr><tr><td id=""x_i9"" style=""padding: 6px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">浏览器: Chrome</td></tr><tr><td id=""x_i10"" style=""padding: 25px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">请转到最近活动页，让我们确定这是否是你本人。如果不是，我们将帮助你保护帐户。如果是，我们将在以后信任类似活动。</td></tr><tr><td style=""padding: 25px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;""><table border=""0"" cellspacing=""0""><tbody><tr><td bgcolor=""#2672ec"" style=""background-color: rgb(38, 114, 236) !important; padding: 5px 20px; min-width: 50px;""><a href=""https://account.microsoft.com/activity"" target=""_blank"" rel=""noopener noreferrer"" data-auth=""NotApplicable"" id=""x_i11"" style=""font-family: &quot;Segoe UI Semibold&quot;, &quot;Segoe UI Bold&quot;, &quot;Segoe UI&quot;, &quot;Helvetica Neue Medium&quot;, Arial, sans-serif; font-size: 14px; text-align: center; text-decoration: none; font-weight: 600; letter-spacing: 0.02em; color: rgb(255, 255, 255) !important;"" data-linkindex=""1"">查看最近的活动</a></td></tr></tbody></table></td></tr><tr><td id=""x_i12"" style=""padding: 25px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">若要在收到安全通知的位置选择退出或进行更改，<a href=""https://account.live.com/SecurityNotifications/Update"" target=""_blank"" rel=""noopener noreferrer"" data-auth=""NotApplicable"" id=""x_iLink5"" class=""x_link"" style=""color: rgb(38, 114, 236) !important; text-decoration: none;"" data-linkindex=""2"">请单击此处</a>。</td></tr><tr><td id=""x_i13"" style=""padding: 25px 0px 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">谢谢!</td></tr><tr><td id=""x_i14"" style=""padding: 0px; font-family: &quot;Segoe UI&quot;, Tahoma, Verdana, Arial, sans-serif; font-size: 14px; color: rgb(42, 42, 42) !important;"">Microsoft 帐户团队</td></tr></tbody></table>"
            });
            return Ok();
        }
    }
}