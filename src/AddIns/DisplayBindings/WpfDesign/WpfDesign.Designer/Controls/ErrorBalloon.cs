// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2263 $</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// An ErrorBalloon window.
	/// </summary>
	public class ErrorBalloon : Window
	{
		static ErrorBalloon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ErrorBalloon), new FrameworkPropertyMetadata(typeof(ErrorBalloon)));
		}
	}
}
