using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Telerik_ForumTeamProject.Helpers
{
    public class DateTimeFormatter
    {
        private readonly DateTime _dateTime;
        private readonly string _dateFormat;
        public DateTimeFormatter(DateTime dateTime)
        {
            
        }
        public static string FormatToStandard(DateTime dateTime)
        {
            string parsedDate = dateTime.ToString("dd.MM.yyyy HH:mm:s");

            return parsedDate;
        }
    }
}
