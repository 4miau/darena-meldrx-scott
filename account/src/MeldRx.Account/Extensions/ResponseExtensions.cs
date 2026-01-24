using MeldRx.Account.Helpers;
using MeldRx.Account.Services;
using MeldRx.Sdk.ApiClientModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MeldRx.Account.Extensions
{
    public static class ResponseExtensions
    {

        public static bool ProcessIdentityResult(this IdentityResult result, ITempDataDictionary tempData, NotificationService notificationService, string? successMessage = null, string? errorMessage = null, string? title = null)
        {
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(successMessage))
                {
                    notificationService.SuccessNotification(tempData, successMessage, title);
                }
            }
            else
            {
                //Note: To Do
                //notificationService.CreateNotification(tempData, NotificationHelpers.AlertType.Error, result.Errors, title);
            }
            return result.Succeeded;
        }
        public static bool ProcessOperationResult(this OperationResult result, ITempDataDictionary tempData, NotificationService notificationService, string? successMessage = null, string? errorMessage = null, string? title = null)
        {
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(successMessage))
                {
                    notificationService.SuccessNotification(tempData, successMessage, title);
                }
            }
            else
            {
                notificationService.CreateNotification(tempData, NotificationHelpers.AlertType.Error, result.ErrorMessage, title);
            }
            return result.Success;
        } 
        
        public static bool ProcessOperationResult<T>(this OperationResult<T> result, ITempDataDictionary tempData, NotificationService notificationService, string? successMessage = null, string? errorMessage = null, string? title = null)
        {
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(successMessage))
                {
                    notificationService.SuccessNotification(tempData, successMessage, title);
                }
            }
            else
            {
                notificationService.CreateNotification(tempData, NotificationHelpers.AlertType.Error, result.ErrorMessage, title);
            }
            return result.Success;
        }

        public static bool ProcessHttpResponse<T>(this HttpResponseModel<T> result, ITempDataDictionary tempData, NotificationService notificationService, string? successMessage = null, string? errorMessage = null, string? title = null)
        {
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(successMessage))
                {
                    notificationService.SuccessNotification(tempData, successMessage, title);
                }
            }
            else
            {
                var messageFromResponse = result.DeserializeException?.Message ?? result?.RawResponseContent;
                notificationService.CreateNotification(tempData, NotificationHelpers.AlertType.Error, string.IsNullOrEmpty(messageFromResponse) ?
                    (string.IsNullOrEmpty(errorMessage) ? "Something went wrong..." : errorMessage) :
                    messageFromResponse, title);
            }
            return result.Success;
        }
    }
}
