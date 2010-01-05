using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util;
using iTextSharp.text.pdf;
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

namespace iTextSharp.text.pdf.hyphenation {
    /**
    * This class is the main entry point to the hyphenation package.
    * You can use only the static methods or create an instance.
    *
    * @author Carlos Villegas <cav@uniscope.co.jp>
    */
    public class Hyphenator {
        
        /** TODO: Don't use statics */
        private static Hashtable hyphenTrees = Hashtable.Synchronized(new Hashtable());

        private HyphenationTree hyphenTree = null;
        private int remainCharCount = 2;
        private int pushCharCount = 2;
        private const String defaultHyphLocation = "iTextSharp.text.pdf.hyphenation.hyph.";
       
        /**
        * @param lang
        * @param country
        * @param leftMin
        * @param rightMin
        */
        public Hyphenator(String lang, String country, int leftMin,
                        int rightMin) {
            hyphenTree = GetHyphenationTree(lang, country);
            remainCharCount = leftMin;
            pushCharCount = rightMin;
        }

        /**
        * @param lang
        * @param country
        * @return the hyphenation tree
        */
        public static HyphenationTree GetHyphenationTree(String lang,
                String country) {
            String key = lang;
            // check whether the country code has been used
            if (country != null && !country.Equals("none")) {
                key += "_" + country;
            }
                // first try to find it in the cache
            if (hyphenTrees.ContainsKey(key)) {
                return (HyphenationTree)hyphenTrees[key];
            }
            if (hyphenTrees.ContainsKey(lang)) {
                return (HyphenationTree)hyphenTrees[lang];
            }

            HyphenationTree hTree = GetResourceHyphenationTree(key);
            //if (hTree == null)
            //    hTree = GetFileHyphenationTree(key);
            // put it into the pattern cache
            if (hTree != null) {
                hyphenTrees[key] = hTree;
            }
            return hTree;
        }

        /**
        * @param key
        * @return a hyphenation tree
        */
        public static HyphenationTree GetResourceHyphenationTree(String key) {
            try {
                Stream stream = BaseFont.GetResourceStream(defaultHyphLocation + key + ".xml");
                if (stream == null && key.Length > 2)
                    stream = BaseFont.GetResourceStream(defaultHyphLocation + key.Substring(0, 2) + ".xml");
                if (stream == null)
                    return null;
                HyphenationTree hTree = new HyphenationTree();
                hTree.LoadSimplePatterns(stream);
                return hTree;
            }
            catch {
                return null;
            }
        }

        /**
        * @param key
        * @return a hyphenation tree
        */
/*        public static HyphenationTree GetFileHyphenationTree(String key) {
            try {
                if (hyphenDir == null)
                    return null;
                Stream stream = null;
                string hyphenFile = Path.Combine(hyphenDir, key + ".xml");
                if (File.Exists(hyphenFile))
                    stream = new FileStream(hyphenFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (stream == null && key.Length > 2) {
                    hyphenFile = Path.Combine(hyphenDir, key.Substring(0, 2) + ".xml");
                    if (File.Exists(hyphenFile))
                        stream = new FileStream(hyphenFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                if (stream == null)
                    return null;
                HyphenationTree hTree = new HyphenationTree();
                hTree.LoadSimplePatterns(stream);
                return hTree;
            }
            catch (Exception e) {
                return null;
            }
        }*/

        /**
        * @param lang
        * @param country
        * @param word
        * @param leftMin
        * @param rightMin
        * @return a hyphenation object
        */
        public static Hyphenation Hyphenate(String lang, String country,
                                            String word, int leftMin,
                                            int rightMin) {
            HyphenationTree hTree = GetHyphenationTree(lang, country);
            if (hTree == null) {
                //log.Error("Error building hyphenation tree for language "
                //                       + lang);
                return null;
            }
            return hTree.Hyphenate(word, leftMin, rightMin);
        }

        /**
        * @param lang
        * @param country
        * @param word
        * @param offset
        * @param len
        * @param leftMin
        * @param rightMin
        * @return a hyphenation object
        */
        public static Hyphenation Hyphenate(String lang, String country,
                                            char[] word, int offset, int len,
                                            int leftMin, int rightMin) {
            HyphenationTree hTree = GetHyphenationTree(lang, country);
            if (hTree == null) {
                //log.Error("Error building hyphenation tree for language "
                //                       + lang);
                return null;
            }
            return hTree.Hyphenate(word, offset, len, leftMin, rightMin);
        }

        /**
        * @param min
        */
        public void SetMinRemainCharCount(int min) {
            remainCharCount = min;
        }

        /**
        * @param min
        */
        public void SetMinPushCharCount(int min) {
            pushCharCount = min;
        }

        /**
        * @param lang
        * @param country
        */
        public void SetLanguage(String lang, String country) {
            hyphenTree = GetHyphenationTree(lang, country);
        }

        /**
        * @param word
        * @param offset
        * @param len
        * @return a hyphenation object
        */
        public Hyphenation Hyphenate(char[] word, int offset, int len) {
            if (hyphenTree == null) {
                return null;
            }
            return hyphenTree.Hyphenate(word, offset, len, remainCharCount,
                                        pushCharCount);
        }

        /**
        * @param word
        * @return a hyphenation object
        */
        public Hyphenation Hyphenate(String word) {
            if (hyphenTree == null) {
                return null;
            }
            return hyphenTree.Hyphenate(word, remainCharCount, pushCharCount);
        }
    }
}
