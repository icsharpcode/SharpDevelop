// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		List<Properties> definedDoozers    = new List<Properties>(1);
		List<Properties> definedConditionEvaluators = new List<Properties>(1);
		
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

		bool isAssemblyLoaded;

		public Assembly LoadedAssembly {
			get {
				if (!isAssemblyLoaded) {
					LoggingService.Info("Loading addin " + assembly);

					isAssemblyLoaded = true;

					try {
						if (assembly[0] == ':') {
							loadedAssembly = System.Reflection.Assembly.Load(assembly.Substring(1));
						} else if (assembly[0] == '$') {
							int pos = assembly.IndexOf('/');
							if (pos < 0)
								throw new ApplicationException("Expected '/' in path beginning with '$'!");
							string referencedAddIn = assembly.Substring(1, pos - 1);
							foreach (AddIn addIn in AddInTree.AddIns) {
								if (addIn.Enabled && addIn.Manifest.Identities.ContainsKey(referencedAddIn)) {
									string assemblyFile = Path.Combine(Path.GetDirectoryName(addIn.FileName),
									                                   assembly.Substring(pos + 1));
									loadedAssembly = System.Reflection.Assembly.LoadFrom(assemblyFile);
									break;
								}
							}
							if (loadedAssembly == null) {
								throw new FileNotFoundException("Could not find referenced AddIn " + referencedAddIn);
							}
						} else {
							loadedAssembly = System.Reflection.Assembly.LoadFrom(Path.Combine(hintPath, assembly));
						}

						#if DEBUG
						// preload assembly to provoke FileLoadException if dependencies are missing
						loadedAssembly.GetExportedTypes();
						#endif
					} catch (FileNotFoundException ex) {
						MessageService.ShowError("The addin '" + assembly + "' could not be loaded:\n" + ex.ToString());
					} catch (FileLoadException ex) {
						MessageService.ShowError("The addin '" + assembly + "' could not be loaded:\n" + ex.ToString());
					}
				}
				return loadedAssembly;
			}
		}
		
		public List<Properties> DefinedDoozers {
			get {
				return definedDoozers;
			}
		}
		
		public List<Properties> DefinedConditionEvaluators {
			get {
				return definedConditionEvaluators;
			}
		}
		
		public object CreateInstance(string instance)
		{
			Assembly asm = LoadedAssembly;
			if (asm == null)
				return null;
			return asm.CreateInstance(instance);
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
								case "Doozer":
									if (!reader.IsEmptyElement) {
										throw new AddInLoadException("Doozer nodes must be empty!");
									}
									LazyLoadDoozer lazyLoadDoozer = new LazyLoadDoozer(addIn, properties);
									
									if (AddInTree.Doozers.ContainsKey(lazyLoadDoozer.Name)) {
										throw new AddInLoadException("Duplicate doozer: " + lazyLoadDoozer.Name);
									}
									AddInTree.Doozers.Add(lazyLoadDoozer.Name, lazyLoadDoozer);
									runtime.definedDoozers.Add(properties);
									break;
								case "ConditionEvaluator":
									if (!reader.IsEmptyElement) {
										throw new AddInLoadException("ConditionEvaluator nodes must be empty!");
									}
									LazyConditionEvaluator lazyLoadConditionEvaluator = new LazyConditionEvaluator(addIn, properties);
									if (AddInTree.ConditionEvaluators.ContainsKey(lazyLoadConditionEvaluator.Name)) {
										throw new AddInLoadException("Duplicate condition evaluator: " + lazyLoadConditionEvaluator.Name);
									}
									AddInTree.ConditionEvaluators.Add(lazyLoadConditionEvaluator.Name, lazyLoadConditionEvaluator);
									runtime.definedConditionEvaluators.Add(properties);
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
