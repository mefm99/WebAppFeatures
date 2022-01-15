using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using System.Web;
using WebAppFeatures.Models;
using WebAppFeatures.Models.DataModel;
using WebAppFeatures.ServicesRepository;
using WebAppFeatures.Helpers;
using ZXing;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace WebAppFeatures.Controllers
{
    public class SearchController : Controller
    {
        private readonly INotyfService notyf;
        private readonly IUserRepository userRepository;
        private readonly IWebHostEnvironment env;
        private readonly AppSettings appSettings;
        public SearchController(IOptions<AppSettings> _appSettings,
            INotyfService _notyf,IWebHostEnvironment _env,IUserRepository _userRepository)
        {
            notyf = _notyf;
            appSettings = _appSettings.Value;
            env = _env;
            userRepository = _userRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [SupportedOSPlatform("windows")]
        public IActionResult UserInfo(string Key)
        {
            if (TempData["CHECKUSerInfo"] != null)
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
                                string uId = user.UserId;
                                var wwwroot = env.WebRootPath;
                                var imgQR = Path.Combine(wwwroot, $"QrCodes/{uId}.png");
                                var NumSearchUser = Path.Combine(wwwroot, "NumberSearch", $"{uId}{DateTime.Now:yymmssfff}.a");
                                var exists = System.IO.File.Exists(imgQR);
                                if (exists)
                                {
                                    var reader = new BarcodeReaderGeneric();
                                    Bitmap image = (Bitmap)Image.FromFile(imgQR);
                                    using (image)
                                    {
                                        LuminanceSource source;
                                        source = new ZXing.Windows.Compatibility.BitmapLuminanceSource(image);
                                        if (source != null && reader.Decode(source) != null)
                                        {
                                            Result result = reader.Decode(source);
                                            string urlOriginal = result.Text;
                                            var KeyUrl = appSettings.UrlSearch +
                                                "?Key=" + HttpUtility.UrlEncode(Key);
                                            if (KeyUrl == urlOriginal)
                                            {
                                                System.IO.File.Create(NumSearchUser).Close();
                                                UserInfoSearch model = new()
                                                {
                                                    Key = Key,
                                                    FullName = user.FullName,
                                                    City = user.City,
                                                    Gender = user.Gender,
                                                    Country = user.Country,
                                                    CreatedDate = user.CreatedDate,
                                                    Email = user.Email,
                                                    Image = user.Image,
                                                    PhoneKey = user.PhoneKey,
                                                    PhoneNumber = user.PhoneNumber,
                                                    UserName = user.UserName
                                                };
                                                return View(model);
                                            }
                                            else
                                            {
                                                notyf.Warning("QR has expired");
                                                return RedirectToAction("Index", "Search");
                                            }
                                        }
                                        else
                                        {
                                            notyf.Warning("Make sure QR is valid");
                                            return RedirectToAction("Index", "Search");
                                        }
                                    } 
                                }
                                else
                                {
                                    notyf.Information("QR for this User is deleting");
                                    return RedirectToAction("Index", "Search");
                                }

                            }
                            else
                            {
                                notyf.Information("Not Found, Check Your QR");
                                return RedirectToAction("Index", "Search");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Search");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Search");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Search");
                }
            }
            else
            {
                return RedirectToAction("Index", "Search");
            }
        }
    }
}
