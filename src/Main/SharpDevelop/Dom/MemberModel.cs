// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A mutable class that can track a member as the solution is being changed.
	/// </summary>
	sealed class MemberModel : IMemberModel
	{
		/// <summary>
		/// A strong reference to the parent TypeDefinitionModel.Members collection.
		/// This is necessary to prevent the garbage collector from
		/// freeing the weak reference to the collection while one of the member models
		/// is still in use.
		/// If we don't prevent this, the type definition model might create multiple
		/// MemberModels for the same member.
		/// </summary>
		internal IModelCollection<MemberModel> strongParentCollectionReference;
		
		readonly IEntityModelContext context;
		IUnresolvedMember member;
		
		public MemberModel(IEntityModelContext context, IUnresolvedMember member)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (member == null)
				throw new ArgumentNullException("member");
			this.context = context;
			this.member = member;
		}
		
		public event PropertyChangedEventHandler PropertyChanged { add {} remove {} }
		
		public void Update(IUnresolvedMember newMember)
		{
			if (newMember == null)
				throw new ArgumentNullException("newMember");
			this.member = newMember;
		}
		
		public IProject ParentProject {
			get { return context.Project; }
		}
		
		public IUnresolvedMember UnresolvedMember {
			get { return member; }
		}
		
		public EntityType EntityType {
			get { return member.EntityType; }
		}
		
		public DomRegion Region {
			get { return member.Region; }
		}
		
		public string Name {
			get { return member.Name; }
		}
		
		/// <summary>
		/// Gets the full type name of the type that declares this member.
		/// </summary>
		public FullTypeName DeclaringTypeName {
			get {
				if (member.DeclaringTypeDefinition != null)
					return member.DeclaringTypeDefinition.FullTypeName;
				else
					return new TopLevelTypeName(string.Empty, string.Empty);
			}
		}
		
		#region Resolve
		public IMember Resolve()
		{
			var compilation = context.GetCompilation(null);
			return member.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		public IMember Resolve(ISolutionSnapshotWithProjectMapping solutionSnapshot)
		{
			var compilation = context.GetCompilation(solutionSnapshot);
			return member.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		IEntity IEntityModel.Resolve()
		{
			return Resolve();
		}
		
		IEntity IEntityModel.Resolve(ISolutionSnapshotWithProjectMapping solutionSnapshot)
		{
			return Resolve(solutionSnapshot);
		}
		#endregion
	}
}
