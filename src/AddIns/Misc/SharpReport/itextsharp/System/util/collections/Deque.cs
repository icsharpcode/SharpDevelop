using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// Circular buffer of arrays
    /// </summary>
    public class k_Deque : ISequence
    {
        #region k_BlockIterator Implementation

        private class k_BlockIterator : k_Iterator
        {
            private readonly k_Deque mk_Deque;
            private int mi_Index;
            private int mi_BlockIndex;
            private int mi_BlockOffset;

            public k_BlockIterator(k_Deque ak_Deque, int ai_Index)
            {
                mk_Deque = ak_Deque;
                mi_Index = ai_Index;
                mi_BlockIndex = mk_Deque.CalcBlockAndPos(mi_Index, out mi_BlockOffset);
            }

            public override void Move(int ai_Count)
            {
                int li_Index = mi_Index + ai_Count;

                if (li_Index > mk_Deque.Count)
                    throw new InvalidOperationException("Tried to move beyond end element.");
                else if (li_Index < 0)
                    throw new InvalidOperationException("Tried to move before first element.");

                mi_Index = li_Index;
                mi_BlockOffset += ai_Count;

                if (mi_BlockOffset >= k_Deque.mi_BlockSize || mi_BlockOffset < 0)
                    mi_BlockIndex = mk_Deque.CalcBlockAndPos(mi_Index, out mi_BlockOffset);
            }

            public override int Distance(k_Iterator ak_Iter)
            {
                return mi_Index - ((k_BlockIterator)ak_Iter).mi_Index;
            }

            public override object Collection
            {
                get { return mk_Deque; }
            }

            public override object Current
            {
                get 
                {
                    if (mi_Index < 0 || mi_Index >= mk_Deque.mi_Count)
                        throw new k_InvalidPositionException();
                    return mk_Deque.mk_Blocks[mi_BlockIndex][mi_BlockOffset];
                }
                set
                {
                    if (mi_Index < 0 || mi_Index >= mk_Deque.mi_Count)
                        throw new k_InvalidPositionException();
                    mk_Deque.mk_Blocks[mi_BlockIndex][mi_BlockOffset] = value;
                }
            }

            public override bool Equals(object ak_Obj)
            {
                k_BlockIterator lk_Iter = ak_Obj as k_BlockIterator;
                if (lk_Iter == null)
                    return false;
                
                return (mi_Index == lk_Iter.mi_Index) && object.ReferenceEquals(this.Collection, lk_Iter.Collection);
            }

            public override int GetHashCode()
            {
                return mk_Deque.GetHashCode() ^ mi_Index;
            }

            public override k_Iterator Clone()
            {
                return new k_BlockIterator(mk_Deque, mi_Index);
            }

            internal int Index
            {
                get { return mi_Index; }
            }
        }

        private class k_PinnedBlockIterator : k_BlockIterator
        {
            public k_PinnedBlockIterator(k_Deque ak_Deque, int ai_Index)
                : base(ak_Deque, ai_Index)
            {
            }

            public override void Move(int ai_Count)
            {
                throw new k_IteratorPinnedException();
            }
        }

        #endregion

        private const int mi_BlockSize = 16;
        private object[][] mk_Blocks;

        private int mi_Offset;
        private int mi_Count;

        public k_Deque()
            : this(mi_BlockSize)
        {
        }

        public k_Deque(int ai_Capacity)
        {
            if (ai_Capacity < 0)
                throw new ArgumentException("Capacity must be positive.", "ai_Capacity");
            mk_Blocks = new object[(ai_Capacity+mi_BlockSize-1)/mi_BlockSize][];
            for (int i=0; i<mk_Blocks.Length; ++i)
                mk_Blocks[i] = new object[mi_BlockSize];
        }

        // IContainer Members
        public k_Iterator Begin
        {
            get { return new k_PinnedBlockIterator(this, 0); }
        }

        public k_Iterator End
        {
            get { return new k_PinnedBlockIterator(this, mi_Count); }
        }

        public bool IsEmpty
        {
            get { return (this.Count == 0); }
        }

        public k_Iterator Find(object ak_Value)
        {
            return k_Algorithm.Find(this.Begin, this.End, ak_Value);
        }

        public k_Iterator Erase(k_Iterator ak_Where)
        {
            return Erase(ak_Where, ak_Where + 1);
        }

        public k_Iterator Erase(k_Iterator ak_First, k_Iterator ak_Last)
        {
            if (ak_First == ak_Last)
                return ak_Last;

            int li_FirstIndex = ((k_BlockIterator)ak_First).Index;
            int li_Count = ak_Last - ak_First;
            int li_LastCount = this.End - ak_Last;

            if (li_FirstIndex < li_LastCount)
            {
                k_Algorithm.CopyBackward(this.Begin, ak_First, ak_Last);
                k_Algorithm.Fill(this.Begin, ak_First, null);
                mi_Offset += li_Count;
                mi_Offset %= (mk_Blocks.Length * mi_BlockSize);
            }
            else
            {
                k_Algorithm.Copy(ak_Last, this.End, ak_First);
                k_Algorithm.Fill(ak_Last, this.End, null);
            }

            mi_Count -= li_Count;

            return new k_BlockIterator(this, li_FirstIndex);
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
            if (mi_Offset % mi_BlockSize == 0                        // currently on block boundary
                && mk_Blocks.Length * mi_BlockSize - mi_Count < mi_BlockSize)
            {
                AllocateBlock(mi_BlockSize);
            }

            if (mi_Offset == 0)
                mi_Offset = mk_Blocks.Length * mi_BlockSize;
            --mi_Offset;
            mk_Blocks[mi_Offset/mi_BlockSize][mi_Offset%mi_BlockSize] = ak_Value;
            ++mi_Count;
        }

        public void PopFront()
        {
            Erase(this.Begin);
        }

        public void PushBack(object ak_Value)
        {
            if ((mi_Offset+mi_Count) % mi_BlockSize == 0            // currently on block boundary
                && mk_Blocks.Length * mi_BlockSize - mi_Count < mi_BlockSize)
            {
                AllocateBlock(mi_BlockSize);
            }

            int li_Pos = mi_Offset + mi_Count;
            int li_Block = li_Pos/mi_BlockSize;
            if (li_Block >= mk_Blocks.Length)
                li_Block -= mk_Blocks.Length;
            mk_Blocks[li_Block][li_Pos%mi_BlockSize] = ak_Value;
            ++mi_Count;
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
            for (int i = 0; i < ai_Count; ++i)
                Insert(this.End, ak_Value);
        }
        
        public void Insert(k_Iterator ak_Where, object ak_Value)
        {
            if (ak_Where == this.Begin)
                PushFront(ak_Value);
            else if (ak_Where == this.End)
                PushBack(ak_Value);
            else
            {
                int li_Index = ((k_BlockIterator)ak_Where).Index;
                if (mk_Blocks.Length * mi_BlockSize - mi_Count < mi_BlockSize)
                    AllocateBlock(mi_BlockSize);
                
                ++mi_Count;
                if (li_Index < mi_Count/2)
                {
                    if (mi_Offset == 0)
                        mi_Offset = mk_Blocks.Length * mi_BlockSize;
                    --mi_Offset;
                    k_Iterator lk_Dest = k_Algorithm.Copy(this.Begin+1, this.Begin+li_Index+1, this.Begin);
                    lk_Dest.Current = ak_Value;
                }
                else
                {
                    // count has been incremented - there is a free element at the end
                    k_Iterator lk_Dest = this.Begin + li_Index;
                    k_Algorithm.CopyBackward(lk_Dest, this.End - 1, this.End);
                    lk_Dest.Current = ak_Value;
                }
            }
        }

        public void Insert(k_Iterator ak_Where, k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd)
        {
            int li_FirstIndex = ((k_BlockIterator)ak_Where).Index;
            int li_Count = ak_SrcEnd - ak_SrcBegin;
            if (mk_Blocks.Length * mi_BlockSize <= mi_Count + li_Count + mi_BlockSize)
                AllocateBlock(li_Count);

            mi_Count += li_Count;

            k_Iterator lk_Dest;
            if (li_FirstIndex < li_Count/2)
            {
                if (mi_Offset == 0)
                    mi_Offset = mk_Blocks.Length * mi_BlockSize;
                mi_Offset -= li_Count;
                lk_Dest = k_Algorithm.Copy(this.Begin+li_Count, this.Begin+li_FirstIndex+li_Count, this.Begin);
            }
            else
            {
                // count has been incremented - there are li_Count free elements at the end
                lk_Dest = this.Begin + li_FirstIndex;
                k_Algorithm.CopyBackward(lk_Dest, this.End - li_Count, this.End);
            }

            k_Algorithm.Copy(ak_SrcBegin, ak_SrcEnd, lk_Dest);
        }

        #region IList Members

        public int Add(object ak_Value)
        {
            PushBack(ak_Value);
            return mi_Count;
        }

        public void Clear()
        {
            for (int i=0; i<mk_Blocks.Length; ++i)
                mk_Blocks[i] = new object[mi_BlockSize];
            mi_Count = 0;
            mi_Offset = 0;
        }

        public bool Contains(object ak_Value)
        {
            return (Find(ak_Value) != this.End);
        }

        public int IndexOf(object ak_Value)
        {
            k_Iterator lk_Found = Find(ak_Value);
            if (lk_Found == this.End)
                return -1;
            return ((k_BlockIterator)lk_Found).Index;
        }

        public void Insert(int ai_Index, object ak_Value)
        {
            Insert(this.Begin + ai_Index, ak_Value);
        }

        public void Remove(object ak_Value)
        {
            Erase(Find(ak_Value));
        }

        public void RemoveAt(int ai_Index)
        {
            Erase(this.Begin + ai_Index);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public object this[int ai_Index]
        {
            get
            {
                if (ai_Index >= mi_Count || ai_Index < 0)
                    throw new ArgumentOutOfRangeException("Position out of boundary");

                int li_Pos, li_Block = CalcBlockAndPos(ai_Index, out li_Pos);
                return mk_Blocks[li_Block][li_Pos];
            }
            set
            {
                if (ai_Index >= mi_Count || ai_Index < 0)
                    throw new ArgumentOutOfRangeException("Position out of boundary");

                int li_Pos, li_Block = CalcBlockAndPos(ai_Index, out li_Pos);
                mk_Blocks[li_Block][li_Pos] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array ak_Array, int ai_Index)
        {
            for (k_Iterator lk_Iter = this.Begin.Clone(); lk_Iter != this.End; lk_Iter.Next())
                ak_Array.SetValue(lk_Iter.Current, ai_Index++);
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
            k_Deque lk_Clone = new k_Deque(this.Count);
            lk_Clone.Insert(lk_Clone.End, this.Begin, this.End);
            return lk_Clone;
        }

        #endregion

        private void AllocateBlock(int ai_MinElements)
        {
            // number of new blocks - grow by half block count (150%)
            int li_Increment = mk_Blocks.Length / 2;
            if (ai_MinElements > li_Increment*mi_BlockSize)
                li_Increment = (ai_MinElements + mi_BlockSize - 1)/mi_BlockSize;

            object[][] lk_NewBlocks = new object[mk_Blocks.Length + li_Increment][];

            // first move all blocks after offset to front
            int li_StartBlock = mi_Offset / mi_BlockSize;
            int li_BackCount = mk_Blocks.Length - li_StartBlock;
            Array.Copy(mk_Blocks, li_StartBlock, lk_NewBlocks, 0, li_BackCount);
            
            int li_TotalOld = li_BackCount;

            // second move all blocks before offset to end
            int li_FrontCount = (mi_Offset + mi_Count + mi_BlockSize - 1) / mi_BlockSize - mk_Blocks.Length;
            if (li_FrontCount > 0)
            {
                Array.Copy(mk_Blocks, 0, lk_NewBlocks, li_BackCount, li_FrontCount);
                li_TotalOld += li_FrontCount;
            }

            // actually create new empty blocks
            for (int i=li_TotalOld; i < li_TotalOld+li_Increment; ++i)
                lk_NewBlocks[i] = new object[mi_BlockSize];

            mk_Blocks = lk_NewBlocks;
            mi_Offset %= mi_BlockSize;
        }

        private int CalcBlockAndPos(int ai_Index, out int ai_Pos)
        {
            ai_Pos = mi_Offset + ai_Index;
            int li_BlockIndex = ai_Pos / mi_BlockSize;
            if (li_BlockIndex >= mk_Blocks.Length)
                li_BlockIndex -= mk_Blocks.Length;
            ai_Pos %= mi_BlockSize;
            return li_BlockIndex;
        }
    }
}
