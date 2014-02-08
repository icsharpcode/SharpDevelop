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

		private static string cache2FileName = String.Empty;
		private static CodeCoverageStringTextSource cache2Document = null;

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
		public CodeCoverageSequencePoint BodyStartSP { get; private set; }
		public CodeCoverageSequencePoint BodyFinalSP { get; private set; }

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
					if (cacheFileName != this.FileName) {
						cacheFileName = this.FileName;
						cacheDocument = GetSource (cacheFileName);
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
				this.GetSequencePoints();
				this.GetSequencePointsContent();
				this.getBodyStartSP(); // before OrderBy Line/Col
				this.getBodyFinalSP(); // before orderBy Line/Col
				//this.FilterSequencePoints(); // before orderBy Line/Col
				this.GetBranchRatio();
				this.GetBranchCoverage();

				// SP's are originaly ordered by CIL offset
				// but ccrewrite can move offset of
				//   Contract.Requires before method signature SP { and
				//   Contract.Ensures after method closing SP }
				// So sort SP's back by line/column
				this.SequencePoints.OrderBy(item => item.Line).OrderBy(item => item.Column);
			}
		}
		
		private static var cacheGetSource_LastFileName = (string)null;
		private static var cacheGetSource_LastSource = (CodeCoverageStringTextSource)null;

		static CodeCoverageStringTextSource GetSource(string filename) {

			if (filename == cacheGetSource_LastFileName) return cacheGetSource_LastSource;

			var retSource = (CodeCoverageStringTextSource)null;
			try {
				using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
					try {
						stream.Position = 0;
						string textSource = ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(stream, Encoding.Default);
						retSource = new CodeCoverageStringTextSource(textSource);
					} catch (Exception e) { Debug.Fail(e.Message); }
				}
			} catch (Exception e) { Debug.Fail(e.Message); }

			cacheGetSource_LastFileName = filename;
			cacheGetSource_LastSource = retSource;
			return retSource;
		}

		void GetSequencePoints() {

			var xSPoints = this.element
				.Elements("SequencePoints")
				.Elements("SequencePoint");

			foreach (XElement xSPoint in xSPoints) {
				var sp = new CodeCoverageSequencePoint();
				sp.Line = (int)GetDecimalAttributeValue(xSPoint.Attribute("sl"));
				sp.EndLine = (int)GetDecimalAttributeValue(xSPoint.Attribute("el"));
				sp.Column = (int)GetDecimalAttributeValue(xSPoint.Attribute("sc"));
				sp.EndColumn = (int)GetDecimalAttributeValue(xSPoint.Attribute("ec"));
				sp.VisitCount = (int)GetDecimalAttributeValue(xSPoint.Attribute("vc"));
				sp.Offset = (int)GetDecimalAttributeValue(xSPoint.Attribute("offset"));
				sp.BranchExitsCount = (int)GetDecimalAttributeValue(xSPoint.Attribute("bec"));
				sp.BranchExitsVisit = (int)GetDecimalAttributeValue(xSPoint.Attribute("bev"));
				sp.FileID = xSPoint.Attribute("fileid").Value?? "0";
				if (sp.FileID == "0") {
					// SequencePoint from not covered (not runnable) file
					// ie: interface with CodeContractClass/CodeContractClassFor
					sp.Document = xSPoint.Attribute("fileid").Value?? "";
				}
				else if (sp.FileID == this.FileID) {
					// This method SequencePoint (from this.FileName) 
					sp.Document = this.FileName;
				}
				else {
					// SequencePoint from another method/file
					// ie: ccrewriten CodeContractClass/CodeContractClassFor 
					// [or dependency-injected or fody-weaved???]
					sp.Document = parent.GetFileName(sp.FileID);
				}
				sp.BranchCoverage = (sp.BranchExitsCount == sp.BranchExitsVisit);
				sp.Content = String.Empty;
				sp.Length = 0;

				this.SequencePoints.Add(sp);
			}
		}
	
		void GetSequencePointsContent()
		{
			foreach (var sp in this.SequencePoints) {
				GetSequencePointContent(sp);
			}
		}

		void GetSequencePointContent(CodeCoverageSequencePoint sp)
		{
			if (cacheFileName == sp.Document) {
				// check primary cache (this.Filename)
				sp.Content = cacheDocument == null? "" : cacheDocument.GetText(sp);
			}
			else {
				// check & update secondary cache
				if (cache2FileName == sp.Document) {
					sp.Content = cache2Document == null? "" : cache2Document.GetText(sp);
				}
				else {
					cache2FileName = sp.Document;
					cache2Document = GetSource (cache2FileName);
					sp.Content = cache2Document == null? "" : cache2Document.GetText(sp);
				}
			}
			if (sp.Content != String.Empty) {
				if (sp.Line != sp.EndLine) {
					// merge multiple lines to single line
					sp.Content = Regex.Replace(sp.Content, @"\s+", " ");
				}
				// SequencePoint.Length counts all but whitespace
				sp.Length = Regex.Replace(sp.Content, @"\s", "").Length;
			}
		}

		// Find method-body start SequencePoint "{" (sp.Content required)
		// Sequence points expected to be ordered by Offset
		// Cannot just get first one because of ccrewrite&ContractClassFor
		void getBodyStartSP() {
			bool startPointFound = false;
			CodeCoverageSequencePoint startSeqPoint = null;
			foreach (CodeCoverageSequencePoint sp in this.SequencePoints) {
				if ( sp.Content == "{") {
					if ( this.IsConstructor ) {
						// take previous/last one if not null
						startSeqPoint = startSeqPoint?? sp;
					}
					else {
						startSeqPoint = sp;
					}
					startPointFound = true;
					break;
				}
				startSeqPoint = sp;
			}
			this.BodyStartSP = startPointFound? startSeqPoint: null;
		}

		// Find method-body final SequencePoint "}" (sp.Content required)
		// Sequence points expected to be ordered by Offset
		void getBodyFinalSP() {
			CodeCoverageSequencePoint finalSeqPoint = null;
			foreach (CodeCoverageSequencePoint sp in ((IEnumerable<CodeCoverageSequencePoint>)this.SequencePoints).Reverse()) {
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
						// duplicate found, so far no reason to expect "triplicate" :)
						break;
					}
				}
			}
			this.BodyFinalSP = finalSeqPoint;
		}
		
		void FilterSequencePoints() {

			if (this.SequencePoints.Count != 0 &&
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
				var selected = new List<CodeCoverageSequencePoint>();

				foreach (var point in this.SequencePoints) {
					
					// if Content.Length is 0, GetText() is failed by ccrewrite inserted invalid SequencePoint
					if (point.Content.Length != 0
						&& (point.Line > BodyStartSP.Line || (point.Line == BodyStartSP.Line && point.Column >= BodyStartSP.Column))
						&& (point.Line < BodyFinalSP.Line || (point.Line == BodyFinalSP.Line && point.Column < BodyFinalSP.Column))
					) {
						selected.Add (point);
					}
					// After ccrewrite ContractClass/ContractClassFor
					// duplicate method end-sequence-point "}" is added
					//
					// Add only first finalSP (can be a duplicate)
					// Note: IL.Offset of second duplicate finalSP will
					// extend branch coverage outside method-end "}",
					// and that can lead to wrong branch coverage display!
					if (object.ReferenceEquals (point, this.BodyFinalSP)) {
						selected.Add (point);
					}
				}

				this.SequencePoints = selected;
			}
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

		void GetBranchPoints() {
			// get all BranchPoints
			var xBPoints = this.element
				.Elements("BranchPoints")
				.Elements("BranchPoint");
			foreach (XElement xBPoint in xBPoints) {
				CodeCoverageBranchPoint bp = new CodeCoverageBranchPoint();
				bp.VisitCount = (int)GetDecimalAttributeValue(xBPoint.Attribute("vc"));
				bp.Offset = (int)GetDecimalAttributeValue(xBPoint.Attribute("offset"));
				bp.Path = (int)GetDecimalAttributeValue(xBPoint.Attribute("path"));
				bp.OffsetEnd = (int)GetDecimalAttributeValue(xBPoint.Attribute("offsetend"));
				this.BranchPoints.Add(bp);
			}
		}
		
		void GetBranchRatio () {

			this.BranchCoverageRatio = null;
			
			Debug.Assert (this.SequencePoints != null);
			if ( this.SequencePoints.Count == 0 ) return;

			// This sequence point offset is used to skip CCRewrite(n) BranchPoint's (Requires)
			// and '{' branches at static methods
			if (this.BodyStartSP == null) return; // empty body

			// This sequence point offset is used to skip CCRewrite(n) BranchPoint's (Ensures)
			if (this.BodyFinalSP == null) return; // empty body
			
			// Calculate Method Branch coverage
			int totalBranchVisit = 0;
			int totalBranchCount = 0;
			foreach (var sp in this.SequencePoints) {

				// SequencePoint is visited and belongs to this method?
				if (sp.VisitCount != 0 && sp.Document == this.FileName) {

					// Don't want branch coverage of ccrewrite(n)
					// SequencePoint's with offset before and after method body
					if (sp.Offset < BodyStartSP.Offset ||
						sp.Offset > BodyFinalSP.Offset) {
						sp.BranchCoverage = true;
						continue; // skip
					}

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
						sp.BranchCoverage = true;
						continue; // skip
					}

					totalBranchCount += sp.BranchExitsCount;
					totalBranchVisit += sp.BranchExitsVisit;
				}
			}

			this.BranchCoverageRatio = (totalBranchCount!=0) ? new Tuple<int,int>(totalBranchVisit,totalBranchCount) : null;
			
		}

		void GetBranchCoverage () {
			
			this.BranchCoverage = this.BranchCoverageRatio == null ? 0m : ((decimal)(this.BranchCoverageRatio.Item1*100))/((decimal)this.BranchCoverageRatio.Item2);
			
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
