// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
