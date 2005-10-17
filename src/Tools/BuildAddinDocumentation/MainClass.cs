/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 17.10.2005
 * Time: 14:50
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace BuildAddinDocumentation
{
	public class MainClass
	{
		static FileVersionInfo sdVersion;
		
		public static void Main()
		{
			string srcDir = @"..\..\..\..\";
			Debug.WriteLine(Path.GetFullPath(srcDir));
			List<XmlElement> doozers = new List<XmlElement>();
			List<XmlElement> conditions = new List<XmlElement>();
			
			if (!ReadXmlDocu(srcDir + "Main\\Core\\Project", doozers, conditions))
				return;
			if (!ReadXmlDocu(srcDir + "Main\\Base\\Project", doozers, conditions))
				return;
			
			// build normal SharpDevelop:
			ProcessStartInfo info = new ProcessStartInfo("cmd", "/c debugbuild.bat");
			info.WorkingDirectory = srcDir;
			Process p = Process.Start(info);
			if (!p.WaitForExit(60000)) {
				Debug.WriteLine("msbuild did not exit");
				return;
			}
			if (p.ExitCode != 0) {
				Debug.WriteLine("msbuild exit code: " + p.ExitCode);
				return;
			}
			
			sdVersion = FileVersionInfo.GetVersionInfo(Path.GetFullPath(Path.Combine(srcDir, "..\\bin\\SharpDevelop.exe")));
			
			Comparison<XmlElement> comparison = delegate(XmlElement a, XmlElement b) {
				string shortNameA = a.GetAttribute("name").Substring(a.GetAttribute("name").LastIndexOf('.') + 1);
				string shortNameB = b.GetAttribute("name").Substring(b.GetAttribute("name").LastIndexOf('.') + 1);
				return shortNameA.CompareTo(shortNameB);
			};
			doozers.Sort(comparison);
			conditions.Sort(comparison);
			
			using (StreamWriter html = new StreamWriter(Path.Combine(srcDir, "..\\doc\\technotes\\DoozerList.html"))) {
				WriteHeader(html, "Doozer List");
				WriteList(html, doozers, true);
				WriteFooter(html);
			}
			using (StreamWriter html = new StreamWriter(Path.Combine(srcDir, "..\\doc\\technotes\\ConditionList.html"))) {
				WriteHeader(html, "Condition List");
				WriteList(html, conditions, false);
				WriteFooter(html);
			}
		}
		
		static void WriteList(StreamWriter html, List<XmlElement> elementList, bool isDoozer)
		{
			html.WriteLine("<ul>");
			foreach (XmlElement e in elementList) {
				string fullname = e.GetAttribute("name").Substring(2);
				string shortName = fullname.Substring(fullname.LastIndexOf('.') + 1);
				if (shortName == "LazyLoadDoozer" || shortName == "LazyConditionEvaluator") continue;
				if (isDoozer)
					shortName = shortName.Substring(0, shortName.Length - "doozer".Length);
				else
					shortName = shortName.Substring(0, shortName.Length - "conditionEvaluator".Length);
				if (shortName == "I") continue; // skip the interface
				
				html.WriteLine("  <li><a href=\"#" + shortName + "\">" + shortName + "</a>");
			}
			html.WriteLine("</ul>");
			foreach (XmlElement e in elementList) {
				string fullname = e.GetAttribute("name").Substring(2);
				string shortName = fullname.Substring(fullname.LastIndexOf('.') + 1);
				if (shortName == "LazyLoadDoozer" || shortName == "LazyConditionEvaluator") continue;
				if (isDoozer)
					shortName = shortName.Substring(0, shortName.Length - "doozer".Length);
				else
					shortName = shortName.Substring(0, shortName.Length - "conditionEvaluator".Length);
				if (shortName == "I") continue; // skip the interface
				
				html.WriteLine("<div>");
				html.WriteLine("  <h2><a name=\"" + shortName + "\">" + shortName + "</a></h2>");
				html.WriteLine("  <p>" + e["summary"].InnerXml + "</p>");
				html.WriteLine("  <table>");
				html.WriteLine("    <tr>");
				if (isDoozer)
					html.WriteLine("       <th>Doozer name:</td>");
				else
					html.WriteLine("       <th>Condition name:</td>");
				html.WriteLine("       <td>" + fullname + "</td>");
				html.WriteLine("    </tr>");
				html.WriteLine("    <tr><td colspan=2><hr><h3>Attributes:</h3></td></tr>");
				bool firstNonAttribute = true;
				foreach (XmlElement sub in e) {
					switch (sub.Name) {
						case "summary":
							break;
						case "attribute":
							html.WriteLine("    <tr>");
							html.WriteLine("       <th>" + sub.GetAttribute("name") + ":</td>");
							html.WriteLine("       <td>" + sub.InnerXml + "</td>");
							html.WriteLine("    </tr>");
							break;
						default:
							if (firstNonAttribute) {
								firstNonAttribute = false;
								html.WriteLine("    <tr><td colspan=2><hr></td></tr>");
							}
							html.WriteLine("    <tr>");
							html.WriteLine("       <th>" + char.ToUpper(sub.Name[0]) + sub.Name.Substring(1) + ":</td>");
							html.WriteLine("       <td>" + sub.InnerXml + "</td>");
							html.WriteLine("    </tr>");
							break;
					}
				}
				html.WriteLine("  </table>");
				html.WriteLine("</div>");
			}
		}
		
		static void WriteHeader(StreamWriter html, string title)
		{
			html.WriteLine("<html><head>");
			html.WriteLine("   <title>" + title + "</title>");
			html.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"listing.css\">");
			html.WriteLine("  <meta name=\"generator\" value=\"BuildAddinDocumentation\">");
			html.WriteLine("</head><body>");
			html.WriteLine("<h1>" + title + "</h1>");
			html.WriteLine("<p class=\"notice\">This file was generated by the tool 'BuildAddinDocumentation'.");
			html.WriteLine("It is based on SharpDevelop " + sdVersion.FileMajorPart + "." + sdVersion.FileMinorPart +
			               "." + sdVersion.FileBuildPart + "." + sdVersion.FilePrivatePart + ".</p>");
		}
		
		static void WriteFooter(StreamWriter html)
		{
			html.WriteLine("</body></html>");
		}
		
		static bool ReadXmlDocu(string projectFolder, List<XmlElement> doozers, List<XmlElement> conditions)
		{
			XmlDocument doc = GetXmlDocu(projectFolder);
			if (doc == null) return false;
			foreach (XmlElement member in doc.DocumentElement["members"]) {
				if (member.GetAttribute("name").EndsWith("Doozer"))
					doozers.Add(member);
				if (member.GetAttribute("name").EndsWith("ConditionEvaluator"))
					conditions.Add(member);
			}
			return true;
		}
		
		static XmlDocument GetXmlDocu(string projectFolder)
		{
			string docFile = Path.GetFullPath("doc.xml");
			if (File.Exists(docFile))
				File.Delete(docFile);
			string args = "\"/p:DocumentationFile=" + docFile +  "\" \"/p:NoWarn=1591 1573 1574 1572 419\"";
			string msbuild = Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "msbuild.exe");
			ProcessStartInfo info = new ProcessStartInfo(msbuild, args);
			info.WorkingDirectory = projectFolder;
			Process p = Process.Start(info);
			if (!p.WaitForExit(60000)) {
				Debug.WriteLine("msbuild did not exit");
				return null;
			}
			if (p.ExitCode != 0) {
				Debug.WriteLine("msbuild exit code: " + p.ExitCode);
				return null;
			}
			XmlDocument doc = new XmlDocument();
			doc.Load(docFile);
			return doc;
		}
	}
}
