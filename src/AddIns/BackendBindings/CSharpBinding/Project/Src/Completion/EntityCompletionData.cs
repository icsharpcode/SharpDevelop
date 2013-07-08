// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

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
	}
}
