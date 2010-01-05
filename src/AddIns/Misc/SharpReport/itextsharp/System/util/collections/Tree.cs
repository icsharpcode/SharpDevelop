using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// k_Tree is a red-black balanced search tree (BST) implementation.
    /// Complexity of find, insert and erase operations is near O(lg n).
    /// </summary>
    public class k_Tree : ISortedMap
    {
        #region k_Node Implementation

        private class k_Node
        {
            private object mk_Key;
            private object mk_Value;
            private bool mb_Red;
            public k_Node mk_Left, mk_Right, mk_Parent;        // public to simplify fixup & clone (passing by ref)
            
            public k_Node(object ak_Key, object ak_Value, k_Node ak_Parent)
            {
                mk_Key = ak_Key;
                mk_Value = ak_Value;
                mk_Parent = ak_Parent;
                mb_Red = true;
            }

            public object Key
            {
                get { return mk_Key; }
            }

            public object Value
            {
                get { return mk_Value; }
                set { mk_Value = value; }
            }

            public DictionaryEntry Item
            {
                get { return new DictionaryEntry(mk_Key, mk_Value); }
            }

            public bool Red
            {
                get { return mb_Red; }
                set { mb_Red = value; }
            }

            public static void SwapItems(k_Node ak_A, k_Node ak_B)
            {
                object lk_Tmp = ak_A.mk_Key;
                ak_A.mk_Key = ak_B.mk_Key;
                ak_B.mk_Key = lk_Tmp;
                
                lk_Tmp = ak_A.mk_Value;
                ak_A.mk_Value = ak_B.mk_Value;
                ak_B.mk_Value = lk_Tmp;
            }
        }

        #endregion

        #region k_NodeIterator Implementation

        private class k_NodeIterator : k_Iterator
        {
            private readonly k_Tree mk_Tree;
            private k_Node mk_Current;

            public k_NodeIterator(k_Tree ak_Tree, k_Node ak_Node)
            {
                mk_Tree = ak_Tree;
                mk_Current = ak_Node;
            }

            public override object Current
            {
                get 
                { 
                    if (mk_Current == null)
                        throw new k_InvalidPositionException();
                    return mk_Current.Item; 
                }
                set
                {
                    DictionaryEntry lr_Entry = (DictionaryEntry)value;
                    if (mk_Tree.mk_Comparer.Compare(lr_Entry.Key, mk_Current.Key) != 0)
                        throw new ArgumentException("Key values must not be changed.");
                    mk_Current.Value = lr_Entry.Value;
                }
            }

            public override object Collection
            {
                get { return mk_Tree; }
            }

            public override void Move(int ai_Count)
            {
                k_Node lk_NewPos = mk_Current;

                if (ai_Count > 0)
                {
                    while (ai_Count-- > 0)
                    {
                        if (lk_NewPos == null)
                            throw new InvalidOperationException("Tried to moved beyond end element.");

                        lk_NewPos = k_Tree.Next(lk_NewPos);
                    }
                }
                else
                {
                    while (ai_Count++ < 0)
                    {
                        if (lk_NewPos == null)
                            lk_NewPos = mk_Tree.mk_Right;
                        else
                            lk_NewPos = k_Tree.Previous(lk_NewPos);

                        if (lk_NewPos == null)
                            throw new InvalidOperationException("Tried to move before first element.");
                    }
                }
                
                mk_Current = lk_NewPos;
            }

            public override int Distance(k_Iterator ak_Iter)
            {
                k_NodeIterator lk_Iter = ak_Iter as k_NodeIterator;
                if (lk_Iter == null || !object.ReferenceEquals(lk_Iter.Collection, this.Collection))
                    throw new ArgumentException("Cannot determine distance of iterators belonging to different collections.");
                
                k_Iterator lk_End = mk_Tree.End;
    
                int li_KeyDiff;
                if (this == lk_End || ak_Iter == lk_End)
                    li_KeyDiff = (this == lk_End && this != ak_Iter) ? 1 : 0;
                else
                    li_KeyDiff = mk_Tree.mk_Comparer.Compare(mk_Current.Key, lk_Iter.mk_Current.Key);

                if (li_KeyDiff <= 0)
                {
                    int li_Diff = 0;
                    k_Iterator lk_Bck = this.Clone(); 
                    for (; lk_Bck != ak_Iter && lk_Bck != lk_End; lk_Bck.Next())
                        --li_Diff;

                    if (lk_Bck == ak_Iter)
                        return li_Diff;
                }
                
                if (li_KeyDiff >= 0)
                {
                    int li_Diff = 0;
                    k_Iterator lk_Fwd = ak_Iter.Clone(); 
                    for (; lk_Fwd != this && lk_Fwd != lk_End; lk_Fwd.Next())
                        ++li_Diff;

                    if (lk_Fwd == this)
                        return li_Diff;
                }
                
                throw new Exception("Inconsistent state. Concurrency?");
            }

            public override bool Equals(object ak_Obj)
            {
                k_NodeIterator lk_Iter = ak_Obj as k_NodeIterator;
                if (lk_Iter == null)
                    return false;
                return object.ReferenceEquals(mk_Current, lk_Iter.mk_Current);
            }

            public override int GetHashCode()
            {
                if (mk_Current == null)
                    return mk_Tree.GetHashCode();
                return mk_Current.GetHashCode();
            }

            public override k_Iterator Clone()
            {
                return new k_NodeIterator(mk_Tree, mk_Current);
            }

            internal k_Node Node
            {
                get { return mk_Current; }
            }
        }

        private class k_PinnedNodeIterator : k_NodeIterator
        {
            public k_PinnedNodeIterator(k_Tree ak_Tree, k_Node ak_Node)
                : base(ak_Tree, ak_Node)
            {
            }

            public override void Move(int ai_Count)
            {
                throw new k_IteratorPinnedException();
            }
        }

        #endregion

        private k_Node mk_Head, mk_Left, mk_Right;
        private k_Iterator mk_End;
        private int mi_Count;
        private IComparer mk_Comparer;
        private bool mb_AllowDuplicateKeys;

        public k_Tree()
            : this(false)
        {
        }

        public k_Tree(bool ab_AllowDuplicateKeys)
            : this(ab_AllowDuplicateKeys, System.Collections.Comparer.Default)
        {
        }

        public k_Tree(bool ab_AllowDuplicateKeys, IComparer ak_Comparer)
        {
            mb_AllowDuplicateKeys = ab_AllowDuplicateKeys;
            mk_Comparer = ak_Comparer;
            mk_End = new k_PinnedNodeIterator(this, null);
        }

        // IContainer Members
        public k_Iterator Begin
        {
            get 
            {
                if (mi_Count == 0) 
                    return this.End;
                return new k_NodeIterator(this, mk_Left);
            }
        }

        public k_Iterator End
        {
            get { return mk_End; }
        }

        public bool IsEmpty
        {
            get { return (mi_Count == 0); }
        }

        public k_Iterator Find(object ak_Value)
        {
            DictionaryEntry lr_Item = (DictionaryEntry)ak_Value;
            k_Node lk_Found = FindInternal(mk_Head, lr_Item.Key);
            if (lk_Found != null && mk_Comparer.Compare(lk_Found.Value, lr_Item.Value) == 0)
                return new k_NodeIterator(this, lk_Found);
            return this.End;
        }

        public k_Iterator Erase(k_Iterator ak_Where)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this tree.");
            k_Iterator lk_Successor = ak_Where + 1;
            RemoveNode(((k_NodeIterator)ak_Where).Node);
            return lk_Successor;
        }

        public k_Iterator Erase(k_Iterator ak_First, k_Iterator ak_Last)
        {
            if (ak_First == this.Begin && ak_Last == this.End)
            {
                Clear();
                return ak_Last.Clone();
            }
            
            k_Iterator lk_Current = ak_First;
            while (lk_Current != ak_Last)
                lk_Current = Erase(lk_Current);
            return lk_Current;
        }

        // IMap Members
        public void Add(DictionaryEntry ar_Item)
        {
            Add(ar_Item.Key, ar_Item.Value);
        }

        public k_Iterator FindKey(object ak_Key)
        {
            return new k_NodeIterator(this, FindInternal(mk_Head, ak_Key));
        }

        public void Insert(k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd)
        {
            for (k_Iterator lk_Iter = ak_SrcBegin.Clone(); lk_Iter != ak_SrcEnd; lk_Iter.Next())
                Add((DictionaryEntry)lk_Iter.Current);
        }

        // ISortedMap Members
        public IComparer Comparer 
        { 
            get { return mk_Comparer; }
        }

        public k_Iterator LowerBound(object ak_Key)
        {
            k_Node lk_Node = mk_Head;
            k_Node lk_Found = null;
            while (lk_Node != null)
            {
                if (mk_Comparer.Compare(lk_Node.Key, ak_Key) < 0)
                    lk_Node = lk_Node.mk_Right;
                else
                {
                    lk_Found = lk_Node;
                    lk_Node = lk_Node.mk_Left;
                }
            }
            return new k_NodeIterator(this, lk_Found);
        }

        public k_Iterator UpperBound(object ak_Key)
        {
            k_Node lk_Node = mk_Head;
            k_Node lk_Found = null;
            while (lk_Node != null)
            {
                if (mk_Comparer.Compare(lk_Node.Key, ak_Key) > 0)
                {
                    lk_Found = lk_Node;
                    lk_Node = lk_Node.mk_Left;
                }
                else
                    lk_Node = lk_Node.mk_Right;
            }
            return new k_NodeIterator(this, lk_Found);
        }

        #region IDictionary Members

        public void Add(object ak_Key, object ak_Value)
        {
            Insert(ref mk_Head, null, ak_Key, ak_Value, false);
            mk_Head.Red = false;
            ++mi_Count;
        }
        
        public void Clear()
        {
            mi_Count = 0;
            mk_Head = null;
            mk_Left = null;
            mk_Right = null;
        }

        public bool Contains(object ak_Key)
        {
            return (FindInternal(mk_Head, ak_Key) != null);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new k_IteratorDictEnumerator(this.Begin, this.End);
        }

        public void Remove(object ak_Key)
        {
            RemoveNode(FindInternal(mk_Head, ak_Key));
        }

        public bool IsFixedSize
        {
            get { return false; }
        }
        
        public bool IsReadOnly
        {
            get { return false; }
        }

        public object this[object ak_Key]
        {
            get 
            {
                k_Node lk_Node = FindInternal(mk_Head, ak_Key);
                if (lk_Node == null)
                    return null;
                return lk_Node.Value;
            }
            set 
            {
                k_Node lk_Node = FindInternal(mk_Head, ak_Key);
                if (lk_Node == null)
                    Add(new DictionaryEntry(ak_Key, value));
                else
                    lk_Node.Value = value;
            }
        }

        public ICollection Keys
        {
            get
            {
                int i=0;
                object[] lk_Keys = new object[mi_Count];
                foreach (DictionaryEntry lr_Entry in this)
                    lk_Keys[i++] = lr_Entry.Key;
                return lk_Keys;
            }
        }

        public ICollection Values
        {
            get
            {
                int i=0;
                object[] lk_Values = new object[mi_Count];
                foreach (DictionaryEntry lr_Entry in this)
                    lk_Values[i++] = lr_Entry.Value;
                return lk_Values;
            }
        }
            
        #endregion

        #region ICollection Members

        public void CopyTo(Array ak_Array, int ai_Index)
        {
            foreach (DictionaryEntry lr_Entry in this)
                ak_Array.SetValue(lr_Entry, ai_Index++);
        }

        public int Count
        {
            get { return mi_Count; }
        }

        public bool IsSynchronized
        {
            get {  return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new k_IteratorEnumerator(this.Begin, this.End);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            k_Tree lk_Clone = new k_Tree(mb_AllowDuplicateKeys, mk_Comparer);
            lk_Clone.mi_Count = mi_Count;
            CloneRecursive(mk_Head, null, ref lk_Clone.mk_Head);
            lk_Clone.mk_Left = k_Tree.LeftMost(lk_Clone.mk_Head);
            lk_Clone.mk_Right = k_Tree.RightMost(lk_Clone.mk_Head);

            return lk_Clone;
        }

        #endregion

        private void CloneRecursive(k_Node ak_Node, k_Node ak_Parent, ref k_Node ak_Link)
        {
            if (ak_Node == null)
                return;

            ak_Link = new k_Node(ak_Node.Key, ak_Node.Value, ak_Parent);
            ak_Link.Red = ak_Node.Red;

            CloneRecursive(ak_Node.mk_Left, ak_Link, ref ak_Link.mk_Left);
            CloneRecursive(ak_Node.mk_Right, ak_Link, ref ak_Link.mk_Right);
        }

        private bool IsRed(k_Node ak_Node)
        {
            return (ak_Node != null && ak_Node.Red);
        }

        private k_Node FindInternal(k_Node ak_Node, object ak_Key)
        {
            while (ak_Node != null)
            {
                int li_Diff = mk_Comparer.Compare(ak_Key, ak_Node.Key);
                if (li_Diff == 0)
                    return ak_Node;
                ak_Node = (li_Diff < 0) ? ak_Node.mk_Left : ak_Node.mk_Right;
            }
            return null;
        }

        /// <summary>
        /// Return leftmost node in subtree.
        /// </summary>
        /// <param name="ak_Node">Node where to start search</param>
        /// <returns>Found node</returns>
        private static k_Node LeftMost(k_Node ak_Node)
        {
            if (ak_Node == null)
                return null;
            while (ak_Node.mk_Left != null)
                ak_Node = ak_Node.mk_Left;
            return ak_Node;
        }

        /// <summary>
        /// Return rightmost node in subtree.
        /// </summary>
        /// <param name="ak_Node">Node where to start search</param>
        /// <returns>Found node</returns>
        private static k_Node RightMost(k_Node ak_Node)
        {
            if (ak_Node == null)
                return null;
            while (ak_Node.mk_Right != null)
                ak_Node = ak_Node.mk_Right;
            return ak_Node;
        }

        private static k_Node Previous(k_Node ak_Node) // the next smaller
        {
            if (ak_Node.mk_Left != null)
                return RightMost(ak_Node.mk_Left);

            k_Node lk_Parent = ak_Node.mk_Parent;
            while (lk_Parent != null && lk_Parent.mk_Left == ak_Node)
            {
                ak_Node = lk_Parent;
                lk_Parent = lk_Parent.mk_Parent;
            }
            return lk_Parent;
        }

        private static k_Node Next(k_Node ak_Node)
        {
            if (ak_Node.mk_Right != null)
                return LeftMost(ak_Node.mk_Right);
            
            k_Node lk_Parent = ak_Node.mk_Parent;
            while (lk_Parent != null && lk_Parent.mk_Right == ak_Node)
            {
                ak_Node = lk_Parent;
                lk_Parent = lk_Parent.mk_Parent;
            }
            return lk_Parent;
        }

        private void RemoveNode(k_Node ak_Node)
        {
            if (ak_Node == null)
                return;
            if (ak_Node == mk_Head)
                UnlinkNode(ref mk_Head);
            else  if (ak_Node == ak_Node.mk_Parent.mk_Right)
                UnlinkNode(ref ak_Node.mk_Parent.mk_Right);
            else
                UnlinkNode(ref ak_Node.mk_Parent.mk_Left);
        }

        private void UnlinkNode(ref k_Node ak_Node)
        {
            bool lb_Red = ak_Node.Red;
            k_Node lk_Erased = ak_Node;

            k_Node lk_PatchNode = null;
            if (ak_Node.mk_Right == null)
                lk_PatchNode = ak_Node.mk_Left;
            else if (ak_Node.mk_Left == null)
                lk_PatchNode = ak_Node.mk_Right;
            else 
                lk_PatchNode = ak_Node;

            k_Node lk_PatchParent = null, lk_FixNode = null;
            if (lk_PatchNode == null)
            {
                lk_PatchParent = ak_Node.mk_Parent;
                ak_Node = null;
            }
            else if (lk_PatchNode != ak_Node)
            {
                lk_PatchNode.mk_Parent = ak_Node.mk_Parent;
                ak_Node = lk_PatchNode;
                lk_PatchParent = lk_PatchNode.mk_Parent;
            }
            else
            {
                // two subtrees
                lk_PatchNode = RightMost(ak_Node.mk_Left);
                if (lk_PatchNode.mk_Parent.mk_Right == lk_PatchNode)
                    lk_PatchNode.mk_Parent.mk_Right = lk_PatchNode.mk_Left;
                else
                    lk_PatchNode.mk_Parent.mk_Left = lk_PatchNode.mk_Left;
                
                lb_Red = lk_PatchNode.Red;
                if (lk_PatchNode.mk_Left != null)
                    lk_PatchNode.mk_Left.mk_Parent = lk_PatchNode.mk_Parent;
                
                lk_PatchParent = lk_PatchNode.mk_Parent;
                lk_FixNode = lk_PatchNode.mk_Left;

                k_Node.SwapItems(ak_Node, lk_PatchNode);
            
                // ensure that mk_Left and/or mk_Right are corrected after unlink
                lk_Erased = lk_PatchNode;
            }

            if (!lb_Red && lk_PatchParent != null)
            {
                // erased node was black link - rebalance the tree
                while (!IsRed(lk_FixNode) && lk_FixNode != mk_Head)
                {
                    if (lk_PatchParent.mk_Left != null || lk_PatchParent.mk_Right != null)
                    {
                        if (lk_PatchParent.mk_Left == lk_FixNode)
                        {
                            // fixup right subtree
                            k_Node lk_Node = lk_PatchParent.mk_Right;

                            if (IsRed(lk_Node))
                            {
                                lk_Node.Red = false;
                                lk_PatchParent.Red = true;
                                RotateLeft(lk_PatchParent);
                                lk_Node = lk_PatchParent.mk_Right;
                            }

                            if (lk_Node != null)
                            {
                                if (!IsRed(lk_Node.mk_Left) && !IsRed(lk_Node.mk_Right))
                                    lk_Node.Red = true;
                                else
                                {
                                    if (!IsRed(lk_Node.mk_Right))
                                    {
                                        lk_Node.Red = true;
                                        lk_Node.mk_Left.Red = false;
                                        RotateRight(lk_Node);
                                        lk_Node = lk_PatchParent.mk_Right;
                                    }

                                    lk_Node.Red = lk_PatchParent.Red;
                                    lk_PatchParent.Red = false;
                                    lk_Node.mk_Right.Red = false;
                                    RotateLeft(lk_PatchParent);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // fixup leftsubtree
                            k_Node lk_Node = lk_PatchParent.mk_Left;

                            if (IsRed(lk_Node))
                            {
                                lk_Node.Red = false;
                                lk_PatchParent.Red = true;
                                RotateRight(lk_PatchParent);
                                lk_Node = lk_PatchParent.mk_Left;
                            }

                            if (lk_Node != null)
                            {
                                if (!IsRed(lk_Node.mk_Left) && !IsRed(lk_Node.mk_Right))
                                    lk_Node.Red = true;
                                else
                                {
                                    if (!IsRed(lk_Node.mk_Left))
                                    {
                                        lk_Node.Red = true;
                                        lk_Node.mk_Right.Red = false;
                                        RotateLeft(lk_Node);
                                        lk_Node = lk_PatchParent.mk_Left;
                                    }

                                    lk_Node.Red = lk_PatchParent.Red;
                                    lk_PatchParent.Red = false;
                                    lk_Node.mk_Left.Red = false;
                                    RotateRight(lk_PatchParent);
                                    break;
                                }
                            }
                        }
                    }
                    lk_FixNode = lk_PatchParent;
                    lk_PatchParent = lk_PatchParent.mk_Parent;
                }

                if (lk_FixNode != null)
                    lk_FixNode.Red = false;
            }

            --mi_Count;

            if (object.ReferenceEquals(lk_Erased, mk_Right))
                mk_Right = k_Tree.RightMost(mk_Head);
            if (object.ReferenceEquals(lk_Erased, mk_Left))
                mk_Left = k_Tree.LeftMost(mk_Head);
        }

        private void Insert(ref k_Node ak_Node, k_Node ak_Parent, object ak_Key, object ak_Value, bool ab_RightMove)
        {
            if (ak_Node == null)
            {
                ak_Node = new k_Node(ak_Key, ak_Value, ak_Parent);
                if (object.ReferenceEquals(ak_Parent, mk_Right) && (ak_Parent == null || ab_RightMove))
                    mk_Right = ak_Node;
                if (object.ReferenceEquals(ak_Parent, mk_Left) && (ak_Parent == null || !ab_RightMove))
                    mk_Left = ak_Node;
                return;
            }

            if (IsRed(ak_Node.mk_Left) && IsRed(ak_Node.mk_Right))
            {
                ak_Node.Red = true;
                ak_Node.mk_Left.Red = false;
                ak_Node.mk_Right.Red = false;
            }

            int li_Diff = mk_Comparer.Compare(ak_Key, ak_Node.Key);
            if (!mb_AllowDuplicateKeys && li_Diff == 0)
                throw new ArgumentException("An element with the same key already exists in the tree.");

            if (li_Diff < 0)
            {
                Insert(ref ak_Node.mk_Left, ak_Node, ak_Key, ak_Value, false);
                if (IsRed(ak_Node) && IsRed(ak_Node.mk_Left) && ab_RightMove)
                    ak_Node = RotateRight(ak_Node);
                if (IsRed(ak_Node.mk_Left) && IsRed(ak_Node.mk_Left.mk_Left))
                {
                    ak_Node = RotateRight(ak_Node);
                    ak_Node.Red = false;
                    ak_Node.mk_Right.Red = true;
                }
            }
            else
            {
                Insert(ref ak_Node.mk_Right, ak_Node, ak_Key, ak_Value, true);
                if (IsRed(ak_Node) && IsRed(ak_Node.mk_Right) && !ab_RightMove)
                    ak_Node = RotateLeft(ak_Node);
                if (IsRed(ak_Node.mk_Right) && IsRed(ak_Node.mk_Right.mk_Right))
                {
                    ak_Node = RotateLeft(ak_Node);
                    ak_Node.Red = false;
                    ak_Node.mk_Left.Red = true;
                }
            }
        }

        /*
         A right rotation: ak_Node.Left takes old position of ak_Node.
         Makes the old root the right subtree of the new root.
         
              5                 2
           2     7     ->    1     5  
          1 3   6 8              3   7
                                    6 8
        */
        private k_Node RotateRight(k_Node ak_Node)
        {
            k_Node lk_Tmp = ak_Node.mk_Left;

            lk_Tmp.mk_Parent = ak_Node.mk_Parent;
            ak_Node.mk_Parent = lk_Tmp;

            ak_Node.mk_Left = lk_Tmp.mk_Right;
            if (ak_Node.mk_Left != null)
                ak_Node.mk_Left.mk_Parent = ak_Node;
            lk_Tmp.mk_Right = ak_Node;

            // correct parent
            if (lk_Tmp.mk_Parent == null)
                mk_Head = lk_Tmp;
            else if (lk_Tmp.mk_Parent.mk_Right == ak_Node)
                lk_Tmp.mk_Parent.mk_Right = lk_Tmp;
            else
                lk_Tmp.mk_Parent.mk_Left = lk_Tmp;

            return lk_Tmp;
        }

        /*
         A left rotation: ak_Node.Right takes old position of ak_Node.
         Makes the old root the left subtree of the new root.
         
              5                   7
           2     7     ->      5     8
          1 3   6 8          2   6
                            1 3      
        */
        private k_Node RotateLeft(k_Node ak_Node)
        {
            k_Node lk_Tmp = ak_Node.mk_Right;

            lk_Tmp.mk_Parent = ak_Node.mk_Parent;
            ak_Node.mk_Parent = lk_Tmp;

            ak_Node.mk_Right = lk_Tmp.mk_Left;
            if (ak_Node.mk_Right != null)
                ak_Node.mk_Right.mk_Parent = ak_Node;
            lk_Tmp.mk_Left = ak_Node;

            // correct parent
            if (lk_Tmp.mk_Parent == null)
                mk_Head = lk_Tmp;
            else if (lk_Tmp.mk_Parent.mk_Right == ak_Node)
                lk_Tmp.mk_Parent.mk_Right = lk_Tmp;
            else
                lk_Tmp.mk_Parent.mk_Left = lk_Tmp;

            return lk_Tmp;
        }
    }
}
