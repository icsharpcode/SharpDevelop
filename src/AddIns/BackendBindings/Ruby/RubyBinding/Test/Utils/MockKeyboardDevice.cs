// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

namespace RubyBinding.Tests.Utils
{
	public class MockKeyboardDevice : KeyboardDevice
	{
		public MockKeyboardDevice()
			: base(InputManager.Current)
		{
		}
		
		protected override KeyStates GetKeyStatesFromSystem(Key key)
		{
			throw new NotImplementedException();
		}
	}
}
