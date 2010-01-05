using System;
using System.Collections;
using System.Text;

/*
 * $Id: TernaryTree.cs,v 1.2 2005/06/18 08:17:05 psoares33 Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
    /**
     * <h2>Ternary Search Tree</h2>
     *
     * <p>A ternary search tree is a hibrid between a binary tree and
     * a digital search tree (trie). Keys are limited to strings.
     * A data value of type char is stored in each leaf node.
     * It can be used as an index (or pointer) to the data.
     * Branches that only contain one key are compressed to one node
     * by storing a pointer to the trailer substring of the key.
     * This class is intended to serve as base class or helper class
     * to implement Dictionary collections or the like. Ternary trees
     * have some nice properties as the following: the tree can be
     * traversed in sorted order, partial matches (wildcard) can be
     * implemented, retrieval of all keys within a given distance
     * from the target, etc. The storage requirements are higher than
     * a binary tree but a lot less than a trie. Performance is
     * comparable with a hash table, sometimes it outperforms a hash
     * function (most of the time can determine a miss faster than a hash).</p>
     *
     * <p>The main purpose of this java port is to serve as a base for
     * implementing TeX's hyphenation algorithm (see The TeXBook,
     * appendix H). Each language requires from 5000 to 15000 hyphenation
     * patterns which will be keys in this tree. The strings patterns
     * are usually small (from 2 to 5 characters), but each char in the
     * tree is stored in a node. Thus memory usage is the main concern.
     * We will sacrify 'elegance' to keep memory requirenments to the
     * minimum. Using java's char type as pointer (yes, I know pointer
     * it is a forbidden word in java) we can keep the size of the node
     * to be just 8 bytes (3 pointers and the data char). This gives
     * room for about 65000 nodes. In my tests the english patterns
     * took 7694 nodes and the german patterns 10055 nodes,
     * so I think we are safe.</p>
     *
     * <p>All said, this is a map with strings as keys and char as value.
     * Pretty limited!. It can be extended to a general map by
     * using the string representation of an object and using the
     * char value as an index to an array that contains the object
     * values.</p>
     *
     * @author cav@uniscope.co.jp
     */

    public class TernaryTree : ICloneable {

        /**
         * We use 4 arrays to represent a node. I guess I should have created
         * a proper node class, but somehow Knuth's pascal code made me forget
         * we now have a portable language with memory management and
         * automatic garbage collection! And now is kind of late, furthermore,
         * if it ain't broken, don't fix it.
         */

        /**
         * Pointer to low branch and to rest of the key when it is
         * stored directly in this node, we don't have unions in java!
         */
        protected char[] lo;

        /**
         * Pointer to high branch.
         */
        protected char[] hi;

        /**
         * Pointer to equal branch and to data when this node is a string terminator.
         */
        protected char[] eq;

        /**
         * <P>The character stored in this node: splitchar
         * Two special values are reserved:</P>
         * <ul><li>0x0000 as string terminator</li>
         * <li>0xFFFF to indicate that the branch starting at
         * this node is compressed</li></ul>
         * <p>This shouldn't be a problem if we give the usual semantics to
         * strings since 0xFFFF is garanteed not to be an Unicode character.</p>
         */
        protected char[] sc;

        /**
         * This vector holds the trailing of the keys when the branch is compressed.
         */
        protected CharVector kv;

        protected char root;
        protected char freenode;
        protected int length;    // number of items in tree

        protected static int BLOCK_SIZE = 2048;    // allocation size for arrays

        internal TernaryTree() {
            Init();
        }

        protected void Init() {
            root = (char)0;
            freenode = (char)1;
            length = 0;
            lo = new char[BLOCK_SIZE];
            hi = new char[BLOCK_SIZE];
            eq = new char[BLOCK_SIZE];
            sc = new char[BLOCK_SIZE];
            kv = new CharVector();
        }

        /**
         * Branches are initially compressed, needing
         * one node per key plus the size of the string
         * key. They are decompressed as needed when
         * another key with same prefix
         * is inserted. This saves a lot of space,
         * specially for long keys.
         */
        public void Insert(string key, char val) {
            // make sure we have enough room in the arrays
            int len = key.Length
                + 1;    // maximum number of nodes that may be generated
            if (freenode + len > eq.Length)
                RedimNodeArrays(eq.Length + BLOCK_SIZE);
            char[] strkey = new char[len--];
            key.CopyTo(0, strkey, 0, len); 
            strkey[len] = (char)0;
            root = Insert(root, strkey, 0, val);
        }

        public void Insert(char[] key, int start, char val) {
            int len = Strlen(key) + 1;
            if (freenode + len > eq.Length)
                RedimNodeArrays(eq.Length + BLOCK_SIZE);
            root = Insert(root, key, start, val);
        }

        /**
         * The actual insertion function, recursive version.
         */
        private char Insert(char p, char[] key, int start, char val) {
            int len = Strlen(key, start);
            if (p == 0) {
                // this means there is no branch, this node will start a new branch.
                // Instead of doing that, we store the key somewhere else and create
                // only one node with a pointer to the key
                p = freenode++;
                eq[p] = val;           // holds data
                length++;
                hi[p] = (char)0;
                if (len > 0) {
                    sc[p] = (char)0xFFFF;    // indicates branch is compressed
                    lo[p] = (char)kv.Alloc(len
                        + 1);    // use 'lo' to hold pointer to key
                    Strcpy(kv.Arr, lo[p], key, start);
                } else {
                    sc[p] = (char)0;
                    lo[p] = (char)0;
                }
                return p;
            }

            if (sc[p] == 0xFFFF) {
                // branch is compressed: need to decompress
                // this will generate garbage in the external key array
                // but we can do some garbage collection later
                char pp = freenode++;
                lo[pp] = lo[p];    // previous pointer to key
                eq[pp] = eq[p];    // previous pointer to data
                lo[p] = (char)0;
                if (len > 0) {
                    sc[p] = kv[lo[pp]];
                    eq[p] = pp;
                    lo[pp]++;
                    if (kv[lo[pp]] == 0) {
                        // key completly decompressed leaving garbage in key array
                        lo[pp] = (char)0;
                        sc[pp] = (char)0;
                        hi[pp] = (char)0;
                    } else
                        sc[pp] =
                            (char)0xFFFF;    // we only got first char of key, rest is still there
                } else {
                    // In this case we can save a node by swapping the new node
                    // with the compressed node
                    sc[pp] = (char)0xFFFF;
                    hi[p] = pp;
                    sc[p] = (char)0;
                    eq[p] = val;
                    length++;
                    return p;
                }
            }
            char s = key[start];
            if (s < sc[p])
                lo[p] = Insert(lo[p], key, start, val);
            else if (s == sc[p]) {
                if (s != 0)
                    eq[p] = Insert(eq[p], key, start + 1, val);
                else {
                    // key already in tree, overwrite data
                    eq[p] = val;
                }

            } else
                hi[p] = Insert(hi[p], key, start, val);
            return p;
        }

        /**
         * Compares 2 null terminated char arrays
         */
        public static int Strcmp(char[] a, int startA, char[] b, int startB) {
            for (; a[startA] == b[startB]; startA++, startB++)
                if (a[startA] == 0)
                    return 0;
            return a[startA] - b[startB];
        }

        /**
         * Compares a string with null terminated char array
         */
        public static int Strcmp(string str, char[] a, int start) {
            int i, d, len = str.Length;
            for (i = 0; i < len; i++) {
                d = (int)str[i] - a[start + i];
                if (d != 0)
                    return d;
                if (a[start + i] == 0)
                    return d;
            }
            if (a[start + i] != 0)
                return (int)-a[start + i];
            return 0;

        }

        public static void Strcpy(char[] dst, int di, char[] src, int si) {
            while (src[si] != 0)
                dst[di++] = src[si++];
            dst[di] = (char)0;
        }

        public static int Strlen(char[] a, int start) {
            int len = 0;
            for (int i = start; i < a.Length && a[i] != 0; i++)
                len++;
            return len;
        }

        public static int Strlen(char[] a) {
            return Strlen(a, 0);
        }

        public int Find(string key) {
            int len = key.Length;
            char[] strkey = new char[len + 1];
            key.CopyTo(0, strkey, 0, len);
            strkey[len] = (char)0;

            return Find(strkey, 0);
        }

        public int Find(char[] key, int start) {
            int d;
            char p = root;
            int i = start;
            char c;

            while (p != 0) {
                if (sc[p] == 0xFFFF) {
                    if (Strcmp(key, i, kv.Arr, lo[p]) == 0)
                        return eq[p];
                    else
                        return -1;
                }
                c = key[i];
                d = c - sc[p];
                if (d == 0) {
                    if (c == 0)
                        return eq[p];
                    i++;
                    p = eq[p];
                } else if (d < 0)
                    p = lo[p];
                else
                    p = hi[p];
            }
            return -1;
        }

        public bool Knows(string key) {
            return (Find(key) >= 0);
        }

        // redimension the arrays
        private void RedimNodeArrays(int newsize) {
            int len = newsize < lo.Length ? newsize : lo.Length;
            char[] na = new char[newsize];
            Array.Copy(lo, 0, na, 0, len);
            lo = na;
            na = new char[newsize];
            Array.Copy(hi, 0, na, 0, len);
            hi = na;
            na = new char[newsize];
            Array.Copy(eq, 0, na, 0, len);
            eq = na;
            na = new char[newsize];
            Array.Copy(sc, 0, na, 0, len);
            sc = na;
        }

        public int Size {
            get {
                return length;
            }
        }

        public Object Clone() {
            TernaryTree t = new TernaryTree();
            t.lo = (char[])this.lo.Clone();
            t.hi = (char[])this.hi.Clone();
            t.eq = (char[])this.eq.Clone();
            t.sc = (char[])this.sc.Clone();
            t.kv = (CharVector)this.kv.Clone();
            t.root = this.root;
            t.freenode = this.freenode;
            t.length = this.length;

            return t;
        }

        /**
         * Recursively insert the median first and then the median of the
         * lower and upper halves, and so on in order to get a balanced
         * tree. The array of keys is assumed to be sorted in ascending
         * order.
         */
        protected void InsertBalanced(string[] k, char[] v, int offset, int n) {
            int m;
            if (n < 1)
                return;
            m = n >> 1;

            Insert(k[m + offset], v[m + offset]);
            InsertBalanced(k, v, offset, m);

            InsertBalanced(k, v, offset + m + 1, n - m - 1);
        }


        /**
         * Balance the tree for best search performance
         */
        public void Balance() {
            // System.out.Print("Before root splitchar = "); System.out.Println(sc[root]);

            int i = 0, n = length;
            string[] k = new string[n];
            char[] v = new char[n];
            Iterator iter = new Iterator(this);
            while (iter.HasMoreElements()) {
                v[i] = iter.Value;
                k[i++] = (string)iter.NextElement();
            }
            Init();
            InsertBalanced(k, v, 0, n);

            // With uniform letter distribution sc[root] should be around 'm'
            // System.out.Print("After root splitchar = "); System.out.Println(sc[root]);
        }

        /**
         * Each node stores a character (splitchar) which is part of
         * some Key(s). In a compressed branch (one that only contain
         * a single string key) the trailer of the key which is not
         * already in nodes is stored  externally in the kv array.
         * As items are inserted, key substrings decrease.
         * Some substrings may completely  disappear when the whole
         * branch is totally decompressed.
         * The tree is traversed to find the key substrings actually
         * used. In addition, duplicate substrings are removed using
         * a map (implemented with a TernaryTree!).
         *
         */
        public void TrimToSize() {
            // first balance the tree for best performance
            Balance();

            // redimension the node arrays
            RedimNodeArrays(freenode);

            // ok, compact kv array
            CharVector kx = new CharVector();
            kx.Alloc(1);
            TernaryTree map = new TernaryTree();
            Compact(kx, map, root);
            kv = kx;
            kv.TrimToSize();
        }

        private void Compact(CharVector kx, TernaryTree map, char p) {
            int k;
            if (p == 0)
                return;
            if (sc[p] == 0xFFFF) {
                k = map.Find(kv.Arr, lo[p]);
                if (k < 0) {
                    k = kx.Alloc(Strlen(kv.Arr, lo[p]) + 1);
                    Strcpy(kx.Arr, k, kv.Arr, lo[p]);
                    map.Insert(kx.Arr, k, (char)k);
                }
                lo[p] = (char)k;
            } else {
                Compact(kx, map, lo[p]);
                if (sc[p] != 0)
                    Compact(kx, map, eq[p]);
                Compact(kx, map, hi[p]);
            }
        }


        public Iterator Keys {
            get {
                return new Iterator(this);
            }
        }

        public class Iterator {

            /**
            * current node index
            */
            int cur;

            /**
             * current key
             */
            string curkey;

            /**
             * TernaryTree parent
             */
            TernaryTree parent; 

            private class Item : ICloneable {
                internal char parent;
                internal char child;

                public Item() {
                    parent = (char)0;
                    child = (char)0;
                }

                public Item(char p, char c) {
                    parent = p;
                    child = c;
                }

                public Object Clone() {
                    return new Item(parent, child);
                }

            }

            /**
             * Node stack
             */
            Stack ns;

            /**
             * key stack implemented with a StringBuilder
             */
            StringBuilder ks;

            public Iterator(TernaryTree parent) {
                this.parent = parent;
                cur = -1;
                ns = new Stack();
                ks = new StringBuilder();
                Rewind();
            }

            public void Rewind() {
                ns.Clear();
                ks.Length = 0;
                cur = parent.root;
                Run();
            }

            public Object NextElement() {
                string res = curkey;
                cur = Up();
                Run();
                return res;
            }

            public char Value {
                get {
                    if (cur >= 0)
                        return this.parent.eq[cur];
                    return (char)0;
                }
            }

            public bool HasMoreElements() {
                return (cur != -1);
            }

            /**
             * traverse upwards
             */
            private int Up() {
                Item i = new Item();
                int res = 0;

                if (ns.Count == 0)
                    return -1;

                if (cur != 0 && parent.sc[cur] == 0)
                    return parent.lo[cur];

                bool climb = true;

                while (climb) {
                    i = (Item)ns.Pop();
                    i.child++;
                    switch (i.child) {
                        case (char)1:
                            if (parent.sc[i.parent] != 0) {
                                res = parent.eq[i.parent];
                                ns.Push(i.Clone());
                                ks.Append(parent.sc[i.parent]);
                            } else {
                                i.child++;
                                ns.Push(i.Clone());
                                res = parent.hi[i.parent];
                            }
                            climb = false;
                            break;

                        case (char)2:
                            res = parent.hi[i.parent];
                            ns.Push(i.Clone());
                            if (ks.Length > 0)
                                ks.Length = ks.Length - 1;    // pop
                            climb = false;
                            break;

                        default:
                            if (ns.Count == 0)
                                return -1;
                            climb = true;
                            break;
                    }
                }
                return res;
            }

            /**
             * traverse the tree to find next key
             */
            private int Run() {
                if (cur == -1)
                    return -1;

                bool leaf = false;
                for (; ; ) {
                    // first go down on low branch until leaf or compressed branch
                    while (cur != 0) {
                        if (parent.sc[cur] == 0xFFFF) {
                            leaf = true;
                            break;
                        }
                        ns.Push(new Item((char)cur, '\u0000'));
                        if (parent.sc[cur] == 0) {
                            leaf = true;
                            break;
                        }
                        cur = parent.lo[cur];
                    }
                    if (leaf)
                        break;
                    // nothing found, go up one node and try again
                    cur = Up();
                    if (cur == -1) {
                        return -1;
                    }
                }
                // The current node should be a data node and
                // the key should be in the key stack (at least partially)
                StringBuilder buf = new StringBuilder(ks.ToString());
                if (parent.sc[cur] == 0xFFFF) {
                    int p = parent.lo[cur];
                    while (parent.kv[p] != 0)
                        buf.Append(parent.kv[p++]);
                }
                curkey = buf.ToString();
                return 0;
            }

        }

        public virtual void PrintStats() {
            Console.Error.WriteLine("Number of keys = " + length.ToString());
            Console.Error.WriteLine("Node count = " + freenode.ToString());
            // Console.Error.WriteLine("Array length = " + int.ToString(eq.Length));
            Console.Error.WriteLine("Key Array length = "
                       + kv.Length.ToString());

            /*
             * for (int i=0; i<kv.Length; i++)
             * if ( kv[i] != 0 )
             * System.out.Print(kv[i]);
             * else
             * System.out.Println("");
             * System.out.Println("Keys:");
             * for (Enumeration enum = Keys(); enum.HasMoreElements(); )
             * System.out.Println(enum.NextElement());
             */

        }
    }
}
