using System;
using System.Text;

/*
 * $Id: Hyphenation.cs,v 1.2 2005/06/18 08:17:05 psoares33 Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
    /**
     * This class represents a hyphenated word.
     *
     * @author Carlos Villegas <cav@uniscope.co.jp>
     */
    public class Hyphenation {
        int[] hyphenPoints;
        string word;

        /**
         * number of hyphenation points in word
         */
        int len;

        /**
         * rawWord as made of alternating strings and {@link Hyphen Hyphen}
         * instances
         */
        internal Hyphenation(string word, int[] points) {
            this.word = word;
            hyphenPoints = points;
            len = points.Length;
        }

        /**
         * @return the number of hyphenation points in the word
         */
        public int Length {
            get {
                return len;
            }
        }

        /**
         * @return the pre-break text, not including the hyphen character
         */
        public string GetPreHyphenText(int index) {
            return word.Substring(0, hyphenPoints[index]);
        }

        /**
         * @return the post-break text
         */
        public string GetPostHyphenText(int index) {
            return word.Substring(hyphenPoints[index]);
        }

        /**
         * @return the hyphenation points
         */
        public int[] HyphenationPoints {
            get {
                return hyphenPoints;
            }
        }

        public override string ToString() {
            StringBuilder str = new StringBuilder();
            int start = 0;
            for (int i = 0; i < len; i++) {
                str.Append(word.Substring(start, hyphenPoints[i]) + "-");
                start = hyphenPoints[i];
            }
            str.Append(word.Substring(start));
            return str.ToString();
        }
    }
}
