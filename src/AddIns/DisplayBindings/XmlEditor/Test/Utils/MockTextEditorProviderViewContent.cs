// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace XmlEditor.Tests.Utils
{
	public class MockTextEditorProviderViewContent : MockViewContent
	{
		MockTextEditor textEditor = new MockTextEditor();
		
		public MockTextEditor MockTextEditor {
			get { return textEditor; }
		}
		
		public override object GetService(Type serviceType)
		{
			return textEditor.GetService(serviceType);
		}
	}
}
