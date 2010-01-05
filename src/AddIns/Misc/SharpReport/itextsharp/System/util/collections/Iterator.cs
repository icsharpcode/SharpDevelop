using System;
using System.Collections;

namespace System.util.collections
{
    public abstract class k_Iterator : IComparable, ICloneable
    {
        public static k_Iterator operator ++(k_Iterator ak_Iter)
        {
            return ak_Iter + 1;
        }

        public static k_Iterator operator --(k_Iterator ak_Iter)
        {
            return ak_Iter - 1;
        }

        public static k_Iterator operator +(k_Iterator ak_Iter, int ai_Distance)
        {
            k_Iterator lk_Clone = ak_Iter.Clone();
            lk_Clone.Move(ai_Distance);
            return lk_Clone;
        }

        public static k_Iterator operator -(k_Iterator ak_Iter, int ai_Distance)
        {
            k_Iterator lk_Clone = ak_Iter.Clone();
            lk_Clone.Move(-ai_Distance);
            return lk_Clone;
        }

        public static int operator -(k_Iterator ak_Left, k_Iterator ak_Right)
        {
            return ak_Left.Distance(ak_Right);
        }

        public static bool operator ==(k_Iterator ak_Left, k_Iterator ak_Right)
        {
            return object.Equals(ak_Left, ak_Right);
        }

        public static bool operator !=(k_Iterator ak_Left, k_Iterator ak_Right)
        {
            return !object.Equals(ak_Left, ak_Right);
        }

        public override bool Equals(object ak_Obj)
        {
            return base.Equals(ak_Obj);    // implemented to avoid warning
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();    // implemented to avoid warning
        }

        public int CompareTo(object ak_Obj)
        {
            k_Iterator lk_Iter = ak_Obj as k_Iterator;
            if (lk_Iter == null || !object.ReferenceEquals(lk_Iter.Collection, this.Collection))
                throw new ArgumentException("Cannot compare iterators of different origin.");

            return Distance(lk_Iter);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public void Next()
        {
            Move(1);
        }

        public void Prev()
        {
            Move(-1);
        }

        public abstract object Current { get; set; }
        public abstract object Collection { get; }
        public abstract k_Iterator Clone();

        public abstract void Move(int ai_Count);
        public abstract int Distance(k_Iterator ak_Iter);
    }

    public class k_IteratorEnumerator : IEnumerator
    {
        protected k_Iterator mk_Current;
        protected k_Iterator mk_Begin, mk_End;
        protected bool mb_Fresh;

        public k_IteratorEnumerator(k_Iterator ak_Begin, k_Iterator ak_End)
        {
            mk_Begin = ak_Begin;
            mk_End = ak_End;
            mb_Fresh = true;
        }

        #region IEnumerator Members

        public bool MoveNext()
        {
            if (mb_Fresh)
            {
                mk_Current = mk_Begin.Clone();
                mb_Fresh = false;
            }
            else if (mk_Current != mk_End)
                mk_Current.Next();

            return (mk_Current != mk_End);
        }

        public void Reset()
        {
            mb_Fresh = true;
            mk_Current = null;
        }

        public object Current
        {
            get
            {
                if (mb_Fresh || mk_Current == mk_End)
                    throw new InvalidOperationException("The enumerator is positioned before the first element of the collection or after the last element.");
                return mk_Current.Current;
            }
        }

        #endregion
    }

    public class k_IteratorDictEnumerator : k_IteratorEnumerator, IDictionaryEnumerator
    {
        public k_IteratorDictEnumerator(k_Iterator ak_Begin, k_Iterator ak_End)
            : base(ak_Begin, ak_End)
        {
        }

        #region IDictionaryEnumerator Members

        public object Key
        {
            get { return this.Entry.Key; }
        }

        public object Value
        {
            get { return this.Entry.Value; }
        }

        public DictionaryEntry Entry
        {
            get { return (DictionaryEntry)this.Current; }
        }

        #endregion
    }

    public class k_IListIterator : k_Iterator
    {
        private readonly IList mk_List;
        private int mi_Index;

        public static k_IListIterator CreateBegin(IList ak_List)
        {
            return new k_PinnedIListIterator(ak_List, 0);
        }

        public static k_IListIterator CreateEnd(IList ak_List)
        {
            return new k_PinnedIListIterator(ak_List, ak_List.Count);
        }

        public k_IListIterator(IList ak_List, int ai_Index)
        {
            mk_List = ak_List;
            mi_Index = ai_Index;
        }

        public override void Move(int ai_Count)
        {
            int li_Index = mi_Index + ai_Count;
            
            if (li_Index < 0)
                throw new InvalidOperationException("Tried to move before first element.");
            else if (li_Index > mk_List.Count)
                throw new InvalidOperationException("Tried to moved beyond end element.");

            mi_Index = li_Index;
        }

        public override int Distance(k_Iterator ak_Iter)
        {
            return mi_Index - ((k_IListIterator)ak_Iter).mi_Index;
        }

        public override object Collection
        {
            get { return mk_List; }
        }

        public override object Current
        {
            get
            {
                if (mi_Index < 0 || mi_Index >= mk_List.Count)
                    throw new k_InvalidPositionException();
                return mk_List[mi_Index];
            }
            set
            {
                if (mi_Index < 0 || mi_Index >= mk_List.Count)
                    throw new k_InvalidPositionException();
                mk_List[mi_Index] = value;
            }
        }

        public override bool Equals(object ak_Obj)
        {
            k_IListIterator lk_Iter = ak_Obj as k_IListIterator;
            if (lk_Iter == null)
                return false;
            return (mi_Index == lk_Iter.mi_Index) && object.ReferenceEquals(this.Collection, lk_Iter.Collection);
        }

        public override int GetHashCode()
        {
            return mk_List.GetHashCode() ^ mi_Index;
        }

        public override k_Iterator Clone()
        {
            return new k_IListIterator(mk_List, mi_Index);
        }

        internal int Index
        {
            get { return mi_Index; }
        }
    }

    internal class k_PinnedIListIterator : k_IListIterator
    {
        public k_PinnedIListIterator(IList ak_List, int ai_Index)
            : base(ak_List, ai_Index)
        {
        }

        public override void Move(int ai_Count)
        {
            throw new k_IteratorPinnedException();
        }
    }

    public class k_CollectionOnIterators : ICollection
    {
        private k_Iterator mk_Begin, mk_End;
        private int mi_Count;

        public k_CollectionOnIterators(k_Iterator ak_Begin, k_Iterator ak_End)
        {
            mk_Begin = ak_Begin;
            mk_End = ak_End;
            mi_Count = mk_End - mk_Begin;
        }

        #region ICollection Members

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
            get { return mk_Begin.Collection; }
        }

        public void CopyTo(Array ak_Array, int ai_Index)
        {
            foreach (object lk_Obj in this)
                ak_Array.SetValue(lk_Obj, ai_Index++);
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new k_IteratorEnumerator(mk_Begin, mk_End);
        }

        #endregion
    }

    [Serializable]
    public class k_IteratorPinnedException : InvalidOperationException
    {
        public k_IteratorPinnedException()
            : base("Cannot move pinned iterator. Use Clone() to create a movable copy.")
        {
        }
    }

    [Serializable]
    public class k_InvalidPositionException : InvalidOperationException
    {
        public k_InvalidPositionException()
            : base("Iterator positioned on End or invalid element.")
        {
        }
    }
}
