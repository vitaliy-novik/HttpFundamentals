using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using static System.Console;

namespace LocalSite
{
	class Program
	{
		private static FolderService folderService;
		static void Main(string[] args)
		{
			string url = RequestInput("Enter URL:", Crawler.ValidateUrl);
			string path = RequestInput("Enter path:", FolderService.ValidatePath);
			int depth = Int32.Parse(RequestInput("Enter depth:", Program.ValidateDepth));

			folderService = new FolderService(path);
			folderService.CreateIfNotExists(path);
			Crawler crawler = new Crawler(url, depth);

			crawler.OnRespose += ProcessResponse;
			crawler.Start();

		}

		private static void ProcessResponse(Stream stream, string url, string statusCode)
		{
			string path = folderService.SaveFile(stream, url);
			WriteLine($"{url} - {statusCode} -> {path}");
		}

		static string RequestInput(string message, Func<string, bool> validate)
		{
			bool isValid = false;
			string input = string.Empty;
			while (!isValid)
			{
				WriteLine(message);
				input = ReadLine();
				if (!string.IsNullOrEmpty(input))
				{
					try
					{
						isValid = validate(input);
					}
					catch (Exception ex)
					{

						WriteLine(ex.Message);
					}
				}
			}

			return input;
		}

		static bool ValidateDepth(string input)
		{
			int depth;
			bool isValid = Int32.TryParse(input, out depth);
			if (!isValid || depth < 0)
			{
				throw new ArgumentException("depth should be a positive integer");
			}

			return isValid;
		}
	}
}
