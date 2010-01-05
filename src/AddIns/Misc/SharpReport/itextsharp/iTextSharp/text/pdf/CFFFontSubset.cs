using System;
using System.Collections;

namespace iTextSharp.text.pdf {
    /**
    * This Class subsets a CFF Type Font. The subset is preformed for CID fonts and NON CID fonts.
    * The Charstring is subseted for both types. For CID fonts only the FDArray which are used are embedded. 
    * The Lsubroutines of the FDArrays used are subsetted as well. The Subroutine subset supports both Type1 and Type2
    * formatting altough only tested on Type2 Format.
    * For Non CID the Lsubroutines are subsetted. On both types the Gsubroutines is subsetted. 
    * A font which was not of CID type is transformed into CID as a part of the subset process. 
    * The CID synthetic creation was written by Sivan Toledo <sivan@math.tau.ac.il> 
    * @author Oren Manor <manorore@post.tau.ac.il> & Ygal Blum <blumygal@post.tau.ac.il>
    */
    public class CFFFontSubset : CFFFont {
        
        /**
        *  The Strings in this array represent Type1/Type2 operator names
        */
        internal static String[] SubrsFunctions = {
                "RESERVED_0","hstem","RESERVED_2","vstem","vmoveto","rlineto","hlineto","vlineto",
                "rrcurveto","RESERVED_9","callsubr","return","escape","RESERVED_13",
                "endchar","RESERVED_15","RESERVED_16","RESERVED_17","hstemhm","hintmask",
                "cntrmask","rmoveto","hmoveto","vstemhm","rcurveline","rlinecurve","vvcurveto",
                "hhcurveto","shortint","callgsubr","vhcurveto","hvcurveto"
                };
        /**
        * The Strings in this array represent Type1/Type2 escape operator names
        */
        internal static String[] SubrsEscapeFuncs = {
                "RESERVED_0","RESERVED_1","RESERVED_2","and","or","not","RESERVED_6",
                "RESERVED_7","RESERVED_8","abs","add","sub","div","RESERVED_13","neg",
                "eq","RESERVED_16","RESERVED_17","drop","RESERVED_19","put","get","ifelse",
                "random","mul","RESERVED_25","sqrt","dup","exch","index","roll","RESERVED_31",
                "RESERVED_32","RESERVED_33","hflex","flex","hflex1","flex1","RESERVED_REST"
        };
        
        /**
        * A HashMap containing the glyphs used in the text after being converted
        * to glyph number by the CMap 
        */
        internal Hashtable GlyphsUsed;
        /**
        * The GlyphsUsed keys as an ArrayList
        */
        internal ArrayList glyphsInList;
        /**
        * A HashMap for keeping the FDArrays being used by the font
        */
        internal Hashtable FDArrayUsed = new Hashtable();
        /**
        * A HashMaps array for keeping the subroutines used in each FontDict
        */
        internal Hashtable[] hSubrsUsed;
        /**
        * The SubroutinesUsed HashMaps as ArrayLists
        */
        internal ArrayList[] lSubrsUsed;
        /**
        * A HashMap for keeping the Global subroutines used in the font
        */
        internal Hashtable hGSubrsUsed  = new Hashtable();
        /**
        * The Global SubroutinesUsed HashMaps as ArrayLists
        */
        internal ArrayList lGSubrsUsed = new ArrayList();
        /**
        * A HashMap for keeping the subroutines used in a non-cid font
        */
        internal Hashtable hSubrsUsedNonCID  = new Hashtable();
        /**
        * The SubroutinesUsed HashMap as ArrayList
        */
        internal ArrayList lSubrsUsedNonCID = new ArrayList();
        /**
        * An array of the new Indexs for the local Subr. One index for each FontDict
        */
        internal byte[][] NewLSubrsIndex;
        /**
        * The new subroutines index for a non-cid font
        */
        internal byte[] NewSubrsIndexNonCID;
        /**
        * The new global subroutines index of the font
        */
        internal byte[] NewGSubrsIndex;
        /**
        * The new CharString of the font
        */
        internal byte[] NewCharStringsIndex;
        
        /**
        * The bias for the global subroutines
        */
        internal int GBias = 0;
        
        /**
        * The linked list for generating the new font stream
        */
        internal ArrayList OutputList;
        
        /**
        * Number of arguments to the stem operators in a subroutine calculated recursivly
        */
        internal int NumOfHints=0;

        
        /**     
        * C'tor for CFFFontSubset
        * @param rf - The font file
        * @param GlyphsUsed - a HashMap that contains the glyph used in the subset 
        */
        public CFFFontSubset(RandomAccessFileOrArray rf,Hashtable GlyphsUsed) : base(rf) {
            // Use CFFFont c'tor in order to parse the font file.
            this.GlyphsUsed = GlyphsUsed;
            //Put the glyphs into a list
            glyphsInList = new ArrayList(GlyphsUsed.Keys);
            
            
            for (int i=0;i<fonts.Length;++i)
            {
                // Read the number of glyphs in the font
                Seek(fonts[i].charstringsOffset);
                fonts[i].nglyphs = GetCard16();
                
                // Jump to the count field of the String Index
                Seek(stringIndexOffset);
                fonts[i].nstrings = GetCard16()+standardStrings.Length;
                
                // For each font save the offset array of the charstring
                fonts[i].charstringsOffsets = GetIndex(fonts[i].charstringsOffset);
                
                // Proces the FDSelect if exist 
                if (fonts[i].fdselectOffset>=0)
                {
                    // Proces the FDSelect
                    ReadFDSelect(i);
                    // Build the FDArrayUsed hashmap
                    BuildFDArrayUsed(i);
                }
                if (fonts[i].isCID)
                    // Build the FD Array used Hash Map
                    ReadFDArray(i);
                // compute the charset length 
                fonts[i].CharsetLength = CountCharset(fonts[i].charsetOffset,fonts[i].nglyphs);
            }
        }

        /**
        * Calculates the length of the charset according to its format
        * @param Offset The Charset Offset
        * @param NumofGlyphs Number of glyphs in the font
        * @return the length of the Charset
        */
        internal int CountCharset(int Offset,int NumofGlyphs){
            int format;
            int Length=0;
            Seek(Offset);
            // Read the format
            format = GetCard8();
            // Calc according to format
            switch (format){
                case 0:
                    Length = 1+2*NumofGlyphs;
                    break;
                case 1:
                    Length = 1+3*CountRange(NumofGlyphs,1);
                    break;
                case 2:
                    Length = 1+4*CountRange(NumofGlyphs,2);
                    break;
                default:
                    break;
            }
            return Length;
        }
        
        /**
        * Function calculates the number of ranges in the Charset
        * @param NumofGlyphs The number of glyphs in the font
        * @param Type The format of the Charset
        * @return The number of ranges in the Charset data structure
        */
        int CountRange(int NumofGlyphs,int Type){
            int num=0;
            char Sid;
            int i=1,nLeft;
            while (i<NumofGlyphs){
                num++;
                Sid = GetCard16();
                if (Type==1)
                    nLeft = GetCard8();
                else
                    nLeft = GetCard16();
                i += nLeft+1;
            }
            return num;
        }


        /**
        * Read the FDSelect of the font and compute the array and its length
        * @param Font The index of the font being processed
        * @return The Processed FDSelect of the font
        */
        protected void ReadFDSelect(int Font)
        {
            // Restore the number of glyphs
            int NumOfGlyphs = fonts[Font].nglyphs;
            int[] FDSelect = new int[NumOfGlyphs];
            // Go to the beginning of the FDSelect
            Seek(fonts[Font].fdselectOffset);
            // Read the FDSelect's format
            fonts[Font].FDSelectFormat = GetCard8();
            
            switch (fonts[Font].FDSelectFormat){
                // Format==0 means each glyph has an entry that indicated
                // its FD.
                case 0:
                    for (int i=0;i<NumOfGlyphs;i++)
                    {
                        FDSelect[i] = GetCard8();
                    }
                    // The FDSelect's Length is one for each glyph + the format
                    // for later use
                    fonts[Font].FDSelectLength = fonts[Font].nglyphs+1;
                    break;
                case 3:
                    // Format==3 means the ranges version
                    // The number of ranges
                    int nRanges = GetCard16();
                    int l=0;
                    // Read the first in the first range
                    int first = GetCard16();
                    for (int i=0;i<nRanges;i++)
                    {
                        // Read the FD index
                        int fd = GetCard8();
                        // Read the first of the next range
                        int last = GetCard16();
                        // Calc the steps and write to the array
                        int steps = last-first;
                        for (int k=0;k<steps;k++)
                        {
                            FDSelect[l] = fd;
                            l++;
                        }
                        // The last from this iteration is the first of the next
                        first = last;
                    }
                    // Store the length for later use
                    fonts[Font].FDSelectLength = 1+2+nRanges*3+2;
                    break;
                default:
                    break;
            }
            // Save the FDSelect of the font 
            fonts[Font].FDSelect = FDSelect; 
        }
        
        /**
        * Function reads the FDSelect and builds the FDArrayUsed HashMap According to the glyphs used
        * @param Font the Number of font being processed
        */
        protected void BuildFDArrayUsed(int Font)
        {
            int[] FDSelect = fonts[Font].FDSelect;
            // For each glyph used
            for (int i=0;i<glyphsInList.Count;i++)
            {
                // Pop the glyphs index
                int glyph = (int)glyphsInList[i];
                // Pop the glyph's FD
                int FD = FDSelect[glyph];
                // Put the FD index into the FDArrayUsed HashMap
                FDArrayUsed[FD] = null;
            }
        }

        /**
        * Read the FDArray count, offsize and Offset array
        * @param Font
        */
        protected void ReadFDArray(int Font)
        {
            Seek(fonts[Font].fdarrayOffset);
            fonts[Font].FDArrayCount = GetCard16();
            fonts[Font].FDArrayOffsize = GetCard8();
            // Since we will change values inside the FDArray objects
            // We increase its offsize to prevent errors 
            if (fonts[Font].FDArrayOffsize < 4)
                fonts[Font].FDArrayOffsize++;
            fonts[Font].FDArrayOffsets = GetIndex(fonts[Font].fdarrayOffset);
        }

        
        /**
        * The Process function extracts one font out of the CFF file and returns a
        * subset version of the original.
        * @param fontName - The name of the font to be taken out of the CFF
        * @return The new font stream
        * @throws IOException
        */
        public byte[] Process(String fontName) {
            try
            {    
                // Verify that the file is open
                buf.ReOpen();
                // Find the Font that we will be dealing with
                int j;
                for (j=0; j<fonts.Length; j++)
                    if (fontName.Equals(fonts[j].name)) break;
                if (j==fonts.Length) return null;
                
                // Calc the bias for the global subrs
                if (gsubrIndexOffset >= 0)
                    GBias = CalcBias(gsubrIndexOffset,j);

                // Prepare the new CharStrings Index
                BuildNewCharString(j);
                // Prepare the new Global and Local Subrs Indices
                BuildNewLGSubrs(j);
                // Build the new file 
                byte[] Ret = BuildNewFile(j);
                return Ret;
            }
            finally {
                try {
                    buf.Close();
                }
                catch {
                    // empty on purpose
                }
            }
        }

        /**
        * Function calcs bias according to the CharString type and the count
        * of the subrs
        * @param Offset The offset to the relevent subrs index
        * @param Font the font
        * @return The calculated Bias
        */
        protected int CalcBias(int Offset,int Font)
        {
            Seek(Offset);
            int nSubrs = GetCard16();
            // If type==1 -> bias=0 
            if (fonts[Font].CharstringType == 1)
                return 0;
            // else calc according to the count
            else if (nSubrs < 1240)
                return 107;
            else if (nSubrs < 33900)
                return 1131;
            else
                return 32768;
        }

        /**
        *Function uses BuildNewIndex to create the new index of the subset charstrings
        * @param FontIndex the font
        * @throws IOException
        */
        protected void BuildNewCharString(int FontIndex) 
        {
            NewCharStringsIndex = BuildNewIndex(fonts[FontIndex].charstringsOffsets,GlyphsUsed);
        }
        
        /**
        * Function builds the new local & global subsrs indices. IF CID then All of 
        * the FD Array lsubrs will be subsetted. 
        * @param Font the font
        * @throws IOException
        */
        protected void BuildNewLGSubrs(int Font)
        {
            // If the font is CID then the lsubrs are divided into FontDicts.
            // for each FD array the lsubrs will be subsetted.
            if (fonts[Font].isCID)
            {
                // Init the hasmap-array and the arraylist-array to hold the subrs used
                // in each private dict.
                hSubrsUsed = new Hashtable[fonts[Font].fdprivateOffsets.Length];
                lSubrsUsed = new ArrayList[fonts[Font].fdprivateOffsets.Length];
                // A [][] which will store the byte array for each new FD Array lsubs index
                NewLSubrsIndex = new byte[fonts[Font].fdprivateOffsets.Length][];
                // An array to hold the offset for each Lsubr index 
                fonts[Font].PrivateSubrsOffset = new int[fonts[Font].fdprivateOffsets.Length];
                // A [][] which will store the offset array for each lsubr index            
                fonts[Font].PrivateSubrsOffsetsArray = new int[fonts[Font].fdprivateOffsets.Length][];
                
                // Put the FDarrayUsed into a list
                ArrayList FDInList = new ArrayList(FDArrayUsed.Keys);
                // For each FD array which is used subset the lsubr 
                for (int j=0;j<FDInList.Count;j++)
                {
                    // The FDArray index, Hash Map, Arrat List to work on
                    int FD = (int)FDInList[j];
                    hSubrsUsed[FD] = new Hashtable();
                    lSubrsUsed[FD] = new ArrayList();
                    //Reads the private dicts looking for the subr operator and 
                    // store both the offest for the index and its offset array
                    BuildFDSubrsOffsets(Font,FD);
                    // Verify that FDPrivate has a LSubrs index
                    if (fonts[Font].PrivateSubrsOffset[FD]>=0)
                    {
                        //Scans the Charsting data storing the used Local and Global subroutines 
                        // by the glyphs. Scans the Subrs recursivley. 
                        BuildSubrUsed(Font,FD,fonts[Font].PrivateSubrsOffset[FD],fonts[Font].PrivateSubrsOffsetsArray[FD],hSubrsUsed[FD],lSubrsUsed[FD]);
                        // Builds the New Local Subrs index
                        NewLSubrsIndex[FD] = BuildNewIndex(fonts[Font].PrivateSubrsOffsetsArray[FD],hSubrsUsed[FD]);
                    }
                }
            }
            // If the font is not CID && the Private Subr exists then subset:
            else if (fonts[Font].privateSubrs>=0)
            {
                // Build the subrs offsets;
                fonts[Font].SubrsOffsets = GetIndex(fonts[Font].privateSubrs);
                //Scans the Charsting data storing the used Local and Global subroutines 
                // by the glyphs. Scans the Subrs recursivley.
                BuildSubrUsed(Font,-1,fonts[Font].privateSubrs,fonts[Font].SubrsOffsets,hSubrsUsedNonCID,lSubrsUsedNonCID);
            }
            // For all fonts susbset the Global Subroutines
            // Scan the Global Subr Hashmap recursivly on the Gsubrs
            BuildGSubrsUsed(Font);
            if (fonts[Font].privateSubrs>=0)
                // Builds the New Local Subrs index
                NewSubrsIndexNonCID = BuildNewIndex(fonts[Font].SubrsOffsets,hSubrsUsedNonCID);
            //Builds the New Global Subrs index
            NewGSubrsIndex = BuildNewIndex(gsubrOffsets,hGSubrsUsed);
        }

        /**
        * The function finds for the FD array processed the local subr offset and its 
        * offset array.  
        * @param Font the font
        * @param FD The FDARRAY processed
        */
        protected void BuildFDSubrsOffsets(int Font,int FD)
        {
            // Initiate to -1 to indicate lsubr operator present
            fonts[Font].PrivateSubrsOffset[FD] = -1;
            // Goto begining of objects
            Seek(fonts[Font].fdprivateOffsets[FD]);
            // While in the same object:
            while (GetPosition() < fonts[Font].fdprivateOffsets[FD]+fonts[Font].fdprivateLengths[FD])
            {
                GetDictItem();
                // If the dictItem is the "Subrs" then find and store offset, 
                if (key=="Subrs")
                    fonts[Font].PrivateSubrsOffset[FD] = (int)args[0]+fonts[Font].fdprivateOffsets[FD];
            }
            //Read the lsub index if the lsubr was found
            if (fonts[Font].PrivateSubrsOffset[FD] >= 0)
                fonts[Font].PrivateSubrsOffsetsArray[FD] = GetIndex(fonts[Font].PrivateSubrsOffset[FD]); 
        }

        /**
        * Function uses ReadAsubr on the glyph used to build the LSubr & Gsubr HashMap.
        * The HashMap (of the lsub only) is then scaned recursivly for Lsubr & Gsubrs
        * calls.  
        * @param Font the font
        * @param FD FD array processed. 0 indicates function was called by non CID font
        * @param SubrOffset the offset to the subr index to calc the bias
        * @param SubrsOffsets the offset array of the subr index
        * @param hSubr HashMap of the subrs used
        * @param lSubr ArrayList of the subrs used
        */
        protected void BuildSubrUsed(int Font,int FD,int SubrOffset,int[] SubrsOffsets,Hashtable hSubr,ArrayList lSubr)
        {

            // Calc the Bias for the subr index
            int LBias = CalcBias(SubrOffset,Font);
            
            // For each glyph used find its GID, start & end pos
            for (int i=0;i<glyphsInList.Count;i++)
            {
                int glyph = (int)glyphsInList[i];
                int Start = fonts[Font].charstringsOffsets[glyph];
                int End = fonts[Font].charstringsOffsets[glyph+1];
                
                // IF CID:
                if (FD >= 0)
                {
                    EmptyStack();
                    NumOfHints=0;
                    // Using FDSELECT find the FD Array the glyph belongs to.
                    int GlyphFD = fonts[Font].FDSelect[glyph];
                    // If the Glyph is part of the FD being processed 
                    if (GlyphFD == FD)
                        // Find the Subrs called by the glyph and insert to hash:
                        ReadASubr(Start,End,GBias,LBias,hSubr,lSubr,SubrsOffsets);
                }
                else
                    // If the font is not CID 
                    //Find the Subrs called by the glyph and insert to hash:
                    ReadASubr(Start,End,GBias,LBias,hSubr,lSubr,SubrsOffsets);
            }
            // For all Lsubrs used, check recusrivly for Lsubr & Gsubr used
            for (int i=0;i<lSubr.Count;i++)
            {
                // Pop the subr value from the hash
                int Subr = (int)lSubr[i];
                // Ensure the Lsubr call is valid
                if (Subr < SubrsOffsets.Length-1 && Subr>=0)
                {
                    // Read and process the subr
                    int Start = SubrsOffsets[Subr];
                    int End = SubrsOffsets[Subr+1];
                    ReadASubr(Start,End,GBias,LBias,hSubr,lSubr,SubrsOffsets);
                }
            }
        }
        
        /**
        * Function scans the Glsubr used ArrayList to find recursive calls 
        * to Gsubrs and adds to Hashmap & ArrayList
        * @param Font the font
        */
        protected void BuildGSubrsUsed(int Font)
        {
            int LBias = 0;
            int SizeOfNonCIDSubrsUsed = 0;
            if (fonts[Font].privateSubrs>=0)
            {
                LBias = CalcBias(fonts[Font].privateSubrs,Font);
                SizeOfNonCIDSubrsUsed = lSubrsUsedNonCID.Count;
            }
            
            // For each global subr used 
            for (int i=0;i<lGSubrsUsed.Count;i++)
            {
                //Pop the value + check valid 
                int Subr = (int)lGSubrsUsed[i];
                if (Subr < gsubrOffsets.Length-1 && Subr>=0)
                {
                    // Read the subr and process
                    int Start = gsubrOffsets[Subr];
                    int End = gsubrOffsets[Subr+1];
                    
                    if (fonts[Font].isCID)
                        ReadASubr(Start,End,GBias,0,hGSubrsUsed,lGSubrsUsed,null);
                    else
                    {
                        ReadASubr(Start,End,GBias,LBias,hSubrsUsedNonCID,lSubrsUsedNonCID,fonts[Font].SubrsOffsets);
                        if (SizeOfNonCIDSubrsUsed < lSubrsUsedNonCID.Count)
                        {
                            for (int j=SizeOfNonCIDSubrsUsed;j<lSubrsUsedNonCID.Count;j++)
                            {
                                //Pop the value + check valid 
                                int LSubr = (int)lSubrsUsedNonCID[j];
                                if (LSubr < fonts[Font].SubrsOffsets.Length-1 && LSubr>=0)
                                {
                                    // Read the subr and process
                                    int LStart = fonts[Font].SubrsOffsets[LSubr];
                                    int LEnd = fonts[Font].SubrsOffsets[LSubr+1];
                                    ReadASubr(LStart,LEnd,GBias,LBias,hSubrsUsedNonCID,lSubrsUsedNonCID,fonts[Font].SubrsOffsets);
                                }
                            }
                            SizeOfNonCIDSubrsUsed = lSubrsUsedNonCID.Count;
                        }
                    }
                }
            }
        }

        /**
        * The function reads a subrs (glyph info) between begin and end.
        * Adds calls to a Lsubr to the hSubr and lSubrs.
        * Adds calls to a Gsubr to the hGSubr and lGSubrs.
        * @param begin the start point of the subr
        * @param end the end point of the subr
        * @param GBias the bias of the Global Subrs
        * @param LBias the bias of the Local Subrs
        * @param hSubr the HashMap for the lSubrs
        * @param lSubr the ArrayList for the lSubrs
        */
        protected void ReadASubr(int begin,int end,int GBias,int LBias,Hashtable hSubr,ArrayList lSubr,int[] LSubrsOffsets)
        {
            // Clear the stack for the subrs
            EmptyStack();
            NumOfHints = 0;
            // Goto begining of the subr
            Seek(begin);
            while (GetPosition() < end)
            {
                // Read the next command
                ReadCommand();
                int pos = GetPosition();
                Object TopElement=null;
                if (arg_count > 0)
                    TopElement = args[arg_count-1];
                int NumOfArgs = arg_count;
                // Check the modification needed on the Argument Stack according to key;
                HandelStack();
                // a call to a Lsubr
                if (key=="callsubr") 
                {
                    // Verify that arguments are passed 
                    if (NumOfArgs > 0)
                    {
                        // Calc the index of the Subrs
                        int Subr = (int)TopElement + LBias;
                        // If the subr isn't in the HashMap -> Put in
                        if (!hSubr.ContainsKey(Subr))
                        {
                            hSubr[Subr] = null;
                            lSubr.Add(Subr);
                        }
                        CalcHints(LSubrsOffsets[Subr],LSubrsOffsets[Subr+1],LBias,GBias,LSubrsOffsets);
                        Seek(pos);
                    }                
                }
                // a call to a Gsubr
                else if (key=="callgsubr")
                {
                    // Verify that arguments are passed 
                    if (NumOfArgs > 0)
                    {
                        // Calc the index of the Subrs
                        int Subr = (int)TopElement + GBias;
                        // If the subr isn't in the HashMap -> Put in
                        if (!hGSubrsUsed.ContainsKey(Subr))
                        {
                            hGSubrsUsed[Subr] = null;
                            lGSubrsUsed.Add(Subr);
                        }
                        CalcHints(gsubrOffsets[Subr],gsubrOffsets[Subr+1],LBias,GBias,LSubrsOffsets);
                        Seek(pos);
                    }
                }
                // A call to "stem"
                else if (key == "hstem" || key == "vstem" || key == "hstemhm" || key == "vstemhm")
                    // Increment the NumOfHints by the number couples of of arguments
                    NumOfHints += NumOfArgs/2;
                // A call to "mask"
                else if (key == "hintmask" || key == "cntrmask")
                {
                    // Compute the size of the mask
                    int SizeOfMask = NumOfHints/8;
                    if (NumOfHints%8 != 0 || SizeOfMask == 0)
                        SizeOfMask++;
                    // Continue the pointer in SizeOfMask steps
                    for (int i=0;i<SizeOfMask;i++)
                        GetCard8();
                }
            }
        }

        /**
        * Function Checks how the current operator effects the run time stack after being run 
        * An operator may increase or decrease the stack size
        */
        protected void HandelStack()
        {
            // Findout what the operator does to the stack
            int StackHandel = StackOpp();
            if (StackHandel < 2)
            {
                // The operators that enlarge the stack by one
                if (StackHandel==1)
                    PushStack();
                // The operators that pop the stack
                else
                {
                    // Abs value for the for loop
                    StackHandel *= -1;
                    for (int i=0;i<StackHandel;i++)
                        PopStack();
                }
                
            }
            // All other flush the stack
            else
                EmptyStack();        
        }
        
        /**
        * Function checks the key and return the change to the stack after the operator
        * @return The change in the stack. 2-> flush the stack
        */
        protected int StackOpp()
        {
            if (key == "ifelse")
                return -3;
            if (key == "roll" || key == "put")
                return -2;
            if (key == "callsubr" || key == "callgsubr" || key == "add" || key == "sub" ||
                key == "div" || key == "mul" || key == "drop" || key == "and" || 
                key == "or" || key == "eq")
                return -1;
            if (key == "abs" || key == "neg" || key == "sqrt" || key == "exch" || 
                key == "index" || key == "get" || key == "not" || key == "return")
                return 0;
            if (key == "random" || key == "dup")
                return 1;
            return 2;
        }
        
        /**
        * Empty the Type2 Stack
        *
        */
        protected void EmptyStack()
        {
            // Null the arguments
            for (int i=0; i<arg_count; i++) args[i]=null;
            arg_count = 0;        
        }
        
        /**
        * Pop one element from the stack 
        *
        */
        protected void PopStack()
        {
            if (arg_count>0)
            {
                args[arg_count-1]=null;
                arg_count--;
            }
        }
        
        /**
        * Add an item to the stack
        *
        */
        protected void PushStack()
        {
            arg_count++;
        }
        
        /**
        * The function reads the next command after the file pointer is set
        */
        protected void ReadCommand()
        {
            key = null;
            bool gotKey = false;
            // Until a key is found
            while (!gotKey) {
                // Read the first Char
                char b0 = GetCard8();
                // decode according to the type1/type2 format
                if (b0 == 28) // the two next bytes represent a short int;
                {
                    int first = GetCard8();
                    int second = GetCard8();
                    args[arg_count] = first<<8 | second;
                    arg_count++;
                    continue;
                }
                if (b0 >= 32 && b0 <= 246) // The byte read is the byte;
                {
                    args[arg_count] = (int)b0 - 139;
                    arg_count++;
                    continue;
                }
                if (b0 >= 247 && b0 <= 250) // The byte read and the next byte constetute a short int
                {
                    int w = GetCard8();
                    args[arg_count] = ((int)b0-247)*256 + w + 108;
                    arg_count++;
                    continue;
                }
                if (b0 >= 251 && b0 <= 254)// Same as above except negative
                {
                    int w = GetCard8();
                    args[arg_count] = -((int)b0-251)*256 - w - 108;
                    arg_count++;
                    continue;
                }
                if (b0 == 255)// The next for bytes represent a double.
                {
                    int first = GetCard8();
                    int second = GetCard8();
                    int third = GetCard8();
                    int fourth = GetCard8();
                    args[arg_count] = first<<24 | second<<16 | third<<8 | fourth;
                    arg_count++;
                    continue;
                }
                if (b0<=31 && b0 != 28) // An operator was found.. Set Key.
                {
                    gotKey=true;
                    // 12 is an escape command therefor the next byte is a part
                    // of this command
                    if (b0 == 12)
                    {
                        int b1 = GetCard8();
                        if (b1>SubrsEscapeFuncs.Length-1)
                            b1 = SubrsEscapeFuncs.Length-1;
                        key = SubrsEscapeFuncs[b1];
                    }
                    else
                        key = SubrsFunctions[b0];
                    continue;
                }
            }        
        }
        
        /**
        * The function reads the subroutine and returns the number of the hint in it.
        * If a call to another subroutine is found the function calls recursively.
        * @param begin the start point of the subr
        * @param end the end point of the subr
        * @param LBias the bias of the Local Subrs
        * @param GBias the bias of the Global Subrs
        * @param LSubrsOffsets The Offsets array of the subroutines
        * @return The number of hints in the subroutine read.
        */
        protected int CalcHints(int begin,int end,int LBias,int GBias,int[] LSubrsOffsets)
        {
            // Goto begining of the subr
            Seek(begin);
            while (GetPosition() < end)
            {
                // Read the next command
                ReadCommand();
                int pos = GetPosition();
                Object TopElement = null;
                if (arg_count>0)
                    TopElement = args[arg_count-1];
                int NumOfArgs = arg_count;
                //Check the modification needed on the Argument Stack according to key;
                HandelStack();
                // a call to a Lsubr
                if (key=="callsubr") 
                {
                    if (NumOfArgs>0)
                    {
                        int Subr = (int)TopElement + LBias;
                        CalcHints(LSubrsOffsets[Subr],LSubrsOffsets[Subr+1],LBias,GBias,LSubrsOffsets);
                        Seek(pos);                    
                    }
                }
                // a call to a Gsubr
                else if (key=="callgsubr")
                {
                    if (NumOfArgs>0)
                    {
                        int Subr = (int)TopElement + GBias;
                        CalcHints(gsubrOffsets[Subr],gsubrOffsets[Subr+1],LBias,GBias,LSubrsOffsets);
                        Seek(pos);                    
                    }
                }
                // A call to "stem"
                else if (key == "hstem" || key == "vstem" || key == "hstemhm" || key == "vstemhm")
                    // Increment the NumOfHints by the number couples of of arguments
                    NumOfHints += NumOfArgs/2;
                // A call to "mask"
                else if (key == "hintmask" || key == "cntrmask")
                {
                    // Compute the size of the mask
                    int SizeOfMask = NumOfHints/8;
                    if (NumOfHints%8 != 0 || SizeOfMask == 0)
                        SizeOfMask++;
                    // Continue the pointer in SizeOfMask steps
                    for (int i=0;i<SizeOfMask;i++)
                        GetCard8();
                }
            }
            return NumOfHints;
        }


        /**
        * Function builds the new offset array, object array and assembles the index.
        * used for creating the glyph and subrs subsetted index 
        * @param Offsets the offset array of the original index  
        * @param Used the hashmap of the used objects
        * @return the new index subset version 
        * @throws IOException
        */
        protected byte[] BuildNewIndex(int[] Offsets,Hashtable Used) 
        {
            int Offset=0;
            int[] NewOffsets = new int[Offsets.Length];
            // Build the Offsets Array for the Subset
            for (int i=0;i<Offsets.Length;++i)
            {
                NewOffsets[i] = Offset;
                // If the object in the offset is also present in the used
                // HashMap then increment the offset var by its size
                if (Used.ContainsKey(i))
                    Offset += Offsets[i+1] - Offsets[i];
                    // Else the same offset is kept in i+1.
            }
            // Offset var determines the size of the object array
            byte[] NewObjects = new byte[Offset];
            // Build the new Object array
            for (int i=0;i<Offsets.Length-1;++i)
            {
                int start = NewOffsets[i];
                int end = NewOffsets[i+1];
                // If start != End then the Object is used
                // So, we will copy the object data from the font file
                if (start != end)
                {
                    // All offsets are Global Offsets relative to the begining of the font file.
                    // Jump the file pointer to the start address to read from.
                    buf.Seek(Offsets[i]);
                    // Read from the buffer and write into the array at start.  
                    buf.ReadFully(NewObjects, start, end-start);
                }
            }
            // Use AssembleIndex to build the index from the offset & object arrays
            return AssembleIndex(NewOffsets,NewObjects);
        }

        /**
        * Function creates the new index, inserting the count,offsetsize,offset array
        * and object array.
        * @param NewOffsets the subsetted offset array
        * @param NewObjects the subsetted object array
        * @return the new index created
        */
        protected byte[] AssembleIndex(int[] NewOffsets,byte[] NewObjects)
        {
            // Calc the index' count field
            char Count = (char)(NewOffsets.Length-1);
            // Calc the size of the object array
            int Size = NewOffsets[NewOffsets.Length-1];
            // Calc the Offsize
            byte Offsize;
            if (Size <= 0xff) Offsize = 1;
            else if (Size <= 0xffff) Offsize = 2;
            else if (Size <= 0xffffff) Offsize = 3;
            else Offsize = 4;
            // The byte array for the new index. The size is calc by
            // Count=2, Offsize=1, OffsetArray = Offsize*(Count+1), The object array
            byte[] NewIndex = new byte[2+1+Offsize*(Count+1)+NewObjects.Length];
            // The counter for writing
            int Place = 0;
            // Write the count field
            NewIndex[Place++] = (byte) ((Count >> 8) & 0xff);
            NewIndex[Place++] = (byte) ((Count >> 0) & 0xff);
            // Write the offsize field
            NewIndex[Place++] = Offsize;
            // Write the offset array according to the offsize
            for (int i=0;i<NewOffsets.Length;i++)
            {
                // The value to be written
                int Num = NewOffsets[i]-NewOffsets[0]+1;
                // Write in bytes according to the offsize
                switch (Offsize) {
                    case 4:
                        NewIndex[Place++] = (byte) ((Num >> 24) & 0xff);
                        goto case 3;
                    case 3:
                        NewIndex[Place++] = (byte) ((Num >> 16) & 0xff);
                        goto case 2;
                    case 2:
                        NewIndex[Place++] = (byte) ((Num >>  8) & 0xff);
                        goto case 1;
                    case 1:
                        NewIndex[Place++] = (byte) ((Num >>  0) & 0xff);
                        break;
                }                    
            }
            // Write the new object array one by one
            for (int i=0;i<NewObjects.Length;i++)
            {
                NewIndex[Place++] = NewObjects[i];
            }
            // Return the new index
            return NewIndex;
        }
        
        /**
        * The function builds the new output stream according to the subset process
        * @param Font the font
        * @return the subseted font stream
        * @throws IOException
        */
        protected byte[] BuildNewFile(int Font)
        {
            // Prepare linked list for new font components
            OutputList = new ArrayList();

            // copy the header of the font
            CopyHeader();
                    
            // create a name index
            BuildIndexHeader(1,1,1);
            OutputList.Add(new UInt8Item((char)( 1+fonts[Font].name.Length)));
            OutputList.Add(new StringItem(fonts[Font].name));
            
            // create the topdict Index
            BuildIndexHeader(1,2,1);
            OffsetItem topdictIndex1Ref = new IndexOffsetItem(2);
            OutputList.Add(topdictIndex1Ref);
            IndexBaseItem topdictBase = new IndexBaseItem();
            OutputList.Add(topdictBase);
                    
            // Initialise the Dict Items for later use
            OffsetItem charsetRef     = new DictOffsetItem();
            OffsetItem charstringsRef = new DictOffsetItem();
            OffsetItem fdarrayRef     = new DictOffsetItem();
            OffsetItem fdselectRef    = new DictOffsetItem();
            OffsetItem privateRef     = new DictOffsetItem();
            
            // If the font is not CID create the following keys
            if ( !fonts[Font].isCID ) {
                // create a ROS key
                OutputList.Add(new DictNumberItem(fonts[Font].nstrings));
                OutputList.Add(new DictNumberItem(fonts[Font].nstrings+1));
                OutputList.Add(new DictNumberItem(0));
                OutputList.Add(new UInt8Item((char)12));
                OutputList.Add(new UInt8Item((char)30));
                // create a CIDCount key
                OutputList.Add(new DictNumberItem(fonts[Font].nglyphs));
                OutputList.Add(new UInt8Item((char)12));
                OutputList.Add(new UInt8Item((char)34));
                // Sivan's comments
                // What about UIDBase (12,35)? Don't know what is it.
                // I don't think we need FontName; the font I looked at didn't have it.
            }
            // Go to the TopDict of the font being processed
            Seek(topdictOffsets[Font]);
            // Run untill the end of the TopDict
            while (GetPosition() < topdictOffsets[Font+1]) {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                // The encoding key is disregarded since CID has no encoding
                if (key=="Encoding"
                // These keys will be added manualy by the process.
                || key=="Private" 
                || key=="FDSelect"
                || key=="FDArray" 
                || key=="charset" 
                || key=="CharStrings"
                ) {
                }else {
                //OtherWise copy key "as is" to the output list
                    OutputList.Add(new RangeItem(buf,p1,p2-p1));
                }
            }
            // Create the FDArray, FDSelect, Charset and CharStrings Keys
            CreateKeys(fdarrayRef,fdselectRef,charsetRef,charstringsRef);
            
            // Mark the end of the top dict area
            OutputList.Add(new IndexMarkerItem(topdictIndex1Ref,topdictBase));
            
            // Copy the string index

            if (fonts[Font].isCID) 
                OutputList.Add(GetEntireIndexRange(stringIndexOffset));
            // If the font is not CID we need to append new strings.
            // We need 3 more strings: Registry, Ordering, and a FontName for one FD.
            // The total length is at most "Adobe"+"Identity"+63 = 76
            else
                CreateNewStringIndex(Font);
            
            // copy the new subsetted global subroutine index       
            OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewGSubrsIndex),0,NewGSubrsIndex.Length));
            
            // deal with fdarray, fdselect, and the font descriptors
            // If the font is CID:
            if (fonts[Font].isCID) {
                // copy the FDArray, FDSelect, charset
           
                // Copy FDSelect
                // Mark the beginning
                OutputList.Add(new MarkerItem(fdselectRef));
                // If an FDSelect exists copy it
                if (fonts[Font].fdselectOffset>=0)
                    OutputList.Add(new RangeItem(buf,fonts[Font].fdselectOffset,fonts[Font].FDSelectLength));
                // Else create a new one
                else
                    CreateFDSelect(fdselectRef,fonts[Font].nglyphs);
                               
                  // Copy the Charset
                // Mark the beginning and copy entirly 
                OutputList.Add(new MarkerItem(charsetRef));
                OutputList.Add(new RangeItem(buf,fonts[Font].charsetOffset,fonts[Font].CharsetLength));
                
                // Copy the FDArray
                // If an FDArray exists
                if (fonts[Font].fdarrayOffset>=0)
                {
                    // Mark the beginning
                    OutputList.Add(new MarkerItem(fdarrayRef));
                    // Build a new FDArray with its private dicts and their LSubrs
                    Reconstruct(Font);
                }
                else
                    // Else create a new one
                    CreateFDArray(fdarrayRef,privateRef,Font);
               
            }
            // If the font is not CID
            else 
            {
                // create FDSelect
                CreateFDSelect(fdselectRef,fonts[Font].nglyphs);            
                // recreate a new charset
                CreateCharset(charsetRef,fonts[Font].nglyphs);            
                // create a font dict index (fdarray)
                CreateFDArray(fdarrayRef,privateRef,Font);            
            }
            
            // if a private dict exists insert its subsetted version
            if (fonts[Font].privateOffset>=0)
            {
                // Mark the beginning of the private dict
                IndexBaseItem PrivateBase = new IndexBaseItem();
                OutputList.Add(PrivateBase);
                OutputList.Add(new MarkerItem(privateRef));

                OffsetItem Subr = new DictOffsetItem();
                // Build and copy the new private dict
                CreateNonCIDPrivate(Font,Subr);
                // Copy the new LSubrs index
                CreateNonCIDSubrs(Font,PrivateBase,Subr);
            }
            
            // copy the charstring index
            OutputList.Add(new MarkerItem(charstringsRef));

            // Add the subsetted charstring
            OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewCharStringsIndex),0,NewCharStringsIndex.Length));
            
            // now create the new CFF font        
            int[] currentOffset = new int[1];
            currentOffset[0] = 0;
            // Count and save the offset for each item
            foreach (Item item in OutputList) {
                item.Increment(currentOffset);
            }
            // Compute the Xref for each of the offset items
            foreach (Item item in OutputList) {
                item.Xref();
            }
            
            int size = currentOffset[0];
            byte[] b = new byte[size];
            
            // Emit all the items into the new byte array
            foreach (Item item in OutputList) {
                item.Emit(b);
            }
            // Return the new stream
            return b;
        }

        /**
        * Function Copies the header from the original fileto the output list
        */
        protected void CopyHeader()
        {
            Seek(0);
            int major = GetCard8();
            int minor = GetCard8();
            int hdrSize = GetCard8();
            int offSize = GetCard8();
            nextIndexOffset = hdrSize;
            OutputList.Add(new RangeItem(buf,0,hdrSize));
        }

        /**
        * Function Build the header of an index
        * @param Count the count field of the index
        * @param Offsize the offsize field of the index
        * @param First the first offset of the index
        */
        protected void BuildIndexHeader(int Count,int Offsize,int First)
        {
            // Add the count field
            OutputList.Add(new UInt16Item((char)Count)); // count
            // Add the offsize field
            OutputList.Add(new UInt8Item((char)Offsize)); // offSize
            // Add the first offset according to the offsize
            switch (Offsize){
                case 1:
                    OutputList.Add(new UInt8Item((char)First)); // first offset
                    break;
                case 2:
                    OutputList.Add(new UInt16Item((char)First)); // first offset
                    break;
                case 3:
                    OutputList.Add(new UInt24Item((char)First)); // first offset
                    break;
                case 4:
                    OutputList.Add(new UInt32Item((char)First)); // first offset
                    break;
                default:
                    break;    
            }
        }
        
        /**
        * Function adds the keys into the TopDict
        * @param fdarrayRef OffsetItem for the FDArray
        * @param fdselectRef OffsetItem for the FDSelect
        * @param charsetRef OffsetItem for the CharSet
        * @param charstringsRef OffsetItem for the CharString
        */
        protected void CreateKeys(OffsetItem fdarrayRef,OffsetItem fdselectRef,OffsetItem charsetRef,OffsetItem charstringsRef)
        {
            // create an FDArray key
            OutputList.Add(fdarrayRef);
            OutputList.Add(new UInt8Item((char)12));
            OutputList.Add(new UInt8Item((char)36));
            // create an FDSelect key
            OutputList.Add(fdselectRef);
            OutputList.Add(new UInt8Item((char)12));
            OutputList.Add(new UInt8Item((char)37));
            // create an charset key
            OutputList.Add(charsetRef);
            OutputList.Add(new UInt8Item((char)15));
            // create a CharStrings key
            OutputList.Add(charstringsRef);
            OutputList.Add(new UInt8Item((char)17));
        }
        
        /**
        * Function takes the original string item and adds the new strings
        * to accomodate the CID rules
        * @param Font the font
        */
        protected void CreateNewStringIndex(int Font)
        {
            String fdFontName = fonts[Font].name+"-OneRange";
            if (fdFontName.Length > 127)
                fdFontName = fdFontName.Substring(0,127);
            String extraStrings = "Adobe"+"Identity"+fdFontName;
            
            int origStringsLen = stringOffsets[stringOffsets.Length-1]
            - stringOffsets[0];
            int stringsBaseOffset = stringOffsets[0]-1;
            
            byte stringsIndexOffSize;
            if (origStringsLen+extraStrings.Length <= 0xff) stringsIndexOffSize = 1;
            else if (origStringsLen+extraStrings.Length <= 0xffff) stringsIndexOffSize = 2;
            else if (origStringsLen+extraStrings.Length <= 0xffffff) stringsIndexOffSize = 3;
            else stringsIndexOffSize = 4;
            
            OutputList.Add(new UInt16Item((char)((stringOffsets.Length-1)+3))); // count
            OutputList.Add(new UInt8Item((char)stringsIndexOffSize)); // offSize
            for (int i=0; i<stringOffsets.Length; i++)
                OutputList.Add(new IndexOffsetItem(stringsIndexOffSize,
                stringOffsets[i]-stringsBaseOffset));
            int currentStringsOffset = stringOffsets[stringOffsets.Length-1]
            - stringsBaseOffset;
            //l.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
            currentStringsOffset += ("Adobe").Length;
            OutputList.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
            currentStringsOffset += ("Identity").Length;
            OutputList.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
            currentStringsOffset += fdFontName.Length;
            OutputList.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
            
            OutputList.Add(new RangeItem(buf,stringOffsets[0],origStringsLen));
            OutputList.Add(new StringItem(extraStrings));
        }
         
        /**
        * Function creates new FDSelect for non-CID fonts.
        * The FDSelect built uses a single range for all glyphs
        * @param fdselectRef OffsetItem for the FDSelect
        * @param nglyphs the number of glyphs in the font
        */
        protected void CreateFDSelect(OffsetItem fdselectRef,int nglyphs)
        {
            OutputList.Add(new MarkerItem(fdselectRef));
            OutputList.Add(new UInt8Item((char)3)); // format identifier
            OutputList.Add(new UInt16Item((char)1)); // nRanges
            
            OutputList.Add(new UInt16Item((char)0)); // Range[0].firstGlyph
            OutputList.Add(new UInt8Item((char)0)); // Range[0].fd
            
            OutputList.Add(new UInt16Item((char)nglyphs)); // sentinel
        }

        /**
        * Function creates new CharSet for non-CID fonts.
        * The CharSet built uses a single range for all glyphs
        * @param charsetRef OffsetItem for the CharSet
        * @param nglyphs the number of glyphs in the font
        */
        protected void CreateCharset(OffsetItem charsetRef,int nglyphs)
        {
            OutputList.Add(new MarkerItem(charsetRef));
            OutputList.Add(new UInt8Item((char)2)); // format identifier
            OutputList.Add(new UInt16Item((char)1)); // first glyph in range (ignore .notdef)
            OutputList.Add(new UInt16Item((char)(nglyphs-1))); // nLeft
        }
        
        /**
        * Function creates new FDArray for non-CID fonts.
        * The FDArray built has only the "Private" operator that points to the font's
        * original private dict 
        * @param fdarrayRef OffsetItem for the FDArray
        * @param privateRef OffsetItem for the Private Dict
        * @param Font the font
        */
        protected void CreateFDArray(OffsetItem fdarrayRef,OffsetItem privateRef,int Font)
        {
            OutputList.Add(new MarkerItem(fdarrayRef));
            // Build the header (count=offsize=first=1)
            BuildIndexHeader(1,1,1);
            
            // Mark
            OffsetItem privateIndex1Ref = new IndexOffsetItem(1);
            OutputList.Add(privateIndex1Ref);
            IndexBaseItem privateBase = new IndexBaseItem();
            // Insert the private operands and operator
            OutputList.Add(privateBase);
            // Calc the new size of the private after subsetting
            // Origianl size
            int NewSize = fonts[Font].privateLength;
            // Calc the original size of the Subr offset in the private
            int OrgSubrsOffsetSize = CalcSubrOffsetSize(fonts[Font].privateOffset,fonts[Font].privateLength);
            // Increase the ptivate's size
            if (OrgSubrsOffsetSize != 0)
                NewSize += 5-OrgSubrsOffsetSize;
            OutputList.Add(new DictNumberItem(NewSize));
            OutputList.Add(privateRef);
            OutputList.Add(new UInt8Item((char)18)); // Private
            
            OutputList.Add(new IndexMarkerItem(privateIndex1Ref,privateBase));
        }
        
        /**
        * Function reconstructs the FDArray, PrivateDict and LSubr for CID fonts
        * @param Font the font
        * @throws IOException
        */
        void Reconstruct(int Font)
        {
            // Init for later use
            OffsetItem[] fdPrivate = new DictOffsetItem[fonts[Font].FDArrayOffsets.Length-1];
            IndexBaseItem[] fdPrivateBase = new IndexBaseItem[fonts[Font].fdprivateOffsets.Length]; 
            OffsetItem[] fdSubrs = new DictOffsetItem[fonts[Font].fdprivateOffsets.Length];
            // Reconstruct each type
            ReconstructFDArray(Font,fdPrivate);
            ReconstructPrivateDict(Font,fdPrivate,fdPrivateBase,fdSubrs);
            ReconstructPrivateSubrs(Font,fdPrivateBase,fdSubrs);
        }

        /**
        * Function subsets the FDArray and builds the new one with new offsets
        * @param Font The font
        * @param fdPrivate OffsetItem Array (one for each FDArray)
        * @throws IOException
        */
        void ReconstructFDArray(int Font,OffsetItem[] fdPrivate)
        {
            // Build the header of the index
            BuildIndexHeader(fonts[Font].FDArrayCount,fonts[Font].FDArrayOffsize,1);

            // For each offset create an Offset Item
            OffsetItem[] fdOffsets = new IndexOffsetItem[fonts[Font].FDArrayOffsets.Length-1];
            for (int i=0;i<fonts[Font].FDArrayOffsets.Length-1;i++)
            {
                fdOffsets[i] = new IndexOffsetItem(fonts[Font].FDArrayOffsize);
                OutputList.Add(fdOffsets[i]);
            }
            
            // Declare beginning of the object array
            IndexBaseItem fdArrayBase = new IndexBaseItem();
            OutputList.Add(fdArrayBase);
            
            // For each object check if that FD is used.
            // if is used build a new one by changing the private object
            // Else do nothing
            // At the end of each object mark its ending (Even if wasn't written)
            for (int k=0; k<fonts[Font].FDArrayOffsets.Length-1; k++) {
                if (FDArrayUsed.ContainsKey(k))
                {
                    // Goto begining of objects
                    Seek(fonts[Font].FDArrayOffsets[k]);
                    while (GetPosition() < fonts[Font].FDArrayOffsets[k+1])
                    {
                        int p1 = GetPosition();
                        GetDictItem();
                        int p2 = GetPosition();
                        // If the dictItem is the "Private" then compute and copy length, 
                        // use marker for offset and write operator number
                        if (key=="Private") {
                            // Save the original length of the private dict
                            int NewSize = (int)args[0];
                            // Save the size of the offset to the subrs in that private
                            int OrgSubrsOffsetSize = CalcSubrOffsetSize(fonts[Font].fdprivateOffsets[k],fonts[Font].fdprivateLengths[k]);
                            // Increase the private's length accordingly
                            if (OrgSubrsOffsetSize != 0)
                                NewSize += 5-OrgSubrsOffsetSize;
                            // Insert the new size, OffsetItem and operator key number
                            OutputList.Add(new DictNumberItem(NewSize));
                            fdPrivate[k] = new DictOffsetItem();
                            OutputList.Add(fdPrivate[k]);
                            OutputList.Add(new UInt8Item((char)18)); // Private
                            // Go back to place 
                            Seek(p2);
                        }
                        // Else copy the entire range
                        else  // other than private
                            OutputList.Add(new RangeItem(buf,p1,p2-p1));
                    }
                }
                // Mark the ending of the object (even if wasn't written)
                OutputList.Add(new IndexMarkerItem(fdOffsets[k],fdArrayBase));
            }
        }
        /**
        * Function Adds the new private dicts (only for the FDs used) to the list
        * @param Font the font
        * @param fdPrivate OffsetItem array one element for each private
        * @param fdPrivateBase IndexBaseItem array one element for each private
        * @param fdSubrs OffsetItem array one element for each private
        * @throws IOException
        */
        internal void ReconstructPrivateDict(int Font,OffsetItem[] fdPrivate,IndexBaseItem[] fdPrivateBase,
                OffsetItem[] fdSubrs)
        {
            
            // For each fdarray private dict check if that FD is used.
            // if is used build a new one by changing the subrs offset
            // Else do nothing
            for (int i=0;i<fonts[Font].fdprivateOffsets.Length;i++)
            {
                if (FDArrayUsed.ContainsKey(i))
                {
                    // Mark beginning
                    OutputList.Add(new MarkerItem(fdPrivate[i]));
                    fdPrivateBase[i] = new IndexBaseItem();
                    OutputList.Add(fdPrivateBase[i]);
                    // Goto begining of objects
                    Seek(fonts[Font].fdprivateOffsets[i]);
                    while (GetPosition() < fonts[Font].fdprivateOffsets[i]+fonts[Font].fdprivateLengths[i])
                    {
                        int p1 = GetPosition();
                        GetDictItem();
                        int p2 = GetPosition();
                        // If the dictItem is the "Subrs" then, 
                        // use marker for offset and write operator number
                        if (key=="Subrs") {
                            fdSubrs[i] = new DictOffsetItem();
                            OutputList.Add(fdSubrs[i]);
                            OutputList.Add(new UInt8Item((char)19)); // Subrs
                        }
                        // Else copy the entire range
                        else
                            OutputList.Add(new RangeItem(buf,p1,p2-p1));
                    }
                }
            }
        }
        
        /**
        * Function Adds the new LSubrs dicts (only for the FDs used) to the list
        * @param Font  The index of the font
        * @param fdPrivateBase The IndexBaseItem array for the linked list
        * @param fdSubrs OffsetItem array for the linked list
        * @throws IOException
        */
        
        internal void ReconstructPrivateSubrs(int Font,IndexBaseItem[] fdPrivateBase,
                OffsetItem[] fdSubrs)
        {
            // For each private dict
            for (int i=0;i<fonts[Font].fdprivateLengths.Length;i++)
            {
                // If that private dict's Subrs are used insert the new LSubrs
                // computed earlier
                if (fdSubrs[i]!= null && fonts[Font].PrivateSubrsOffset[i] >= 0)
                {                
                    OutputList.Add(new SubrMarkerItem(fdSubrs[i],fdPrivateBase[i]));
                    OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewLSubrsIndex[i]),0,NewLSubrsIndex[i].Length));
                }
            }
        }

        /**
        * Calculates how many byte it took to write the offset for the subrs in a specific
        * private dict.
        * @param Offset The Offset for the private dict
        * @param Size The size of the private dict
        * @return The size of the offset of the subrs in the private dict
        */
        internal int CalcSubrOffsetSize(int Offset,int Size)
        {
            // Set the size to 0
            int OffsetSize = 0;
            // Go to the beginning of the private dict
            Seek(Offset);
            // Go until the end of the private dict 
            while (GetPosition() < Offset+Size)
            {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                // When reached to the subrs offset
                if (key=="Subrs") {
                    // The Offsize (minus the subrs key)
                    OffsetSize = p2-p1-1;
                }
                // All other keys are ignored
            }
            // return the size
            return OffsetSize;
        }
        
        /**
        * Function computes the size of an index
        * @param indexOffset The offset for the computed index
        * @return The size of the index
        */
        protected int CountEntireIndexRange(int indexOffset) 
        {
            // Go to the beginning of the index 
            Seek(indexOffset);
            // Read the count field
            int count = GetCard16();
            // If count==0 -> size=2
            if (count==0) 
                return 2;
            else 
            {
                // Read the offsize field
                int indexOffSize = GetCard8();
                // Go to the last element of the offset array
                Seek(indexOffset+2+1+count*indexOffSize);
                // The size of the object array is the value of the last element-1
                int size = GetOffset(indexOffSize)-1;
                // Return the size of the entire index
                return 2+1+(count+1)*indexOffSize+size;
            }
        }
        
        /**
        * The function creates a private dict for a font that was not CID
        * All the keys are copied as is except for the subrs key 
        * @param Font the font
        * @param Subr The OffsetItem for the subrs of the private 
        */
        internal void CreateNonCIDPrivate(int Font,OffsetItem Subr)
        {
            // Go to the beginning of the private dict and read untill the end
            Seek(fonts[Font].privateOffset);
            while (GetPosition() < fonts[Font].privateOffset+fonts[Font].privateLength)
            {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                // If the dictItem is the "Subrs" then, 
                // use marker for offset and write operator number
                if (key=="Subrs") {
                    OutputList.Add(Subr);
                    OutputList.Add(new UInt8Item((char)19)); // Subrs
                }
                // Else copy the entire range
                else
                    OutputList.Add(new RangeItem(buf,p1,p2-p1));
            }
        }
        
        /**
        * the function marks the beginning of the subrs index and adds the subsetted subrs
        * index to the output list. 
        * @param Font the font
        * @param PrivateBase IndexBaseItem for the private that's referencing to the subrs
        * @param Subrs OffsetItem for the subrs
        * @throws IOException
        */
        internal void CreateNonCIDSubrs(int Font,IndexBaseItem PrivateBase,OffsetItem Subrs)
        {
            // Mark the beginning of the Subrs index
            OutputList.Add(new SubrMarkerItem(Subrs,PrivateBase));
            // Put the subsetted new subrs index
            OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewSubrsIndexNonCID),0,NewSubrsIndexNonCID.Length));
        }    
    }
}