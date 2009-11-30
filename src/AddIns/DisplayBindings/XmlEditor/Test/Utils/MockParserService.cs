// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class MockParserService : IParserService
	{
		IProjectContent projectContentPassedToGetExistingParseInfoMethod;
		Dictionary<string, ParseInformation> parseInfoDictionary = new Dictionary<string, ParseInformation>();
		
		public MockParserService()
		{
		}
		
		public ParseInformation GetExistingParseInformation(IProjectContent content, string fileName)
		{
			projectContentPassedToGetExistingParseInfoMethod = content;
			
			ParseInformation parseInfo;
			if (parseInfoDictionary.TryGetValue(fileName, out parseInfo)) {
				return parseInfo;
			}
			return null;
		}
		
		public void SetExistingParseInformation(string fileName, ParseInformation parseInfo)
		{
			parseInfoDictionary.Add(fileName, parseInfo);
		}
		
		public IProjectContent ProjectContentPassedToGetExistingParseInforMethod {
			get { return projectContentPassedToGetExistingParseInfoMethod; }
		}
	}
}
