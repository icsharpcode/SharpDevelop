/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.08.2005
 * Time: 18:25
 */

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of ICanBeDirty.
	/// </summary>
	public interface ICanBeDirty
	{
		/// <summary>
		/// If this property returns true the content has changed since
		/// the last load/save operation.
		/// </summary>
		bool IsDirty {
			get;
			set;
		}
		
		/// <summary>
		/// Is called when the content is changed after a save/load operation
		/// and this signals that changes could be saved.
		/// </summary>
		event EventHandler DirtyChanged;
	}
}
