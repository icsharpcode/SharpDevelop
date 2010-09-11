// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockWorkbench : IScriptingWorkbench
	{		
		public FakeScriptingConsolePad FakeScriptingConsolePad = new FakeScriptingConsolePad();
		public MockEditableViewContent ActiveMockEditableViewContent;
		
		public static MockWorkbench CreateWorkbenchWithOneViewContent(string fileName)
		{
			MockEditableViewContent viewContent = new MockEditableViewContent();
			viewContent.PrimaryFileName = new FileName(fileName);
						
			MockWorkbench workbench = new MockWorkbench();
			workbench.ActiveMockEditableViewContent = viewContent;
			
			return workbench;
		}
		
		public IViewContent ActiveViewContent {
			get { return ActiveMockEditableViewContent; }
		}		
				
		public IScriptingConsolePad GetScriptingConsolePad()
		{
			return FakeScriptingConsolePad;
		}
	}
}
