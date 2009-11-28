// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;

namespace WixBinding.Tests.Utils
{
	public class MockCaret : ITextEditorCaret
	{
		Location position = Location.Empty;
		
		public MockCaret()
		{
		}
		
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
		
		public Location Position {
			get { return position; }
			set { position = value; }
		}
	}
}
