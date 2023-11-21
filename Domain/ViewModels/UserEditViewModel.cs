using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class UserEditViewModel
    {
        public UserAccount User { get; set; }
        public List<string> Roles { get; set; }
    }
}
