using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.UriFilters
{
	internal class ExtensionFilter : BaseFilter
	{
		internal List<string> Extensions { get; set; }

		internal ExtensionFilter(string extensions)
		{
			Extensions = extensions.Split(',').ToList();
		}

		internal override bool IsValid(Uri uri)
		{
			if (!base.IsValid(uri))
			{
				return false;
			}

			return Extensions.Any(ext => uri.AbsolutePath.EndsWith("." + ext, StringComparison.OrdinalIgnoreCase));
		}
	}
}
