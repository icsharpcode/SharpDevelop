// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RubyBinding.Tests.Utils
{
	public class MockPresentationSource : PresentationSource
	{
		public MockPresentationSource()
		{
		}
		
		public override Visual RootVisual {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool IsDisposed {
			get {
				throw new NotImplementedException();
			}
		}
		
		protected override CompositionTarget GetCompositionTargetCore()
		{
			throw new NotImplementedException();
		}
	}
}
