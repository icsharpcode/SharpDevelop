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

		private class branch {
			public int Visit;
			public int Count;
		}

		public CodeCoverageMethodElement(XElement element)
		{
			this.element = element;
			Init();
		}
		
		public bool IsVisited { get; private set; }
		public int CyclomaticComplexity { get; private set; }
		public decimal SequenceCoverage { get; private set; }
		public int SequencePointsCount { get; private set; }
		public decimal BranchCoverage { get; private set; }
		public Tuple<int,int> BranchCoverageRatio { get; private set; }
		public bool IsConstructor { get; private set; }
		public bool IsStatic { get; private set; }

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

			this.IsVisited = this.GetBooleanAttributeValue("visited");
			this.CyclomaticComplexity = (int)this.GetDecimalAttributeValue("cyclomaticComplexity");
			this.SequencePointsCount = this.GetSequencePointsCount();
			this.SequenceCoverage = this.GetDecimalAttributeValue("sequenceCoverage");
			this.BranchCoverageRatio = this.GetBranchRatio();
			this.BranchCoverage = this.GetBranchCoverage();
			this.IsConstructor = this.GetBooleanAttributeValue("isConstructor");
			this.IsStatic = this.GetBooleanAttributeValue("isStatic");

		}

		int GetSequencePointsCount() {
			XElement summary = this.element.Element("Summary");
			if ( summary != null ) {
				XAttribute nsp = summary.Attribute("numSequencePoints");
				if ( nsp != null ) {
					return (int)this.GetDecimalAttributeValue( nsp );
				}
			}
			return 0;
		}

		Tuple<int,int> GetBranchRatio () {

			// goal: Get branch ratio and exclude (rewriten) Code Contracts branches 

			// get all BranchPoints
			var bPoints = this.element
				.Elements("BranchPoints")
				.Elements("BranchPoint");

			if ( bPoints == null || bPoints.Count() == 0 ) {
				return null;
			}

			// get all SequencePoints
			var sPoints = this.element			
				.Elements("SequencePoints")
				.Elements("SequencePoint");
			
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
			foreach (XElement sPoint in sPoints) {
				startLine = (int)GetDecimalAttributeValue(sPoint.Attribute("sl"));
				finalLine = (int)GetDecimalAttributeValue(sPoint.Attribute("el"));
				startChar = (int)GetDecimalAttributeValue(sPoint.Attribute("sc"));
				finalChar = (int)GetDecimalAttributeValue(sPoint.Attribute("ec"));
				if ( nextMatch ) {
					methodStartLine = startLine;
					methodStartOffset = (int)GetDecimalAttributeValue(sPoint.Attribute("offset"));
					break;
				}
				if ( startLine==finalLine && (finalChar-startChar)==1 ) {
					nextMatch = true;
				}
			}

			// find method body last SequencePoint and final offset (})
			// This will skip Contract.Ensures SequencePoints 
			// and help to filter-out Contract.Ensures BranchPoint's
			foreach (XElement sPoint in sPoints.Reverse()) {
				startLine = (int)GetDecimalAttributeValue(sPoint.Attribute("sl"));
				finalLine = (int)GetDecimalAttributeValue(sPoint.Attribute("el"));
				startChar = (int)GetDecimalAttributeValue(sPoint.Attribute("sc"));
				finalChar = (int)GetDecimalAttributeValue(sPoint.Attribute("ec"));
				if ( startLine==finalLine && (finalChar-startChar)==1 ) {
					methodFinalLine = finalLine;
					methodFinalOffset = (int)GetDecimalAttributeValue(sPoint.Attribute("offset"));
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
			int currLine = 0;
			List<Tuple<int,int>> excludeList = new List<Tuple<int, int>>();
			foreach (XElement sPoint in sPoints) {
				currLine = (int)GetDecimalAttributeValue(sPoint.Attribute("sl"));
				if ( (currLine < methodStartLine) || currLine > methodFinalLine ) { continue ; }
				
				if (nextMatch && (currLine > lastLine)) {
					nextMatch = false;
					excludeList.Add(new Tuple<int, int> ( lastOffset , (int)GetDecimalAttributeValue(sPoint.Attribute("offset"))));
				}
				// reversed line number?
				if (!nextMatch && (currLine < lastLine)) {
					nextMatch = true;
					lastOffset = (int)GetDecimalAttributeValue(sPoint.Attribute("offset"));
				}
				lastLine = currLine;
			}

			int visited = 0;
			int offset = 0;

			// collect all branch offsets
			Dictionary<int, branch> branches = new Dictionary<int, branch>();
			foreach (XElement bPoint in bPoints) {
				visited = (int)GetDecimalAttributeValue(bPoint.Attribute("vc"));
				offset =  (int)GetDecimalAttributeValue(bPoint.Attribute("offset"));
				if ( offset > methodStartOffset && offset < methodFinalOffset ) {

					// Apply exclude BranchPoint filter
					nextMatch = true;
					foreach (var range in excludeList) {
						if (offset > range.Item1 && offset < range.Item2) {
							nextMatch = false; break;
						}
					} if (!nextMatch) { continue; }

					// Add/insert coverage data
					if ( branches.ContainsKey( offset ) ) {
						branch update = branches[offset];
						update.Visit += visited!=0?1:0;
						update.Count += 1;
					} else {
						// Insert first branch
						branches[offset] = new branch{Visit=visited!=0?1:0,Count=1};
					}
				}
			}
			
			int totalVisit = 0;
			int totalCount = 0;
			// Branch percentage will display only if code SequencePoints coverage is 100%, so ...
			// Do not add branch if branch is not visited at all because ... :
			// If "branch" is completely unvisited and 100% SequencePoints covered, 
			// then that "branch" does not really exists in SOURCE code we try to cover
			foreach ( branch item in branches.Values ) {
				if ( item.Visit != 0 ) {
					totalVisit += item.Visit;
					totalCount += item.Count;
				}
			}

			return (totalCount!=0) ? new Tuple<int,int>(totalVisit,totalCount) : null;
			
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
