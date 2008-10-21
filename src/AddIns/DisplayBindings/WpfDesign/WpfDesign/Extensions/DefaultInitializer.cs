// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2258 $</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Base class for extensions that initialize new controls with default values.
	/// </summary>
	[ExtensionServer(typeof(NeverApplyExtensionsExtensionServer))]
	public abstract class DefaultInitializer : Extension
	{
		/// <summary>
		/// Initializes the design item to default values.
		/// </summary>
		public abstract void InitializeDefaults(DesignItem item);
	}
}
