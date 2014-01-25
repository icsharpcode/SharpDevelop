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
