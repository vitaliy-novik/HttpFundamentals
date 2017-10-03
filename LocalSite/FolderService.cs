using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalSite
{
	public class FolderService
	{
		private string basePath;

		public event Action<string> OnSaved;

		public FolderService(string path)
		{
			this.basePath = path;
		}

		public string SaveFile(Stream stream, string path)
		{
			path = path.Replace('/', Path.DirectorySeparatorChar);
			if (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
			{
				path = this.basePath + path;
			}
			else
			{
				path = Path.Combine(this.basePath, path);
			}

			string fileName = Path.GetFileName(path);
			if (Directory.Exists(path))
			{
				path = Path.Combine(path, "index.html");
			}
			string directories = Path.GetDirectoryName(path);
			//fileName = this.ReplaceInvalidCharacters(fileName);
			Directory.CreateDirectory(directories);
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				stream.CopyToAsync(fileStream).Wait();
			}
				
			return path;
		}

		public string ReplaceInvalidCharacters(string name)
		{
			string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			foreach (var ch in invalid)
			{
				name = name.Replace(ch.ToString(), "");
			}

			return name;
		}

		public static bool ValidatePath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("Path can not be empty");
			}

			if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
			{
				throw new ArgumentException("Directory is not empty");
			}

			return true;
		}
		
		public void CreateIfNotExists(string path)
		{
			if (!Directory.Exists(path))
			{
				DirectoryInfo directory = Directory.CreateDirectory(path);
			}
		}
	}
}
