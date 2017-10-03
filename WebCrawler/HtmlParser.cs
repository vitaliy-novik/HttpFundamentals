using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace WebCrawler
{
	public class HtmlParser
	{
		private HtmlDocument document;

		public HtmlParser(string html)
		{
			this.document = new HtmlDocument();
			this.document.LoadHtml(html);
		}

		public HtmlParser(Stream stream)
		{
			this.document = new HtmlDocument();
			this.document.Load(stream);
		}

		public IEnumerable<string> GetLinks()
		{
			HtmlNodeCollection collection = this.document.DocumentNode.SelectNodes("//a");
			IEnumerable<string> links = collection?.Select(node => node.GetAttributeValue("href", string.Empty));

			return links ?? new List<string>();
		}

		public IEnumerable<string> GetImages()
		{
			HtmlNodeCollection collection = this.document.DocumentNode.SelectNodes("//img");
			IEnumerable<string> links = collection?.Select(node => node.GetAttributeValue("src", string.Empty));

			return links ?? new List<string>();
		}

		public IEnumerable<string> GetScripts()
		{
			HtmlNodeCollection collection = this.document.DocumentNode.SelectNodes("//script[@src]");
			IEnumerable<string> links = collection?.Select(node => node.GetAttributeValue("src", string.Empty));

			return links ?? new List<string>();
		}

		public IEnumerable<string> GetStylesheets()
		{
			HtmlNodeCollection collection = this.document.DocumentNode.SelectNodes("//link[@rel='stylesheet']");
			IEnumerable<string> links = collection?.Select(node => node.GetAttributeValue("href", string.Empty));

			return links ?? new List<string>();
		}
	}
}
