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
			Verbose verbose = (Verbose)GetVerboseSelection();
			string extensions = RequestInput("Enter comma-separated lists of file extensions:", input => true);

			folderService = new FolderService(path);
			folderService.CreateIfNotExists(path);
			Crawler crawler = new Crawler(url, depth, verbose, extensions);

			crawler.OnRespose += ProcessResponse;
			crawler.Start();

		}

		private static void ProcessResponse(Stream stream, string url, string statusCode)
		{
			string path = string.Empty;
			if (statusCode.Equals("OK", StringComparison.OrdinalIgnoreCase))
			{
				path = folderService.SaveFile(stream, url);
			}
			
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

		static int GetVerboseSelection()
		{
			List<string> verboses = Enum.GetNames(typeof(Verbose)).ToList();
			WriteLine("Select verbose:");
			int currentSelection = 0;
			ConsoleKeyInfo key = new ConsoleKeyInfo();
			while (key.Key != ConsoleKey.Enter)
			{
				for (int i = 0; i < verboses.Count; ++i)
				{
					WriteLine($"{GetSelector(currentSelection, i)} {verboses.ElementAt(i)}");
				}
				key = ReadKey(true);
				if (key.Key == ConsoleKey.UpArrow)
				{
					currentSelection = (currentSelection - 1) % verboses.Count;
				}
				if (key.Key == ConsoleKey.DownArrow)
				{
					currentSelection = (currentSelection + 1) % verboses.Count;
				}

				SetCursorPosition(0, CursorTop - verboses.Count);
			}

			return currentSelection;
		}

		static string GetSelector(int currentSelector, int index)
		{
			return index == currentSelector ? " * " : "   ";
		}
	}
}
