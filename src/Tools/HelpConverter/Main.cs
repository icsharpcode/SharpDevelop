// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using ICSharpCode.HelpConverter.HelpTreeBuilder;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;

namespace ICSharpCode.HelpConverter
{
	public class HelpBrowserApp
	{
		HhcFileParser hhcFileParser;
		public static ArrayList HelpFiles = new ArrayList();
		
		/// <remarks>
		/// Parses the xml tree and generates a TreeNode tree out of it.
		/// </remarks>
		void ParseTree(XmlDocument doc, XmlNode docParent, XmlNode parentNode)
		{
			try {
				foreach (XmlNode node in parentNode.ChildNodes) {
					XmlNode importNode = doc.ImportNode(node, true);
					switch (node.Name) {
						case "Condition":
							Console.WriteLine("trying to load : " + node.Attributes["canload"].InnerText);
							Assembly assembly = Assembly.Load(node.Attributes["canload"].InnerText);
							if(assembly != null) {
								Console.WriteLine("Success");
								ParseTree(doc, docParent, node);
							} else {
								Console.WriteLine("Failure");
							}
							break;
						case "HelpFolder":
							docParent.AppendChild(importNode);
							ParseTree(doc, importNode, node);
							break;
						case "HelpFile":
							Console.WriteLine("Parse hhc : " + node.Attributes["hhc"].InnerText);
							hhcFileParser.Parse(doc,
							                    docParent,
							                    node.Attributes["hhc"].InnerText,
							                    node.Attributes["chm"].InnerText);
							break;
						case "HelpAssemblies":
							ArrayList assemblies = new ArrayList();
							foreach (XmlNode childNode in node.ChildNodes) {
								assemblies.Add(childNode.InnerText);
							}
							if (node.Attributes["helpformat"].InnerText == "SDK") {
								classNodeBuilder.helpFileFormat = new SDKHelpFileFormat();
							} else {
								classNodeBuilder.helpFileFormat = new DirectX9HelpFileFormat();
							}
							new Generator(doc, docParent, (string[])assemblies.ToArray(typeof(string)));
							break;
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine("unexpected exception : " + e.ToString());
			}
		}
		
		void ConvertHelpfile()
		{
			string basePath = Application.StartupPath + Path.DirectorySeparatorChar +
			                  ".." + Path.DirectorySeparatorChar +
			                  ".." + Path.DirectorySeparatorChar +
			                  "doc" + Path.DirectorySeparatorChar +
			                  "help";
			
			if (!File.Exists(basePath + Path.DirectorySeparatorChar + @"HelpDescription.xml")) {
				Console.WriteLine("HelpDescription.xml not found!");
				return;
			}
			
			XmlDocument doc = new XmlDocument();
			doc.Load(basePath + Path.DirectorySeparatorChar + @"HelpDescription.xml");
			
			XmlDocument newDoc = new XmlDocument();
			newDoc.LoadXml("<HelpCollection/>");
			hhcFileParser = new HhcFileParser(basePath);
			ParseTree(newDoc, newDoc.DocumentElement, doc.DocumentElement);
			
			try {
				newDoc.Save(basePath + Path.DirectorySeparatorChar + "HelpConv.xml");
			} catch (Exception e) {
				Console.Error.WriteLine("Can't save HelpConv.xml (No write permission?) : " + e.ToString());
			}
			
			try {
				ZipHelpFile(basePath);
			} catch (Exception e) {
				Console.Error.WriteLine("Error while zipping helpfile : " + e.ToString());
			}
		}
		
		void ZipHelpFile(string basePath)
		{
			Console.WriteLine("ZIP Help Contents");
			HelpFiles.Insert(0, basePath + Path.DirectorySeparatorChar + "HelpConv.xml");
			ZipOutputStream s = new ZipOutputStream(File.Create(basePath + Path.DirectorySeparatorChar + "SharpDevelopHelp.zip"));
			try {
				s.SetLevel(6); 
				Crc32 crc = new Crc32();
				foreach (string file in HelpFiles) {
					Console.WriteLine("zip " + file);
					
					FileStream fs = null;
					ZipEntry entry;
					byte[] buffer;
					
					try {
						fs = File.OpenRead(file);
						buffer = new byte[fs.Length];
						fs.Read(buffer, 0, buffer.Length);
						
						entry = new ZipEntry(Path.GetFileName(file));
						entry.DateTime = DateTime.Now;
						entry.Size = fs.Length;
						crc.Reset();
						crc.Update(buffer);
						entry.Crc  = crc.Value;
					} catch (Exception e) {
						Console.Error.WriteLine("Error reading temp xml file : " + e.ToString());
						continue;
					} finally {
						if (fs != null) {
							fs.Close();
						}
					}
					
					s.PutNextEntry(entry);
					s.Write(buffer, 0, buffer.Length);
					
					Console.WriteLine("remove " + file + " from disk");
					File.Delete(file);
				}
			} catch (Exception e) {
				Console.Error.WriteLine("Error while zipping helpfile : " + e.ToString());
			} finally {
				s.Finish();
				s.Close();
				Console.WriteLine("finished");
			}
		}
		
		public static void Main(String[] args)
		{
			new HelpBrowserApp().ConvertHelpfile();
		}
	}	
}
