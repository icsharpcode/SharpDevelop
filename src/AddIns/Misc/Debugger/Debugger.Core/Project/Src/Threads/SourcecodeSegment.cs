// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics.SymbolStore;

namespace DebuggerLibrary
{
	[Serializable]
	public class SourcecodeSegment
	{
		string moduleFilename;
		string sourceFullFilename;
		int startLine;
		int startColumn;
		int endLine;
		int endColumn;
		int ilOffset;
		int ilStart;
		int ilEnd;
		int[] stepRanges;

		internal SourcecodeSegment()
		{

		}

		public SourcecodeSegment(string sourceFilename, int line)
		{
			this.sourceFullFilename = sourceFilename;
			this.startLine = line;
			this.endLine = line;
		}

		public SourcecodeSegment(string sourceFilename, int line, int startColumn, int endColumn)
		{
			this.sourceFullFilename = sourceFilename;
			this.startLine = line;
			this.endLine = line;
			this.startColumn = startColumn;
			this.endColumn = endColumn;
		}

		public SourcecodeSegment(string sourceFilename, int startLine, int endLine, int startColumn, int endColumn)
		{
			this.sourceFullFilename = sourceFilename;
			this.startLine = startLine;
			this.endLine = endLine;
			this.startColumn = startColumn;
			this.endColumn = endColumn;
		}
		
		public string ModuleFilename {
			get {
				return moduleFilename;
			}
			set {
				moduleFilename = value;
			}
		}
		
		public string SourceFullFilename {
			get {
				return sourceFullFilename;
			}
			set {
				sourceFullFilename = value;
			}
		}

		public string SourceFilename {
			get {
				return System.IO.Path.GetFileName(sourceFullFilename);
			}
		}
		
		public int StartLine {
			get {
				return startLine;
			}
			set {
				startLine = value;
			}
		}
		
		public int StartColumn {
			get {
				return startColumn;
			}
			set {
				startColumn = value;
			}
		}
		
		public int EndLine {
			get {
				return endLine;
			}
			set {
				endLine = value;
			}
		}
		
		public int EndColumn {
			get {
				return endColumn;
			}
			set {
				endColumn = value;
			}
		}

		public int[] StepRanges {
			get {
				return stepRanges;
			}
			set {
				stepRanges = value;
			}
		}

		public int ILOffset {
			get { 
				return ilOffset;
			}
			set {
				ilOffset = value;
			}
		}

		public int ILStart {
			get { 
				return ilStart;
			}
			set {
				ilStart = value;
			}
		}

		public int ILEnd {
			get { 
				return ilEnd;
			}
			set {
				ilEnd = value;
			}
		}
	}
}
