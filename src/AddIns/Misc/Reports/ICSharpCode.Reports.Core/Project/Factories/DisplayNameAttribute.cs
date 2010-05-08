// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Reports.Core{
	
	
	public sealed class DisplayNameAttribute : Attribute{
		string name;
		
		public string Name{
			get { return name; }
		}
		
		public DisplayNameAttribute(string name){
			this.name = name;
		}
	}
}
