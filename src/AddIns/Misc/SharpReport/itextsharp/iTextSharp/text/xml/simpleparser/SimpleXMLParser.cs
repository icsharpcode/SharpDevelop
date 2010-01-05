using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
/*
 * Copyright 2003 Paulo Soares
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
 *
 * The code to recognize the encoding in this class and in the convenience class IanaEncodings was taken from Apache Xerces published under the following license:
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Part of this code is based on the Quick-and-Dirty XML parser by Steven Brandt.
 * The code for the Quick-and-Dirty parser was published in JavaWorld (java tip 128).
 * Steven Brandt and JavaWorld gave permission to use the code for free.
 * (Bruno Lowagie and Paulo Soares chose to use it under the MPL/LGPL in
 * conformance with the rest of the code).
 * The original code can be found on this url: <A HREF="http://www.javaworld.com/javatips/jw-javatip128_p.html">http://www.javaworld.com/javatips/jw-javatip128_p.html</A>.
 * It was substantially refactored by Bruno Lowagie.
 * 
 * The method 'private static String getEncodingName(byte[] b4)' was found
 * in org.apache.xerces.impl.XMLEntityManager, originaly published by the
 * Apache Software Foundation under the Apache Software License; now being
 * used in iText under the MPL.
 */

namespace iTextSharp.text.xml.simpleparser {
    /**
    * A simple XML and HTML parser.  This parser is, like the SAX parser,
    * an event based parser, but with much less functionality.
    * <p>
    * The parser can:
    * <p>
    * <ul>
    * <li>It recognizes the encoding used
    * <li>It recognizes all the elements' start tags and end tags
    * <li>It lists attributes, where attribute values can be enclosed in single or double quotes
    * <li>It recognizes the <code>&lt;[CDATA[ ... ]]&gt;</code> construct
    * <li>It recognizes the standard entities: &amp;amp;, &amp;lt;, &amp;gt;, &amp;quot;, and &amp;apos;, as well as numeric entities
    * <li>It maps lines ending in <code>\r\n</code> and <code>\r</code> to <code>\n</code> on input, in accordance with the XML Specification, Section 2.11
    * </ul>
    * <p>
    * The code is based on <A HREF="http://www.javaworld.com/javaworld/javatips/javatip128/">
    * http://www.javaworld.com/javaworld/javatips/javatip128/</A> with some extra
    * code from XERCES to recognize the encoding.
    */
    public sealed class SimpleXMLParser {
        /** possible states */
        private const int UNKNOWN = 0;
        private const int TEXT = 1;
        private const int TAG_ENCOUNTERED = 2;
        private const int EXAMIN_TAG = 3;
        private const int TAG_EXAMINED = 4;
        private const int IN_CLOSETAG = 5;
        private const int SINGLE_TAG = 6;
        private const int CDATA = 7;
        private const int COMMENT = 8;
        private const int PI = 9;
        private const int ENTITY = 10;
        private const int QUOTE = 11;
        private const int ATTRIBUTE_KEY = 12;
        private const int ATTRIBUTE_EQUAL = 13;
        private const int ATTRIBUTE_VALUE = 14;
        
        /** the state stack */
        internal Stack stack;
        /** The current character. */
        internal int character = 0;
        /** The previous character. */
        internal int previousCharacter = -1;
        /** the line we are currently reading */
        internal int lines = 1;
        /** the column where the current character occurs */
        internal int columns = 0;
        /** was the last character equivalent to a newline? */
        internal bool eol = false;
        /** the current state */
        internal int state;
        /** Are we parsing HTML? */
        internal bool html;
        /** current text (whatever is encountered between tags) */
        internal StringBuilder text = new StringBuilder();
        /** current entity (whatever is encountered between & and ;) */
        internal StringBuilder entity = new StringBuilder();
        /** current tagname */
        internal String tag = null;
        /** current attributes */
        internal Hashtable attributes = null;
        /** The handler to which we are going to forward document content */
        internal ISimpleXMLDocHandler doc;
        /** The handler to which we are going to forward comments. */
        internal ISimpleXMLDocHandlerComment comment;
        /** Keeps track of the number of tags that are open. */
        internal int nested = 0;
        /** the quote character that was used to open the quote. */
        internal int quoteCharacter = '"';
        /** the attribute key. */
        internal String attributekey = null;
        /** the attribute value. */
        internal String attributevalue = null;
        
        /**
        * Creates a Simple XML parser object.
        * Call Go(BufferedReader) immediately after creation.
        */
        private SimpleXMLParser(ISimpleXMLDocHandler doc, ISimpleXMLDocHandlerComment comment, bool html) {
            this.doc = doc;
            this.comment = comment;
            this.html = html;
            stack = new Stack();
            state = html ? TEXT : UNKNOWN;
        }
        
        /**
        * Does the actual parsing. Perform this immediately
        * after creating the parser object.
        */
        private void Go(TextReader reader) {
            doc.StartDocument();
            while (true) {
                // read a new character
                if (previousCharacter == -1) {
                    character = reader.Read();
                }
                // or re-examin the previous character
                else {
                    character = previousCharacter;
                    previousCharacter = -1;
                }
                
                // the end of the file was reached
                if (character == -1) {
                    if (html) {
                        if (html && state == TEXT)
                            Flush();
                        doc.EndDocument();
                    } else {
                        ThrowException("Missing end tag");
                    }
                    return;
                }
                
                // dealing with  \n and \r
                if (character == '\n' && eol) {
                    eol = false;
                    continue;
                } else if (eol) {
                    eol = false;
                } else if (character == '\n') {
                    lines++;
                    columns = 0;
                } else if (character == '\r') {
                    eol = true;
                    character = '\n';
                    lines++;
                    columns = 0;
                } else {
                    columns++;
                }
                
                switch (state) {
                // we are in an unknown state before there's actual content
                case UNKNOWN:
                    if (character == '<') {
                        SaveState(TEXT);
                        state = TAG_ENCOUNTERED;
                    }
                    break;
                // we can encounter any content
                case TEXT:
                    if (character == '<') {
                        Flush();
                        SaveState(state);
                        state = TAG_ENCOUNTERED;
                    } else if (character == '&') {
                        SaveState(state);
                        entity.Length = 0;
                        state = ENTITY;
                    } else
                        text.Append((char)character);
                    break;
                // we have just seen a < and are wondering what we are looking at
                // <foo>, </foo>, <!-- ... --->, etc.
                case TAG_ENCOUNTERED:
                    InitTag();
                    if (character == '/') {
                        state = IN_CLOSETAG;
                    } else if (character == '?') {
                        RestoreState();
                        state = PI;
                    } else {
                        text.Append((char)character);
                        state = EXAMIN_TAG;
                    }
                    break;
                // we are processing something like this <foo ... >.
                // It could still be a <!-- ... --> or something.
                case EXAMIN_TAG:
                    if (character == '>') {
                        DoTag();
                        ProcessTag(true);
                        InitTag();
                        state = RestoreState();
                    } else if (character == '/') {
                        state = SINGLE_TAG;
                    } else if (character == '-' && text.ToString().Equals("!-")) {
                        Flush();
                        state = COMMENT;
                    } else if (character == '[' && text.ToString().Equals("![CDATA")) {
                        Flush();
                        state = CDATA;
                    } else if (character == 'E' && text.ToString().Equals("!DOCTYP")) {
                        Flush();
                        state = PI;
                    } else if (char.IsWhiteSpace((char)character)) {
                        DoTag();
                        state = TAG_EXAMINED;
                    } else {
                        text.Append((char)character);
                    }
                    break;
                // we know the name of the tag now.
                case TAG_EXAMINED:
                    if (character == '>') {
                        ProcessTag(true);
                        InitTag();
                        state = RestoreState();
                    } else if (character == '/') {
                        state = SINGLE_TAG;
                    } else if (char.IsWhiteSpace((char)character)) {
                        // empty
                    } else {
                        text.Append((char)character);
                        state = ATTRIBUTE_KEY;
                    }
                    break;
                    
                    // we are processing a closing tag: e.g. </foo>
                case IN_CLOSETAG:
                    if (character == '>') {
                        DoTag();
                        ProcessTag(false);
                        if (!html && nested==0) return;
                        state = RestoreState();
                    } else {
                        if (!char.IsWhiteSpace((char)character))
                            text.Append((char)character);
                    }
                    break;
                    
                // we have just seen something like this: <foo a="b"/
                // and are looking for the final >.
                case SINGLE_TAG:
                    if (character != '>')
                        ThrowException("Expected > for tag: <"+tag+"/>");
                    DoTag();
                    ProcessTag(true);
                    ProcessTag(false);
                    InitTag();
                    if (!html && nested==0) {
                        doc.EndDocument();
                        return;
                    }
                    state = RestoreState();
                    break;
                    
                // we are processing CDATA
                case CDATA:
                    if (character == '>'
                    && text.ToString().EndsWith("]]")) {
                        text.Length = text.Length - 2;
                        Flush();
                        state = RestoreState();
                    } else
                        text.Append((char)character);
                    break;
                    
                // we are processing a comment.  We are inside
                // the <!-- .... --> looking for the -->.
                case COMMENT:
                    if (character == '>'
                    && text.ToString().EndsWith("--")) {
                        text.Length = text.Length - 2;
                        Flush();
                        state = RestoreState();
                    } else
                        text.Append((char)character);
                    break;
                    
                // We are inside one of these <? ... ?> or one of these <!DOCTYPE ... >
                case PI:
                    if (character == '>') {
                        state = RestoreState();
                        if (state == TEXT) state = UNKNOWN;
                    }
                    break;
                    
                // we are processing an entity, e.g. &lt;, &#187;, etc.
                case ENTITY:
                    if (character == ';') {
                        state = RestoreState();
                        String cent = entity.ToString();
                        entity.Length = 0;
                        char ce = EntitiesToUnicode.DecodeEntity(cent);
                        if (ce == '\0')
                            text.Append('&').Append(cent).Append(';');
                        else
                            text.Append(ce);
                    } else if ((character != '#' && (character < '0' || character > '9') && (character < 'a' || character > 'z')
                        && (character < 'A' || character > 'Z')) || entity.Length >= 7) {
                        state = RestoreState();
                        previousCharacter = character;
                        text.Append('&').Append(entity.ToString());
                        entity.Length = 0;
                    }
                    else {
                        entity.Append((char)character);
                    }
                    break;
                // We are processing the quoted right-hand side of an element's attribute.
                case QUOTE:
                    if (html && quoteCharacter == ' ' && character == '>') {
                        Flush();
                        ProcessTag(true);
                        InitTag();
                        state = RestoreState();
                    }
                    else if (html && quoteCharacter == ' ' && char.IsWhiteSpace((char)character)) {
                        Flush();
                        state = TAG_EXAMINED;
                    }
                    else if (html && quoteCharacter == ' ') {
                        text.Append((char)character);
                    }
                    else if (character == quoteCharacter) {
                        Flush();
                        state = TAG_EXAMINED;
                    } else if (" \r\n\u0009".IndexOf((char)character)>=0) {
                        text.Append(' ');
                    } else if (character == '&') {
                        SaveState(state);
                        state = ENTITY;
                        entity.Length = 0;
                    } else {
                        text.Append((char)character);
                    }
                    break;
                    
                case ATTRIBUTE_KEY:
                    if (char.IsWhiteSpace((char)character)) {
                        Flush();
                        state = ATTRIBUTE_EQUAL;
                    } else if (character == '=') {
                        Flush();
                        state = ATTRIBUTE_VALUE;
                    } else if (html && character == '>') {
                        text.Length = 0;
                        ProcessTag(true);
                        InitTag();
                        state = RestoreState();
                    } else {
                        text.Append((char)character);
                    }
                    break;
                    
                case ATTRIBUTE_EQUAL:
                    if (character == '=') {
                        state = ATTRIBUTE_VALUE;
                    } else if (char.IsWhiteSpace((char)character)) {
                        // empty
                    } else if (html && character == '>') {
                        text.Length = 0;
                        ProcessTag(true);
                        InitTag();
                        state = RestoreState();
                    } else if (html && character == '/') {
                        Flush();
                        state = SINGLE_TAG;
                    } else if (html) {
                        Flush();
                        text.Append((char)character);
                        state = ATTRIBUTE_KEY;
                    } else {
                        ThrowException("Error in attribute processing.");
                    }
                    break;
                    
                case ATTRIBUTE_VALUE:
                    if (character == '"' || character == '\'') {
                        quoteCharacter = character;
                        state = QUOTE;
                    } else if (char.IsWhiteSpace((char)character)) {
                        // empty
                    } else if (html && character == '>') {
                        Flush();
                        ProcessTag(true);
                        InitTag();
                        state = RestoreState();
                    } else if (html) {
                        text.Append((char)character);
                        quoteCharacter = ' ';
                        state = QUOTE;
                    } else {
                        ThrowException("Error in attribute processing");
                    }
                    break;
                }
            }
        }

        /**
        * Gets a state from the stack
        * @return the previous state
        */
        private int RestoreState() {
            if (stack.Count != 0)
                return (int)stack.Pop();
            else
                return UNKNOWN;
        }
        /**
        * Adds a state to the stack.
        * @param   s   a state to add to the stack
        */
        private void SaveState(int s) {
            stack.Push(s);
        }
        /**
        * Flushes the text that is currently in the buffer.
        * The text can be ignored, added to the document
        * as content or as comment,... depending on the current state.
        */
        private void Flush() {
            switch (state){
            case TEXT:
            case CDATA:
                if (text.Length > 0) {
                    doc.Text(text.ToString());
                }
                break;
            case COMMENT:
                if (comment != null) {
                    comment.Comment(text.ToString());
                }
                break;
            case ATTRIBUTE_KEY:
                attributekey = text.ToString();
                if (html)
                    attributekey = attributekey.ToLower(CultureInfo.InvariantCulture);
                break;
            case QUOTE:
            case ATTRIBUTE_VALUE:
                attributevalue = text.ToString();
                attributes[attributekey] = attributevalue;
                break;
            default:
                // do nothing
                break;
            }
            text.Length = 0;
        }
        /**
        * Initialized the tag name and attributes.
        */
        private void InitTag() {
            tag = null;
            attributes = new Hashtable();
        }
        /** Sets the name of the tag. */
        private void DoTag() {
            if (tag == null)
                tag = text.ToString();
            if (html)
                tag = tag.ToLower(CultureInfo.InvariantCulture);
            text.Length = 0;
        }
        /**
        * processes the tag.
        * @param start if true we are dealing with a tag that has just been opened; if false we are closing a tag.
        */
        private void ProcessTag(bool start) {
            if (start) {
                nested++;
                doc.StartElement(tag,attributes);
            }
            else {
                nested--;
                doc.EndElement(tag);
            }
        }
        /** Throws an exception */
        private void ThrowException(String s) {
            throw new IOException(s+" near line " + lines + ", column " + columns);
        }
        
        /**
        * Parses the XML document firing the events to the handler.
        * @param doc the document handler
        * @param r the document. The encoding is already resolved. The reader is not closed
        * @throws IOException on error
        */
        public static void Parse(ISimpleXMLDocHandler doc, ISimpleXMLDocHandlerComment comment, TextReader r, bool html) {
            SimpleXMLParser parser = new SimpleXMLParser(doc, comment, html);
            parser.Go(r);
        }
        
        /**
        * Parses the XML document firing the events to the handler.
        * @param doc the document handler
        * @param in the document. The encoding is deduced from the stream. The stream is not closed
        * @throws IOException on error
        */    
        public static void Parse(ISimpleXMLDocHandler doc, Stream inp) {
            byte[] b4 = new byte[4];
            int count = inp.Read(b4, 0, b4.Length);
            if (count != 4)
                throw new IOException("Insufficient length.");
            String encoding = GetEncodingName(b4);
            String decl = null;
            if (encoding.Equals("UTF-8")) {
                StringBuilder sb = new StringBuilder();
                int c;
                while ((c = inp.ReadByte()) != -1) {
                    if (c == '>')
                        break;
                    sb.Append((char)c);
                }
                decl = sb.ToString();
            }
            else if (encoding.Equals("CP037")) {
                MemoryStream bi = new MemoryStream();
                int c;
                while ((c = inp.ReadByte()) != -1) {
                    if (c == 0x6e) // that's '>' in ebcdic
                        break;
                    bi.WriteByte((byte)c);
                }
                decl = Encoding.GetEncoding(37).GetString(bi.ToArray());//cp037 ebcdic
            }
            if (decl != null) {
                decl = GetDeclaredEncoding(decl);
                if (decl != null)
                    encoding = decl;
            }
            Parse(doc, new StreamReader(inp, IanaEncodings.GetEncodingEncoding(encoding)));
        }
        
        private static String GetDeclaredEncoding(String decl) {
            if (decl == null)
                return null;
            int idx = decl.IndexOf("encoding");
            if (idx < 0)
                return null;
            int idx1 = decl.IndexOf('"', idx);
            int idx2 = decl.IndexOf('\'', idx);
            if (idx1 == idx2)
                return null;
            if ((idx1 < 0 && idx2 > 0) || (idx2 > 0 && idx2 < idx1)) {
                int idx3 = decl.IndexOf('\'', idx2 + 1);
                if (idx3 < 0)
                    return null;
                return decl.Substring(idx2 + 1, idx3 - (idx2 + 1));
            }
            if ((idx2 < 0 && idx1 > 0) || (idx1 > 0 && idx1 < idx2)) {
                int idx3 = decl.IndexOf('"', idx1 + 1);
                if (idx3 < 0)
                    return null;
                return decl.Substring(idx1 + 1, idx3 - (idx1 + 1));
            }
            return null;
        }
        
        public static void Parse(ISimpleXMLDocHandler doc, TextReader r) {
            Parse(doc, null, r, false);
        }
        
        /**
        * Escapes a string with the appropriated XML codes.
        * @param s the string to be escaped
        * @param onlyASCII codes above 127 will always be escaped with &amp;#nn; if <CODE>true</CODE>
        * @return the escaped string
        */    
        public static String EscapeXML(String s, bool onlyASCII) {
            char[] cc = s.ToCharArray();
            int len = cc.Length;
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < len; ++k) {
                int c = cc[k];
                switch (c) {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '\'':
                        sb.Append("&apos;");
                        break;
                    default:
                        if (onlyASCII && c > 127)
                            sb.Append("&#").Append(c).Append(';');
                        else
                            sb.Append((char)c);
                        break;
                }
            }
            return sb.ToString();
        }
        
        /**
        * Returns the IANA encoding name that is auto-detected from
        * the bytes specified, with the endian-ness of that encoding where appropriate.
        * (method found in org.apache.xerces.impl.XMLEntityManager, originaly published
        * by the Apache Software Foundation under the Apache Software License; now being
        * used in iText under the MPL)
        * @param b4    The first four bytes of the input.
        * @return an IANA-encoding string
        */
        private static String GetEncodingName(byte[] b4) {
            // UTF-16, with BOM
            int b0 = b4[0] & 0xFF;
            int b1 = b4[1] & 0xFF;
            if (b0 == 0xFE && b1 == 0xFF) {
                // UTF-16, big-endian
                return "UTF-16BE";
            }
            if (b0 == 0xFF && b1 == 0xFE) {
                // UTF-16, little-endian
                return "UTF-16LE";
            }
            
            // UTF-8 with a BOM
            int b2 = b4[2] & 0xFF;
            if (b0 == 0xEF && b1 == 0xBB && b2 == 0xBF) {
                return "UTF-8";
            }
            
            // other encodings
            int b3 = b4[3] & 0xFF;
            if (b0 == 0x00 && b1 == 0x00 && b2 == 0x00 && b3 == 0x3C) {
                // UCS-4, big endian (1234)
                return "ISO-10646-UCS-4";
            }
            if (b0 == 0x3C && b1 == 0x00 && b2 == 0x00 && b3 == 0x00) {
                // UCS-4, little endian (4321)
                return "ISO-10646-UCS-4";
            }
            if (b0 == 0x00 && b1 == 0x00 && b2 == 0x3C && b3 == 0x00) {
                // UCS-4, unusual octet order (2143)
                // REVISIT: What should this be?
                return "ISO-10646-UCS-4";
            }
            if (b0 == 0x00 && b1 == 0x3C && b2 == 0x00 && b3 == 0x00) {
                // UCS-4, unusual octect order (3412)
                // REVISIT: What should this be?
                return "ISO-10646-UCS-4";
            }
            if (b0 == 0x00 && b1 == 0x3C && b2 == 0x00 && b3 == 0x3F) {
                // UTF-16, big-endian, no BOM
                // (or could turn out to be UCS-2...
                // REVISIT: What should this be?
                return "UTF-16BE";
            }
            if (b0 == 0x3C && b1 == 0x00 && b2 == 0x3F && b3 == 0x00) {
                // UTF-16, little-endian, no BOM
                // (or could turn out to be UCS-2...
                return "UTF-16LE";
            }
            if (b0 == 0x4C && b1 == 0x6F && b2 == 0xA7 && b3 == 0x94) {
                // EBCDIC
                // a la xerces1, return CP037 instead of EBCDIC here
                return "CP037";
            }
            
            // default encoding
            return "UTF-8";
        }
    }
}
