// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RubyBinding.Tests.Utils
{
	public class SupportInitCustomControl : UserControl, ISupportInitialize
	{
		bool beginInitCalled;
		bool endInitCalled;
		
		public SupportInitCustomControl()
		{
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool IsBeginInitCalled {
			get { return beginInitCalled; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool IsEndInitCalled {
			get { return endInitCalled; }
		}
		
		/// <summary>
		/// Deliberately making the interface explicit for the BeginInit method but not the EndInit method.
		/// </summary>
		void ISupportInitialize.BeginInit()
		{
			beginInitCalled = true;
		}
		
		public void EndInit()
		{
			endInitCalled = true;
		}		
	}
}
