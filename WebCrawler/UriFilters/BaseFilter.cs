using System;
using System.Text.RegularExpressions;

namespace WebCrawler.UriFilters
{
	class BaseFilter
	{
		internal virtual bool IsValid(Uri uri)
		{
			return !(uri.Scheme.Equals("data", StringComparison.OrdinalIgnoreCase)) || 
					uri.Scheme.Equals("javascript", StringComparison.OrdinalIgnoreCase));
		}
	}
}
