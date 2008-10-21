// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2237 $</version>
// </file>

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
		protected abstract void Disposed();
		
		void IDisposable.Dispose()
		{
			Disposed();
		}
	}
}
