// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Makes TabItems clickable.
	/// </summary>
	[ExtensionFor(typeof(TabItem))]
	public sealed class TabItemClickableExtension : BehaviorExtension, IProvideComponentInputHandlingLayer
	{
		/// <summary/>
		protected override void OnInitialized()
		{
			this.ExtendedItem.AddBehavior(typeof(IProvideComponentInputHandlingLayer), this);
		}
		
		InputHandlingLayer IProvideComponentInputHandlingLayer.InputLayer {
			get {
				return InputHandlingLayer.ComponentHigh;
			}
		}
	}
}
