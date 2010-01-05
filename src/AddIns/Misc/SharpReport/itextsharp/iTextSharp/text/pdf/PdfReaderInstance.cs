using System;
using System.IO;
using System.Collections;

using iTextSharp.text;

/*
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
    /**
    * Instance of PdfReader in each output document.
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class PdfReaderInstance {
        internal static PdfLiteral IDENTITYMATRIX = new PdfLiteral("[1 0 0 1 0 0]");
        internal static PdfNumber ONE = new PdfNumber(1);
        internal int[] myXref;
        internal PdfReader reader;
        internal RandomAccessFileOrArray file;
        internal Hashtable importedPages = new Hashtable();
        internal PdfWriter writer;
        internal Hashtable visited = new Hashtable();
        internal ArrayList nextRound = new ArrayList();
        
        internal PdfReaderInstance(PdfReader reader, PdfWriter writer) {
            this.reader = reader;
            this.writer = writer;
            file = reader.SafeFile;
            myXref = new int[reader.XrefSize];
        }
        
        internal PdfReader Reader {
            get {
                return reader;
            }
        }
        
        internal PdfImportedPage GetImportedPage(int pageNumber) {
            if (!reader.IsOpenedWithFullPermissions)
                throw new ArgumentException("PdfReader not opened with owner password");
            if (pageNumber < 1 || pageNumber > reader.NumberOfPages)
                throw new ArgumentException("Invalid page number: " + pageNumber);
            PdfImportedPage pageT = (PdfImportedPage)importedPages[pageNumber];
            if (pageT == null) {
                pageT = new PdfImportedPage(this, writer, pageNumber);
                importedPages[pageNumber] = pageT;
            }
            return pageT;
        }
        
        internal int GetNewObjectNumber(int number, int generation) {
            if (myXref[number] == 0) {
                myXref[number] = writer.IndirectReferenceNumber;
                nextRound.Add(number);
            }
            return myXref[number];
        }
        
        internal RandomAccessFileOrArray ReaderFile {
            get {
                return file;
            }
        }
        
        internal PdfObject GetResources(int pageNumber) {
            PdfObject obj = PdfReader.GetPdfObjectRelease(reader.GetPageNRelease(pageNumber).Get(PdfName.RESOURCES));
            return obj;
        }
        
        
        internal PdfStream GetFormXObject(int pageNumber) {
            PdfDictionary page = reader.GetPageNRelease(pageNumber);
            PdfObject contents = PdfReader.GetPdfObjectRelease(page.Get(PdfName.CONTENTS));
            PdfDictionary dic = new PdfDictionary();
            byte[] bout = null;
            if (contents != null) {
                if (contents.IsStream())
                    dic.Merge((PRStream)contents);
                else
                    bout = reader.GetPageContent(pageNumber, file);
            }
            else
                bout = new byte[0];
            dic.Put(PdfName.RESOURCES, PdfReader.GetPdfObjectRelease(page.Get(PdfName.RESOURCES)));
            dic.Put(PdfName.TYPE, PdfName.XOBJECT);
            dic.Put(PdfName.SUBTYPE, PdfName.FORM);
            PdfImportedPage impPage = (PdfImportedPage)importedPages[pageNumber];
            dic.Put(PdfName.BBOX, new PdfRectangle(impPage.BoundingBox));
            PdfArray matrix = impPage.Matrix;
            if (matrix == null)
                dic.Put(PdfName.MATRIX, IDENTITYMATRIX);
            else
                dic.Put(PdfName.MATRIX, matrix);
            dic.Put(PdfName.FORMTYPE, ONE);
            PRStream stream;
            if (bout == null) {
                stream = new PRStream((PRStream)contents, dic);
            }
            else {
                stream = new PRStream(reader, bout);
                stream.Merge(dic);
            }
            return stream;
        }
        
        internal void WriteAllVisited() {
            while (nextRound.Count > 0) {
                ArrayList vec = nextRound;
                nextRound = new ArrayList();
                for (int k = 0; k < vec.Count; ++k) {
                    int i = (int)vec[k];
                    if (!visited.ContainsKey(i)) {
                        visited[i] = null;
                        writer.AddToBody(reader.GetPdfObjectRelease(i), myXref[i]);
                    }
                }
            }
        }
        
        internal void WriteAllPages() {
            try {
                file.ReOpen();
                foreach (PdfImportedPage ip in importedPages.Values) {
                    writer.AddToBody(ip.FormXObject, ip.IndirectReference);
                }
                WriteAllVisited();
            }
            finally {
                try {
                    reader.Close();
                    file.Close();
                }
                catch  {
                    //Empty on purpose
                }
            }
        }
    }
}