// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		
		List<LazyLoadDoozer> definedDoozers = new List<LazyLoadDoozer>();
		List<LazyConditionEvaluator> definedConditionEvaluators = new List<LazyConditionEvaluator>();
		ICondition[] conditions;
		IAddInTree addInTree;
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
		
		public Runtime(IAddInTree addInTree, string assembly, string hintPath)
		{
			if (addInTree == null)
				throw new ArgumentNullException("addInTree");
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			this.addInTree = addInTree;
			this.assembly = assembly;
			this.hintPath = hintPath;
		}
		
		public string Assembly {
			get { return assembly; }
		}
		
		/// <summary>
		/// Gets whether the assembly belongs to the host application (':' prefix).
		/// </summary>
		public bool IsHostApplicationAssembly {
			get { return !string.IsNullOrEmpty(assembly) && assembly[0] == ':'; }
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
							foreach (var addIn in addInTree.AddIns) {
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
		
		public IEnumerable<KeyValuePair<string, IDoozer>> DefinedDoozers {
			get {
				return definedDoozers.Select(d => new KeyValuePair<string, IDoozer>(d.Name, d));
			}
		}
		
		public IEnumerable<KeyValuePair<string, IConditionEvaluator>> DefinedConditionEvaluators {
			get {
				return definedConditionEvaluators.Select(c => new KeyValuePair<string, IConditionEvaluator>(c.Name, c));
			}
		}
		
		public Type FindType(string className)
		{
			Assembly asm = LoadedAssembly;
			if (asm == null)
				return null;
			return asm.GetType(className);
		}
		
		internal static List<Runtime> ReadSection(XmlReader reader, AddIn addIn, string hintPath)
		{
			List<Runtime> runtimes = new List<Runtime>();
			Stack<ICondition> conditionStack = new Stack<ICondition>();
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition") {
							conditionStack.Pop();
						} else if (reader.LocalName == "Runtime") {
							return runtimes;
						}
						break;
					case XmlNodeType.Element:
						switch (reader.LocalName) {
							case "Condition":
								conditionStack.Push(Condition.Read(reader, addIn));
								break;
							case "ComplexCondition":
								conditionStack.Push(Condition.ReadComplexCondition(reader, addIn));
								break;
							case "Import":
								runtimes.Add(Runtime.Read(addIn, reader, hintPath, conditionStack));
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
			return runtimes;
		}
		
		internal static Runtime Read(AddIn addIn, XmlReader reader, string hintPath, Stack<ICondition> conditionStack)
		{
			if (reader.AttributeCount != 1) {
				throw new AddInLoadException("Import node requires ONE attribute.");
			}
			Runtime	runtime = new Runtime(addIn.AddInTree, reader.GetAttribute(0), hintPath);
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
			ServiceSingleton.GetRequiredService<IMessageService>().ShowError(message);
		}
	}
}
