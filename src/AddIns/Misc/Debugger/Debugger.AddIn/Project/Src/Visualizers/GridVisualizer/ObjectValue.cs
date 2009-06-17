// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Description of ObjectValue.
	/// </summary>
	public class ObjectValue
	{
		private Dictionary<string, ObjectProperty> properties = new Dictionary<string, ObjectProperty>();

		public ObjectProperty this[string key]
		{
			get
			{
				return properties.GetValue(key);
			}
			set
			{
				properties[key] = value;
			}
		}
	}
}
