// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		bool updateCommentTagsPassedToGetParseInfo;
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
		/// Gets the update comment tags flag passed to the GetParseInfo method.
		/// </summary>
		public bool UpdateCommentTagsPassedToGetParseInfo {
			get { return updateCommentTagsPassedToGetParseInfo; }
		}
		
		/// <summary>
		/// Gets the parse information from the parse service
		/// for the specified file.
		/// </summary>
		protected override ParseInformation GetParseInfo(string fileName, string textContent, bool updateCommentTags)
		{
			fileNamePassedToGetParseInfo = fileName;
			textContentNamePassedToGetParseInfo = textContent;
			updateCommentTagsPassedToGetParseInfo = updateCommentTags;
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
