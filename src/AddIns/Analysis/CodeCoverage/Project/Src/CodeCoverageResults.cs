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
using System.Xml.Linq;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Reads the code coverage results from the output xml file.
	/// </summary>
	public class CodeCoverageResults
	{
		readonly List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
		readonly Dictionary<string, string> fileNames = new Dictionary<string, string>();
		readonly Dictionary<string, string> assemblies = new Dictionary<string, string>();
		
		public CodeCoverageResults(string fileName)
			: this(new StreamReader(fileName, true))
		{
		}
		
		public CodeCoverageResults(XContainer reader)
		{
			ReadResults(reader);
		}
		
		public CodeCoverageResults(TextReader reader)
			: this(XDocument.Load(reader))
		{
		}
		
		public List<CodeCoverageSequencePoint> GetSequencePoints(string fileName)
		{
			var sequencePoints = new List<CodeCoverageSequencePoint>();
			foreach (CodeCoverageModule module in modules) {
				sequencePoints.AddRange(module.GetSequencePoints(fileName));
			}
			return sequencePoints;
		}
		
		public List<CodeCoverageModule> Modules {
			get { return modules; }
		}
		
		void ReadResults(XContainer reader)
		{
			var modules = reader.Descendants("Module").Where(m => m.Attribute("skippedDueTo") == null);
			foreach (XElement file in reader.Descendants("File")) {
				AddFileName(file);
			}
			foreach (XElement assembly in modules) {
				AddAssembly(assembly);
				RegisterAssembly(assembly);
			}
		}

		private void RegisterAssembly(XElement assembly)
		{
			var classNames =
				assembly.Elements("Classes").Elements("Class").Where(
					c =>
					!c.Element("FullName").Value.Contains("__") && 
					c.Attribute("skippedDueTo") == null).Select(
						c => c.Element("FullName").Value).Distinct().OrderBy(name => name);
			foreach (string className in classNames) {
				AddModule(assembly, className);
			}
		}
		
		/// <summary>
		/// Add module if it does not already exist.
		/// </summary>
		CodeCoverageModule AddModule(XElement reader, string className)
		{
			string assemblyName = GetAssemblyName(reader);
			CodeCoverageModule module = null;
			foreach (CodeCoverageModule existingModule in modules) {
				if (existingModule.Name == assemblyName) {
					module = existingModule;
					break;
				}
			}
			if (module == null) {
				module = new CodeCoverageModule(assemblyName);
				modules.Add(module);
			}
			var methods = reader
				.Elements("Classes")
				.Elements("Class")
				.Where(c => c.Element("FullName").Value.Equals(className, StringComparison.Ordinal))
				.Elements("Methods")
				.Elements("Method");
			foreach (XElement method in methods) {
				AddMethod(module, className.Replace('/','.'), method);
			}
			return module;
		}
		
		string GetAssemblyName(XElement reader)
		{
			string id = reader.Attribute("hash").Value;
			return GetAssembly(id);
		}

		/// <summary>
		/// Sequence points that do not have a file id are not
		/// added to the code coverage method. Typically these are
		/// for types that are not part of the project but types from
		/// the .NET framework. 
		/// </summary>
		CodeCoverageMethod AddMethod(CodeCoverageModule module, string className, XElement reader)
		{
			var method = new CodeCoverageMethod(className, reader, this);
			if (!method.Name.Contains("__")) {
			    module.Methods.Add(method);
			}
			return method;
		}
		
		/// <summary>
		/// Cache result because same FileID is repeated for all (class.)method(s).SequencePoints
		/// </summary>
		private static Tuple<string,string> fileIdNameCache = new Tuple<string, string>(String.Empty,String.Empty);

		/// <summary>
		/// Returns a filename based on the file id. The filenames are stored in the
		/// PartCover results xml at the start of the file each with its own id.
		/// </summary>
		public string GetFileName(string id)
		{
			if (fileIdNameCache.Item1 != id) {
				fileIdNameCache = new Tuple<string, string>(id, GetDictionaryValue(fileNames, id));
			}
			return fileIdNameCache.Item2;
		}
		
		/// <summary>
		/// Returns an assembly based on the id. The assemblies are stored in the
		/// PartCover results xml at the start of the file each with its own id.
		/// </summary>
		string GetAssembly(string id)
		{
			return GetDictionaryValue(assemblies, id);
		}
		
		string GetDictionaryValue(IReadOnlyDictionary<string, string> dictionary, string key)
		{
			return dictionary.ContainsKey(key) ? dictionary[key] : String.Empty;
		}
		
		/// <summary>
		/// Saves the filename and its associated id for reference.
		/// </summary>
		void AddFileName(XElement reader)
		{
			string id = reader.Attribute("uid").Value;
			string fileName = reader.Attribute("fullPath").Value;
			fileNames.Add(id, fileName);
		}

		/// <summary>
		/// Saves the assembly and its associated id for reference.
		/// </summary>
		void AddAssembly(XElement reader)
		{
			string id = reader.Attribute("hash").Value;
			string name = reader.Element("ModuleName").Value;
			assemblies.Add(id, name);
		}
	}
}
