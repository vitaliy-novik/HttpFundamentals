using System;

namespace WebCrawler
{
	public static class UriFactory
	{
		internal static Uri Create(string uriString)
		{
			if (!uriString.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
			{
				uriString = "http://" + uriString;
			}

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

		internal static Uri Create(Uri absolute, Uri uri)
		{
			if (uri.IsAbsoluteUri)
			{
				return uri;
			}

			return new Uri(absolute, uri);
		}

		internal static Uri Create(Uri absolute, string uriString)
		{
			if (Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
			{
				return new Uri(uriString);
			}

			if (Uri.IsWellFormedUriString(uriString, UriKind.Relative))
			{
				return new Uri(absolute, uriString);
			}

			return absolute;
		}
	}
}
