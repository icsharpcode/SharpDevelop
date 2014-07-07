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
		readonly XElement element;
		readonly CodeCoverageResults results;

		/// <summary>Enables CodeCoverage.Test to compile</summary>
		/// <param name="element">XMLElement</param>
		public CodeCoverageMethodElement(XElement element)
			: this (element, null) {}
		
		/// <summary>Create Method Element</summary>
		/// <param name="element">XMLElement</param>
		/// <param name="results">has .GetFileName(FileID)</param>
		public CodeCoverageMethodElement(XElement element, CodeCoverageResults results)
		{
			this.results = results;
			this.element = element;
			this.SequencePoints = new List<CodeCoverageSequencePoint>();
			Init();
		}

		// Primary TextSource cache
		private static string cacheFileName = String.Empty;
		private static CodeCoverageStringTextSource cacheDocument = null;

		// Secondary TextSource cache
		private static string cache2FileName = String.Empty;
		private static CodeCoverageStringTextSource cache2Document = null;

		public string FileID { get; private set; }
		public string FileName { get; private set; }
		public string FileNameExt { get; private set; }
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
			this.FileNameExt = String.Empty;
			if (!String.IsNullOrEmpty(this.FileID)) {
				if (results != null) {
					this.FileName = results.GetFileName(this.FileID);
					try {
						this.FileNameExt = Path.GetExtension(this.FileName).ToLowerInvariant();
					}
					catch {}
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
				this.getBodyStartSP();
				this.getBodyFinalSP();
				this.GetBranchRatio();
				this.GetBranchCoverage();

				// SP's are originaly ordered by CIL offset
				// because ccrewrite can move offset of
				//   Contract.Requires before method start ({) SP offset
				//   Contract.Ensures after method final (}) SP offset
				// So sort SP's back by line/column
				this.SequencePoints.OrderBy(item => item.Line).OrderBy(item => item.Column);
			}
		}
		
		private static string cacheGetSource_LastFileName = null;
		private static CodeCoverageStringTextSource cacheGetSource_LastSource = null;

		static CodeCoverageStringTextSource GetSource(string filename) {

			if (filename == cacheGetSource_LastFileName) return cacheGetSource_LastSource;

			var retSource = (CodeCoverageStringTextSource)null;
			try {
				using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
					try {
						stream.Position = 0;
						string textSource = ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(stream, Encoding.Default);
						retSource = new CodeCoverageStringTextSource(textSource);
					} catch (Exception) {}
				}
			} catch (Exception) {}

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
				sp.FileID = xSPoint.Attribute("fileid") != null? xSPoint.Attribute("fileid").Value : "0";
				if (sp.FileID == "0") {
					// SequencePoint from not covered (not runnable) file
					// ie: interface with CodeContractClass/CodeContractClassFor
					sp.Document = xSPoint.Attribute("fileid") != null? xSPoint.Attribute("fileid").Value : "";
				}
				else if (sp.FileID == this.FileID) {
					// This method SequencePoint (from this.FileName)
					sp.Document = this.FileName;
				}
				else {
					// SequencePoint from another method/file
					// ie: ccrewriten CodeContractClass/CodeContractClassFor
					// [or dependency-injected or fody-weaved???]
					sp.Document = results.GetFileName(sp.FileID);
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

		// Find method-body first SequencePoint
		// -> this method SP with lowest Line/Column
		void getBodyStartSP() {
			if (this.SequencePoints.Count != 0) {
				if (this.FileNameExt == ".cs") {
					foreach (CodeCoverageSequencePoint sp in this.SequencePoints) {
						if (sp.FileID != this.FileID) continue;
						if (this.BodyStartSP == null || (sp.Line < this.BodyStartSP.Line) ||
						   (sp.Line == this.BodyStartSP.Line && sp.Column < this.BodyStartSP.Column)
						   ) {
							this.BodyStartSP = sp;
						}
					}
				}
				else {
					this.BodyStartSP = this.SequencePoints.First();
				}
			}
		}

		// Find method-body last SequencePoint
		// -> this method SP.Content=="}" with highest Line/Column
		// and lowest Offset (when duplicated bw ccrewrite)
		void getBodyFinalSP() {
			if (this.SequencePoints.Count != 0) {
				if (this.FileNameExt == ".cs") {
					for (int i = this.SequencePoints.Count-1; i > 0; i--) {
						var sp = this.SequencePoints[i];
						if (sp.FileID != this.FileID) continue;
						if (sp.Content != "}") continue;
						if (this.BodyFinalSP == null || (sp.Line > this.BodyFinalSP.Line) ||
						   (sp.Line == this.BodyFinalSP.Line && sp.Column >= this.BodyFinalSP.Column)
						   ) {
							// ccrewrite ContractClass/ContractClassFor
							// adds duplicate method end-sequence-point "}"
							//
							// Take duplicate BodyFinalSP with lower Offset
							// Because IL.Offset of second duplicate
							// will extend branch coverage of this method
							// by coverage of ContractClassFor inserted SequencePoint!
							if (this.BodyFinalSP != null &&
								sp.Line == this.BodyFinalSP.Line &&
								sp.Column == this.BodyFinalSP.Column &&
								sp.Offset < this.BodyFinalSP.Offset) {
								this.SequencePoints.Remove(this.BodyFinalSP); // remove duplicate
							}
							this.BodyFinalSP = sp;
						}
					}
				}
				else {
					this.BodyFinalSP = this.SequencePoints.Last();
				}
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

		const string @assert = "Assert";
		const string @contract = "Contract";

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
				if (sp.VisitCount != 0 && sp.FileID == this.FileID) {

					if (this.FileNameExt == ".cs") {
						// Only for C#

						// Don't want branch coverage of ccrewrite(n)
						// SequencePoint(s) with offset before and after method body
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
						    sp.Content.StartsWith(@assert + ".", StringComparison.Ordinal) ||
						    sp.Content.StartsWith(@assert + " ", StringComparison.Ordinal) ||
						    sp.Content.StartsWith(@contract + ".", StringComparison.Ordinal) ||
						    sp.Content.StartsWith(@contract + " ", StringComparison.Ordinal)
						   ) {
							sp.BranchCoverage = true;
							continue; // skip
						}
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
			return nameElement != null ? GetMethodName(nameElement.Value) : String.Empty;
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
