// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3523 $</version>
// </file>

using System;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Adorners
{
	// Some classes that derive from AdornerProvider to specify a certain ExtensionServer.
	
	/// <summary>
	/// An adorner extension that is attached permanently.
	/// </summary>
	[ExtensionServer(typeof(DefaultExtensionServer.Permanent))]
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
