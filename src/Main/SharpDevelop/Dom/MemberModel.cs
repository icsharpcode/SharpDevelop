// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		/// <summary>
		/// Updates the member model with the specified new member.
		/// </summary>
		public void Update(IUnresolvedMember newMember)
		{
			if (newMember == null)
				throw new ArgumentNullException("newMember");
			this.member = newMember;
			RaisePropertyChanged();
		}
		
		private void RaisePropertyChanged()
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this, new PropertyChangedEventArgs(null));
			}
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
