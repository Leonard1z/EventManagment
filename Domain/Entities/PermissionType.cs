using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum PermissionType
    {
        AccessDashboard,//-----|
        AccessCategories,//    |
        AccessEvents,//        |>These permissions allows users to access specific controller
        AccessRoles,//---------|

        ViewAllCategories,//-------------
        CreateCategory,//               |
        EditCategory,//                 |
        DeleteCategory,//               |
        ViewAllEvents,//                |
        ViewEvent,//                    |
        CreateEvent,//                  |
        UpdateEvent,//                  |
        DeleteEvent,//                  |
        ApproveEvent,//                 |
        ViewAllTickets,//               |> Specific action-level permissions within controllers
        CreateTicket,//                 |
        UpdateTicket,//                 |
        DeleteTicket,//                 |
        ViewAllUsers,//                 |
        CreateUser,//                   |
        UpdateUser,//                   |
        DeleteUser,//                   |
        ViewRoles,//                    |
        CreateRole,//                   |
        EditRole,//                     |
        DeleteRole,//--------------------

        ManageUserRoles,// > Special permission for managing user roles within the application

        ViewDashboardData,
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
