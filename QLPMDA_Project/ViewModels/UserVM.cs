using System.ComponentModel.DataAnnotations;

namespace QLPMDA_Project.ViewModels
{
    public class UserVM
    {
        [Display(Description = "Tên đăng nhập")]
        [Required(ErrorMessage = "{0} không thể để trống")]
        public string UserName { get; set; }

        [Display(Description = "Mật khẩu")]
        [Required(ErrorMessage = "{0} không thể để trống")]
        public string Password { get; set; }
    }
}
