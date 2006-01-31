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
		
		public CodeCoverageModule(string name)
		{
			this.name = name;
		}
		
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
	}
}
