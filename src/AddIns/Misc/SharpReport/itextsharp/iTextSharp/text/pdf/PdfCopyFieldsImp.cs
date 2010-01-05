using System;
using System.Collections;
using System.IO;
using System.util;
/*
 * Copyright 2004 by Paulo Soares.
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
    * @author  psoares
    */
    internal class PdfCopyFieldsImp : PdfWriter {

        private static readonly PdfName iTextTag = new PdfName("_iTextTag_");
        private static object zero = 0;
        internal ArrayList readers = new ArrayList();
        internal Hashtable readers2intrefs = new Hashtable();
        internal Hashtable pages2intrefs = new Hashtable();
        internal Hashtable visited = new Hashtable();
        internal ArrayList fields = new ArrayList();
        internal RandomAccessFileOrArray file;
        internal Hashtable fieldTree = new Hashtable();
        internal ArrayList pageRefs = new ArrayList();
        internal ArrayList pageDics = new ArrayList();
        internal PdfDictionary resources = new PdfDictionary();
        internal PdfDictionary form;
        bool closing = false;
        internal Document nd;
        private Hashtable tabOrder;
        private ArrayList calculationOrder = new ArrayList();
        private ArrayList calculationOrderRefs;
        
        internal PdfCopyFieldsImp(Stream os) : this(os, '\0') {
        }
        
        internal PdfCopyFieldsImp(Stream os, char pdfVersion) : base(new PdfDocument(), os) {
            pdf.AddWriter(this);
            if (pdfVersion != 0)
                base.PdfVersion = pdfVersion;
            nd = new Document();
            nd.AddDocListener(pdf);
        }
        
        internal void AddDocument(PdfReader reader, ArrayList pagesToKeep) {
            if (!readers2intrefs.ContainsKey(reader) && reader.Tampered)
                throw new DocumentException("The document was reused.");
            reader = new PdfReader(reader);        
            reader.SelectPages(pagesToKeep);
            if (reader.NumberOfPages == 0)
                return;
            reader.Tampered = false;
            AddDocument(reader);
        }
        
        internal void AddDocument(PdfReader reader) {
            if (!reader.IsOpenedWithFullPermissions)
                throw new ArgumentException("PdfReader not opened with owner password");
            OpenDoc();
            if (readers2intrefs.ContainsKey(reader)) {
                reader = new PdfReader(reader);
            }
            else {
                if (reader.Tampered)
                    throw new DocumentException("The document was reused.");
                reader.ConsolidateNamedDestinations();
                reader.Tampered = true;
            }
            reader.ShuffleSubsetNames();
            readers2intrefs[reader] =  new IntHashtable();
            readers.Add(reader);
            int len = reader.NumberOfPages;
            IntHashtable refs = new IntHashtable();
            for (int p = 1; p <= len; ++p) {
                refs[reader.GetPageOrigRef(p).Number] = 1;
                reader.ReleasePage(p);
            }
            pages2intrefs[reader] =  refs;
            visited[reader] =  new IntHashtable();
            fields.Add(reader.AcroFields);
            UpdateCalculationOrder(reader);
        }
        
        private static String GetCOName(PdfReader reader, PRIndirectReference refi) {
            String name = "";
            while (refi != null) {
                PdfObject obj = PdfReader.GetPdfObject(refi);
                if (obj == null || obj.Type != PdfObject.DICTIONARY)
                    break;
                PdfDictionary dic = (PdfDictionary)obj;
                PdfString t = (PdfString)PdfReader.GetPdfObject(dic.Get(PdfName.T));
                if (t != null) {
                    name = t.ToUnicodeString()+ "." + name;
                }
                refi = (PRIndirectReference)dic.Get(PdfName.PARENT);
            }
            if (name.EndsWith("."))
                name = name.Substring(0, name.Length - 1);
            return name;
        }
        
        private void UpdateCalculationOrder(PdfReader reader) {
            PdfDictionary catalog = reader.Catalog;
            PdfDictionary acro = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.ACROFORM));
            if (acro == null)
                return;
            PdfArray co = (PdfArray)PdfReader.GetPdfObject(acro.Get(PdfName.CO));
            if (co == null || co.Size == 0)
                return;
            AcroFields af = reader.AcroFields;
            ArrayList coa = co.ArrayList;
            for (int k = 0; k < coa.Count; ++k) {
                PdfObject obj = (PdfObject)coa[k];
                if (obj == null || !obj.IsIndirect())
                    continue;
                String name = GetCOName(reader, (PRIndirectReference)obj) ;
                if (af.GetFieldItem(name) == null)
                    continue;
                name = "." + name;
                if (calculationOrder.Contains(name))
                    continue;
                calculationOrder.Add(name);
            }
        }
        
        internal void Propagate(PdfObject obj, PdfIndirectReference refo, bool restricted) {
            if (obj == null)
                return;
    //        if (refo != null)
    //            AddToBody(obj, refo);
            if (obj is PdfIndirectReference)
                return;
            switch (obj.Type) {
                case PdfObject.DICTIONARY:
                case PdfObject.STREAM: {
                    PdfDictionary dic = (PdfDictionary)obj;
                    foreach (PdfName key in dic.Keys) {
                        if (restricted && (key.Equals(PdfName.PARENT) || key.Equals(PdfName.KIDS)))
                            continue;
                        PdfObject ob = dic.Get(key);
                        if (ob != null && ob.IsIndirect()) {
                            PRIndirectReference ind = (PRIndirectReference)ob;
                            if (!SetVisited(ind) && !IsPage(ind)) {
                                PdfIndirectReference refi = GetNewReference(ind);
                                Propagate(PdfReader.GetPdfObjectRelease(ind), refi, restricted);
                            }
                        }
                        else
                            Propagate(ob, null, restricted);
                    }
                    break;
                }
                case PdfObject.ARRAY: {
                    ArrayList list = ((PdfArray)obj).ArrayList;
                    //PdfArray arr = new PdfArray();
                    foreach (PdfObject ob in list) {
                        if (ob != null && ob.IsIndirect()) {
                            PRIndirectReference ind = (PRIndirectReference)ob;
                            if (!IsVisited(ind) && !IsPage(ind)) {
                                PdfIndirectReference refi = GetNewReference(ind);
                                Propagate(PdfReader.GetPdfObjectRelease(ind), refi, restricted);
                            }
                        }
                        else
                            Propagate(ob, null, restricted);
                    }
                    break;
                }
                case PdfObject.INDIRECT: {
                    throw new Exception("Reference pointing to reference.");
                }
            }
        }
        
        private void AdjustTabOrder(PdfArray annots, PdfIndirectReference ind, PdfNumber nn) {
            int v = nn.IntValue;
            ArrayList t = (ArrayList)tabOrder[annots] ;
            if (t == null) {
                t = new ArrayList();
                int size = annots.Size - 1;
                for (int k = 0; k < size; ++k) {
                    t.Add(zero);
                }
                t.Add(v);
                tabOrder[annots] =  t;
                annots.Add(ind);
            }
            else {
                int size = t.Count - 1;
                for (int k = size; k >= 0; --k) {
                    if ((int)t[k] <= v) {
                        t.Insert(k + 1, v);
                        annots.ArrayList.Insert(k + 1, ind);
                        size = -2;
                        break;
                    }
                }
                if (size != -2) {
                    t.Insert(0, v);
                    annots.ArrayList.Insert(0, ind);
                }
            }
        }
        
        protected PdfArray BranchForm(Hashtable level, PdfIndirectReference parent, String fname) {
            PdfArray arr = new PdfArray();
            foreach (DictionaryEntry entry in level) {
                String name = (String)entry.Key;
                Object obj = entry.Value;
                PdfIndirectReference ind = PdfIndirectReference;
                PdfDictionary dic = new PdfDictionary();
                if (parent != null)
                    dic.Put(PdfName.PARENT, parent);
                dic.Put(PdfName.T, new PdfString(name, PdfObject.TEXT_UNICODE));
                String fname2 = fname + "." + name;
                int coidx = calculationOrder.IndexOf(fname2);
                if (coidx >= 0)
                    calculationOrderRefs[coidx] = ind;
                if (obj is Hashtable) {
                    dic.Put(PdfName.KIDS, BranchForm((Hashtable)obj, ind, fname2));
                    arr.Add(ind);
                    AddToBody(dic, ind);
                }
                else {
                    ArrayList list = (ArrayList)obj;
                    dic.MergeDifferent((PdfDictionary)list[0]);
                    if (list.Count == 3) {
                        dic.MergeDifferent((PdfDictionary)list[2]);
                        int page = (int)list[1];
                        PdfDictionary pageDic = (PdfDictionary)pageDics[page - 1];
                        PdfArray annots = (PdfArray)PdfReader.GetPdfObject(pageDic.Get(PdfName.ANNOTS));
                        if (annots == null) {
                            annots = new PdfArray();
                            pageDic.Put(PdfName.ANNOTS, annots);
                        }
                        PdfNumber nn = (PdfNumber)dic.Get(iTextTag);
                        dic.Remove(iTextTag);
                        AdjustTabOrder(annots, ind, nn);
                    }
                    else {
                        PdfArray kids = new PdfArray();
                        for (int k = 1; k < list.Count; k += 2) {
                            int page = (int)list[k];
                            PdfDictionary pageDic = (PdfDictionary)pageDics[page - 1];
                            PdfArray annots = (PdfArray)PdfReader.GetPdfObject(pageDic.Get(PdfName.ANNOTS));
                            if (annots == null) {
                                annots = new PdfArray();
                                pageDic.Put(PdfName.ANNOTS, annots);
                            }
                            PdfDictionary widget = new PdfDictionary();
                            widget.Merge((PdfDictionary)list[k + 1]);
                            widget.Put(PdfName.PARENT, ind);
                            PdfNumber nn = (PdfNumber)widget.Get(iTextTag);
                            widget.Remove(iTextTag);
                            PdfIndirectReference wref = AddToBody(widget).IndirectReference;
                            AdjustTabOrder(annots, wref, nn);
                            kids.Add(wref);
                            Propagate(widget, null, false);
                        }
                        dic.Put(PdfName.KIDS, kids);
                    }
                    arr.Add(ind);
                    AddToBody(dic, ind);
                    Propagate(dic, null, false);
                }
            }
            return arr;
        }
        
        protected void CreateAcroForms() {
            if (fieldTree.Count == 0)
                return;
            form = new PdfDictionary();
            form.Put(PdfName.DR, resources);
            Propagate(resources, null, false);
            form.Put(PdfName.DA, new PdfString("/Helv 0 Tf 0 g "));
            tabOrder = new Hashtable();
            calculationOrderRefs = new ArrayList(calculationOrder);
            form.Put(PdfName.FIELDS, BranchForm(fieldTree, null, ""));
            PdfArray co = new PdfArray();
            for (int k = 0; k < calculationOrderRefs.Count; ++k) {
                Object obj = calculationOrderRefs[k];
                if (obj is PdfIndirectReference)
                    co.Add((PdfIndirectReference)obj);
            }
            if (co.Size > 0)
                form.Put(PdfName.CO, co);
        }
        
        public override void Close() {
            if (closing) {
                base.Close();
                return;
            }
            closing = true;
            CloseIt();
        }
        
        protected void CloseIt() {
            for (int k = 0; k < readers.Count; ++k) {
                ((PdfReader)readers[k]).RemoveFields();
            }
            for (int r = 0; r < readers.Count; ++r) {
                PdfReader reader = (PdfReader)readers[r];
                for (int page = 1; page <= reader.NumberOfPages; ++page) {
                    pageRefs.Add(GetNewReference(reader.GetPageOrigRef(page)));
                    pageDics.Add(reader.GetPageN(page));
                }
            }
            MergeFields();
            CreateAcroForms();
            for (int r = 0; r < readers.Count; ++r) {
                    PdfReader reader = (PdfReader)readers[r];
                    for (int page = 1; page <= reader.NumberOfPages; ++page) {
                        PdfDictionary dic = reader.GetPageN(page);
                        PdfIndirectReference pageRef = GetNewReference(reader.GetPageOrigRef(page));
                        PdfIndirectReference parent = root.AddPageRef(pageRef);
                        dic.Put(PdfName.PARENT, parent);
                        Propagate(dic, pageRef, false);
                    }
            }
            foreach (DictionaryEntry entry in readers2intrefs) {
                PdfReader reader = (PdfReader)entry.Key;
                try {
                    file = reader.SafeFile;
                    file.ReOpen();
                    IntHashtable t = (IntHashtable)entry.Value;
                    int[] keys = t.ToOrderedKeys();
                    for (int k = 0; k < keys.Length; ++k) {
                        PRIndirectReference refi = new PRIndirectReference(reader, keys[k]);
                        AddToBody(PdfReader.GetPdfObjectRelease(refi), t[keys[k]]);
                    }
                }
                finally {
                    try {
                        file.Close();
                        reader.Close();
                    }
                    catch  {
                        // empty on purpose
                    }
                }
            }
            pdf.Close();
        }
        
        internal void AddPageOffsetToField(Hashtable fd, int pageOffset) {
            if (pageOffset == 0)
                return;
            foreach (AcroFields.Item item in fd.Values) {
                ArrayList page = item.page;
                for (int k = 0; k < page.Count; ++k)
                    page[k] = (int)page[k] + pageOffset;
            }
        }

        internal void CreateWidgets(ArrayList list, AcroFields.Item item) {
            for (int k = 0; k < item.merged.Count; ++k) {
                list.Add(item.page[k]);
                PdfDictionary merged = (PdfDictionary)item.merged[k];
                PdfObject dr = merged.Get(PdfName.DR);
                if (dr != null)
                    PdfFormField.MergeResources(resources, (PdfDictionary)PdfReader.GetPdfObject(dr));
                PdfDictionary widget = new PdfDictionary();
                foreach (PdfName key in merged.Keys) {
                    if (widgetKeys.ContainsKey(key))
                        widget.Put(key, merged.Get(key));
                }
                widget.Put(iTextTag, new PdfNumber((int)item.tabOrder[k] + 1));
                list.Add(widget);
            }
        }
        
        internal void MergeField(String name, AcroFields.Item item) {
            Hashtable map = fieldTree;
            StringTokenizer tk = new StringTokenizer(name, ".");
            if (!tk.HasMoreTokens())
                return;
            while (true) {
                String s = tk.NextToken();
                Object obj = map[s];
                if (tk.HasMoreTokens()) {
                    if (obj == null) {
                        obj = new Hashtable();
                        map[s] =  obj;
                        map = (Hashtable)obj;
                        continue;
                    }
                    else if (obj is Hashtable)
                        map = (Hashtable)obj;
                    else
                        return;
                }
                else {
                    if (obj is Hashtable)
                        return;
                    PdfDictionary merged = (PdfDictionary)item.merged[0];
                    if (obj == null) {
                        PdfDictionary field = new PdfDictionary();
                        foreach (PdfName key in merged.Keys) {
                            if (fieldKeys.ContainsKey(key))
                                field.Put(key, merged.Get(key));
                        }
                        ArrayList list = new ArrayList();
                        list.Add(field);
                        CreateWidgets(list, item);
                        map[s] =  list;
                    }
                    else {
                        ArrayList list = (ArrayList)obj;
                        PdfDictionary field = (PdfDictionary)list[0];
                        PdfName type1 = (PdfName)field.Get(PdfName.FT);
                        PdfName type2 = (PdfName)merged.Get(PdfName.FT);
                        if (type1 == null || !type1.Equals(type2))
                            return;
                        int flag1 = 0;
                        PdfObject f1 = field.Get(PdfName.FF);
                        if (f1 != null && f1.IsNumber())
                            flag1 = ((PdfNumber)f1).IntValue;
                        int flag2 = 0;
                        PdfObject f2 = merged.Get(PdfName.FF);
                        if (f2 != null && f2.IsNumber())
                            flag2 = ((PdfNumber)f2).IntValue;
                        if (type1.Equals(PdfName.BTN)) {
                            if (((flag1 ^ flag2) & PdfFormField.FF_PUSHBUTTON) != 0)
                                return;
                            if ((flag1 & PdfFormField.FF_PUSHBUTTON) == 0 && ((flag1 ^ flag2) & PdfFormField.FF_RADIO) != 0)
                                return;
                        }
                        else if (type1.Equals(PdfName.CH)) {
                            if (((flag1 ^ flag2) & PdfFormField.FF_COMBO) != 0)
                                return;
                        }
                        CreateWidgets(list, item);
                    }
                    return;
                }
            }
        }
        
        internal void MergeWithMaster(Hashtable fd) {
            foreach (DictionaryEntry entry in fd) {
                String name = (String)entry.Key;
                MergeField(name, (AcroFields.Item)entry.Value);
            }
        }
        
        internal void MergeFields() {
            int pageOffset = 0;
            for (int k = 0; k < fields.Count; ++k) {
                Hashtable fd = ((AcroFields)fields[k]).Fields;
                AddPageOffsetToField(fd, pageOffset);
                MergeWithMaster(fd);
                pageOffset += ((PdfReader)readers[k]).NumberOfPages;
            }
        }

        public override PdfIndirectReference GetPageReference(int page) {
            return (PdfIndirectReference)pageRefs[page - 1];
        }
        
        protected override PdfDictionary GetCatalog(PdfIndirectReference rootObj) {
            PdfDictionary cat = pdf.GetCatalog(rootObj);
            if (form != null) {
                PdfIndirectReference refi = AddToBody(form).IndirectReference;
                cat.Put(PdfName.ACROFORM, refi);
            }
            WriteOutlines(cat, false);
            return cat;
        }

        protected PdfIndirectReference GetNewReference(PRIndirectReference refi) {
            return new PdfIndirectReference(0, GetNewObjectNumber(refi.Reader, refi.Number, 0));
        }
        
        protected internal override int GetNewObjectNumber(PdfReader reader, int number, int generation) {
            IntHashtable refs = (IntHashtable)readers2intrefs[reader];
            int n = refs[number];
            if (n == 0) {
                n = IndirectReferenceNumber;
                refs[number] = n;
            }
            return n;
        }
        
        protected bool IsVisited(PdfReader reader, int number, int generation) {
            IntHashtable refs = (IntHashtable)readers2intrefs[reader];
            return refs.ContainsKey(number);
        }
        
        protected bool IsVisited(PRIndirectReference refi) {
            IntHashtable refs = (IntHashtable)visited[refi.Reader];
            return refs.ContainsKey(refi.Number);
        }
        
        protected bool SetVisited(PRIndirectReference refi) {
            IntHashtable refs = (IntHashtable)visited[refi.Reader];
            int old = refs[refi.Number];
            refs[refi.Number] = 1;
            return (old != 0);
        }
        
        protected bool IsPage(PRIndirectReference refi) {
            IntHashtable refs = (IntHashtable)pages2intrefs[refi.Reader] ;
            return refs.ContainsKey(refi.Number);
        }

        internal override RandomAccessFileOrArray GetReaderFile(PdfReader reader) {
                return file;
        }

        public void OpenDoc() {
            if (!nd.IsOpen())
                nd.Open();
        }    
        
        protected internal static Hashtable widgetKeys = new Hashtable();
        protected internal static Hashtable fieldKeys = new Hashtable();
        static PdfCopyFieldsImp() {
            object one = 1;
            widgetKeys[PdfName.SUBTYPE] =  one;
            widgetKeys[PdfName.CONTENTS] =  one;
            widgetKeys[PdfName.RECT] =  one;
            widgetKeys[PdfName.NM] =  one;
            widgetKeys[PdfName.M] =  one;
            widgetKeys[PdfName.F] =  one;
            widgetKeys[PdfName.BS] =  one;
            widgetKeys[PdfName.BORDER] =  one;
            widgetKeys[PdfName.AP] =  one;
            widgetKeys[PdfName.AS] =  one;
            widgetKeys[PdfName.C] =  one;
            widgetKeys[PdfName.A] =  one;
            widgetKeys[PdfName.STRUCTPARENT] =  one;
            widgetKeys[PdfName.OC] =  one;
            widgetKeys[PdfName.H] =  one;
            widgetKeys[PdfName.MK] =  one;
            widgetKeys[PdfName.DA] =  one;
            widgetKeys[PdfName.Q] =  one;
            fieldKeys[PdfName.AA] =  one;
            fieldKeys[PdfName.FT] =  one;
            fieldKeys[PdfName.TU] =  one;
            fieldKeys[PdfName.TM] =  one;
            fieldKeys[PdfName.FF] =  one;
            fieldKeys[PdfName.V] =  one;
            fieldKeys[PdfName.DV] =  one;
            fieldKeys[PdfName.DS] =  one;
            fieldKeys[PdfName.RV] =  one;
            fieldKeys[PdfName.OPT] =  one;
            fieldKeys[PdfName.MAXLEN] =  one;
            fieldKeys[PdfName.TI] =  one;
            fieldKeys[PdfName.I] =  one;
            fieldKeys[PdfName.LOCK] =  one;
            fieldKeys[PdfName.SV] =  one;
        }
    }
}
