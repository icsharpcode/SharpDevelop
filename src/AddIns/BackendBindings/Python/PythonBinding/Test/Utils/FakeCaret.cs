// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace PythonBinding.Tests.Utils
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
