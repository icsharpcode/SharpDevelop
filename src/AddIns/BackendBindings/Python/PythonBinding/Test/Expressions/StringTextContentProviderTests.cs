// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class StringTextContentProviderTests
	{
		[Test]
		public void ReadToEndFromStringTextContentProvider()
		{
			string text = "abc";
			StringTextContentProvider provider = new StringTextContentProvider(text);
			using (SourceCodeReader reader = provider.GetReader()) {
				Assert.AreEqual("abc", reader.ReadToEnd());
			}
		}
		
		[Test]
		public void StringTextContentProviderIsMicrosoftScriptingTextContentProvider()
		{
			StringTextContentProvider provider = new StringTextContentProvider(String.Empty);
			Assert.IsNotNull(provider as TextContentProvider);
		}
	}
}
