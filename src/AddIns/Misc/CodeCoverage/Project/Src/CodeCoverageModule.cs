// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageModule
	{
		string name = String.Empty;
		List<CodeCoverageMethod> methods = new List<CodeCoverageMethod>();
		List<string> rootNamespaces;
		
		public CodeCoverageModule(string name)
		{
			this.name = name;
		}
		
		public bool IsExcluded {
			get {
				int excludedMethods = 0;
				foreach (CodeCoverageMethod method in methods) {
					if (method.IsExcluded) {
						++excludedMethods;
					}
				}
				return excludedMethods == methods.Count;
			}
		}
		
		/// <summary>
		/// The module's assembly name.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		public List<CodeCoverageMethod> Methods {
			get {
				return methods;
			}
		}
		
		public int VisitedSequencePointsCount {
			get {
				int count = 0;
				foreach (CodeCoverageMethod method in methods) {
					count += method.VisitedSequencePointsCount;
				}
				return count;
			}
		}
		
		public int NotVisitedSequencePointsCount {
			get {
				int count = 0;
				foreach (CodeCoverageMethod method in methods) {
					count += method.NotVisitedSequencePointsCount;
				}
				return count;
			}
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
