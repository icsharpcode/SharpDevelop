using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// A doubly linked list
    /// </summary>
    public class k_List : ISequence
    {
        #region k_Node Implementation

        private class k_Node
        {
            private object mk_Value;
            public k_Node mk_Prev, mk_Next;

            public k_Node(object ak_Value)
            {
                mk_Value = ak_Value;
            }

            public object Value
            {
                get { return mk_Value; }
                set { mk_Value = value; }
            }
        }

        #endregion

        #region k_NodeIterator Implementation

        private class k_NodeIterator : k_Iterator, ICloneable
        {
            private readonly k_List mk_List;
            private k_Node mk_Current;

            public k_NodeIterator(k_List ak_List, k_Node ak_Node)
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
                    return mk_Current.Value;
                }
                set
                {
                    if (mk_Current == null)
                        throw new k_InvalidPositionException();
                    mk_Current.Value = value;
                }
            }

            public override object Collection
            {
                get { return mk_List; }
            }

            public override void Move(int ai_Count)
            {
                k_Node lk_NewPos = mk_Current;

                int li_Count = ai_Count;
                if (li_Count > 0)
                {
                    while (li_Count-- > 0)
                    {
                        if (lk_NewPos == null)
                            throw new InvalidOperationException("Tried to moved beyond end element.");

                        lk_NewPos = lk_NewPos.mk_Next;
                    }
                }
                else
                {
                    while (li_Count++ < 0)
                    {
                        if (lk_NewPos == null)
                            lk_NewPos = mk_List.mk_Tail;
                        else
                            lk_NewPos = lk_NewPos.mk_Prev;

                        if (lk_NewPos == null)
                            throw new InvalidOperationException("Tried to move before first element.");
                    }
                }
                
#if (DEBUG)
                if (ai_Count != 0 && object.ReferenceEquals(mk_Current, lk_NewPos))
                    throw new IndexOutOfRangeException("Iterator is positioned on invalid node.");
#endif

                mk_Current = lk_NewPos;
            }

            public override int Distance(k_Iterator ak_Iter)
            {
                k_NodeIterator lk_Iter = (k_NodeIterator)ak_Iter;

                if (!this.IsValid || !lk_Iter.IsValid)
                    throw new ArgumentException("Iterator is invalid.");

                int li_Diff = 0;
                k_Iterator lk_End = mk_List.End;
                k_Iterator lk_Fwd = lk_Iter.Clone();
                for (; lk_Fwd != this && lk_Fwd != lk_End; lk_Fwd.Next())
                    ++li_Diff;

                if (lk_Fwd == this)
                    return li_Diff;

                li_Diff = 0;
                k_Iterator lk_Bck = this.Clone(); 
                for (; lk_Bck != lk_Iter && lk_Bck != lk_End; lk_Bck.Next())
                    --li_Diff;

                if (lk_Bck == lk_Iter)
                    return li_Diff;

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

            internal bool IsValid
            {
                get { return (mk_Current == null || (!object.ReferenceEquals(mk_Current.mk_Next, mk_Current) && !object.ReferenceEquals(mk_Current.mk_Prev, mk_Current))); }
            }
        }

        private class k_PinnedNodeIterator : k_NodeIterator
        {
            public k_PinnedNodeIterator(k_List ak_List, k_Node ak_Node)
                : base(ak_List, ak_Node)
            {
            }

            public override void Move(int ai_Count)
            {
                throw new k_IteratorPinnedException();
            }
        }

        #endregion

        private int mi_Count;
        private k_Node mk_Head, mk_Tail;
        private k_Iterator mk_Begin, mk_End;

        public k_List()
        {
            mk_End = new k_PinnedNodeIterator(this, null);
            mk_Begin = mk_End;
        }

        // IContainer Members
        public k_Iterator Begin
        {
            get 
            { 
                if (mi_Count == 0)
                    return mk_End;
                if (mk_Begin == null)
                    mk_Begin = new k_PinnedNodeIterator(this, mk_Head);
                return mk_Begin; 
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
            return k_Algorithm.Find(this.Begin, this.End, ak_Value);
        }
        
        public k_Iterator Erase(k_Iterator ak_Where)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this list.");
            if (ak_Where == this.End)
                return this.End;
            return Erase(ak_Where, ak_Where + 1);
        }

        public k_Iterator Erase(k_Iterator ak_First, k_Iterator ak_Last)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_First.Collection) && object.ReferenceEquals(this, ak_Last.Collection), "Iterators do not belong to this collection.");
            int li_Distance = ak_Last - ak_First;
            if (li_Distance == 0)
                return ak_Last;
            
            k_Node lk_First = ((k_NodeIterator)ak_First).Node;
            k_Node lk_Prev = lk_First.mk_Prev;
            k_Node lk_Next = (ak_Last != this.End) ? ((k_NodeIterator)ak_Last).Node : null;

            if (lk_Prev != null)
                lk_Prev.mk_Next = lk_Next;
            else
            {
                //System.Diagnostics.Debug.Assert(object.ReferenceEquals(mk_Head, lk_First), "Inconsistent list state");
                mk_Head = lk_Next;
                mk_Begin = null;
            }

            if (lk_Next != null)
                lk_Next.mk_Prev = lk_Prev;
            else
            {
                //System.Diagnostics.Debug.Assert(object.ReferenceEquals(mk_Tail, ((k_NodeIterator)(ak_Last-1)).Node), "Inconsistent list state");
                mk_Tail = lk_Prev;
            }

            mi_Count -= li_Distance;

#if (DEBUG)            
            // create invalid nodes linking to itself
            k_Node lk_Node = lk_First;
            while (lk_Node != null && lk_Node != lk_Next)
            {
                k_Node lk_Tmp = lk_Node.mk_Next;
                lk_Node.mk_Next = lk_Node;
                lk_Node.mk_Prev = lk_Node;

                lk_Node = lk_Tmp;
            }
#endif    

            return ak_Last;
        }

        // ISequence Members
        public object Front
        {
            get 
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Empty list");
                return mk_Head.Value; 
            }
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Empty list");
                mk_Head.Value = value;
            }
        }

        public object Back
        {
            get
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Empty list");
                return mk_Tail.Value;
            }
            set 
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Empty list");
                mk_Tail.Value = value; 
            }
        }

        public void PushFront(object ak_Value)
        {
            Insert(this.Begin, ak_Value);
        }

        public void PopFront()
        {
            Erase(this.Begin);
        }

        public void PushBack(object ak_Value)
        {
            Insert(this.End, ak_Value);
        }

        public void PopBack()
        {
            Erase(this.End-1);
        }

        public void Assign(k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd)
        {
            Clear();
            Insert(this.End, ak_SrcBegin, ak_SrcEnd);
        }

        public void Assign(object ak_Value, int ai_Count)
        {
            Clear();
            Insert(this.End, ak_Value, ai_Count);
        }

        public void Insert(k_Iterator ak_Where, object ak_Value)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this collection.");

            k_Node lk_New = new k_Node(ak_Value);
            PasteNodeRange((k_NodeIterator)ak_Where, lk_New, lk_New);
            ++mi_Count;
        }

        public void Insert(k_Iterator ak_Where, k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this collection.");

            k_Node lk_Start = new k_Node(null), lk_End = lk_Start;
            
            int li_Count = 0;
            for (k_Iterator lk_Iter = ak_SrcBegin.Clone(); lk_Iter != ak_SrcEnd; lk_Iter.Next(), ++li_Count)
            {
                k_Node lk_New = new k_Node(lk_Iter.Current);
                lk_End.mk_Next = lk_New;
                lk_New.mk_Prev = lk_End;
                lk_End = lk_New;
            }

            if (li_Count > 0)
            {
                PasteNodeRange((k_NodeIterator)ak_Where, lk_Start.mk_Next, lk_End);
                mi_Count += li_Count;
            }
        }

        public void Insert(k_Iterator ak_Where, object ak_Value, int ai_Count)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this collection.");

            k_Node lk_Start = new k_Node(null), lk_End = lk_Start;
            
            for (int i=0; i<ai_Count; ++i)
            {
                k_Node lk_New = new k_Node(ak_Value);
                lk_End.mk_Next = lk_New;
                lk_New.mk_Prev = lk_End;
                lk_End = lk_New;
            }

            if (ai_Count > 0)
            {
                PasteNodeRange((k_NodeIterator)ak_Where, lk_Start.mk_Next, lk_End);
                mi_Count += ai_Count;
            }
        }
        
        #region IList Members

        public int Add(object ak_Value)
        {
            Insert(this.End, ak_Value);
            return mi_Count;
        }

        public void Clear()
        {
            mk_Head = mk_Tail = null;
            mk_Begin = mk_End;
            mi_Count = 0;
        }

        public bool Contains(object ak_Value)
        {
            return (this.Find(ak_Value) != this.End);
        }

        public int IndexOf(object ak_Value)
        {
            int li_Index = 0;
            foreach (object lk_Val in this)
            {
                if (object.Equals(lk_Val, ak_Value))
                    return li_Index;
                ++li_Index;
            }
            return -1;
        }

        void IList.Insert(int ai_Index, object ak_Value)
        {
            this.Insert(this.Begin + ai_Index, ak_Value);
        }

        void IList.Remove(object ak_Value)
        {
            k_NodeIterator lk_Found = (k_NodeIterator)this.Find(ak_Value);
            if (lk_Found != this.End)
                Erase(lk_Found);
        }

        public void RemoveAt(int ai_Index)
        {
            Erase(this.Begin + ai_Index);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        public object this[int index]
        {
            get 
            { 
                k_Iterator lk_Iter = this.Begin + index;
                return lk_Iter.Current; 
            }
            set 
            { 
                k_Iterator lk_Iter = this.Begin + index;
                lk_Iter.Current = value; 
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

        public IEnumerator GetEnumerator()
        {
            return new k_IteratorEnumerator(this.Begin, this.End);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            k_List lk_Clone = new k_List();
            for (k_Iterator lk_Iter = this.Begin.Clone(); lk_Iter != this.End; lk_Iter.Next())
                lk_Clone.Add(lk_Iter.Current);
            return lk_Clone;
        }

        #endregion

        private void PasteNodeRange(k_NodeIterator ak_Where, k_Node ak_First, k_Node ak_Last)
        {
            if (ak_Where != this.End)
            {
                k_Node lk_Next = ak_Where.Node;
                k_Node lk_Prev = lk_Next.mk_Prev;

                ak_Last.mk_Next = lk_Next;
                ak_First.mk_Prev = lk_Prev;
                if (lk_Next != null)
                    lk_Next.mk_Prev = ak_Last;
                if (lk_Prev != null)
                    lk_Prev.mk_Next = ak_First;
            }
            else
            {
                if (mk_Tail != null)
                {
                    mk_Tail.mk_Next = ak_First;
                    ak_First.mk_Prev = mk_Tail;
                }
                mk_Tail = ak_Last;
            }

            if (ak_Where == this.Begin)
            {
                mk_Head = ak_First;
                mk_Begin = null;        // recalc on next get
            }
        }
    }
}
