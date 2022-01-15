using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using WebAppFeatures.Models;
using WebAppFeatures.ServicesRepository;
using WebAppFeatures.ViewModel;
using System.Globalization;
using WebAppFeatures.Helpers;
using System.Net.Mail;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace WebAppFeatures.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly IUserRepository userRepository;
        private readonly IVerificationRepository verificationRepository;
        private readonly AppSettings appSettings;
        private readonly IWebHostEnvironment env;
        public AccountController(ILogger<AccountController> _logger,
            IUserRepository _userRepository,
            IVerificationRepository _verificationRepository,
            IOptions<AppSettings> _appSettings,
            IWebHostEnvironment _env)
        {

            logger = _logger;
            userRepository = _userRepository;
            verificationRepository = _verificationRepository;
            appSettings = _appSettings.Value;
            env = _env;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if(HttpContext.Session.GetString("UserId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            model.UserName = model.UserName.Trim();
            if (ModelState.IsValid)
            {
                var culture = CultureInfo.CurrentCulture;
                logger.LogInformation($"culture: {culture}");
                if (model.UserName.Contains('@'))
                {
                    if (!userRepository.CheckEmailExists(model.UserName))
                    {
                        if (culture.Name == "en-US")
                        {
                            ModelState.AddModelError("UserName", "Email is invalid");
                        }
                        else if (culture.Name == "ar-EG")
                        {
                            ModelState.AddModelError("UserName", "البريد الإلكتروني غير صحيح");
                        }
                    }
                    else
                    {
                        var getUserDetailsByEmail = userRepository.GetUserbyEmail(model.UserName);
                        string UserId = getUserDetailsByEmail.UserId;
                        string UserPasswordHash = getUserDetailsByEmail.PasswordHash;
                        var userSalt = userRepository.GetUserSaltbyUserId(UserId);
                        
                        var UserSaltedPassword = GenerateHashSha512.Sha512(model.Password, userSalt.PasswordSalt);
                      
                        if (string.Equals(UserPasswordHash, UserSaltedPassword, StringComparison.Ordinal))
                        {
                            if (!verificationRepository.CheckIsAlreadyVerifiedRegistration(UserId))
                            {
                                if (culture.Name == "en-US")
                                {
                                    TempData["InfoLoginVerfy"] = "Email Verification Pending";
                                }
                                else if (culture.Name == "ar-EG")
                                {
                                    TempData["InfoLoginVerfy"] = "التحقق من البريد الإلكتروني معلق";
                                }
                                return View();
                            }
                            else
                            {
                                if(getUserDetailsByEmail.Deleted == false)
                                {
                                    HttpContext.Session.SetString("UserId", Convert.ToString(UserId));
                                    return RedirectToAction("Index", "Dashboard");
                                }
                                else
                                {
                                    if (culture.Name == "en-US")
                                    {
                                       TempData["InfoLoginVerfy"] = "Account is Deleteing";
                                    }
                                    else if (culture.Name == "ar-EG")
                                    {

                                        TempData["InfoLoginVerfy"] = "الحساب محذوف";
                                    }
                                    return View();
                                }                              
                            }
                        }
                        else
                        {
                            if (culture.Name == "en-US")
                            {
                                ModelState.AddModelError("Password", "Password is Invalid");
                            }
                            else if (culture.Name == "ar-EG")
                            {
                                ModelState.AddModelError("Password", "كلمة المرور غير صيحية");
                            }
                        }
                    }
                }
                else
                {
                    if (!userRepository.CheckUserNameExists(model.UserName))
                    {
                        if (culture.Name == "en-US")
                        {
                            ModelState.AddModelError("UserName", "Username is invalid");
                        }
                        else if (culture.Name == "ar-EG")
                        {
                            ModelState.AddModelError("UserName", "اسم المستخدم غير صحيح");
                        }
                    }
                    else
                    {
                        var getUserDetailsByUserName = userRepository.GetUserbyUserName(model.UserName);
                        string UserId = getUserDetailsByUserName.UserId;
                        string UserPasswordHash = getUserDetailsByUserName.PasswordHash;
                        var userSalt = userRepository.GetUserSaltbyUserId(UserId);
                       
                        var UserSaltedPassword = GenerateHashSha512.Sha512(model.Password, userSalt.PasswordSalt);
                        
                        if (string.Equals(UserPasswordHash, UserSaltedPassword, StringComparison.Ordinal))
                        {
                            if (!verificationRepository.CheckIsAlreadyVerifiedRegistration(UserId))
                            {
                                if (culture.Name == "en-US")
                                {
                                    TempData["InfoLoginVerfy"] = "Email Verification Pending";
                                }
                                else if (culture.Name == "ar-EG")
                                {
                                    TempData["InfoLoginVerfy"] = "التحقق من البريد الإلكتروني معلق";
                                }
                                return View();
                            }
                            else
                            {
                                if (getUserDetailsByUserName.Deleted == false)
                                {
                                    HttpContext.Session.SetString("UserId", Convert.ToString(UserId));
                                    return RedirectToAction("Index", "Dashboard");
                                }
                                else
                                {
                                    if (culture.Name == "en-US")
                                    {
                                        TempData["InfoLoginVerfy"] = "Account is Deleteing";
                                    }
                                    else if (culture.Name == "ar-EG")
                                    {
                                        TempData["InfoLoginVerfy"] = "الحساب محذوف";
                                    }
                                    return View();
                                }
                            }
                        }
                        else
                        {
                            if (culture.Name == "en-US")
                            {
                                ModelState.AddModelError("Password", "Password is Invalid");
                            }
                            else if (culture.Name == "ar-EG")
                            {
                                ModelState.AddModelError("Password", "كلمة المرور غير صيحية");
                            }
                        }
                    }
                }              
            }
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (userRepository.CheckEmailExists(model.Email))
                {
                    var culture = CultureInfo.CurrentCulture;
                    if (culture.Name == "en-US")
                    {
                        TempData["msgCheckEmailExists"] = "This email is already registered";
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["msgCheckEmailExists"] = "عنوان البريد الإلكترونى هذا مسجل بالفعل";
                    }
                    string msgError = TempData["msgCheckEmailExists"].ToString();
                    ModelState.AddModelError("Email", msgError);
                    return View(model);
                }
                if (userRepository.CheckUserNameExists(new MailAddress(model.Email).User))
                {
                    var culture = CultureInfo.CurrentCulture;
                    if (culture.Name == "en-US")
                    {
                        TempData["msgCheckEmailExists"] = "This User name for email is already registered";
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["msgCheckEmailExists"] = " اسم المستخدم لعنوان البريد الإلكترونى هذا مسجل بالفعل";
                    }
                    string msgError = TempData["msgCheckEmailExists"].ToString();
                    ModelState.AddModelError("Email", msgError);
                    return View(model);
                }
                if (new MailAddress(model.Email).User.Length < 5)
                {
                    var culture = CultureInfo.CurrentCulture;
                    if (culture.Name == "en-US")
                    {
                        TempData["msgCheckEmailExists"] = "This User name for email is short";
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["msgCheckEmailExists"] = " اسم المستخدم لعنوان البريد الإلكترونى قصير للغاية";
                    }
                    string msgError = TempData["msgCheckEmailExists"].ToString();
                    ModelState.AddModelError("Email", msgError);
                    return View(model);
                }
                var salt = GenerateRandomNumbers.RandomNumbers(20);
                var saltedpassword = GenerateHashSha512.Sha512(model.Password, salt);
                var path = env.WebRootFileProvider.GetFileInfo("images/default-avatar.png")?.PhysicalPath;
                var img = System.IO.File.OpenRead(path);
                var dataImage = new MemoryStream();
                img.CopyTo(dataImage);
                logger.LogInformation(dataImage.CanWrite.ToString());
                string AvalibaleId= Guid.NewGuid().ToString();              
                User user = new()
                {
                    UserId = AvalibaleId,
                    FullName = new MailAddress(model.Email).User,
                    UserName = new MailAddress(model.Email).User,
                    Email = model.Email,
                    PasswordHash = saltedpassword,
                    Country = GetInfoByIP.GetCountery(),
                    City = GetInfoByIP.GetCity(),
                    CreatedDate = DateTime.Now,
                    Deleted = false,
                    Status = true,
                    PhoneKey = "",
                    PhoneNumber = "",
                    Gender = "",
                    Image = dataImage.ToArray()
                };
                var userIdResult = userRepository.RegisterUser(user, salt);
                if (userIdResult.Length > 0)
                {
                    Send(model, userIdResult);
                    var culture = CultureInfo.CurrentCulture;
                    if (culture.Name == "en-US")
                    {
                        TempData["Registered_Success_Message"] = "User successfully registered Check your email to confirm your account";
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["Registered_Success_Message"] = "تم تسجيل المستخدم بنجاح تحقق من بريدك الإلكتروني لتأكيد حسابك";
                    }
                }
                else
                {
                    var culture = CultureInfo.CurrentCulture;
                    if (culture.Name == "en-US")
                    {
                        TempData["Registered_Error_Message"] = "Message! Error while registering";
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["Registered_Error_Message"] = "رسالة! خطأ أثناء التسجيل";
                    }
                }
            }
            else
            {
                return View(model);
            }
            return RedirectToAction("Register", "Account");
        }
        [NonAction]
        private void Send(RegisterViewModel model, string userid)
        {
            var emailVerficationToken = GenerateHashSha256.ComputeSha256Hash((GenerateRandomNumbers.RandomNumbers(6)));
            verificationRepository.SendRegistrationVerificationToken(userid, emailVerficationToken);
            MailMessage message = new ();
            SmtpClient smtpClient = new ();

            try
            {
                MailAddress fromAddress = new (appSettings.EmailFrom);
                message.From = fromAddress;
                message.To.Add(model.Email);
                message.Subject = "Confirm your account";
                message.IsBodyHtml = true;
                message.Body = SendVerificationEmail(model, userid, emailVerficationToken);
                smtpClient.Host = appSettings.Host;
                smtpClient.Port = Convert.ToInt32(appSettings.Port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(appSettings.EmailFrom, appSettings.Password);
                smtpClient.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [NonAction]
        public string SendVerificationEmail(RegisterViewModel model, string userid, string token)
        {
            AesAlgorithm aesAlgorithm = new();
            var key = string.Join(":", new string[] { DateTime.Now.Ticks.ToString(), userid.ToString() });
            var encrypt = aesAlgorithm.EncryptToBase64String(key);
            var linktoverify = appSettings.VerifyRegistrationUrl + "?key=" + HttpUtility.UrlEncode(encrypt) + "&hashtoken=" + HttpUtility.UrlEncode(token);
            var path = env.WebRootFileProvider.GetFileInfo("index-email-confirm.html")?.PhysicalPath;
            var stringtemplate = new StreamReader(path);
            var mailText = stringtemplate.ReadToEnd();
            stringtemplate.Close();
            mailText = mailText.Replace("[usernameHere]", new MailAddress(model.Email).User).Replace("[txtlinktoverify]", linktoverify);
            return mailText;
        }
        public IActionResult Verify(string key, string hashtoken)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(hashtoken))
            {
                var culture = CultureInfo.CurrentCulture;
                try
                {
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(hashtoken))
                    {
                        var arrayVakue = SecurityManager.SplitToken(key);
                        if (arrayVakue != null)
                        {
                            //arrayVakue[1] "UserId"
                            string userid = arrayVakue[1];
                            var rvModel = verificationRepository.GetRegistrationGeneratedToken(userid);
                            if (rvModel != null)
                            {
                                var result = SecurityManager.IsTokenValid(arrayVakue, hashtoken, rvModel.GeneratedToken);
                                if (result == 1)
                                {
                                    if (culture.Name == "en-US")
                                    {
                                        TempData["TokenErrorMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                                    }
                                    else if (culture.Name == "ar-EG")
                                    {
                                        TempData["TokenErrorMessage"] = "عذراً فشل التحقق، من فضلك أطلب رابط تحقق جديد!";
                                    }
                                    return RedirectToAction("Login", "Account");
                                }
                                if (result == 2)
                                {
                                    if (culture.Name == "en-US")
                                    {
                                        TempData["TokenErrorMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                                    }
                                    else if (culture.Name == "ar-EG")
                                    {
                                        TempData["TokenErrorMessage"] = "عذراً فشل التحقق، من فضلك أطلب رابط تحقق جديد!";
                                    }
                                    return RedirectToAction("Login", "Account");
                                }
                                if (result == 0)
                                {
                                    if (verificationRepository.CheckIsAlreadyVerifiedRegistration(Convert.ToString(arrayVakue[1])))
                                    {
                                        if (culture.Name == "en-US")
                                        {
                                            TempData["TokenErrorMessage"] = "Sorry Link Expired";
                                        }
                                        else if (culture.Name == "ar-EG")
                                        {
                                            TempData["TokenErrorMessage"] = "انتهت صلاحية الرابط";
                                        }
                                        return RedirectToAction("Login", "Account");
                                    }
                                    HttpContext.Session.SetString("VerificationUserId", arrayVakue[1]);
                                    var updateresult = verificationRepository.UpdateRegisterVerification(Convert.ToString(arrayVakue[1]));
                                    if (updateresult)
                                    {
                                        if (culture.Name == "en-US")
                                        {
                                            TempData["Verify"] = "Your account has been successfully confirmed";
                                        }
                                        else if (culture.Name == "ar-EG")
                                        {
                                            TempData["Verify"] = "تم التأكيد حسابك بنجاح";
                                        }
                                        TempData["CHECK"] = "CHECK";
                                        return RedirectToAction("Completed", "Account");
                                    }
                                    else
                                    {
                                        if (culture.Name == "en-US")
                                        {
                                            TempData["TokenErrorMessage"] = "Sorry Link Expired";
                                        }
                                        else if (culture.Name == "ar-EG")
                                        {
                                            TempData["TokenErrorMessage"] = "انتهت صلاحية الرابط";
                                        }
                                        return RedirectToAction("Login", "Account");
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    if (culture.Name == "en-US")
                    {
                        TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["TokenMessage"] = "عذراً فشل التحقق، من فضلك أطلب رابط تحقق جديد!";
                    }
                    return RedirectToAction("Login", "Account");
                }
                if (culture.Name == "en-US")
                {
                    TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                }
                else if (culture.Name == "ar-EG")
                {
                    TempData["TokenMessage"] = "عذراً فشل التحقق، من فضلك أطلب رابط تحقق جديد!";
                }
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpGet]
        public IActionResult Completed()
        {
            if (TempData["CHECK"] != null)
            {
                if (Convert.ToString(TempData["Verify"]) == "Your account has been successfully confirmed"
                              || Convert.ToString(TempData["Verify"]) == "تم التأكيد حسابك بنجاح")
                {
                    var culture = CultureInfo.CurrentCulture;
                    if (culture.Name == "en-US")
                    {

                        TempData["RegistrationCompleted"] = "Registration Process Completed, Now you can Login and Access Account";
                        TempData["TokenErrorMessage"] = null;
                        TempData["TokenMessage"] = null;
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["RegistrationCompleted"] = "اكتملت عملية التسجيل ويمكنك الآن تسجيل الدخول والوصول إلى الحساب";
                        TempData["TokenErrorMessage"] = null;
                        TempData["TokenMessage"] = null;
                    }

                    return View();
                }
                else
                {
                    var culture = CultureInfo.CurrentCulture;
                    if (culture.Name == "en-US")
                    {
                        TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                        TempData["TokenErrorMessage"] = null;
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["TokenMessage"] = "عذراً فشل التحقق، من فضلك أطلب رابط تحقق جديد!";
                        TempData["TokenErrorMessage"] = null;
                    }
                    return RedirectToAction("Login", "Account");
                }

            }
            else
            {
                return RedirectToAction("Register", "Account");
            }
        }
        public IActionResult RenderImage(string Key)
        {
            if (!string.IsNullOrEmpty(Key))
            {
                var arrayVakue = SecurityManager.SplitToken(Key);
                if (arrayVakue != null)
                {
                    string userId = arrayVakue[1];
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = userRepository.GetUserbyUserId(userId);
                        if (user != null && user.Deleted == false)
                        {

                            byte[] photoBackUser = user.Image;
                            return File(photoBackUser, "image/png");

                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgetPassword(ForgetPasswordViewModel model)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                if (ModelState.IsValid)
                {
                    var culture = CultureInfo.CurrentCulture;
                    logger.LogInformation($"culture: {culture}");
                    if (!userRepository.CheckEmailExists(model.Email))
                    {
                        if (culture.Name == "en-US")
                        {
                            TempData["InfoForegetVerfy"] = "This account is not registered with us";
                        }
                        else if (culture.Name == "ar-EG")
                        {
                            TempData["InfoForegetVerfy"] = "هذا الحساب غير مسجل لدينا";
                        }
                    }
                    else
                    {
                        User getUser = userRepository.GetUserbyEmail(model.Email);
                        if (getUser !=null)
                        {
                            SendEmailReset(getUser);
                            if (culture.Name == "en-US")
                            {
                                TempData["InfoForegetVerfy"] = " An email has been sent to the address you have registered, Please follow the link in the email to complete your password reset request.";
                            }
                            else if (culture.Name == "ar-EG")
                            {
                                TempData["InfoForegetVerfy"] = "تم إرسال بريد إلكتروني إلى العنوان الذي قمت بتسجيله ، يرجى اتباع الرابط الموجود في البريد الإلكتروني لإكمال طلب إعادة تعيين كلمة المرور الخاصة بك";
                            }
                        }
                    }
                }
                return RedirectToAction("ForgetPassword", "Account");

            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [NonAction]
        private void SendEmailReset(User model)
        {
            var resetVerficationToken = GenerateHashSha256.ComputeSha256Hash((GenerateRandomNumbers.RandomNumbers(6)));
            verificationRepository.SendResetVerificationToken(model.UserId, resetVerficationToken);
            MailMessage message = new();
            SmtpClient smtpClient = new();

            try
            {
                MailAddress fromAddress = new(appSettings.EmailFrom);
                message.From = fromAddress;
                message.To.Add(model.Email);
                message.Subject = "Reset Your Password";
                message.IsBodyHtml = true;
                message.Body = SendResetPasswordEmail(model, resetVerficationToken);
                smtpClient.Host = appSettings.Host;
                smtpClient.Port = Convert.ToInt32(appSettings.Port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(appSettings.EmailFrom, appSettings.Password);
                smtpClient.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [NonAction]
        public string SendResetPasswordEmail(User model, string token)
        {
            AesAlgorithm aesAlgorithm = new();
            var key = string.Join(":", new string[] { DateTime.Now.Ticks.ToString(), model.UserId.ToString() });
            var encrypt = aesAlgorithm.EncryptToBase64String(key);
            var linktoverify = appSettings.VerifyResetPasswordUrl + "?key=" + HttpUtility.UrlEncode(encrypt) + "&hashtoken=" + HttpUtility.UrlEncode(token);
            var path = env.WebRootFileProvider.GetFileInfo("index-password-reset.html")?.PhysicalPath;
            var stringtemplate = new StreamReader(path);
            var mailText = stringtemplate.ReadToEnd();
            stringtemplate.Close();
            mailText = mailText.Replace("[txtlinktoverify]", linktoverify).Replace("[FullNameHere]",model.FullName);
            return mailText;
        }
        public IActionResult VerifyResetPassword(string key, string hashtoken)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(hashtoken))
            {
                var culture = CultureInfo.CurrentCulture;
                try
                {
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(hashtoken))
                    {
                        var arrayVakue = SecurityManager.SplitToken(key);
                        if (arrayVakue != null)
                        {
                            string userid = arrayVakue[1];
                            var rvModel = verificationRepository.GetResetGeneratedToken(userid);
                            if (rvModel != null)
                            {
                                var result = SecurityManager.IsTokenValid(arrayVakue, hashtoken, rvModel.GeneratedToken);
                                if (result == 1)
                                {
                                    if (culture.Name == "en-US")
                                    {
                                        TempData["TokenErrorMessage"] = "Sorry Reset Password Verification Failed Please request a new Verification link!";
                                    }
                                    else if (culture.Name == "ar-EG")
                                    {
                                        TempData["TokenErrorMessage"] = "عذرًا ، فشل التحقق من إعادة تعيين كلمة المرور ، الرجاء طلب ارتباط تحقق جديد!";
                                    }
                                    return RedirectToAction("ForgetPassword", "Account");
                                }
                                if (result == 2)
                                {
                                    if (culture.Name == "en-US")
                                    {
                                        TempData["TokenErrorMessage"] = "Sorry Reset Password Verification Failed Please request a new Verification link!";
                                    }
                                    else if (culture.Name == "ar-EG")
                                    {
                                        TempData["TokenErrorMessage"] = "عذرًا ، فشل التحقق من إعادة تعيين كلمة المرور ، الرجاء طلب ارتباط تحقق جديد!";
                                    }
                                    return RedirectToAction("ForgetPassword", "Account");
                                }
                                if (result == 0)
                                {
                                    if (verificationRepository.CheckIsAlreadyVerifiedResetPassword(Convert.ToString(arrayVakue[1]), hashtoken))
                                    {
                                        if (culture.Name == "en-US")
                                        {
                                            TempData["TokenErrorMessage"] = "Sorry Link Expired";
                                        }
                                        else if (culture.Name == "ar-EG")
                                        {
                                            TempData["TokenErrorMessage"] = "انتهت صلاحية الرابط";
                                        }
                                        return RedirectToAction("ForgetPassword", "Account");
                                    }
                                    TempData["CHECKRESET"] = "CHECKRESET";
                                    HttpContext.Session.SetString("VerificationUserId1", arrayVakue[1]);
                                    return RedirectToAction("ResetPassword", "Account");
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    if (culture.Name == "en-US")
                    {
                        TempData["TokenErrorMessage"] = "Sorry Reset Password Verification Failed Please request a new Verification link!";
                    }
                    else if (culture.Name == "ar-EG")
                    {
                        TempData["TokenErrorMessage"] = "عذرًا ، فشل التحقق من إعادة تعيين كلمة المرور ، الرجاء طلب ارتباط تحقق جديد!";
                    }
                    return RedirectToAction("ForgetPassword", "Account");
                }
                if (culture.Name == "en-US")
                {
                    TempData["TokenErrorMessage"] = "Sorry Reset Password Verification Failed Please request a new Verification link!";
                }
                else if (culture.Name == "ar-EG")
                {
                    TempData["TokenErrorMessage"] = "عذرًا ، فشل التحقق من إعادة تعيين كلمة المرور ، الرجاء طلب ارتباط تحقق جديد!";
                }
                return RedirectToAction("ForgetPassword", "Account");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (TempData["CHECKRESET"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (HttpContext.Session.GetString("VerificationUserId1") != null)
            {
                if (ModelState.IsValid)
                {
                    string userid = Convert.ToString(HttpContext.Session.GetString("VerificationUserId1"));
                    if (!string.Equals(model.Password, model.ComparePassword, StringComparison.Ordinal))
                    {
                        TempData["ResetErrorMessage"] = "Password Does not Match";
                        return View(model);
                    }
                    else
                    {
                        var getuserdetails = userRepository.GetUserbyUserId(userid);
                        var salt = GenerateRandomNumbers.RandomNumbers(20);
                        var saltedpassword = GenerateHashSha512.Sha512(model.Password, salt);
                        bool result = userRepository.UpdatePasswordandHistory(getuserdetails.UserId, saltedpassword, salt);
                        if (result)
                        {
                            if (verificationRepository.UpdateResetVerification(getuserdetails.UserId))
                            {
                                return RedirectToAction("Login", "Account");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Account");
                            }
                        }
                        else
                        {
                            TempData["ResetErrorMessage"] = "Something Went Wrong Please try again!";
                            return View(model);
                        }
                    }
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
