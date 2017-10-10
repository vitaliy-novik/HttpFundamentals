using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.UriFilters
{
	internal class VerboseFilter : BaseFilter
	{
		internal Verbose Verbose { get; set; }
		internal Uri BaseUri { get; set; }

		internal VerboseFilter(Verbose verbose, Uri uriBuilder)
		{
			this.Verbose = verbose;
			this.BaseUri = uriBuilder;
		}

		internal override bool IsValid(Uri uri)
		{
			if (!base.IsValid(uri))
			{
				return false;
			}
			
			switch (this.Verbose)
			{
				case Verbose.Unlimited:
					return true;
				case Verbose.Domain:
					return IsDomainEquals(uri);
				case Verbose.Path:
					return IsPathEquals(uri);
				default:
					return true;
			}
		}

		private bool IsDomainEquals(Uri uri)
		{
			if (uri.IsAbsoluteUri)
			{
				return uri.Host.Equals(this.BaseUri.Host, StringComparison.OrdinalIgnoreCase);
			}

			return true;
		}

		private bool IsPathEquals(Uri uri)
		{
			return uri.PathAndQuery.StartsWith(BaseUri.PathAndQuery, StringComparison.OrdinalIgnoreCase);
		}
	}
}
