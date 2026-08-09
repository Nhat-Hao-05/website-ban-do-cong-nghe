using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class SlugHelper
{
    public static string ToSlug(this string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        title = title.ToLowerInvariant();

        // Xử lý tiếng Việt
        title = title.Replace("đ", "d");
        title = Regex.Replace(title, @"[ôốồổỗộơớởờỡợ]", "o");

        // Loại bỏ dấu
        title = title.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in title)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        title = sb.ToString();

        // Chỉ giữ lại a-z, 0-9, khoảng trắng và dấu -
        title = Regex.Replace(title, @"[^a-z0-9\s-]", "")
                     .Trim()
                     .Replace(' ', '-');

        // Loại bỏ dấu - thừa
        title = Regex.Replace(title, @"-+", "-");

        return title;
    }
}
