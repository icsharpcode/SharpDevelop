// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Adorners
{
	// Some classes that derive from AdornerProvider to specify a certain ExtensionServer.
	
	/// <summary>
	/// An adorner extension that is attached permanently.
	/// </summary>
	[ExtensionServer(typeof(DefaultExtensionServer.PermanentWithDesignPanel))]
	public abstract class PermanentAdornerProvider : AdornerProvider
	{
		
	}
	
	/// <summary>
	/// An adorner extension that is attached to selected components.
	/// </summary>
	[ExtensionServer(typeof(SelectionExtensionServer))]
	public abstract class SelectionAdornerProvider : AdornerProvider
	{
		
	}
	
	/// <summary>
	/// An adorner extension that is attached to the primary selection.
	/// </summary>
	[ExtensionServer(typeof(PrimarySelectionExtensionServer))]
	public abstract class PrimarySelectionAdornerProvider : AdornerProvider
	{
		
	}
	
	/// <summary>
	/// An adorner extension that is attached to the secondary selection.
	/// </summary>
	[ExtensionServer(typeof(SecondarySelectionExtensionServer))]
	public abstract class SecondarySelectionAdornerProvider : AdornerProvider
	{
		
	}
}
