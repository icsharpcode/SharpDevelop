// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	/// <summary>
	/// Description of MockOpenedFile.
	/// </summary>
	public class MockOpenedFile : OpenedFile
	{
		public MockOpenedFile(string fileName)
		{
			base.FileName = new ICSharpCode.Core.FileName(fileName);
			base.IsUntitled = true;
			SetData(new byte[0]);
		}
		
		List<IViewContent> registeredViews = new List<IViewContent>();
		
		public override IList<IViewContent> RegisteredViewContents {
			get { return registeredViews.AsReadOnly(); }
		}
		
		public override void RegisterView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (registeredViews.Contains(view))
				throw new ArgumentException("registeredViews already contains view");
			
			registeredViews.Add(view);
			
			if (registeredViews.Count == 1)
				SwitchedToView(registeredViews[0]);
		}
		
		public override void UnregisterView(IViewContent view)
		{
			if (!registeredViews.Remove(view))
				throw new ArgumentException("registeredViews does not contain view");
		}
		
		public void SwitchToView(IViewContent view)
		{
			if (!registeredViews.Contains(view))
				throw new ArgumentException("registeredViews does not contain view");
			base.SwitchedToView(view);
		}
		
		public override event EventHandler FileClosed { add {} remove {} }
	}
}
