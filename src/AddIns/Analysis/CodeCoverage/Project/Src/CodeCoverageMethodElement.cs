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

		public CodeCoverageMethodElement(XElement element) 
		    : this (element, null) {}
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
		public CodeCoverageSequencePoint BodyStartSP { get; private set; }
		public CodeCoverageSequencePoint BodyFinalSP { get; private set; }
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
			    if (parent != null) {
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
			}
			
			this.IsVisited = this.GetBooleanAttributeValue("visited");
			this.CyclomaticComplexity = (int)this.GetDecimalAttributeValue("cyclomaticComplexity");
			this.SequencePointsCount = this.GetSequencePointsCount();
			this.SequenceCoverage = (int)this.GetDecimalAttributeValue("sequenceCoverage");
			this.IsConstructor = this.GetBooleanAttributeValue("isConstructor");
			this.IsStatic = this.GetBooleanAttributeValue("isStatic");
			if ( !String.IsNullOrEmpty( this.FileID ) ) {
				this.SequencePoints = this.GetSequencePoints();
				this.BodyStartSP = getBodyStartSP(); // before OrderBy Line/Col
				// SP's are originaly ordered by CIL offset
				// but ccrewrite can move offset of
				//   Contract.Requires before method signature SP { and
				//   Contract.Ensures after method closing SP }
				// So sort SP's back by line/column
				this.SequencePoints.OrderBy(item => item.Line).OrderBy(item => item.Column);
				this.BodyFinalSP = getBodyFinalSP(); // after orderBy Line/Col
				this.SequencePoints = this.FilterSequencePoints(this.SequencePoints);
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
	
		// Find method-body start SequencePoint "{"
		// Sequence points expected to be ordered by Offset
		// Cannot just get first one because of ccrewrite&ContractClassFor
		public CodeCoverageSequencePoint getBodyStartSP() {
			bool startPointFound = false;
			CodeCoverageSequencePoint startSeqPoint = null;
			foreach (CodeCoverageSequencePoint sPoint in this.SequencePoints) {
				if ( sPoint.Content == "{") {
					if ( this.IsConstructor ) {
						// take previous/last one if not null
						startSeqPoint = startSeqPoint?? sPoint;
					}
					else {
						startSeqPoint = sPoint;
					}
					startPointFound = true;
					break;
				}
				startSeqPoint = sPoint;
			}
			return startPointFound? startSeqPoint: null;
		}

		// Find method-body final SequencePoint "}"
		// Sequence points expected to be ordered by Line/Column
		public CodeCoverageSequencePoint getBodyFinalSP() {
			CodeCoverageSequencePoint finalSeqPoint = null;
			foreach (CodeCoverageSequencePoint sp in this.SequencePoints) {
				if ( sp.Content == "}") {
					if (finalSeqPoint == null) {
						finalSeqPoint = sp;
					}
					// check for ccrewrite duplicate
					else if (sp.Line == finalSeqPoint.Line &&
							 sp.Column == finalSeqPoint.Column &&
							 sp.EndLine == finalSeqPoint.EndLine &&
							 sp.EndColumn == finalSeqPoint.EndColumn &&
							 sp.Offset < finalSeqPoint.Offset) {
						finalSeqPoint = sp;
					}
					else if (sp.Line < finalSeqPoint.Line) {
						break;
					}
				}
			}
			return finalSeqPoint;
		}
		
		List<CodeCoverageSequencePoint> FilterSequencePoints(List<CodeCoverageSequencePoint> sps) {

			List<CodeCoverageSequencePoint> returnList = sps;

			if (sps.Count > 2 &&
				this.BodyStartSP != null &&
				this.BodyFinalSP != null ) {
				
				// After ccrewrite ContractClass/ContractClassFor
				// sequence point(s) from another file/class/method
				// is inserted into this method sequence points
				//
				// To remove alien sequence points, all sequence points on lines
				// before method signature and after end-brackets xxx{} are removed
				// If ContractClassFor is in another file but interleaves this method lines
				// then, afaik, not much can be done to remove inserted alien SP's
				List<CodeCoverageSequencePoint> selected = new List<CodeCoverageSequencePoint>();

				foreach (var point in sps) {
					if (
						(point.Line > BodyStartSP.Line || (point.Line == BodyStartSP.Line && point.Column >= BodyStartSP.Column)) &&
						(point.Line < BodyFinalSP.Line || (point.Line == BodyFinalSP.Line && point.Column < BodyFinalSP.Column))
					   ) {
						selected.Add (point);
					}
					// After ccrewrite ContractClass/ContractClassFor
					// duplicate method end-sequence-point (}) is added
					//
					// Add first finalSP (can be a duplicate)
					// Note: IL.Offset of second duplicate finalSP will
					// extend branch coverage outside method-end "}",
					// and that can lead to wrong branch coverage display!
					if (object.ReferenceEquals (point, this.BodyFinalSP)) {
						   selected.Add (point);
					}
				}

				returnList = selected;
			}

			return returnList;
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
				bp.OffsetEnd = (int)GetDecimalAttributeValue(xBPoint.Attribute("offsetend"));
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

			// This sequence point offset is used to skip CCRewrite(n) BranchPoint's (Requires)
			// and '{' branches at static methods
			if (this.BodyStartSP == null) { return null; } // empty body

			// This sequence point offset is used to skip CCRewrite(n) BranchPoint's (Ensures)
			if (this.BodyFinalSP == null) { return null; } // empty body
			
			// Connect Sequence & Branches
			IEnumerator<CodeCoverageSequencePoint> SPEnumerator = this.SequencePoints.GetEnumerator();
			CodeCoverageSequencePoint currSeqPoint = BodyStartSP;
			int nextSeqPointOffset = BodyStartSP.Offset;
			
			foreach (var bp in this.BranchPoints) {
				
				// ignore branches outside of method body offset range
				if (bp.Offset < BodyStartSP.Offset)
					continue;
				if (bp.Offset > BodyFinalSP.Offset)
					break;

				// Sync with SequencePoint
				while ( nextSeqPointOffset < bp.Offset ) {
					currSeqPoint = SPEnumerator.Current;
					if ( SPEnumerator.MoveNext() ) {
						nextSeqPointOffset = SPEnumerator.Current.Offset;
					} else {
						nextSeqPointOffset = int.MaxValue;
					}
				}
				if (currSeqPoint.Branches == null) {
					currSeqPoint.Branches = new List<CodeCoverageBranchPoint>();
				}
				// Add Branch to Branches
				currSeqPoint.Branches.Add(bp);
			}

			// Merge sp.Branches on exit-offset
			// Calculate Method Branch coverage
			int totalBranchVisit = 0;
			int totalBranchCount = 0;
			int pointBranchVisit = 0;
			int pointBranchCount = 0;
			Dictionary<int, CodeCoverageBranchPoint> bpExits = new Dictionary<int, CodeCoverageBranchPoint>();
			foreach (var sp in this.SequencePoints) {

				// SequencePoint covered & has branches?
				if (sp.VisitCount != 0 && sp.Branches != null) {

					// 1) Generated "in" code for IEnumerables contains hidden "try/catch/finally" branches that
					// one do not want or cannot cover by test-case because is handled earlier at same method.
					// ie: NullReferenceException in foreach loop is pre-handled at method entry, ie. by Contract.Require(items!=null)
					// 2) Branches within sequence points "{" and "}" are not source branches but compiler generated branches
					// ie: static methods start sequence point "{" contains compiler generated branches
					// 3) Exclude Contract class (EnsuresOnThrow/Assert/Assume is inside method body)
					// 4) Exclude NUnit Assert(.Throws) class
					if (sp.Content == "in" || sp.Content == "{" || sp.Content == "}" ||
						sp.Content.StartsWith("Assert.") ||
						sp.Content.StartsWith("Assert ") ||
						sp.Content.StartsWith("Contract.") ||
						sp.Content.StartsWith("Contract ")
					   ) {
						sp.Branches = null;
						continue; // skip
					}

					// Merge sp.Branches on OffsetEnd using bpExits key
					bpExits.Clear();
					foreach (var bp in sp.Branches) {
						if (!bpExits.ContainsKey(bp.OffsetEnd)) {
							bpExits[bp.OffsetEnd] = bp; // insert branch
						} else {
							bpExits[bp.OffsetEnd].VisitCount += bp.VisitCount; // update branch
						}
					}

					// Compute branch coverage
					pointBranchVisit = 0;
					pointBranchCount = 0;
					foreach (var bp in bpExits.Values) {
						pointBranchVisit += bp.VisitCount == 0? 0 : 1 ;
						pointBranchCount += 1;
					}
					// Not full coverage?
					if (pointBranchVisit != pointBranchCount) {
						   sp.BranchCoverage = false; // => part-covered
					}
					totalBranchVisit += pointBranchVisit;
					totalBranchCount += pointBranchCount;
				}
				if (sp.Branches != null)
					sp.Branches = null; // release memory
			}

			return (totalBranchCount!=0) ? new Tuple<int,int>(totalBranchVisit,totalBranchCount) : null;
			
		}

		decimal GetBranchCoverage () {
			
			return this.BranchCoverageRatio == null ? 0m : ((decimal)(this.BranchCoverageRatio.Item1*100))/((decimal)this.BranchCoverageRatio.Item2);
			
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
