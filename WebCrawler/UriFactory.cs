using System;

namespace WebCrawler
{
	public static class UriFactory
	{
		internal static Uri Create(string uriString)
		{
			if (!Uri.IsWellFormedUriString(uriString, UriKind.RelativeOrAbsolute))
			{
				throw new ArgumentException("Uri string is not valid");
			}

			Uri uri;
			if (Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
			{
				uri = new Uri(uriString, UriKind.Absolute);
			}
			else
			{
				uri = new Uri(uriString, UriKind.Relative);
			}

			return uri;
		}
	}
}
