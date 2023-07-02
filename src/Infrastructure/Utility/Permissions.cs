using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utility
{
    public static class Permissions
    {

        public static List<string> GeneratePermissionsList(string model)
        {
            return new List<string>()
            {
                $"Permissions.{model}.View",
                $"Permissions.{model}.Edit",
                $"Permissions.{model}.Create",
                $"Permissions.{model}.Delete",
                $"Permissions.{model}.Index"
            };
        }

        public static List<string> GenerateAllAvailablePermissions()
        {
            List<string> permissions = new List<string>();

            foreach(string name in Enum.GetNames(typeof(Entities)))
            {
                permissions.AddRange(GeneratePermissionsList(name));
            }

            return permissions;
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Index = "Permissions.Users.Index";

        }

        public static class ResourceTypes
        {
            public const string View = "Permissions.ResourceTypes.View";
            public const string Create = "Permissions.ResourceTypes.Create";
            public const string Edit = "Permissions.ResourceTypes.Edit";
            public const string Delete = "Permissions.ResourceTypes.Delete";
            public const string Index = "Permissions.ResourceTypes.Index";

        }
    }
}
