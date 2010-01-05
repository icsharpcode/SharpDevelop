using System;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.interfaces;

/*
 * $Id: PdfViewerPreferencesImp.cs,v 1.7 2008/05/04 10:49:46 psoares33 Exp $
 *
 * Copyright 2006 Bruno Lowagie
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

namespace iTextSharp.text.pdf.intern {

    /**
    * Stores the information concerning viewer preferences,
    * and contains the business logic that allows you to set viewer preferences.
    */
    public class PdfViewerPreferencesImp : IPdfViewerPreferences {

        public static readonly PdfName[] VIEWER_PREFERENCES = {
                PdfName.HIDETOOLBAR,            // 0
                PdfName.HIDEMENUBAR,            // 1
                PdfName.HIDEWINDOWUI,           // 2
                PdfName.FITWINDOW,              // 3
                PdfName.CENTERWINDOW,           // 4
                PdfName.DISPLAYDOCTITLE,        // 5
                PdfName.NONFULLSCREENPAGEMODE,  // 6
                PdfName.DIRECTION,              // 7
                PdfName.VIEWAREA,               // 8
                PdfName.VIEWCLIP,               // 9
                PdfName.PRINTAREA,              // 10
                PdfName.PRINTCLIP,              // 11
                PdfName.PRINTSCALING,           // 12
                PdfName.DUPLEX,                 // 13
                PdfName.PICKTRAYBYPDFSIZE,      // 14
                PdfName.PRINTPAGERANGE,         // 15
                PdfName.NUMCOPIES               // 16
            };


        /** A series of viewer preferences. */
        public static readonly PdfName[] NONFULLSCREENPAGEMODE_PREFERENCES = {
            PdfName.USENONE, PdfName.USEOUTLINES, PdfName.USETHUMBS, PdfName.USEOC
        };
        /** A series of viewer preferences. */
        public static readonly PdfName[] DIRECTION_PREFERENCES = {
            PdfName.L2R, PdfName.R2L
        };
        /** A series of viewer preferences. */
        public static readonly PdfName[] PAGE_BOUNDARIES = {
            PdfName.MEDIABOX, PdfName.CROPBOX, PdfName.BLEEDBOX, PdfName.TRIMBOX, PdfName.ARTBOX
        };
        /** A series of viewer preferences */
        public static readonly PdfName[] PRINTSCALING_PREFERENCES = {
            PdfName.APPDEFAULT, PdfName.NONE
        };
        /** A series of viewer preferences. */
        public static readonly PdfName[] DUPLEX_PREFERENCES = {
            PdfName.SIMPLEX, PdfName.DUPLEXFLIPSHORTEDGE, PdfName.DUPLEXFLIPLONGEDGE
        };
        /** This value will hold the viewer preferences for the page layout and page mode. */
        private int pageLayoutAndMode = 0;
        
        /** This dictionary holds the viewer preferences (other than page layout and page mode). */
        private PdfDictionary viewerPreferences = new PdfDictionary();
        
        /** The mask to decide if a ViewerPreferences dictionary is needed */
        private const int viewerPreferencesMask = 0xfff000;

        /**
        * Returns the page layout and page mode value.
        */
        public int PageLayoutAndMode {
            get {
                return pageLayoutAndMode;
            }
        }

        /**
        * Returns the viewer preferences.
        */
        public PdfDictionary GetViewerPreferences() {
            return viewerPreferences;
        }
        
        /**
        * Sets the viewer preferences as the sum of several constants.
        * 
        * @param preferences
        *            the viewer preferences
        * @see PdfWriter#setViewerPreferences
        */
        public int ViewerPreferences {
            set {
                int preferences = value;
                this.pageLayoutAndMode |= preferences;
                // for backwards compatibility, it is also possible
                // to set the following viewer preferences with this method:
                if ((preferences & viewerPreferencesMask) != 0) {
                    pageLayoutAndMode = ~viewerPreferencesMask & pageLayoutAndMode;
                    if ((preferences & PdfWriter.HideToolbar) != 0)
                        viewerPreferences.Put(PdfName.HIDETOOLBAR, PdfBoolean.PDFTRUE);
                    if ((preferences & PdfWriter.HideMenubar) != 0)
                        viewerPreferences.Put(PdfName.HIDEMENUBAR, PdfBoolean.PDFTRUE);
                    if ((preferences & PdfWriter.HideWindowUI) != 0)
                        viewerPreferences.Put(PdfName.HIDEWINDOWUI, PdfBoolean.PDFTRUE);
                    if ((preferences & PdfWriter.FitWindow) != 0)
                        viewerPreferences.Put(PdfName.FITWINDOW, PdfBoolean.PDFTRUE);
                    if ((preferences & PdfWriter.CenterWindow) != 0)
                        viewerPreferences.Put(PdfName.CENTERWINDOW, PdfBoolean.PDFTRUE);
                    if ((preferences & PdfWriter.DisplayDocTitle) != 0)
                        viewerPreferences.Put(PdfName.DISPLAYDOCTITLE, PdfBoolean.PDFTRUE);
                    
                    if ((preferences & PdfWriter.NonFullScreenPageModeUseNone) != 0)
                        viewerPreferences.Put(PdfName.NONFULLSCREENPAGEMODE, PdfName.USENONE);
                    else if ((preferences & PdfWriter.NonFullScreenPageModeUseOutlines) != 0)
                        viewerPreferences.Put(PdfName.NONFULLSCREENPAGEMODE, PdfName.USEOUTLINES);
                    else if ((preferences & PdfWriter.NonFullScreenPageModeUseThumbs) != 0)
                        viewerPreferences.Put(PdfName.NONFULLSCREENPAGEMODE, PdfName.USETHUMBS);
                    else if ((preferences & PdfWriter.NonFullScreenPageModeUseOC) != 0)
                        viewerPreferences.Put(PdfName.NONFULLSCREENPAGEMODE, PdfName.USEOC);

                    if ((preferences & PdfWriter.DirectionL2R) != 0)
                        viewerPreferences.Put(PdfName.DIRECTION, PdfName.L2R);
                    else if ((preferences & PdfWriter.DirectionR2L) != 0)
                        viewerPreferences.Put(PdfName.DIRECTION, PdfName.R2L);

                    if ((preferences & PdfWriter.PrintScalingNone) != 0)
                        viewerPreferences.Put(PdfName.PRINTSCALING, PdfName.NONE);          
                }
            }
        }
        
        /**
        * Given a key for a viewer preference (a PdfName object),
        * this method returns the index in the VIEWER_PREFERENCES array.
        * @param key    a PdfName referring to a viewer preference
        * @return   an index in the VIEWER_PREFERENCES array
        */
        private int GetIndex(PdfName key) {
            for (int i = 0; i < VIEWER_PREFERENCES.Length; i++) {
                if (VIEWER_PREFERENCES[i].Equals(key))
                    return i;
            }
            return -1;
        }
        
        /**
        * Checks if some value is valid for a certain key.
        */
        private bool IsPossibleValue(PdfName value, PdfName[] accepted) {
            for (int i = 0; i < accepted.Length; i++) {
                if (accepted[i].Equals(value)) {
                    return true;
                }
            }
            return false;
        }
        
        /**
        * Sets the viewer preferences for printing.
        */
        public virtual void AddViewerPreference(PdfName key, PdfObject value) {
            switch (GetIndex(key)) {
            case 0: // HIDETOOLBAR
            case 1: // HIDEMENUBAR
            case 2: // HIDEWINDOWUI
            case 3: // FITWINDOW
            case 4: // CENTERWINDOW
            case 5: // DISPLAYDOCTITLE
            case 14: // PICKTRAYBYPDFSIZE
                if (value is PdfBoolean) {
                    viewerPreferences.Put(key, value);
                }
                break;
            case 6: // NONFULLSCREENPAGEMODE
                if (value is PdfName
                        && IsPossibleValue((PdfName)value, NONFULLSCREENPAGEMODE_PREFERENCES)) {
                    viewerPreferences.Put(key, value);
                }
                break;
            case 7: // DIRECTION
                if (value is PdfName
                        && IsPossibleValue((PdfName)value, DIRECTION_PREFERENCES)) {
                    viewerPreferences.Put(key, value);
                }
                break;
            case 8:  // VIEWAREA
            case 9:  // VIEWCLIP
            case 10: // PRINTAREA
            case 11: // PRINTCLIP
                if (value is PdfName
                        && IsPossibleValue((PdfName)value, PAGE_BOUNDARIES)) {
                    viewerPreferences.Put(key, value);
                }
                break;
            case 12: // PRINTSCALING
                if (value is PdfName
                        && IsPossibleValue((PdfName)value, PRINTSCALING_PREFERENCES)) {
                    viewerPreferences.Put(key, value);
                }
                break;
            case 13: // DUPLEX
                if (value is PdfName
                        && IsPossibleValue((PdfName)value, DUPLEX_PREFERENCES)) {
                    viewerPreferences.Put(key, value);
                }
                break;
            case 15: // PRINTPAGERANGE
                if (value is PdfArray) {
                    viewerPreferences.Put(key, value);
                }
                break;
            case 16: // NUMCOPIES
                if (value is PdfNumber)  {
                    viewerPreferences.Put(key, value);
                }
                break;
            }
        }

        /**
        * Adds the viewer preferences defined in the preferences parameter to a
        * PdfDictionary (more specifically the root or catalog of a PDF file).
        * 
        * @param catalog
        */
        public void AddToCatalog(PdfDictionary catalog) {
            // Page Layout
            catalog.Remove(PdfName.PAGELAYOUT);
            if ((pageLayoutAndMode & PdfWriter.PageLayoutSinglePage) != 0)
                catalog.Put(PdfName.PAGELAYOUT, PdfName.SINGLEPAGE);
            else if ((pageLayoutAndMode & PdfWriter.PageLayoutOneColumn) != 0)
                catalog.Put(PdfName.PAGELAYOUT, PdfName.ONECOLUMN);
            else if ((pageLayoutAndMode & PdfWriter.PageLayoutTwoColumnLeft) != 0)
                catalog.Put(PdfName.PAGELAYOUT, PdfName.TWOCOLUMNLEFT);
            else if ((pageLayoutAndMode & PdfWriter.PageLayoutTwoColumnRight) != 0)
                catalog.Put(PdfName.PAGELAYOUT, PdfName.TWOCOLUMNRIGHT);
            else if ((pageLayoutAndMode & PdfWriter.PageLayoutTwoPageLeft) != 0)
                catalog.Put(PdfName.PAGELAYOUT, PdfName.TWOPAGELEFT);
            else if ((pageLayoutAndMode & PdfWriter.PageLayoutTwoPageRight) != 0)
                catalog.Put(PdfName.PAGELAYOUT, PdfName.TWOPAGERIGHT);

            // Page Mode
            catalog.Remove(PdfName.PAGEMODE);
            if ((pageLayoutAndMode & PdfWriter.PageModeUseNone) != 0)
                catalog.Put(PdfName.PAGEMODE, PdfName.USENONE);
            else if ((pageLayoutAndMode & PdfWriter.PageModeUseOutlines) != 0)
                catalog.Put(PdfName.PAGEMODE, PdfName.USEOUTLINES);
            else if ((pageLayoutAndMode & PdfWriter.PageModeUseThumbs) != 0)
                catalog.Put(PdfName.PAGEMODE, PdfName.USETHUMBS);
            else if ((pageLayoutAndMode & PdfWriter.PageModeFullScreen) != 0)
                catalog.Put(PdfName.PAGEMODE, PdfName.FULLSCREEN);
            else if ((pageLayoutAndMode & PdfWriter.PageModeUseOC) != 0)
                catalog.Put(PdfName.PAGEMODE, PdfName.USEOC);
            else if ((pageLayoutAndMode & PdfWriter.PageModeUseAttachments) != 0)
                catalog.Put(PdfName.PAGEMODE, PdfName.USEATTACHMENTS);

            // viewer preferences (Table 8.1 of the PDF Reference)
            catalog.Remove(PdfName.VIEWERPREFERENCES);
            if (viewerPreferences.Size > 0) {
                catalog.Put(PdfName.VIEWERPREFERENCES, viewerPreferences);
            }
        }

        public static PdfViewerPreferencesImp GetViewerPreferences(PdfDictionary catalog) {
            PdfViewerPreferencesImp preferences = new PdfViewerPreferencesImp();
            int prefs = 0;
            PdfName name = null;
            // page layout
            PdfObject obj = PdfReader.GetPdfObjectRelease(catalog.Get(PdfName.PAGELAYOUT));
            if (obj != null && obj.IsName()) {
                name = (PdfName) obj;
                if (name.Equals(PdfName.SINGLEPAGE))
                    prefs |= PdfWriter.PageLayoutSinglePage;
                else if (name.Equals(PdfName.ONECOLUMN))
                    prefs |= PdfWriter.PageLayoutOneColumn;
                else if (name.Equals(PdfName.TWOCOLUMNLEFT))
                    prefs |= PdfWriter.PageLayoutTwoColumnLeft;
                else if (name.Equals(PdfName.TWOCOLUMNRIGHT))
                    prefs |= PdfWriter.PageLayoutTwoColumnRight;
                else if (name.Equals(PdfName.TWOPAGELEFT))
                    prefs |= PdfWriter.PageLayoutTwoPageLeft;
                else if (name.Equals(PdfName.TWOPAGERIGHT))
                    prefs |= PdfWriter.PageLayoutTwoPageRight;
            }
            // page mode
            obj = PdfReader.GetPdfObjectRelease(catalog.Get(PdfName.PAGEMODE));
            if (obj != null && obj.IsName()) {
                name = (PdfName) obj;
                if (name.Equals(PdfName.USENONE))
                    prefs |= PdfWriter.PageModeUseNone;
                else if (name.Equals(PdfName.USEOUTLINES))
                    prefs |= PdfWriter.PageModeUseOutlines;
                else if (name.Equals(PdfName.USETHUMBS))
                    prefs |= PdfWriter.PageModeUseThumbs;
                else if (name.Equals(PdfName.USEOC))
                    prefs |= PdfWriter.PageModeUseOC;
                else if (name.Equals(PdfName.USEATTACHMENTS))
                    prefs |= PdfWriter.PageModeUseAttachments;
            }
            // set page layout and page mode preferences
            preferences.ViewerPreferences = prefs;
            // other preferences
            obj = PdfReader.GetPdfObjectRelease(catalog
                    .Get(PdfName.VIEWERPREFERENCES));
            if (obj != null && obj.IsDictionary()) {
                PdfDictionary vp = (PdfDictionary) obj;
                for (int i = 0; i < VIEWER_PREFERENCES.Length; i++) {
                    obj = PdfReader.GetPdfObjectRelease(vp.Get(VIEWER_PREFERENCES[i]));
                    preferences.AddViewerPreference(VIEWER_PREFERENCES[i], obj);
                }
            }
            return preferences;
        }
    }
}