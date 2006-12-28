// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// The DesignSite connects a component with the service system and the designers.
	/// </summary>
	public abstract class DesignSite : IServiceProvider
	{
		/// <summary>
		/// Gets the component this DesignSite was created for.
		/// </summary>
		public abstract object Component { get; }
		
		DefaultServiceProvider _defaultServiceProvider;
		
		/// <summary>
		/// Gets an instance that provides convenience properties for the most-used designers.
		/// </summary>
		public DefaultServiceProvider Services {
			get {
				if (_defaultServiceProvider == null) {
					_defaultServiceProvider = new DefaultServiceProvider(this);
				}
				return _defaultServiceProvider;
			}
		}
		
		/// <summary>
		/// Gets the service with the specified type.
		/// </summary>
		public abstract object GetService(Type serviceType);
	}
}
