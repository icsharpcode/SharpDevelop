// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethodElement
	{
		XElement element;

		private class branchOffset {
			public int Offset;
			public int Visit;
			public int Count;
			public branchOffset( int offset ) {
				this.Offset = offset;
			}
		}

		public CodeCoverageMethodElement(XElement element)
		{
			this.element = element;
			this.SequencePoints = new List<CodeCoverageSequencePoint>();
			this.BranchPoints = new List<CodeCoverageBranchPoint>();
			Init();
		}
		
		public string FileRef { get; private set; }
		public bool IsVisited { get; private set; }
		public int CyclomaticComplexity { get; private set; }
		public decimal SequenceCoverage { get; private set; }
		public int SequencePointsCount { get; private set; }
		public decimal BranchCoverage { get; private set; }
		public Tuple<int,int> BranchCoverageRatio { get; private set; }
		public bool IsConstructor { get; private set; }
		public bool IsStatic { get; private set; }
		public List<CodeCoverageSequencePoint> SequencePoints { get; private set; }
		public List<CodeCoverageBranchPoint> BranchPoints { get; private set; }

		public bool IsGetter { get; private set; }
		public bool IsSetter { get; private set; }
		public string MethodName { get; private set; }
		
		public bool IsProperty {
			get { return IsGetter || IsSetter; }
		}
		
		void Init()
		{
			MethodName = GetMethodName();
			IsGetter = GetBooleanAttributeValue("isGetter");
			IsSetter = GetBooleanAttributeValue("isSetter");

			this.FileRef = GetFileRef();
			this.IsVisited = this.GetBooleanAttributeValue("visited");
			this.CyclomaticComplexity = (int)this.GetDecimalAttributeValue("cyclomaticComplexity");
			this.SequencePoints = this.GetSequencePoints();
			this.SequencePointsCount = this.SequencePoints.Count;
			//this.SequencePointsCount = this.GetSequencePointsCount();
			this.SequenceCoverage = (int)this.GetDecimalAttributeValue("sequenceCoverage");
			this.BranchPoints = this.GetBranchPoints();
			this.BranchCoverageRatio = this.GetBranchRatio();
			this.BranchCoverage = this.GetBranchCoverage();
			this.IsConstructor = this.GetBooleanAttributeValue("isConstructor");
			this.IsStatic = this.GetBooleanAttributeValue("isStatic");

		}
		
		List<CodeCoverageSequencePoint> GetSequencePoints() {
			// get all SequencePoints
			List<CodeCoverageSequencePoint> sps = new List<CodeCoverageSequencePoint>();
			var xSPoints = this.element			
				.Elements("SequencePoints")
				.Elements("SequencePoint");
			foreach (XElement xSPoint in xSPoints) {
				CodeCoverageSequencePoint sp = new CodeCoverageSequencePoint();
				sp.FileRef = this.FileRef;
				sp.Line = (int)GetDecimalAttributeValue(xSPoint.Attribute("sl"));
				sp.EndLine = (int)GetDecimalAttributeValue(xSPoint.Attribute("el"));
				sp.Column = (int)GetDecimalAttributeValue(xSPoint.Attribute("sc"));
				sp.EndColumn = (int)GetDecimalAttributeValue(xSPoint.Attribute("ec"));
				sp.VisitCount = (int)GetDecimalAttributeValue(xSPoint.Attribute("vc"));
				sp.Length = 1;
				sp.BranchCovered = true;
				sps.Add(sp);
			}
			return sps;
		}

		int GetSequencePointsCount() {
			XElement summary = this.element.Element("Summary");
			if ( summary != null ) {
				XAttribute nsp = summary.Attribute("numSequencePoints");
				if ( nsp != null ) {
					return (int)GetDecimalAttributeValue( nsp );
				}
			}
			return 0;
		}

		List<CodeCoverageBranchPoint> GetBranchPoints() {
			// get all SequencePoints
			List<CodeCoverageBranchPoint> bps = new List<CodeCoverageBranchPoint>();
			var xBPoints = this.element			
				.Elements("BranchPoints")
				.Elements("BranchPoint");
			foreach (XElement xBPoint in xBPoints) {
				CodeCoverageBranchPoint bp = new CodeCoverageBranchPoint();
				bp.VisitCount = (int)GetDecimalAttributeValue(xBPoint.Attribute("vc"));
				bp.Offset = (int)GetDecimalAttributeValue(xBPoint.Attribute("offset"));
				bp.Path = (int)GetDecimalAttributeValue(xBPoint.Attribute("path"));
				bps.Add(bp);
			}
			return bps;
		}

		Tuple<int,int> GetBranchRatio () {

			// goal: Get branch ratio and exclude (rewriten) Code Contracts branches 

			if ( this.BranchPoints == null || this.BranchPoints.Count() == 0 ) {
				return null;
			}

			// start & final method sequence points line and offset
			int methodStartLine = 0;
			int methodStartOffset = 0;

			int methodFinalLine = 0;
			int methodFinalOffset = 0;

			// find start & final method sequence points and offsets
			bool nextMatch = false;
			int startLine = 0;
			int finalLine = 0;
			int startChar = 0;
			int finalChar = 0;

			// Find method body SequencePoint ({)
			// and then first next sequencePoint (and offset of first instruction in body)
			// This will skip Contract.Require SequencePoints 
			// and help to filter-out Contract.Require BranchPoint's
			foreach (CodeCoverageSequencePoint sp in this.SequencePoints) {
				startLine = sp.Line;
				finalLine = sp.EndLine;
				startChar = sp.Column;
				finalChar = sp.EndColumn;
				if ( nextMatch ) {
					methodStartLine = startLine;
					methodStartOffset = sp.Offset;
					break;
				}
				if ( startLine==finalLine && (finalChar-startChar)==1 ) {
					nextMatch = true;
				}
			}

			// find method body last SequencePoint and final offset (})
			// This will skip Contract.Ensures SequencePoints 
			// and help to filter-out Contract.Ensures BranchPoint's
			foreach (CodeCoverageSequencePoint sp in Enumerable.Reverse(this.SequencePoints)) {
				startLine = sp.Line;
				finalLine = sp.EndLine;
				startChar = sp.Column;
				finalChar = sp.EndColumn;
				if ( startLine==finalLine && (finalChar-startChar)==1 ) {
					methodFinalLine = finalLine;
					methodFinalOffset = sp.Offset;
					break;
				}
			}
			
			// Find compiler inserted code (=> inserted hidden branches)
			// SequencePoints are ordered by offset and that order exposes reverse ordered lines
			// ie: In code: foreach ( var item in items ) 
			// Keyword "in" code is generated after loop with lower line number than previous line (reversed line order).
			// That "in" code contains hidden branches that we 
			// do not want or cannot cover by test-case because is handled by earlier line
			// ie: Possible NullReferenceException in foreach loop is pre-handled at method entry, ie. by Contract.Require(items!=null)
			nextMatch = false;
			int lastOffset = 0;
			int lastLine = methodStartLine;
			List<Tuple<int,int>> excludeList = new List<Tuple<int, int>>();
			foreach (CodeCoverageSequencePoint sp in this.SequencePoints) {
				if ( (sp.Line < methodStartLine) || methodFinalLine < sp.Line ) { continue ; }
				
				if (nextMatch && (sp.Line > lastLine)) {
					nextMatch = false;
					excludeList.Add(new Tuple<int, int> ( lastOffset , sp.Offset ));
				}
				// reversed line number?
				if (!nextMatch && (sp.Line < lastLine)) {
					nextMatch = true;
					lastOffset = sp.Offset;
				}
				lastLine = sp.Line;
			}

			// Collect branch offsets within method boundary { } (exclude Contracts)
			// and filter with excludeList
			Dictionary<int, branchOffset> branchDictionary = new Dictionary<int, branchOffset>();
			foreach (CodeCoverageBranchPoint bp in this.BranchPoints) {

				if ( methodStartOffset <= bp.Offset && bp.Offset <= methodFinalOffset ) {

					// Apply exclude BranchPoint filter
					nextMatch = true;
					foreach (var range in excludeList) {
						if (range.Item1 < bp.Offset  && bp.Offset < range.Item2) {
							// exclude range match
							nextMatch = false; break;
						}
					} if (!nextMatch) { continue; }

					// update/insert branch offset coverage data
					if ( branchDictionary.ContainsKey( bp.Offset ) ) {
						branchOffset update = branchDictionary[bp.Offset];
						update.Visit += bp.VisitCount!=0?1:0;
						update.Count += 1;
					} else {
						// Insert first branch at offset
						branchOffset insert = new branchOffset(bp.Offset);
						insert.Visit = bp.VisitCount!=0?1:0;
						insert.Count = 1;
						branchDictionary[insert.Offset] = insert;
					}
				}
			}
			
			int totalBranchVisit = 0;
			int totalBranchCount = 0;
			CodeCoverageSequencePoint sp_target = null;
			// Branch percentage will display only if code SequencePoints coverage is 100%, so ...
			// Do not add branch if branch is not visited at all because ... :
			// If "branch" is completely unvisited and 100% SequencePoints covered, 
			// then that "branch" does not really exists in SOURCE code we try to cover
			foreach ( branchOffset uniqueBranch in branchDictionary.Values ) {
				if ( uniqueBranch.Visit != 0 ) {
					totalBranchVisit += uniqueBranch.Visit;
					totalBranchCount += uniqueBranch.Count;

					// not full branch coverage?
					if ( uniqueBranch.Visit != uniqueBranch.Count ) {
						// update matching sequence point branch covered
						sp_target = null;
						foreach ( CodeCoverageSequencePoint sp in this.SequencePoints ) {
							if ( sp.Offset > uniqueBranch.Offset ) {
								if ( !Object.ReferenceEquals( sp_target, null ) ) {
									sp_target.BranchCovered = false;
								}
								break;
							}
							sp_target = sp;
						}
					}
				}
			}

			return (totalBranchCount!=0) ? new Tuple<int,int>(totalBranchVisit,totalBranchCount) : null;
			
		}

		decimal GetBranchCoverage () {
			
			return this.BranchCoverageRatio != null ? decimal.Round( ((decimal)(this.BranchCoverageRatio.Item1*100))/(decimal)this.BranchCoverageRatio.Item2, 2) : 0m;
			
		}

		decimal GetDecimalAttributeValue(string name)
		{
			return GetDecimalAttributeValue(element.Attribute(name));
		}
		
		decimal GetDecimalAttributeValue(XAttribute attribute)
		{
			if (attribute != null) {
				decimal value = 0;
				if (Decimal.TryParse(attribute.Value, out value)) {
					return value;
				}
			}
			return 0;
		}
		
		bool GetBooleanAttributeValue(string name)
		{
			return GetBooleanAttributeValue(element.Attribute(name));
		}
		
		bool GetBooleanAttributeValue(XAttribute attribute)
		{
			if (attribute != null) {
				bool value = false;
				if (Boolean.TryParse(attribute.Value, out value)) {
					return value;
				}
			}
			return false;
		}

		string GetFileRef() {
			XElement fileId = element.Element("FileRef");
			if (fileId != null) {
				return fileId.Attribute("uid").Value;
			}
			return String.Empty;
		}

		string GetMethodName()
		{
			XElement nameElement = element.Element("Name");
			if (nameElement != null) {
				return GetMethodName(nameElement.Value);
			}
			return String.Empty;
		}
		
		string GetMethodName(string methodSignature)
		{
			int startIndex = methodSignature.IndexOf("::");
			int endIndex = methodSignature.IndexOf('(', startIndex);
			return methodSignature
				.Substring(startIndex, endIndex - startIndex)
				.Substring(2);
		}
	}
}
