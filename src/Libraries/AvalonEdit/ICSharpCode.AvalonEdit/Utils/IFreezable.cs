// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Utils
{
	interface IFreezable
	{
		/// <summary>
		/// Gets if this instance is frozen. Frozen instances are immutable and thus thread-safe.
		/// </summary>
		bool IsFrozen { get; }
		
		/// <summary>
		/// Freezes this instance.
		/// </summary>
		void Freeze();
	}
	
	[Serializable]
	sealed class FreezableNullSafeCollection<T> : NullSafeCollection<T>, IFreezable where T : class, IFreezable
	{
		bool isFrozen;
		
		public bool IsFrozen { get { return isFrozen; } }
		
		public void Freeze()
		{
			if (!isFrozen) {
				foreach (T item in this) {
					item.Freeze();
				}
				isFrozen = true;
			}
		}
		
		void CheckBeforeMutation()
		{
			if (isFrozen)
				throw new InvalidOperationException("Cannot mutate frozen " + GetType().Name);
		}
		
		protected override void ClearItems()
		{
			this.CheckBeforeMutation();
			base.ClearItems();
		}
		
		protected override void InsertItem(int index, T item)
		{
			this.CheckBeforeMutation();
			base.InsertItem(index, item);
		}
		
		protected override void RemoveItem(int index)
		{
			this.CheckBeforeMutation();
			base.RemoveItem(index);
		}
		
		protected override void SetItem(int index, T item)
		{
			this.CheckBeforeMutation();
			base.SetItem(index, item);
		}
	}
}
