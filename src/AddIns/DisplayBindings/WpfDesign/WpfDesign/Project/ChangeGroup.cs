// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Base class for change groups.
	/// </summary>
	public abstract class ChangeGroup : IDisposable
	{
		string title;
		
		/// <summary>
		/// Gets/Sets the title of the change group.
		/// </summary>
		public string Title {
			get { return title; }
			set { title = value; }
		}
		
		/// <summary>
		/// Commits the change group.
		/// </summary>
		public abstract void Commit();
		
		/// <summary>
		/// Aborts the change group.
		/// </summary>
		public abstract void Abort();
		
		/// <summary>
		/// Is called when the change group is disposed. Should Abort the change group if it is not already committed.
		/// </summary>
		protected abstract void Dispose();
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
		void IDisposable.Dispose()
		{
			Dispose();
		}
	}
}
