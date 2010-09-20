// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageModule : ICodeCoverageWithVisits
	{
		string name = String.Empty;
		List<CodeCoverageMethod> methods = new List<CodeCoverageMethod>();
		List<string> rootNamespaces;
		
		public CodeCoverageModule(string name)
		{
			this.name = name;
		}
		
		/// <summary>
		/// The module's assembly name.
		/// </summary>
		public string Name {
			get { return name; }
		}
		
		public List<CodeCoverageMethod> Methods {
			get { return methods; }
		}
		
		public int GetVisitedCodeLength()
		{
			int total = 0;
			foreach (CodeCoverageMethod method in methods) {
				total += method.GetVisitedCodeLength();
			}
			return total;
		}
				
		public int GetUnvisitedCodeLength()
		{
			int total = 0;
			foreach (CodeCoverageMethod method in methods) {
				total += method.GetUnvisitedCodeLength();
			}
			return total;
		}
		
		public List<CodeCoverageSequencePoint> GetSequencePoints(string fileName)
		{
			List<CodeCoverageSequencePoint> sequencePoints = new List<CodeCoverageSequencePoint>();
			foreach (CodeCoverageMethod method in methods) {
				sequencePoints.AddRange(method.GetSequencePoints(fileName));
			}
			return sequencePoints;
		}
		
		/// <summary>
		/// Gets the distinct root namespaces for all the methods.  
		/// </summary>
		/// <remarks>
		/// If one of the namespaces is 'ICSharpCode.XmlEditor' then this 
		/// method will return 'ICSharpCode' as one of the root namespaces.
		/// </remarks>
		public List<string> RootNamespaces {
			get {
				if (rootNamespaces == null) {
					GetRootNamespaces();
				}
				return rootNamespaces;
			}
		}
		
		void GetRootNamespaces()
		{
			rootNamespaces = new List<string>();
			foreach (CodeCoverageMethod method in methods) {
				if (method.RootNamespace.Length > 0 && !rootNamespaces.Contains(method.RootNamespace)) {
					rootNamespaces.Add(method.RootNamespace);
				}
			}
		}
	}
}
