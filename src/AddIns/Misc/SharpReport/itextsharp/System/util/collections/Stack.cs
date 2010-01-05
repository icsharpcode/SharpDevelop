using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// A push-down stack using an underlying k_Vector.
    /// Last in first out (LIFO).
    /// </summary>
    public class k_Stack : ICollection
    {
        private ISequence mk_Items;        // stack container
        private int mi_MaxSize;

        public k_Stack()
        {
            mk_Items = new k_Vector();
            mi_MaxSize = int.MaxValue;
        }

        public k_Stack(int ai_Capacity, bool ab_FixedSize)
        {
            mk_Items = new k_Vector(ai_Capacity);
            mi_MaxSize = (ab_FixedSize) ? ai_Capacity : int.MaxValue;
        }

        public object Top
        {
            get { return mk_Items.Back; }
            set { mk_Items.Back = value; }
        }

        public object this[int ai_Index]
        {
            get { return (mk_Items.Begin+ai_Index).Current; }
            set { (mk_Items.Begin+ai_Index).Current = value; }
        }

        public void Push(object ak_Value)
        {
            if (mk_Items.Count >= mi_MaxSize)
                throw new StackOverflowException("Stack overflow");

            mk_Items.PushBack(ak_Value);
        }

        public object Pop()
        {
            if (mk_Items.Count == 0)
                throw new StackOverflowException("Stack underflow");

            object lk_Obj = mk_Items.Back;
            mk_Items.PopBack();
            return lk_Obj;
        }

        public bool IsEmpty
        {
            get { return mk_Items.IsEmpty; }
        }

        public void Clear()
        {
            mk_Items.Clear();
        }

        #region ICollection Members

        public void CopyTo(Array ak_Array, int ai_Index)
        {
            for (k_Iterator lk_Iter = mk_Items.Begin.Clone(); lk_Iter != mk_Items.End; lk_Iter.Next())
                ak_Array.SetValue(lk_Iter.Current, ai_Index++);
        }

        public int Count
        {
            get { return mk_Items.Count; }
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
            return new k_IteratorEnumerator(mk_Items.Begin, mk_Items.End);
        }

        #endregion
    }
}
