// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// WeakEventManager for INotifyPropertyChanged.PropertyChanged.
	/// </summary>
	public sealed class PropertyChangedWeakEventManager : WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>
	{
		/// <inheritdoc/>
		protected override void StartListening(INotifyPropertyChanged source)
		{
			source.PropertyChanged += DeliverEvent;
		}
		
		/// <inheritdoc/>
		protected override void StopListening(INotifyPropertyChanged source)
		{
			source.PropertyChanged -= DeliverEvent;
		}
	}
}
