// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	public class Runtime
	{
		string           hintPath;
		string           assembly;
		Assembly         loadedAssembly = null;
		List<Properties> definedErbauer    = new List<Properties>(1);
		List<Properties> definedConditions = new List<Properties>(1);
		
		public Runtime(string assembly, string hintPath)
		{
			this.assembly = assembly;
			this.hintPath = hintPath;
		}
		
		public string Assembly {
			get { 
				return assembly;
			}
		}
		
		public Assembly LoadedAssembly {
			get {
				if (loadedAssembly == null) {
					#if DEBUG
					Console.WriteLine("Loading addin " + assembly + "...");
					#endif
					if (assembly[0] == '/') {
						loadedAssembly = System.Reflection.Assembly.Load(assembly.Substring(1));
					} else {
						loadedAssembly = System.Reflection.Assembly.LoadFrom(Path.Combine(hintPath, assembly));
					}
				}
				return loadedAssembly;
			}
		}
		
		public List<Properties> DefinedErbauer {
			get { 
				return definedErbauer;
			}
		}
		
		public List<Properties> DefinedConditions {
			get { 
				return definedConditions;
			}
		}
		
		public object CreateInstance(string instance)
		{
			return LoadedAssembly.CreateInstance(instance);
		}
		
		internal static Runtime Read(AddIn addIn, XmlTextReader reader, string hintPath)
		{
			if (reader.AttributeCount != 1) {
				throw new AddInLoadException("Import node requires ONE attribute.");
			}
			Runtime	runtime = new Runtime(reader.GetAttribute(0), hintPath);
			if (!reader.IsEmptyElement) {
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.EndElement:
							if (reader.LocalName == "Import") {
								return runtime;
							}
							break;
						case XmlNodeType.Element:
							string nodeName = reader.LocalName;
							Properties properties = Properties.ReadFromAttributes(reader);
							switch (nodeName) {
								case "Erbauer":
									if (!reader.IsEmptyElement) {
										throw new AddInLoadException("Erbauer nodes must be empty!");
									}
									LazyLoadErbauer lazyLoadErbauer = new LazyLoadErbauer(addIn, properties);
									
									if (AddInTree.Erbauer.ContainsKey(lazyLoadErbauer.Name)) {
										throw new AddInLoadException("Duplicate erbauer: " + lazyLoadErbauer.Name);
									}
									AddInTree.Erbauer.Add(lazyLoadErbauer.Name, lazyLoadErbauer);
									runtime.definedErbauer.Add(properties);
									break;
								case "Auswerter":
									if (!reader.IsEmptyElement) {
										throw new AddInLoadException("Auswerter nodes must be empty!");
									}
									LazyLoadAuswerter lazyLoadAuswerter = new LazyLoadAuswerter(addIn, properties);
									if (AddInTree.Auswerter.ContainsKey(lazyLoadAuswerter.Name)) {
										throw new AddInLoadException("Duplicate auswerter: " + lazyLoadAuswerter.Name);
									}
									AddInTree.Auswerter.Add(lazyLoadAuswerter.Name, lazyLoadAuswerter);
									runtime.definedConditions.Add(properties);
									break;
								default:
									throw new AddInLoadException("Unknown node in Import section:" + nodeName);
							}
							break;
					}
				}
			}
			return runtime;
		}
	}
}
