// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethod
	{
		string name = String.Empty;
		string className = String.Empty;
		string fullClassName = String.Empty;
		string classNamespace = String.Empty;
		List<CodeCoverageSequencePoint> sequencePoints = new List<CodeCoverageSequencePoint>();
		
		public CodeCoverageMethod(string name, string className)
		{
			this.name = name;
			this.fullClassName = className;
			
			int index = fullClassName.LastIndexOf('.');
			this.classNamespace = fullClassName.Substring(0, index);
			
			index = fullClassName.LastIndexOf('.');
			if (index > 0) {
				this.className = fullClassName.Substring(index + 1);
			} else {
				this.className = fullClassName;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string ClassName {
			get {
				return className;
			}
		}
		
		public string FullClassName {
			get {
				return fullClassName;
			}
		}
		
		public string ClassNamespace {
			get {
				return classNamespace;
			}
		}
		
		public List<CodeCoverageSequencePoint> SequencePoints {
			get {
				return sequencePoints;
			}
		}
		
		public int VisitedSequencePointsCount {
			get {
				int count = 0;
				foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
					if (sequencePoint.VisitCount > 0) {
						count++;
					}
				}
				return count;
			}
		}
		
		public int NotVisitedSequencePointsCount {
			get {
				int count = 0;
				foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
					if (sequencePoint.VisitCount == 0) {
						count++;
					}
				}
				return count;
			}
		}
		
		public List<CodeCoverageSequencePoint> GetSequencePoints(string fileName)
		{
			List<CodeCoverageSequencePoint> matchedSequencePoints = new List<CodeCoverageSequencePoint>();
			foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
				if (FileUtility.IsEqualFileName(fileName, sequencePoint.Document)) {
					matchedSequencePoints.Add(sequencePoint);
				}
			}
			return matchedSequencePoints;
		}
		
	}
}
