using System;
using System.Text.RegularExpressions;

namespace WebCrawler.UriFilters
{
	class BaseFilter
	{
		internal virtual bool IsValid(string uri)
		{
			if (!Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
			{
				return false;
			}

			uri = Regex.Replace(uri, "\\s", "");
			return !(uri.StartsWith("javascript:") || uri.StartsWith("data:"));
		}
	}
}
