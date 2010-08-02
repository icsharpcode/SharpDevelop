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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using System.Threading;

namespace UpdateAssemblyInfo
{
	// Updates the version numbers in the assembly information.
	class MainClass
	{
		const string templateFile       = "Main/GlobalAssemblyInfo.template";
		const string globalAssemblyInfo = "Main/GlobalAssemblyInfo.cs";
		const string configTemplateFile = "Main/StartUp/Project/app.template.config";
		const string configFile         = "Main/StartUp/Project/SharpDevelop.exe.config";
		const string configFile2        = "Main/ICSharpCode.SharpDevelop.Sda/ICSharpCode.SharpDevelop.Sda.dll.config";
		const string subversionLibraryDir = "Libraries/SharpSvn";
		
		const string BaseCommit = "69a9221f57e6b184010f5bad16aa181ced91a0df";
		const int BaseCommitRev = 6351;
		
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
					string versionNumber = GetMajorVersion() + "." + revisionNumber;
					UpdateStartup();
					UpdateRedirectionConfig(versionNumber);
					return 0;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
				return 3;
			}
		}
		
		static void UpdateStartup()
		{
			string content;
			using (StreamReader r = new StreamReader(templateFile)) {
				content = r.ReadToEnd();
			}
			content = content.Replace("-INSERTREVISION-", revisionNumber);
			content = content.Replace("-INSERTCOMMITHASH-", gitCommitHash);
			if (File.Exists(globalAssemblyInfo)) {
				using (StreamReader r = new StreamReader(globalAssemblyInfo)) {
					if (r.ReadToEnd() == content) {
						// nothing changed, do not overwrite file to prevent recompilation
						// every time.
						return;
					}
				}
			}
			using (StreamWriter w = new StreamWriter(globalAssemblyInfo, false, Encoding.UTF8)) {
				w.Write(content);
			}
		}
		
		static void UpdateRedirectionConfig(string fullVersionNumber)
		{
			string content;
			using (StreamReader r = new StreamReader(configTemplateFile)) {
				content = r.ReadToEnd();
			}
			content = content.Replace("$INSERTVERSION$", fullVersionNumber);
			content = content.Replace("$INSERTCOMMITHASH$", gitCommitHash);
			if (File.Exists(configFile) && File.Exists(configFile2)) {
				using (StreamReader r = new StreamReader(configFile)) {
					if (r.ReadToEnd() == content) {
						// nothing changed, do not overwrite file to prevent recompilation
						// every time.
						return;
					}
				}
			}
			using (StreamWriter w = new StreamWriter(configFile, false, Encoding.UTF8)) {
				w.Write(content);
			}
			using (StreamWriter w = new StreamWriter(configFile2, false, Encoding.UTF8)) {
				w.Write(content);
			}
		}
		
		static string GetMajorVersion()
		{
			string version = "?";
			// Get main version from startup
			using (StreamReader r = new StreamReader(templateFile)) {
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
		static string gitCommitHash;
		
		static void RetrieveRevisionNumber()
		{
			if (revisionNumber == null) {
				if (Directory.Exists("..\\..\\.git")) {
					ReadRevisionNumberFromGit();
				}
			}
			
			if (revisionNumber == null) {
				ReadRevisionFromFile();
			}
		}
		
		static void ReadRevisionNumberFromGit()
		{
			ProcessStartInfo info = new ProcessStartInfo("cmd", "/c git rev-list " + BaseCommit + "..HEAD");
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
				using (StreamReader reader = new StreamReader(@"..\REVISION")) {
					revisionNumber = reader.ReadLine();
					gitCommitHash = reader.ReadLine();
				}
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
