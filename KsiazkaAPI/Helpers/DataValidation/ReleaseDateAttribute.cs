using System.ComponentModel.DataAnnotations;

namespace KsiazkaAPI
{
    public class ReleaseDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime releaseDate = Convert.ToDateTime(value);
         
            
        
            return releaseDate <= currentDate;
        }
    }
}