// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2658$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;
using System.Collections.Generic;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Description of MockOpenedFile.
	/// </summary>
	public class MockOpenedFile : OpenedFile
	{
		public MockOpenedFile(string fileName)
		{
			base.FileName = fileName;
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
	}
}
