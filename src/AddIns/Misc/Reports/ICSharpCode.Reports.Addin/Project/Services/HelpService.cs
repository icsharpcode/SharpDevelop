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

// created on 10/10/2002 at 16:13

using System;
using System.ComponentModel.Design;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// BaseImlementation of IHelpService
	/// </summary>
	/// <remarks>
	/// 	created by - Niv
	/// 	created on - 10/10/2002 11:44:46
	/// </remarks>
	public class HelpService : IHelpService
	{
		string f1Keyword;
		string generalKeyword;
		
		
		public HelpService()
		{
		}
		
		public void AddContextAttribute(string name, string value, HelpKeywordType keywordType)
		{
			switch (keywordType) {
				case HelpKeywordType.F1Keyword:
					f1Keyword = value;
					return;
				case HelpKeywordType.GeneralKeyword:
					generalKeyword = value;
					return;
			}
		}
		
		public void ClearContextAttributes()
		{
		}
		
		public IHelpService CreateLocalContext(HelpContextType contextType)
		{
			return this;
		}
		
		public void RemoveContextAttribute(string name, string value)
		{
//			System.Console.WriteLine("child removeing {0} : {1}",name,value);
//			object att = helpGUI.RemoveContextAttributeFromView(name,value);
//			ContextAttributes.Remove(att);;
		}
		
		public void RemoveLocalContext(IHelpService localContext)
		{
		}
		
		public void ShowHelpFromKeyword(string helpKeyword)
		{
			HelpProvider.ShowHelpByKeyword(helpKeyword);
		}
		public void ShowGeneralHelp()
		{
			ShowHelpFromKeyword(generalKeyword);
		}
		public void ShowHelp()
		{
//			HelpProvider.ShowHelp(f1Keyword);
		}
		
		public void ShowHelpFromUrl(string helpUrl)
		{
			FileService.OpenFile("browser://" + helpUrl);
		}
	}
}
