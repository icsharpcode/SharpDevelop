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
		
		public EntityCompletionData(IEntity entity)
		{
			this.entity = entity;
			this.CompletionText = entity.Name;
			this.DisplayText = entity.Name;
			this.Description = entity.Documentation;
			this.Image = ClassBrowserIconService.GetIcon(entity);
		}
		
		List<ICompletionData> overloads = new List<ICompletionData>();
		
		public override void AddOverload(ICompletionData data)
		{
			overloads.Add(data);
		}
		
		public override bool HasOverloads {
			get { return overloads.Count > 0; }
		}
		
		public override IEnumerable<ICompletionData> OverloadedData {
			get { return overloads; }
		}
	}
}
