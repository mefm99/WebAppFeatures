using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nancy;
using System;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Runtime.Versioning;
using System.Web;
using WebAppFeatures.Models;
using WebAppFeatures.Models.DataModel;
using WebAppFeatures.ServicesRepository;
using WebAppFeatures.ViewModel;
using WebAppFeatures.Helpers;
using ZXing;
using ZXing.QrCode;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Collections.Generic;

namespace WebAppFeatures.Controllers
{
    public class DashboardController : Controller
    {
        private readonly INotyfService notyf;
        private readonly IWebHostEnvironment env;
        private readonly AppSettings appSettings;
        private readonly IUserRepository userRepository;
        private readonly IVerificationRepository verificationRepository;
        public DashboardController(IWebHostEnvironment _env,
             IOptions<AppSettings> _appSettings,
             IUserRepository _userRepository,
             INotyfService _notyf,
            IVerificationRepository _verificationRepository)
        {
            notyf = _notyf;
            appSettings = _appSettings.Value;
            env = _env;
            userRepository = _userRepository;
            verificationRepository = _verificationRepository;
        }
        public IActionResult Index()
        {    
            if (HttpContext.Session.GetString("UserId") != null)
            {
                Program.CounterLogin = ++Program.CounterLogin;
                if (Program.CounterLogin == 1)
                {
                    notyf.Custom("Hello Again", 10, "#135224", "fa fa-user");
                }
                string uId = HttpContext.Session.GetString("UserId");
                User user = userRepository.GetUserbyUserId(uId);
                RegisterVerification registerVerification = verificationRepository.GetRegistrationGeneratedToken(uId);
                var wwwroot = env.WebRootPath;
                string[] filePaths = Directory.GetFiles(Path.Combine(wwwroot, "NumberSearch/"), $"{uId}*");
                var imgQR = Path.Combine(wwwroot, $"QrCodes/{uId}.png");
                var exists = System.IO.File.Exists(imgQR);
                if (exists)
                {
                    ViewBag.IsQrExists = "YES";
                    ViewBag.Url = appSettings.UrlIndexSearch;
                }
                else
                {
                    ViewBag.IsQrExists = "NO";
                }
                DashBoardInfo model = new()
                {
                    RegisterVerification = registerVerification,
                    User = user,
                    NumViewers = filePaths.Length.ToString()
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string uId = HttpContext.Session.GetString("UserId");
                User user = userRepository.GetUserbyUserId(uId);
                AesAlgorithm aesAlgorithm = new();
                var key = string.Join(":", new string[] { DateTime.Now.Ticks.ToString(), uId.ToString() });
                var encrypt = aesAlgorithm.EncryptToBase64String(key);
                EditUserViewModel model = new()
                {
                    FullName = user.FullName,
                    UserName = user.UserName,
                    City = user.City,
                    Country = user.Country,
                    Gender = user.Gender,
                    Email = user.Email,
                    Key = encrypt
                };
                var wwwroot = env.WebRootPath;
                var imgQR = Path.Combine(wwwroot, $"QrCodes/{uId}.png");
                var exists = System.IO.File.Exists(imgQR);
                if (!exists)
                {
                    ViewBag.QrGenerated = "OK";
                }
                else
                {
                    TempData["QrSrc"] = $"/QrCodes/{uId}.png";
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public IActionResult Users()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
               
                List<User> users = userRepository.AllUser();
                List<UserInfo> model = new ();
                foreach (User item in users)
                {
                    model.Add(new UserInfo
                    {
                        Deleted = item.Deleted,
                        FullName = item.FullName,
                        CreatedDate = item.CreatedDate,
                        Email = item.Email,
                        UserName = item.UserName
                    });
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Program.CounterLogin = 0;
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedOSPlatform("windows")]
        public IActionResult CreateQR()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string uId = HttpContext.Session.GetString("UserId");
                var wwwroot = env.WebRootPath;
                var imgQR = Path.Combine(wwwroot, $"QrCodes/{uId}.png");
                var exists = System.IO.File.Exists(imgQR);
                if (!exists)
                {
                    AesAlgorithm aesAlgorithm = new();
                    var key = string.Join(":", new string[] { DateTime.Now.Ticks.ToString(), uId.ToString() });
                    var encrypt = aesAlgorithm.EncryptToBase64String(key);
                    var QrString = appSettings.UrlSearch +
                        "?Key=" + HttpUtility.UrlEncode(encrypt);
                    var writer = new QRCodeWriter();
                    var resultBit = writer.encode(QrString, BarcodeFormat.QR_CODE, 200, 200);
                    var matrix = resultBit;
                    int scale = 2;
                    Bitmap result = new(matrix.Width * scale, matrix.Height * scale);
                    for (int x = 0; x < matrix.Height; x++)
                        for (int y = 0; y < matrix.Width; y++)
                        {
                            Color pixel = matrix[x, y] ? Color.Black : Color.White;
                            for (int i = 0; i < scale; i++)
                                for (int j = 0; j < scale; j++)
                                    result.SetPixel(x * scale + i, y * scale + j, pixel);
                        }
                    result.Save(wwwroot + $"\\QrCodes\\{uId}.png");
                    TempData["QrSrc"] = $"/QrCodes/{uId}.png";
                    notyf.Success("Your QR is Created Successfully");
                    return RedirectToAction("Profile", "Dashboard");
                }
                else
                {
                    return RedirectToAction("Profile", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQR()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string uId = HttpContext.Session.GetString("UserId");
                var wwwroot = env.WebRootPath;
                var imgQR = Path.Combine(wwwroot, $"QrCodes/{uId}.png");
                var exists = System.IO.File.Exists(imgQR);
                if (exists)
                {
                    System.IO.File.Delete(imgQR);
                }
                notyf.Warning("Your QR is Deleted Successfully");
                return RedirectToAction("Profile", "Dashboard");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedOSPlatform("windows")]
        public IActionResult SearchQR(IFormFile fileInput)
        {
            if (fileInput != null && fileInput.Length > 0)
            {
                string wwwRootPath = env.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(fileInput.FileName);
                string extension = Path.GetExtension(fileInput.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Search/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    fileInput.CopyTo(fileStream);
                }
                string url;
                var reader = new BarcodeReaderGeneric();
                Bitmap image = (Bitmap)Image.FromFile(path);
                using (image)
                {
                    LuminanceSource source;
                    source = new ZXing.Windows.Compatibility.BitmapLuminanceSource(image);
                    if (source != null && reader.Decode(source) != null)
                    {
                        Result result = reader.Decode(source);
                        url = result.Text;
                        Uri uri = new(url);
                        if (uri.GetLeftPart(UriPartial.Path) == appSettings.UrlSearch)
                        {
                            TempData["CHECKUSerInfo"] = "CHECK";
                            return Redirect(url);
                        }
                        else
                        {
                            notyf.Warning("Error, Check Your QR");
                            return RedirectToAction("Index", "Search");
                        }
                    }
                    else
                    {
                        notyf.Warning("Error, Check Your QR");
                        return RedirectToAction("Index", "Search");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Search");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EmptyYourCounter()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string uId = HttpContext.Session.GetString("UserId");
                var wwwroot = env.WebRootPath;
                string[] filePaths = Directory.GetFiles(Path.Combine(wwwroot, "NumberSearch/"), $"{uId}*");
                for (int i = 0; i < filePaths.Length; i++)
                {
                    var exists = System.IO.File.Exists(filePaths[i]);
                    if (exists)
                    {
                        System.IO.File.Delete(filePaths[i]);
                    }
                }
                notyf.Information("Now, No Viwers for your account");
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public IActionResult DownloadQR()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string uId = HttpContext.Session.GetString("UserId");
                var wwwroot = env.WebRootPath;
                var imgQR = Path.Combine(wwwroot, $"QrCodes/{uId}.png");
                var exists = System.IO.File.Exists(imgQR);
                if (exists)
                {
                    var filename = string.Join(":", new string[] { DateTime.Now.Ticks.ToString(), "WepAppFeaturesQRCode.png" });
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = filename,
                        Inline = false
                    };
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    return PhysicalFile(imgQR, MimeTypes.GetMimeType(imgQR), filename);
                }
                else
                {
                    return RedirectToAction("Profile", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadImage(IFormFile fileUpload)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string uId = HttpContext.Session.GetString("UserId");
                if (fileUpload != null && fileUpload.Length > 0)
                {
                    var wwwroot = env.WebRootPath;
                    var extension = Path.GetExtension(fileUpload.FileName);
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                    {
                        var fileName = uId + DateTime.Now.ToString("yymmssfff") + extension;
                        var filePath = Path.Combine(wwwroot, "images\\UpdatedUserImage", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            fileUpload.CopyTo(fileStream);
                        }
                        var img = System.IO.File.OpenRead(filePath);
                        var dataImage = new MemoryStream();
                        img.CopyTo(dataImage);
                        bool user = userRepository.UpdateImageUser(uId, dataImage.ToArray());
                        if (user)
                        {
                            notyf.Success("Image Updated Successfully");
                        }
                        else
                        {
                            notyf.Error("Image has not been updated");
                        }
                    }
                    else
                    {
                        notyf.Error("Image has not been updated");
                    }
                    return RedirectToAction("Profile", "Dashboard");
                }
                else if (fileUpload == null)
                {
                    return RedirectToAction("Profile", "Dashboard");
                }
                else
                {
                    return RedirectToAction("Profile", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(EditUserViewModel model)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                if (ModelState.IsValid)
                {
                    string uId = HttpContext.Session.GetString("UserId");
                    User user = userRepository.GetUserbyUserId(uId);
                    if(user != null)
                    {
                        if (model.Gender != user.Gender&& user.Gender != Lists.Gender.None.ToString())
                        {
                            if (userRepository.UpdateGenderUser(uId, model.Gender))
                            {
                                notyf.Success("Gender Updated successfully");                            
                            }                 

                        }
                        if (model.City != user.City)
                        {
                            if (userRepository.UpdateCityUser(uId, model.City))
                            {
                                notyf.Success("City Updated successfully");
                            }
                        }
                        if (model.Country != user.Country)
                        {
                            if(model.Country == "Egypt")
                            {
                                model.Country = "EG";
                            }
                            if (userRepository.UpdateCountryUser(uId, model.Country))
                            {
                                notyf.Success("Country Updated successfully");
                            }
                        }
                        if (model.FullName != user.FullName)
                        {
                            if (userRepository.UpdateNameUser(uId, model.FullName))
                            {
                                notyf.Success("Your Name Updated successfully");
                            }
                        }                
                        if (model.UserName.Trim() != user.UserName)
                        {
                            if (model.UserName.Contains('@'))
                            {
                                notyf.Information("User Name Should Not be contain @");
                            }
                            else
                            {
                                bool checkUserEmail = userRepository.CheckUserNameEmailExists(model.UserName.Trim());
                                bool checkUser = userRepository.CheckUserNameExists(model.UserName.Trim());
                                string usernameMyEmail = new MailAddress(user.Email).User;
                                if(checkUser == false)
                                {
                                    if (checkUserEmail == false)
                                    {
                                        userRepository.UpdateUserNameUser(uId, model.UserName.Trim());
                                        notyf.Success("Username Updated successfully");
                                    }
                                    else if (checkUserEmail == true && usernameMyEmail == model.UserName.Trim())
                                    {
                                        userRepository.UpdateUserNameUser(uId, model.UserName.Trim());
                                        notyf.Success("Hello Again, Username Updated successfully");
                                    }
                                    else if(checkUserEmail == true && usernameMyEmail != model.UserName.Trim())
                                    {
                                        notyf.Information("User Name is Exists");
                                    }
                                }
                                else
                                {
                                    notyf.Information("User Name is Exists");
                                }
                            }
                        }
                    }
                    else
                    {

                       notyf.Error("Error in Update Profile");
                    }
                }
                return RedirectToAction("Profile", "Dashboard");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                if (ModelState.IsValid)
                {
                    string userid = Convert.ToString(HttpContext.Session.GetString("UserId"));
                    var getuserdetails = userRepository.GetUserbyUserId(userid);
                    var usersalt = userRepository.GetUserSaltbyUserId(getuserdetails.UserId);
                    var generatehash = GenerateHashSha512.Sha512(model.OldPassword, usersalt.PasswordSalt);
                    if (model.OldPassword == model.NewPassword)
                    {
                        notyf.Warning("New Password Cannot be same as Old Password");
                        return View();
                    }
                    if (!string.Equals(getuserdetails.PasswordHash, generatehash, StringComparison.Ordinal))
                    {
                        notyf.Warning("Current Password Entered is InValid");
                        return View();
                    }
                    if (!string.Equals(model.NewPassword, model.NewPasswordAgain, StringComparison.Ordinal))
                    {
                        notyf.Warning("Password Does not Match");
                        return View();
                    }
                    else
                    {
                        var salt = GenerateRandomNumbers.RandomNumbers(20);
                        var saltedpassword = GenerateHashSha512.Sha512(model.NewPassword, salt);
                        bool result = userRepository.UpdatePasswordandHistory(getuserdetails.UserId, saltedpassword, salt);
                        if (result)
                        {
                            notyf.Success("Password Changed Successfully");
                        }
                        else
                        {
                            notyf.Error("Something Went Wrong Please try again!");
                        }
                        return View();
                    }
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProfile()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userid = Convert.ToString(HttpContext.Session.GetString("UserId"));
                bool result = userRepository.DeleteMyProfile(userid);
                if (result)
                {
                    HttpContext.Session.Clear();
                    Program.CounterLogin = 0;
                    TempData["Registered_Success_Message"] = "Sorry, your account has been successfully deleted";
                    return RedirectToAction("Register", "Account");
                }
                else
                {
                    notyf.Error("Error in delete Your Account");
                    return View("Profile","Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
