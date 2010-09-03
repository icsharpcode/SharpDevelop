// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
