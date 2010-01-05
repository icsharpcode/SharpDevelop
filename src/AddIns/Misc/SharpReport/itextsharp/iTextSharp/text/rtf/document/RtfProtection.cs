using System;
/*
 * $Id: RtfProtection.cs,v 1.2 2008/05/13 11:25:50 psoares33 Exp $
 * 
 *
 * Copyright 2008 by Howard Shank (hgshank@yahoo.com)
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
 * the Initial Developer are Copyright (C) 1999-2006 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2006 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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
 
namespace iTextSharp.text.rtf.document {

    /**
    * <code>RtfProtection</code> 
    * <pre>
    * See ECMA Specification for WordprocessingML documentProtection element.
    * 
    * <strong>Reference:</strong>
    * Standard ECMA-376 1st Edition / December 2006
    * Office Open XML File Formats
    * </pre>
    * @since 2.1.1
    * @author Howard Shank (hgshank@yahoo.com)
    */
    public sealed class RtfProtection {
        /**
        * Default for protection level. 
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int LEVEL_NONE = 0x0000;
        /**
        * REVPROT
        * Mutually exclusive
        * This document is protected for revisions. The user can edit the document, 
        * but revision marking cannot be disabled.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int LEVEL_REVPROT = 0x0001; // protlevel0
        /**
        * ANNNOTPROT
        * Mutually exclusive
        * This document is protected for comments (annotations).
        * The user cannot edit the document but can insert comments (annotations).
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int LEVEL_ANNOTPROT = 0x0002; // protlevel1
        /**
        * FORMPROT
        * Mutually exclusive
        * Document is protected for forms.
        * see also \allprot (forms controlword)
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int LEVEL_FORMPROT = 0x0004; // protlevel2
        /**
        * READPROT
        * Mutually exclusive but can be combined with ANNOTPROT for backward compatibility 
        * Document is protected for editing, except areas marked as exceptions by \protstart and\protend
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int LEVEL_READPROT = 0x0008; // protlevel3


        /**
        * STYLELOCK
        * 
        * The document contains styles and formatting restrictions.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int STYLELOCK = 0x0001;
        /**
        * STYLELOCKENFORCED
        * 
        * The styles and formatting restrictions are being enforced.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int STYLELOCKENFORCED = 0x0002;
        /**
        * STYLELOCKBACKCOMP
        * 
        * Style lockdown backward compatibility flag, indicating we emitted protection 
        * keywords to get documents with styles and formatting restrictions to behave 
        * in a reasonable way when opened by older versions.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int STYLELOCKBACKCOMP = 0x0004;
        /**
        * STYLELOCKBACKCOMP
        * 
        * Allow AutoFormat to override styles and formatting restrictions.  When style 
        * protection is on, the user cannot add direct formatting.  This setting allows 
        * AutoFormat actions to apply direct formatting when needed.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public const int AUTOFMTOVERRIDE = 0x0008;
        
        
        /**
        * <code>initialCodeArray</code> Table from ECMA-376 Specification
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private static int[] initialCodeArray = { 
                0xE1F0, 
                0x1D0F, 
                0xCC9C, 
                0x84C0,
                0x110C,
                0x0E10,
                0xF1CE,
                0x313E,
                0x1872,
                0xE139,
                0xD40F,
                0x84F9,
                0x280C,
                0xA96A,
                0x4EC3
        };
        
        /**
        * <code>encryptionMatrix</code> Table from ECMA-376 Specification
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private static int[][] encryptionMatrix  = {
            /*              bit1    bit2    bit3    bit4    bit5    bit6    bit7   **bit8 is ignored** */
            /* char 1  */ new int[]{0x1021, 0x2042, 0x4084, 0x8108, 0x1231, 0x2462, 0x48C4},
            /* char 2  */ new int[]{0x3331, 0x6662, 0xCCC4, 0x89A9, 0x0373, 0x06E6, 0x0DCC},
            /* char 3  */ new int[]{0x3730, 0x6E60, 0xDCC0, 0xA9A1, 0x4363, 0x86C6, 0x1DAD},
            /* char 4  */ new int[]{0x76B4, 0xED68, 0xCAF1, 0x85C3, 0x1BA7, 0x374E, 0x6E9C},
            /* char 5  */ new int[]{0xAA51, 0x4483, 0x8906, 0x022D, 0x045A, 0x08B4, 0x1168},
            /* char 6  */ new int[]{0x45A0, 0x8B40, 0x06A1, 0x0D42, 0x1A84, 0x3508, 0x6A10},
            /* char 7  */ new int[]{0xB861, 0x60E3, 0xC1C6, 0x93AD, 0x377B, 0x6EF6, 0xDDEC},
            /* char 8  */ new int[]{0x47D3, 0x8FA6, 0x0F6D, 0x1EDA, 0x3DB4, 0x7B68, 0xF6D0},
            /* char 9  */ new int[]{0xEB23, 0xC667, 0x9CEF, 0x29FF, 0x53FE, 0xA7FC, 0x5FD9},
            /* char 10 */ new int[]{0x6F45, 0xDE8A, 0xAD35, 0x4A4B, 0x9496, 0x390D, 0x721A},
            /* char 11 */ new int[]{0xD849, 0xA0B3, 0x5147, 0xA28E, 0x553D, 0xAA7A, 0x44D5},
            /* char 12 */ new int[]{0x0375, 0x06EA, 0x0DD4, 0x1BA8, 0x3750, 0x6EA0, 0xDD40},
            /* char 13 */ new int[]{0x4563, 0x8AC6, 0x05AD, 0x0B5A, 0x16B4, 0x2D68, 0x5AD0},
            /* char 14 */ new int[]{0x7B61, 0xF6C2, 0xFDA5, 0xEB6B, 0xC6F7, 0x9DCF, 0x2BBF},
            /* char 15 */ new int[]{0xAEFC, 0x4DD9, 0x9BB2, 0x2745, 0x4E8A, 0x9D14, 0x2A09}
        };
        
        /**
        * <code>generateHash</code> generates the password hash from a clear text string.
        * 
        * @param pwd Clear text string input
        * @return hex encoded password hash
        * 
        * @author Howard Shank (hgshank@yahoo.com)
        * @since 2.1.1
        */
        public static String GenerateHash(String pwd) {
            String encryptedPwd="00000000";
            String password = pwd;
            
            // if there is no password or the length is 0, then skip this and return "00000000" as default
            // otherwise process the password
            if (password != null && password.Length > 0) {
                int hi=0;
                int lo=0;

                // Truncate the password to 15 characters.
                if (password.Length > 15) {
                    password = password.Substring(0,15);
                }

                // compute key's high-order word
                // initialize to table value
                hi = initialCodeArray[password.Length-1];
                
                int fidx = 0;
                int idxR = password.Length-1;
                // process each character left to right.
                // check each bit and if it is set, xor the hi word with 
                // the table entry for the position in password and bit position.
                for (; fidx<password.Length; fidx++,idxR--) {
                    int ch = password[fidx];
                    if ((ch & 0x0001)!= 0) {
                        hi = hi ^ encryptionMatrix[idxR][0];
                    }
                    if ((ch & 0x0002)!= 0) {
                        hi = hi ^ encryptionMatrix[idxR][1];
                    }
                    if ((ch & 0x0004)!= 0) {
                        hi = hi ^ encryptionMatrix[idxR][2];
                    }
                    if ((ch & 0x0008)!= 0) {
                        hi = hi ^ encryptionMatrix[idxR][3];
                    }
                    if ((ch & 0x0010)!= 0) {
                        hi = hi ^ encryptionMatrix[idxR][4];
                    }
                    if ((ch & 0x0020)!= 0) {
                        hi = hi ^ encryptionMatrix[idxR][5];
                    }
                    if ((ch & 0x0040)!= 0) {
                        hi = hi ^ encryptionMatrix[idxR][6];
                    }
                }
                // Compute Key's low-order word
                fidx = password.Length-1;
                lo = 0;
                // low order word is computed in reverse.
                for (;fidx>= 0; fidx--) {
                    int ch = password[fidx];
                    lo = (((lo >> 14) & 0x001) | (( lo << 1) & 0x7fff)) ^ ch;
                }
                // finally incorporate the password length into the low word and use value from formula
                lo = (((lo >> 14) & 0x001) | (( lo << 1) & 0x7fff)) ^ password.Length ^ 0xCE4B;
                
                // correct for little-endian - 
                // Java always uses big-endian. According to tests - RTF wants little-endian but is not documented
                string s = lo.ToString("x8");
                encryptedPwd = s.Substring(6,2) + s.Substring(4,2);
                s = hi.ToString("x8");
                encryptedPwd += s.Substring(6,2) + s.Substring(4,2);
            }
            return encryptedPwd;
        }
    }
}