/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 21.07.2005
 * Time: 12:00
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace UpdateAssemblyInfo
{
	// Updates the version numbers in the assembly information.
	class MainClass
	{
		static Regex AssemblyVersion = new Regex(@"AssemblyVersion\(.*\)]");
		static Regex BindingRedirect = new Regex(@"<bindingRedirect oldVersion=""2.0.0.1"" newVersion=""[\d\.]+""/>");
		const string mainConfig = "Main/StartUp/Project/Configuration/";
		
		public static int Main(string[] args)
		{
			try {
				if (args.Length != 1) {
					PrintHelp();
					return 1;
				}
				
				if (!File.Exists("SharpDevelop.sln")) {
					if (File.Exists("..\\src\\SharpDevelop.sln")) {
						Directory.SetCurrentDirectory("..\\src");
					}
				}
				if (!File.Exists("SharpDevelop.sln")) {
					Console.WriteLine("Working directory must be SharpDevelop\\src or SharpDevelop\\bin!");
					return 2;
				}
				string arg = args[0];
				if (arg == "StartupOnly") {
					RetrieveRevisionNumber();
					UpdateStartup();
				} else if (arg == "Libraries") {
					RetrieveRevisionNumber();
					UpdateLibraries(GetMajorVersion() + "." + revisionNumber);
				} else if (arg == "ResetLibraries") {
					UpdateLibraries("2.0.0.1");
				} else {
					PrintHelp();
					return 1;
				}
				return 0;
			} catch (Exception ex) {
				Console.WriteLine(ex);
				return 3;
			}
		}
		
		static void PrintHelp()
		{
			Console.WriteLine("Application must be started with 1 argument:");
			Console.WriteLine("UpdateAssemblyInfo StartupOnly    // updates version info in Startup project");
			Console.WriteLine("UpdateAssemblyInfo Libraries      // updates all versions in the libraries, but not in Startup");
			Console.WriteLine("UpdateAssemblyInfo ResetLibraries // resets versions in the libraries to 2.0.0.1 (use this before committing)");
		}
		
		static void UpdateStartup()
		{
			string content;
			using (StreamReader r = new StreamReader(mainConfig + "AssemblyInfo.template")) {
				content = r.ReadToEnd();
			}
			content = content.Replace("-INSERTREVISION-", revisionNumber);
			if (File.Exists(mainConfig + "AssemblyInfo.cs")) {
				using (StreamReader r = new StreamReader(mainConfig + "AssemblyInfo.cs")) {
					if (r.ReadToEnd() == content) {
						// nothing changed, do not overwrite file to prevent recompilation of StartUp
						// every time.
						return;
					}
				}
			}
			using (StreamWriter w = new StreamWriter(mainConfig + "AssemblyInfo.cs", false, Encoding.UTF8)) {
				w.Write(content);
			}
		}
		
		static string GetMajorVersion()
		{
			string version = "?";
			// Get main version from startup
			using (StreamReader r = new StreamReader(mainConfig + "AssemblyInfo.template")) {
				string line;
				while ((line = r.ReadLine()) != null) {
					const string search = "string Version = \"";
					int pos = line.IndexOf(search);
					if (pos >= 0) {
						int e = line.IndexOf('"', pos + search.Length + 1);
						version = line.Substring(pos + search.Length, e - pos - search.Length);
						break;
					}
				}
			}
			return version;
		}
		
		static void UpdateLibraries(string versionNumber)
		{
			StringCollection col = SearchDirectory("AddIns", "AssemblyInfo.cs");
			SearchDirectory("Main", "AssemblyInfo.cs", col);
			SearchDirectory("Libraries", "AssemblyInfo.cs", col);
			string[] dontTouchList = new string[] {
				"Main/StartUp/Project/", // Startup is special case
				"Libraries/log4net/",
				"Libraries/DockPanel_Src/",
				"AddIns/Misc/Debugger/TreeListView/Project/",
			};
			foreach (string fileName in col) {
				bool doSetVersion = true;
				foreach (string dontTouch in dontTouchList) {
					if (fileName.StartsWith(dontTouch.Replace("/", Path.DirectorySeparatorChar.ToString()))) {
						doSetVersion = false;
						break;
					}
				}
				if (doSetVersion) {
					Console.WriteLine("Set revision to file: " + fileName + " to " + versionNumber);
					SetVersionInfo(fileName, AssemblyVersion, "AssemblyVersion(\"" + versionNumber + "\")]");
				}
			}
			SetVersionInfo("Main/StartUp/Project/SharpDevelop.exe.config", BindingRedirect, "<bindingRedirect oldVersion=\"2.0.0.1\" newVersion=\"" + versionNumber + "\"/>");
		}
		
		#region SearchDirectory
		static StringCollection SearchDirectory(string directory, string filemask)
		{
			StringCollection collection = new StringCollection();
			SearchDirectory(directory, filemask, collection);
			return collection;
		}
		
		static void SearchDirectory(string directory, string filemask, StringCollection collection)
		{
			try {
				string[] file = Directory.GetFiles(directory, filemask);
				foreach (string f in file) {
					collection.Add(f);
				}
				
				string[] dir = Directory.GetDirectories(directory);
				foreach (string d in dir) {
					if (d.EndsWith("\\.svn")) continue;
					SearchDirectory(d, filemask, collection);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
		}
		#endregion
		
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
		static string revisionNumber = "0";
		static string ReadRevisionFromFile()
		{
			try {
				using (StreamReader reader = new StreamReader(@"..\REVISION")) {
					return reader.ReadLine();
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine();
				Console.WriteLine("The revision number of the SharpDevelop version being compiled could not be retrieved.");
				Console.WriteLine();
				Console.WriteLine("Build continues with revision number '9999'...");
				try {
					Process[] p = Process.GetProcessesByName("msbuild");
					if (p != null && p.Length > 0) {
						System.Threading.Thread.Sleep(3000);
					}
				} catch {}
				return "9999";
			}
		}
		static void RetrieveRevisionNumber()
		{
			// we use NSvn to be independent from the installed subversion client
			// NSvn is a quite big library (>1 MB).
			// We don't want to have it twice in the repository, so we must use the version in the
			// subversion addin directory without copying it to the UpdateAssemblyInfo directory.
			// That means we have to use reflection on the library.
			string oldWorkingDir = Environment.CurrentDirectory;
			try {
				// Set working directory so msvcp70.dll and msvcr70.dll can be found
				Environment.CurrentDirectory = Path.Combine(oldWorkingDir, "AddIns\\Misc\\SubversionAddIn\\RequiredLibraries");
				Assembly asm = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "NSvn.Core.dll"));
				Type clientType = asm.GetType("NSvn.Core.Client");
				object clientInstance = Activator.CreateInstance(clientType);
				object statusInstance = clientType.InvokeMember("SingleStatus",
				                                                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
				                                                null, clientInstance,
				                                                new object[] { oldWorkingDir });
				Type statusType = statusInstance.GetType();
				object entryInstance = statusType.InvokeMember("Entry",
				                                               BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public,
				                                               null, statusInstance, new object[0]);
				Type entryType = entryInstance.GetType();
				int revision = (int)entryType.InvokeMember("Revision",
				                                           BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public,
				                                           null, entryInstance, new object[0]);
				revisionNumber = revision.ToString();
			} catch (Exception e) {
				Console.WriteLine("Reading revision number with NSvn failed: " + e.Message);
				revisionNumber = ReadRevisionFromFile();
			} finally {
				Environment.CurrentDirectory = oldWorkingDir;
			}
			if (revisionNumber == null || revisionNumber.Length == 0 || revisionNumber == "0") {
				throw new ApplicationException("Error reading revision number");
			}
		}
		#endregion
	}
}
