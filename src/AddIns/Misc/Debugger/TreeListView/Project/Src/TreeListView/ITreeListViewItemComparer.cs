using System;

namespace System.Windows.Forms
{
	/// <summary>
	/// Interface ITreeListViewItemComparer
	/// </summary>
	public interface ITreeListViewItemComparer : System.Collections.IComparer
	{
		/// <summary>
		/// Sort order
		/// </summary>
		SortOrder SortOrder
		{
			get;
			set;
		}
		/// <summary>
		/// Column for the comparison
		/// </summary>
		int Column
		{
			get;
			set;
		}
	}
}
