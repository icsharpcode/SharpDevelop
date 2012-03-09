// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class GoToEntityAction : IContextAction
	{
		public string DisplayName { get; private set; }
		public IEntity Entity { get; private set; }
		
		public GoToEntityAction(IEntity entity, IAmbience ambience)
		{
			if (ambience == null)
				throw new ArgumentNullException("ambience");
			if (entity == null)
				throw new ArgumentNullException("entity");
			this.Entity = entity;
			this.DisplayName = ambience.ConvertEntity(entity);
		}
		
		public Task ExecuteAsync(EditorContext context)
		{
			NavigationService.NavigateTo(this.Entity);
			return Task.FromResult<object>(null);
		}
	}
}
