using System;
using System.Text;

/*
 * $Id: PdfName.cs,v 1.31 2008/05/24 18:41:23 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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
     * <CODE>PdfName</CODE> is an object that can be used as a name in a PDF-file.
     * <P>
     * A name, like a string, is a sequence of characters. It must begin with a slash
     * followed by a sequence of ASCII characters in the range 32 through 136 except
     * %, (, ), [, ], &lt;, &gt;, {, }, / and #. Any character except 0x00 may be included
     * in a name by writing its twocharacter hex code, preceded by #. The maximum number
     * of characters in a name is 127.<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.5 (page 39-40).
     * <P>
     *
     * @see        PdfObject
     * @see        PdfDictionary
     * @see        BadPdfFormatException
     */

    public class PdfName : PdfObject, IComparable {
    
        // static membervariables (a variety of standard names used in PDF)
    
        /** A name */
        public static readonly PdfName A = new PdfName("A");    
        /** A name */
        public static readonly PdfName AA = new PdfName("AA");
        /** A name */
        public static readonly PdfName ABSOLUTECALORIMETRIC = new PdfName("AbsoluteColorimetric");
        /** A name */
        public static readonly PdfName AC = new PdfName("AC");
        /** A name */
        public static readonly PdfName ACROFORM = new PdfName("AcroForm");
        /** A name */
        public static readonly PdfName ACTION = new PdfName("Action");
        /** A name */
        public static readonly PdfName ADBE_PKCS7_DETACHED = new PdfName("adbe.pkcs7.detached");
        /** A name */
        public static readonly PdfName ADBE_PKCS7_S4 =new PdfName("adbe.pkcs7.s4");
        /** A name */
        public static readonly PdfName ADBE_PKCS7_S5 =new PdfName("adbe.pkcs7.s5");
        /** A name */
        public static readonly PdfName ADBE_PKCS7_SHA1 = new PdfName("adbe.pkcs7.sha1");
        /** A name */
        public static readonly PdfName ADBE_X509_RSA_SHA1 = new PdfName("adbe.x509.rsa_sha1");
        /** A name */
        public static readonly PdfName ADOBE_PPKLITE = new PdfName("Adobe.PPKLite");
        /** A name */
        public static readonly PdfName ADOBE_PPKMS = new PdfName("Adobe.PPKMS");
        /** A name */
        public static readonly PdfName AESV2 = new PdfName("AESV2");
        /** A name */
        public static readonly PdfName AIS = new PdfName("AIS");
        /** A name */
        public static readonly PdfName ALLPAGES = new PdfName("AllPages");
        /** A name */
        public static readonly PdfName ALT = new PdfName("Alt");
        /** A name */
        public static readonly PdfName ALTERNATE = new PdfName("Alternate");
        /** A name */
        public static readonly PdfName ANNOT = new PdfName("Annot");
        /** A name */
        public static readonly PdfName ANTIALIAS = new PdfName("AntiAlias");
        /** A name */
        public static readonly PdfName ANNOTS = new PdfName("Annots");
        /** A name */
        public static readonly PdfName AP = new PdfName("AP");
        /** A name */
        public static readonly PdfName APPDEFAULT = new PdfName("AppDefault");
        /** A name */
        public static readonly PdfName ARTBOX = new PdfName("ArtBox");
        /** A name */
        public static readonly PdfName ASCENT = new PdfName("Ascent");
        /** A name */
        public static readonly PdfName AS = new PdfName("AS");
        /** A name */
        public static readonly PdfName ASCII85DECODE = new PdfName("ASCII85Decode");
        /** A name */
        public static readonly PdfName ASCIIHEXDECODE = new PdfName("ASCIIHexDecode");
        /** A name */
        public static readonly PdfName AUTHEVENT = new PdfName("AuthEvent");
        /** A name */
        public static readonly PdfName AUTHOR = new PdfName("Author");
        /** A name */
        public static readonly PdfName B = new PdfName("B");
        /** A name */
        public static readonly PdfName BASEENCODING = new PdfName("BaseEncoding");
        /** A name */
        public static readonly PdfName BASEFONT = new PdfName("BaseFont");
        /** A name */
        public static readonly PdfName BBOX = new PdfName("BBox");
        /** A name */
        public static readonly PdfName BC = new PdfName("BC");
        /** A name */
        public static readonly PdfName BG = new PdfName("BG");
        /** A name */
        public static readonly PdfName BIGFIVE = new PdfName("BigFive");
        /** A name */
        public static readonly PdfName BITSPERCOMPONENT = new PdfName("BitsPerComponent");
        /** A name */
        public static readonly PdfName BITSPERSAMPLE = new PdfName("BitsPerSample");
        /** A name */
        public static readonly PdfName BL = new PdfName("Bl");
        /** A name */
        public static readonly PdfName BLACKIS1 = new PdfName("BlackIs1");
        /** A name */
        public static readonly PdfName BLACKPOINT = new PdfName("BlackPoint");
        /** A name */
        public static readonly PdfName BLEEDBOX = new PdfName("BleedBox");
        /** A name */
        public static readonly PdfName BLINDS = new PdfName("Blinds");
        /** A name */
        public static readonly PdfName BM = new PdfName("BM");
        /** A name */
        public static readonly PdfName BORDER = new PdfName("Border");
        /** A name */
        public static readonly PdfName BOUNDS = new PdfName("Bounds");
        /** A name */
        public static readonly PdfName BOX = new PdfName("Box");
        /** A name */
        public static readonly PdfName BS = new PdfName("BS");
        /** A name */
        public static readonly PdfName BTN = new PdfName("Btn");
        /** A name */
        public static readonly PdfName BYTERANGE = new PdfName("ByteRange");
        /** A name */
        public static readonly PdfName C = new PdfName("C");
        /** A name */
        public static readonly PdfName C0 = new PdfName("C0");
        /** A name */
        public static readonly PdfName C1 = new PdfName("C1");
        /** A name */
        public static readonly PdfName CA = new PdfName("CA");
        /** A name */
        public static readonly PdfName ca_ = new PdfName("ca");
        /** A name */
        public static readonly PdfName CALGRAY = new PdfName("CalGray");
        /** A name */
        public static readonly PdfName CALRGB = new PdfName("CalRGB");
        /** A name */
        public static readonly PdfName CAPHEIGHT = new PdfName("CapHeight");
        /** A name */
        public static readonly PdfName CATALOG = new PdfName("Catalog");
        /** A name */
        public static readonly PdfName CATEGORY = new PdfName("Category");
        /** A name */
        public static readonly PdfName CCITTFAXDECODE = new PdfName("CCITTFaxDecode");
        /** A name */
        public static readonly PdfName CENTERWINDOW = new PdfName("CenterWindow");
        /** A name */
        public static readonly PdfName CERT = new PdfName("Cert");
        /** A name */
        public static readonly PdfName CF = new PdfName("CF");
        /** A name */
        public static readonly PdfName CFM = new PdfName("CFM");
        /** A name */
        public static readonly PdfName CH = new PdfName("Ch");
        /** A name */
        public static readonly PdfName CHARPROCS = new PdfName("CharProcs");
        /** A name */
        public static readonly PdfName CI = new PdfName("CI");
        /** A name */
        public static readonly PdfName CIDFONTTYPE0 = new PdfName("CIDFontType0");
        /** A name */
        public static readonly PdfName CIDFONTTYPE2 = new PdfName("CIDFontType2");
        /** A name */
        public static readonly PdfName CIDSET = new PdfName("CIDSet");
        /** A name */
        public static readonly PdfName CIDSYSTEMINFO = new PdfName("CIDSystemInfo");
        /** A name */
        public static readonly PdfName CIDTOGIDMAP = new PdfName("CIDToGIDMap");
        /** A name */
        public static readonly PdfName CIRCLE = new PdfName("Circle");
        /** A name */
        public static readonly PdfName CO = new PdfName("CO");
        /** A name */
        public static readonly PdfName COLORS = new PdfName("Colors");
        /** A name */
        public static readonly PdfName COLORSPACE = new PdfName("ColorSpace");
        /** A name */
        public static readonly PdfName COLLECTION = new PdfName("Collection");
        /** A name */
        public static readonly PdfName COLLECTIONFIELD = new PdfName("CollectionField");
        /** A name */
        public static readonly PdfName COLLECTIONITEM = new PdfName("CollectionItem");
        /** A name */
        public static readonly PdfName COLLECTIONSCHEMA = new PdfName("CollectionSchema");
        /** A name */
        public static readonly PdfName COLLECTIONSORT = new PdfName("CollectionSort");
        /** A name */
        public static readonly PdfName COLLECTIONSUBITEM = new PdfName("CollectionSubitem");
        /** A name */
        public static readonly PdfName COLUMNS = new PdfName("Columns");
        /** A name */
        public static readonly PdfName CONTACTINFO = new PdfName("ContactInfo");
        /** A name */
        public static readonly PdfName CONTENT = new PdfName("Content");
        /** A name */
        public static readonly PdfName CONTENTS = new PdfName("Contents");
        /** A name */
        public static readonly PdfName COORDS = new PdfName("Coords");
        /** A name */
        public static readonly PdfName COUNT = new PdfName("Count");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName COURIER = new PdfName("Courier");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName COURIER_BOLD = new PdfName("Courier-Bold");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName COURIER_OBLIQUE = new PdfName("Courier-Oblique");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName COURIER_BOLDOBLIQUE = new PdfName("Courier-BoldOblique");
        /** A name */
        public static readonly PdfName CREATIONDATE = new PdfName("CreationDate");
        /** A name */
        public static readonly PdfName CREATOR = new PdfName("Creator");
        /** A name */
        public static readonly PdfName CREATORINFO = new PdfName("CreatorInfo");
        /** A name */
        public static readonly PdfName CROPBOX = new PdfName("CropBox");
        /** A name */
        public static readonly PdfName CRYPT = new PdfName("Crypt");
        /** A name */
        public static readonly PdfName CS = new PdfName("CS");
        /** A name */
        public static readonly PdfName D = new PdfName("D");
        /** A name */
        public static readonly PdfName DA = new PdfName("DA");
        /** A name */
        public static readonly PdfName DATA = new PdfName("Data");
        /** A name */
        public static readonly PdfName DC = new PdfName("DC");
        /** A name */
        public static readonly PdfName DCTDECODE = new PdfName("DCTDecode");
        /** A name */
        public static readonly PdfName DECODE = new PdfName("Decode");
        /** A name */
        public static readonly PdfName DECODEPARMS = new PdfName("DecodeParms");
        /** A name */
        public static readonly PdfName DEFAULTCRYPTFILER = new PdfName("DefaultCryptFilter");
        /** A name */
        public static readonly PdfName DEFAULTCMYK = new PdfName("DefaultCMYK");
        /** A name */
        public static readonly PdfName DEFAULTGRAY = new PdfName("DefaultGray");
        /** A name */
        public static readonly PdfName DEFAULTRGB = new PdfName("DefaultRGB");
        /** A name */
        public static readonly PdfName DESC = new PdfName("Desc");
        /** A name */
        public static readonly PdfName DESCENDANTFONTS = new PdfName("DescendantFonts");
        /** A name */
        public static readonly PdfName DESCENT = new PdfName("Descent");
        /** A name */
        public static readonly PdfName DEST = new PdfName("Dest");
        /** A name */
        public static readonly PdfName DESTOUTPUTPROFILE = new PdfName("DestOutputProfile");
        /** A name */
        public static readonly PdfName DESTS = new PdfName("Dests");
        /** A name */
        public static readonly PdfName DEVICEGRAY = new PdfName("DeviceGray");
        /** A name */
        public static readonly PdfName DEVICERGB = new PdfName("DeviceRGB");
        /** A name */
        public static readonly PdfName DEVICECMYK = new PdfName("DeviceCMYK");
        /** A name */
        public static readonly PdfName DI = new PdfName("Di");
        /** A name */
        public static readonly PdfName DIFFERENCES = new PdfName("Differences");
        /** A name */
        public static readonly PdfName DISSOLVE = new PdfName("Dissolve");
        /** A name */
        public static readonly PdfName DIRECTION = new PdfName("Direction");
        /** A name */
        public static readonly PdfName DISPLAYDOCTITLE = new PdfName("DisplayDocTitle");
        /** A name */
        public static readonly PdfName DIV = new PdfName("Div");
        /** A name */
        public static readonly PdfName DM = new PdfName("Dm");
        /** A name */
        public static readonly PdfName DOCMDP = new PdfName("DocMDP");
        /** A name */
        public static readonly PdfName DOCOPEN = new PdfName("DocOpen");
        /** A name */
        public static readonly PdfName DOMAIN = new PdfName("Domain");
        /** A name */
        public static readonly PdfName DP = new PdfName("DP");
        /** A name */
        public static readonly PdfName DR = new PdfName("DR");
        /** A name */
        public static readonly PdfName DS = new PdfName("DS");
        /** A name */
        public static readonly PdfName DUR = new PdfName("Dur");
        /** A name */
        public static readonly PdfName DUPLEX = new PdfName("Duplex");
        /** A name */
        public static readonly PdfName DUPLEXFLIPSHORTEDGE = new PdfName("DuplexFlipShortEdge");
        /** A name */
        public static readonly PdfName DUPLEXFLIPLONGEDGE = new PdfName("DuplexFlipLongEdge");
        /** A name */
        public static readonly PdfName DV = new PdfName("DV");
        /** A name */
        public static readonly PdfName DW = new PdfName("DW");
        /** A name */
        public static readonly PdfName E = new PdfName("E");
        /** A name */
        public static readonly PdfName EARLYCHANGE = new PdfName("EarlyChange");
        /** A name */
        public static readonly PdfName EF = new PdfName("EF");
        /** A name */
        public static readonly PdfName EMBEDDEDFILE = new PdfName("EmbeddedFile");
        /** A name */
        public static readonly PdfName EMBEDDEDFILES = new PdfName("EmbeddedFiles");
        /** A name */
        public static readonly PdfName ENCODE = new PdfName("Encode");
        /** A name */
        public static readonly PdfName ENCODEDBYTEALIGN = new PdfName("EncodedByteAlign");
        /** A name */
        public static readonly PdfName ENCODING = new PdfName("Encoding");
        /** A name */
        public static readonly PdfName ENCRYPT = new PdfName("Encrypt");
        /** A name */
        public static readonly PdfName ENCRYPTMETADATA = new PdfName("EncryptMetadata");
        /** A name */
        public static readonly PdfName ENDOFBLOCK = new PdfName("EndOfBlock");
        /** A name */
        public static readonly PdfName ENDOFLINE = new PdfName("EndOfLine");
        /** A name */
        public static readonly PdfName EXTEND = new PdfName("Extend");
        /** A name */
        public static readonly PdfName EXTGSTATE = new PdfName("ExtGState");
        /** A name */
        public static readonly PdfName EXPORT = new PdfName("Export");
        /** A name */
        public static readonly PdfName EXPORTSTATE = new PdfName("ExportState");
        /** A name */
        public static readonly PdfName EVENT = new PdfName("Event");
        /** A name */
        public static readonly PdfName F = new PdfName("F");
        /** A name */
        public static readonly PdfName FB = new PdfName("FB");
        /** A name */
        public static readonly PdfName FDECODEPARMS = new PdfName("FDecodeParms");
        /** A name */
        public static readonly PdfName FDF = new PdfName("FDF");
        /** A name */
        public static readonly PdfName FF = new PdfName("Ff");
        /** A name */
        public static readonly PdfName FFILTER = new PdfName("FFilter");
        /** A name */
        public static readonly PdfName FIELDS = new PdfName("Fields");
        /** A name */
        public static readonly PdfName FILEATTACHMENT = new PdfName("FileAttachment");
        /** A name */
        public static readonly PdfName FILESPEC = new PdfName("Filespec");
        /** A name */
        public static readonly PdfName FILTER = new PdfName("Filter");
        /** A name */
        public static readonly PdfName FIRST = new PdfName("First");
        /** A name */
        public static readonly PdfName FIRSTCHAR = new PdfName("FirstChar");
        /** A name */
        public static readonly PdfName FIRSTPAGE = new PdfName("FirstPage");
        /** A name */
        public static readonly PdfName FIT = new PdfName("Fit");
        /** A name */
        public static readonly PdfName FITH = new PdfName("FitH");
        /** A name */
        public static readonly PdfName FITV = new PdfName("FitV");
        /** A name */
        public static readonly PdfName FITR = new PdfName("FitR");
        /** A name */
        public static readonly PdfName FITB = new PdfName("FitB");
        /** A name */
        public static readonly PdfName FITBH = new PdfName("FitBH");
        /** A name */
        public static readonly PdfName FITBV = new PdfName("FitBV");
        /** A name */
        public static readonly PdfName FITWINDOW = new PdfName("FitWindow");
        /** A name */
        public static readonly PdfName FLAGS = new PdfName("Flags");
        /** A name */
        public static readonly PdfName FLATEDECODE = new PdfName("FlateDecode");
        /** A name */
        public static readonly PdfName FO = new PdfName("Fo");
        /** A name */
        public static readonly PdfName FONT = new PdfName("Font");
        /** A name */
        public static readonly PdfName FONTBBOX = new PdfName("FontBBox");
        /** A name */
        public static readonly PdfName FONTDESCRIPTOR = new PdfName("FontDescriptor");
        /** A name */
        public static readonly PdfName FONTFILE = new PdfName("FontFile");
        /** A name */
        public static readonly PdfName FONTFILE2 = new PdfName("FontFile2");
        /** A name */
        public static readonly PdfName FONTFILE3 = new PdfName("FontFile3");
        /** A name */
        public static readonly PdfName FONTMATRIX = new PdfName("FontMatrix");
        /** A name */
        public static readonly PdfName FONTNAME = new PdfName("FontName");
        /** A name */
        public static readonly PdfName FORM = new PdfName("Form");
        /** A name */
        public static readonly PdfName FORMTYPE = new PdfName("FormType");
        /** A name */
        public static readonly PdfName FREETEXT = new PdfName("FreeText");
        /** A name */
        public static readonly PdfName FRM = new PdfName("FRM");
        /** A name */
        public static readonly PdfName FS = new PdfName("FS");
        /** A name */
        public static readonly PdfName FT = new PdfName("FT");
        /** A name */
        public static readonly PdfName FULLSCREEN = new PdfName("FullScreen");
        /** A name */
        public static readonly PdfName FUNCTION = new PdfName("Function");
        /** A name */
        public static readonly PdfName FUNCTIONS = new PdfName("Functions");
        /** A name */
        public static readonly PdfName FUNCTIONTYPE = new PdfName("FunctionType");
        /** A name of an attribute. */
        public static readonly PdfName GAMMA = new PdfName("Gamma");
        /** A name of an attribute. */
        public static readonly PdfName GBK = new PdfName("GBK");
        /** A name of an attribute. */
        public static readonly PdfName GLITTER = new PdfName("Glitter");
        /** A name of an attribute. */
        public static readonly PdfName GOTO = new PdfName("GoTo");
        /** A name of an attribute. */
        public static readonly PdfName GOTOE = new PdfName("GoToE");
        /** A name of an attribute. */
        public static readonly PdfName GOTOR = new PdfName("GoToR");
        /** A name of an attribute. */
        public static readonly PdfName GROUP = new PdfName("Group");
        /** A name of an attribute. */
        public static readonly PdfName GTS_PDFA1 = new PdfName("GTS_PDFA1");
        /** A name of an attribute. */
        public static readonly PdfName GTS_PDFX = new PdfName("GTS_PDFX");
        /** A name of an attribute. */
        public static readonly PdfName GTS_PDFXVERSION = new PdfName("GTS_PDFXVersion");
        /** A name of an attribute. */
        public static readonly PdfName H = new PdfName("H");
        /** A name of an attribute. */
        public static readonly PdfName HEIGHT = new PdfName("Height");
        /** A name */
        public static readonly PdfName HELV = new PdfName("Helv");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName HELVETICA = new PdfName("Helvetica");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName HELVETICA_BOLD = new PdfName("Helvetica-Bold");
        /** This is a static PdfName PdfName of a base 14 type 1 font */
        public static readonly PdfName HELVETICA_OBLIQUE = new PdfName("Helvetica-Oblique");
        /** This is a static PdfName PdfName of a base 14 type 1 font */
        public static readonly PdfName HELVETICA_BOLDOBLIQUE = new PdfName("Helvetica-BoldOblique");
        /** A name */
        public static readonly PdfName HID = new PdfName("Hid");
        /** A name */
        public static readonly PdfName HIDE = new PdfName("Hide");
        /** A name */
        public static readonly PdfName HIDEMENUBAR = new PdfName("HideMenubar");
        /** A name */
        public static readonly PdfName HIDETOOLBAR = new PdfName("HideToolbar");
        /** A name */
        public static readonly PdfName HIDEWINDOWUI = new PdfName("HideWindowUI");
        /** A name */
        public static readonly PdfName HIGHLIGHT = new PdfName("Highlight");
        /** A name */
        public static readonly PdfName I = new PdfName("I");
        /** A name */
        public static readonly PdfName ICCBASED = new PdfName("ICCBased");
        /** A name */
        public static readonly PdfName ID = new PdfName("ID");
        /** A name */
        public static readonly PdfName IDENTITY = new PdfName("Identity");
        /** A name */
        public static readonly PdfName IF = new PdfName("IF");
        /** A name */
        public static readonly PdfName IMAGE = new PdfName("Image");
        /** A name */
        public static readonly PdfName IMAGEB = new PdfName("ImageB");
        /** A name */
        public static readonly PdfName IMAGEC = new PdfName("ImageC");
        /** A name */
        public static readonly PdfName IMAGEI = new PdfName("ImageI");
        /** A name */
        public static readonly PdfName IMAGEMASK = new PdfName("ImageMask");
        /** A name */
        public static readonly PdfName INDEX = new PdfName("Index");
        /** A name */
        public static readonly PdfName INDEXED = new PdfName("Indexed");
        /** A name */
        public static readonly PdfName INFO = new PdfName("Info");
        /** A name */
        public static readonly PdfName INK = new PdfName("Ink");
        /** A name */
        public static readonly PdfName INKLIST = new PdfName("InkList");
        /** A name */
        public static readonly PdfName IMPORTDATA = new PdfName("ImportData");
        /** A name */
        public static readonly PdfName INTENT = new PdfName("Intent");
        /** A name */
        public static readonly PdfName INTERPOLATE = new PdfName("Interpolate");
        /** A name */
        public static readonly PdfName ISMAP = new PdfName("IsMap");
        /** A name */
        public static readonly PdfName IRT = new PdfName("IRT");
        /** A name */
        public static readonly PdfName ITALICANGLE = new PdfName("ItalicAngle");
        /** A name */
        public static readonly PdfName IX = new PdfName("IX");
        /** A name */
        public static readonly PdfName JAVASCRIPT = new PdfName("JavaScript");
        /** A name */
        public static readonly PdfName JPXDECODE = new PdfName("JPXDecode");
        /** A name */
        public static readonly PdfName JS = new PdfName("JS");
        /** A name */
        public static readonly PdfName K = new PdfName("K");
        /** A name */
        public static readonly PdfName KEYWORDS = new PdfName("Keywords");
        /** A name */
        public static readonly PdfName KIDS = new PdfName("Kids");
        /** A name */
        public static readonly PdfName L = new PdfName("L");
        /** A name */
        public static readonly PdfName L2R = new PdfName("L2R");
        /** A name */
        public static readonly PdfName LANG = new PdfName("Lang");
        /** A name */
        public static readonly PdfName LANGUAGE = new PdfName("Language");
        /** A name */
        public static readonly PdfName LAST = new PdfName("Last");
        /** A name */
        public static readonly PdfName LASTCHAR = new PdfName("LastChar");
        /** A name */
        public static readonly PdfName LASTPAGE = new PdfName("LastPage");
        /** A name */
        public static readonly PdfName LAUNCH = new PdfName("Launch");
        /** A name */
        public static readonly PdfName LENGTH = new PdfName("Length");
        /** A name */
        public static readonly PdfName LENGTH1 = new PdfName("Length1");
        /** A name */
        public static readonly PdfName LIMITS = new PdfName("Limits");
        /** A name */
        public static readonly PdfName LINE = new PdfName("Line");
        /** A name */
        public static readonly PdfName LINK = new PdfName("Link");
        /** A name */
        public static readonly PdfName LISTMODE = new PdfName("ListMode");
        /** A name */
        public static readonly PdfName LOCATION = new PdfName("Location");
        /** A name */
        public static readonly PdfName LOCK = new PdfName("Lock");
        /** A name */
        public static readonly PdfName LOCKED = new PdfName("Locked");
        /** A name */
        public static readonly PdfName LZWDECODE = new PdfName("LZWDecode");
        /** A name */
        public static readonly PdfName M = new PdfName("M");
        /** A name */
        public static readonly PdfName MATRIX = new PdfName("Matrix");
        /** A name of an encoding */
        public static readonly PdfName MAC_EXPERT_ENCODING = new PdfName("MacExpertEncoding");
        /** A name of an encoding */
        public static readonly PdfName MAC_ROMAN_ENCODING = new PdfName("MacRomanEncoding");
        /** A name */
        public static readonly PdfName MARKED = new PdfName("Marked");
        /** A name */
        public static readonly PdfName MARKINFO = new PdfName("MarkInfo");
        /** A name */
        public static readonly PdfName MASK = new PdfName("Mask");
        /** A name */
        public static readonly PdfName MAX = new PdfName("max");
        /** A name */
        public static readonly PdfName MAXLEN = new PdfName("MaxLen");
        /** A name */
        public static readonly PdfName MEDIABOX = new PdfName("MediaBox");
        /** A name */
        public static readonly PdfName MCID = new PdfName("MCID");
        /** A name */
        public static readonly PdfName MCR = new PdfName("MCR");
        /** A name */
        public static readonly PdfName METADATA = new PdfName("Metadata");
        /** A name */
        public static readonly PdfName MIN = new PdfName("min");
        /** A name */
        public static readonly PdfName MK = new PdfName("MK");
        /** A name */
        public static readonly PdfName MMTYPE1 = new PdfName("MMType1");
        /** A name */
        public static readonly PdfName MODDATE = new PdfName("ModDate");
        /** A name */
        public static readonly PdfName N = new PdfName("N");
        /** A name */
        public static readonly PdfName N0 = new PdfName("n0");
        /** A name */
        public static readonly PdfName N1 = new PdfName("n1");
        /** A name */
        public static readonly PdfName N2 = new PdfName("n2");
        /** A name */
        public static readonly PdfName N3 = new PdfName("n3");
        /** A name */
        public static readonly PdfName N4 = new PdfName("n4");
        /** A name */
        public new static readonly PdfName NAME = new PdfName("Name");
        /** A name */
        public static readonly PdfName NAMED = new PdfName("Named");
        /** A name */
        public static readonly PdfName NAMES = new PdfName("Names");
        /** A name */
        public static readonly PdfName NEEDAPPEARANCES = new PdfName("NeedAppearances");
        /** A name */
        public static readonly PdfName NEWWINDOW = new PdfName("NewWindow");
        /** A name */
        public static readonly PdfName NEXT = new PdfName("Next");
        /** A name */
        public static readonly PdfName NEXTPAGE = new PdfName("NextPage");
        /** A name */
        public static readonly PdfName NM = new PdfName("NM");
        /** A name */
        public static readonly PdfName NONE = new PdfName("None");
        /** A name */
        public static readonly PdfName NONFULLSCREENPAGEMODE = new PdfName("NonFullScreenPageMode");
        /** A name */
        public static readonly PdfName NUMCOPIES = new PdfName("NumCopies");
        /** A name */
        public static readonly PdfName NUMS = new PdfName("Nums");
        /** A name */
        public static readonly PdfName O = new PdfName("O");
        /** A name */
        public static readonly PdfName OBJSTM = new PdfName("ObjStm");
        /** A name */
        public static readonly PdfName OC = new PdfName("OC");
        /** A name */
        public static readonly PdfName OCG = new PdfName("OCG");
        /** A name */
        public static readonly PdfName OCGS = new PdfName("OCGs");
        /** A name */
        public static readonly PdfName OCMD = new PdfName("OCMD");
        /** A name */
        public static readonly PdfName OCPROPERTIES = new PdfName("OCProperties");
        /** A name */
        public static readonly PdfName Off_ = new PdfName("Off");
        /** A name */
        public static readonly PdfName OFF = new PdfName("OFF");
        /** A name */
        public static readonly PdfName ON = new PdfName("ON");
        /** A name */
        public static readonly PdfName ONECOLUMN = new PdfName("OneColumn");
        /** A name */
        public static readonly PdfName OPEN = new PdfName("Open");
        /** A name */
        public static readonly PdfName OPENACTION = new PdfName("OpenAction");
        /** A name */
        public static readonly PdfName OP = new PdfName("OP");
        /** A name */
        public static readonly PdfName op_ = new PdfName("op");
        /** A name */
        public static readonly PdfName OPM = new PdfName("OPM");
        /** A name */
        public static readonly PdfName OPT = new PdfName("Opt");
        /** A name */
        public static readonly PdfName ORDER = new PdfName("Order");
        /** A name */
        public static readonly PdfName ORDERING = new PdfName("Ordering");
        /** A name */
        public static readonly PdfName OUTLINES = new PdfName("Outlines");
        /** A name */
        public static readonly PdfName OUTPUTCONDITION = new PdfName("OutputCondition");
        /** A name */
        public static readonly PdfName OUTPUTCONDITIONIDENTIFIER = new PdfName("OutputConditionIdentifier");
        /** A name */
        public static readonly PdfName OUTPUTINTENT = new PdfName("OutputIntent");
        /** A name */
        public static readonly PdfName OUTPUTINTENTS = new PdfName("OutputIntents");
        /** A name */
        public static readonly PdfName P = new PdfName("P");
        /** A name */
        public static readonly PdfName PAGE = new PdfName("Page");
        /** A name */
        public static readonly PdfName PAGELABELS = new PdfName("PageLabels");
        /** A name */
        public static readonly PdfName PAGELAYOUT = new PdfName("PageLayout");
        /** A name */
        public static readonly PdfName PAGEMODE = new PdfName("PageMode");
        /** A name */
        public static readonly PdfName PAGES = new PdfName("Pages");
        /** A name */
        public static readonly PdfName PAINTTYPE = new PdfName("PaintType");
        /** A name */
        public static readonly PdfName PANOSE = new PdfName("Panose");
        /** A name */
        public static readonly PdfName PARAMS = new PdfName("Params");
        /** A name */
        public static readonly PdfName PARENT = new PdfName("Parent");
        /** A name */
        public static readonly PdfName PARENTTREE = new PdfName("ParentTree");
        /** A name */
        public static readonly PdfName PATTERN = new PdfName("Pattern");
        /** A name */
        public static readonly PdfName PATTERNTYPE = new PdfName("PatternType");
        /** A name */
        public static readonly PdfName PDF = new PdfName("PDF");
        /** A name */
        public static readonly PdfName PDFDOCENCODING = new PdfName("PDFDocEncoding");
        /** A name */
        public static readonly PdfName PERCEPTUAL = new PdfName("Perceptual");
        /** A name */
        public static readonly PdfName PERMS = new PdfName("Perms");
        /** A name */
        public static readonly PdfName PG = new PdfName("Pg");
        /** A name */
        public static readonly PdfName PICKTRAYBYPDFSIZE = new PdfName("PickTrayByPDFSize");
        /** A name */
        public static readonly PdfName POPUP = new PdfName("Popup");
        /** A name */
        public static readonly PdfName PREDICTOR = new PdfName("Predictor");
        /** A name */
        public static readonly PdfName PREFERRED = new PdfName("Preferred");
        /** A name */
        public static readonly PdfName PRESERVERB = new PdfName("PreserveRB");
        /** A name */
        public static readonly PdfName PREV = new PdfName("Prev");
        /** A name */
        public static readonly PdfName PREVPAGE = new PdfName("PrevPage");
        /** A name */
        public static readonly PdfName PRINT = new PdfName("Print");
        /** A name */
        public static readonly PdfName PRINTAREA = new PdfName("PrintArea");
        /** A name */
        public static readonly PdfName PRINTCLIP = new PdfName("PrintClip");
        /** A name */
        public static readonly PdfName PRINTPAGERANGE = new PdfName("PrintPageRange");
        /** A name */
        public static readonly PdfName PRINTSCALING = new PdfName("PrintScaling");
        /** A name */
        public static readonly PdfName PRINTSTATE = new PdfName("PrintState");
        /** A name */
        public static readonly PdfName PROCSET = new PdfName("ProcSet");
        /** A name */
        public static readonly PdfName PRODUCER = new PdfName("Producer");
        /** A name */
        public static readonly PdfName PROPERTIES = new PdfName("Properties");
        /** A name */
        public static readonly PdfName PS = new PdfName("PS");
        /** A name */
        public static readonly PdfName PUBSEC = new PdfName("Adobe.PubSec");
        /** A name */
        public static readonly PdfName Q = new PdfName("Q");
        /** A name */
        public static readonly PdfName QUADPOINTS = new PdfName("QuadPoints");
        /** A name */
        public static readonly PdfName R = new PdfName("R");
        /** A name */
        public static readonly PdfName R2L = new PdfName("R2L");
        /** A name */
        public static readonly PdfName RANGE = new PdfName("Range");
        /** A name */
        public static readonly PdfName RC = new PdfName("RC");
        /** A name */
        public static readonly PdfName RBGROUPS = new PdfName("RBGroups");
        /** A name */
        public static readonly PdfName REASON = new PdfName("Reason");
        /** A name */
        public static readonly PdfName RECIPIENTS = new PdfName("Recipients");
        /** A name */
        public static readonly PdfName RECT = new PdfName("Rect");
        /** A name */
        public static readonly PdfName REFERENCE = new PdfName("Reference");
        /** A name */
        public static readonly PdfName REGISTRY = new PdfName("Registry");
        /** A name */
        public static readonly PdfName REGISTRYNAME = new PdfName("RegistryName");
        /** A name */
        public static readonly PdfName RELATIVECALORIMETRIC = new PdfName("RelativeColorimetric");
        /** A name */
        public static readonly PdfName RENDITION = new PdfName("Rendition");
        /** A name */
        public static readonly PdfName RESETFORM = new PdfName("ResetForm");
        /** A name */
        public static readonly PdfName RESOURCES = new PdfName("Resources");
        /** A name */
        public static readonly PdfName RI = new PdfName("RI");
        /** A name */
        public static readonly PdfName ROLEMAP = new PdfName("RoleMap");
        /** A name */
        public static readonly PdfName ROOT = new PdfName("Root");
        /** A name */
        public static readonly PdfName ROTATE = new PdfName("Rotate");
        /** A name */
        public static readonly PdfName ROWS = new PdfName("Rows");
        /** A name */
        public static readonly PdfName RUNLENGTHDECODE = new PdfName("RunLengthDecode");
        /** A name */
        public static readonly PdfName RV = new PdfName("RV");
        /** A name */
        public static readonly PdfName S = new PdfName("S");
        /** A name */
        public static readonly PdfName SATURATION = new PdfName("Saturation");
        /** A name */
        public static readonly PdfName SCHEMA = new PdfName("Schema");
        /** A name */
        public static readonly PdfName SCREEN = new PdfName("Screen");
        /** A name */
        public static readonly PdfName SECT = new PdfName("Sect");
        /** A name */
        public static readonly PdfName SEPARATION = new PdfName("Separation");
        /** A name */
        public static readonly PdfName SETOCGSTATE = new PdfName("SetOCGState");
        /** A name */
        public static readonly PdfName SHADING = new PdfName("Shading");
        /** A name */
        public static readonly PdfName SHADINGTYPE = new PdfName("ShadingType");
        /** A name */
        public static readonly PdfName SHIFT_JIS = new PdfName("Shift-JIS");
        /** A name */
        public static readonly PdfName SIG = new PdfName("Sig");
        /** A name */
        public static readonly PdfName SIGFLAGS = new PdfName("SigFlags");
        /** A name */
        public static readonly PdfName SIGREF = new PdfName("SigRef");
        /** A name */
        public static readonly PdfName SIMPLEX = new PdfName("Simplex");
        /** A name */
        public static readonly PdfName SINGLEPAGE = new PdfName("SinglePage");
        /** A name */
        public static readonly PdfName SIZE = new PdfName("Size");
        /** A name */
        public static readonly PdfName SMASK = new PdfName("SMask");
        /** A name */
        public static readonly PdfName SORT = new PdfName("Sort");
        /** A name */
        public static readonly PdfName SPAN = new PdfName("Span");
        /** A name */
        public static readonly PdfName SPLIT = new PdfName("Split");
        /** A name */
        public static readonly PdfName SQUARE = new PdfName("Square");
        /** A name */
        public static readonly PdfName SQUIGGLY = new PdfName("Squiggly");
        /** A name */
        public static readonly PdfName ST = new PdfName("St");
        /** A name */
        public static readonly PdfName STAMP = new PdfName("Stamp");
        /** A name */
        public static readonly PdfName STANDARD = new PdfName("Standard");
        /** A name */
        public static readonly PdfName STATE = new PdfName("State");
        /** A name */
        public static readonly PdfName STDCF = new PdfName("StdCF");
        /** A name */
        public static readonly PdfName STEMV = new PdfName("StemV");
        /** A name */
        public static readonly PdfName STMF = new PdfName("StmF");
        /** A name */
        public static readonly PdfName STRF = new PdfName("StrF");
        /** A name */
        public static readonly PdfName STRIKEOUT = new PdfName("StrikeOut");
        /** A name */
        public static readonly PdfName STRUCTPARENT = new PdfName("StructParent");
        /** A name */
        public static readonly PdfName STRUCTPARENTS = new PdfName("StructParents");
        /** A name */
        public static readonly PdfName STRUCTTREEROOT = new PdfName("StructTreeRoot");
        /** A name */
        public static readonly PdfName STYLE = new PdfName("Style");
        /** A name */
        public static readonly PdfName SUBFILTER = new PdfName("SubFilter");
        /** A name */
        public static readonly PdfName SUBJECT = new PdfName("Subject");
        /** A name */
        public static readonly PdfName SUBMITFORM = new PdfName("SubmitForm");
        /** A name */
        public static readonly PdfName SUBTYPE = new PdfName("Subtype");
        /** A name */
        public static readonly PdfName SUPPLEMENT = new PdfName("Supplement");
        /** A name */
        public static readonly PdfName SV = new PdfName("SV");
        /** A name */
        public static readonly PdfName SW = new PdfName("SW");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName SYMBOL = new PdfName("Symbol");
        /** A name */
        public static readonly PdfName T = new PdfName("T");
        /** A name */
        public static readonly PdfName TEXT = new PdfName("Text");
        /** A name */
        public static readonly PdfName THUMB = new PdfName("Thumb");
        /** A name */
        public static readonly PdfName THREADS = new PdfName("Threads");
        /** A name */
        public static readonly PdfName TI = new PdfName("TI");
        /** A name */
        public static readonly PdfName TILINGTYPE = new PdfName("TilingType");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName TIMES_ROMAN = new PdfName("Times-Roman");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName TIMES_BOLD = new PdfName("Times-Bold");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName TIMES_ITALIC = new PdfName("Times-Italic");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName TIMES_BOLDITALIC = new PdfName("Times-BoldItalic");
        /** A name */
        public static readonly PdfName TITLE = new PdfName("Title");
        /** A name */
        public static readonly PdfName TK = new PdfName("TK");
        /** A name */
        public static readonly PdfName TM = new PdfName("TM");
        /** A name */
        public static readonly PdfName TOGGLE = new PdfName("Toggle");
        /** A name */
        public static readonly PdfName TOUNICODE = new PdfName("ToUnicode");
        /** A name */
        public static readonly PdfName TP = new PdfName("TP");
        /** A name */
        public static readonly PdfName TRANS = new PdfName("Trans");
        /** A name */
        public static readonly PdfName TRANSFORMPARAMS = new PdfName("TransformParams");
        /** A name */
        public static readonly PdfName TRANSFORMMETHOD = new PdfName("TransformMethod");
        /** A name */
        public static readonly PdfName TRANSPARENCY = new PdfName("Transparency");
        /** A name */
        public static readonly PdfName TRAPPED = new PdfName("Trapped");
        /** A name */
        public static readonly PdfName TRIMBOX = new PdfName("TrimBox");
        /** A name */
        public static readonly PdfName TRUETYPE = new PdfName("TrueType");
        /** A name */
        public static readonly PdfName TU = new PdfName("TU");
        /** A name */
        public static readonly PdfName TWOCOLUMNLEFT = new PdfName("TwoColumnLeft");
        /** A name */
        public static readonly PdfName TWOCOLUMNRIGHT = new PdfName("TwoColumnRight");
        /** A name */
        public static readonly PdfName TWOPAGELEFT = new PdfName("TwoPageLeft");
        /** A name */
        public static readonly PdfName TWOPAGERIGHT = new PdfName("TwoPageRight");
        /** A name */
        public static readonly PdfName TX = new PdfName("Tx");
        /** A name */
        public static readonly PdfName TYPE = new PdfName("Type");
        /** A name */
        public static readonly PdfName TYPE0 = new PdfName("Type0");
        /** A name */
        public static readonly PdfName TYPE1 = new PdfName("Type1");
        /** A name of an attribute. */
        public static readonly PdfName TYPE3 = new PdfName("Type3");
        /** A name of an attribute. */
        public static readonly PdfName U = new PdfName("U");
        /** A name of an attribute. */
        public static readonly PdfName UF = new PdfName("UF");
        /** A name of an attribute. */
        public static readonly PdfName UHC = new PdfName("UHC");
        /** A name of an attribute. */
        public static readonly PdfName UNDERLINE = new PdfName("Underline");
        /** A name */
        public static readonly PdfName UR = new PdfName("UR");
        /** A name */
        public static readonly PdfName UR3 = new PdfName("UR3");
        /** A name */
        public static readonly PdfName URI = new PdfName("URI");
        /** A name */
        public static readonly PdfName URL = new PdfName("URL");
        /** A name */
        public static readonly PdfName USAGE = new PdfName("Usage");
        /** A name */
        public static readonly PdfName USEATTACHMENTS = new PdfName("UseAttachments");
        /** A name */
        public static readonly PdfName USENONE = new PdfName("UseNone");
        /** A name */
        public static readonly PdfName USEOC = new PdfName("UseOC");
        /** A name */
        public static readonly PdfName USEOUTLINES = new PdfName("UseOutlines");
        /** A name */
        public static readonly PdfName USER = new PdfName("User");
        /** A name */
        public static readonly PdfName USERPROPERTIES = new PdfName("UserProperties");
        /** A name */
        public static readonly PdfName USERUNIT = new PdfName("UserUnit");
        /** A name */
        public static readonly PdfName USETHUMBS = new PdfName("UseThumbs");
        /** A name */
        public static readonly PdfName V = new PdfName("V");
        /** A name */
        public static readonly PdfName V2 = new PdfName("V2");
        /** A name */
        public static readonly PdfName VERISIGN_PPKVS = new PdfName("VeriSign.PPKVS");
        /** A name */
        public static readonly PdfName VERSION = new PdfName("Version");
        /** A name */
        public static readonly PdfName VIEW = new PdfName("View");
        /** A name */
        public static readonly PdfName VIEWAREA = new PdfName("ViewArea");
        /** A name */
        public static readonly PdfName VIEWCLIP = new PdfName("ViewClip");
        /** A name */
        public static readonly PdfName VIEWERPREFERENCES = new PdfName("ViewerPreferences");
        /** A name */
        public static readonly PdfName VIEWSTATE = new PdfName("ViewState");
        /** A name */
        public static readonly PdfName VISIBLEPAGES = new PdfName("VisiblePages");
        /** A name of an attribute. */
        public static readonly PdfName W = new PdfName("W");
        /** A name of an attribute. */
        public static readonly PdfName W2 = new PdfName("W2");
        /** A name of an attribute. */
        public static readonly PdfName WC = new PdfName("WC");
        /** A name of an attribute. */
        public static readonly PdfName WIDGET = new PdfName("Widget");
        /** A name of an attribute. */
        public static readonly PdfName WIDTH = new PdfName("Width");
        /** A name */
        public static readonly PdfName WIDTHS = new PdfName("Widths");
        /** A name of an encoding */
        public static readonly PdfName WIN = new PdfName("Win");
        /** A name of an encoding */
        public static readonly PdfName WIN_ANSI_ENCODING = new PdfName("WinAnsiEncoding");
        /** A name of an encoding */
        public static readonly PdfName WIPE = new PdfName("Wipe");
        /** A name */
        public static readonly PdfName WHITEPOINT = new PdfName("WhitePoint");
        /** A name */
        public static readonly PdfName WP = new PdfName("WP");
        /** A name of an encoding */
        public static readonly PdfName WS = new PdfName("WS");
        /** A name */
        public static readonly PdfName X = new PdfName("X");
        /** A name */
        public static readonly PdfName XFA = new PdfName("XFA");
        /** A name */
        public static readonly PdfName XML = new PdfName("XML");
        /** A name */
        public static readonly PdfName XOBJECT = new PdfName("XObject");
        /** A name */
        public static readonly PdfName XSTEP = new PdfName("XStep");
        /** A name */
        public static readonly PdfName XREF = new PdfName("XRef");
        /** A name */
        public static readonly PdfName XREFSTM = new PdfName("XRefStm");
        /** A name */
        public static readonly PdfName XYZ = new PdfName("XYZ");
        /** A name */
        public static readonly PdfName YSTEP = new PdfName("YStep");
        /** A name */
        public static readonly PdfName ZADB = new PdfName("ZaDb");
        /** A name of a base 14 type 1 font */
        public static readonly PdfName ZAPFDINGBATS = new PdfName("ZapfDingbats");
        /** A name */
        public static readonly PdfName ZOOM = new PdfName("Zoom");
    
        private int hash = 0;
    
        // constructors
    
        /**
        * Constructs a new <CODE>PdfName</CODE>. The name length will be checked.
        * @param name the new name
        */
        public PdfName(String name) : this(name, true) {
        }
        
        /**
        * Constructs a new <CODE>PdfName</CODE>.
        * @param name the new name
        * @param lengthCheck if <CODE>true</CODE> check the lenght validity, if <CODE>false</CODE> the name can
        * have any length
        */
        public PdfName(string name, bool lengthCheck) : base(PdfObject.NAME) {
            // The minimum number of characters in a name is 0, the maximum is 127 (the '/' not included)
            int length = name.Length;
            if (lengthCheck && length > 127) {
                throw new ArgumentException("The name '" + name + "' is too long (" + length + " characters).");
            }
            // every special character has to be substituted
            ByteBuffer pdfName = new ByteBuffer(length + 20);
            pdfName.Append('/');
            char[] chars = name.ToCharArray();
            char character;
            // loop over all the characters
            foreach (char cc in chars) {
                character = (char)(cc & 0xff);
                // special characters are escaped (reference manual p.39)
                switch (character) {
                    case ' ':
                    case '%':
                    case '(':
                    case ')':
                    case '<':
                    case '>':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case '/':
                    case '#':
                        pdfName.Append('#');
                        pdfName.Append(System.Convert.ToString(character, 16));
                        break;
                    default:
                        if (character > 126 || character < 32) {
                            pdfName.Append('#');
                            if (character < 16)
                                pdfName.Append('0');
                            pdfName.Append(System.Convert.ToString(character, 16));
                        }
                        else
                            pdfName.Append(character);
                        break;
                }
            }
            bytes = pdfName.ToByteArray();
        }

        public PdfName(byte[] bytes) : base(PdfObject.NAME, bytes) {
        }
       
        // methods
    
        /**
         * Compares this object with the specified object for order.  Returns a
         * negative int, zero, or a positive int as this object is less
         * than, equal to, or greater than the specified object.<p>
         *
         *
         * @param   object the Object to be compared.
         * @return  a negative int, zero, or a positive int as this object
         *        is less than, equal to, or greater than the specified object.
         *
         * @throws Exception if the specified object's type prevents it
         *         from being compared to this Object.
         */
        public int CompareTo(Object obj) {
            PdfName name = (PdfName) obj;
        
            byte[] myBytes = bytes;
            byte[] objBytes = name.bytes;
            int len = Math.Min(myBytes.Length, objBytes.Length);
            for (int i=0; i<len; i++) {
                if (myBytes[i] > objBytes[i])
                    return 1;
            
                if (myBytes[i] < objBytes[i])
                    return -1;
            }
            if (myBytes.Length < objBytes.Length)
                return -1;
            if (myBytes.Length > objBytes.Length)
                return 1;
            return 0;
        }
    
        /**
         * Indicates whether some other object is "equal to" this one.
         *
         * @param   obj   the reference object with which to compare.
         * @return  <code>true</code> if this object is the same as the obj
         *          argument; <code>false</code> otherwise.
         */
        public override bool Equals(Object obj) {
            if (this == obj)
                return true;
            if (obj is PdfName)
                return CompareTo(obj) == 0;
            return false;
        }
    
        /**
         * Returns a hash code value for the object. This method is
         * supported for the benefit of hashtables such as those provided by
         * <code>java.util.Hashtable</code>.
         *
         * @return  a hash code value for this object.
         */
        public override int GetHashCode() {
            int h = hash;
            if (h == 0) {
                int ptr = 0;
                int len = bytes.Length;
            
                for (int i = 0; i < len; i++)
                    h = 31*h + (bytes[ptr++] & 0xff);
                hash = h;
            }
            return h;
        }
    
        /** Decodes an escaped name in the form "/AB#20CD" into "AB CD".
         * @param name the name to decode
         * @return the decoded name
         */
        public static string DecodeName(string name) {
            StringBuilder buf = new StringBuilder();
            int len = name.Length;
            for (int k = 1; k < len; ++k) {
                char c = name[k];
                if (c == '#') {
                    c = (char)((PRTokeniser.GetHex(name[k + 1]) << 4) + PRTokeniser.GetHex(name[k + 2]));
                    k += 2;
                }
                buf.Append(c);
            }
            return buf.ToString();
        }
    }
}
