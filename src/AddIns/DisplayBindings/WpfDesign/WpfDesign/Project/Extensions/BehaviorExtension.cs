// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
