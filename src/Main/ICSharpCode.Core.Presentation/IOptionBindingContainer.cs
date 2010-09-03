// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Provides access to objects containing OptionBindings, such as OptionPanels.
	/// </summary>
	public interface IOptionBindingContainer
	{
		void AddBinding(OptionBinding binding);
	}
}
