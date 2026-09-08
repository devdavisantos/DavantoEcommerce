using Api.Controllers.Responses;
using Api.Shared.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Extensions
{
    public static class ErrorHttpExtensions
    {
        public static IActionResult ToActionResult(
            this ControllerBase controller,
            Error error)
        {
            var response = new ApiErrorResponse(
                error.Code,
                error.Message);

            return error.Code switch
            {
                // Authentication
                "Customer.Login.InvalidCredentials"
                    => controller.Unauthorized(response),

                "Customer.Login.AccountInactive"
                    => controller.Unauthorized(response),

                "Customer.Login.AccountBlocked"
                    => controller.Unauthorized(response),

                // Refresh Token
                "Customer.RefreshToken.InvalidToken"
                    => controller.Unauthorized(response),

                "Customer.RefreshToken.ExpiredToken"
                    => controller.Unauthorized(response),

                "Customer.RefreshToken.RevokedToken"
                    => controller.Unauthorized(response),

                "Customer.RefreshToken.ReplacedToken"
                    => controller.Unauthorized(response),

                "Customer.RefreshToken.InactiveLogin"
                    => controller.Unauthorized(response),

                "Customer.RefreshToken.UserInactive"
                    => controller.Unauthorized(response),

                "Customer.RefreshToken.UserBlocked"
                    => controller.Unauthorized(response),

                // Register
                "Customer.Register.EmailAlreadyExists"
                    => controller.Conflict(response),

                // Profile
                "Customer.Profile.UserNotFound"
                    => controller.NotFound(response),

                "Customer.Profile.UserInactive"
                    => controller.NotFound(response),

                "Customer.User.DocumentCannotBeRemoved"
                    => controller.BadRequest(response),

                "Customer.User.PhoneCannotBeRemoved"
                    => controller.BadRequest(response),

                "Customer.User.InvalidDocument"
                    => controller.BadRequest(response),

                "Customer.User.DocumentAlreadyExists"
                    => controller.Conflict(response),

                "Customer.User.InvalidPhone"
                    => controller.BadRequest(response),

                "Customer.User.PhoneMustContainOnlyNumbers"
                    => controller.BadRequest(response),

                // Change Password
                "Customer.ChangePassword.InvalidCurrentPassword"
                    => controller.BadRequest(response),

                "Customer.ChangePassword.SamePassword"
                    => controller.BadRequest(response),

                "Customer.ChangePassword.UserNotFound"
                    => controller.NotFound(response),

                "Customer.ChangePassword.LoginNotFound"
                    => controller.Unauthorized(response),

                "Customer.ChangePassword.UserInactive"
                    => controller.Unauthorized(response),

                "Customer.PasswordReset.Validation"
                    => controller.BadRequest(response),

                "Customer.ChangePassword.Validation"
                    => controller.BadRequest(response),

                // Change Email
                "Customer.EmailChange.UserNotFound"
                    => controller.NotFound(response),

                "Customer.EmailChange.UserInactive"
                    => controller.Unauthorized(response),

                "Customer.EmailChange.EmailAlreadyExists"
                    => controller.Conflict(response),

                "Customer.EmailChange.InvalidToken"
                    => controller.BadRequest(response),

                "Customer.EmailChange.ExpiredToken"
                    => controller.BadRequest(response),

                "Customer.EmailChange.RevokedToken"
                    => controller.BadRequest(response),

                "Customer.EmailChange.AlreadyConfirmed"
                    => controller.BadRequest(response),

                "Customer.EmailChange.InvalidMetadata"
                    => controller.BadRequest(response),

                "Customer.PasswordReset.InvalidEmail"
                    => controller.BadRequest(response),

                "Customer.PasswordReset.EmailNotFound"
                    => controller.NotFound(response),

                "Customer.PasswordReset.InvalidToken"
                    => controller.BadRequest(response),

                "Customer.PasswordReset.ExpiredToken"
                    => controller.BadRequest(response),

                "Customer.PasswordReset.RevokedToken"
                    => controller.BadRequest(response),

                "Customer.PasswordReset.AlreadyCompleted"
                    => controller.BadRequest(response),

                "Customer.PasswordReset.SamePassword"
                    => controller.BadRequest(response),

                "Customer.PasswordReset.UserNotFound"
                    => controller.NotFound(response),

                "Customer.PasswordReset.UserInactive"
                    => controller.Unauthorized(response),

                "Customer.PasswordReset.LoginNotFound"
                    => controller.Unauthorized(response),

                // Default
                _ => controller.BadRequest(response)
            };
        }
    }
}
