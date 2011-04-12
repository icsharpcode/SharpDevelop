// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.SharpDevelop.Services;
using System;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Contains ObjectGraphProperty.
	/// </summary>
	public class PropertyNode : AbstractNode
	{
		public PropertyNode(ObjectGraphProperty objectGraphProperty)
		{
			if (objectGraphProperty == null)
				throw new ArgumentNullException("objectGraphProperty");
			
			this.property = objectGraphProperty;
		}
		
		private ObjectGraphProperty property;
		public ObjectGraphProperty Property
		{
			get { return this.property; }
		}
	}
}
