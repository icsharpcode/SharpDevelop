// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A mutable class that can track a type definition as the solution is being changed.
	/// </summary>
	sealed class TypeDefinitionModel : ITypeDefinitionModel
	{
		readonly IEntityModelContext context;
		readonly FullTypeName fullTypeName;
		List<IUnresolvedTypeDefinition> parts = new List<IUnresolvedTypeDefinition>();
		
		public TypeDefinitionModel(IEntityModelContext context, IUnresolvedTypeDefinition firstPart)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (firstPart == null)
				throw new ArgumentNullException("firstPart");
			this.context = context;
			this.parts.Add(firstPart);
			this.fullTypeName = firstPart.FullTypeName;
		}
		
		public IReadOnlyList<IUnresolvedTypeDefinition> Parts {
			get { return parts; }
		}
		
		public IProject ParentProject {
			get { return context.Project; }
		}
		
		public SymbolKind SymbolKind {
			get { return SymbolKind.TypeDefinition; }
		}
		
		public Accessibility Accessibility {
			get { 
				var td = Resolve();
				if (td != null)
					return td.Accessibility;
				else
					return Accessibility.None;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public FullTypeName FullTypeName {
			get { return fullTypeName; }
		}
		
		public DomRegion Region {
			get { return parts[0].Region; }
		}
		
		public string Name {
			get { return fullTypeName.Name; }
		}
		
		#region Resolve
		public ITypeDefinition Resolve()
		{
			var compilation = context.GetCompilation(null);
			return compilation.MainAssembly.GetTypeDefinition(fullTypeName);
		}
		
		public ITypeDefinition Resolve(ISolutionSnapshotWithProjectMapping solutionSnapshot)
		{
			var compilation = context.GetCompilation(solutionSnapshot);
			return compilation.MainAssembly.GetTypeDefinition(fullTypeName);
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
		
		public event PropertyChangedEventHandler PropertyChanged { add {} remove {} }
		
		#region Members collection
		sealed class MemberCollection : IModelCollection<MemberModel>
		{
			readonly TypeDefinitionModel parent;
			List<List<MemberModel>> lists = new List<List<MemberModel>>();
			
			public MemberCollection(TypeDefinitionModel parent)
			{
				this.parent = parent;
			}
			
			public void InsertPart(int partIndex, IUnresolvedTypeDefinition newPart)
			{
				List<MemberModel> newItems = new List<MemberModel>(newPart.Members.Count);
				foreach (var newMember in newPart.Members) {
					newItems.Add(new MemberModel(parent.context, newMember) { strongParentCollectionReference = this });
				}
				lists.Insert(partIndex, newItems);
				if (collectionChanged != null)
					collectionChanged(EmptyList<MemberModel>.Instance, newItems);
			}
			
			public void RemovePart(int partIndex)
			{
				var oldItems = lists[partIndex];
				lists.RemoveAt(partIndex);
				if (collectionChanged != null)
					collectionChanged(oldItems, EmptyList<MemberModel>.Instance);
			}
			
			public void UpdatePart(int partIndex, IUnresolvedTypeDefinition newPart)
			{
				List<MemberModel> list = lists[partIndex];
				var newMembers = newPart.Members;
				int startPos = 0;
				// Look at the initial members and update them if they're matching
				while (startPos < list.Count && startPos < newMembers.Count && IsMatch(list[startPos], newMembers[startPos])) {
					list[startPos].Update(newMembers[startPos]);
					startPos++;
				}
				// Look at the final members
				int endPosOld = list.Count - 1;
				int endPosNew = newMembers.Count - 1;
				while (endPosOld >= startPos && endPosNew >= startPos && IsMatch(list[endPosOld], newMembers[endPosNew])) {
					list[endPosOld--].Update(newMembers[endPosNew--]);
				}
				// [startPos, endPos] is the middle portion that contains all the changes
				// Add one to endPos so that it's the exclusive end of the middle portion:
				endPosOld++;
				endPosNew++;
				// [startPos, endPos)
				
				// Now we still need to update the members in between.
				// We might try to be clever here and find a LCS so that we only update the members that were actually changed,
				// or we might consider moving members around (INotifyCollectionChanged supports moves)
				// However, the easiest solution by far is to just remove + readd the whole middle portion.
				var oldItems = collectionChanged != null ? list.GetRange(startPos, endPosOld - startPos) : null;
				list.RemoveRange(startPos, endPosOld - startPos);
				var newItems = new MemberModel[endPosNew - startPos];
				for (int i = 0; i < newItems.Length; i++) {
					newItems[i] = new MemberModel(parent.context, newMembers[startPos + i]);
					newItems[i].strongParentCollectionReference = this;
				}
				list.InsertRange(startPos, newItems);
				if (collectionChanged != null && (oldItems.Count > 0 || newItems.Length > 0)) {
					collectionChanged(oldItems, newItems);
				}
			}
			
			static bool IsMatch(MemberModel memberModel, IUnresolvedMember newMember)
			{
				return memberModel.SymbolKind == newMember.SymbolKind && memberModel.Name == newMember.Name;
			}
			
			ModelCollectionChangedEventHandler<MemberModel> collectionChanged;
			
			public event ModelCollectionChangedEventHandler<MemberModel> CollectionChanged {
				add {
					collectionChanged += value;
					// Set strong reference to collection while there are event listeners
					if (collectionChanged != null)
						parent.membersStrongReference = this;
				}
				remove {
					collectionChanged -= value;
					if (collectionChanged == null)
						parent.membersStrongReference = null;
				}
			}
			
			int GetCount(int partIndex)
			{
				int count = 0;
				for (int i = 0; i < partIndex; i++) {
					count += lists[i].Count;
				}
				return count;
			}
			
			public int Count {
				get { return GetCount(lists.Count); }
			}
			
			public IReadOnlyCollection<MemberModel> CreateSnapshot()
			{
				return this.ToArray();
			}
			
			public IEnumerator<MemberModel> GetEnumerator()
			{
				return lists.SelectMany(i => i).GetEnumerator();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		
		WeakReference<MemberCollection> membersWeakReference = new WeakReference<MemberCollection>(null);
		
		/// <summary>
		/// Used to keep the member model collection alive while there are event listeners.
		/// This is necessary to prevent us from creating multiple member models for the same member.
		/// </summary>
		MemberCollection membersStrongReference;
		
		public IModelCollection<IMemberModel> Members {
			get {
				SD.MainThread.VerifyAccess();
				MemberCollection members;
				if (!membersWeakReference.TryGetTarget(out members)) {
					members = new MemberCollection(this);
					for (int i = 0; i < parts.Count; i++) {
						members.InsertPart(i, parts[i]);
					}
					membersWeakReference.SetTarget(members);
				}
				return members;
			}
		}
		#endregion
		
		#region Nested Types collection
		NestedTypeDefinitionModelCollection nestedTypes;
		
		public IModelCollection<ITypeDefinitionModel> NestedTypes {
			get {
				if (nestedTypes == null) {
					nestedTypes = new NestedTypeDefinitionModelCollection(context);
					foreach (var part in parts) {
						nestedTypes.Update(null, part.NestedTypes);
					}
				}
				return nestedTypes;
			}
		}
		
		public ITypeDefinitionModel GetNestedType(string name, int atpc)
		{
			foreach (var nestedType in this.NestedTypes) {
				if (nestedType.Name == name) {
					var nestedTypeFullName = nestedType.FullTypeName;
					if (nestedTypeFullName.GetNestedTypeAdditionalTypeParameterCount(nestedTypeFullName.NestingLevel - 1) == atpc)
						return nestedType;
				}
			}
			return null;
		}
		#endregion
		
		#region Update
		public void Update(IUnresolvedTypeDefinition oldPart, IUnresolvedTypeDefinition newPart)
		{
			SD.MainThread.VerifyAccess();
			MemberCollection members;
			membersWeakReference.TryGetTarget(out members);
			if (oldPart == null) {
				if (newPart == null) {
					// nothing changed
					return;
				}
				// Part added
				int newPartIndex = 0;
				while (newPartIndex < parts.Count && !context.IsBetterPart(newPart, parts[newPartIndex]))
					newPartIndex++;
				if (members != null)
					members.InsertPart(newPartIndex, newPart);
				parts.Insert(newPartIndex, newPart);
			} else {
				int partIndex = parts.IndexOf(oldPart);
				if (partIndex < 0)
					throw new ArgumentException("could not find old part");
				if (newPart == null) {
					// Part removed
					parts.RemoveAt(partIndex);
					if (members != null)
						members.RemovePart(partIndex);
				} else {
					// Part updated
					parts[partIndex] = newPart;
					if (members != null)
						members.UpdatePart(partIndex, newPart);
				}
			}
			if (nestedTypes != null) {
				nestedTypes.Update(oldPart != null ? oldPart.NestedTypes : null, newPart != null ? newPart.NestedTypes : null);
			}
		}
		#endregion
	}
}
