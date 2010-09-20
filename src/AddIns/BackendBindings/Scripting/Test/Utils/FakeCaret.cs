// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeCaret : ITextEditorCaret
	{
		public event EventHandler PositionChanged;
		
		protected virtual void OnPositionChanged(EventArgs e)
		{
			if (PositionChanged != null) {
				PositionChanged(this, e);
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
		
		public int Line { get; set; }
		
		public int Column {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.NRefactory.Location Position {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
