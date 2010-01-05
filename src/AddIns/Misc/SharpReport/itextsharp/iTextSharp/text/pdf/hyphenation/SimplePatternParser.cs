using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util;
using iTextSharp.text.xml.simpleparser;
/*
 * Copyright 2005 by Paulo Soares.
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.pdf.hyphenation {
    /** Parses the xml hyphenation pattern.
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class SimplePatternParser : ISimpleXMLDocHandler {
        internal int currElement;
        internal IPatternConsumer consumer;
        internal StringBuilder token;
        internal ArrayList exception;
        internal char hyphenChar;
        
        internal const int ELEM_CLASSES = 1;
        internal const int ELEM_EXCEPTIONS = 2;
        internal const int ELEM_PATTERNS = 3;
        internal const int ELEM_HYPHEN = 4;

        /** Creates a new instance of PatternParser2 */
        public SimplePatternParser() {
            token = new StringBuilder();
            hyphenChar = '-';    // default
        }
        
        public void Parse(Stream stream, IPatternConsumer consumer) {
            this.consumer = consumer;
            try {
                SimpleXMLParser.Parse(this, stream);
            }
            finally {
                try{stream.Close();}catch{}
            }
        }
        
        protected static String GetPattern(String word) {
            StringBuilder pat = new StringBuilder();
            int len = word.Length;
            for (int i = 0; i < len; i++) {
                if (!char.IsDigit(word[i])) {
                    pat.Append(word[i]);
                }
            }
            return pat.ToString();
        }

        protected ArrayList NormalizeException(ArrayList ex) {
            ArrayList res = new ArrayList();
            for (int i = 0; i < ex.Count; i++) {
                Object item = ex[i];
                if (item is String) {
                    String str = (String)item;
                    StringBuilder buf = new StringBuilder();
                    for (int j = 0; j < str.Length; j++) {
                        char c = str[j];
                        if (c != hyphenChar) {
                            buf.Append(c);
                        } else {
                            res.Add(buf.ToString());
                            buf.Length = 0;
                            char[] h = new char[1];
                            h[0] = hyphenChar;
                            // we use here hyphenChar which is not necessarily
                            // the one to be printed
                            res.Add(new Hyphen(new String(h), null, null));
                        }
                    }
                    if (buf.Length > 0) {
                        res.Add(buf.ToString());
                    }
                } else {
                    res.Add(item);
                }
            }
            return res;
        }

        protected String GetExceptionWord(ArrayList ex) {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < ex.Count; i++) {
                Object item = ex[i];
                if (item is String) {
                    res.Append((String)item);
                } else {
                    if (((Hyphen)item).noBreak != null) {
                        res.Append(((Hyphen)item).noBreak);
                    }
                }
            }
            return res.ToString();
        }

        protected static String GetInterletterValues(String pat) {
            StringBuilder il = new StringBuilder();
            String word = pat + "a";    // add dummy letter to serve as sentinel
            int len = word.Length;
            for (int i = 0; i < len; i++) {
                char c = word[i];
                if (char.IsDigit(c)) {
                    il.Append(c);
                    i++;
                } else {
                    il.Append('0');
                }
            }
            return il.ToString();
        }

        public void EndDocument() {
        }
        
        public void EndElement(String tag) {
            if (token.Length > 0) {
                String word = token.ToString();
                switch (currElement) {
                case ELEM_CLASSES:
                    consumer.AddClass(word);
                    break;
                case ELEM_EXCEPTIONS:
                    exception.Add(word);
                    exception = NormalizeException(exception);
                    consumer.AddException(GetExceptionWord(exception),
                                        (ArrayList)exception.Clone());
                    break;
                case ELEM_PATTERNS:
                    consumer.AddPattern(GetPattern(word),
                                        GetInterletterValues(word));
                    break;
                case ELEM_HYPHEN:
                    // nothing to do
                    break;
                }
                if (currElement != ELEM_HYPHEN) {
                    token.Length = 0;
                }
            }
            if (currElement == ELEM_HYPHEN) {
                currElement = ELEM_EXCEPTIONS;
            } else {
                currElement = 0;
            }
        }
        
        public void StartDocument() {
        }
        
        public void StartElement(String tag, Hashtable h) {
            if (tag.Equals("hyphen-char")) {
                String hh = (String)h["value"];
                if (hh != null && hh.Length == 1) {
                    hyphenChar = hh[0];
                }
            } else if (tag.Equals("classes")) {
                currElement = ELEM_CLASSES;
            } else if (tag.Equals("patterns")) {
                currElement = ELEM_PATTERNS;
            } else if (tag.Equals("exceptions")) {
                currElement = ELEM_EXCEPTIONS;
                exception = new ArrayList();
            } else if (tag.Equals("hyphen")) {
                if (token.Length > 0) {
                    exception.Add(token.ToString());
                }
                exception.Add(new Hyphen((String)h["pre"],
                                                (String)h["no"],
                                                (String)h["post"]));
                currElement = ELEM_HYPHEN;
            }
            token.Length = 0;
        }
        
        public void Text(String str) {
            StringTokenizer tk = new StringTokenizer(str);
            while (tk.HasMoreTokens()) {
                String word = tk.NextToken();
                // System.out.Println("\"" + word + "\"");
                switch (currElement) {
                case ELEM_CLASSES:
                    consumer.AddClass(word);
                    break;
                case ELEM_EXCEPTIONS:
                    exception.Add(word);
                    exception = NormalizeException(exception);
                    consumer.AddException(GetExceptionWord(exception),
                                        (ArrayList)exception.Clone());
                    exception.Clear();
                    break;
                case ELEM_PATTERNS:
                    consumer.AddPattern(GetPattern(word),
                                        GetInterletterValues(word));
                    break;
                }
            }
        }
    }
}
