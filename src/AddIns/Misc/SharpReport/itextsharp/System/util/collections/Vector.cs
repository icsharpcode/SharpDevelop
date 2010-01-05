using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// One dimensional array of variable size
    /// </summary>
    public class k_Vector : ArrayList, ISequence
    {
        public k_Vector()
            : base()
        {
        }

        public k_Vector(int ai_Capacity) 
            : base(ai_Capacity)
        {
        }

        public k_Vector(ICollection ak_Collection)
            : base(ak_Collection)
        {
        }

        // IContainer Members
        public k_Iterator Begin
        {
            get { return k_IListIterator.CreateBegin(this); }
        }

        public k_Iterator End
        {
            get { return k_IListIterator.CreateEnd(this); }
        }

        public bool IsEmpty
        {
            get { return (this.Count == 0); }
        }

        public k_Iterator Find(object ak_Value)
        {
            int li_Index = this.IndexOf(ak_Value);
            if (li_Index < 0)
                return this.End;
            return new k_IListIterator(this, li_Index);
        }

        public k_Iterator Erase(k_Iterator ak_Where)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this collection.");
            int li_Index = ((k_IListIterator)ak_Where).Index;
            if (li_Index < this.Count)
                base.RemoveAt(li_Index);
            return new k_IListIterator(this, li_Index);
        }

        public k_Iterator Erase(k_Iterator ak_First, k_Iterator ak_Last)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_First.Collection) && object.ReferenceEquals(this, ak_Last.Collection), "Iterators do not belong to this collection.");
            int li_FirstIndex = ((k_IListIterator)ak_First).Index;
            int li_Count = ak_Last - ak_First;

            base.RemoveRange(li_FirstIndex, li_Count);
            
            return new k_IListIterator(this, li_FirstIndex);
        }

        // ISequence Members
        public object Front
        {
            get { return this.Begin.Current; }
            set { this.Begin.Current = value; }
        }

        public object Back
        {
            get { return (this.End-1).Current; }
            set { (this.End-1).Current = value; }
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
            this.Insert(((k_IListIterator)ak_Where).Index, ak_Value);
        }

        public void Insert(k_Iterator ak_Where, k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this collection.");
            InsertRange(((k_IListIterator)ak_Where).Index, new k_CollectionOnIterators(ak_SrcBegin, ak_SrcEnd));
        }

        public void Insert(k_Iterator ak_Where, object ak_Value, int ai_Count)
        {
            int li_Pos = ((k_IListIterator)ak_Where).Index;
            for (int i=0; i<ai_Count; ++i)
                base.Insert(li_Pos+i, ak_Value);
        }

        #region ICloneable Members

        public override object Clone()
        {
            return new k_Vector(this);
        }

        #endregion
    }
}
