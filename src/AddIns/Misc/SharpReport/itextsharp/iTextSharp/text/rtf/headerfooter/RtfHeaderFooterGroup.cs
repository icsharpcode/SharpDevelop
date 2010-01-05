using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
/*
 * Created on Aug 6, 2004
 *
 * To change the template for this generated file go to
 * Window - Preferences - Java - Code Generation - Code and Comments
 */
namespace iTextSharp.text.rtf.headerfooter {

    /**
    * The RtfHeaderFooterGroup holds 0 - 3 RtfHeaderFooters that create a group
    * of headers or footers.
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfHeaderFooterGroup : HeaderFooter, IRtfBasicElement {
        
        /**
        * This RtfHeaderFooterGroup contains no RtfHeaderFooter objects
        */
        private const int MODE_NONE = 0;
        /**
        * This RtfHeaderFooterGroup contains one RtfHeaderFooter object
        */
        private const int MODE_SINGLE = 1;
        /**
        * This RtfHeaderFooterGroup contains two or three RtfHeaderFooter objects
        */
        private const int MODE_MULTIPLE = 2;
        
        /**
        * The current mode of this RtfHeaderFooterGroup. Defaults to MODE_NONE
        */
        private int mode = MODE_NONE;
        /**
        * The current type of this RtfHeaderFooterGroup. Defaults to RtfHeaderFooter.TYPE_HEADER
        */
        private int type = RtfHeaderFooter.TYPE_HEADER;
        
        /**
        * The RtfHeaderFooter for all pages
        */
        private RtfHeaderFooter headerAll = null;
        /**
        * The RtfHeaderFooter for the first page
        */
        private RtfHeaderFooter headerFirst = null;
        /**
        * The RtfHeaderFooter for the left hand pages
        */
        private RtfHeaderFooter headerLeft = null;
        /**
        * The RtfHeaderFooter for the right hand pages
        */
        private RtfHeaderFooter headerRight = null;
        /**
        * The RtfDocument this RtfHeaderFooterGroup belongs to
        */
        private RtfDocument document = null;

        /**
        * Constructs a RtfHeaderGroup to which you add headers/footers using 
        * via the setHeaderFooter method.
        *
        */
        public RtfHeaderFooterGroup() : base(new Phrase(""), false) {
            this.mode = MODE_NONE;
        }
        
        /**
        * Constructs a certain type of RtfHeaderFooterGroup. RtfHeaderFooter.TYPE_HEADER
        * and RtfHeaderFooter.TYPE_FOOTER are valid values for type.
        * 
        * @param doc The RtfDocument this RtfHeaderFooter belongs to
        * @param type The type of RtfHeaderFooterGroup to create
        */
        public RtfHeaderFooterGroup(RtfDocument doc, int type) : base(new Phrase(""), false) {
            this.document = doc;
            this.type = type;
        }
        
        /**
        * Constructs a RtfHeaderFooterGroup by copying the content of the original
        * RtfHeaderFooterGroup
        * 
        * @param doc The RtfDocument this RtfHeaderFooter belongs to
        * @param headerFooter The RtfHeaderFooterGroup to copy
        * @param type The type of RtfHeaderFooterGroup to create
        */
        public RtfHeaderFooterGroup(RtfDocument doc, RtfHeaderFooterGroup headerFooter, int type) : base(new Phrase(""), false) {
            this.document = doc;
            this.mode = headerFooter.GetMode();
            this.type = type;
            if (headerFooter.GetHeaderAll() != null) {
                this.headerAll = new RtfHeaderFooter(this.document, headerFooter.GetHeaderAll(), RtfHeaderFooter.DISPLAY_ALL_PAGES);
            }
            if (headerFooter.GetHeaderFirst() != null) {
                this.headerFirst = new RtfHeaderFooter(this.document, headerFooter.GetHeaderFirst(), RtfHeaderFooter.DISPLAY_FIRST_PAGE);
            }
            if (headerFooter.GetHeaderLeft() != null) {
                this.headerLeft = new RtfHeaderFooter(this.document, headerFooter.GetHeaderLeft(), RtfHeaderFooter.DISPLAY_LEFT_PAGES);
            }
            if (headerFooter.GetHeaderRight() != null) {
                this.headerRight = new RtfHeaderFooter(this.document, headerFooter.GetHeaderRight(), RtfHeaderFooter.DISPLAY_RIGHT_PAGES);
            }
            SetType(this.type);
        }
        
        /**
        * Constructs a RtfHeaderFooterGroup for a certain RtfHeaderFooter.
        * 
        * @param doc The RtfDocument this RtfHeaderFooter belongs to
        * @param headerFooter The RtfHeaderFooter to display
        * @param type The typ of RtfHeaderFooterGroup to create
        */
        public RtfHeaderFooterGroup(RtfDocument doc, RtfHeaderFooter headerFooter, int type) : base(new Phrase(""), false) {
            this.document = doc;
            this.type = type;
            this.mode = MODE_SINGLE;
            headerAll = new RtfHeaderFooter(doc, headerFooter, RtfHeaderFooter.DISPLAY_ALL_PAGES);
            headerAll.SetType(this.type);
        }
        
        /**
        * Constructs a RtfHeaderGroup for a certain HeaderFooter
        * 
        * @param doc The RtfDocument this RtfHeaderFooter belongs to
        * @param headerFooter The HeaderFooter to display
        * @param type The typ of RtfHeaderFooterGroup to create
        */
        public RtfHeaderFooterGroup(RtfDocument doc, HeaderFooter headerFooter, int type) : base(new Phrase(""), false) {
            this.document = doc;
            this.type = type;
            this.mode = MODE_SINGLE;
            headerAll = new RtfHeaderFooter(doc, headerFooter, type, RtfHeaderFooter.DISPLAY_ALL_PAGES);
            headerAll.SetType(this.type);
        }
        
        /**
        * Sets the RtfDocument this RtfElement belongs to
        * 
        * @param doc The RtfDocument to use
        */
        public void SetRtfDocument(RtfDocument doc) {
            this.document = doc;
            if (headerAll != null) {
                headerAll.SetRtfDocument(this.document);
            }
            if (headerFirst != null) {
                headerFirst.SetRtfDocument(this.document);
            }
            if (headerLeft != null) {
                headerLeft.SetRtfDocument(this.document);
            }
            if (headerRight != null) {
                headerRight.SetRtfDocument(this.document);
            }
        }
        
        /**
        * Write the content of this RtfHeaderFooterGroup.
        */
        public virtual void WriteContent(Stream result) {       
            if (this.mode == MODE_SINGLE) {
                headerAll.WriteContent(result);
            } else if (this.mode == MODE_MULTIPLE) {
                if (headerFirst != null) {
                    headerFirst.WriteContent(result);
                }
                if (headerLeft != null) {
                    headerLeft.WriteContent(result);
                }
                if (headerRight != null) {
                    headerRight.WriteContent(result);
                }
                if (headerAll != null) {
                    headerAll.WriteContent(result);
                }
            }
        }
        
        /**
        * Set a RtfHeaderFooter to be displayed at a certain position
        * 
        * @param headerFooter The RtfHeaderFooter to display
        * @param displayAt The display location to use
        */
        public void SetHeaderFooter(RtfHeaderFooter headerFooter, int displayAt) {
            this.mode = MODE_MULTIPLE;
            headerFooter.SetRtfDocument(this.document);
            headerFooter.SetType(this.type);
            headerFooter.SetDisplayAt(displayAt);
            switch (displayAt) {
                case RtfHeaderFooter.DISPLAY_ALL_PAGES:
                    headerAll = headerFooter;
                    break;
                case RtfHeaderFooter.DISPLAY_FIRST_PAGE:
                    headerFirst = headerFooter;
                    break;
                case RtfHeaderFooter.DISPLAY_LEFT_PAGES:
                    headerLeft = headerFooter;
                    break;
                case RtfHeaderFooter.DISPLAY_RIGHT_PAGES:
                    headerRight = headerFooter;
                    break;
            }
        }
        
        /**
        * Set a HeaderFooter to be displayed at a certain position
        * 
        * @param headerFooter The HeaderFooter to set
        * @param displayAt The display location to use
        */
        public void SetHeaderFooter(HeaderFooter headerFooter, int displayAt) {
            this.mode = MODE_MULTIPLE;
            switch (displayAt) {
                case RtfHeaderFooter.DISPLAY_ALL_PAGES:
                    headerAll = new RtfHeaderFooter(this.document, headerFooter, this.type, displayAt);
                    break;
                case RtfHeaderFooter.DISPLAY_FIRST_PAGE:
                    headerFirst = new RtfHeaderFooter(this.document, headerFooter, this.type, displayAt);
                    break;
                case RtfHeaderFooter.DISPLAY_LEFT_PAGES:
                    headerLeft = new RtfHeaderFooter(this.document, headerFooter, this.type, displayAt);
                    break;
                case RtfHeaderFooter.DISPLAY_RIGHT_PAGES:
                    headerRight = new RtfHeaderFooter(this.document, headerFooter, this.type, displayAt);
                    break;
            }
        }
        
        /**
        * Set that this RtfHeaderFooterGroup should have a title page. If only
        * a header / footer for all pages exists, then it will be copied to the
        * first page aswell.
        */
        public void SetHasTitlePage() {
            if (this.mode == MODE_SINGLE) {
                this.mode = MODE_MULTIPLE;
                headerFirst = new RtfHeaderFooter(this.document, headerAll, RtfHeaderFooter.DISPLAY_FIRST_PAGE);
                headerFirst.SetType(this.type);
            }
        }
        
        /**
        * Set that this RtfHeaderFooterGroup should have facing pages. If only
        * a header / footer for all pages exists, then it will be copied to the left
        * and right pages aswell.
        */
        public void SetHasFacingPages() {
            if (this.mode == MODE_SINGLE) {
                this.mode = MODE_MULTIPLE;
                this.headerLeft = new RtfHeaderFooter(this.document, this.headerAll, RtfHeaderFooter.DISPLAY_LEFT_PAGES);
                this.headerLeft.SetType(this.type);
                this.headerRight = new RtfHeaderFooter(this.document, this.headerAll, RtfHeaderFooter.DISPLAY_RIGHT_PAGES);
                this.headerRight.SetType(this.type);
                this.headerAll = null;
            } else if (this.mode == MODE_MULTIPLE) {
                if (this.headerLeft == null && this.headerAll != null) {
                    this.headerLeft = new RtfHeaderFooter(this.document, this.headerAll, RtfHeaderFooter.DISPLAY_LEFT_PAGES);
                    this.headerLeft.SetType(this.type);
                }
                if (this.headerRight == null && this.headerAll != null) {
                    this.headerRight = new RtfHeaderFooter(this.document, this.headerAll, RtfHeaderFooter.DISPLAY_RIGHT_PAGES);
                    this.headerRight.SetType(this.type);
                }
                this.headerAll = null;
            }
        }
        
        /**
        * Get whether this RtfHeaderFooterGroup has a titlepage
        * 
        * @return Whether this RtfHeaderFooterGroup has a titlepage
        */
        public bool HasTitlePage() {
            return (headerFirst != null);
        }
        
        /**
        * Get whether this RtfHeaderFooterGroup has facing pages
        * 
        * @return Whether this RtfHeaderFooterGroup has facing pages
        */
        public bool HasFacingPages() {
            return (headerLeft != null || headerRight != null);
        }

        /**
        * Unused
        * @param inTable
        */
        public void SetInTable(bool inTable) {
        }
        
        /**
        * Unused
        * @param inHeader
        */
        public void SetInHeader(bool inHeader) {
        }
        
        /**
        * Set the type of this RtfHeaderFooterGroup. RtfHeaderFooter.TYPE_HEADER
        * or RtfHeaderFooter.TYPE_FOOTER. Also sets the type for all RtfHeaderFooters
        * of this RtfHeaderFooterGroup.
        * 
        * @param type The type to use
        */
        public void SetType(int type) {
            this.type = type;
            if (headerAll != null) {
                headerAll.SetType(this.type);
            }
            if (headerFirst != null) {
                headerFirst.SetType(this.type);
            }
            if (headerLeft != null) {
                headerLeft.SetType(this.type);
            }
            if (headerRight != null) {
                headerRight.SetType(this.type);
            }
        }
        
        /**
        * Gets the mode of this RtfHeaderFooterGroup
        * 
        * @return The mode of this RtfHeaderFooterGroup
        */
        protected int GetMode() {
            return this.mode;
        }
        
        /**
        * Gets the RtfHeaderFooter for all pages
        * 
        * @return The RtfHeaderFooter for all pages 
        */
        protected RtfHeaderFooter GetHeaderAll() {
            return headerAll;
        }

        /**
        * Gets the RtfHeaderFooter for the title page
        * 
        * @return The RtfHeaderFooter for the title page 
        */
        protected RtfHeaderFooter GetHeaderFirst() {
            return headerFirst;
        }

        /**
        * Gets the RtfHeaderFooter for all left hand pages
        * 
        * @return The RtfHeaderFooter for all left hand pages 
        */
        protected RtfHeaderFooter GetHeaderLeft() {
            return headerLeft;
        }

        /**
        * Gets the RtfHeaderFooter for all right hand pages
        * 
        * @return The RtfHeaderFooter for all right hand pages 
        */
        protected RtfHeaderFooter GetHeaderRight() {
            return headerRight;
        }
    }
}