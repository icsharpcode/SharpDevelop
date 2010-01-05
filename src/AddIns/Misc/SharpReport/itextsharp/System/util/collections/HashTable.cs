using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// A HashTable with iterators
    /// </summary>
    public class k_HashTable : IMap
    {
        #region static helper functions

        private readonly static int[] mk_Primes = 
        {
            11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 
            156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
            968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 
            4999559, 5999471, 7199369 
        };

        private static bool IsPrime(int ai_Number) 
        {
            if ((ai_Number & 1) == 0) 
                return (ai_Number == 2);

            int li_Max = (int)Math.Sqrt(ai_Number);
            for (int li_Div=3; li_Div < li_Max; li_Div+=2)
            {
                if ((ai_Number % li_Div) == 0)
                    return false;
            }
            return true;
        }

        private static int FindPrimeGreater(int ai_Min)
        {
            if (ai_Min < 0)
                throw new ArgumentException("k_HashTable capacity overflow.");

            // do binary search lookup in primes array
            int li_Pos = Array.BinarySearch(mk_Primes, ai_Min);
            if (li_Pos >= 0)
                return mk_Primes[li_Pos];

            li_Pos = ~li_Pos;
            if (li_Pos < mk_Primes.Length)
                return mk_Primes[li_Pos];

            // ai_Min is greater than highest number in mk_Primes
            for (int i = (ai_Min|1); i <= Int32.MaxValue; i+=2)
            {
                if (IsPrime(i))
                    return i;
            }
            return ai_Min;
        }

        #endregion

        #region Bucket Structure

        private struct r_Bucket 
        {
            public object mk_Key;
            public object mk_Value;
            public int mi_HashCode;        // MSB (sign bit) indicates a collision.
        }

        #endregion

        #region k_BucketIterator Implementation

        private class k_BucketIterator : k_Iterator
        {
            private readonly k_HashTable mk_Table;
            private int mi_Index;

            public k_BucketIterator(k_HashTable ak_Table, int ai_Index)
            {
                mk_Table = ak_Table;
                mi_Index = -1;
                if (ai_Index >= 0)
                    mi_Index = FindNext(ai_Index-1);
            }

            public override object Current
            {
                get
                {
                    if (mi_Index < 0 || mk_Table.mk_Buckets[mi_Index].mk_Key == null)
                        throw new k_InvalidPositionException();

                    r_Bucket lr_Bucket = mk_Table.mk_Buckets[mi_Index];
                    return new DictionaryEntry(lr_Bucket.mk_Key, lr_Bucket.mk_Value);
                }
                set
                {
                    if (mi_Index < 0 || mk_Table.mk_Buckets[mi_Index].mk_Key == null)
                        throw new k_InvalidPositionException();

                    DictionaryEntry lr_Entry = (DictionaryEntry)value;
                    r_Bucket lr_Bucket = mk_Table.mk_Buckets[mi_Index];
                    if (mk_Table.mk_Comparer.Compare(lr_Entry.Key, lr_Bucket.mk_Key) != 0)
                        throw new ArgumentException("Key values must not be changed.");
                    mk_Table.mk_Buckets[mi_Index].mk_Value = lr_Entry.Value;
                }
            }

            public override void Move(int ai_Count)
            {
                int li_NewIndex = mi_Index;

                if (ai_Count > 0)
                {
                    while (ai_Count-- > 0)
                    {
                        if (li_NewIndex < 0)
                            throw new InvalidOperationException("Tried to moved beyond end element.");

                        li_NewIndex = FindNext(li_NewIndex);
                    }
                }
                else
                {
                    while (ai_Count++ < 0)
                    {
                        if (li_NewIndex < 0)
                            li_NewIndex = FindPrev(mk_Table.mk_Buckets.Length);
                        else
                            li_NewIndex = FindPrev(li_NewIndex);

                        if (li_NewIndex < 0)
                            throw new InvalidOperationException("Tried to move before first element.");
                    }
                }

                mi_Index = li_NewIndex;
            }

            public override int Distance(k_Iterator ak_Iter)
            {
                k_BucketIterator lk_Iter = ak_Iter as k_BucketIterator;
                if (lk_Iter == null || !object.ReferenceEquals(lk_Iter.Collection, this.Collection))
                    throw new ArgumentException("Cannot determine distance of iterators belonging to different collections.");
                
                k_Iterator lk_End = mk_Table.End;
    
                int li_IndexDiff; 
                if (this != lk_End && ak_Iter != lk_End)
                    li_IndexDiff = mi_Index - lk_Iter.mi_Index;
                else
                    li_IndexDiff = (this == lk_End) ? 1 : -1;    // 1 is also fine when both are End

                if (li_IndexDiff < 0)
                {
                    int li_Diff = 0;
                    k_Iterator lk_Bck = this.Clone(); 
                    for (; lk_Bck != ak_Iter && lk_Bck != lk_End; lk_Bck.Next())
                        --li_Diff;

                    if (lk_Bck == ak_Iter)
                        return li_Diff;
                } 
                else
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

            public override object Collection
            {
                get { return mk_Table; }
            }

            public override bool Equals(object ak_Obj)
            {
                k_BucketIterator lk_Iter = ak_Obj as k_BucketIterator;
                if (lk_Iter == null)
                    return false;

                return (mi_Index == lk_Iter.mi_Index && object.ReferenceEquals(mk_Table, lk_Iter.mk_Table));
            }

            public override int GetHashCode()
            {
                return mk_Table.GetHashCode() ^ mi_Index;
            }

            public override k_Iterator Clone()
            {
                return new k_BucketIterator(mk_Table, mi_Index);
            }

            private int FindPrev(int ai_Index)
            {
                --ai_Index;
                r_Bucket[] lk_Buckets = mk_Table.mk_Buckets;
                while (ai_Index >= 0 && lk_Buckets[ai_Index].mk_Key == null)
                    --ai_Index;
                if (ai_Index < -1)
                    return -1;
                return ai_Index;
            }

            private int FindNext(int ai_Index)
            {
                ++ai_Index;
                r_Bucket[] lk_Buckets = mk_Table.mk_Buckets;
                while (ai_Index < lk_Buckets.Length && lk_Buckets[ai_Index].mk_Key == null)
                    ++ai_Index;

                if (ai_Index >= lk_Buckets.Length)
                    return -1;
                return ai_Index;
            }

            internal int Index
            {
                get { return mi_Index; }
            }
        }

        private class k_PinnedBucketIterator : k_BucketIterator
        {
            public k_PinnedBucketIterator(k_HashTable ak_Table, int ai_Index)
                : base(ak_Table, ai_Index)
            {
            }

            public override void Move(int ai_Count)
            {
                throw new k_IteratorPinnedException();
            }
        }

        #endregion

        private IHashCodeProvider mk_HashProvider;
        private IComparer mk_Comparer;
        private double md_LoadFactor;
        private int mi_GrowSize;
        private r_Bucket[] mk_Buckets;
        private int mi_Count;
        private readonly k_Iterator mk_End;

        public k_HashTable()
            : this(0, 0.72)
        {
        }

        public k_HashTable(int ai_Capacity, double ad_LoadFactor)
            : this(ai_Capacity, ad_LoadFactor, null, null)
        {
        }

        public k_HashTable(int ai_Capacity, double ad_LoadFactor, IHashCodeProvider ak_HashProvider, IComparer ak_Comparer)
        {
            if (ad_LoadFactor <= .0 || ad_LoadFactor > 1.0)
                throw new ArgumentException("Load factor must be greater than .0 and smaller or equal to 1.0", "ad_LoadFactor");
            md_LoadFactor = ad_LoadFactor;
            
            double ld_Size = ai_Capacity/ad_LoadFactor;
            if (ld_Size > int.MaxValue)
                throw new ArgumentException("k_HashTable overflow");

            int li_TableSize = FindPrimeGreater((int)ld_Size);
            mk_Buckets = new r_Bucket[li_TableSize];
            mi_GrowSize = (md_LoadFactor < 1.0) ? (int)(md_LoadFactor * li_TableSize) : li_TableSize-1;
            
            mk_HashProvider = ak_HashProvider;
            mk_Comparer = ak_Comparer;

            mk_End = new k_PinnedBucketIterator(this, -1);
        }

        // IContainer Members
        public k_Iterator Begin
        {
            get 
            { 
                if (mi_Count == 0)
                    return mk_End;
                return new k_PinnedBucketIterator(this, 0);
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
            int li_Index = FindBucket(lr_Item.Key);
            if (li_Index < 0 || !object.Equals(mk_Buckets[li_Index].mk_Value, lr_Item.Value))
                return this.End;
            return new k_BucketIterator(this, li_Index);
        }

        public k_Iterator Erase(k_Iterator ak_Where)
        {
            //System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, ak_Where.Collection), "Iterator does not belong to this collection.");
            k_Iterator lk_Successor = ak_Where + 1;
            EmptyBucket(((k_BucketIterator)ak_Where).Index);
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
        public k_Iterator FindKey(object ak_Key)
        {
            return new k_BucketIterator(this, FindBucket(ak_Key));
        }

        public void Add(DictionaryEntry ar_Item)
        {
            Add(ar_Item.Key, ar_Item.Value);
        }

        public void Insert(k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd)
        {
            for (k_Iterator lk_Iter = ak_SrcBegin.Clone(); lk_Iter != ak_SrcEnd; lk_Iter.Next())
                Add((DictionaryEntry)lk_Iter.Current);
        }

        #region IDictionary Members

        public void Add(object ak_Key, object ak_Value)
        {
            SetValue(ak_Key, ak_Value, true);
        }

        public void Clear() 
        {
            if (mi_Count == 0)
                return;

            for (int i=0; i < mk_Buckets.Length; ++i)
                mk_Buckets[i] = new r_Bucket();

            mi_Count = 0;
        }

        public bool Contains(object ak_Key)
        {
            return (FindBucket(ak_Key) >= 0);
        }

        public void Remove(object ak_Key)
        {
            EmptyBucket(FindBucket(ak_Key));
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new k_IteratorDictEnumerator(this.Begin, this.End);
        }

        public bool IsReadOnly
        {
            get { return false; }

        }
        public bool IsFixedSize
        {
            get { return false; }
        }

        public object this[object ak_Key]
        {
            get 
            {
                int li_Index = FindBucket(ak_Key);
                if (li_Index < 0)
                    return null;

                return mk_Buckets[li_Index].mk_Value;
            }
            set 
            {
                SetValue(ak_Key, value, false);
            }
        }

        public ICollection Keys
        {
            get
            {
                int i = 0;
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
            k_HashTable lk_Clone = new k_HashTable(this.Count, md_LoadFactor, mk_HashProvider, mk_Comparer);

            int i = mk_Buckets.Length;
            while (i-- > 0)
            {
                object lk_Key = mk_Buckets[i].mk_Key;
                if (lk_Key != null)
                    lk_Clone[lk_Key] = mk_Buckets[i].mk_Value;
            }
            return lk_Clone;
        }

        #endregion

        private void EmptyBucket(int ai_Index)
        {
            if (ai_Index < 0 || ai_Index >= mk_Buckets.Length)
                return;
            
            if (mk_Buckets[ai_Index].mk_Key == null)
                throw new InvalidOperationException("Key was removed earlier.");

            mk_Buckets[ai_Index].mi_HashCode &= unchecked((int)0x80000000);
            mk_Buckets[ai_Index].mk_Key = null;
            mk_Buckets[ai_Index].mk_Value = null;
            --mi_Count;
        }

        private int FindBucket(object ak_Key)
        {
            if (ak_Key == null)
                throw new ArgumentException("Key must not be null.", "ak_Key");

            uint lui_BucketCount = (uint)mk_Buckets.Length;

            uint lui_Increment;
            uint lui_HashCode = ComputeHashAndStep(ak_Key, out lui_Increment);
                
            uint lui_Walker = lui_HashCode % lui_BucketCount;
            r_Bucket lr_Bucket;
            do
            {
                int li_Index = (int)lui_Walker;
                lr_Bucket = mk_Buckets[li_Index];
                if (lr_Bucket.mk_Key == null && lr_Bucket.mi_HashCode >= 0)
                    break;    // stop on empty non-duplicate

                if ((lr_Bucket.mi_HashCode & 0x7fffffff) == lui_HashCode
                    && EqualsHelper(lr_Bucket.mk_Key, ak_Key))
                    return li_Index;

                lui_Walker += lui_Increment;
                lui_Walker %= lui_BucketCount;
            }
            while (lr_Bucket.mi_HashCode < 0 && lui_Walker != lui_HashCode);

            return -1;        // not found
        }

        private void SetValue(object ak_Key, object ak_Value, bool ab_Add)
        {
            if (mi_Count >= mi_GrowSize)
                ExpandBucketsArray();

            uint lui_BucketCount = (uint)mk_Buckets.Length;

            uint lui_Increment;
            uint lui_HashCode = ComputeHashAndStep(ak_Key, out lui_Increment);
                
            r_Bucket lr_Bucket;
            int li_Free = -1;
            uint lui_Walker = lui_HashCode % lui_BucketCount;
            do
            {
                int li_Index = (int)lui_Walker;
                lr_Bucket = mk_Buckets[li_Index];
                if (li_Free < 0 && lr_Bucket.mk_Key == null && lr_Bucket.mi_HashCode < 0)
                    li_Free = li_Index;

                if (lr_Bucket.mk_Key == null && (lr_Bucket.mi_HashCode & unchecked(0x80000000)) == 0)
                {
                    if (li_Free >= 0)
                        li_Index = li_Free;
                    mk_Buckets[li_Index].mk_Key = ak_Key;
                    mk_Buckets[li_Index].mk_Value = ak_Value;
                    mk_Buckets[li_Index].mi_HashCode |= (int)lui_HashCode;
                    ++mi_Count;
                    return;
                }

                if ((lr_Bucket.mi_HashCode & 0x7fffffff) == lui_HashCode
                    && EqualsHelper(lr_Bucket.mk_Key, ak_Key))
                {
                    if (ab_Add)
                        throw new ArgumentException("duplicate key");
                    mk_Buckets[li_Index].mk_Value = ak_Value;
                    return;
                }

                // mark all as dupes as long as we have not found a free bucket
                if (li_Free == -1)
                    mk_Buckets[li_Index].mi_HashCode |= unchecked((int)0x80000000);

                lui_Walker += lui_Increment;
                lui_Walker %= lui_BucketCount;
            }
            while (lui_Walker != lui_HashCode);

            if (li_Free == -1)
                throw new InvalidOperationException("Corrupted hash table. Insert failed.");

            mk_Buckets[li_Free].mk_Key = ak_Key;
            mk_Buckets[li_Free].mk_Value = ak_Value;
            mk_Buckets[li_Free].mi_HashCode |= (int)lui_HashCode;
            ++mi_Count;
        }

        private static void InternalExpandInsert(r_Bucket[] ak_Buckets, r_Bucket ar_Bucket)
        {
            ar_Bucket.mi_HashCode &= 0x7fffffff;
            uint lui_BucketCount = (uint)ak_Buckets.Length;
            uint lui_Increment = (uint)(1 + ((((uint)ar_Bucket.mi_HashCode >> 5) + 1) % (lui_BucketCount - 1)));

            uint lui_Walker = (uint)ar_Bucket.mi_HashCode % lui_BucketCount;
            for (;;)
            {
                int li_Index = (int)lui_Walker;
                if (ak_Buckets[li_Index].mk_Key == null)
                {
                    ak_Buckets[li_Index] = ar_Bucket;
                    return;
                }

                // since current bucket is occupied mark it as duplicate
                ak_Buckets[li_Index].mi_HashCode |= unchecked((int)0x80000000);

                lui_Walker += lui_Increment;
                lui_Walker %= lui_BucketCount;
            }
        }

        private void ExpandBucketsArray()
        {
            int li_NewSize = FindPrimeGreater(mk_Buckets.Length * 2);

            r_Bucket[] lk_Buckets = new r_Bucket[li_NewSize];
            foreach (r_Bucket lr_Bucket in mk_Buckets)
            {
                if (lr_Bucket.mk_Key == null)
                    continue;
                InternalExpandInsert(lk_Buckets, lr_Bucket);
            }

            mk_Buckets = lk_Buckets;
            mi_GrowSize = (md_LoadFactor < 1.0) ? (int)(md_LoadFactor * li_NewSize) : li_NewSize-1;
        }

        private uint ComputeHashAndStep(object ak_Key, out uint aui_Increment) 
        {
            // mask the sign bit (our collision indicator)
            uint lui_HashCode = (uint)GetHashHelper(ak_Key) & 0x7fffffff;
            // calc increment value relatively prime to mk_Buckets.Length 
            aui_Increment = (uint)(1 + (((lui_HashCode >> 5) + 1) % ((uint)mk_Buckets.Length - 1)));
            return lui_HashCode;
        }

        private int GetHashHelper(object ak_Key)
        {
            if (mk_HashProvider != null)
                return mk_HashProvider.GetHashCode(ak_Key);
            return ak_Key.GetHashCode();
        }

        private bool EqualsHelper(object ak_ObjA, object ak_ObjB)
        {
            if (mk_Comparer != null)
                return (mk_Comparer.Compare(ak_ObjA, ak_ObjB) == 0);
            return Object.Equals(ak_ObjA, ak_ObjB);
        }
    }
}
