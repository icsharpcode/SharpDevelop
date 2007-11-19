// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
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
						if (reader.Name == "type") {
							currentModule = AddModule(reader);
							currentClassName = reader.GetAttribute("name");
						} else if (reader.Name == "method" && currentModule != null) {
							currentMethod = AddMethod(currentModule, currentClassName, reader);
						} else if (reader.Name == "pt" && currentMethod != null) {
							AddSequencePoint(currentMethod, reader);
						} else if (reader.Name == "file") {
							AddFileName(reader);
						}
						break;
					case XmlNodeType.EndElement:
						if (currentMethod != null && reader.Name == "method" && currentMethod.SequencePoints.Count == 0) {
							// Remove method that has no sequence points.
							currentModule.Methods.Remove(currentMethod);
						}
						break;
				}
			}
			reader.Close();
			
			RemoveModulesWithNoMethods();
			
//			RemoveExcludedModules();
//			RemoveExcludedMethods();
		}
		
		/// <summary>
		/// Add module if it does not already exist.
		/// </summary>
		CodeCoverageModule AddModule(XmlReader reader)
		{
			string assembly = reader.GetAttribute("asm");
			foreach (CodeCoverageModule existingModule in modules) {
				if (existingModule.Name == assembly) {
					return existingModule;
				}
			}
			
			CodeCoverageModule module = new CodeCoverageModule(assembly);
			modules.Add(module);
			return module;
		}
		
		CodeCoverageMethod AddMethod(CodeCoverageModule module, string className, XmlReader reader)
		{
			CodeCoverageMethod method = new CodeCoverageMethod(reader.GetAttribute("name"), className);
			module.Methods.Add(method);
			return method;
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
		
//		void RemoveExcludedModules()
//		{
//			List<CodeCoverageModule> excludedModules = new List<CodeCoverageModule>();
//			
//			foreach (CodeCoverageModule module in modules) {
//				if (module.IsExcluded) {
//					excludedModules.Add(module);
//				}
//			}
//			
//			foreach (CodeCoverageModule excludedModule in excludedModules) {
//				modules.Remove(excludedModule);
//			}
//		}
//		
//		void RemoveExcludedMethods()
//		{
//			List<CodeCoverageMethod> excludedMethods = new List<CodeCoverageMethod>();
//			
//			foreach (CodeCoverageModule module in modules) {
//				foreach (CodeCoverageMethod method in module.Methods) {
//					if (method.IsExcluded) {
//						excludedMethods.Add(method);
//					}
//				}
//				foreach (CodeCoverageMethod excludedMethod in excludedMethods) {
//					module.Methods.Remove(excludedMethod);
//				}
//			}
//		}
//		
//		bool IsExcluded(XmlReader reader)
//		{
//			string excludedValue = reader.GetAttribute("excluded");
//			if (excludedValue != null) {
//				bool excluded;
//				if (Boolean.TryParse(excludedValue, out excluded)) {
//					return excluded;
//				}
//			}
//			return false;
//		}
		
		/// <summary>
		/// Returns a filename based on the file id. The filenames are stored in the
		/// PartCover results xml at the start of the file each with its own
		/// id.
		/// </summary>
		string GetFileName(string id)
		{
			if (fileNames.ContainsKey(id)) {
				return fileNames[id];
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
