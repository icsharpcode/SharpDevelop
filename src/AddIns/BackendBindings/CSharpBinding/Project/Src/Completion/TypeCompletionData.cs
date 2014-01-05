// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Completion
{
	class TypeCompletionData : CompletionData
	{
		readonly IType type;
		
		public IType Type {
			get { return type; }
		}
		
		public TypeCompletionData(IType type) : base(type.Name)
		{
			this.type = type;
			ITypeDefinition typeDef = type.GetDefinition();
			if (typeDef != null)
				this.Description = typeDef.Documentation;
			this.Image = ClassBrowserIconService.GetIcon(type);
		}
		
		protected override object CreateFancyDescription()
		{
			return new FlowDocumentScrollViewer {
				Document = XmlDocFormatter.CreateTooltip(type),
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
		}
	}
}
