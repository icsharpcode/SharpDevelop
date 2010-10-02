// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace PythonBinding.Tests.Utils
{
	public class FakeInsightWindow : IInsightWindow
	{
		#pragma warning disable 67
		public event EventHandler<TextChangeEventArgs> DocumentChanged;
		public event EventHandler SelectedItemChanged;
		public event EventHandler CaretPositionChanged;
		public event EventHandler Closed;
		#pragma warning restore 67
		
		public bool IsClosed;
		
		public IList<IInsightItem> Items {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IInsightItem SelectedItem {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public double Width {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
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
		
		public int StartOffset { get; set; }
		
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
			IsClosed = true;
		}
		
		public void FireDocumentChangedEvent(TextChangeEventArgs e)
		{
			if (DocumentChanged != null) {
				DocumentChanged(this, e);
			}
		}
	}
}
