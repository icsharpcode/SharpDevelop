// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2022 $</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// NamedValue is a Value which has some name associated with it - 
	/// eg the name of the field that holds the value.
	/// </summary>
	public class NamedValue: Value
	{
		string name;
		
		/// <summary> Gets the name associated with the value </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		internal NamedValue(string name,
		                    Process process,
		                    IExpirable[] expireDependencies,
		                    IMutable[] mutateDependencies,
		                    CorValueGetter corValueGetter)
			:base (process,
			       expireDependencies,
			       mutateDependencies,
			       corValueGetter)
		{
			this.name = name;
			
			// TODO: clean up
			if (name.StartsWith("<") && name.Contains(">") && name != "<Base class>") {
				string middle = name.TrimStart('<').Split('>')[0]; // Get text between '<' and '>'
				if (middle != "") {
					this.name = middle;
				}
			}
		}
	}
}
