using System;
using System.Collections;
using System.util;
using iTextSharp.text.pdf.collection;

/*
 * $Id: PdfAction.cs,v 1.6 2008/05/13 11:25:18 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 by Bruno Lowagie.
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
     * A <CODE>PdfAction</CODE> defines an action that can be triggered from a PDF file.
     *
     * @see     PdfDictionary
     */

    public class PdfAction : PdfDictionary {
    
        /** A named action to go to the first page.
         */
        public const int FIRSTPAGE = 1;
        /** A named action to go to the previous page.
         */
        public const int PREVPAGE = 2;
        /** A named action to go to the next page.
         */
        public const int NEXTPAGE = 3;
        /** A named action to go to the last page.
         */
        public const int LASTPAGE = 4;

        /** A named action to open a print dialog.
         */
        public const int PRINTDIALOG = 5;

        // constructors
        public const int SUBMIT_EXCLUDE = 1;
        public const int SUBMIT_INCLUDE_NO_VALUE_FIELDS = 2;
        public const int SUBMIT_HTML_FORMAT = 4;
        public const int SUBMIT_HTML_GET = 8;
        public const int SUBMIT_COORDINATES = 16;
        /** a possible submitvalue */
        public const int SUBMIT_XFDF = 32;
        /** a possible submitvalue */
        public const int SUBMIT_INCLUDE_APPEND_SAVES = 64;
        /** a possible submitvalue */
        public const int SUBMIT_INCLUDE_ANNOTATIONS = 128;
        /** a possible submitvalue */
        public const int SUBMIT_PDF = 256;
        /** a possible submitvalue */
        public const int SUBMIT_CANONICAL_FORMAT = 512;
        /** a possible submitvalue */
        public const int SUBMIT_EXCL_NON_USER_ANNOTS = 1024;
        /** a possible submitvalue */
        public const int SUBMIT_EXCL_F_KEY = 2048;
        /** a possible submitvalue */
        public const int SUBMIT_EMBED_FORM = 8196;
        /** a possible submitvalue */
        public const int RESET_EXCLUDE = 1;
    
        /** Create an empty action.
         */    
        public PdfAction() {
        }
    
        /**
         * Constructs a new <CODE>PdfAction</CODE> of Subtype URI.
         *
         * @param url the Url to go to
         */
    
        public PdfAction(Uri url) : this(url.AbsoluteUri) {}
    
        public PdfAction(Uri url, bool isMap) : this(url.AbsoluteUri, isMap) {}
    
        /**
         * Constructs a new <CODE>PdfAction</CODE> of Subtype URI.
         *
         * @param url the url to go to
         */
    
        public PdfAction(string url) : this(url, false) {}
    
        public PdfAction(string url, bool isMap) {
            Put(PdfName.S, PdfName.URI);
            Put(PdfName.URI, new PdfString(url));
            if (isMap)
                Put(PdfName.ISMAP, PdfBoolean.PDFTRUE);
        }
    
        /**
         * Constructs a new <CODE>PdfAction</CODE> of Subtype GoTo.
         * @param destination the destination to go to
         */
    
        internal PdfAction(PdfIndirectReference destination) {
            Put(PdfName.S, PdfName.GOTO);
            Put(PdfName.D, destination);
        }
    
        /**
         * Constructs a new <CODE>PdfAction</CODE> of Subtype GoToR.
         * @param filename the file name to go to
         * @param name the named destination to go to
         */
    
        public PdfAction(string filename, string name) {
            Put(PdfName.S, PdfName.GOTOR);
            Put(PdfName.F, new PdfString(filename));
            Put(PdfName.D, new PdfString(name));
        }
    
        /**
         * Constructs a new <CODE>PdfAction</CODE> of Subtype GoToR.
         * @param filename the file name to go to
         * @param page the page destination to go to
         */
    
        public PdfAction(string filename, int page) {
            Put(PdfName.S, PdfName.GOTOR);
            Put(PdfName.F, new PdfString(filename));
            Put(PdfName.D, new PdfLiteral("[" + (page - 1) + " /FitH 10000]"));
        }
    
        /** Implements name actions. The action can be FIRSTPAGE, LASTPAGE,
         * NEXTPAGE and PREVPAGE.
         * @param named the named action
         */
        public PdfAction(int named) {
            Put(PdfName.S, PdfName.NAMED);
            switch (named) {
                case FIRSTPAGE:
                    Put(PdfName.N, PdfName.FIRSTPAGE);
                    break;
                case LASTPAGE:
                    Put(PdfName.N, PdfName.LASTPAGE);
                    break;
                case NEXTPAGE:
                    Put(PdfName.N, PdfName.NEXTPAGE);
                    break;
                case PREVPAGE:
                    Put(PdfName.N, PdfName.PREVPAGE);
                    break;
                case PRINTDIALOG:
                    Put(PdfName.S, PdfName.JAVASCRIPT);
                    Put(PdfName.JS, new PdfString("this.print(true);\r"));
                    break;
                default:
                    throw new ArgumentException("Invalid named action.");
            }
        }
    
        /** Launchs an application or a document.
         * @param application the application to be launched or the document to be opened or printed.
         * @param parameters (Windows-specific) A parameter string to be passed to the application.
         * It can be <CODE>null</CODE>.
         * @param operation (Windows-specific) the operation to perform: "open" - Open a document,
         * "print" - Print a document.
         * It can be <CODE>null</CODE>.
         * @param defaultDir (Windows-specific) the default directory in standard DOS syntax.
         * It can be <CODE>null</CODE>.
         */
        public PdfAction(string application, string parameters, string operation, string defaultDir) {
            Put(PdfName.S, PdfName.LAUNCH);
            if (parameters == null && operation == null && defaultDir == null)
                Put(PdfName.F, new PdfString(application));
            else {
                PdfDictionary dic = new PdfDictionary();
                dic.Put(PdfName.F, new PdfString(application));
                if (parameters != null)
                    dic.Put(PdfName.P, new PdfString(parameters));
                if (operation != null)
                    dic.Put(PdfName.O, new PdfString(operation));
                if (defaultDir != null)
                    dic.Put(PdfName.D, new PdfString(defaultDir));
                Put(PdfName.WIN, dic);
            }
        }
    
        /** Launchs an application or a document.
        * @param application the application to be launched or the document to be opened or printed.
        * @param parameters (Windows-specific) A parameter string to be passed to the application.
        * It can be <CODE>null</CODE>.
        * @param operation (Windows-specific) the operation to perform: "open" - Open a document,
        * "print" - Print a document.
        * It can be <CODE>null</CODE>.
        * @param defaultDir (Windows-specific) the default directory in standard DOS syntax.
        * It can be <CODE>null</CODE>.
        * @return a Launch action
        */
        public static PdfAction CreateLaunch(String application, String parameters, String operation, String defaultDir) {
            return new PdfAction(application, parameters, operation, defaultDir);
        }
        
        /**Creates a Rendition action
        * @param file
        * @param fs
        * @param mimeType
        * @param ref
        * @return a Media Clip action
        * @throws IOException
        */
        public static PdfAction Rendition(String file, PdfFileSpecification fs, String mimeType, PdfIndirectReference refi) {
            PdfAction js = new PdfAction();
            js.Put(PdfName.S, PdfName.RENDITION);
            js.Put(PdfName.R, new PdfRendition(file, fs, mimeType));
            js.Put(new PdfName("OP"), new PdfNumber(0));
            js.Put(new PdfName("AN"), refi);
            return js;
        }

        /** Creates a JavaScript action. If the JavaScript is smaller than
         * 50 characters it will be placed as a string, otherwise it will
         * be placed as a compressed stream.
         * @param code the JavaScript code
         * @param writer the writer for this action
         * @param unicode select JavaScript unicode. Note that the internal
         * Acrobat JavaScript engine does not support unicode,
         * so this may or may not work for you
         * @return the JavaScript action
         */    
        public static PdfAction JavaScript(string code, PdfWriter writer, bool unicode) {
            PdfAction js = new PdfAction();
            js.Put(PdfName.S, PdfName.JAVASCRIPT);
            if (unicode && code.Length < 50) {
                js.Put(PdfName.JS, new PdfString(code, PdfObject.TEXT_UNICODE));
            }
            else if (!unicode && code.Length < 100) {
                js.Put(PdfName.JS, new PdfString(code));
            }
            else {
                byte[] b = PdfEncodings.ConvertToBytes(code, unicode ? PdfObject.TEXT_UNICODE : PdfObject.TEXT_PDFDOCENCODING);
                PdfStream stream = new PdfStream(b);
                stream.FlateCompress();
                js.Put(PdfName.JS, writer.AddToBody(stream).IndirectReference);
            }
            return js;
        }

        /** Creates a JavaScript action. If the JavaScript is smaller than
         * 50 characters it will be place as a string, otherwise it will
         * be placed as a compressed stream.
         * @param code the JavaScript code
         * @param writer the writer for this action
         * @return the JavaScript action
         */    
        public static PdfAction JavaScript(string code, PdfWriter writer) {
            return JavaScript(code, writer, false);
        }
    
        internal static PdfAction CreateHide(PdfObject obj, bool hide) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.HIDE);
            action.Put(PdfName.T, obj);
            if (!hide)
                action.Put(PdfName.H, PdfBoolean.PDFFALSE);
            return action;
        }
    
        public static PdfAction CreateHide(PdfAnnotation annot, bool hide) {
            return CreateHide(annot.IndirectReference, hide);
        }
    
        public static PdfAction CreateHide(string name, bool hide) {
            return CreateHide(new PdfString(name), hide);
        }
    
        internal static PdfArray BuildArray(Object[] names) {
            PdfArray array = new PdfArray();
            for (int k = 0; k < names.Length; ++k) {
                Object obj = names[k];
                if (obj is string)
                    array.Add(new PdfString((string)obj));
                else if (obj is PdfAnnotation)
                    array.Add(((PdfAnnotation)obj).IndirectReference);
                else
                    throw new ArgumentException("The array must contain string or PdfAnnotation.");
            }
            return array;
        }
    
        public static PdfAction CreateHide(Object[] names, bool hide) {
            return CreateHide(BuildArray(names), hide);
        }
    
        public static PdfAction CreateSubmitForm(string file, Object[] names, int flags) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.SUBMITFORM);
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.F, new PdfString(file));
            dic.Put(PdfName.FS, PdfName.URL);
            action.Put(PdfName.F, dic);
            if (names != null)
                action.Put(PdfName.FIELDS, BuildArray(names));
            action.Put(PdfName.FLAGS, new PdfNumber(flags));
            return action;
        }
    
        public static PdfAction CreateResetForm(Object[] names, int flags) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.RESETFORM);
            if (names != null)
                action.Put(PdfName.FIELDS, BuildArray(names));
            action.Put(PdfName.FLAGS, new PdfNumber(flags));
            return action;
        }
    
        public static PdfAction CreateImportData(string file) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.IMPORTDATA);
            action.Put(PdfName.F, new PdfString(file));
            return action;
        }
    
        /** Add a chained action.
         * @param na the next action
         */    
        public void Next(PdfAction na) {
            PdfObject nextAction = Get(PdfName.NEXT);
            if (nextAction == null)
                Put(PdfName.NEXT, na);
            else if (nextAction.IsDictionary()) {
                PdfArray array = new PdfArray(nextAction);
                array.Add(na);
                Put(PdfName.NEXT, array);
            }
            else {
                ((PdfArray)nextAction).Add(na);
            }
        }
    
        /** Creates a GoTo action to an internal page.
         * @param page the page to go. First page is 1
         * @param dest the destination for the page
         * @param writer the writer for this action
         * @return a GoTo action
         */    
        public static PdfAction GotoLocalPage(int page, PdfDestination dest, PdfWriter writer) {
            PdfIndirectReference piref = writer.GetPageReference(page);
            dest.AddPage(piref);
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.GOTO);
            action.Put(PdfName.D, dest);
            return action;
        }

        /**
        * Creates a GoTo action to a named destination.
        * @param dest the named destination
        * @param isName if true sets the destination as a name, if false sets it as a String
        * @return a GoToR action
        */
        public static PdfAction GotoLocalPage(String dest, bool isName) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.GOTO);
            if (isName)
                action.Put(PdfName.D, new PdfName(dest));
            else
                action.Put(PdfName.D, new PdfString(dest, null));
            return action;
        }

        /**
        * Creates a GoToR action to a named destination.
        * @param filename the file name to go to
        * @param dest the destination name
        * @param isName if true sets the destination as a name, if false sets it as a String
        * @param newWindow open the document in a new window if <CODE>true</CODE>, if false the current document is replaced by the new document.
        * @return a GoToR action
        */
        public static PdfAction GotoRemotePage(String filename, String dest, bool isName, bool newWindow) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.F, new PdfString(filename));
            action.Put(PdfName.S, PdfName.GOTOR);
            if (isName)
                action.Put(PdfName.D, new PdfName(dest));
            else
                action.Put(PdfName.D, new PdfString(dest, null));
            if (newWindow)
                action.Put(PdfName.NEWWINDOW, PdfBoolean.PDFTRUE);
            return action;
        }

        /**
        * Creates a GoToE action to an embedded file.
        * @param filename   the root document of the target (null if the target is in the same document)
        * @param dest the named destination
        * @param isName if true sets the destination as a name, if false sets it as a String
        * @return a GoToE action
        */
        public static PdfAction GotoEmbedded(String filename, PdfTargetDictionary target, String dest, bool isName, bool newWindow) {
            if (isName)
                return GotoEmbedded(filename, target, new PdfName(dest), newWindow);
            else
                return GotoEmbedded(filename, target, new PdfString(dest, null), newWindow);
        }

        /**
        * Creates a GoToE action to an embedded file.
        * @param filename   the root document of the target (null if the target is in the same document)
        * @param target a path to the target document of this action
        * @param dest       the destination inside the target document, can be of type PdfDestination, PdfName, or PdfString
        * @param newWindow  if true, the destination document should be opened in a new window
        * @return a GoToE action
        */
        public static PdfAction GotoEmbedded(String filename, PdfTargetDictionary target, PdfObject dest, bool newWindow) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.GOTOE);
            action.Put(PdfName.T, target);
            action.Put(PdfName.D, dest);
            action.Put(PdfName.NEWWINDOW, new PdfBoolean(newWindow));
            if (filename != null) {
                action.Put(PdfName.F, new PdfString(filename));
            }
            return action;
        }

        /**
        * A set-OCG-state action (PDF 1.5) sets the state of one or more optional content
        * groups.
        * @param state an array consisting of any number of sequences beginning with a <CODE>PdfName</CODE>
        * or <CODE>String</CODE> (ON, OFF, or Toggle) followed by one or more optional content group dictionaries
        * <CODE>PdfLayer</CODE> or a <CODE>PdfIndirectReference</CODE> to a <CODE>PdfLayer</CODE>.<br>
        * The array elements are processed from left to right; each name is applied
        * to the subsequent groups until the next name is encountered:
        * <ul>
        * <li>ON sets the state of subsequent groups to ON</li>
        * <li>OFF sets the state of subsequent groups to OFF</li>
        * <li>Toggle reverses the state of subsequent groups</li>
        * </ul>
        * @param preserveRB if <CODE>true</CODE>, indicates that radio-button state relationships between optional
        * content groups (as specified by the RBGroups entry in the current configuration
        * dictionary) should be preserved when the states in the
        * <CODE>state</CODE> array are applied. That is, if a group is set to ON (either by ON or Toggle) during
        * processing of the <CODE>state</CODE> array, any other groups belong to the same radio-button
        * group are turned OFF. If a group is set to OFF, there is no effect on other groups.<br>
        * If <CODE>false</CODE>, radio-button state relationships, if any, are ignored
        * @return the action
        */    
        public static PdfAction SetOCGstate(ArrayList state, bool preserveRB) {
            PdfAction action = new PdfAction();
            action.Put(PdfName.S, PdfName.SETOCGSTATE);
            PdfArray a = new PdfArray();
            for (int k = 0; k < state.Count; ++k) {
                Object o = state[k];
                if (o == null)
                    continue;
                if (o is PdfIndirectReference)
                    a.Add((PdfIndirectReference)o);
                else if (o is PdfLayer)
                    a.Add(((PdfLayer)o).Ref);
                else if (o is PdfName)
                    a.Add((PdfName)o);
                else if (o is String) {
                    PdfName name = null;
                    String s = (String)o;
                    if (Util.EqualsIgnoreCase(s, "on"))
                        name = PdfName.ON;
                    else if (Util.EqualsIgnoreCase(s, "off"))
                        name = PdfName.OFF;
                    else if (Util.EqualsIgnoreCase(s, "toggle"))
                        name = PdfName.TOGGLE;
                    else
                        throw new ArgumentException("A string '" + s + " was passed in state. Only 'ON', 'OFF' and 'Toggle' are allowed.");
                    a.Add(name);
                }
                else
                    throw new ArgumentException("Invalid type was passed in state: " + o.GetType().ToString());
            }
            action.Put(PdfName.STATE, a);
            if (!preserveRB)
                action.Put(PdfName.PRESERVERB, PdfBoolean.PDFFALSE);
            return action;
        }
    }
}
