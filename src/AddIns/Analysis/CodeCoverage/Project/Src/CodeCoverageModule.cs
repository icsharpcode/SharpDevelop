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
		
		public decimal GetVisitedBranchCoverage() {
			decimal total = 0;
			int count = 0;
			foreach (CodeCoverageMethod method in methods) {
				if (method.IsVisited) {
					++count;
					total += method.BranchCoverage == 0 ? 100 : method.BranchCoverage ;
				}
			}
			if (count!=0) {
				return total/count;
			}
			return 0;
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
