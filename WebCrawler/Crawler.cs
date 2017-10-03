using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace WebCrawler
{
	public class Crawler
	{
		private string url;
		private int depth;
		private HttpClient client;
		private HtmlParser parser;
		private Verbose verbose;
		private UriBuilder uriBuilder;

		public event Action<Stream, string, string> OnRespose = (stream, url, statusCode) => { };


		public Crawler(string url)
		{
			this.uriBuilder = new UriBuilder(url);
			client = new HttpClient { BaseAddress = this.uriBuilder.Uri };
		}

		public Crawler(string url, int depth, Verbose verbose) : this(url)
		{
			this.url = url;
			this.depth = depth;
			this.verbose = verbose;
		}

		public void Start()
		{
			List<string> pages = new List<string>() { string.Empty };
			for (int curDepth = 0; curDepth <= this.depth; curDepth++)
			{
				pages = this.DownloadDepth(pages);
				pages.RemoveAll(s => s.Contains("javascript:"));
			}
		}

		public List<string> DownloadDepth(List<string> links)
		{
			List<string> newDepthLinks = new List<string>();
			foreach (var link in links)
			{
				newDepthLinks.AddRange(this.GetPage(link));
			}

			return newDepthLinks;
		}

		public List<string> GetPage(string url)
		{
			HttpResponseMessage response = client.GetAsync(url).Result;
			parser = new HtmlParser(response.Content.ReadAsStringAsync().Result);
			List<string> files = parser.GetStylesheets().Union(parser.GetScripts()).Union(parser.GetImages()).ToList();
			OnRespose(response.Content.ReadAsStreamAsync().Result, url, response.StatusCode.ToString());
			List<string> links = parser.GetLinks().ToList();
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
