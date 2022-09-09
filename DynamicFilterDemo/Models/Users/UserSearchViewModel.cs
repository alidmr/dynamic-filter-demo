﻿using DynamicFilterDemo.Models.Response;

namespace DynamicFilterDemo.Models.Users
{
    public class UserSearchViewModel : BaseDataTableSearchModel
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
