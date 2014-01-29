// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Completion
{
	class EntityCompletionData : CompletionData, IEntityCompletionData
	{
		readonly IEntity entity;
		
		public IEntity Entity {
			get { return entity; }
		}
		
		public EntityCompletionData(IEntity entity) : base(entity.Name)
		{
			this.entity = entity;
			this.Description = entity.Documentation;
			this.Image = ClassBrowserIconService.GetIcon(entity);
		}
		
		protected override object CreateFancyDescription()
		{
			return new FlowDocumentScrollViewer {
				Document = XmlDocFormatter.CreateTooltip(entity, false),
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
		}
	}
}
