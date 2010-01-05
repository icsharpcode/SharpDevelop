using System;
using System.Collections;
using System.IO;

/*
 * $Id: PdfContentParser.cs,v 1.4 2006/09/17 15:55:03 psoares33 Exp $
 *
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

namespace iTextSharp.text.pdf {
    /**
    * Parses the page or template content.
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class PdfContentParser {
        
        /**
        * Commands have this type.
        */    
        public const int COMMAND_TYPE = 200;
        /**
        * Holds value of property tokeniser.
        */
        private PRTokeniser tokeniser;    
        
        /**
        * Creates a new instance of PdfContentParser
        * @param tokeniser the tokeniser with the content
        */
        public PdfContentParser(PRTokeniser tokeniser) {
            this.tokeniser = tokeniser;
        }
        
        /**
        * Parses a single command from the content. Each command is output as an array of arguments
        * having the command itself as the last element. The returned array will be empty if the
        * end of content was reached.
        * @param ls an <CODE>ArrayList</CODE> to use. It will be cleared before using. If it's
        * <CODE>null</CODE> will create a new <CODE>ArrayList</CODE>
        * @return the same <CODE>ArrayList</CODE> given as argument or a new one
        * @throws IOException on error
        */    
        public ArrayList Parse(ArrayList ls) {
            if (ls == null)
                ls = new ArrayList();
            else
                ls.Clear();
            PdfObject ob = null;
            while ((ob = ReadPRObject()) != null) {
                ls.Add(ob);
                if (ob.Type == COMMAND_TYPE)
                    break;
            }
            return ls;
        }
        
        /**
        * Gets the tokeniser.
        * @return the tokeniser.
        */
        public PRTokeniser GetTokeniser() {
            return this.tokeniser;
        }
        
        /**
        * Sets the tokeniser.
        * @param tokeniser the tokeniser
        */
        public PRTokeniser Tokeniser {
            set {
                tokeniser = value;
            }
            get {
                return tokeniser;
            }
        }
        
        /**
        * Reads a dictionary. The tokeniser must be positioned past the "&lt;&lt;" token.
        * @return the dictionary
        * @throws IOException on error
        */    
        public PdfDictionary ReadDictionary() {
            PdfDictionary dic = new PdfDictionary();
            while (true) {
                if (!NextValidToken())
                    throw new IOException("Unexpected end of file.");
                    if (tokeniser.TokenType == PRTokeniser.TK_END_DIC)
                        break;
                    if (tokeniser.TokenType != PRTokeniser.TK_NAME)
                        throw new IOException("Dictionary key is not a name.");
                    PdfName name = new PdfName(tokeniser.StringValue, false);
                    PdfObject obj = ReadPRObject();
                    int type = obj.Type;
                    if (-type == PRTokeniser.TK_END_DIC)
                        throw new IOException("Unexpected '>>'");
                    if (-type == PRTokeniser.TK_END_ARRAY)
                        throw new IOException("Unexpected ']'");
                    dic.Put(name, obj);
            }
            return dic;
        }
        
        /**
        * Reads an array. The tokeniser must be positioned past the "[" token.
        * @return an array
        * @throws IOException on error
        */    
        public PdfArray ReadArray() {
            PdfArray array = new PdfArray();
            while (true) {
                PdfObject obj = ReadPRObject();
                int type = obj.Type;
                if (-type == PRTokeniser.TK_END_ARRAY)
                    break;
                if (-type == PRTokeniser.TK_END_DIC)
                    throw new IOException("Unexpected '>>'");
                array.Add(obj);
            }
            return array;
        }
        
        /**
        * Reads a pdf object.
        * @return the pdf object
        * @throws IOException on error
        */    
        public PdfObject ReadPRObject() {
            if (!NextValidToken())
                return null;
            int type = tokeniser.TokenType;
            switch (type) {
                case PRTokeniser.TK_START_DIC: {
                    PdfDictionary dic = ReadDictionary();
                    return dic;
                }
                case PRTokeniser.TK_START_ARRAY:
                    return ReadArray();
                case PRTokeniser.TK_STRING:
                    PdfString str = new PdfString(tokeniser.StringValue, null).SetHexWriting(tokeniser.IsHexString());
                    return str;
                case PRTokeniser.TK_NAME:
                    return new PdfName(tokeniser.StringValue, false);
                case PRTokeniser.TK_NUMBER:
                    return new PdfNumber(tokeniser.StringValue);
                 case PRTokeniser.TK_OTHER:
                    return new PdfLiteral(COMMAND_TYPE, tokeniser.StringValue);
                default:
                    return new PdfLiteral(-type, tokeniser.StringValue);
            }
        }
        
        /**
        * Reads the next token skipping over the comments.
        * @return <CODE>true</CODE> if a token was read, <CODE>false</CODE> if the end of content was reached
        * @throws IOException on error
        */    
        public bool NextValidToken() {
            while (tokeniser.NextToken()) {
                if (tokeniser.TokenType == PRTokeniser.TK_COMMENT)
                    continue;
                return true;
            }
            return false;
        }
    }
}