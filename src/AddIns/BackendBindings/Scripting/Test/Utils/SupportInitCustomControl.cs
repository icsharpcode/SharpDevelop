// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
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
