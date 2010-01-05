using System;
using System.IO;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: EnumerateTTC.cs,v 1.3 2008/05/13 11:25:17 psoares33 Exp $
 * 
 *
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

    /** Enumerates all the fonts inside a True Type Collection.
     *
     * @author  Paulo Soares (psoares@consiste.pt)
     */
    internal class EnumerateTTC : TrueTypeFont {

        protected String[] names;

        internal EnumerateTTC(String ttcFile) {
            fileName = ttcFile;
            rf = new RandomAccessFileOrArray(ttcFile);
            FindNames();
        }

        internal EnumerateTTC(byte[] ttcArray) {
            fileName = "Byte array TTC";
            rf = new RandomAccessFileOrArray(ttcArray);
            FindNames();
        }
    
        internal void FindNames() {
            tables = new Hashtable();
        
            try {
                String mainTag = ReadStandardString(4);
                if (!mainTag.Equals("ttcf"))
                    throw new DocumentException(fileName + " is not a valid TTC file.");
                rf.SkipBytes(4);
                int dirCount = rf.ReadInt();
                names = new String[dirCount];
                int dirPos = rf.FilePointer;
                for (int dirIdx = 0; dirIdx < dirCount; ++dirIdx) {
                    tables.Clear();
                    rf.Seek(dirPos);
                    rf.SkipBytes(dirIdx * 4);
                    directoryOffset = rf.ReadInt();
                    rf.Seek(directoryOffset);
                    if (rf.ReadInt() != 0x00010000)
                        throw new DocumentException(fileName + " is not a valid TTF file.");
                    int num_tables = rf.ReadUnsignedShort();
                    rf.SkipBytes(6);
                    for (int k = 0; k < num_tables; ++k) {
                        String tag = ReadStandardString(4);
                        rf.SkipBytes(4);
                        int[] table_location = new int[2];
                            table_location[0] = rf.ReadInt();
                        table_location[1] = rf.ReadInt();
                        tables[tag] = table_location;
                    }
                    names[dirIdx] = this.BaseFont;
                }
            }
            finally {
                if (rf != null)
                    rf.Close();
            }
        }
    
        internal String[] Names {
            get {
                return names;
            }
        }
    }
}