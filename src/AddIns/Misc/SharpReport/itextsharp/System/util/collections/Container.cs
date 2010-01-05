using System;
using System.Collections;

namespace System.util.collections
{
    /// <summary>
    /// Base interface for all containers
    /// </summary>
    public interface IContainer : ICollection, ICloneable
    {
        k_Iterator Begin { get; }
        k_Iterator End { get; }

        bool IsEmpty { get; }

        k_Iterator Find(object ak_Value);
    
        k_Iterator Erase(k_Iterator ak_Where);
        k_Iterator Erase(k_Iterator ak_First, k_Iterator ak_Last);
    }

    /// <summary>
    /// Interface for non-associative sequential containers (k_Vector, k_Deque, k_List)
    /// </summary>
    public interface ISequence : IContainer, IList
    {
        object Front { get; set; }
        object Back { get; set; }

        void PushFront(object ak_Value);
        void PopFront();
        
        void PushBack(object ak_Value);
        void PopBack();

        void Assign(k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd);
        void Assign(object ak_Value, int ai_Count);

        void Insert(k_Iterator ak_Where, object ak_Value);
        void Insert(k_Iterator ak_Where, k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd);
    }

    /// <summary>
    /// Interface for IDictionary derived containers which provide key to value mapping (k_HashTable)
    /// </summary>
    public interface IMap : IContainer, IDictionary
    {
        k_Iterator FindKey(object ak_Key);

        void Add(DictionaryEntry ar_Item);
        void Insert(k_Iterator ak_SrcBegin, k_Iterator ak_SrcEnd);
    }

    /// <summary>
    /// Interface for sorted mapping containers (k_SkipList, k_Tree)
    /// </summary>
    public interface ISortedMap : IMap
    {
        IComparer Comparer { get; }

        k_Iterator LowerBound(object ak_Key);
        k_Iterator UpperBound(object ak_Key);
    }
}
