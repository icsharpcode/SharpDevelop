// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A thumb where the look can depend on the IsPrimarySelection property.
	/// Used by UIElementSelectionRectangle.
	/// </summary>
	public class ResizeThumb : Thumb
	{
		/// <summary>
		/// Dependency property for <see cref="IsPrimarySelection"/>.
		/// </summary>
		public static readonly DependencyProperty IsPrimarySelectionProperty
			= DependencyProperty.Register("IsPrimarySelection", typeof(bool), typeof(ResizeThumb));
		
		static ResizeThumb()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeThumb), new FrameworkPropertyMetadata(typeof(ResizeThumb)));
		}
		
		/// <summary>
		/// Gets/Sets if the resize thumb is attached to the primary selection.
		/// </summary>
		public bool IsPrimarySelection {
			get { return (bool)GetValue(IsPrimarySelectionProperty); }
			set { SetValue(IsPrimarySelectionProperty, value); }
		}
	}
}
