/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.02.2014
 * Time: 18:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reporting.Addin.Services
{
	class HelpService : IHelpService
	{
		string f1Keyword;
		string generalKeyword;
		
		
		public HelpService()
		{
			LoggingService.Info("Create HelpService");
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
//		public void ShowGeneralHelp()
//		{
//			ShowHelpFromKeyword(generalKeyword);
//		}
//		static public void ShowHelp()
//		{
////			HelpProvider.ShowHelp(f1Keyword);
//		}
		
		public void ShowHelpFromUrl(string helpUrl)
		{
			FileService.OpenFile("browser://" + helpUrl);
		}
	}
}
