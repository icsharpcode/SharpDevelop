// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{

	public class VirtualizingIEnumerable<T> : ObservableCollection<T>
	{
		private IEnumerator<T> originalSourceEnumerator;
		
		public VirtualizingIEnumerable(IEnumerable<T> originalSource)
		{
			if (originalSource == null)
				throw new ArgumentNullException("originalSource");
			
			this.originalSourceEnumerator = originalSource.GetEnumerator();
		}
		
		public void RequestNextItems(int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (!originalSourceEnumerator.MoveNext()) break;
				this.Add(originalSourceEnumerator.Current);
			}
		}
	}
}
