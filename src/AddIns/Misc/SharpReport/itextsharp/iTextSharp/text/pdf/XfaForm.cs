using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
/*
 * $Id: XfaForm.cs,v 1.7 2007/05/21 10:55:28 psoares33 Exp $
 *
 * Copyright 2006 Paulo Soares
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
    * Processes XFA forms.
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class XfaForm {

        private Xml2SomTemplate templateSom;
        private Xml2SomDatasets datasetsSom;
        private AcroFieldsSearch acroFieldsSom;
        private PdfReader reader;
        private bool xfaPresent;
        private XmlDocument domDocument;
        private bool changed;
        private XmlNode datasetsNode;
        public const String XFA_DATA_SCHEMA = "http://www.xfa.org/schema/xfa-data/1.0/";

        /**
        * An empty constructor to build on.
        */
        public XfaForm() {
        }
        
        /**
        * A constructor from a <CODE>PdfReader</CODE>. It basically does everything
        * from finding the XFA stream to the XML parsing.
        * @param reader the reader
        * @throws java.io.IOException on error
        * @throws javax.xml.parsers.ParserConfigurationException on error
        * @throws org.xml.sax.SAXException on error
        */
        public XfaForm(PdfReader reader) {
            this.reader = reader;
            PdfDictionary af = (PdfDictionary)PdfReader.GetPdfObjectRelease(reader.Catalog.Get(PdfName.ACROFORM));
            if (af == null) {
                xfaPresent = false;
                return;
            }
            PdfObject xfa = PdfReader.GetPdfObjectRelease(af.Get(PdfName.XFA));
            if (xfa == null) {
                xfaPresent = false;
                return;
            }
            xfaPresent = true;
            MemoryStream bout = new MemoryStream();
            if (xfa.IsArray()) {
                ArrayList ar = ((PdfArray)xfa).ArrayList;
                for (int k = 1; k < ar.Count; k += 2) {
                    PdfObject ob = PdfReader.GetPdfObject((PdfObject)ar[k]);
                    if (ob is PRStream) {
                        byte[] b = PdfReader.GetStreamBytes((PRStream)ob);
                        bout.Write(b, 0, b.Length);
                    }
                }
            }
            else if (xfa is PRStream) {
                byte[] b = PdfReader.GetStreamBytes((PRStream)xfa);
                bout.Write(b, 0, b.Length);
            }
            bout.Seek(0, SeekOrigin.Begin);
            XmlTextReader xtr = new XmlTextReader(bout);
            domDocument = new XmlDocument();
            domDocument.Load(xtr);
            XmlNode n = domDocument.FirstChild;
            while (n.NodeType != XmlNodeType.Element)
                n = n.NextSibling;
            n = n.FirstChild;
            while (n != null) {
                if (n.NodeType == XmlNodeType.Element) {
                    String s = n.LocalName;
                    if (s.Equals("template")) {
                        templateSom = new Xml2SomTemplate(n);
                    }
                    else if (s.Equals("datasets")) {
                        datasetsNode = n;
                        datasetsSom = new Xml2SomDatasets(n.FirstChild);
                    }
                }
                n = n.NextSibling;
            }
        }
        
        /**
        * Sets the XFA key from a byte array. The old XFA is erased.
        * @param xfaData the data
        * @param reader the reader
        * @param writer the writer
        * @throws java.io.IOException on error
        */
        public static void SetXfa(byte[] xfaData, PdfReader reader, PdfWriter writer) {
            PdfDictionary af = (PdfDictionary)PdfReader.GetPdfObjectRelease(reader.Catalog.Get(PdfName.ACROFORM));
            if (af == null) {
                return;
            }
            reader.KillXref(af.Get(PdfName.XFA));
            PdfStream str = new PdfStream(xfaData);
            str.FlateCompress();
            PdfIndirectReference refe = writer.AddToBody(str).IndirectReference;
            af.Put(PdfName.XFA, refe);
        }

        /**
        * Sets the XFA key from the instance data. The old XFA is erased.
        * @param writer the writer
        * @throws java.io.IOException on error
        */
        public void SetXfa(PdfWriter writer) {
            SetXfa(SerializeDoc(domDocument), reader, writer);
        }

        /**
        * Serializes a XML document to a byte array.
        * @param n the XML document
        * @throws java.io.IOException on error
        * @return the serialized XML document
        */
        public static byte[] SerializeDoc(XmlNode n) {
            MemoryStream fout = new MemoryStream();
            XmlTextWriter xw = new XmlTextWriter(fout, new UTF8Encoding(false));
            xw.WriteNode(new XmlNodeReader(n), true);
            xw.Close();
            return fout.ToArray();
        }
        
        /**
        * Returns <CODE>true</CODE> if it is a XFA form.
        * @return <CODE>true</CODE> if it is a XFA form
        */
        public bool XfaPresent {
            get {
                return xfaPresent;
            }
            set {
                xfaPresent = value;
            }
        }

        /**
        * Gets the top level DOM document.
        * @return the top level DOM document
        */
        public XmlDocument DomDocument {
            get {
                return domDocument;
            }
            set {
                domDocument = value;
            }
        }
        
        
        /**
        * Finds the complete field name contained in the "classic" forms from a partial
        * name.
        * @param name the complete or partial name
        * @param af the fields
        * @return the complete name or <CODE>null</CODE> if not found
        */
        public String FindFieldName(String name, AcroFields af) {
            Hashtable items = af.Fields;
            if (items.ContainsKey(name))
                return name;
            if (acroFieldsSom == null) {
                acroFieldsSom = new AcroFieldsSearch(items.Keys);
            }
            if (acroFieldsSom.AcroShort2LongName.ContainsKey(name))
                return (String)acroFieldsSom.AcroShort2LongName[name];
            return acroFieldsSom.InverseSearchGlobal(Xml2Som.SplitParts(name));
        }
        
        /**
        * Finds the complete SOM name contained in the datasets section from a 
        * possibly partial name.
        * @param name the complete or partial name
        * @return the complete name or <CODE>null</CODE> if not found
        */
        public String FindDatasetsName(String name) {
            if (datasetsSom.Name2Node.ContainsKey(name))
                return name;
            return datasetsSom.InverseSearchGlobal(Xml2Som.SplitParts(name));
        }

        /**
        * Finds the <CODE>Node</CODE> contained in the datasets section from a 
        * possibly partial name.
        * @param name the complete or partial name
        * @return the <CODE>Node</CODE> or <CODE>null</CODE> if not found
        */
        public XmlNode FindDatasetsNode(String name) {
            if (name == null)
                return null;
            name = FindDatasetsName(name);
            if (name == null)
                return null;
            return (XmlNode)datasetsSom.Name2Node[name];
        }

        /**
        * Gets all the text contained in the child nodes of this node.
        * @param n the <CODE>Node</CODE>
        * @return the text found or "" if no text was found
        */
        public static String GetNodeText(XmlNode n) {
            if (n == null)
                return "";
            return GetNodeText(n, "");
            
        }
        
        private static String GetNodeText(XmlNode n, String name) {
            XmlNode n2 = n.FirstChild;
            while (n2 != null) {
                if (n2.NodeType == XmlNodeType.Element) {
                    name = GetNodeText(n2, name);
                }
                else if (n2.NodeType == XmlNodeType.Text) {
                    name += n2.Value;
                }
                n2 = n2.NextSibling;
            }
            return name;
        }
        
        /**
        * Sets the text of this node. All the child's node are deleted and a new
        * child text node is created.
        * @param n the <CODE>Node</CODE> to add the text to
        * @param text the text to add
        */
        public void SetNodeText(XmlNode n, String text) {
            if (n == null)
                return;
            XmlNode nc = null;
            while ((nc = n.FirstChild) != null) {
                n.RemoveChild(nc);
            }
            n.Attributes.RemoveNamedItem("dataNode", XFA_DATA_SCHEMA);
            n.AppendChild(domDocument.CreateTextNode(text));
            changed = true;
        }
        
        /**
        * Sets the <CODE>PdfReader</CODE> to be used by this instance.
        * @param reader the <CODE>PdfReader</CODE> to be used by this instance
        */
        public PdfReader Reader {
            set { 
                reader = value;
            }
            get {
                return reader;
            }
        }

        /**
        * Checks if this XFA form was changed.
        * @return <CODE>true</CODE> if this XFA form was changed
        */
        public bool Changed {
            get {
                return changed;
            }
            set {
                changed = value;
            }
        }

        /**
        * A structure to store each part of a SOM name and link it to the next part
        * beginning from the lower hierarchie.
        */
        public class InverseStore {
            protected internal ArrayList part = new ArrayList();
            protected internal ArrayList follow = new ArrayList();
            
            /**
            * Gets the full name by traversing the hiearchie using only the
            * index 0.
            * @return the full name
            */
            public String DefaultName {
                get {
                    InverseStore store = this;
                    while (true) {
                        Object obj = store.follow[0];
                        if (obj is String)
                            return (String)obj;
                        store = (InverseStore)obj;
                    }
                }
            }
            
            /**
            * Search the current node for a similar name. A similar name starts
            * with the same name but has a differnt index. For example, "detail[3]" 
            * is similar to "detail[9]". The main use is to discard names that
            * correspond to out of bounds records.
            * @param name the name to search
            * @return <CODE>true</CODE> if a similitude was found
            */
            public bool IsSimilar(String name) {
                int idx = name.IndexOf('[');
                name = name.Substring(0, idx + 1);
                foreach (String n in part) { 
                    if (n.StartsWith(name))
                        return true;
                }
                return false;
            }
        }

        /**
        * Another stack implementation. The main use is to facilitate
        * the porting to other languages.
        */
        public class Stack2 : ArrayList {
            /**
            * Looks at the object at the top of this stack without removing it from the stack.
            * @return the object at the top of this stack
            */
            public Object Peek() {
                if (Count == 0)
                    throw new InvalidOperationException();
                return this[Count - 1];
            }
            
            /**
            * Removes the object at the top of this stack and returns that object as the value of this function.
            * @return the object at the top of this stack 
            */
            public Object Pop() {
                if (Count == 0)
                    throw new InvalidOperationException();
                Object ret = this[Count - 1];
                RemoveAt(Count - 1);
                return ret;
            }
            
            /**
            * Pushes an item onto the top of this stack.
            * @param item the item to be pushed onto this stack
            * @return the <CODE>item</CODE> argument
            */
            public Object Push(Object item) {
                Add(item);
                return item;
            }
            
            /**
            * Tests if this stack is empty.
            * @return <CODE>true</CODE> if and only if this stack contains no items; <CODE>false</CODE> otherwise
            */
            public bool Empty() {
                return Count == 0;
            }
        }
        
        /**
        * A class for some basic SOM processing.
        */
        public class Xml2Som {
            /**
            * The order the names appear in the XML, depth first.
            */
            protected ArrayList order;
            /**
            * The mapping of full names to nodes.
            */
            protected Hashtable name2Node;
            /**
            * The data to do a search from the bottom hierarchie.
            */
            protected Hashtable inverseSearch;
            /**
            * A stack to be used when parsing.
            */
            protected Stack2 stack;
            /**
            * A temporary store for the repetition count.
            */
            protected int anform;

            /**
            * Escapes a SOM string fragment replacing "." with "\.".
            * @param s the unescaped string
            * @return the escaped string
            */
            public static String EscapeSom(String s) {
                int idx = s.IndexOf('.');
                if (idx < 0)
                    return s;
                StringBuilder sb = new StringBuilder();
                int last = 0;
                while (idx >= 0) {
                    sb.Append(s.Substring(last, idx - last));
                    sb.Append('\\');
                    last = idx;
                    idx = s.IndexOf('.', idx + 1);
                }
                sb.Append(s.Substring(last));
                return sb.ToString();
            }

            /**
            * Unescapes a SOM string fragment replacing "\." with ".".
            * @param s the escaped string
            * @return the unescaped string
            */
            public static String UnescapeSom(String s) {
                int idx = s.IndexOf('\\');
                if (idx < 0)
                    return s;
                StringBuilder sb = new StringBuilder();
                int last = 0;
                while (idx >= 0) {
                    sb.Append(s.Substring(last, idx - last));
                    last = idx + 1;
                    idx = s.IndexOf('\\', idx + 1);
                }
                sb.Append(s.Substring(last));
                return sb.ToString();
            }

            /**
            * Outputs the stack as the sequence of elements separated
            * by '.'.
            * @return the stack as the sequence of elements separated by '.'
            */
            protected String PrintStack() {
                if (stack.Empty())
                    return "";
                StringBuilder s = new StringBuilder();
                foreach (String part in stack)
                    s.Append('.').Append(part);
                return s.ToString(1, s.Length - 1);
            }
            
            /**
            * Gets the name with the <CODE>#subform</CODE> removed.
            * @param s the long name
            * @return the short name
            */
            public static String GetShortName(String s) {
                int idx = s.IndexOf(".#subform[");
                if (idx < 0)
                    return s;
                int last = 0;
                StringBuilder sb = new StringBuilder();
                while (idx >= 0) {
                    sb.Append(s.Substring(last, idx - last));
                    idx = s.IndexOf("]", idx + 10);
                    if (idx < 0)
                        return sb.ToString();
                    last = idx + 1;
                    idx = s.IndexOf(".#subform[", last);
                }
                sb.Append(s.Substring(last));
                return sb.ToString();
            }
            
            /**
            * Adds a SOM name to the search node chain.
            * @param unstack the SOM name
            */
            public void InverseSearchAdd(String unstack) {
                InverseSearchAdd(inverseSearch, stack, unstack);
            }
            
            /**
            * Adds a SOM name to the search node chain.
            * @param inverseSearch the start point
            * @param stack the stack with the separeted SOM parts
            * @param unstack the full name
            */
            public static void InverseSearchAdd(Hashtable inverseSearch, Stack2 stack, String unstack) {
                String last = (String)stack.Peek();
                InverseStore store = (InverseStore)inverseSearch[last];
                if (store == null) {
                    store = new InverseStore();
                    inverseSearch[last] = store;
                }
                for (int k = stack.Count - 2; k >= 0; --k) {
                    last = (String)stack[k];
                    InverseStore store2;
                    int idx = store.part.IndexOf(last);
                    if (idx < 0) {
                        store.part.Add(last);
                        store2 = new InverseStore();
                        store.follow.Add(store2);
                    }
                    else
                        store2 = (InverseStore)store.follow[idx];
                    store = store2;
                }
                store.part.Add("");
                store.follow.Add(unstack);
            }

            /**
            * Searchs the SOM hiearchie from the bottom.
            * @param parts the SOM parts
            * @return the full name or <CODE>null</CODE> if not found
            */
            public String InverseSearchGlobal(ArrayList parts) {
                if (parts.Count == 0)
                    return null;
                InverseStore store = (InverseStore)inverseSearch[parts[parts.Count - 1]];
                if (store == null)
                    return null;
                for (int k = parts.Count - 2; k >= 0; --k) {
                    String part = (String)parts[k];
                    int idx = store.part.IndexOf(part);
                    if (idx < 0) {
                        if (store.IsSimilar(part))
                            return null;
                        return store.DefaultName;
                    }
                    store = (InverseStore)store.follow[idx];
                }
                return store.DefaultName;
            }
        
            /**
            * Splits a SOM name in the individual parts.
            * @param name the full SOM name
            * @return the split name
            */
            public static Stack2 SplitParts(String name) {
                while (name.StartsWith("."))
                    name = name.Substring(1);
                Stack2 parts = new Stack2();
                int last = 0;
                int pos = 0;
                String part;
                while (true) {
                    pos = last;
                    while (true) {
                        pos = name.IndexOf('.', pos);
                        if (pos < 0)
                            break;
                        if (name[pos - 1] == '\\')
                            ++pos;
                        else
                            break;
                    }
                    if (pos < 0)
                        break;
                    part = name.Substring(last, pos - last);
                    if (!part.EndsWith("]"))
                        part += "[0]";
                    parts.Add(part);
                    last = pos + 1;
                }
                part = name.Substring(last);
                if (!part.EndsWith("]"))
                    part += "[0]";
                parts.Add(part);
                return parts;
            }

            /**
            * Gets the order the names appear in the XML, depth first.
            * @return the order the names appear in the XML, depth first
            */
            public ArrayList Order {
                get {
                    return order;
                }
                set {
                    order = value;
                }
            }

            /**
            * Gets the mapping of full names to nodes.
            * @return the mapping of full names to nodes
            */
            public Hashtable Name2Node {
                get {
                    return name2Node;
                }
                set {
                    name2Node = value;
                }
            }

            /**
            * Gets the data to do a search from the bottom hierarchie.
            * @return the data to do a search from the bottom hierarchie
            */
            public Hashtable InverseSearch {
                get {
                    return inverseSearch;
                }
                set {
                    inverseSearch = value;
                }
            }
        }
        
        /**
        * Processes the datasets section in the XFA form.
        */
        public class Xml2SomDatasets : Xml2Som {
            /**
            * Creates a new instance from the datasets node. This expects
            * not the datasets but the data node that comes below.
            * @param n the datasets node
            */
            public Xml2SomDatasets(XmlNode n) {
                order = new ArrayList();
                name2Node = new Hashtable();
                stack = new Stack2();
                anform = 0;
                inverseSearch = new Hashtable();
                ProcessDatasetsInternal(n);
            }

            /**
            * Inserts a new <CODE>Node</CODE> that will match the short name.
            * @param n the datasets top <CODE>Node</CODE>
            * @param shortName the short name
            * @return the new <CODE>Node</CODE> of the inserted name
            */
            public XmlNode InsertNode(XmlNode n, String shortName) {
                Stack2 stack = SplitParts(shortName);
                XmlDocument doc = n.OwnerDocument;
                XmlNode n2 = null;
                n = n.FirstChild;
                for (int k = 0; k < stack.Count; ++k) {
                    String part = (String)stack[k];
                    int idx = part.LastIndexOf('[');
                    String name = part.Substring(0, idx);
                    idx = int.Parse(part.Substring(idx + 1, part.Length - idx - 2));
                    int found = -1;
                    for (n2 = n.FirstChild; n2 != null; n2 = n2.NextSibling) {
                        if (n2.NodeType == XmlNodeType.Element) {
                            String s = EscapeSom(n2.LocalName);
                            if (s.Equals(name)) {
                                ++found;
                                if (found == idx)
                                    break;
                            }
                        }
                    }
                    for (; found < idx; ++found) {
                        n2 = doc.CreateElement(name);
                        n2 = n.AppendChild(n2);
                        XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, "dataNode", XFA_DATA_SCHEMA);
                        attr.Value = "dataGroup";
                        n2.Attributes.SetNamedItem(attr);
                    }
                    n = n2;
                }
                InverseSearchAdd(inverseSearch, stack, shortName);
                name2Node[shortName] = n2;
                order.Add(shortName);
                return n2;
            }

            private static bool HasChildren(XmlNode n) {
                XmlNode dataNodeN = n.Attributes.GetNamedItem("dataNode", XFA_DATA_SCHEMA);
                if (dataNodeN != null) {
                    String dataNode = dataNodeN.Value;
                    if ("dataGroup".Equals(dataNode))
                        return true;
                    else if ("dataValue".Equals(dataNode))
                        return false;
                }
                if (!n.HasChildNodes)
                    return false;
                XmlNode n2 = n.FirstChild;
                while (n2 != null) {
                    if (n2.NodeType == XmlNodeType.Element) {
                        return true;
                    }
                    n2 = n2.NextSibling;
                }
                return false;
            }

            private void ProcessDatasetsInternal(XmlNode n) {
                Hashtable ss = new Hashtable();
                XmlNode n2 = n.FirstChild;
                while (n2 != null) {
                    if (n2.NodeType == XmlNodeType.Element) {
                        String s = EscapeSom(n2.LocalName);
                        int i;
                        if (ss[s] == null)
                            i = 0;
                        else
                            i = (int)ss[s] + 1;
                        ss[s] = i;
                        if (HasChildren(n2)) {
                            stack.Push(s + "[" + i.ToString() + "]");
                            ProcessDatasetsInternal(n2);
                            stack.Pop();
                        }
                        else {
                            stack.Push(s + "[" + i.ToString() + "]");
                            String unstack = PrintStack();
                            order.Add(unstack);
                            InverseSearchAdd(unstack);
                            name2Node[unstack] = n2;
                            stack.Pop();
                        }
                    }
                    n2 = n2.NextSibling;
                }
            }
        }

        /**
        * A class to process "classic" fields.
        */
        public class AcroFieldsSearch : Xml2Som {
            private Hashtable acroShort2LongName;
            
            /**
            * Creates a new instance from a Collection with the full names.
            * @param items the Collection
            */
            public AcroFieldsSearch(ICollection items) {
                inverseSearch = new Hashtable();
                acroShort2LongName = new Hashtable();
                foreach (String itemName in items) {
                    String itemShort = GetShortName(itemName);
                    acroShort2LongName[itemShort] = itemName;
                    InverseSearchAdd(inverseSearch, SplitParts(itemShort), itemName);
                }
            }

            /**
            * Gets the mapping from short names to long names. A long 
            * name may contain the #subform name part.
            * @return the mapping from short names to long names
            */
            public Hashtable AcroShort2LongName {
                get {
                    return acroShort2LongName;
                }
                set {
                    acroShort2LongName = value;
                }
            }
        }

        /**
        * Processes the template section in the XFA form.
        */
        public class Xml2SomTemplate : Xml2Som {
            private bool dynamicForm;
            private int templateLevel;
            
            /**
            * Creates a new instance from the datasets node.
            * @param n the template node
            */
            public Xml2SomTemplate(XmlNode n) {
                order = new ArrayList();
                name2Node = new Hashtable();
                stack = new Stack2();
                anform = 0;
                templateLevel = 0;
                inverseSearch = new Hashtable();
                ProcessTemplate(n, null);
            }

            /**
            * Gets the field type as described in the <CODE>template</CODE> section of the XFA.
            * @param s the exact template name
            * @return the field type or <CODE>null</CODE> if not found
            */
            public String GetFieldType(String s) {
                XmlNode n = (XmlNode)name2Node[s];
                if (n == null)
                    return null;
                if (n.LocalName.Equals("exclGroup"))
                    return "exclGroup";
                XmlNode ui = n.FirstChild;
                while (ui != null) {
                    if (ui.NodeType == XmlNodeType.Element && ui.LocalName.Equals("ui")) {
                        break;
                    }
                    ui = ui.NextSibling;
                }
                if (ui == null)
                    return null;
                XmlNode type = ui.FirstChild;
                while (type != null) {
                    if (type.NodeType == XmlNodeType.Element && !(type.LocalName.Equals("extras") && type.LocalName.Equals("picture"))) {
                        return type.LocalName;
                    }
                    type = type.NextSibling;
                }
                return null;
            }

            private void ProcessTemplate(XmlNode n, Hashtable ff) {
                if (ff == null)
                    ff = new Hashtable();
                Hashtable ss = new Hashtable();
                XmlNode n2 = n.FirstChild;
                while (n2 != null) {
                    if (n2.NodeType == XmlNodeType.Element) {
                        String s = n2.LocalName;
                        if (s.Equals("subform")) {
                            XmlNode name = n2.Attributes.GetNamedItem("name");
                            String nn = "#subform";
                            bool annon = true;
                            if (name != null) {
                                nn = EscapeSom(name.Value);
                                annon = false;
                            }
                            int i;
                            if (annon) {
                                i = anform;
                                ++anform;
                            }
                            else {
                                if (ss[nn] == null)
                                    i = 0;
                                else
                                    i = (int)ss[nn] + 1;
                                ss[nn] = i;
                            }
                            stack.Push(nn + "[" + i.ToString() + "]");
                            ++templateLevel;
                            if (annon)
                                ProcessTemplate(n2, ff);
                            else
                                ProcessTemplate(n2, null);
                            --templateLevel;
                            stack.Pop();
                        }
                        else if (s.Equals("field") || s.Equals("exclGroup")) {
                            XmlNode name = n2.Attributes.GetNamedItem("name");
                            if (name != null) {
                                String nn = EscapeSom(name.Value);
                                int i;
                                if (ff[nn] == null)
                                    i = 0;
                                else
                                    i = (int)ff[nn] + 1;
                                ff[nn] = i;
                                stack.Push(nn + "[" + i.ToString() + "]");
                                String unstack = PrintStack();
                                order.Add(unstack);
                                InverseSearchAdd(unstack);
                                name2Node[unstack] = n2;
                                stack.Pop();
                            }
                        }
                        else if (!dynamicForm && templateLevel > 0 && s.Equals("occur")) {
                            int initial = 1;
                            int min = 1;
                            int max = 1;
                            XmlNode a = n2.Attributes.GetNamedItem("initial");
                            if (a != null)
                                try{initial = int.Parse(a.Value.Trim());}catch{};
                            a = n2.Attributes.GetNamedItem("min");
                            if (a != null)
                                try{min = int.Parse(a.Value.Trim());}catch{};
                            a = n2.Attributes.GetNamedItem("max");
                            if (a != null)
                                try{max = int.Parse(a.Value.Trim());}catch{};
                            if (initial != min || min != max)
                                dynamicForm = true;
                        }
                    }
                    n2 = n2.NextSibling;
                }
            }

            /**
            * <CODE>true</CODE> if it's a dynamic form; <CODE>false</CODE>
            * if it's a static form.
            * @return <CODE>true</CODE> if it's a dynamic form; <CODE>false</CODE>
            * if it's a static form
            */
            public bool DynamicForm {
                get {
                    return dynamicForm;
                }
                set {
                    dynamicForm = value;
                }
            }
        }

        /**
        * Gets the class that contains the template processing section of the XFA.
        * @return the class that contains the template processing section of the XFA
        */
        public Xml2SomTemplate TemplateSom {
            get {
                return templateSom;
            }
            set {
                templateSom = value;
            }
        }

        /**
        * Gets the class that contains the datasets processing section of the XFA.
        * @return the class that contains the datasets processing section of the XFA
        */
        public Xml2SomDatasets DatasetsSom {
            get {
                return datasetsSom;
            }
            set {
                datasetsSom = value;
            }
        }

        /**
        * Gets the class that contains the "classic" fields processing.
        * @return the class that contains the "classic" fields processing
        */
        public AcroFieldsSearch AcroFieldsSom {
            get {
                return acroFieldsSom;
            }
            set {
                acroFieldsSom = value;
            }
        }

        /**
        * Gets the <CODE>Node</CODE> that corresponds to the datasets part.
        * @return the <CODE>Node</CODE> that corresponds to the datasets part
        */
        public XmlNode DatasetsNode {
            get {
                return datasetsNode;
            }
        }
    }
}