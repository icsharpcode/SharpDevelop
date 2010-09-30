// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			HelpProvider.ShowHelp(f1Keyword);
		}
		
		public void ShowHelpFromUrl(string helpUrl)
		{
			FileService.OpenFile("browser://" + helpUrl);
		}
	}
}
