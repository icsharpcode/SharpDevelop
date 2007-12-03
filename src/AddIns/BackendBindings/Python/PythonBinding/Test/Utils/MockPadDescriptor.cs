// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Gui;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Dummy IPadDescriptor class.
	/// </summary>
	public class MockPadDescriptor : IPadDescriptor
	{
		bool bringPadToFrontCalled;
		
		public MockPadDescriptor()
		{
		}
		
		public void BringPadToFront()
		{
			bringPadToFrontCalled = true;
		}
		
		/// <summary>
		/// Gets whether the BringPadToFront was called.
		/// </summary>
		public bool BringPadToFrontCalled {
			get {
				return bringPadToFrontCalled;
			}
		}
	}
}
