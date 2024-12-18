﻿using Domain._DTO.Event;
using Domain._DTO.Ticket;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Security;
using Services.Categories;
using Services.Events;
using Services.Tickets;
using Services.UserAccount;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    [Authorize(Policy ="ManageEvents")]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly IUserAccountService _userAccountService;
        private readonly ITicketTypeService _ticketTypesService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<EventController> _localizer;
        private readonly ILogger<EventController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EventController(IEventService eventService,
            ICategoryService categoryService,
            IUserAccountService userAccountService,
            ITicketTypeService ticketTypesService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<EventController> localizer,
            ILogger<EventController> logger,
            IWebHostEnvironment hostEnvironment)
        {
            _eventService = eventService;
            _categoryService = categoryService;
            _userAccountService = userAccountService;
            _ticketTypesService = ticketTypesService;
            _protector = provider.CreateProtector(purpose: purposeStrings.EventControllerPs);
            _localizer = localizer;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }


        [Route("EventData")]
        [Authorize(Policy ="ViewAllEvents")]
        public ActionResult Index()
        {
            return View();
        }

        //GET Method
        [Route("Event/Create")]
        [Authorize(Policy ="CreateEvent")]
        public ActionResult Create()
        {
            try
            {
                if (User.Identity.IsAuthenticated && User.IsInRole("Admin") || User.IsInRole("EventCreator"))
                {
                    var result = EventCreateDtoEncryption();

                    return View(result);
                }

                return RedirectToAction("VerifyPhoneNumber", "Verification");
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
        [Authorize(Policy ="CreateEvent")]
        public ActionResult Create(EventCreateDto eventCreateDto, IFormFile file, string ticketData)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (User.Identity.IsAuthenticated && User.IsInRole("Admin") || User.IsInRole("EventCreator"))
                {
                    eventCreateDto.UserAccountId = claim.Value != null ? int.Parse(claim.Value) : 0;
                    eventCreateDto.CategoryId = eventCreateDto.EncryptedCategoryId != null ? int.Parse(_protector.Unprotect(eventCreateDto.EncryptedCategoryId)) : 0;

                    if (string.IsNullOrEmpty(ticketData))
                    {
                        eventCreateDto.TicketTypes = new List<TicketTypeDto>();
                    }
                    else
                    {
                        // Converts the JSON string containing ticket data into a list of TicketTypeDto objects.
                        var ticketTypes = eventCreateDto.TicketTypes = JsonConvert.DeserializeObject<List<TicketTypeDto>>(ticketData);

                        foreach (var ticket in ticketTypes)
                        {
                            if (string.IsNullOrEmpty(ticket.Name) || ticket.Price < 0 || ticket.Quantity < 0)
                            {
                                TempData["message"] = "Error";
                                TempData["entity"] = "Invalid ticket data.";

                                return RedirectToAction(nameof(Index));
                            }
                        }

                        eventCreateDto.TicketTypes = ticketTypes;
                    }

                    if (file != null && file.Length > 0)
                    {
                        //retrieves the root path of the web application www.root
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        //generates a unique filename using Guid.NewGuid()
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(file.FileName).ToLower();

                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                        {
                            //path where the file will be saved by combining the web root path with the "images\events" folder.
                            string uploads = Path.Combine(wwwRootPath, @"images\events");
                            string filePath = Path.Combine(uploads, fileName + extension);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }

                            eventCreateDto.Image = @"\images\events\" + fileName + extension;
                        }
                        else
                        {
                            TempData["message"] = "Error";
                            TempData["entity"] = "Invalid image format. Please upload a valid image format: .jpg,.jpeg,.png";

                            return RedirectToAction(nameof(Index));
                        }

                    }

                    var result = _eventService.Create(eventCreateDto);

                    TempData["message"] = "Added";
                    TempData["entity"] = _localizer["Event"].ToString();

                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction("VerifyPhoneNumber", "Verification");

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
        [Authorize(Policy ="UpdateEvent")]
        public async Task<ActionResult> Details(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = await _eventService.GetByIdWithCategory(id);

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
        [Authorize(Policy ="UpdateEvent")]
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
        [Authorize(Policy ="UpdateEvent")]
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
        [HttpGet]
        [Route("Event/ViewTickets")]
        [Authorize(Policy ="ViewAllTickets")]
        public ActionResult ViewTickets(string encryptedId)
        {
            ViewBag.EncryptedId = encryptedId;
            return View();
        }
        [HttpGet]
        [Route("Event/AddTicket")]
        [Authorize(Policy ="CreateTickets")]
        public ActionResult AddTicket([FromQuery] string encryptedEventId, [FromQuery] string option)
        {
            ViewBag.EncryptedEventId = encryptedEventId;
            ViewBag.Option = option;
            return View();
        }
        [HttpPost]
        [Authorize(Policy ="CreateTickets")]
        public async Task<IActionResult> AddTicket([FromBody]TicketTypeDto formData)
        {
            try
            {
                var eventId = int.Parse(_protector.Unprotect(formData.EncryptedEventId));
                formData.EventId = eventId;

                var result = await _eventService.GetById(eventId);
                var ticketStartDate = formData.SaleStartDate;
                var ticketEndDate = formData.SaleEndDate;

                if(ticketStartDate > result.EndDate || ticketEndDate > result.EndDate || ticketStartDate > ticketEndDate)
                {
                    return BadRequest("Ticket dates must be within the event's timeframe.");
                }
                if (!formData.IsFree)
                {
                    if (formData.Price <= 0)
                    {
                        ModelState.AddModelError("Price", "Price must be a positive number.");
                        return BadRequest(ModelState);
                    }
                }
                if (formData.Quantity <= 0)
                {
                    ModelState.AddModelError("Quantity", "Quantity must be a positive number.");
                    return BadRequest(ModelState);
                }

                _ticketTypesService.AddTicket(formData);

                return Ok(new { success = true, Message = "Ticket added succesfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the ticket: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Ticket/Edit")]
        public ActionResult EditTicket()
        {
            return View();
        }
        [HttpPost]
        [Route("Event/UpdateEventStatus")]
        [Authorize(Policy ="UpdateEvent")]
        public async Task<IActionResult> UpdateEventStatus(string encryptedId)
        {
            try
            {
                var eventId = int.Parse(_protector.Unprotect(encryptedId));
                var eventToUpdate = await _eventService.GetByIdEditForEditStatus(eventId);

                if (eventToUpdate != null)
                {
                    if (eventToUpdate.Status == "Draft")
                    {

                        eventToUpdate.Status = "Published";
                        await _eventService.UpdateAsync(eventToUpdate);

                        return Json(new { success = true, message = "Event published successfully." });
                    }
                    else if (eventToUpdate.Status == "Published")
                    {
                        eventToUpdate.Status = "Draft";
                         await _eventService.UpdateAsync(eventToUpdate);

                        return Json(new { success = true, message = "Event unpublished successfully." });
                    }
                }

                return Json(new { success = false, message = "Event not found or invalid status." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json(new { success = false, message = "Error occurred while updating the event status." });
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
                }).ToList(),

                TicketTypes = new List<TicketTypeDto>()

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
        [Route("Event/GetTickets")]
        public async Task<ActionResult> GetTickets(string encryptedId)
        {
            try
            {
                var eventId = int.Parse(_protector.Unprotect(encryptedId));
                var tickets = await _ticketTypesService.GetTicketsByEventId(eventId);
                foreach (var ticket in tickets)
                {
                    ticket.EncryptedId = _protector.Protect(ticket.Id.ToString());
                    ticket.EncryptedEventId = _protector.Protect(ticket.EventId.ToString());
                }
                return Json(new { data = tickets });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return Json(new { data = new List<TicketTypeDto>() });
            }
        }


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
                    var result = await _eventService.GetAllEventsWithSoldAndGross();

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
                    var result = await _eventService.GetActiveEventsWithSoldAndGrossForEventCreator(userId);

                    foreach (var item in result)
                    {
                        item.EncryptedId = _protector.Protect(item.Id.ToString());
                        item.EncryptedCategoryId = _protector.Protect(item.CategoryId.ToString());
                        item.EncryptedUserAccountId = _protector.Protect(item.UserAccountId.ToString());
                        item.Id = 0;
                        item.CategoryId = 0;
                        item.Category.Id = 0;
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
        [Authorize(Policy ="DeleteEvent")]
        public ActionResult Delete(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId.ToString()));

                var obj = _eventService.GetByIdWithCategory(id);

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = int.Parse(claim.Value);
                var isAdmin = User.IsInRole("Admin");

                // Check if the event has expired
                if (obj != null && obj.Result.EndDate < DateTime.Now)
                {
                    if (isAdmin)
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
                        obj.Result.IsActive = false;
                        var updateEvent = _eventService.UpdateByIsActive(obj.Result);
                        return Json(new { success = true, message = "Event Deleted Successfully" });
                    }
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

