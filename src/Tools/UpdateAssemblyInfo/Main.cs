/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 21.07.2005
 * Time: 12:00
 */
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

namespace UpdateAssemblyInfo
{
	// Updates the version numbers in the assembly information.
	class MainClass
	{
		const string BaseCommit = "574708106ab9f057ab721c00708c5b6b9db46872";
		const int BaseCommitRev = 6450;
		
		const string globalAssemblyInfoTemplateFile = "Main/GlobalAssemblyInfo.template";
		static readonly TemplateFile[] templateFiles = {
			new TemplateFile {
				Input = globalAssemblyInfoTemplateFile,
				Output = "Main/GlobalAssemblyInfo.cs"
			},
			new TemplateFile {
				Input = "Main/StartUp/Project/app.template.config",
				Output = "Main/StartUp/Project/SharpDevelop.exe.config"
			},
			new TemplateFile {
				Input = "Main/StartUp/Project/app.template.config",
				Output = "Main/ICSharpCode.SharpDevelop.Sda/ICSharpCode.SharpDevelop.Sda.dll.config"
			},
			new TemplateFile {
				Input = "Setup/SharpDevelop.Setup.wixproj.user.template",
				Output = "Setup/SharpDevelop.Setup.wixproj.user"
			},
			new TemplateFile {
				Input = "../doc/ChangeLog.template.html",
				Output = "../doc/ChangeLog.html"
			},
		};
		
		class TemplateFile
		{
			public string Input, Output;
		}
		
		public static int Main(string[] args)
		{
			try {
				string exeDir = Path.GetDirectoryName(typeof(MainClass).Assembly.Location);
				bool createdNew;
				using (Mutex mutex = new Mutex(true, "SharpDevelopUpdateAssemblyInfo" + exeDir.GetHashCode(), out createdNew)) {
					if (!createdNew) {
						// multiple calls in parallel?
						// it's sufficient to let one call run, so just wait for the other call to finish
						try {
							mutex.WaitOne(10000);
						} catch (AbandonedMutexException) {
						}
						return 0;
					}
					if (!File.Exists("SharpDevelop.sln")) {
						string mainDir = Path.GetFullPath(Path.Combine(exeDir, "../../../.."));
						if (File.Exists(mainDir + "\\SharpDevelop.sln")) {
							Directory.SetCurrentDirectory(mainDir);
						}
					}
					if (!File.Exists("SharpDevelop.sln")) {
						Console.WriteLine("Working directory must be SharpDevelop\\src!");
						return 2;
					}
					RetrieveRevisionNumber();
					UpdateFiles();
					if (args.Contains("--REVISION")) {
						var doc = new XDocument(new XElement(
							"versionInfo",
							new XElement("version", fullVersionNumber),
							new XElement("revision", revisionNumber),
							new XElement("commitHash", gitCommitHash)
						));
						doc.Save("../REVISION");
					}
					return 0;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
				return 3;
			}
		}
		
		static void UpdateFiles()
		{
			foreach (var file in templateFiles) {
				string content;
				using (StreamReader r = new StreamReader(file.Input)) {
					content = r.ReadToEnd();
				}
				content = content.Replace("$INSERTVERSION$", fullVersionNumber);
				content = content.Replace("$INSERTREVISION$", revisionNumber);
				content = content.Replace("$INSERTCOMMITHASH$", gitCommitHash);
				content = content.Replace("$INSERTDATE$", DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
				if (File.Exists(file.Output)) {
					using (StreamReader r = new StreamReader(file.Output)) {
						if (r.ReadToEnd() == content) {
							// nothing changed, do not overwrite file to prevent recompilation
							// every time.
							continue;
						}
					}
				}
				using (StreamWriter w = new StreamWriter(file.Output, false, Encoding.UTF8)) {
					w.Write(content);
				}
			}
		}
		
		static string GetMajorVersion()
		{
			string version = "?";
			// Get main version from startup
			using (StreamReader r = new StreamReader(globalAssemblyInfoTemplateFile)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					string search = "string Major = \"";
					int pos = line.IndexOf(search);
					if (pos >= 0) {
						int e = line.IndexOf('"', pos + search.Length + 1);
						version = line.Substring(pos + search.Length, e - pos - search.Length);
					}
					search = "string Minor = \"";
					pos = line.IndexOf(search);
					if (pos >= 0) {
						int e = line.IndexOf('"', pos + search.Length + 1);
						version = version + "." + line.Substring(pos + search.Length, e - pos - search.Length);
					}
					search = "string Build = \"";
					pos = line.IndexOf(search);
					if (pos >= 0) {
						int e = line.IndexOf('"', pos + search.Length + 1);
						version = version + "." + line.Substring(pos + search.Length, e - pos - search.Length);
					}
				}
			}
			return version;
		}
		
		static void SetVersionInfo(string fileName, Regex regex, string replacement)
		{
			string content;
			using (StreamReader inFile = new StreamReader(fileName)) {
				content = inFile.ReadToEnd();
			}
			string newContent = regex.Replace(content, replacement);
			if (newContent == content)
				return;
			using (StreamWriter outFile = new StreamWriter(fileName, false, Encoding.UTF8)) {
				outFile.Write(newContent);
			}
		}
		
		#region Retrieve Revision Number
		static string revisionNumber;
		static string fullVersionNumber;
		static string gitCommitHash;
		
		static void RetrieveRevisionNumber()
		{
			if (revisionNumber == null) {
				if (Directory.Exists("..\\.git")) {
					ReadRevisionNumberFromGit();
				}
			}
			
			if (revisionNumber == null) {
				ReadRevisionFromFile();
			}
			fullVersionNumber = GetMajorVersion() + "." + revisionNumber;
		}
		
		static void ReadRevisionNumberFromGit()
		{
			ProcessStartInfo info = new ProcessStartInfo("cmd", "/c git rev-list " + BaseCommit + "..HEAD");
			string path = Environment.GetEnvironmentVariable("PATH");
			path += ";" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "git\\bin");
			info.EnvironmentVariables["PATH"] =  path;
			info.RedirectStandardOutput = true;
			info.UseShellExecute = false;
			using (Process p = Process.Start(info)) {
				string line;
				int revNum = BaseCommitRev;
				while ((line = p.StandardOutput.ReadLine()) != null) {
					if (gitCommitHash == null) {
						// first entry is HEAD
						gitCommitHash = line;
					}
					revNum++;
				}
				revisionNumber = revNum.ToString();
				p.WaitForExit();
				if (p.ExitCode != 0)
					throw new Exception("git-rev-list exit code was " + p.ExitCode);
			}
		}
		
		static void ReadRevisionFromFile()
		{
			try {
				XDocument doc = XDocument.Load("../REVISION");
				revisionNumber = (string)doc.Root.Element("revision");
				gitCommitHash = (string)doc.Root.Element("commitHash");
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine();
				Console.WriteLine("The revision number of the SharpDevelop version being compiled could not be retrieved.");
				Console.WriteLine();
				Console.WriteLine("Build continues with revision number '0'...");
				
				revisionNumber = "0";
				gitCommitHash = null;
			}
		}
		#endregion
	}
}
