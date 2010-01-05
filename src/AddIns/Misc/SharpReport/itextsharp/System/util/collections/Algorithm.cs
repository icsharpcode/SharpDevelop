using System;

namespace System.util.collections
{
    /// <summary>
    /// Very basic algorithms tool class.
    /// </summary>
    public class k_Algorithm
    {
        public static k_Iterator Copy(k_Iterator ak_SrcFirst, k_Iterator ak_BehindSrcLast, k_Iterator ak_DstFirst)
        {
            k_Iterator lk_Src = ak_SrcFirst.Clone(), lk_Dst = ak_DstFirst.Clone();
            while (lk_Src != ak_BehindSrcLast)
            {
                lk_Dst.Current = lk_Src.Current;
                lk_Src.Next(); lk_Dst.Next();
            }
            return lk_Dst;    
        }

        public static k_Iterator CopyBackward(k_Iterator ak_SrcFirst, k_Iterator ak_BehindSrcLast, k_Iterator ak_BehindDstLast)
        {
            k_Iterator lk_Src = ak_BehindSrcLast.Clone(), lk_Dst = ak_BehindDstLast.Clone();
            while (lk_Src != ak_SrcFirst)
            {
                lk_Src.Prev(); lk_Dst.Prev();
                lk_Dst.Current = lk_Src.Current;
            }
            return lk_Dst;
        }

        public static void Fill(k_Iterator ak_DstFirst, k_Iterator ak_BehindDstLast, object ak_Value)
        {
            for (k_Iterator lk_Iter = ak_DstFirst.Clone(); lk_Iter != ak_BehindDstLast; lk_Iter.Next())
                lk_Iter.Current = ak_Value;
        }

        public static k_Iterator Find(k_Iterator ak_First, k_Iterator ak_Last, object ak_Value)
        {
            k_Iterator lk_Iter = ak_First.Clone();
            for (; lk_Iter != ak_Last; lk_Iter.Next())
            {
                if (object.Equals(lk_Iter.Current, ak_Value))
                    break;
            }
            return lk_Iter;
        }
    }
}
