using System.ComponentModel.DataAnnotations;

namespace Common
{
    public enum ApiResultStatusCode 
    {
        [Display( Name ="عملیات با موفقیت انجام شد")]
        Success = 0,
        [Display(Name = "خطا در سرور رخ داده است")]
        ServerError = 1,
        [Display(Name = "پارامتر های ارسالی معتبر نیستند")]
        BadRequest = 2,
        [Display(Name = "یافت نشد ")]
        NotFound = 3,
        [Display(Name = "خطای پردازش")]
        LogicError = 4,
        [Display(Name = "عدم دسترسی")]
        UnAuthorized = 5
    }
}
