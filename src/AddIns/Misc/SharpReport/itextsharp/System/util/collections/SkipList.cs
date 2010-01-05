using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// A Skip List
    /// </summary>
    public class k_SkipList : ISortedMap
    {
        #region k_Node Implementation

        private class k_Node
        {
            private object mk_Key;
            private object mk_Value;

            private k_Node[] mk_Next;

            public k_Node(object ak_Key, object ak_Value, int ai_Height)
            {
                mk_Next = new k_Node[ai_Height];
                mk_Key = ak_Key;
                mk_Value = ak_Value;
            }

            public object Key
            {
                get { return mk_Key; }
            }

            public object Value
            {
                get { return mk_Value; }
                set { mk_Value = Value; }
            }

            public DictionaryEntry Item
            {
                get { return new DictionaryEntry(mk_Key, mk_Value); }
            }

            public k_Node[] Next
            {
                get { return mk_Next; }
            }

            public int Height
            {
                get { return mk_Next.Length; }
            }
        }

        #endregion

        #region k_NodeIterator Implementation

        private class k_NodeIterator : k_Iterator
        {
            private readonly k_SkipList mk_List;
            private k_Node mk_Current;

            public k_NodeIterator(k_SkipList ak_List, k_Node ak_Node)
            {
                mk_List = ak_List;
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
                    if (mk_List.mk_Comparer.Compare(lr_Entry.Key, mk_Current.Key) != 0)
                        throw new ArgumentException("Key values must not be changed.");
                    mk_Current.Value = lr_Entry.Value;
                }
            }

            public override object Collection
            {
                get { return mk_List; }
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

                        lk_NewPos = mk_List.Next(lk_NewPos);
                    }
                }
                else
                {
                    while (ai_Count++ < 0)
                    {
                        if (lk_NewPos == null)
                            lk_NewPos = mk_List.RightMost();
                        else
                            lk_NewPos = mk_List.Previous(lk_NewPos);

                        if (lk_NewPos == null)
                            throw new InvalidOperationException("Tried to move before first element.");
                    }
                }

                mk_Current = lk_NewPos;
            }

            public override int Distance(k_Iterator ak_Iter)
            {
                k_NodeIterator lk_Iter = (k_NodeIterator)ak_Iter;
                k_Iterator lk_End = mk_List.End;
    
                int li_KeyDiff;
                if (this == lk_End || ak_Iter == lk_End)
                    li_KeyDiff = (this == lk_End && this != ak_Iter) ? 1 : 0;
                else
                    li_KeyDiff = mk_List.mk_Comparer.Compare(mk_Current.Key, lk_Iter.mk_Current.Key);

                if (li_KeyDiff <= 0)
                {
                    int li_Diff = 0;
                    k_Iterator lk_Bck = this.Clone(); 
                    for (; lk_Bck != lk_Iter && lk_Bck != lk_End; lk_Bck.Next())
                        --li_Diff;

                    if (lk_Bck == lk_Iter)
                        return li_Diff;
                }
                
                if (li_KeyDiff >= 0)
                {
                    int li_Diff = 0;
                    k_Iterator lk_Fwd = lk_Iter.Clone(); 
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
                    return mk_List.GetHashCode();
                return mk_Current.GetHashCode();
            }

            public override k_Iterator Clone()
            {
                return new k_NodeIterator(mk_List, mk_Current);
            }

            internal k_Node Node
            {
                get { return mk_Current; }
            }
        }

        private class k_PinnedNodeIterator : k_NodeIterator
        {
            public k_PinnedNodeIterator(k_SkipList ak_List, k_Node ak_Node)
                : base(ak_List, ak_Node)
            {
            }

            public override void Move(int ai_Count)
            {
                throw new k_IteratorPinnedException();
            }
        }

        #endregion

        private static Random mk_Rand = new Random();

        private IComparer mk_Comparer;
        private double md_Prob;
        private int mi_MaxLevel;
        private int mi_HighestNode;
        private k_Node mk_Head;
        private int mi_Count;
        private k_Iterator mk_End;

        public k_SkipList()
            : this(System.Collections.Comparer.Default)
        {
        }

        public k_SkipList(IComparer ak_Comparer)
            : this(ak_Comparer, 1.0/Math.E, 16)
        {
        }

        public k_SkipList(IComparer ak_Comparer, double ad_Prob, int ai_MaxLevel)
        {
            if (ad_Prob >= 1.0 || ad_Prob <= 0)
                throw new ArgumentException("Invalid probability. Must be (0-1).", "ad_Prob");
            md_Prob = ad_Prob;
            mi_MaxLevel = ai_MaxLevel;
            mk_Comparer = ak_Comparer;
            mk_Head = new k_Node(null, null, ai_MaxLevel);
            mk_End = new k_PinnedNodeIterator(this, null);
        }

        // IContainer Members
        public k_Iterator Begin
        {
            get 
            {
                if (mi_Count == 0) 
                    return this.End;
                return new k_NodeIterator(this, this.LeftMost());
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
            k_NodeIterator lk_Found = (k_NodeIterator)LowerBound(lr_Item.Key);
            if (lk_Found != this.End
                && mk_Comparer.Compare(lr_Item.Key, lk_Found.Node.Key) == 0 && mk_Comparer.Compare(lr_Item.Value, lk_Found.Node.Value) == 0)
                return lk_Found;
            return this.End;
        }

        public k_Iterator Erase(k_Iterator ak_Where)
        {
            return Erase(ak_Where, ak_Where+1);
        }

        public k_Iterator Erase(k_Iterator ak_First, k_Iterator ak_Last)
        {
            if (ak_First == ak_Last)
                return ak_Last.Clone();
            
            int li_Count = ak_Last - ak_First;
            
            k_Node lk_First = ((k_NodeIterator)ak_First).Node;
            k_Node lk_Last = (ak_Last != this.End) ? ((k_NodeIterator)ak_Last).Node : null;
            k_Node lk_Node = new k_Node(null, null, mi_HighestNode);

            k_Node lk_Current = mk_Head;
            for (int li_Level = mi_HighestNode-1; li_Level >= 0; --li_Level)
            {
                while (lk_Current.Next[li_Level] != null)
                {
                    if (ComparePos(lk_Current.Next[li_Level], lk_First) >= 0)
                        break;
                    lk_Current = lk_Current.Next[li_Level];
                }
                lk_Node.Next[li_Level] = lk_Current;
            }

            if (lk_Last == null)
            {
                for (int i=0; i<lk_Node.Height; ++i)
                {
                    k_Node lk_Left = lk_Node.Next[i];
                    lk_Left.Next[i] = null;
                }
            }
            else
            {
                for (int i=0; i<lk_Node.Height; ++i)
                {
                    k_Node lk_Left = lk_Node.Next[i];

                        // for each level skip over erased range
                    lk_Current = lk_Left.Next[i];
                    while (lk_Current != null)
                    {
                        if (ComparePos(lk_Current, lk_Last) >= 0)
                            break;
                        lk_Current = lk_Current.Next[i];
                    }
                    lk_Left.Next[i] = lk_Current;
                }
            }

            mi_Count -= li_Count;
 
            while (mi_HighestNode > 0 && mk_Head.Next[mi_HighestNode-1] == null)
                --mi_HighestNode;

            return ak_Last;
        }

        // IMap Members
        public k_Iterator FindKey(object ak_Key)
        {
            k_NodeIterator lk_Found = (k_NodeIterator)LowerBound(ak_Key);
            if (lk_Found != this.End && mk_Comparer.Compare(ak_Key, lk_Found.Node.Key) == 0)
                return lk_Found;
            return this.End;
        }

        public void Add(DictionaryEntry ar_Entry)
        {
            Add(ar_Entry.Key, ar_Entry.Value);
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

        /// <summary>
        /// Returns an iterator to the first element in a list with a key value 
        /// that is equal to or greater than that of a specified key.
        /// </summary>
        /// <param name="ak_Key">
        /// The argument key value to be compared with the sort key of an element 
        /// from the list being searched.
        /// </param>
        /// <returns>
        /// Location of an element in a list that with a key that is equal to 
        /// or greater than the argument key, or this.End if no match is found for the key.
        /// </returns>
        public k_Iterator LowerBound(object ak_Key)
        {
            k_Node lk_Found = null;
            k_Node lk_Current = mk_Head;
            for (int li_Level = mi_HighestNode-1; li_Level >= 0; --li_Level)
            {
                k_Node lk_Next = lk_Current.Next[li_Level];
                while (lk_Next != null)
                {
                    int li_Diff = mk_Comparer.Compare(lk_Next.Key, ak_Key);
                    if (li_Diff >= 0)
                    {
                        lk_Found = lk_Next;
                        break;
                    }
                            
                    lk_Current = lk_Next;
                    lk_Next = lk_Next.Next[li_Level];
                }
            }

            return new k_NodeIterator(this, lk_Found);
        }

        /// <summary>
        /// Returns an iterator to the first element in a list with a key value 
        /// that is greater than that of a specified key.
        /// </summary>
        /// <param name="ak_Key">
        /// The argument key value to be compared with the sort key of an element 
        /// from the list being searched.
        /// </param>
        /// <returns>
        /// Location of an element in a list that with a key that is greater
        /// than the argument key, or this.End if no match is found for the key.
        /// </returns>
        public k_Iterator UpperBound(object ak_Key)
        {
            k_Node lk_Found = null;
            k_Node lk_Current = mk_Head;
            for (int li_Level = mi_HighestNode-1; li_Level >= 0; --li_Level)
            {
                k_Node lk_Next = lk_Current.Next[li_Level];
                while (lk_Next != null)
                {
                    int li_Diff = mk_Comparer.Compare(lk_Next.Key, ak_Key);
                    if (li_Diff > 0)
                    {
                        lk_Found = lk_Next;
                        break;
                    }
                    
                    lk_Current = lk_Next;
                    lk_Next = lk_Next.Next[li_Level];
                }
            }

            return new k_NodeIterator(this, lk_Found);
        }

        #region IDictionary Members

        public void Add(object ak_Key, object ak_Value)
        {
            k_Node lk_Node = new k_Node(ak_Key, ak_Value, CalcNewNodeHeight());
            if (lk_Node.Height > mi_HighestNode)
                mi_HighestNode = lk_Node.Height;

            FindInsertPos(lk_Node);
            for (int i=0; i<lk_Node.Height; ++i)
            {
                k_Node lk_Left = lk_Node.Next[i];
                k_Node lk_Tmp = lk_Left.Next[i];
                lk_Left.Next[i] = lk_Node;
                lk_Node.Next[i] = lk_Tmp;
            }

            ++mi_Count;
        }

        public void Clear()
        {
            Array.Clear(mk_Head.Next, 0, mk_Head.Next.Length);
            mi_HighestNode = 0;
            mi_Count = 0;
        }

        public bool Contains(object ak_Key)
        {
            return (FindKey(ak_Key) != this.End);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new k_IteratorDictEnumerator(this.Begin, this.End);
        }

        public void Remove(object ak_Key)
        {
            Erase(LowerBound(ak_Key), UpperBound(ak_Key));
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
                k_NodeIterator lk_Iter = (k_NodeIterator)FindKey(ak_Key);
                if (lk_Iter == this.End)
                    return null;
                return lk_Iter.Node.Value;
            }
            set
            {
                k_NodeIterator lk_Iter = (k_NodeIterator)FindKey(ak_Key);
                if (lk_Iter == this.End)
                    throw new ArgumentException("No element for key was found.", "ak_Key");
                lk_Iter.Node.Value = value;
            }
        }

        public ICollection Keys
        {
            get
            {
                object[] lk_Keys = new object[mi_Count];
                int i = 0;
                for (k_Iterator lk_Iter = this.Begin.Clone(); lk_Iter != this.End; lk_Iter.Next())
                    lk_Keys[i++] = ((k_NodeIterator)lk_Iter).Node.Key;
                return lk_Keys;
            }
        }

        public ICollection Values
        {
            get
            {
                object[] lk_Values = new object[mi_Count];
                int i=0;
                for (k_Iterator lk_Iter = this.Begin.Clone(); lk_Iter != this.End; lk_Iter.Next())
                    lk_Values[i++] = ((k_NodeIterator)lk_Iter).Node.Value;
                return lk_Values;
            }
        }
        
        #endregion

        #region ICollection Members

        public void CopyTo(Array ak_Array, int ai_Index)
        {
            foreach (object lk_Obj in this)
                ak_Array.SetValue(lk_Obj, ai_Index++);
        }

        public int Count
        {
            get { return mi_Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
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
            k_SkipList lk_Clone = new k_SkipList(mk_Comparer, md_Prob, mi_MaxLevel);
            lk_Clone.mi_Count = mi_Count;
            lk_Clone.mi_HighestNode = mi_HighestNode;
            lk_Clone.mk_Head = CloneR(mk_Head, null);
            return lk_Clone;
        }

        #endregion

        private k_Node Previous(k_Node ak_Node)
        {
            k_Node lk_Current = mk_Head;
            for (int li_Level = mi_HighestNode-1; li_Level >= 0; --li_Level)
            {
                while (lk_Current.Next[li_Level] != null)
                {
                    int li_Diff = mk_Comparer.Compare(lk_Current.Next[li_Level].Key, ak_Node.Key);
                    if (li_Diff > 0)
                        break;
                    if (li_Diff == 0)
                    {
                        k_Node lk_Next = lk_Current;
                        while (lk_Next != null && !object.ReferenceEquals(lk_Next.Next[0], ak_Node))
                        {
                            if (mk_Comparer.Compare(lk_Next.Key, ak_Node.Key) > 0)
                                lk_Next = null;
                            else
                                lk_Next = lk_Next.Next[0];
                        }
                        if (lk_Next == null)
                            break;
                        
                        return lk_Next;     // found previous node during right-scan of nodes with equal key value
                    }
                    lk_Current = lk_Current.Next[li_Level];
                }
            }
            if (object.ReferenceEquals(mk_Head, lk_Current))
                return null;
            return lk_Current;
        }

        private k_Node Next(k_Node ak_Node)
        {
            return ak_Node.Next[0];
        }

        /// <summary>
        /// Return leftmost node in list.
        /// </summary>
        /// <returns>Found node</returns>
        private k_Node LeftMost()
        {
            return mk_Head.Next[0];
        }

        /// <summary>
        /// Return rightmost node in list.
        /// </summary>
        /// <returns>Found node</returns>
        private k_Node RightMost()
        {
            k_Node lk_Current = mk_Head.Next[mi_HighestNode-1];
            for (int li_Level = mi_HighestNode-1; li_Level >= 0; --li_Level)
            {
                while (lk_Current.Next[li_Level] != null)
                    lk_Current = lk_Current.Next[li_Level];
            }
            return lk_Current;
        }

        private void FindInsertPos(k_Node ak_Node)
        {
            k_Node lk_Current = mk_Head;
            for (int li_Level = mi_HighestNode-1; li_Level >= 0; --li_Level)
            {
                while (lk_Current.Next[li_Level] != null && mk_Comparer.Compare(lk_Current.Next[li_Level].Key, ak_Node.Key) < 0)
                    lk_Current = lk_Current.Next[li_Level];

                if (li_Level < ak_Node.Height)
                    ak_Node.Next[li_Level] = lk_Current;
            }
        }

        private int CalcNewNodeHeight()
        {
            double ld_Rnd = mk_Rand.NextDouble();

            int li_Level = 1;
            for (double ld_Pow = md_Prob; li_Level < mi_MaxLevel; ++li_Level, ld_Pow*=md_Prob)
            {
                if (ld_Pow < ld_Rnd)
                    break;
            }

            return li_Level;
        }

        private int ComparePos(k_Node ak_Left, k_Node ak_Right)
        {
            if (object.ReferenceEquals(ak_Left, ak_Right))
                return 0;

            int li_Diff = mk_Comparer.Compare(ak_Left.Key, ak_Right.Key);
            if (li_Diff != 0)
                return li_Diff;

            k_Node lk_Current = ak_Left;
            for (;;)
            {
                if (lk_Current == null || mk_Comparer.Compare(lk_Current.Key, ak_Right.Key) > 0)
                    return 1;
                else if (object.ReferenceEquals(lk_Current, ak_Right))
                    return -1;

                lk_Current = lk_Current.Next[0];
            }
        }

        private k_Node CloneR(k_Node ak_Node, k_Node ak_NextHigher)
        {
            k_Node lk_New = new k_Node(ak_Node.Key, ak_Node.Value, ak_Node.Height);
        
            for (int i=ak_Node.Height-1; i>=0; --i)
            {
                // simply copy two links with equal target next to each other
                if (i < ak_Node.Height-1 && object.ReferenceEquals(ak_Node.Next[i], ak_Node.Next[i+1]))
                {
                    lk_New.Next[i] = lk_New.Next[i+1];
                    continue;
                }

                k_Node lk_Next = ak_Node.Next[i];
                if (lk_Next != null && lk_Next.Height-1 <= i)
                {
                    k_Node lk_Higher = (i < ak_Node.Height-1) ? ak_Node.Next[i+1] : ak_NextHigher;
                    lk_New.Next[i] = CloneR(lk_Next, lk_Higher);
                }
                else
                    lk_New.Next[i] = ak_NextHigher;
            }
            return lk_New;
        }
    }
}
