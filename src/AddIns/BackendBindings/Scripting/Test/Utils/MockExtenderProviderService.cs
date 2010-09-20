// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockExtenderProviderService : IExtenderProviderService
	{
		bool languageExtendersAdded;
		
		public MockExtenderProviderService()
		{
		}
				
		public bool IsLanguageExtendersAdded { 
			get { return languageExtendersAdded; }
		}
		
		public void AddExtenderProvider(IExtenderProvider provider)
		{
			if (provider.GetType().Name == "LanguageExtenders") {
				languageExtendersAdded = true;
			}
		}
		
		public void RemoveExtenderProvider(IExtenderProvider provider)
		{
		}		
	}
}
