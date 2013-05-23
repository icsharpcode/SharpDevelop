// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Workbench
{
	public abstract class AbstractPadContent : IPadContent
	{
		/// <inheritdoc/>
		public abstract object Control {
			get;
		}
		
		/// <inheritdoc/>
		public virtual object InitiallyFocusedControl {
			get {
				return null;
			}
		}
		
		public virtual void Dispose()
		{
		}
		
		public void BringToFront()
		{
			PadDescriptor d = this.PadDescriptor;
			if (d != null)
				d.BringPadToFront();
		}
		
		protected virtual PadDescriptor PadDescriptor {
			get {
				return SD.Workbench.GetPad(GetType());
			}
		}
		
		public virtual object GetService(Type serviceType)
		{
			if (serviceType.IsInstanceOfType(this))
				return this;
			else
				return null;
		}
	}
}
