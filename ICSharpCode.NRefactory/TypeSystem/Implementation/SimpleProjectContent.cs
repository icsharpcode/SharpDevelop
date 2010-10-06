// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Simple <see cref="IProjectContent"/> implementation that stores the list of classes/namespaces.
	/// </summary>
	public class SimpleProjectContent : IProjectContent
	{
		ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		
		#region AssemblyAttributes
		IList<IAttribute> assemblyAttributes = new List<IAttribute>();
		
		/// <inheritdoc/>
		public IList<IAttribute> AssemblyAttributes {
			get { return assemblyAttributes; }
			protected set {
				if (value == null)
					throw new ArgumentNullException();
				assemblyAttributes = value;
			}
		}
		#endregion
		
		#region ITypeResolveContext implementation
		/// <inheritdoc/>
		public ITypeDefinition GetClass(string fullTypeName, int typeParameterCount, StringComparer nameComparer)
		{
			throw new NotImplementedException();
		}
		
		/// <inheritdoc/>
		public IEnumerable<ITypeDefinition> GetClasses()
		{
			throw new NotImplementedException();
		}
		
		/// <inheritdoc/>
		public IEnumerable<ITypeDefinition> GetClasses(string nameSpace, StringComparer nameComparer)
		{
			throw new NotImplementedException();
		}
		
		/// <inheritdoc/>
		public IEnumerable<string> GetNamespaces()
		{
			throw new NotImplementedException();
		}
		#endregion
		
		#region Synchronize
		/// <inheritdoc/>
		public virtual ISynchronizedTypeResolveContext Synchronize()
		{
			readWriteLock.EnterReadLock();
			return new ReadWriteSynchronizedTypeResolveContext(this, readWriteLock);
		}
		
		sealed class ReadWriteSynchronizedTypeResolveContext : ProxyTypeResolveContext, ISynchronizedTypeResolveContext
		{
			readonly ReaderWriterLockSlim readWriteLock;
			bool disposed;
			
			public ReadWriteSynchronizedTypeResolveContext(ITypeResolveContext context, ReaderWriterLockSlim readWriteLock)
				: base(context)
			{
				this.readWriteLock = readWriteLock;
			}
			
			public void Dispose()
			{
				if (!disposed) {
					disposed = true;
					readWriteLock.ExitReadLock();
				}
			}
		}
		#endregion
		
		#region AddType
		/// <summary>
		/// Adds a type definition to this project content.
		/// </summary>
		public virtual void AddType(ITypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			if (!typeDefinition.IsFrozen)
				throw new ArgumentException("Type definition must be frozen before it can be added to a project content");
			if (typeDefinition.ProjectContent != this)
				throw new ArgumentException("Cannot add a type definition that belongs to another project content");
			try {
				readWriteLock.EnterWriteLock();
				throw new NotImplementedException();
			} finally {
				readWriteLock.ExitWriteLock();
			}
		}
		#endregion
		
		#region RemoveType
		/// <summary>
		/// Removes a type definition from this project content.
		/// </summary>
		public virtual void RemoveType(ITypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			try {
				readWriteLock.EnterWriteLock();
				throw new NotImplementedException();
			} finally {
				readWriteLock.ExitWriteLock();
			}
		}
		#endregion
	}
}
