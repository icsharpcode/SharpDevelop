using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.util;

/*
 * $Id: GlyphList.cs,v 1.7 2008/05/13 11:25:17 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 Paulo Soares
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

    public class GlyphList {
        private static Hashtable unicode2names = new Hashtable();
        private static Hashtable names2unicode = new Hashtable();
    
        static GlyphList() {
            Stream istr = null;
            try {
                istr = BaseFont.GetResourceStream(BaseFont.RESOURCE_PATH + "glyphlist.txt");
                if (istr == null) {
                    String msg = "glyphlist.txt not found as resource.";
                    throw new Exception(msg);
                }
                byte[] buf = new byte[1024];
                MemoryStream outp = new MemoryStream();
                while (true) {
                    int size = istr.Read(buf, 0, buf.Length);
                    if (size == 0)
                        break;
                    outp.Write(buf, 0, size);
                }
                istr.Close();
                istr = null;
                String s = PdfEncodings.ConvertToString(outp.ToArray(), null);
                StringTokenizer tk = new StringTokenizer(s, "\r\n");
                while (tk.HasMoreTokens()) {
                    String line = tk.NextToken();
                    if (line.StartsWith("#"))
                        continue;
                    StringTokenizer t2 = new StringTokenizer(line, " ;\r\n\t\f");
                    String name = null;
                    String hex = null;
                    if (!t2.HasMoreTokens())
                        continue;
                    name = t2.NextToken();
                    if (!t2.HasMoreTokens())
                        continue;
                    hex = t2.NextToken();
                    int num = int.Parse(hex, NumberStyles.HexNumber);
                    unicode2names[num] = name;
                    names2unicode[name] = new int[]{num};
                }
            }
            catch (Exception e) {
                Console.Error.WriteLine("glyphlist.txt loading error: " + e.Message);
            }
            finally {
                if (istr != null) {
                    try {
                        istr.Close();
                    }
                    catch {
                        // empty on purpose
                    }
                }
            }
        }
    
        public static int[] NameToUnicode(string name) {
            return (int[])names2unicode[name];
        }
    
        public static string UnicodeToName(int num) {
            return (string)unicode2names[num];
        }
    }
}