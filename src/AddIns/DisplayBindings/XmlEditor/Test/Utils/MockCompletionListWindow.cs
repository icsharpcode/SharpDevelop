// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace XmlEditor.Tests.Utils
{
	public class MockCompletionListWindow : ICompletionListWindow
	{
		double width = 0.0;
		
		public MockCompletionListWindow()
		{
		}
		
		public event EventHandler Closed;
		
		protected virtual void OnClosed(EventArgs e)
		{
			if (Closed != null) {
				Closed(this, e);
			}
		}
		
		public ICompletionItem SelectedItem {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public double Width {
			get { return width; }
			set { width = value; }
		}
		
		public double Height {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CloseAutomatically {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public int StartOffset {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public int EndOffset {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public void Close()
		{
			throw new NotImplementedException();
		}
	}
}
