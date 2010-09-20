// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;

namespace PythonBinding.Tests.Utils
{
	public class DerivedPythonFormsDesignerDisplayBinding : PythonFormsDesignerDisplayBinding
	{
		ParseInformation parseInfo;
		ParseInformation parseServiceParseInfo;
		bool isParseInfoDesignable;
		string fileNamePassedToGetParseInfo;
		string textContentNamePassedToGetParseInfo;
		
		public DerivedPythonFormsDesignerDisplayBinding()
		{
		}
		
		/// <summary>
		/// Gets the parse info passed to the Forms Designer's IsDesignable method.
		/// </summary>
		public ParseInformation ParseInfoTestedForDesignability {
			get { return parseInfo; }
		}
		
		/// <summary>
		/// Gets or sets the parse info that will be returned from the
		/// GetParseInfo method.
		/// </summary>
		public ParseInformation ParseServiceParseInfoToReturn {
			get { return parseServiceParseInfo; }
			set { parseServiceParseInfo = value; }
		}
		
		/// <summary>
		/// Gets or sets what the IsDesignable method should return.
		/// </summary>
		public bool IsParseInfoDesignable {
			get { return isParseInfoDesignable; }
			set { isParseInfoDesignable = value; }
		}
		
		/// <summary>
		/// Gets the filename passed to the GetParseInfo method.
		/// </summary>
		public string FileNamePassedToGetParseInfo {
			get { return fileNamePassedToGetParseInfo; }
		}
		
		/// <summary>
		/// Gets the text content passed to the GetParseInfo method.
		/// </summary>
		public string TextContentPassedToGetParseInfo {
			get { return textContentNamePassedToGetParseInfo; }
		}
		
		/// <summary>
		/// Gets the parse information from the parse service
		/// for the specified file.
		/// </summary>
		protected override ParseInformation GetParseInfo(string fileName, ITextBuffer textBuffer)
		{
			fileNamePassedToGetParseInfo = fileName;
			textContentNamePassedToGetParseInfo = textBuffer.Text;
			return parseServiceParseInfo;
		}
		
		/// <summary>
		/// Determines whether the specified parse information contains
		/// a class which is designable.
		/// </summary>
		protected override bool IsDesignable(ParseInformation parseInfo)
		{
			this.parseInfo = parseInfo;
			return isParseInfoDesignable;
		}
	}
}
