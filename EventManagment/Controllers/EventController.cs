using Domain._DTO.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Security;
using Services.Categories;
using Services.Events;
using Services.UserAccount;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly IUserAccountService _userAccountService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<EventController> _localizer;
        private readonly ILogger<EventController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EventController(IEventService eventService,
            ICategoryService categoryService,
            IUserAccountService userAccountService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<EventController> localizer,
            ILogger<EventController> logger,
            IWebHostEnvironment hostEnvironment)
        {
            _eventService = eventService;
            _categoryService = categoryService;
            _userAccountService = userAccountService;
            _protector = provider.CreateProtector(purpose: purposeStrings.EventControllerPs);
            _localizer = localizer;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }


        [Route("EventData")]
        public ActionResult Index()
        {
            return View();
        }

        //GET Method
        [Route("Event/Create")]
        public ActionResult Create()
        {
            try
            {
                var result = EventCreateDtoEncryption();

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();


                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [Route("Event/Create")]
        public ActionResult Create(EventCreateDto eventCreateDto, IFormFile file)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                eventCreateDto.UserAccountId = claim.Value != null ? int.Parse(claim.Value) : 0;
                eventCreateDto.CategoryId = eventCreateDto.EncryptedCategoryId != null ? int.Parse(_protector.Unprotect(eventCreateDto.EncryptedCategoryId)) : 0;

                if (file != null && file.Length > 0)
                {
                    //retrieves the root path of the web application www.root
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    //generates a unique filename using Guid.NewGuid()
                    string fileName = Guid.NewGuid().ToString();
                    //path where the file will be saved by combining the web root path with the "images\events" folder.
                    string uploads = Path.Combine(wwwRootPath, @"images\events");
                    string extension = Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploads, fileName + extension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    eventCreateDto.Image = @"\images\events\" + fileName + extension;

                }

                var result = _eventService.Create(eventCreateDto);

                TempData["message"] = "Added";
                TempData["entity"] = _localizer["Event"].ToString();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        //GET Method
        [Route("EventDetails")]
        public async Task<ActionResult> Details(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = await _eventService.GetById(id);

                result.EncryptedId = _protector.Protect(id.ToString());
                result.Id = 0;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Route("Event/Edit")]
        public async Task<ActionResult> Edit(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = await _eventService.GetByIdEdit(id);

                EventEditDtoEncryption(result);

                return View(result);

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Event/Edit")]
        public ActionResult Edit(EventEditDto eventEditDto, IFormFile file)
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                eventEditDto.UserAccountId = claim.Value != null ? int.Parse(claim.Value) : 0;
                eventEditDto.CategoryId = eventEditDto.EncryptedCategoryId != null ? int.Parse(_protector.Unprotect(eventEditDto.EncryptedCategoryId)) : 0;
                eventEditDto.Id = int.Parse(_protector.Unprotect(eventEditDto.EncryptedId));

                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null && file.Length > 0)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, eventEditDto.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file.FileName);

                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, @"images\events");

                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    string filePath = Path.Combine(uploads, fileName + extension);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    eventEditDto.Image = @"\images\events\" + fileName + extension;
                }

                var result = _eventService.Update(eventEditDto);

                TempData["message"] = "Updated";
                TempData["entity"] = _localizer["Event "].ToString();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }


        private EventCreateDto EventCreateDtoEncryption()
        {
            return new EventCreateDto()
            {
                Category = _categoryService.GetAll().Result.Select(x =>
                {
                    x.EncryptedId = _protector.Protect(x.Id.ToString());
                    x.Id = 0;
                    return x;
                }).ToList()

            };
        }

        private EventEditDto EventEditDtoEncryption(EventEditDto eventEditDto)
        {
            eventEditDto.Category = _categoryService.GetAll().Result.Select(x =>
            {
                x.EncryptedId = _protector.Protect(x.Id.ToString());
                if (eventEditDto.CategoryId == x.Id) eventEditDto.EncryptedCategoryId = x.EncryptedId;
                x.Id = 0;

                return x;
            }).ToList();
            eventEditDto.EncryptedId = _protector.Protect(eventEditDto.Id.ToString());
            eventEditDto.Id = 0;

            return eventEditDto;
        }


        #region API CALLS
        [HttpGet]
        [Route("GetAllEvents")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = int.Parse(claim.Value);
                var isAdmin = User.IsInRole("Admin");

                if (isAdmin)
                {
                    var result = await _eventService.GetAllEvents();

                    foreach (var item in result)
                    {
                        item.EncryptedId = _protector.Protect(item.Id.ToString());
                        item.EncryptedCategoryId = _protector.Protect(item.CategoryId.ToString());
                        item.EncryptedUserAccountId = _protector.Protect(item.UserAccountId.ToString());
                        item.Id = 0;
                        item.CategoryId = 0;
                        item.Category.Id = 0;
                        item.UserAccount.Id = 0;
                        item.UserAccountId = 0;
                    }

                    return Json(new { data = result });
                }
                else
                {
                    var result = await _eventService.GetUserEvents(userId);

                    foreach (var item in result)
                    {
                        item.EncryptedId = _protector.Protect(item.Id.ToString());
                        item.EncryptedCategoryId = _protector.Protect(item.CategoryId.ToString());
                        item.EncryptedUserAccountId = _protector.Protect(item.UserAccountId.ToString());
                        item.Id = 0;
                        item.CategoryId = 0;
                        item.Category.Id = 0;
                        item.UserAccount.Id = 0;
                        item.UserAccountId = 0;
                    }

                    return Json(new { data = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "Invalid value.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpDelete]
        [Route("Event/Delete")]
        public ActionResult Delete(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId.ToString()));

                var obj = _eventService.GetById(id);

                // Check if the event has expired
                if (obj != null && obj.Result.EndDate < DateTime.Now)
                {

                    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.Result.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    var result = _eventService.Delete(id);

                    return Json(new { success = true, message = "Event Deleted Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Event has not expired yet" });
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }

        }
        #endregion


    }

}

