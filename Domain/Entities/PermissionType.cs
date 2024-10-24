using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum PermissionType
    {
        CreateEvent,
        UpdateEvent,
        DeleteEvent,
        ViewAllEvents,
        CreateTicket,
        UpdateTicket,
        DeleteTicket,
        ViewAllTickets,
        CreateUser,
        UpdateUser,
        DeleteUser,
        ViewAllUsers,
        ManageUserRoles,
        ManageRoles,
        ManagePermissions,
        AccessReports,
        EditSettings
    }
}
