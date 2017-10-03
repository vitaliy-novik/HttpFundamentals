using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebCrawler
{
	class StringComparerIgnoreWhiteSpaces : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			x = Regex.Replace(x, "\\s", "");
			y = Regex.Replace(y, "\\s", "");

			return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
		}
	}
}
