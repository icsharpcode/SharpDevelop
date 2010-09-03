// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
