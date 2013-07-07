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
		
		/// <summary>
		/// Updates the member model with the specified new member.
		/// </summary>
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
		
		public SymbolKind SymbolKind {
			get { return member.SymbolKind; }
		}
		
		public Accessibility Accessibility {
			get { return member.Accessibility; }
			set { throw new NotImplementedException(); }
		}
		
		public DomRegion Region {
			get { return member.Region; }
		}
		
		public string Name {
			get { return member.Name; }
		}
		
		#region Resolve
		public IMember Resolve()
		{
			var compilation = context.GetCompilation();
			return member.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		public IMember Resolve(ICompilation compilation)
		{
			return member.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		IEntity IEntityModel.Resolve()
		{
			return Resolve();
		}
		
		IEntity IEntityModel.Resolve(ICompilation compilation)
		{
			return Resolve(compilation);
		}
		#endregion
		
		public bool IsStatic {
			get { return member.IsStatic; }
		}
		
		public bool IsAbstract {
			get { return member.IsAbstract; }
		}
		
		public bool IsSealed {
			get { return member.IsSealed; }
		}
		
		public bool IsShadowing {
			get { return member.IsShadowing; }
		}
		
		public bool IsVirtual {
			get { return member.IsVirtual; }
		}
		
		public bool IsOverride {
			get { return member.IsOverride; }
		}
		
		public bool IsOverridable {
			get { return member.IsOverridable; }
		}
	}
}
