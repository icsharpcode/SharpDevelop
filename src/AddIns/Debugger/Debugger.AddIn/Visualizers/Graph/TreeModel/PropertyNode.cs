// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Services;
using System;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Node containing ObjectGraphProperty.
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
