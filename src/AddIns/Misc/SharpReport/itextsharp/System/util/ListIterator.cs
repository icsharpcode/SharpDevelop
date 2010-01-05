using System;
using System.Collections;

namespace System.util {
    /// <summary>
    /// Summary description for ListIterator.
    /// </summary>
    public class ListIterator {
        ArrayList col;
        int cursor = 0;
        int lastRet = -1;

        public ListIterator(ArrayList col) {
            this.col = col;
        }

        public bool HasNext() {
            return cursor != col.Count;
        }

        public object Next() {
            Object next = col[cursor];
            lastRet = cursor++;
            return next;
        }

        public object Previous() {
            int i = cursor - 1;
            Object previous = col[i];
            lastRet = cursor = i;
            return previous;
        }

        public void Remove() {
            if (lastRet == -1)
                throw new InvalidOperationException();
            col.RemoveAt(lastRet);
            if (lastRet < cursor)
                cursor--;
            lastRet = -1;
        }
    }
}
