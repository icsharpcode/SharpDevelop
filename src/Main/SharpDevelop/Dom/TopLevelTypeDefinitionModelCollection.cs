// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A TypeDefinitionModel-collection that holds models for all top-level types in a project content.
	/// </summary>
	sealed class TopLevelTypeDefinitionModelCollection : KeyedModelCollection<TopLevelTypeName, ITypeDefinitionModel>, IMutableTypeDefinitionModelCollection
	{
		readonly IEntityModelContext context;
		
		public TopLevelTypeDefinitionModelCollection(IEntityModelContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
		}
		
		public ITypeDefinitionModel this[FullTypeName fullTypeName] {
			get {
				ITypeDefinitionModel model = base[fullTypeName.TopLevelTypeName];
				for (int i = 0; i < fullTypeName.NestingLevel; i++) {
					throw new NotImplementedException();
				}
				return model;
			}
		}
		
		/// <summary>
		/// Updates the parse information.
		/// </summary>
		public void Update(IUnresolvedFile oldFile, IUnresolvedFile newFile)
		{
			if (oldFile != null) {
				
			}
			throw new NotImplementedException();
		}
		
		protected override TopLevelTypeName GetKeyForItem(ITypeDefinitionModel item)
		{
			return item.FullTypeName.TopLevelTypeName;
		}
	}
}
