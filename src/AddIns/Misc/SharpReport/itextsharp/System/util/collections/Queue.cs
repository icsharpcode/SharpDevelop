using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// k_Queue is a first-in, first-out (FIFO) data structure.
    /// It hides functionality of the underlying container (e.g. k_List, k_Deque) 
    /// and provides a a basic queue class.
    /// </summary>
    public class k_Queue : ICollection
    {
        private ISequence mk_Container;

        public k_Queue()
            : this(typeof(k_Deque))
        {
        }

        public k_Queue(Type ak_ContainerType)
        {
            mk_Container = Activator.CreateInstance(ak_ContainerType) as ISequence;
            if (mk_Container == null)
                throw new ArgumentException("Container type must implement ISequence.", "ak_ContainerType");
        }

        public k_Iterator Begin 
        {
            get { return mk_Container.Begin; } 
        }

        public k_Iterator End 
        {
            get { return mk_Container.End; }
        }

        public object Front
        {
            get { return mk_Container.Front; }
        }

        public object Back
        {
            get { return mk_Container.Back; }
        }

        public bool IsEmpty
        {
            get { return mk_Container.IsEmpty; }
        }

        public k_Iterator Erase(k_Iterator ak_Where)
        {
            return mk_Container.Erase(ak_Where);
        }

        public void Push(object ak_Value)
        {
            mk_Container.PushBack(ak_Value);
        }

        public object Pop()
        {
            object lk_Obj = mk_Container.Front;
            mk_Container.PopFront();
            return lk_Obj;
        }

        public IContainer UnderlyingContainer
        {
            get { return mk_Container; }
        }

        #region ICollection Members

        public void CopyTo(Array ak_Array, int ai_Index)
        {
            foreach (object lk_Obj in this)
                ak_Array.SetValue(lk_Obj, ai_Index++);
        }

        public int Count
        {
            get { return mk_Container.Count; }
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
            return new k_IteratorEnumerator(mk_Container.Begin, mk_Container.End);
        }

        #endregion
    }
}
