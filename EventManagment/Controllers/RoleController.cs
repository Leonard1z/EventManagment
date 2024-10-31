using Domain._DTO.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Security;
using Services.Role;

namespace EventManagment.Controllers
{
    [Authorize(Policy = "AccessRoles")]
    public class RoleController : Controller
    {

        private readonly IRoleService _roleService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<RoleController> _localizer;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService,
                IDataProtectionProvider provider,
                DataProtectionPurposeStrings purposeStrings,
                IStringLocalizer<RoleController> localizer,
                ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _protector = provider.CreateProtector(purpose: purposeStrings.RoleControllerPs);
            _localizer = localizer;
            _logger = logger;
        }

        [Route("RoleData")]
        [Authorize(Policy = "ViewRoles")]
        public async Task<ActionResult> Index()
        {
            try
            {
                var role = await _roleService.GetAll();

                foreach (var item in role)
                {
                    item.EncryptedId = _protector.Protect(item.Id.ToString());
                    item.Id = 0;
                }

                return View(role);

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString(); ;

                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Route("Role/Create")]
        [Authorize(Policy = "CreateRole")]
        public ActionResult Create()
        {

            return View();

        }

        [HttpPost]
        [Route("Role/Create")]
        [Authorize(Policy = "CreateRole")]
        public ActionResult Create(RoleDto roleDto)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _roleService.Create(roleDto);
                    TempData["message"] = "Added";
                    TempData["entity"] = _localizer["Role "].ToString();

                    return RedirectToAction(nameof(Index));
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View(roleDto);
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString(); ;

                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [Route("Role/Edit")]
        [Authorize(Policy ="EditRole")]
        public async Task<ActionResult> Edit(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = await _roleService.GetById(id);

                result.EncryptedId = _protector.Protect(result.Id.ToString());
                result.Id = 0;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString(); ;

                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Index));

            }
        }

        [HttpPost]
        [Route("Role/Edit")]
        [Authorize(Policy = "EditRole")]
        public ActionResult Edit(RoleDto roleDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    roleDto.Id = int.Parse(_protector.Unprotect(roleDto.EncryptedId));
                    roleDto.EncryptedId = "";

                    var result = _roleService.Update(roleDto);
                    TempData["message"] = "Updated";
                    TempData["entity"] = _localizer["Role "].ToString();

                    return RedirectToAction(nameof(Index));
                }
                return View(roleDto);
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error ocurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [Route("Role/Delete")]
        [Authorize(Policy ="DeleteRole")]
        public ActionResult Delete(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = _roleService.Delete(id);

                TempData["message"] = "Deleted";
                TempData["entity"] = _localizer["Role "].ToString();

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
    }
}