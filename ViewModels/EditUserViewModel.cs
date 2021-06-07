using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Plan_io_T.ViewModels {
    public class EditUserViewModel {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ReqText { get; set; }
    }
}
