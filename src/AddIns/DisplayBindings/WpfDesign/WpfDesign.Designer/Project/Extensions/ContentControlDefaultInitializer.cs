// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2667$</version>
// </file>

using System;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Description of ContentControlDefaultInitializer.
	/// </summary>
	[ExtensionFor(typeof(ContentControl))]
	public sealed class ContentControlDefaultInitializer : DefaultInitializer
	{
		public override void InitializeDefaults(DesignItem item)
		{
			DesignItemProperty contentProperty = item.Properties["Content"];
			if (contentProperty.ValueOnInstance == null) {
				contentProperty.SetValue(item.ComponentType.Name);
			}
		}
	}
}
