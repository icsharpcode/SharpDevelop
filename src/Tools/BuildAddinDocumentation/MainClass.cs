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
using System.Text.RegularExpressions;
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
			
			//sdVersion = FileVersionInfo.GetVersionInfo(Path.GetFullPath(Path.Combine(srcDir, "..\\bin\\ICSharpCode.Core.dll")));
			Comparison<XmlElement> comparison = delegate(XmlElement a, XmlElement b) {
				string shortNameA = a.GetAttribute("name").Substring(a.GetAttribute("name").LastIndexOf('.') + 1);
				string shortNameB = b.GetAttribute("name").Substring(b.GetAttribute("name").LastIndexOf('.') + 1);
				return shortNameA.CompareTo(shortNameB);
			};
			doozers.Sort(comparison);
			conditions.Sort(comparison);
			
			Debug.WriteLine("Writing doozer list");
			using (StreamWriter html = new StreamWriter(Path.Combine(srcDir, "..\\doc\\technotes\\DoozerList.html"))) {
				WriteHeader(html, "Doozer List");
				WriteList(html, doozers, true);
				WriteFooter(html);
			}
			Debug.WriteLine("Writing condition list");
			using (StreamWriter html = new StreamWriter(Path.Combine(srcDir, "..\\doc\\technotes\\ConditionList.html"))) {
				WriteHeader(html, "Condition List");
				WriteList(html, conditions, false);
				WriteFooter(html);
			}
			Debug.WriteLine("Building Addin schema");
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(srcDir, "..\\data\\schemas\\Addin.xsd"));
			UpdateSchema(doc, doozers, conditions);
			using (XmlTextWriter writer = new XmlTextWriter(Path.Combine(srcDir, "..\\data\\schemas\\Addin.xsd"), System.Text.Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.IndentChar = '\t';
				writer.Indentation = 1;
				doc.Save(writer);
			}
			Debug.WriteLine("Finished");
		}
		
		static void RecursiveInsertDoozerList(XmlElement e, List<XmlElement> doozers, List<XmlElement> conditionList)
		{
			List<XmlNode> oldChilds = new List<XmlNode>();
			int foundMark = 0;
			foreach (XmlNode node in e) {
				if (foundMark > 0) {
					oldChilds.Add(node);
				} else {
					if (node.Value != null) {
						if (node.Value.Trim() == "!!! INSERT DOOZER LIST !!!") {
							foundMark = 1;
						} else if (node.Value.Trim() == "!!! INSERT CONDITION ATTRIBUTES !!!") {
							foundMark = 2;
						}
					}
				}
			}
			if (foundMark == 1) {
				foreach (XmlNode node in oldChilds) {
					e.RemoveChild(node);
				}
				foreach (XmlElement doozer in doozers) {
					CreateChild(e, "element").SetAttribute("ref", doozer.GetAttribute("shortname"));
				}
			} else if (foundMark == 2) {
				foreach (XmlNode node in oldChilds) {
					e.RemoveChild(node);
				}
				// create list of attributes
				List<string> attributes = new List<string>();
				foreach (XmlElement condition in conditionList) {
					foreach (XmlElement attribute in condition) {
						if (attribute.Name == "attribute") {
							if (!attributes.Contains(attribute.GetAttribute("name"))) {
								attributes.Add(attribute.GetAttribute("name"));
							}
						}
					}
				}
				attributes.Sort();
				foreach (string attribute in attributes) {
					XmlElement ae = CreateChild(e, "attribute");
					ae.SetAttribute("name", attribute);
					ae.SetAttribute("type", "xs:string");
					ae.SetAttribute("use", "optional");
				}
			} else {
				foreach (XmlNode node in e) {
					if (node is XmlElement)
						RecursiveInsertDoozerList((XmlElement)node, doozers, conditionList);
				}
			}
		}
		
		static void UpdateSchema(XmlDocument doc, List<XmlElement> doozers, List<XmlElement> conditionList)
		{
			List<XmlNode> oldChilds = new List<XmlNode>();
			bool foundMark = false;
			foreach (XmlNode node in doc.DocumentElement) {
				if (foundMark) {
					oldChilds.Add(node);
				} else {
					if (node.Value != null && node.Value.Trim() == "!!! DOOZER START !!!") {
						foundMark = true;
					}
				}
			}
			foreach (XmlNode node in oldChilds) {
				doc.DocumentElement.RemoveChild(node);
			}
			RecursiveInsertDoozerList(doc.DocumentElement, doozers, conditionList);
			foreach (XmlElement doozer in doozers) {
				XmlElement e = CreateChild(doc.DocumentElement, "complexType");
				e.SetAttribute("name", doozer.GetAttribute("shortname"));
				XmlElement e2 = CreateChild(e, "complexContent");
				XmlElement e3 = CreateChild(e2, "extension");
				e3.SetAttribute("base", "AbstractCodon");
				if (doozer["children"] != null) {
					XmlElement choice = CreateChild(e3, "choice");
					choice.SetAttribute("minOccurs", "0");
					choice.SetAttribute("maxOccurs", "unbounded");
					CreateChild(choice, "element").SetAttribute("ref", "ComplexCondition");
					CreateChild(choice, "element").SetAttribute("ref", "Condition");
					foreach (string child in doozer["children"].GetAttribute("childTypes").Split(';')) {
						CreateChild(choice, "element").SetAttribute("ref", child);
					}
					CreateChild(choice, "element").SetAttribute("ref", "Include");
				}
				foreach (XmlElement doozerChild in doozer) {
					if (doozerChild.Name != "attribute")
						continue;
					XmlElement e4 = CreateChild(e3, "attribute");
					e4.SetAttribute("name", doozerChild.GetAttribute("name"));
					if (doozerChild.GetAttribute("use") == "required")
						e4.SetAttribute("use", "required");
					else
						e4.SetAttribute("use", "optional");
					
					XmlElement e5, e6;
					
					e5 = CreateChild(e4, "annotation");
					e6 = CreateChild(e5, "documentation");
					e6.InnerXml = XmlToHtml(doozerChild.InnerXml).Replace("    ", "\t");
					
					if (!doozerChild.HasAttribute("enum")) {
						e4.SetAttribute("type", "xs:string");
					} else {
						e5 = CreateChild(e4, "simpleType");
						e6 = CreateChild(e5, "restriction");
						e6.SetAttribute("base", "xs:string");
						foreach (string val in doozerChild.GetAttribute("enum").Split(';')) {
							CreateChild(e6, "enumeration").SetAttribute("value", val);
						}
					}
				}
				e = CreateChild(doc.DocumentElement, "element");
				e.SetAttribute("name", doozer.GetAttribute("shortname"));
				e.SetAttribute("type", doozer.GetAttribute("shortname"));
				e2 = CreateChild(e, "annotation");
				e3 = CreateChild(e2, "documentation");
				e3.InnerXml = XmlToHtml(doozer["summary"].InnerXml).Replace("    ", "\t");
			}
		}
		
		static XmlElement CreateChild(XmlElement parent, string name)
		{
			XmlElement e = parent.OwnerDocument.CreateElement("xs:" + name, "http://www.w3.org/2001/XMLSchema");
			parent.AppendChild(e);
			return e;
		}
		
		static void WriteList(StreamWriter html, List<XmlElement> elementList, bool isDoozer)
		{
			html.WriteLine("<ul>");
			for (int i = 0; i < elementList.Count; i++) {
				XmlElement e = elementList[i];
				string fullname = e.GetAttribute("name").Substring(2);
				string shortName = fullname.Substring(fullname.LastIndexOf('.') + 1);
				if (shortName == "LazyLoadDoozer" || shortName == "LazyConditionEvaluator") {
					elementList.RemoveAt(i--);
					continue;
				}
				if (isDoozer)
					shortName = shortName.Substring(0, shortName.Length - "doozer".Length);
				else
					shortName = shortName.Substring(0, shortName.Length - "conditionEvaluator".Length);
				if (shortName == "I") { // skip the interface
					elementList.RemoveAt(i--);
					continue;
				}
				e.SetAttribute("shortname", shortName);
				html.WriteLine("  <li><a href=\"#" + shortName + "\">" + shortName + "</a>");
			}
			html.WriteLine("</ul>");
			foreach (XmlElement e in elementList) {
				string fullname = e.GetAttribute("name").Substring(2);
				string shortName = e.GetAttribute("shortname");
				
				html.WriteLine("<div>");
				html.WriteLine("  <h2><a name=\"" + shortName + "\">" + shortName + "</a></h2>");
				html.WriteLine("  <p>" + XmlToHtml(e["summary"].InnerXml) + "</p>");
				html.WriteLine("  <table>");
				html.WriteLine("    <tr>");
				if (isDoozer)
					html.WriteLine("       <th colspan=2>Doozer name:</td>");
				else
					html.WriteLine("       <th colspan=2>Condition name:</td>");
				html.WriteLine("       <td>" + fullname + "</td>");
				html.WriteLine("    </tr>");
				bool lastWasAttribute = false;
				foreach (XmlElement sub in e) {
					switch (sub.Name) {
						case "summary":
						case "example":
							break;
						case "attribute":
							if (!lastWasAttribute) {
								lastWasAttribute = true;
								html.WriteLine("    <tr><td colspan=3><hr><h3>Attributes:</h3></td></tr>");
							}
							html.WriteLine("    <tr>");
							if (sub.HasAttribute("use")) {
								html.WriteLine("       <th>" + sub.GetAttribute("name") + ":</td>");
								html.WriteLine("       <td class=\"userequired\">" + sub.GetAttribute("use") + "</td>");
							} else {
								html.WriteLine("       <th colspan=2>" + sub.GetAttribute("name") + ":</td>");
							}
							html.WriteLine("       <td>" + XmlToHtml(sub.InnerXml) + "</td>");
							html.WriteLine("    </tr>");
							break;
						default:
							if (lastWasAttribute) {
								lastWasAttribute = false;
								html.WriteLine("    <tr><td colspan=3><hr></td></tr>");
							}
							html.WriteLine("    <tr>");
							html.WriteLine("       <th colspan=2>" + char.ToUpper(sub.Name[0]) + sub.Name.Substring(1) + ":</td>");
							html.WriteLine("       <td>" + XmlToHtml(sub.InnerXml) + "</td>");
							html.WriteLine("    </tr>");
							break;
					}
				}
				html.WriteLine("  </table>");
				foreach (XmlElement sub in e) {
					if (sub.Name == "example") {
						html.WriteLine("  <p><span class=\"exampleTitle\">Example: " + XmlToHtml(sub.GetAttribute("title")) + "</span>");
						html.WriteLine("  <br><pre>" + sub.InnerXml.TrimEnd() + "</pre></p>");
					}
				}
				html.WriteLine("</div>");
			}
		}
		
		static string XmlToHtml(string xml)
		{
			return Regex.Replace(xml, @"\<see cref=""\w\:(?:\w+\.)*(\w+)""\s*\/\s*\>", "$1");
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
			XmlDocument doc = GetXmlDocu(Path.GetFullPath(projectFolder));
			if (doc == null) return false;
			foreach (XmlNode node in doc.DocumentElement["members"]) {
				XmlElement member = node as XmlElement;
				if (member == null) {
					Debug.WriteLine(node.Value);
					continue;
				}
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
			string msbuild = Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "..\\v3.5\\msbuild.exe");
			ProcessStartInfo info = new ProcessStartInfo(msbuild, args);
			Debug.WriteLine(projectFolder + ">" + msbuild + " " + args);
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
