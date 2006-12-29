// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// The DesignItem connects a component with the service system and the designers.
	/// Equivalent to Cider's ModelItem.
	/// </summary>
	/// <remarks>
	/// About the Cider extension system:
	/// http://blogs.msdn.com/jnak/archive/2006/04/24/580393.aspx
	/// http://blogs.msdn.com/jnak/archive/2006/08/04/687166.aspx
	/// </remarks>
	public abstract class DesignItem
	{
		/// <summary>
		/// Gets the component this DesignSite was created for.
		/// </summary>
		public abstract object Component { get; }
		
		/// <summary>
		/// Gets the view used for the component.
		/// </summary>
		public abstract UIElement View { get; }
		
		/// <summary>
		/// Gets the design context.
		/// </summary>
		public abstract DesignContext Context { get; }
		
		/// <summary>
		/// Gets an instance that provides convenience properties for the most-used designers.
		/// </summary>
		public ServiceContainer Services {
			[DebuggerStepThrough]
			get { return this.Context.Services; }
		}
	}
}
