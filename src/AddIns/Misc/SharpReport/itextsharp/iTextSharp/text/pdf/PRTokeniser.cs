using System;
using System.IO;
using System.Text;

/*
 * Copyright 2001, 2002 by Paulo Soares.
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
     *
     * @author  Paulo Soares (psoares@consiste.pt)
     */
    public class PRTokeniser {
    
        public const int TK_NUMBER = 1;
        public const int TK_STRING = 2;
        public const int TK_NAME = 3;
        public const int TK_COMMENT = 4;
        public const int TK_START_ARRAY = 5;
        public const int TK_END_ARRAY = 6;
        public const int TK_START_DIC = 7;
        public const int TK_END_DIC = 8;
        public const int TK_REF = 9;
        public const int TK_OTHER = 10;
    
        internal const string EMPTY = "";

    
        protected RandomAccessFileOrArray file;
        protected int type;
        protected string stringValue;
        protected int reference;
        protected int generation;
        protected bool hexString;
        
        public PRTokeniser(string filename) {
            file = new RandomAccessFileOrArray(filename);
        }

        public PRTokeniser(byte[] pdfIn) {
            file = new RandomAccessFileOrArray(pdfIn);
        }
    
        public PRTokeniser(RandomAccessFileOrArray file) {
            this.file = file;
        }

        public void Seek(int pos) {
            file.Seek(pos);
        }
    
        public int FilePointer {
            get {
                return file.FilePointer;
            }
        }

        public void Close() {
            file.Close();
        }
    
        public int Length {
            get {
                return file.Length;
            }
        }

        public int Read() {
            return file.Read();
        }
    
        public RandomAccessFileOrArray SafeFile {
            get {
                return new RandomAccessFileOrArray(file);
            }
        }
    
        public RandomAccessFileOrArray File {
            get {
                return file;
            }
        }

        public string ReadString(int size) {
            StringBuilder buf = new StringBuilder();
            int ch;
            while ((size--) > 0) {
                ch = file.Read();
                if (ch == -1)
                    break;
                buf.Append((char)ch);
            }
            return buf.ToString();
        }

        public static bool IsWhitespace(int ch) {
            return (ch == 0 || ch == 9 || ch == 10 || ch == 12 || ch == 13 || ch == 32);
        }
    
        public static bool IsDelimiter(int ch) {
            return (ch == '(' || ch == ')' || ch == '<' || ch == '>' || ch == '[' || ch == ']' || ch == '/' || ch == '%');
        }

        public int TokenType {
            get {
                return type;
            }
        }
    
        public string StringValue {
            get {
                return stringValue;
            }
        }
    
        public int Reference {
            get {
                return reference;
            }
        }
    
        public int Generation {
            get {
                return generation;
            }
        }
    
        public void BackOnePosition(int ch) {
            if (ch != -1)
                file.PushBack((byte)ch);
        }
    
        public void ThrowError(string error) {
            throw new IOException(error + " at file pointer " + file.FilePointer);
        }
    
        public char CheckPdfHeader() {
            file.StartOffset = 0;
            String str = ReadString(1024);
            int idx = str.IndexOf("%PDF-");
            if (idx < 0)
                throw new IOException("PDF header signature not found.");
            file.StartOffset = idx;
            return str[idx + 7];
        }
        
        public void CheckFdfHeader() {
            file.StartOffset = 0;
            String str = ReadString(1024);
            int idx = str.IndexOf("%FDF-1.2");
            if (idx < 0)
                throw new IOException("FDF header signature not found.");
            file.StartOffset = idx;
        }

        public int Startxref {
            get {
                int size = Math.Min(1024, file.Length);
                int pos = file.Length - size;
                file.Seek(pos);
                string str = ReadString(1024);
                int idx = str.LastIndexOf("startxref");
                if (idx < 0)
                    throw new IOException("PDF startxref not found.");
                return pos + idx;
            }
        }

        public static int GetHex(int v) {
            if (v >= '0' && v <= '9')
                return v - '0';
            if (v >= 'A' && v <= 'F')
                return v - 'A' + 10;
            if (v >= 'a' && v <= 'f')
                return v - 'a' + 10;
            return -1;
        }
    
        public void NextValidToken() {
            int level = 0;
            string n1 = null;
            string n2 = null;
            int ptr = 0;
            while (NextToken()) {
                if (type == TK_COMMENT)
                    continue;
                switch (level) {
                    case 0: {
                        if (type != TK_NUMBER)
                            return;
                        ptr = file.FilePointer;
                        n1 = stringValue;
                        ++level;
                        break;
                    }
                    case 1: {
                        if (type != TK_NUMBER) {
                            file.Seek(ptr);
                            type = TK_NUMBER;
                            stringValue = n1;
                            return;
                        }
                        n2 = stringValue;
                        ++level;
                        break;
                    }
                    default: {
                        if (type != TK_OTHER || !stringValue.Equals("R")) {
                            file.Seek(ptr);
                            type = TK_NUMBER;
                            stringValue = n1;
                            return;
                        }
                        type = TK_REF;
                        reference = int.Parse(n1);
                        generation = int.Parse(n2);
                        return;
                    }
                }
            }
            ThrowError("Unexpected end of file");
        }
    
        public bool NextToken() {
            StringBuilder outBuf = null;
            stringValue = EMPTY;
            int ch = 0;
            do {
                ch = file.Read();
            } while (ch != -1 && IsWhitespace(ch));
            if (ch == -1)
                return false;
            switch (ch) {
                case '[':
                    type = TK_START_ARRAY;
                    break;
                case ']':
                    type = TK_END_ARRAY;
                    break;
                case '/': {
                    outBuf = new StringBuilder();
                    type = TK_NAME;
                    while (true) {
                        ch = file.Read();
                        if (ch == -1 || IsDelimiter(ch) || IsWhitespace(ch))
                            break;
                        if (ch == '#') {
                            ch = (GetHex(file.Read()) << 4) + GetHex(file.Read());
                        }
                        outBuf.Append((char)ch);
                    }
                    BackOnePosition(ch);
                    break;
                }
                case '>':
                    ch = file.Read();
                    if (ch != '>')
                        ThrowError("'>' not expected");
                    type = TK_END_DIC;
                    break;
                case '<': {
                    int v1 = file.Read();
                    if (v1 == '<') {
                        type = TK_START_DIC;
                        break;
                    }
                    outBuf = new StringBuilder();
                    type = TK_STRING;
                    hexString = true;
                    int v2 = 0;
                    while (true) {
                        while (IsWhitespace(v1))
                            v1 = file.Read();
                        if (v1 == '>')
                            break;
                        v1 = GetHex(v1);
                        if (v1 < 0)
                            break;
                        v2 = file.Read();
                        while (IsWhitespace(v2))
                            v2 = file.Read();
                        if (v2 == '>') {
                            ch = v1 << 4;
                            outBuf.Append((char)ch);
                            break;
                        }
                        v2 = GetHex(v2);
                        if (v2 < 0)
                            break;
                        ch = (v1 << 4) + v2;
                        outBuf.Append((char)ch);
                        v1 = file.Read();
                    }
                    if (v1 < 0 || v2 < 0)
                        ThrowError("Error reading string");
                    break;
                }
                case '%':
                    type = TK_COMMENT;
                    do {
                        ch = file.Read();
                    } while (ch != -1 && ch != '\r' && ch != '\n');
                    break;
                case '(': {
                    outBuf = new StringBuilder();
                    type = TK_STRING;
                    hexString = false;
                    int nesting = 0;
                    while (true) {
                        ch = file.Read();
                        if (ch == -1)
                            break;
                        if (ch == '(') {
                            ++nesting;
                        }
                        else if (ch == ')') {
                            --nesting;
                        }
                        else if (ch == '\\') {
                            bool lineBreak = false;
                            ch = file.Read();
                            switch (ch) {
                                case 'n':
                                    ch = '\n';
                                    break;
                                case 'r':
                                    ch = '\r';
                                    break;
                                case 't':
                                    ch = '\t';
                                    break;
                                case 'b':
                                    ch = '\b';
                                    break;
                                case 'f':
                                    ch = '\f';
                                    break;
                                case '(':
                                case ')':
                                case '\\':
                                    break;
                                case '\r':
                                    lineBreak = true;
                                    ch = file.Read();
                                    if (ch != '\n')
                                        BackOnePosition(ch);
                                    break;
                                case '\n':
                                    lineBreak = true;
                                    break;
                                default: {
                                    if (ch < '0' || ch > '7') {
                                        break;
                                    }
                                    int octal = ch - '0';
                                    ch = file.Read();
                                    if (ch < '0' || ch > '7') {
                                        BackOnePosition(ch);
                                        ch = octal;
                                        break;
                                    }
                                    octal = (octal << 3) + ch - '0';
                                    ch = file.Read();
                                    if (ch < '0' || ch > '7') {
                                        BackOnePosition(ch);
                                        ch = octal;
                                        break;
                                    }
                                    octal = (octal << 3) + ch - '0';
                                    ch = octal & 0xff;
                                    break;
                                }
                            }
                            if (lineBreak)
                                continue;
                            if (ch < 0)
                                break;
                        }
                        else if (ch == '\r') {
                            ch = file.Read();
                            if (ch < 0)
                                break;
                            if (ch != '\n') {
                                BackOnePosition(ch);
                                ch = '\n';
                            }
                        }
                        if (nesting == -1)
                            break;
                        outBuf.Append((char)ch);
                    }
                    if (ch == -1)
                        ThrowError("Error reading string");
                    break;
                }
                default: {
                    outBuf = new StringBuilder();
                    if (ch == '-' || ch == '+' || ch == '.' || (ch >= '0' && ch <= '9')) {
                        type = TK_NUMBER;
                        do {
                            outBuf.Append((char)ch);
                            ch = file.Read();
                        } while (ch != -1 && ((ch >= '0' && ch <= '9') || ch == '.'));
                    }
                    else {
                        type = TK_OTHER;
                        do {
                            outBuf.Append((char)ch);
                            ch = file.Read();
                        } while (ch != -1 && !IsDelimiter(ch) && !IsWhitespace(ch));
                    }
                    BackOnePosition(ch);
                    break;
                }
            }
            if (outBuf != null)
                stringValue = outBuf.ToString();
            return true;
        }
    
        public int IntValue {
            get {
                return int.Parse(stringValue);
            }
        }

        public bool ReadLineSegment(byte[] input) {
            int c = -1;
            bool eol = false;
            int ptr = 0;
            int len = input.Length;
            // ssteward, pdftk-1.10, 040922: 
            // skip initial whitespace; added this because PdfReader.RebuildXref()
            // assumes that line provided by readLineSegment does not have init. whitespace;
            if ( ptr < len ) {
                while ( IsWhitespace( (c = Read()) ) );
            }
            while ( !eol && ptr < len ) {
                switch (c) {
                    case -1:
                    case '\n':
                        eol = true;
                        break;
                    case '\r':
                        eol = true;
                        int cur = FilePointer;
                        if ((Read()) != '\n') {
                            Seek(cur);
                        }
                        break;
                    default:
                        input[ptr++] = (byte)c;
                        break;
                }

                // break loop? do it before we Read() again
                if ( eol || len <= ptr ) {
                    break;
                }
                else {
                    c = Read();
                }
            }
            if (ptr >= len) {
                eol = false;
                while (!eol) {
                    switch (c = Read()) {
                        case -1:
                        case '\n':
                            eol = true;
                            break;
                        case '\r':
                            eol = true;
                            int cur = FilePointer;
                            if ((Read()) != '\n') {
                                Seek(cur);
                            }
                            break;
                    }
                }
            }
            
            if ((c == -1) && (ptr == 0)) {
                return false;
            }
            if (ptr + 2 <= len) {
                input[ptr++] = (byte)' ';
                input[ptr] = (byte)'X';
            }
            return true;
        }
        
        public static int[] CheckObjectStart(byte[] line) {
            try {
                PRTokeniser tk = new PRTokeniser(line);
                int num = 0;
                int gen = 0;
                if (!tk.NextToken() || tk.TokenType != TK_NUMBER)
                    return null;
                num = tk.IntValue;
                if (!tk.NextToken() || tk.TokenType != TK_NUMBER)
                    return null;
                gen = tk.IntValue;
                if (!tk.NextToken())
                    return null;
                if (!tk.StringValue.Equals("obj"))
                    return null;
                return new int[]{num, gen};
            }
            catch {
            }
            return null;
        }
        
        public bool IsHexString() {
            return this.hexString;
        }
        
    }
}