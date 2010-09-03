// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			XmlCodeCompletionBinding completionBinding = new XmlCodeCompletionBinding(associations);
			Assert.DoesNotThrow(delegate { keyPressResult = completionBinding.HandleKeyPress(textEditor, '='); });
		}
	}
}
