// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Abstract implementation of the <see cref="ICommand"/> interface.
	/// </summary>
	public abstract class AbstractCommand : ICommand
	{
		object owner = null;
		
		/// <summary>
		/// Returns the owner of the command.
		/// </summary>
		public virtual object Owner {
			get {
				return owner;
			}
			set {
				owner = value;
				OnOwnerChanged(EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Invokes the command.
		/// </summary>
		public abstract void Run();
		
		
		protected virtual void OnOwnerChanged(EventArgs e) 
		{
			if (OwnerChanged != null) {
				OwnerChanged(this, e);
			}
		}
		
		public event EventHandler OwnerChanged;
	}
}
