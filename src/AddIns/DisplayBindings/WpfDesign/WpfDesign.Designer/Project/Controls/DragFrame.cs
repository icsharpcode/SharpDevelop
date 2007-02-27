// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// The rectangle shown during a drag'n'drop operation.
	/// </summary>
	public class DragFrame : Control
	{
		static DragFrame()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DragFrame), new FrameworkPropertyMetadata(typeof(DragFrame)));
		}
	}
}
