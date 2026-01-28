using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.Common.Constants
{
    public static class RespMsg
    {
        // Success
        public const string OK = "OK";
        public const string CREATED = "Created successfully";
        public const string UPDATED = "Updated successfully";
        public const string DELETED = "Deleted successfully";
        public const string SUCCESS = "Success";

        // Client errors
        public const string BAD_REQUEST = "Bad request";
        public const string VALIDATION_FAILED = "Validation failed";
        public const string UNAUTHORIZED = "Unauthorized";
        public const string FORBIDDEN = "Forbidden";
        public const string NOT_FOUND = "Resource not found";
        public const string CONFLICT = "Conflict";

        // Server errors
        public const string INTERNAL_SERVER_ERROR = "Internal server error";
        public const string SERVICE_UNAVAILABLE = "Service temporarily unavailable";
    }
}
