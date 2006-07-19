// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.Ast;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// Converts elements not supported by C# to their C# representation.
	/// Not all elements are converted here, most simple elements (e.g. StopStatement)
	/// are converted in the output visitor.
	/// </summary>
	public class ToCSharpConvertVisitor : AbstractAstTransformer
	{
		// The following conversions are implemented:
		//   Public Event EventName(param As String) -> automatic delegate declaration
		
		public override object Visit(EventDeclaration eventDeclaration, object data)
		{
			if (!eventDeclaration.HasAddRegion && !eventDeclaration.HasRaiseRegion && !eventDeclaration.HasRemoveRegion) {
				if (eventDeclaration.TypeReference.IsNull) {
					DelegateDeclaration dd = new DelegateDeclaration(eventDeclaration.Modifier, null);
					dd.Name = eventDeclaration.Name + "EventHandler";
					dd.Parameters = eventDeclaration.Parameters;
					dd.ReturnType = new TypeReference("System.Void");
					dd.Parent = eventDeclaration.Parent;
					eventDeclaration.Parameters = null;
					int index = eventDeclaration.Parent.Children.IndexOf(eventDeclaration);
					// inserting before current position is not allowed in a Transformer
					eventDeclaration.Parent.Children.Insert(index + 1, dd);
					eventDeclaration.TypeReference = new TypeReference(dd.Name);
				}
			}
			return base.Visit(eventDeclaration, data);
		}
	}
}
