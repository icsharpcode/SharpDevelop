// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethodElement
	{
		XElement element;
		CodeCoverageResults parent;

		private class branchOffset {
			public int Offset;
			public int Visit;
			public int Count;
			public branchOffset( int offset ) {
				this.Offset = offset;
			}
		}

		public CodeCoverageMethodElement(XElement element, CodeCoverageResults parent)
		{
			this.parent = parent;
			this.element = element;
			this.SequencePoints = new List<CodeCoverageSequencePoint>();
			this.BranchPoints = new List<CodeCoverageBranchPoint>();
			Init();
		}
		private static string cacheFileName = String.Empty;
		private static CodeCoverageStringTextSource cacheDocument = null;

		public string FileID { get; private set; }
		public string FileName { get; private set; }
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

			this.FileID = GetFileRef();
			this.FileName = String.Empty;
			if (!String.IsNullOrEmpty(this.FileID)) {
				this.FileName = parent.GetFileName(this.FileID);
				if ( File.Exists(this.FileName) ) {
					if (cacheFileName != this.FileName) {
						cacheFileName = this.FileName;
						cacheDocument = null;
						try {
							using (Stream stream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read)) {
								try {
									stream.Position = 0;
									string textSource = ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(stream, Encoding.Default);
									cacheDocument = new CodeCoverageStringTextSource(textSource);
								} catch {}
							}
						} catch {}
					}
				}
			}
			
			this.IsVisited = this.GetBooleanAttributeValue("visited");
			this.CyclomaticComplexity = (int)this.GetDecimalAttributeValue("cyclomaticComplexity");
			this.SequencePointsCount = this.GetSequencePointsCount();
			this.SequenceCoverage = (int)this.GetDecimalAttributeValue("sequenceCoverage");
			this.IsConstructor = this.GetBooleanAttributeValue("isConstructor");
			this.IsStatic = this.GetBooleanAttributeValue("isStatic");
			if ( !String.IsNullOrEmpty( this.FileID ) ) {
				this.SequencePoints = this.GetSequencePoints();
				this.BranchPoints = this.GetBranchPoints();
				this.BranchCoverageRatio = this.GetBranchRatio();
				this.BranchCoverage = this.GetBranchCoverage();
			}
		}
		
		List<CodeCoverageSequencePoint> GetSequencePoints() {

			List<CodeCoverageSequencePoint> sps = new List<CodeCoverageSequencePoint>();
			var xSPoints = this.element			
				.Elements("SequencePoints")
				.Elements("SequencePoint");

			foreach (XElement xSPoint in xSPoints) {
				CodeCoverageSequencePoint sp = new CodeCoverageSequencePoint();
				sp.FileID = this.FileID;
				sp.Document = this.FileName;
				sp.Line = (int)GetDecimalAttributeValue(xSPoint.Attribute("sl"));
				sp.EndLine = (int)GetDecimalAttributeValue(xSPoint.Attribute("el"));
				sp.Column = (int)GetDecimalAttributeValue(xSPoint.Attribute("sc"));
				sp.EndColumn = (int)GetDecimalAttributeValue(xSPoint.Attribute("ec"));
				sp.VisitCount = (int)GetDecimalAttributeValue(xSPoint.Attribute("vc"));
				if (cacheFileName == sp.Document && cacheDocument != null) {
					sp.Content = cacheDocument.GetText(sp);
					if (sp.Line != sp.EndLine) {
					    sp.Content = Regex.Replace (sp.Content, @"\s+", " ");
					}
    				sp.Length = Regex.Replace (sp.Content, @"\s", "").Length; // ignore white-space for coverage%
				} else {
					sp.Content = String.Empty;
    				sp.Length = 0;
				}
				sp.Offset = (int)GetDecimalAttributeValue(xSPoint.Attribute("offset"));
				sp.BranchCoverage = true;

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
			// get all BranchPoints
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

			if ( this.BranchPoints == null 
			    || this.BranchPoints.Count() == 0 
			    || this.SequencePoints == null
			    || this.SequencePoints.Count == 0
			   )
			{
				return null;
			}

			// Find method-body first SequencePoint "{"
			// and then first next sequencePoint (and offset of that instruction in body)
			// This will be used to skip CCRewrite(n) BranchPoint's (Requires)
			CodeCoverageSequencePoint startSeqPoint = null;
			foreach (CodeCoverageSequencePoint sp in this.SequencePoints) {
				if ( sp.Content == "{") {
					startSeqPoint = sp;
					break;
				}
			}
			Debug.Assert (!Object.ReferenceEquals(null, startSeqPoint));
			if (Object.ReferenceEquals(null, startSeqPoint)) { return null; }

			// Find method-body last SequencePoint "}" and offset 
			// This will be used to skip CCRewrite(n) BranchPoint's (Ensures)
			CodeCoverageSequencePoint finalSeqPoint = null;
			foreach (CodeCoverageSequencePoint sp in Enumerable.Reverse(this.SequencePoints)) {
				if ( sp.Content == "}") {
					finalSeqPoint = sp;
					break;
				}
			}
			Debug.Assert ( !Object.ReferenceEquals( null, finalSeqPoint) );
			if (Object.ReferenceEquals(null, finalSeqPoint)) { return null; }
			
			// Create&populate excludeOffsetList (BranchPoint offset-filter)
			bool nextMatch = false;
			CodeCoverageSequencePoint previousSeqPoint = startSeqPoint;
			List<Tuple<int,int>> excludeOffsetList = new List<Tuple<int, int>>();
			foreach (CodeCoverageSequencePoint currentSeqPoint in this.SequencePoints) {

				// ignore CCRewrite(n) contracts
				if (currentSeqPoint.Offset < startSeqPoint.Offset)
					continue;
				if (currentSeqPoint.Offset > finalSeqPoint.Offset)
					break;
				
				if (nextMatch) {
					nextMatch = false;
					excludeOffsetList.Add(new Tuple<int, int> ( previousSeqPoint.Offset , currentSeqPoint.Offset ));
				}
				// Generated "in" code for IEnumerables contains hidden "try/catch/finally" branches that
				// one do not want or cannot cover by test-case because is handled earlier at same method.
				// ie: NullReferenceException in foreach loop is pre-handled at method entry, ie. by Contract.Require(items!=null)
				if (currentSeqPoint.Content == "in") {
					// Content is equal to "in" keyword
					nextMatch = true;
				}
				previousSeqPoint = currentSeqPoint;
			}

			// Merge BranchPoints on same offset 
			// Exclude BranchPoints outside of method boundary {...} => exclude CCRewrite(n) Contracts
			// Exclude BranchPoints from excludeOffsetList
			Dictionary<int, branchOffset> branchDictionary = new Dictionary<int, branchOffset>();
			foreach (CodeCoverageBranchPoint bp in this.BranchPoints) {

				// exclude CCRewrite(n) contracts
				if (bp.Offset < startSeqPoint.Offset)
					continue;
				if (bp.Offset > finalSeqPoint.Offset)
					break;

				// Apply excludeOffset filter
				if (excludeOffsetList.Count != 0) {
					nextMatch = true;
					foreach (var offsetRange in excludeOffsetList) {
						if (offsetRange.Item1 < bp.Offset  && bp.Offset < offsetRange.Item2) {
							// exclude range match
							nextMatch = false; break;
						}
					} if (!nextMatch) { continue; }
				}

				// merge BranchPoint's with same offset
				if ( branchDictionary.ContainsKey( bp.Offset ) ) {
					// Update BranchPoint coverage at offset
					branchOffset update = branchDictionary[bp.Offset];
					update.Visit += bp.VisitCount!=0?1:0;
					update.Count += 1;
				} else {
					// Insert BranchPoint coverage at offset
					branchOffset insert = new branchOffset(bp.Offset);
					insert.Visit = bp.VisitCount!=0?1:0;
					insert.Count = 1;
					branchDictionary[insert.Offset] = insert;
				}
			}
			
			int totalBranchVisit = 0;
			int totalBranchCount = 0;

			// Branch coverage will display only if sequence coverage is 100%, so ...
			// Ignore branch-offset if is not visited at all because ... :
			// if SequencePoint is covered and branch-offset within that SequencePoint not visited (uncovered), 
			// then that branch-offset does not really exists in SOURCE code we try to cover
			CodeCoverageSequencePoint sp_target = null;
			foreach ( branchOffset uniqueBranch in branchDictionary.Values ) {
				if ( uniqueBranch.Visit != 0 ) {
					totalBranchVisit += uniqueBranch.Visit;
					totalBranchCount += uniqueBranch.Count;

					// not full branch coverage?
					if ( uniqueBranch.Visit != uniqueBranch.Count ) {
						// update parent SequencePoint.BranchCoverage to false (== partial branch coverage)
						sp_target = null;
						foreach ( CodeCoverageSequencePoint sp_next in this.SequencePoints ) {
							if ( uniqueBranch.Offset < sp_next.Offset && sp_target != null ) {
								sp_target.BranchCoverage = false;
								break;
							}
							sp_target = sp_next;
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
