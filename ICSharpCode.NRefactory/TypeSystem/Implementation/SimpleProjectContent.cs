// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Simple <see cref="IProjectContent"/> implementation that stores the list of classes/namespaces.
	/// </summary>
	/// <remarks>
	/// This is a decorator around <see cref="TypeStorage"/> that adds support for the IProjectContent interface
	/// and for partial classes.
	/// </remarks>
	public class SimpleProjectContent : ProxyTypeResolveContext, IProjectContent
	{
		readonly TypeStorage types;
		
		#region Constructors
		/// <summary>
		/// Creates a new SimpleProjectContent.
		/// </summary>
		public SimpleProjectContent()
			: this(new TypeStorage())
		{
		}
		
		/// <summary>
		/// Creates a new SimpleProjectContent that reuses the specified type storage.
		/// </summary>
		protected SimpleProjectContent(TypeStorage types)
			: base(types)
		{
			this.types = types;
			this.assemblyAttributes = new List<IAttribute>();
			this.readOnlyAssemblyAttributes = assemblyAttributes.AsReadOnly();
		}
		#endregion
		
		#region AssemblyAttributes
		readonly List<IAttribute> assemblyAttributes;
		readonly ReadOnlyCollection<IAttribute> readOnlyAssemblyAttributes;
		
		/// <inheritdoc/>
		public virtual IList<IAttribute> AssemblyAttributes {
			get { return readOnlyAssemblyAttributes; }
		}
		
		void AddAssemblyAttributes(ICollection<IAttribute> attributes)
		{
			// API uses ICollection instead of IEnumerable to discourage users from evaluating
			// the list inside the lock (this method is called inside the ReaderWriterLock)
			assemblyAttributes.AddRange(attributes);
		}
		
		void RemoveAssemblyAttributes(ICollection<IAttribute> attributes)
		{
			assemblyAttributes.RemoveAll(attributes.Contains);
		}
		#endregion
		
		#region AddType
		void AddType(ITypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			if (!typeDefinition.IsFrozen)
				throw new ArgumentException("Type definition must be frozen before it can be added to a project content");
			if (typeDefinition.ProjectContent != this)
				throw new ArgumentException("Cannot add a type definition that belongs to another project content");
			throw new NotImplementedException();
		}
		#endregion
		
		#region RemoveType
		void RemoveType(ITypeDefinition typeDefinition)
		{
			throw new NotImplementedException();
		}
		#endregion
		
		#region UpdateProjectContent
		/// <summary>
		/// Removes oldTypes from the project, adds newTypes.
		/// Removes oldAssemblyAttributes, adds newAssemblyAttributes.
		/// </summary>
		/// <remarks>
		/// The update is done inside a write lock; when other threads access this project content
		/// from within a <c>using (Synchronize())</c> block, they will not see intermediate (inconsistent) state.
		/// </remarks>
		public void UpdateProjectContent(ICollection<ITypeDefinition> oldTypes = null,
		                                 ICollection<ITypeDefinition> newTypes = null,
		                                 ICollection<IAttribute> oldAssemblyAttributes = null,
		                                 ICollection<IAttribute> newAssemblyAttributes = null)
		{
			try {
				types.ReadWriteLock.EnterWriteLock();
				if (oldTypes != null) {
					foreach (var element in oldTypes) {
						RemoveType(element);
					}
				}
				if (newTypes != null) {
					foreach (var element in newTypes) {
						AddType(element);
					}
				}
				if (oldAssemblyAttributes != null)
					RemoveAssemblyAttributes(oldAssemblyAttributes);
				if (newAssemblyAttributes != null)
					AddAssemblyAttributes(newAssemblyAttributes);
			} finally {
				types.ReadWriteLock.ExitWriteLock();
			}
		}
		#endregion
	}
}
