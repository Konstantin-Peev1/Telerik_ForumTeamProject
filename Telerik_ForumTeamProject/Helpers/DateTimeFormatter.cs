using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Telerik_ForumTeamProject.Helpers
{
    public class DateTimeFormatter
    {
        public static string FormatToStandard(DateTime dateTime)
        {
            string parsedDate = dateTime.ToString("dd.MM.yyyy HH:mm:s");

            return parsedDate;
        }
    }
}
