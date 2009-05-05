using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace ICSharpCode.TreeView
{
	public class SharpGridView : GridView
	{
		static SharpGridView()
		{
			ItemContainerStyleKey =
				new ComponentResourceKey(typeof(SharpTreeView), "GridViewItemContainerStyleKey");
		}

		public static ResourceKey ItemContainerStyleKey { get; private set; }

		protected override object ItemContainerDefaultStyleKey
		{
			get
			{
				return ItemContainerStyleKey;
			}
		}
	}
}
