// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Description of MockExtenderProviderService.
	/// </summary>
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
