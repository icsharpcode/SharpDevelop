// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2413 $</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Base class for extensions that provide a behavior interface for the designed item.
	/// These extensions are always loaded. They must have an parameter-less constructor.
	/// </summary>
	[ExtensionServer(typeof(DefaultExtensionServer.Permanent))]
	public class BehaviorExtension : DefaultExtension
	{
		
	}
}
