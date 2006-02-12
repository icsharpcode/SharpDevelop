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
		const string templateFile       = "Main/GlobalAssemblyInfo.template";
		const string globalAssemblyInfo = "Main/GlobalAssemblyInfo.cs";
		
		public static int Main(string[] args)
		{
			try {
				if (!File.Exists("SharpDevelop.sln")) {
					string mainDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(MainClass).Assembly.Location), "../../../.."));
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
				SetVersionInfo("Main/StartUp/Project/SharpDevelop.exe.config", BindingRedirect, "<bindingRedirect oldVersion=\"2.0.0.1\" newVersion=\"" + versionNumber + "\"/>");
				return 0;
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
		
		static string GetMajorVersion()
		{
			string version = "?";
			// Get main version from startup
			using (StreamReader r = new StreamReader(templateFile)) {
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
			} finally {
				Environment.CurrentDirectory = oldWorkingDir;
			}
			if (revisionNumber == null || revisionNumber.Length == 0 || revisionNumber == "0") {
				revisionNumber = ReadRevisionFromFile();
			}
			if (revisionNumber == null || revisionNumber.Length == 0 || revisionNumber == "0") {
				throw new ApplicationException("Error reading revision number");
			}
		}
		#endregion
	}
}
