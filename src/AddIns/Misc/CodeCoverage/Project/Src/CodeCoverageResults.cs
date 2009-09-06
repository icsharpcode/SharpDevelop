// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

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
		
		public CodeCoverageResults(XmlReader reader)
		{
			ReadResults(reader);
		}
		
		public CodeCoverageResults(TextReader reader)
			: this(new XmlTextReader(reader))
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
		
		void ReadResults(XmlReader reader)
		{
			CodeCoverageModule currentModule = null;
			CodeCoverageMethod currentMethod = null;
			string currentClassName = String.Empty;
			
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						if (reader.Name == "Type") {
							currentModule = AddModule(reader);
							currentClassName = reader.GetAttribute("name");
						} else if ((reader.Name == "Method") && (currentModule != null) && (!reader.IsEmptyElement)) {
							currentMethod = AddMethod(currentModule, currentClassName, reader);
						} else if ((reader.Name == "pt") && (currentMethod != null)) {
							AddSequencePoint(currentMethod, reader);
						} else if (reader.Name == "File") {
							AddFileName(reader);
						} else if (reader.Name == "Assembly") {
							AddAssembly(reader);
						}
						break;
					case XmlNodeType.EndElement:
						if (currentMethod != null && reader.Name == "Method" && currentMethod.SequencePoints.Count == 0) {
							// Remove method that has no sequence points.
							currentModule.Methods.Remove(currentMethod);
						}
						break;
				}
			}
			reader.Close();
			
			RemoveModulesWithNoMethods();
		}
		
		/// <summary>
		/// Add module if it does not already exist.
		/// </summary>
		CodeCoverageModule AddModule(XmlReader reader)
		{
			string id = reader.GetAttribute("asmref");
			string assemblyName = GetAssembly(id);
			foreach (CodeCoverageModule existingModule in modules) {
				if (existingModule.Name == assemblyName) {
					return existingModule;
				}
			}
			
			CodeCoverageModule module = new CodeCoverageModule(assemblyName);
			modules.Add(module);
			return module;
		}
		
		CodeCoverageMethod AddMethod(CodeCoverageModule module, string className, XmlReader reader)
		{
			CodeCoverageMethod method = new CodeCoverageMethod(reader.GetAttribute("name"), className, GetMethodAttributes(reader));
			module.Methods.Add(method);
			return method;
		}
		
		MethodAttributes GetMethodAttributes(XmlReader reader)
		{
			string flags = reader.GetAttribute("flags");
			if (flags != null) {
				try {
					return (MethodAttributes)Enum.Parse(typeof(MethodAttributes), flags);
				} catch (ArgumentException) { }
			}
			return MethodAttributes.Public;
		}
		
		/// <summary>
		/// Sequence points that do not have a file id are not
		/// added to the code coverage method. Typically these are
		/// for types that are not part of the project but types from
		/// the .NET framework. 
		/// </summary>
		void AddSequencePoint(CodeCoverageMethod method, XmlReader reader)
		{
			string fileId = reader.GetAttribute("fid");
			if (fileId != null) {
				string fileName = GetFileName(fileId);
				CodeCoverageSequencePoint sequencePoint = new CodeCoverageSequencePoint(fileName,
					GetInteger(reader.GetAttribute("visit")),
					GetInteger(reader.GetAttribute("sl")),
					GetInteger(reader.GetAttribute("sc")),
					GetInteger(reader.GetAttribute("el")),
					GetInteger(reader.GetAttribute("ec")),
					false);
				method.SequencePoints.Add(sequencePoint);
			}
		}
		
		int GetInteger(string s)
		{
			int val;
			if (Int32.TryParse(s, out val)) {
				return val;
			}
			return 0;
		}
		
		/// <summary>
		/// Returns a filename based on the file id. The filenames are stored in the
		/// PartCover results xml at the start of the file each with its own
		/// id.
		/// </summary>
		string GetFileName(string id)
		{
			return GetDictionaryValue(fileNames, id);
		}
		
		/// <summary>
		/// Returns an assembly based on the id. The assemblies are stored in the
		/// PartCover results xml at the start of the file each with its own
		/// id.
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
		void AddFileName(XmlReader reader)
		{
			string id = reader.GetAttribute("id");
			string fileName = reader.GetAttribute("url");
			fileNames.Add(id, fileName);
		}

		/// <summary>
		/// Saves the assembly and its associated id for reference.
		/// </summary>
		void AddAssembly(XmlReader reader)
		{
			string id = reader.GetAttribute("id");
			string name = reader.GetAttribute("name");
			assemblies.Add(id, name);
		}
		
		/// <summary>
		/// Removes any modules that do not contain any methods.
		/// </summary>
		void RemoveModulesWithNoMethods()
		{
			for (int i = modules.Count - 1; i >= 0; --i) {
				CodeCoverageModule module = modules[i];
				if (module.Methods.Count == 0) {
					modules.Remove(module);
				}
			}
		}
	}
}
