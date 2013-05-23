// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;

namespace WixBinding.Tests.Utils
{
	public class MockCaret : ITextEditorCaret
	{
		TextLocation location = TextLocation.Empty;
		
		public MockCaret()
		{
		}
		
		public event EventHandler LocationChanged;
		
		protected virtual void OnLocationChanged(EventArgs e)
		{
			if (LocationChanged != null) {
				LocationChanged(this, e);
			}
		}
		
		public int Offset {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public int Line {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public int Column {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public TextLocation Location {
			get { return location; }
			set { location = value; }
		}
	}
}
