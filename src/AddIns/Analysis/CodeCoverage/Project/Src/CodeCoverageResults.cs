// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
		Dictionary<string, string> fileNames = new Dictionary<string, string>();
		Dictionary<string, string> assemblies = new Dictionary<string, string>();
		
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
			List<CodeCoverageSequencePoint> sequencePoints = new List<CodeCoverageSequencePoint>();
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
			IEnumerable<XElement> modules = reader.Descendants("Module").Where(m => m.Attribute("skippedDueTo") == null);
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
					!c.Element("FullName").Value.Contains("__") && !c.Element("FullName").Value.Contains("<") &&
					!c.Element("FullName").Value.Contains("/") && c.Attribute("skippedDueTo") == null).Select(
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
				AddMethod(module, className, method);
			}
			return module;
		}
		
		string GetAssemblyName(XElement reader)
		{
			string id = reader.Attribute("hash").Value;
			return GetAssembly(id);
		}
		
		CodeCoverageMethod AddMethod(CodeCoverageModule module, string className, XElement reader)
		{
			CodeCoverageMethod method = new CodeCoverageMethod(className, reader);
			module.Methods.Add(method);
			var points = reader
				.Elements("SequencePoints")
				.Elements("SequencePoint");
			foreach (XElement point in points) {
				AddSequencePoint(method, point, reader);
			}
			return method;
		}
		
		/// <summary>
		/// Sequence points that do not have a file id are not
		/// added to the code coverage method. Typically these are
		/// for types that are not part of the project but types from
		/// the .NET framework. 
		/// </summary>
		void AddSequencePoint(CodeCoverageMethod method, XElement reader, XElement methodNode)
		{
			string fileName = GetFileName(methodNode);
			
			CodeCoverageSequencePoint sequencePoint = 
				new CodeCoverageSequencePoint(fileName, reader);
			method.SequencePoints.Add(sequencePoint);
		}
		
		string GetFileName(XElement reader)
		{
			XElement fileId = reader.Element("FileRef");
			if (fileId != null) {
				return GetFileName(fileId.Attribute("uid").Value);
			}
			return String.Empty;
		}

		/// <summary>
		/// Returns a filename based on the file id. The filenames are stored in the
		/// PartCover results xml at the start of the file each with its own id.
		/// </summary>
		string GetFileName(string id)
		{
			return GetDictionaryValue(fileNames, id);
		}
		
		/// <summary>
		/// Returns an assembly based on the id. The assemblies are stored in the
		/// PartCover results xml at the start of the file each with its own id.
		/// </summary>
		string GetAssembly(string id)
		{
			return GetDictionaryValue(assemblies, id);
		}
		
		string GetDictionaryValue(Dictionary<string, string> dictionary, string key)
		{
			if (dictionary.ContainsKey(key)) {
				return dictionary[key];
			}
			return String.Empty;
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
