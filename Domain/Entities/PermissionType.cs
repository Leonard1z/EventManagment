using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum PermissionType
    {
        ManageCategories,
        ViewCategories,
        CreateCategory,
        EditCategory,
        DeleteCategory,
        ViewAllEvents,
        ManageEvents,
        CreateEvent,
        UpdateEvent,
        DeleteEvent,
        ApproveEvent,
        ViewAllTickets,
        CreateTicket,
        UpdateTicket,
        DeleteTicket,
        CreateUser,
        UpdateUser,
        DeleteUser,
        ViewAllUsers,
        ManageUserRoles,
        ManageRoles,
        ViewRoles,
        CreateRole,
        EditRole,
        DeleteRole,
        ManagePermissions,
        AccessReports,
        ViewReports,
        GenerateReports,
        CreateNotification,
        UpdateNotification,
        DeleteNotification,
        ViewAllNotifications,
        ManageNotifications,
        ConfigureSystem,
        EditSettings
    }
}
