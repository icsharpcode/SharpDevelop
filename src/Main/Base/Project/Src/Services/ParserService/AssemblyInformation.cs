//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.IO;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using System.Xml;
//using ICSharpCode.SharpDevelop.Dom;
//using System.Reflection;
//
//namespace ICSharpCode.Core {
//	
//	/// <summary>
//	/// This class loads an assembly and converts all types from this assembly
//	/// to a parser layer Class Collection.
//	/// </summary>
//	[Serializable]
//	public class AssemblyInformation : MarshalByRefObject
//	{
//		List<IClass> classes = new List<IClass>();
//		
//		/// <value>
//		/// A <code>ClassColection</code> that contains all loaded classes.
//		/// </value>
//		public List<IClass> Classes
//		{
//			get {
//				return classes;
//			}
//		}
//		
//		public AssemblyInformation()
//		{
//		}
//		
//		string loadingPath = String.Empty;
//		
//		Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
//		{
//			string file = args.Name;
//			int idx = file.IndexOf(',');
//			if (idx >= 0) {
//				file = file.Substring(0, idx);
//			}
//			try {
//				if (File.Exists(loadingPath + file + ".exe")) {
//					return Assembly.ReflectionOnlyLoadFrom(loadingPath + file + ".exe");
//				}
//				if (File.Exists(loadingPath + file + ".dll")) {
//					return Assembly.ReflectionOnlyLoadFrom(loadingPath + file + ".dll");
//				} 
//			} catch (Exception ex) {
//				Console.WriteLine("Can't load assembly : " + ex.ToString());
//			}
//			return null;
//		}
//		
//		public void Load(string fileName)
//		{
//			try {
//				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
//				// read xml documentation for the assembly
//				XmlDocument doc        = null;
//				Hashtable   docuNodes  = new Hashtable();
//				string      xmlDocFile = System.IO.Path.ChangeExtension(fileName, ".xml");
//				
//				string   localizedXmlDocFile = System.IO.Path.GetDirectoryName(fileName) + System.IO.Path.DirectorySeparatorChar +
//				                               Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName + System.IO.Path.DirectorySeparatorChar +
//									           System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(fileName), ".xml");
//				if (System.IO.File.Exists(localizedXmlDocFile)) {
//					xmlDocFile = localizedXmlDocFile;
//				}
//				if (System.IO.File.Exists(xmlDocFile)) {
//					doc = new XmlDocument();
//					doc.Load(xmlDocFile);
//					
//					// convert the XmlDocument into a hash table
//					if (doc.DocumentElement != null && doc.DocumentElement["members"] != null) {
//						foreach (XmlNode node in doc.DocumentElement["members"].ChildNodes) {
//							if (node != null && node.Attributes != null && node.Attributes["name"] != null) {
//								docuNodes[node.Attributes["name"].InnerText] = node;
//							}
//						}
//					}
//				}
//				loadingPath = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
//				System.Reflection.Assembly asm = Assembly.ReflectionOnlyLoadFrom(fileName);
//				foreach (Type type in asm.GetTypes()) {
//					if (!type.FullName.StartsWith("<") && type.IsPublic) {
//						classes.Add(new ReflectionClass(type, docuNodes));
//					}
//				}
//			} catch (Exception e) {
//				Console.WriteLine("Got exception while loading assembly {0} : {1}", fileName, e.Message);
//			} finally {
//				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(MyResolveEventHandler);
//			}
//		}
//	}
//}
