// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace ICSharpCode.Core
{
	public class Runtime
	{
		string   hintPath;
		string   assembly;
		Assembly loadedAssembly = null;
		
		IList<LazyLoadDoozer> definedDoozers = new List<LazyLoadDoozer>();
		IList<LazyConditionEvaluator> definedConditionEvaluators = new List<LazyConditionEvaluator>();
		ICondition[] conditions;
		IList<AddIn> addIns;
		bool isActive = true;
		bool isAssemblyLoaded;
		readonly object lockObj = new object(); // used to protect mutable parts of runtime
		
		public bool IsActive {
			get {
				lock (lockObj) {
					if (conditions != null) {
						isActive = Condition.GetFailedAction(conditions, this) == ConditionFailedAction.Nothing;
						conditions = null;
					}
					return isActive;
				}
			}
		}
		
		public Runtime(string assembly, string hintPath)
			: this(assembly, hintPath, AddInTree.AddIns)
		{
		}
		
		public Runtime(string assembly, string hintPath, IList<AddIn> addIns)
		{
			this.assembly = assembly;
			this.hintPath = hintPath;
			this.addIns = addIns;
		}
		
		public string Assembly {
			get { return assembly; }
		}
		
		/// <summary>
		/// Force loading the runtime assembly now.
		/// </summary>
		public void Load()
		{
			lock (lockObj) {
				if (!isAssemblyLoaded) {
					if (!this.IsActive)
						throw new InvalidOperationException("Cannot load inactive AddIn runtime");
					
					isAssemblyLoaded = true;
					
					try {
						if (assembly[0] == ':') {
							loadedAssembly = LoadAssembly(assembly.Substring(1));
						} else if (assembly[0] == '$') {
							int pos = assembly.IndexOf('/');
							if (pos < 0)
								throw new CoreException("Expected '/' in path beginning with '$'!");
							string referencedAddIn = assembly.Substring(1, pos - 1);
							foreach (AddIn addIn in addIns) {
								if (addIn.Enabled && addIn.Manifest.Identities.ContainsKey(referencedAddIn)) {
									string assemblyFile = Path.Combine(Path.GetDirectoryName(addIn.FileName),
									                                   assembly.Substring(pos + 1));
									loadedAssembly = LoadAssemblyFrom(assemblyFile);
									break;
								}
							}
							if (loadedAssembly == null) {
								throw new FileNotFoundException("Could not find referenced AddIn " + referencedAddIn);
							}
						} else {
							loadedAssembly = LoadAssemblyFrom(Path.Combine(hintPath, assembly));
						}

						#if DEBUG
						// preload assembly to provoke FileLoadException if dependencies are missing
						loadedAssembly.GetExportedTypes();
						#endif
					} catch (FileNotFoundException ex) {
						ShowError("The addin '" + assembly + "' could not be loaded:\n" + ex.ToString());
					} catch (FileLoadException ex) {
						ShowError("The addin '" + assembly + "' could not be loaded:\n" + ex.ToString());
					}
				}
			}
		}
		
		public Assembly LoadedAssembly {
			get {
				if (this.IsActive) {
					Load(); // load the assembly, if not already done
					return loadedAssembly;
				} else {
					return null;
				}
			}
		}
		
		public IList<LazyLoadDoozer> DefinedDoozers {
			get {
				return definedDoozers;
			}
		}
		
		public IList<LazyConditionEvaluator> DefinedConditionEvaluators {
			get {
				return definedConditionEvaluators;
			}
		}
		
		public Type FindType(string className)
		{
			Assembly asm = LoadedAssembly;
			if (asm == null)
				return null;
			return asm.GetType(className);
		}
		
		internal static void ReadSection(XmlReader reader, AddIn addIn, string hintPath)
		{
			Stack<ICondition> conditionStack = new Stack<ICondition>();
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition") {
							conditionStack.Pop();
						} else if (reader.LocalName == "Runtime") {
							return;
						}
						break;
					case XmlNodeType.Element:
						switch (reader.LocalName) {
							case "Condition":
								conditionStack.Push(Condition.Read(reader));
								break;
							case "ComplexCondition":
								conditionStack.Push(Condition.ReadComplexCondition(reader));
								break;
							case "Import":
								addIn.Runtimes.Add(Runtime.Read(addIn, reader, hintPath, conditionStack));
								break;
							case "DisableAddIn":
								if (Condition.GetFailedAction(conditionStack, addIn) == ConditionFailedAction.Nothing) {
									// The DisableAddIn node not was not disabled by a condition
									addIn.CustomErrorMessage = reader.GetAttribute("message");
								}
								break;
							default:
								throw new AddInLoadException("Unknown node in runtime section :" + reader.LocalName);
						}
						break;
				}
			}
		}
		
		internal static Runtime Read(AddIn addIn, XmlReader reader, string hintPath, Stack<ICondition> conditionStack)
		{
			if (reader.AttributeCount != 1) {
				throw new AddInLoadException("Import node requires ONE attribute.");
			}
			Runtime	runtime = new Runtime(reader.GetAttribute(0), hintPath);
			if (conditionStack.Count > 0) {
				runtime.conditions = conditionStack.ToArray();
			}
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
									runtime.definedDoozers.Add(new LazyLoadDoozer(addIn, properties));
									break;
								case "ConditionEvaluator":
									if (!reader.IsEmptyElement) {
										throw new AddInLoadException("ConditionEvaluator nodes must be empty!");
									}
									runtime.definedConditionEvaluators.Add(new LazyConditionEvaluator(addIn, properties));
									break;
								default:
									throw new AddInLoadException("Unknown node in Import section:" + nodeName);
							}
							break;
					}
				}
			}
			runtime.definedDoozers             = (runtime.definedDoozers as List<LazyLoadDoozer>).AsReadOnly();
			runtime.definedConditionEvaluators = (runtime.definedConditionEvaluators as List<LazyConditionEvaluator>).AsReadOnly();
			return runtime;
		}
		
		protected virtual Assembly LoadAssembly(string assemblyString)
		{
			return System.Reflection.Assembly.Load(assemblyString);
		}
		
		protected virtual Assembly LoadAssemblyFrom(string assemblyFile)
		{
			return System.Reflection.Assembly.LoadFrom(assemblyFile);
		}
		
		protected virtual void ShowError(string message)
		{
			MessageService.ShowError(message);
		}
	}
}
