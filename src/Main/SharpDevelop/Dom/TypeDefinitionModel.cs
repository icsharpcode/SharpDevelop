// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

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
		List<IUnresolvedTypeDefinition> parts;
		
		public TypeDefinitionModel(IEntityModelContext context, params IUnresolvedTypeDefinition[] parts)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (parts.Length == 0)
				throw new ArgumentException("Number of parts must not be zero");
			this.context = context;
			this.parts = new List<IUnresolvedTypeDefinition>(parts);
			MovePrimaryPartToFront();
			this.fullTypeName = parts[0].FullTypeName;
		}
		
		void MovePrimaryPartToFront()
		{
			int bestPartIndex = 0;
			for (int i = 1; i < parts.Count; i++) {
				if (context.IsBetterPart(parts[i], parts[bestPartIndex]))
					bestPartIndex = i;
			}
			IUnresolvedTypeDefinition primaryPart = parts[bestPartIndex];
			for (int i = bestPartIndex; i > 0; i--) {
				parts[i] = parts[i - 1];
			}
			parts[0] = primaryPart;
		}
		
		/*
		/// <summary>
		/// Updates the type definition model by removing the old parts and adding the new ones.
		/// </summary>
		public void Update(IReadOnlyList<IUnresolvedTypeDefinition> removedParts, IReadOnlyList<IUnresolvedTypeDefinition> newParts)
		{
			SD.MainThread.VerifyAccess();
			if (removedParts != null)
				foreach (var p in removedParts)
					parts.Remove(p);
			if (newParts != null)
				parts.AddRange(newParts);
			
			MemberModelCollection members;
			if (membersWeakReference.TryGetTarget(out members)) {
				members.Update(parts);
			}
		}*/
		
		public IProject ParentProject {
			get { return context.Project; }
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
			
			public void Insert(int partIndex, IUnresolvedTypeDefinition newPart)
			{
				List<MemberModel> newItems = new List<MemberModel>(newPart.Members.Count);
				foreach (var newMember in newPart.Members) {
					newItems.Add(new MemberModel(parent.context, newMember) { strongParentCollectionReference = this });
				}
				lists.Insert(partIndex, newItems);
				if (collectionChanged != null)
					collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, GetCount(partIndex)));
			}
			
			public void Remove(int partIndex)
			{
				var oldItems = lists[partIndex];
				lists.RemoveAt(partIndex);
				if (collectionChanged != null)
					collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, GetCount(partIndex)));
			}
			
			public void Update(int partIndex, IUnresolvedTypeDefinition newPart)
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
				if (collectionChanged != null)
					collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems, GetCount(partIndex) + startPos));
			}
			
			static bool IsMatch(MemberModel memberModel, IUnresolvedMember newMember)
			{
				return memberModel.EntityType == newMember.EntityType && memberModel.Name == newMember.Name;
			}
			
			NotifyCollectionChangedEventHandler collectionChanged;
			
			public event NotifyCollectionChangedEventHandler CollectionChanged {
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
			
			public IEnumerator<MemberModel> GetEnumerator()
			{
				return lists.SelectMany(i => i).GetEnumerator();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			public MemberModel this[int index] {
				get {
					int inputIndex = 0;
					while (index >= lists[inputIndex].Count) {
						index -= lists[inputIndex].Count;
						inputIndex++;
					}
					return lists[inputIndex][index];
				}
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
					membersWeakReference.SetTarget(members);
				}
				return members;
			}
		}
		#endregion
		
		public IModelCollection<ITypeDefinitionModel> NestedTypes {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
