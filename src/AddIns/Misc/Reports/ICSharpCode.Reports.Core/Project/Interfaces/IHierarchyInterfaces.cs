// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

//ToDo with NET2.0 change this Interface to the one provided by NET2.0
using System;

namespace ICSharpCode.Reports.Core {
	
	public interface IHierarchyData{
		
		IndexList GetChildren {
			get;
		}

		bool HasChildren {
			get;
		}
		
		object Item {
			get;
		}
		string Path {
			get;
		}
		
		string Type {
			get;
		}
	}
}
