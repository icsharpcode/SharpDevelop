// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;
using System.Collections.Generic;

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
