// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class ShowCompletionWindowReturnsNullTestFixture : NamespaceCompletionWindowTestFixtureBase
	{		
		[Test]
		public void CompletionBindingHandleKeyPressDoesNotThrowNullReferenceException()
		{
			base.InitBase();
			
			textEditor.ShowCompletionWindowReturnsNull = true;
			
			XmlCodeCompletionBinding completionBinding = new XmlCodeCompletionBinding(options);
			Assert.DoesNotThrow(delegate { keyPressResult = completionBinding.HandleKeyPress(textEditor, '='); });
		}
	}
}
