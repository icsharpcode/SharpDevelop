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
	public class CodeCoverageResults
	{
		List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
		
		public CodeCoverageResults(string fileName) : this(new StreamReader(fileName, true))
		{
		}
		
		public CodeCoverageResults(XmlReader reader)
		{
			ReadResults(reader);
		}
		
		public CodeCoverageResults(TextReader reader) : this(new XmlTextReader(reader))
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
			get {
				return modules;
			}
		}
		
		void ReadResults(XmlReader reader)
		{
			CodeCoverageModule currentModule = null;
			CodeCoverageMethod currentMethod = null;
			
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						if (reader.Name == "module") {
							currentModule = AddModule(reader);
						} else if (reader.Name == "method" && currentModule != null) {
							currentMethod = AddMethod(currentModule, reader);
						} else if (reader.Name == "seqpnt" && currentMethod != null) {
							AddSequencePoint(currentMethod, reader);
						}
						break;
				}
			}
			reader.Close();
		}
		
		CodeCoverageModule AddModule(XmlReader reader)
		{
			CodeCoverageModule module = new CodeCoverageModule(reader.GetAttribute("assembly"));
			modules.Add(module);
			return module;
		}
		
		CodeCoverageMethod AddMethod(CodeCoverageModule module, XmlReader reader)
		{
			CodeCoverageMethod method = new CodeCoverageMethod(reader.GetAttribute("name"), reader.GetAttribute("class"));
			module.Methods.Add(method);
			return method;
		}
		
		void AddSequencePoint(CodeCoverageMethod method, XmlReader reader)
		{
			CodeCoverageSequencePoint sequencePoint = new CodeCoverageSequencePoint(reader.GetAttribute("document"), 
				GetInteger(reader.GetAttribute("visitcount")),
				GetInteger(reader.GetAttribute("line")),
				GetInteger(reader.GetAttribute("column")),
				GetInteger(reader.GetAttribute("endline")),
				GetInteger(reader.GetAttribute("endcolumn")));
			method.SequencePoints.Add(sequencePoint);
		}
		
		int GetInteger(string s)
		{
			int val;
			if (Int32.TryParse(s, out val)) {
				return val;
			}
			return 0;
		}
	}
}
