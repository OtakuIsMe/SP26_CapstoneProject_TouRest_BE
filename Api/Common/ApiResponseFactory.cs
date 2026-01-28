using Microsoft.AspNetCore.Mvc;
using TouRest.Application.Common.Constants;

namespace TouRest.Api.Common
{
    public static class ApiResponseFactory
    {
        // 200 OK
        public static IActionResult Ok<T>(T data, string? message = null)
        {
            return new JsonResult(new ApiResponse<T>
            {
                Code = RespCode.OK,
                Message = message ?? RespMsg.OK,
                Data = data
            })
            {
                StatusCode = RespCode.OK
            };
        }

        // 201 Created
        public static IActionResult Created<T>(T data, string? message = null)
        {
            return new JsonResult(new ApiResponse<T>
            {
                Code = RespCode.CREATED,
                Message = message ?? RespMsg.CREATED,
                Data = data
            })
            {
                StatusCode = RespCode.CREATED
            };
        }

        // 204 No Content
        public static IActionResult NoContent(string? message = null)
        {
            return new JsonResult(new ApiResponse<object?>
            {
                Code = RespCode.NO_CONTENT,
                Message = message ?? RespMsg.SUCCESS,
                Data = null
            })
            {
                StatusCode = RespCode.NO_CONTENT
            };
        }

        // 302 Redirect (ít dùng, nhưng cho đủ bộ)
        public static IActionResult Redirect(string url)
        {
            return new RedirectResult(url, false);
        }
    }
}
