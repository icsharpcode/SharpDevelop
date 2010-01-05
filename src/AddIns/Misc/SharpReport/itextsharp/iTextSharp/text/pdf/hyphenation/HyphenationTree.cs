using System;
using System.IO;
using System.Text;
using System.Collections;
/*
 * Copyright 1999-2004 The Apache Software Foundation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/* $Id: HyphenationTree.cs,v 1.2 2005/06/18 08:05:23 psoares33 Exp $ */
 
namespace iTextSharp.text.pdf.hyphenation {
    /**
    * This tree structure stores the hyphenation patterns in an efficient
    * way for fast lookup. It provides the provides the method to
    * hyphenate a word.
    *
    * @author Carlos Villegas <cav@uniscope.co.jp>
    */
    public class HyphenationTree : TernaryTree, IPatternConsumer {

        /**
        * value space: stores the inteletter values
        */
        protected ByteVector vspace;

        /**
        * This map stores hyphenation exceptions
        */
        protected Hashtable stoplist;

        /**
        * This map stores the character classes
        */
        protected TernaryTree classmap;

        /**
        * Temporary map to store interletter values on pattern loading.
        */
        private TernaryTree ivalues;

        public HyphenationTree() {
            stoplist = new Hashtable(23);    // usually a small table
            classmap = new TernaryTree();
            vspace = new ByteVector();
            vspace.Alloc(1);    // this reserves index 0, which we don't use
        }

        /**
        * Packs the values by storing them in 4 bits, two values into a byte
        * Values range is from 0 to 9. We use zero as terminator,
        * so we'll add 1 to the value.
        * @param values a string of digits from '0' to '9' representing the
        * interletter values.
        * @return the index into the vspace array where the packed values
        * are stored.
        */
        protected int PackValues(String values) {
            int i, n = values.Length;
            int m = (n & 1) == 1 ? (n >> 1) + 2 : (n >> 1) + 1;
            int offset = vspace.Alloc(m);
            byte[] va = vspace.Arr;
            for (i = 0; i < n; i++) {
                int j = i >> 1;
                byte v = (byte)((values[i] - '0' + 1) & 0x0f);
                if ((i & 1) == 1) {
                    va[j + offset] = (byte)(va[j + offset] | v);
                } else {
                    va[j + offset] = (byte)(v << 4);    // big endian
                }
            }
            va[m - 1 + offset] = 0;    // terminator
            return offset;
        }

        protected String UnpackValues(int k) {
            StringBuilder buf = new StringBuilder();
            byte v = vspace[k++];
            while (v != 0) {
                char c = (char)((v >> 4) - 1 + '0');
                buf.Append(c);
                c = (char)(v & 0x0f);
                if (c == 0) {
                    break;
                }
                c = (char)(c - 1 + '0');
                buf.Append(c);
                v = vspace[k++];
            }
            return buf.ToString();
        }

        public void LoadSimplePatterns(Stream stream) {
            SimplePatternParser pp = new SimplePatternParser();
            ivalues = new TernaryTree();

            pp.Parse(stream, this);

            // patterns/values should be now in the tree
            // let's optimize a bit
            TrimToSize();
            vspace.TrimToSize();
            classmap.TrimToSize();

            // get rid of the auxiliary map
            ivalues = null;
        }


        public String FindPattern(String pat) {
            int k = base.Find(pat);
            if (k >= 0) {
                return UnpackValues(k);
            }
            return "";
        }

        /**
        * String compare, returns 0 if equal or
        * t is a substring of s
        */
        protected int Hstrcmp(char[] s, int si, char[] t, int ti) {
            for (; s[si] == t[ti]; si++, ti++) {
                if (s[si] == 0) {
                    return 0;
                }
            }
            if (t[ti] == 0) {
                return 0;
            }
            return s[si] - t[ti];
        }

        protected byte[] GetValues(int k) {
            StringBuilder buf = new StringBuilder();
            byte v = vspace[k++];
            while (v != 0) {
                char c = (char)((v >> 4) - 1);
                buf.Append(c);
                c = (char)(v & 0x0f);
                if (c == 0) {
                    break;
                }
                c = (char)(c - 1);
                buf.Append(c);
                v = vspace[k++];
            }
            byte[] res = new byte[buf.Length];
            for (int i = 0; i < res.Length; i++) {
                res[i] = (byte)buf[i];
            }
            return res;
        }

        /**
        * <p>Search for all possible partial matches of word starting
        * at index an update interletter values. In other words, it
        * does something like:</p>
        * <code>
        * for (i=0; i<patterns.length; i++) {
        * if ( word.Substring(index).StartsWidth(patterns[i]) )
        * Update_interletter_values(patterns[i]);
        * }
        * </code>
        * <p>But it is done in an efficient way since the patterns are
        * stored in a ternary tree. In fact, this is the whole purpose
        * of having the tree: doing this search without having to test
        * every single pattern. The number of patterns for languages
        * such as English range from 4000 to 10000. Thus, doing thousands
        * of string comparisons for each word to hyphenate would be
        * really slow without the tree. The tradeoff is memory, but
        * using a ternary tree instead of a trie, almost halves the
        * the memory used by Lout or TeX. It's also faster than using
        * a hash table</p>
        * @param word null terminated word to match
        * @param index start index from word
        * @param il interletter values array to update
        */
        protected void SearchPatterns(char[] word, int index, byte[] il) {
            byte[] values;
            int i = index;
            char p, q;
            char sp = word[i];
            p = root;

            while (p > 0 && p < sc.Length) {
                if (sc[p] == 0xFFFF) {
                    if (Hstrcmp(word, i, kv.Arr, lo[p]) == 0) {
                        values = GetValues(eq[p]);    // data pointer is in eq[]
                        int j = index;
                        for (int k = 0; k < values.Length; k++) {
                            if (j < il.Length && values[k] > il[j]) {
                                il[j] = values[k];
                            }
                            j++;
                        }
                    }
                    return;
                }
                int d = sp - sc[p];
                if (d == 0) {
                    if (sp == 0) {
                        break;
                    }
                    sp = word[++i];
                    p = eq[p];
                    q = p;

                    // look for a pattern ending at this position by searching for
                    // the null char ( splitchar == 0 )
                    while (q > 0 && q < sc.Length) {
                        if (sc[q] == 0xFFFF) {        // stop at compressed branch
                            break;
                        }
                        if (sc[q] == 0) {
                            values = GetValues(eq[q]);
                            int j = index;
                            for (int k = 0; k < values.Length; k++) {
                                if (j < il.Length && values[k] > il[j]) {
                                    il[j] = values[k];
                                }
                                j++;
                            }
                            break;
                        } else {
                            q = lo[q];

                            /**
                            * actually the code should be:
                            * q = sc[q] < 0 ? hi[q] : lo[q];
                            * but java chars are unsigned
                            */
                        }
                    }
                } else {
                    p = d < 0 ? lo[p] : hi[p];
                }
            }
        }

        /**
        * Hyphenate word and return a Hyphenation object.
        * @param word the word to be hyphenated
        * @param remainCharCount Minimum number of characters allowed
        * before the hyphenation point.
        * @param pushCharCount Minimum number of characters allowed after
        * the hyphenation point.
        * @return a {@link Hyphenation Hyphenation} object representing
        * the hyphenated word or null if word is not hyphenated.
        */
        public Hyphenation Hyphenate(String word, int remainCharCount,
                                    int pushCharCount) {
            char[] w = word.ToCharArray();
            return Hyphenate(w, 0, w.Length, remainCharCount, pushCharCount);
        }

        /**
        * w = "****nnllllllnnn*****",
        * where n is a non-letter, l is a letter,
        * all n may be absent, the first n is at offset,
        * the first l is at offset + iIgnoreAtBeginning;
        * word = ".llllll.'\0'***",
        * where all l in w are copied into word.
        * In the first part of the routine len = w.length,
        * in the second part of the routine len = word.length.
        * Three indices are used:
        * Index(w), the index in w,
        * Index(word), the index in word,
        * Letterindex(word), the index in the letter part of word.
        * The following relations exist:
        * Index(w) = offset + i - 1
        * Index(word) = i - iIgnoreAtBeginning
        * Letterindex(word) = Index(word) - 1
        * (see first loop).
        * It follows that:
        * Index(w) - Index(word) = offset - 1 + iIgnoreAtBeginning
        * Index(w) = Letterindex(word) + offset + iIgnoreAtBeginning
        */

        /**
        * Hyphenate word and return an array of hyphenation points.
        * @param w char array that contains the word
        * @param offset Offset to first character in word
        * @param len Length of word
        * @param remainCharCount Minimum number of characters allowed
        * before the hyphenation point.
        * @param pushCharCount Minimum number of characters allowed after
        * the hyphenation point.
        * @return a {@link Hyphenation Hyphenation} object representing
        * the hyphenated word or null if word is not hyphenated.
        */
        public Hyphenation Hyphenate(char[] w, int offset, int len,
                                    int remainCharCount, int pushCharCount) {
            int i;
            char[] word = new char[len + 3];

            // normalize word
            char[] c = new char[2];
            int iIgnoreAtBeginning = 0;
            int iLength = len;
            bool bEndOfLetters = false;
            for (i = 1; i <= len; i++) {
                c[0] = w[offset + i - 1];
                int nc = classmap.Find(c, 0);
                if (nc < 0) {    // found a non-letter character ...
                    if (i == (1 + iIgnoreAtBeginning)) {
                        // ... before any letter character
                        iIgnoreAtBeginning ++;
                    } else {
                        // ... after a letter character
                        bEndOfLetters = true;
                    }
                    iLength --;
                } else {
                    if (!bEndOfLetters) {
                        word[i - iIgnoreAtBeginning] = (char)nc;
                    } else {
                        return null;
                    }
                }
            }
            len = iLength;
            if (len < (remainCharCount + pushCharCount)) {
                // word is too short to be hyphenated
                return null;
            }
            int[] result = new int[len + 1];
            int k = 0;

            // check exception list first
            String sw = new String(word, 1, len);
            if (stoplist.ContainsKey(sw)) {
                // assume only simple hyphens (Hyphen.pre="-", Hyphen.post = Hyphen.no = null)
                ArrayList hw = (ArrayList)stoplist[sw];
                int j = 0;
                for (i = 0; i < hw.Count; i++) {
                    Object o = hw[i];
                    // j = Index(sw) = Letterindex(word)?
                    // result[k] = corresponding Index(w)
                    if (o is String) {
                        j += ((String)o).Length;
                        if (j >= remainCharCount && j < (len - pushCharCount)) {
                            result[k++] = j + iIgnoreAtBeginning;
                        }
                    }
                }
            } else {
                // use algorithm to get hyphenation points
                word[0] = '.';                    // word start marker
                word[len + 1] = '.';              // word end marker
                word[len + 2] = (char)0;                // null terminated
                byte[] il = new byte[len + 3];    // initialized to zero
                for (i = 0; i < len + 1; i++) {
                    SearchPatterns(word, i, il);
                }

                // hyphenation points are located where interletter value is odd
                // i is Letterindex(word),
                // i + 1 is Index(word),
                // result[k] = corresponding Index(w)
                for (i = 0; i < len; i++) {
                    if (((il[i + 1] & 1) == 1) && i >= remainCharCount
                            && i <= (len - pushCharCount)) {
                        result[k++] = i + iIgnoreAtBeginning;
                    }
                }
            }


            if (k > 0) {
                // trim result array
                int[] res = new int[k];
                Array.Copy(result, 0, res, 0, k);
                return new Hyphenation(new String(w, offset, len), res);
            } else {
                return null;
            }
        }

        /**
        * Add a character class to the tree. It is used by
        * {@link SimplePatternParser SimplePatternParser} as callback to
        * add character classes. Character classes define the
        * valid word characters for hyphenation. If a word contains
        * a character not defined in any of the classes, it is not hyphenated.
        * It also defines a way to normalize the characters in order
        * to compare them with the stored patterns. Usually pattern
        * files use only lower case characters, in this case a class
        * for letter 'a', for example, should be defined as "aA", the first
        * character being the normalization char.
        */
        public void AddClass(String chargroup) {
            if (chargroup.Length > 0) {
                char equivChar = chargroup[0];
                char[] key = new char[2];
                key[1] = (char)0;
                for (int i = 0; i < chargroup.Length; i++) {
                    key[0] = chargroup[i];
                    classmap.Insert(key, 0, equivChar);
                }
            }
        }

        /**
        * Add an exception to the tree. It is used by
        * {@link SimplePatternParser SimplePatternParser} class as callback to
        * store the hyphenation exceptions.
        * @param word normalized word
        * @param hyphenatedword a vector of alternating strings and
        * {@link Hyphen hyphen} objects.
        */
        public void AddException(String word, ArrayList hyphenatedword) {
            stoplist[word] = hyphenatedword;
        }

        /**
        * Add a pattern to the tree. Mainly, to be used by
        * {@link SimplePatternParser SimplePatternParser} class as callback to
        * add a pattern to the tree.
        * @param pattern the hyphenation pattern
        * @param ivalue interletter weight values indicating the
        * desirability and priority of hyphenating at a given point
        * within the pattern. It should contain only digit characters.
        * (i.e. '0' to '9').
        */
        public void AddPattern(String pattern, String ivalue) {
            int k = ivalues.Find(ivalue);
            if (k <= 0) {
                k = PackValues(ivalue);
                ivalues.Insert(ivalue, (char)k);
            }
            Insert(pattern, (char)k);
        }

        public override void PrintStats() {
            Console.WriteLine("Value space size = " + vspace.Length);
            base.PrintStats();
        }
    }
}