using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree
{
	public enum DrawSelectionMode
	{
		None, Active, Inactive, FullRowSelect
	}

	public enum TreeSelectionMode
	{
		Single, Multi, MultiSameParent
	}

	public enum NodePosition
	{
		Inside, Before, After
	}

	public enum VerticalAlignment
	{
		Top, Bottom, Center
	}

	public enum IncrementalSearchMode
	{
		None, Standard, Continuous
	}
}
