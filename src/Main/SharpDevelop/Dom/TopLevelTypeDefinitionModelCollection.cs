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
	sealed class TopLevelTypeDefinitionModelCollection : KeyedModelCollection<TopLevelTypeName, TypeDefinitionModel>
	{
		readonly IEntityModelContext context;
		
		public TopLevelTypeDefinitionModelCollection(IEntityModelContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
		}
		
		public TypeDefinitionModel this[FullTypeName fullTypeName] {
			get {
				TypeDefinitionModel model = base[fullTypeName.TopLevelTypeName];
				for (int i = 0; i < fullTypeName.NestingLevel; i++) {
					throw new NotImplementedException();
				}
				return model;
			}
		}
		
		/// <summary>
		/// Updates the parse information.
		/// </summary>
		public void NotifyParseInformationChanged(IUnresolvedFile oldFile, IUnresolvedFile newFile)
		{
			if (oldFile != null) {
				
			}
		}
		
		protected override TopLevelTypeName GetKeyForItem(TypeDefinitionModel item)
		{
			return item.FullTypeName.TopLevelTypeName;
		}
	}
}
