using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebCrawler.UriFilters;

namespace WebCrawler
{
	public class Crawler
	{
		private string url;
		private int depth;
		private string extensions;
		private HttpClient client;
		private HtmlParser parser;
		private Verbose verbose;
		private Uri baseUri;
		private ExtensionFilter extensionFilter;
		private VerboseFilter verboseFilter;

		public event Action<Stream, string, string> OnRespose = (stream, url, statusCode) => { };


		public Crawler(string url)
		{
			this.baseUri = UriFactory.Create(url);
			client = new HttpClient { BaseAddress = this.baseUri };
		}

		public Crawler(string url, int depth, Verbose verbose, string extensions) : this(url)
		{
			this.url = url;
			this.depth = depth;
			this.verbose = verbose;
			this.extensions = extensions;
			this.extensionFilter = new ExtensionFilter(extensions);
			this.verboseFilter = new VerboseFilter(verbose, baseUri);
		}

		public void Start()
		{
			List<string> pages = new List<string>() { string.Empty };
			for (int curDepth = 0; curDepth <= this.depth; curDepth++)
			{
				pages = this.DownloadDepth(this.baseUri, pages);
			}
		}

		public List<Uri> DownloadDepth(Uri currentUri, List<string> links)
		{
			List<Uri> newDepthLinks = new List<Uri>();
			foreach (var link in links)
			{
				Uri uri = UriFactory.Create(currentUri, link);
				newDepthLinks.AddRange(this.GetPage(uri));
			}

			return newDepthLinks;
		}

		public List<Uri> GetPage(Uri uri)
		{
			if (!this.verboseFilter.IsValid(uri))
			{
				return new List<Uri>();
			}

			HttpResponseMessage response = client.GetAsync(uri).Result;
			parser = new HtmlParser(response.Content.ReadAsStringAsync().Result);
			List<string> files = parser.GetStylesheets().Union(parser.GetScripts()).Union(parser.GetImages()).ToList();
			OnRespose(response.Content.ReadAsStreamAsync().Result, this.baseUri.MakeRelativeUri(uri).ToString(), response.StatusCode.ToString());
			List<Uri> links = parser.GetLinks().Select(link => UriFactory.Create(link)).Where(u => this.extensionFilter.IsValid(u)).ToList();
			foreach (var file in files)
			{
				GetFile(file);
			}

			return links;
		}

		public void GetFileAsync(string path)
		{
			client.GetAsync(path).ContinueWith(
				(requestTask) =>
				{
					HttpResponseMessage response = requestTask.Result;

					response.EnsureSuccessStatusCode();

					response.Content.ReadAsStreamAsync().ContinueWith((readtask) =>
					{
						this.OnRespose(readtask.Result, path, response.StatusCode.ToString());
					});
				});
		}

		public void GetFile(string path)
		{
			if (!this.extensionFilter.IsValid(path))
			{
				return;
			}

			UriBuilder builder;
			string fileName = string.Empty;
			if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
			{
				if (this.IsAbsoluteUrl(path))
				{
					builder = new UriBuilder(path);
				}
				else
				{
					builder = new UriBuilder(new Uri(client.BaseAddress, path));
				}
				fileName = builder.Path;
			}
			else
			{
				return;
			}

			HttpResponseMessage response = client.GetAsync(path).Result;
			if (!response.StatusCode.Equals(HttpStatusCode.OK))
			{
				return;
			}
			this.OnRespose(response.Content.ReadAsStreamAsync().Result, fileName, response.StatusCode.ToString());
		}


		public static bool ValidateUrl(string url)
		{
			try
			{
				UriBuilder uriBuilder = new UriBuilder(url);
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}

		bool IsAbsoluteUrl(string url)
		{
			Uri result;
			return Uri.TryCreate(url, UriKind.Absolute, out result);
		}
	}
}
