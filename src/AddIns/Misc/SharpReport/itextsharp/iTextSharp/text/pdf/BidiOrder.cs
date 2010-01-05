using System;
/*
 * Copyright 2003 Paulo Soares
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

/*
 * (C) Copyright IBM Corp. 1999, All Rights Reserved
 *
 * version 1.1
 */

/*
 * As stated in the Javadoc comments below, materials from Unicode.org
 * are used in this class. The following license applies to these materials:
 * http://www.unicode.org/copyright.html#Exhibit1
 * 
 * EXHIBIT 1
 * UNICODE, INC. LICENSE AGREEMENT - DATA FILES AND SOFTWARE
 * 
 * Unicode Data Files include all data files under the directories
 * http://www.unicode.org/Public/, http://www.unicode.org/reports/,
 * and http://www.unicode.org/cldr/data/ .
 * Unicode Software includes any source code published in the Unicode Standard
 * or under the directories http://www.unicode.org/Public/, http://www.unicode.org/reports/,
 * and http://www.unicode.org/cldr/data/.
 * 
 * NOTICE TO USER: Carefully read the following legal agreement. BY DOWNLOADING,
 * INSTALLING, COPYING OR OTHERWISE USING UNICODE INC.'S DATA FILES ("DATA FILES"),
 * AND/OR SOFTWARE ("SOFTWARE"), YOU UNEQUIVOCALLY ACCEPT, AND AGREE TO BE BOUND BY,
 * ALL OF THE TERMS AND CONDITIONS OF THIS AGREEMENT. IF YOU DO NOT AGREE, DO NOT
 * DOWNLOAD, INSTALL, COPY, DISTRIBUTE OR USE THE DATA FILES OR SOFTWARE.
 * 
 * COPYRIGHT AND PERMISSION NOTICE
 * Copyright © 1991-2007 Unicode, Inc. All rights reserved. Distributed under
 * the Terms of Use in http://www.unicode.org/copyright.html.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of the Unicode data files and any associated documentation (the "Data Files")
 * or Unicode software and any associated documentation (the "Software") to deal
 * in the Data Files or Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, and/or sell copies
 * of the Data Files or Software, and to permit persons to whom the Data Files
 * or Software are furnished to do so, provided that (a) the above copyright
 * notice(s) and this permission notice appear with all copies of the Data Files
 * or Software, (b) both the above copyright notice(s) and this permission notice
 * appear in associated documentation, and (c) there is clear notice in each
 * modified Data File or in the Software as well as in the documentation associated
 * with the Data File(s) or Software that the data or software has been modified.
 * 
 * THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE
 * LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY
 * DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
 * CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA FILES OR SOFTWARE.
 * 
 * Except as contained in this notice, the name of a copyright holder shall not
 * be used in advertising or otherwise to promote the sale, use or other dealings
 * in these Data Files or Software without prior written authorization of the
 * copyright holder.
 */

/**
 * Reference implementation of the Unicode 3.0 Bidi algorithm.
 *
 * <p>
 * This implementation is not optimized for performance.  It is intended
 * as a reference implementation that closely follows the specification
 * of the Bidirectional Algorithm in The Unicode Standard version 3.0.
 * <p>
 * <b>Input:</b><br>
 * There are two levels of input to the algorithm, since clients may prefer
 * to supply some information from out-of-band sources rather than relying on
 * the default behavior.
 * <ol>
 * <li>unicode type array
 * <li>unicode type array, with externally supplied base line direction
 * </ol>
 * <p><b>Output:</b><br>
 * Output is separated into several stages as well, to better enable clients
 * to evaluate various aspects of implementation conformance.
 * <ol>
 * <li>levels array over entire paragraph
 * <li>reordering array over entire paragraph
 * <li>levels array over line
 * <li>reordering array over line
 * </ol>
 * Note that for conformance, algorithms are only required to generate correct
 * reordering and character directionality (odd or even levels) over a line.
 * Generating identical level arrays over a line is not required.  Bidi
 * explicit format codes (LRE, RLE, LRO, RLO, PDF) and BN can be assigned
 * arbitrary levels and positions as long as the other text matches.
 * <p>
 * As the algorithm is defined to operate on a single paragraph at a time,
 * this implementation is written to handle single paragraphs.  Thus
 * rule P1 is presumed by this implementation-- the data provided to the
 * implementation is assumed to be a single paragraph, and either contains no
 * 'B' codes, or a single 'B' code at the end of the input.  'B' is allowed
 * as input to illustrate how the algorithm assigns it a level.
 * <p>
 * Also note that rules L3 and L4 depend on the rendering engine that uses
 * the result of the bidi algorithm.  This implementation assumes that the
 * rendering engine expects combining marks in visual order (e.g. to the
 * left of their base character in RTL runs) and that it adjust the glyphs
 * used to render mirrored characters that are in RTL runs so that they
 * render appropriately.
 *
 * @author Doug Felt
 */

namespace iTextSharp.text.pdf {
    public sealed class BidiOrder {
        private sbyte[] initialTypes;
        private sbyte[] embeddings; // generated from processing format codes
        private sbyte paragraphEmbeddingLevel = -1; // undefined
        
        private int textLength; // for convenience
        private sbyte[] resultTypes; // for paragraph, not lines
        private sbyte[] resultLevels; // for paragraph, not lines
        
        // The bidi types
        
        /** Left-to-right*/
        public const sbyte L = 0;
        
        /** Left-to-Right Embedding */
        public const sbyte LRE = 1;
        
        /** Left-to-Right Override */
        public const sbyte LRO = 2;
        
        /** Right-to-Left */
        public const sbyte R = 3;
        
        /** Right-to-Left Arabic */
        public const sbyte AL = 4;
        
        /** Right-to-Left Embedding */
        public const sbyte RLE = 5;
        
        /** Right-to-Left Override */
        public const sbyte RLO = 6;
        
        /** Pop Directional Format */
        public const sbyte PDF = 7;
        
        /** European Number */
        public const sbyte EN = 8;
        
        /** European Number Separator */
        public const sbyte ES = 9;
        
        /** European Number Terminator */
        public const sbyte ET = 10;
        
        /** Arabic Number */
        public const sbyte AN = 11;
        
        /** Common Number Separator */
        public const sbyte CS = 12;
        
        /** Non-Spacing Mark */
        public const sbyte NSM = 13;
        
        /** Boundary Neutral */
        public const sbyte BN = 14;
        
        /** Paragraph Separator */
        public const sbyte B = 15;
        
        /** Segment Separator */
        public const sbyte S = 16;
        
        /** Whitespace */
        public const sbyte WS = 17;
        
        /** Other Neutrals */
        public const sbyte ON = 18;
        
        /** Minimum bidi type value. */
        public const sbyte TYPE_MIN = 0;
        
        /** Maximum bidi type value. */
        public const sbyte TYPE_MAX = 18;
        
        //
        // Input
        //
        
        /**
        * Initialize using an array of direction types.  Types range from TYPE_MIN to TYPE_MAX inclusive
        * and represent the direction codes of the characters in the text.
        *
        * @param types the types array
        */
        public BidiOrder(sbyte[] types) {
            ValidateTypes(types);
            
            this.initialTypes = (sbyte[])types.Clone(); // client type array remains unchanged
            
            RunAlgorithm();
        }
        
        /**
        * Initialize using an array of direction types and an externally supplied paragraph embedding level.
        * The embedding level may be -1, 0, or 1.  -1 means to apply the default algorithm (rules P2 and P3),
        * 0 is for LTR paragraphs, and 1 is for RTL paragraphs.
        *
        * @param types the types array
        * @param paragraphEmbeddingLevel the externally supplied paragraph embedding level.
        */
        public BidiOrder(sbyte[] types, sbyte paragraphEmbeddingLevel) {
            ValidateTypes(types);
            ValidateParagraphEmbeddingLevel(paragraphEmbeddingLevel);
            
            this.initialTypes = (sbyte[])types.Clone(); // client type array remains unchanged
            this.paragraphEmbeddingLevel = paragraphEmbeddingLevel;
            
            RunAlgorithm();
        }
        
        public BidiOrder(char[] text, int offset, int length, sbyte paragraphEmbeddingLevel) {
            initialTypes = new sbyte[length];
            for (int k = 0; k < length; ++k) {
                initialTypes[k] = rtypes[text[offset + k]];
            }
            ValidateParagraphEmbeddingLevel(paragraphEmbeddingLevel);
            
            this.paragraphEmbeddingLevel = paragraphEmbeddingLevel;
            
            RunAlgorithm();
        }
        
        public static sbyte GetDirection(char c) {
            return rtypes[c];
        }
        
        /**
        * The algorithm.
        * Does not include line-based processing (Rules L1, L2).
        * These are applied later in the line-based phase of the algorithm.
        */
        private void RunAlgorithm() {
            textLength = initialTypes.Length;
            
            // Initialize output types.
            // Result types initialized to input types.
            resultTypes = (sbyte[])initialTypes.Clone();
            
            
            // 1) determining the paragraph level
            // Rule P1 is the requirement for entering this algorithm.
            // Rules P2, P3.
            // If no externally supplied paragraph embedding level, use default.
            if (paragraphEmbeddingLevel == -1) {
                DetermineParagraphEmbeddingLevel();
            }
            
            // Initialize result levels to paragraph embedding level.
            resultLevels = new sbyte[textLength];
            SetLevels(0, textLength, paragraphEmbeddingLevel);
            
            // 2) Explicit levels and directions
            // Rules X1-X8.
            DetermineExplicitEmbeddingLevels();
            
            // Rule X9.
            textLength = RemoveExplicitCodes();
            
            // Rule X10.
            // Run remainder of algorithm one level run at a time
            sbyte prevLevel = paragraphEmbeddingLevel;
            int start = 0;
            while (start < textLength) {
                sbyte level = resultLevels[start];
                sbyte prevType = TypeForLevel(Math.Max(prevLevel, level));
                
                int limit = start + 1;
                while (limit < textLength && resultLevels[limit] == level) {
                    ++limit;
                }
                
                sbyte succLevel = limit < textLength ? resultLevels[limit] : paragraphEmbeddingLevel;
                sbyte succType = TypeForLevel(Math.Max(succLevel, level));
                
                // 3) resolving weak types
                // Rules W1-W7.
                ResolveWeakTypes(start, limit, level, prevType, succType);
                
                // 4) resolving neutral types
                // Rules N1-N3.
                ResolveNeutralTypes(start, limit, level, prevType, succType);
                
                // 5) resolving implicit embedding levels
                // Rules I1, I2.
                ResolveImplicitLevels(start, limit, level, prevType, succType);
                
                prevLevel = level;
                start = limit;
            }
            
            // Reinsert explicit codes and assign appropriate levels to 'hide' them.
            // This is for convenience, so the resulting level array maps 1-1
            // with the initial array.
            // See the implementation suggestions section of TR#9 for guidelines on
            // how to implement the algorithm without removing and reinserting the codes.
            textLength = ReinsertExplicitCodes(textLength);
        }
        
        /**
        * 1) determining the paragraph level.
        * <p>
        * Rules P2, P3.
        * <p>
        * At the end of this function, the member variable paragraphEmbeddingLevel is set to either 0 or 1.
        */
        private void DetermineParagraphEmbeddingLevel() {
            sbyte strongType = -1; // unknown
            
            // Rule P2.
            for (int i = 0; i < textLength; ++i) {
                sbyte t = resultTypes[i];
                if (t == L || t == AL || t == R) {
                    strongType = t;
                    break;
                }
            }
            
            // Rule P3.
            if (strongType == -1) { // none found
                // default embedding level when no strong types found is 0.
                paragraphEmbeddingLevel = 0;
            } else if (strongType == L) {
                paragraphEmbeddingLevel = 0;
            } else { // AL, R
                paragraphEmbeddingLevel = 1;
            }
        }
        
        /**
        * Process embedding format codes.
        * <p>
        * Calls processEmbeddings to generate an embedding array from the explicit format codes.  The
        * embedding overrides in the array are then applied to the result types, and the result levels are
        * initialized.
        * @see #processEmbeddings
        */
        private void DetermineExplicitEmbeddingLevels() {
            embeddings = ProcessEmbeddings(resultTypes, paragraphEmbeddingLevel);
            
            for (int i = 0; i < textLength; ++i) {
                sbyte level = embeddings[i];
                if ((level & 0x80) != 0) {
                    level &= 0x7f;
                    resultTypes[i] = TypeForLevel(level);
                }
                resultLevels[i] = level;
            }
        }
        
        /**
        * Rules X9.
        * Remove explicit codes so that they may be ignored during the remainder
        * of the main portion of the algorithm.  The length of the resulting text
        * is returned.
        * @return the length of the data excluding explicit codes and BN.
        */
        private int RemoveExplicitCodes() {
            int w = 0;
            for (int i = 0; i < textLength; ++i) {
                sbyte t = initialTypes[i];
                if (!(t == LRE || t == RLE || t == LRO || t == RLO || t == PDF || t == BN)) {
                    embeddings[w] = embeddings[i];
                    resultTypes[w] = resultTypes[i];
                    resultLevels[w] = resultLevels[i];
                    w++;
                }
            }
            return w; // new textLength while explicit levels are removed
        }
        
        /**
        * Reinsert levels information for explicit codes.
        * This is for ease of relating the level information
        * to the original input data.  Note that the levels
        * assigned to these codes are arbitrary, they're
        * chosen so as to avoid breaking level runs.
        * @param textLength the length of the data after compression
        * @return the length of the data (original length of
        * types array supplied to constructor)
        */
        private int ReinsertExplicitCodes(int textLength) {
            for (int i = initialTypes.Length; --i >= 0;) {
                sbyte t = initialTypes[i];
                if (t == LRE || t == RLE || t == LRO || t == RLO || t == PDF || t == BN) {
                    embeddings[i] = 0;
                    resultTypes[i] = t;
                    resultLevels[i] = -1;
                } else {
                    --textLength;
                    embeddings[i] = embeddings[textLength];
                    resultTypes[i] = resultTypes[textLength];
                    resultLevels[i] = resultLevels[textLength];
                }
            }
            
            // now propagate forward the levels information (could have
            // propagated backward, the main thing is not to introduce a level
            // break where one doesn't already exist).
            
            if (resultLevels[0] == -1) {
                resultLevels[0] = paragraphEmbeddingLevel;
            }
            for (int i = 1; i < initialTypes.Length; ++i) {
                if (resultLevels[i] == -1) {
                    resultLevels[i] = resultLevels[i-1];
                }
            }
            
            // Embedding information is for informational purposes only
            // so need not be adjusted.
            
            return initialTypes.Length;
        }
        
        /**
        * 2) determining explicit levels
        * Rules X1 - X8
        *
        * The interaction of these rules makes handling them a bit complex.
        * This examines resultTypes but does not modify it.  It returns embedding and
        * override information in the result array.  The low 7 bits are the level, the high
        * bit is set if the level is an override, and clear if it is an embedding.
        */
        private static sbyte[] ProcessEmbeddings(sbyte[] resultTypes, sbyte paragraphEmbeddingLevel) {
            int EXPLICIT_LEVEL_LIMIT = 62;
            
            int textLength = resultTypes.Length;
            sbyte[] embeddings = new sbyte[textLength];
            
            // This stack will store the embedding levels and override status in a single sbyte
            // as described above.
            sbyte[] embeddingValueStack = new sbyte[EXPLICIT_LEVEL_LIMIT];
            int stackCounter = 0;
            
            // An LRE or LRO at level 60 is invalid, since the new level 62 is invalid.  But
            // an RLE at level 60 is valid, since the new level 61 is valid.  The current wording
            // of the rules requires that the RLE remain valid even if a previous LRE is invalid.
            // This keeps track of ignored LRE or LRO codes at level 60, so that the matching PDFs
            // will not try to pop the stack.
            int overflowAlmostCounter = 0;
            
            // This keeps track of ignored pushes at level 61 or higher, so that matching PDFs will
            // not try to pop the stack.
            int overflowCounter = 0;
            
            // Rule X1.
            
            // Keep the level separate from the value (level | override status flag) for ease of access.
            sbyte currentEmbeddingLevel = paragraphEmbeddingLevel;
            sbyte currentEmbeddingValue = paragraphEmbeddingLevel;
            
            // Loop through types, handling all remaining rules
            for (int i = 0; i < textLength; ++i) {
                
                embeddings[i] = currentEmbeddingValue;
                
                sbyte t = resultTypes[i];
                
                // Rules X2, X3, X4, X5
                switch (t) {
                    case RLE:
                    case LRE:
                    case RLO:
                    case LRO:
                        // Only need to compute new level if current level is valid
                        if (overflowCounter == 0) {
                            sbyte newLevel;
                            if (t == RLE || t == RLO) {
                                newLevel = (sbyte)((currentEmbeddingLevel + 1) | 1); // least greater odd
                            } else { // t == LRE || t == LRO
                                newLevel = (sbyte)((currentEmbeddingLevel + 2) & ~1); // least greater even
                            }
                            
                            // If the new level is valid, push old embedding level and override status
                            // No check for valid stack counter, since the level check suffices.
                            if (newLevel < EXPLICIT_LEVEL_LIMIT) {
                                embeddingValueStack[stackCounter] = currentEmbeddingValue;
                                stackCounter++;
                                
                                currentEmbeddingLevel = newLevel;
                                if (t == LRO || t == RLO) { // override
                                    currentEmbeddingValue = (sbyte)((byte)newLevel | 0x80);
                                } else {
                                    currentEmbeddingValue = newLevel;
                                }
                                
                                // Adjust level of format mark (for expositional purposes only, this gets
                                // removed later).
                                embeddings[i] = currentEmbeddingValue;
                                break;
                            }
                            
                            // Otherwise new level is invalid, but a valid level can still be achieved if this
                            // level is 60 and we encounter an RLE or RLO further on.  So record that we
                            // 'almost' overflowed.
                            if (currentEmbeddingLevel == 60) {
                                overflowAlmostCounter++;
                                break;
                            }
                        }
                        
                        // Otherwise old or new level is invalid.
                        overflowCounter++;
                        break;
                        
                    case PDF:
                        // The only case where this did not actually overflow but may have almost overflowed
                        // is when there was an RLE or RLO on level 60, which would result in level 61.  So we
                        // only test the almost overflow condition in that case.
                        //
                        // Also note that there may be a PDF without any pushes at all.
                        
                        if (overflowCounter > 0) {
                            --overflowCounter;
                        } else if (overflowAlmostCounter > 0 && currentEmbeddingLevel != 61) {
                            --overflowAlmostCounter;
                        } else if (stackCounter > 0) {
                            --stackCounter;
                            currentEmbeddingValue = embeddingValueStack[stackCounter];
                            currentEmbeddingLevel = (sbyte)(currentEmbeddingValue & 0x7f);
                        }
                        break;
                        
                    case B:
                        // Rule X8.
                        
                        // These values are reset for clarity, in this implementation B can only
                        // occur as the last code in the array.
                        stackCounter = 0;
                        overflowCounter = 0;
                        overflowAlmostCounter = 0;
                        currentEmbeddingLevel = paragraphEmbeddingLevel;
                        currentEmbeddingValue = paragraphEmbeddingLevel;
                        
                        embeddings[i] = paragraphEmbeddingLevel;
                        break;
                        
                    default:
                        break;
                }
            }
            
            return embeddings;
        }
        
        
        /**
        * 3) resolving weak types
        * Rules W1-W7.
        *
        * Note that some weak types (EN, AN) remain after this processing is complete.
        */
        private void ResolveWeakTypes(int start, int limit, sbyte level, sbyte sor, sbyte eor) {
            
            // Rule W1.
            // Changes all NSMs.
            sbyte preceedingCharacterType = sor;
            for (int i = start; i < limit; ++i) {
                sbyte t = resultTypes[i];
                if (t == NSM) {
                    resultTypes[i] = preceedingCharacterType;
                } else {
                    preceedingCharacterType = t;
                }
            }
            
            // Rule W2.
            // EN does not change at the start of the run, because sor != AL.
            for (int i = start; i < limit; ++i) {
                if (resultTypes[i] == EN) {
                    for (int j = i - 1; j >= start; --j) {
                        sbyte t = resultTypes[j];
                        if (t == L || t == R || t == AL) {
                            if (t == AL) {
                                resultTypes[i] = AN;
                            }
                            break;
                        }
                    }
                }
            }
            
            // Rule W3.
            for (int i = start; i < limit; ++i) {
                if (resultTypes[i] == AL) {
                    resultTypes[i] = R;
                }
            }
            
            // Rule W4.
            // Since there must be values on both sides for this rule to have an
            // effect, the scan skips the first and last value.
            //
            // Although the scan proceeds left to right, and changes the type values
            // in a way that would appear to affect the computations later in the scan,
            // there is actually no problem.  A change in the current value can only
            // affect the value to its immediate right, and only affect it if it is
            // ES or CS.  But the current value can only change if the value to its
            // right is not ES or CS.  Thus either the current value will not change,
            // or its change will have no effect on the remainder of the analysis.
            
            for (int i = start + 1; i < limit - 1; ++i) {
                if (resultTypes[i] == ES || resultTypes[i] == CS) {
                    sbyte prevSepType = resultTypes[i-1];
                    sbyte succSepType = resultTypes[i+1];
                    if (prevSepType == EN && succSepType == EN) {
                        resultTypes[i] = EN;
                    } else if (resultTypes[i] == CS && prevSepType == AN && succSepType == AN) {
                        resultTypes[i] = AN;
                    }
                }
            }
            
            // Rule W5.
            for (int i = start; i < limit; ++i) {
                if (resultTypes[i] == ET) {
                    // locate end of sequence
                    int runstart = i;
                    int runlimit = FindRunLimit(runstart, limit, new sbyte[] { ET });
                    
                    // check values at ends of sequence
                    sbyte t = runstart == start ? sor : resultTypes[runstart - 1];
                    
                    if (t != EN) {
                        t = runlimit == limit ? eor : resultTypes[runlimit];
                    }
                    
                    if (t == EN) {
                        SetTypes(runstart, runlimit, EN);
                    }
                    
                    // continue at end of sequence
                    i = runlimit;
                }
            }
            
            // Rule W6.
            for (int i = start; i < limit; ++i) {
                sbyte t = resultTypes[i];
                if (t == ES || t == ET || t == CS) {
                    resultTypes[i] = ON;
                }
            }
            
            // Rule W7.
            for (int i = start; i < limit; ++i) {
                if (resultTypes[i] == EN) {
                    // set default if we reach start of run
                    sbyte prevStrongType = sor;
                    for (int j = i - 1; j >= start; --j) {
                        sbyte t = resultTypes[j];
                        if (t == L || t == R) { // AL's have been removed
                            prevStrongType = t;
                            break;
                        }
                    }
                    if (prevStrongType == L) {
                        resultTypes[i] = L;
                    }
                }
            }
        }
        
        /**
        * 6) resolving neutral types
        * Rules N1-N2.
        */
        private void ResolveNeutralTypes(int start, int limit, sbyte level, sbyte sor, sbyte eor) {
            
            for (int i = start; i < limit; ++i) {
                sbyte t = resultTypes[i];
                if (t == WS || t == ON || t == B || t == S) {
                    // find bounds of run of neutrals
                    int runstart = i;
                    int runlimit = FindRunLimit(runstart, limit, new sbyte[] {B, S, WS, ON});
                    
                    // determine effective types at ends of run
                    sbyte leadingType;
                    sbyte trailingType;
                    
                    if (runstart == start) {
                        leadingType = sor;
                    } else {
                        leadingType = resultTypes[runstart - 1];
                        if (leadingType == L || leadingType == R) {
                            // found the strong type
                        } else if (leadingType == AN) {
                            leadingType = R;
                        } else if (leadingType == EN) {
                            // Since EN's with previous strong L types have been changed
                            // to L in W7, the leadingType must be R.
                            leadingType = R;
                        }
                    }
                    
                    if (runlimit == limit) {
                        trailingType = eor;
                    } else {
                        trailingType = resultTypes[runlimit];
                        if (trailingType == L || trailingType == R) {
                            // found the strong type
                        } else if (trailingType == AN) {
                            trailingType = R;
                        } else if (trailingType == EN) {
                            trailingType = R;
                        }
                    }
                    
                    sbyte resolvedType;
                    if (leadingType == trailingType) {
                        // Rule N1.
                        resolvedType = leadingType;
                    } else {
                        // Rule N2.
                        // Notice the embedding level of the run is used, not
                        // the paragraph embedding level.
                        resolvedType = TypeForLevel(level);
                    }
                    
                    SetTypes(runstart, runlimit, resolvedType);
                    
                    // skip over run of (former) neutrals
                    i = runlimit;
                }
            }
        }
        
        /**
        * 7) resolving implicit embedding levels
        * Rules I1, I2.
        */
        private void ResolveImplicitLevels(int start, int limit, sbyte level, sbyte sor, sbyte eor) {
            if ((level & 1) == 0) { // even level
                for (int i = start; i < limit; ++i) {
                    sbyte t = resultTypes[i];
                    // Rule I1.
                    if (t == L ) {
                        // no change
                    } else if (t == R) {
                        resultLevels[i] += 1;
                    } else { // t == AN || t == EN
                        resultLevels[i] += 2;
                    }
                }
            } else { // odd level
                for (int i = start; i < limit; ++i) {
                    sbyte t = resultTypes[i];
                    // Rule I2.
                    if (t == R) {
                        // no change
                    } else { // t == L || t == AN || t == EN
                        resultLevels[i] += 1;
                    }
                }
            }
        }
        
        //
        // Output
        //
        
        public byte[] GetLevels() {
            return GetLevels(new int[]{textLength});
        }
        
        /**
        * Return levels array breaking lines at offsets in linebreaks. <br>
        * Rule L1.
        * <p>
        * The returned levels array contains the resolved level for each
        * bidi code passed to the constructor.
        * <p>
        * The linebreaks array must include at least one value.
        * The values must be in strictly increasing order (no duplicates)
        * between 1 and the length of the text, inclusive.  The last value
        * must be the length of the text.
        *
        * @param linebreaks the offsets at which to break the paragraph
        * @return the resolved levels of the text
        */
        public byte[] GetLevels(int[] linebreaks) {
            
            // Note that since the previous processing has removed all
            // P, S, and WS values from resultTypes, the values referred to
            // in these rules are the initial types, before any processing
            // has been applied (including processing of overrides).
            //
            // This example implementation has reinserted explicit format codes
            // and BN, in order that the levels array correspond to the
            // initial text.  Their final placement is not normative.
            // These codes are treated like WS in this implementation,
            // so they don't interrupt sequences of WS.
            
            ValidateLineBreaks(linebreaks, textLength);
            
            byte[] result = new byte[resultLevels.Length];
            for (int k = 0; k < resultLevels.Length; ++k)
                result[k] = (byte)resultLevels[k];
            
            // don't worry about linebreaks since if there is a break within
            // a series of WS values preceeding S, the linebreak itself
            // causes the reset.
            for (int i = 0; i < result.Length; ++i) {
                sbyte t = initialTypes[i];
                if (t == B || t == S) {
                    // Rule L1, clauses one and two.
                    result[i] = (byte)paragraphEmbeddingLevel;
                    
                    // Rule L1, clause three.
                    for (int j = i - 1; j >= 0; --j) {
                        if (IsWhitespace(initialTypes[j])) { // including format codes
                            result[j] = (byte)paragraphEmbeddingLevel;
                        } else {
                            break;
                        }
                    }
                }
            }
            
            // Rule L1, clause four.
            int start = 0;
            for (int i = 0; i < linebreaks.Length; ++i) {
                int limit = linebreaks[i];
                for (int j = limit - 1; j >= start; --j) {
                    if (IsWhitespace(initialTypes[j])) { // including format codes
                        result[j] = (byte)paragraphEmbeddingLevel;
                    } else {
                        break;
                    }
                }
                
                start = limit;
            }
            
            return result;
        }
        
        /**
        * Return reordering array breaking lines at offsets in linebreaks.
        * <p>
        * The reordering array maps from a visual index to a logical index.
        * Lines are concatenated from left to right.  So for example, the
        * fifth character from the left on the third line is
        * <pre> GetReordering(linebreaks)[linebreaks[1] + 4]</pre>
        * (linebreaks[1] is the position after the last character of the
        * second line, which is also the index of the first character on the
        * third line, and adding four gets the fifth character from the left).
        * <p>
        * The linebreaks array must include at least one value.
        * The values must be in strictly increasing order (no duplicates)
        * between 1 and the length of the text, inclusive.  The last value
        * must be the length of the text.
        *
        * @param linebreaks the offsets at which to break the paragraph.
        */
/*        public int[] GetReordering(int[] linebreaks) {
            ValidateLineBreaks(linebreaks, textLength);
            
            sbyte[] levels = GetLevels(linebreaks);
            
            return ComputeMultilineReordering(levels, linebreaks);
        }*/
        
        /**
        * Return multiline reordering array for a given level array.
        * Reordering does not occur across a line break.
        */
        private static int[] ComputeMultilineReordering(sbyte[] levels, int[] linebreaks) {
            int[] result = new int[levels.Length];
            
            int start = 0;
            for (int i = 0; i < linebreaks.Length; ++i) {
                int limit = linebreaks[i];
                
                sbyte[] templevels = new sbyte[limit - start];
                Array.Copy(levels, start, templevels, 0, templevels.Length);
                
                int[] temporder = ComputeReordering(templevels);
                for (int j = 0; j < temporder.Length; ++j) {
                    result[start + j] = temporder[j] + start;
                }
                
                start = limit;
            }
            
            return result;
        }
        
        /**
        * Return reordering array for a given level array.  This reorders a single line.
        * The reordering is a visual to logical map.  For example,
        * the leftmost char is string.CharAt(order[0]).
        * Rule L2.
        */
        private static int[] ComputeReordering(sbyte[] levels) {
            int lineLength = levels.Length;
            
            int[] result = new int[lineLength];
            
            // initialize order
            for (int i = 0; i < lineLength; ++i) {
                result[i] = i;
            }
            
            // locate highest level found on line.
            // Note the rules say text, but no reordering across line bounds is performed,
            // so this is sufficient.
            sbyte highestLevel = 0;
            sbyte lowestOddLevel = 63;
            for (int i = 0; i < lineLength; ++i) {
                sbyte level = levels[i];
                if (level > highestLevel) {
                    highestLevel = level;
                }
                if (((level & 1) != 0) && level < lowestOddLevel) {
                    lowestOddLevel = level;
                }
            }
            
            for (int level = highestLevel; level >= lowestOddLevel; --level) {
                for (int i = 0; i < lineLength; ++i) {
                    if (levels[i] >= level) {
                        // find range of text at or above this level
                        int start = i;
                        int limit = i + 1;
                        while (limit < lineLength && levels[limit] >= level) {
                            ++limit;
                        }
                        
                        // reverse run
                        for (int j = start, k = limit - 1; j < k; ++j, --k) {
                            int temp = result[j];
                            result[j] = result[k];
                            result[k] = temp;
                        }
                        
                        // skip to end of level run
                        i = limit;
                    }
                }
            }
            
            return result;
        }
        
        /**
        * Return the base level of the paragraph.
        */
        public sbyte GetBaseLevel() {
            return paragraphEmbeddingLevel;
        }
        
        // --- internal utilities -------------------------------------------------
        
        /**
        * Return true if the type is considered a whitespace type for the line break rules.
        */
        private static bool IsWhitespace(sbyte biditype) {
            switch (biditype) {
                case LRE:
                case RLE:
                case LRO:
                case RLO:
                case PDF:
                case BN:
                case WS:
                    return true;
                default:
                    return false;
            }
        }
        
        /**
        * Return the strong type (L or R) corresponding to the level.
        */
        private static sbyte TypeForLevel(int level) {
            return ((level & 0x1) == 0) ? L : R;
        }
        
        /**
        * Return the limit of the run starting at index that includes only resultTypes in validSet.
        * This checks the value at index, and will return index if that value is not in validSet.
        */
        private int FindRunLimit(int index, int limit, sbyte[] validSet) {
            --index;
            loop:
                while (++index < limit) {
                    sbyte t = resultTypes[index];
                    for (int i = 0; i < validSet.Length; ++i) {
                        if (t == validSet[i]) {
                            goto loop;
                        }
                    }
                    // didn't find a match in validSet
                    return index;
                }
                return limit;
        }
        
        /**
        * Return the start of the run including index that includes only resultTypes in validSet.
        * This assumes the value at index is valid, and does not check it.
        */
        private int FindRunStart(int index, sbyte[] validSet) {
            loop:
                while (--index >= 0) {
                    sbyte t = resultTypes[index];
                    for (int i = 0; i < validSet.Length; ++i) {
                        if (t == validSet[i]) {
                            goto loop;
                        }
                    }
                    return index + 1;
                }
                return 0;
        }
        
        /**
        * Set resultTypes from start up to (but not including) limit to newType.
        */
        private void SetTypes(int start, int limit, sbyte newType) {
            for (int i = start; i < limit; ++i) {
                resultTypes[i] = newType;
            }
        }
        
        /**
        * Set resultLevels from start up to (but not including) limit to newLevel.
        */
        private void SetLevels(int start, int limit, sbyte newLevel) {
            for (int i = start; i < limit; ++i) {
                resultLevels[i] = newLevel;
            }
        }
        
        // --- input validation ---------------------------------------------------
        
        /**
        * Throw exception if type array is invalid.
        */
        private static void ValidateTypes(sbyte[] types) {
            if (types == null) {
                throw new ArgumentException("types is null");
            }
            for (int i = 0; i < types.Length; ++i) {
                if (types[i] < TYPE_MIN || types[i] > TYPE_MAX) {
                    throw new ArgumentException("illegal type value at " + i + ": " + types[i]);
                }
            }
            for (int i = 0; i < types.Length - 1; ++i) {
                if (types[i] == B) {
                    throw new ArgumentException("B type before end of paragraph at index: " + i);
                }
            }
        }
        
        /**
        * Throw exception if paragraph embedding level is invalid. Special allowance for -1 so that
        * default processing can still be performed when using this API.
        */
        private static void ValidateParagraphEmbeddingLevel(sbyte paragraphEmbeddingLevel) {
            if (paragraphEmbeddingLevel != -1 &&
            paragraphEmbeddingLevel != 0 &&
            paragraphEmbeddingLevel != 1) {
                throw new ArgumentException("illegal paragraph embedding level: " + paragraphEmbeddingLevel);
            }
        }
        
        /**
        * Throw exception if line breaks array is invalid.
        */
        private static void ValidateLineBreaks(int[] linebreaks, int textLength) {
            int prev = 0;
            for (int i = 0; i < linebreaks.Length; ++i) {
                int next = linebreaks[i];
                if (next <= prev) {
                    throw new ArgumentException("bad linebreak: " + next + " at index: " + i);
                }
                prev = next;
            }
            if (prev != textLength) {
                throw new ArgumentException("last linebreak must be at " + textLength);
            }
        }
        
        private static sbyte[] rtypes = new sbyte[0x10000];
        
        private static char[] baseTypes = {
            (char)0, (char)8, (char)BN, (char)9, (char)9, (char)S, (char)10, (char)10, (char)B, (char)11, (char)11, (char)S, (char)12, (char)12, (char)WS, (char)13, (char)13, (char)B,
            (char)14, (char)27, (char)BN, (char)28, (char)30, (char)B, (char)31, (char)31, (char)S, (char)32, (char)32, (char)WS, (char)33, (char)34, (char)ON, (char)35, (char)37, (char)ET,
            (char)38, (char)42, (char)ON, (char)43, (char)43, (char)ET, (char)44, (char)44, (char)CS, (char)45, (char)45, (char)ET, (char)46, (char)46, (char)CS, (char)47, (char)47, (char)ES,
            (char)48, (char)57, (char)EN, (char)58, (char)58, (char)CS, (char)59, (char)64, (char)ON, (char)65, (char)90, (char)L, (char)91, (char)96, (char)ON, (char)97, (char)122, (char)L,
            (char)123, (char)126, (char)ON, (char)127, (char)132, (char)BN, (char)133, (char)133, (char)B, (char)134, (char)159, (char)BN, (char)160, (char)160, (char)CS,
            (char)161, (char)161, (char)ON, (char)162, (char)165, (char)ET, (char)166, (char)169, (char)ON, (char)170, (char)170, (char)L, (char)171, (char)175, (char)ON,
            (char)176, (char)177, (char)ET, (char)178, (char)179, (char)EN, (char)180, (char)180, (char)ON, (char)181, (char)181, (char)L, (char)182, (char)184, (char)ON,
            (char)185, (char)185, (char)EN, (char)186, (char)186, (char)L, (char)187, (char)191, (char)ON, (char)192, (char)214, (char)L, (char)215, (char)215, (char)ON,
            (char)216, (char)246, (char)L, (char)247, (char)247, (char)ON, (char)248, (char)696, (char)L, (char)697, (char)698, (char)ON, (char)699, (char)705, (char)L,
            (char)706, (char)719, (char)ON, (char)720, (char)721, (char)L, (char)722, (char)735, (char)ON, (char)736, (char)740, (char)L, (char)741, (char)749, (char)ON,
            (char)750, (char)750, (char)L, (char)751, (char)767, (char)ON, (char)768, (char)855, (char)NSM, (char)856, (char)860, (char)L, (char)861, (char)879, (char)NSM,
            (char)880, (char)883, (char)L, (char)884, (char)885, (char)ON, (char)886, (char)893, (char)L, (char)894, (char)894, (char)ON, (char)895, (char)899, (char)L,
            (char)900, (char)901, (char)ON, (char)902, (char)902, (char)L, (char)903, (char)903, (char)ON, (char)904, (char)1013, (char)L, (char)1014, (char)1014, (char)ON,
            (char)1015, (char)1154, (char)L, (char)1155, (char)1158, (char)NSM, (char)1159, (char)1159, (char)L, (char)1160, (char)1161, (char)NSM,
            (char)1162, (char)1417, (char)L, (char)1418, (char)1418, (char)ON, (char)1419, (char)1424, (char)L, (char)1425, (char)1441, (char)NSM,
            (char)1442, (char)1442, (char)L, (char)1443, (char)1465, (char)NSM, (char)1466, (char)1466, (char)L, (char)1467, (char)1469, (char)NSM,
            (char)1470, (char)1470, (char)R, (char)1471, (char)1471, (char)NSM, (char)1472, (char)1472, (char)R, (char)1473, (char)1474, (char)NSM,
            (char)1475, (char)1475, (char)R, (char)1476, (char)1476, (char)NSM, (char)1477, (char)1487, (char)L, (char)1488, (char)1514, (char)R,
            (char)1515, (char)1519, (char)L, (char)1520, (char)1524, (char)R, (char)1525, (char)1535, (char)L, (char)1536, (char)1539, (char)AL,
            (char)1540, (char)1547, (char)L, (char)1548, (char)1548, (char)CS, (char)1549, (char)1549, (char)AL, (char)1550, (char)1551, (char)ON,
            (char)1552, (char)1557, (char)NSM, (char)1558, (char)1562, (char)L, (char)1563, (char)1563, (char)AL, (char)1564, (char)1566, (char)L,
            (char)1567, (char)1567, (char)AL, (char)1568, (char)1568, (char)L, (char)1569, (char)1594, (char)AL, (char)1595, (char)1599, (char)L,
            (char)1600, (char)1610, (char)AL, (char)1611, (char)1624, (char)NSM, (char)1625, (char)1631, (char)L, (char)1632, (char)1641, (char)AN,
            (char)1642, (char)1642, (char)ET, (char)1643, (char)1644, (char)AN, (char)1645, (char)1647, (char)AL, (char)1648, (char)1648, (char)NSM,
            (char)1649, (char)1749, (char)AL, (char)1750, (char)1756, (char)NSM, (char)1757, (char)1757, (char)AL, (char)1758, (char)1764, (char)NSM,
            (char)1765, (char)1766, (char)AL, (char)1767, (char)1768, (char)NSM, (char)1769, (char)1769, (char)ON, (char)1770, (char)1773, (char)NSM,
            (char)1774, (char)1775, (char)AL, (char)1776, (char)1785, (char)EN, (char)1786, (char)1805, (char)AL, (char)1806, (char)1806, (char)L,
            (char)1807, (char)1807, (char)BN, (char)1808, (char)1808, (char)AL, (char)1809, (char)1809, (char)NSM, (char)1810, (char)1839, (char)AL,
            (char)1840, (char)1866, (char)NSM, (char)1867, (char)1868, (char)L, (char)1869, (char)1871, (char)AL, (char)1872, (char)1919, (char)L,
            (char)1920, (char)1957, (char)AL, (char)1958, (char)1968, (char)NSM, (char)1969, (char)1969, (char)AL, (char)1970, (char)2304, (char)L,
            (char)2305, (char)2306, (char)NSM, (char)2307, (char)2363, (char)L, (char)2364, (char)2364, (char)NSM, (char)2365, (char)2368, (char)L,
            (char)2369, (char)2376, (char)NSM, (char)2377, (char)2380, (char)L, (char)2381, (char)2381, (char)NSM, (char)2382, (char)2384, (char)L,
            (char)2385, (char)2388, (char)NSM, (char)2389, (char)2401, (char)L, (char)2402, (char)2403, (char)NSM, (char)2404, (char)2432, (char)L,
            (char)2433, (char)2433, (char)NSM, (char)2434, (char)2491, (char)L, (char)2492, (char)2492, (char)NSM, (char)2493, (char)2496, (char)L,
            (char)2497, (char)2500, (char)NSM, (char)2501, (char)2508, (char)L, (char)2509, (char)2509, (char)NSM, (char)2510, (char)2529, (char)L,
            (char)2530, (char)2531, (char)NSM, (char)2532, (char)2545, (char)L, (char)2546, (char)2547, (char)ET, (char)2548, (char)2560, (char)L,
            (char)2561, (char)2562, (char)NSM, (char)2563, (char)2619, (char)L, (char)2620, (char)2620, (char)NSM, (char)2621, (char)2624, (char)L,
            (char)2625, (char)2626, (char)NSM, (char)2627, (char)2630, (char)L, (char)2631, (char)2632, (char)NSM, (char)2633, (char)2634, (char)L,
            (char)2635, (char)2637, (char)NSM, (char)2638, (char)2671, (char)L, (char)2672, (char)2673, (char)NSM, (char)2674, (char)2688, (char)L,
            (char)2689, (char)2690, (char)NSM, (char)2691, (char)2747, (char)L, (char)2748, (char)2748, (char)NSM, (char)2749, (char)2752, (char)L,
            (char)2753, (char)2757, (char)NSM, (char)2758, (char)2758, (char)L, (char)2759, (char)2760, (char)NSM, (char)2761, (char)2764, (char)L,
            (char)2765, (char)2765, (char)NSM, (char)2766, (char)2785, (char)L, (char)2786, (char)2787, (char)NSM, (char)2788, (char)2800, (char)L,
            (char)2801, (char)2801, (char)ET, (char)2802, (char)2816, (char)L, (char)2817, (char)2817, (char)NSM, (char)2818, (char)2875, (char)L,
            (char)2876, (char)2876, (char)NSM, (char)2877, (char)2878, (char)L, (char)2879, (char)2879, (char)NSM, (char)2880, (char)2880, (char)L,
            (char)2881, (char)2883, (char)NSM, (char)2884, (char)2892, (char)L, (char)2893, (char)2893, (char)NSM, (char)2894, (char)2901, (char)L,
            (char)2902, (char)2902, (char)NSM, (char)2903, (char)2945, (char)L, (char)2946, (char)2946, (char)NSM, (char)2947, (char)3007, (char)L,
            (char)3008, (char)3008, (char)NSM, (char)3009, (char)3020, (char)L, (char)3021, (char)3021, (char)NSM, (char)3022, (char)3058, (char)L,
            (char)3059, (char)3064, (char)ON, (char)3065, (char)3065, (char)ET, (char)3066, (char)3066, (char)ON, (char)3067, (char)3133, (char)L,
            (char)3134, (char)3136, (char)NSM, (char)3137, (char)3141, (char)L, (char)3142, (char)3144, (char)NSM, (char)3145, (char)3145, (char)L,
            (char)3146, (char)3149, (char)NSM, (char)3150, (char)3156, (char)L, (char)3157, (char)3158, (char)NSM, (char)3159, (char)3259, (char)L,
            (char)3260, (char)3260, (char)NSM, (char)3261, (char)3275, (char)L, (char)3276, (char)3277, (char)NSM, (char)3278, (char)3392, (char)L,
            (char)3393, (char)3395, (char)NSM, (char)3396, (char)3404, (char)L, (char)3405, (char)3405, (char)NSM, (char)3406, (char)3529, (char)L,
            (char)3530, (char)3530, (char)NSM, (char)3531, (char)3537, (char)L, (char)3538, (char)3540, (char)NSM, (char)3541, (char)3541, (char)L,
            (char)3542, (char)3542, (char)NSM, (char)3543, (char)3632, (char)L, (char)3633, (char)3633, (char)NSM, (char)3634, (char)3635, (char)L,
            (char)3636, (char)3642, (char)NSM, (char)3643, (char)3646, (char)L, (char)3647, (char)3647, (char)ET, (char)3648, (char)3654, (char)L,
            (char)3655, (char)3662, (char)NSM, (char)3663, (char)3760, (char)L, (char)3761, (char)3761, (char)NSM, (char)3762, (char)3763, (char)L,
            (char)3764, (char)3769, (char)NSM, (char)3770, (char)3770, (char)L, (char)3771, (char)3772, (char)NSM, (char)3773, (char)3783, (char)L,
            (char)3784, (char)3789, (char)NSM, (char)3790, (char)3863, (char)L, (char)3864, (char)3865, (char)NSM, (char)3866, (char)3892, (char)L,
            (char)3893, (char)3893, (char)NSM, (char)3894, (char)3894, (char)L, (char)3895, (char)3895, (char)NSM, (char)3896, (char)3896, (char)L,
            (char)3897, (char)3897, (char)NSM, (char)3898, (char)3901, (char)ON, (char)3902, (char)3952, (char)L, (char)3953, (char)3966, (char)NSM,
            (char)3967, (char)3967, (char)L, (char)3968, (char)3972, (char)NSM, (char)3973, (char)3973, (char)L, (char)3974, (char)3975, (char)NSM,
            (char)3976, (char)3983, (char)L, (char)3984, (char)3991, (char)NSM, (char)3992, (char)3992, (char)L, (char)3993, (char)4028, (char)NSM,
            (char)4029, (char)4037, (char)L, (char)4038, (char)4038, (char)NSM, (char)4039, (char)4140, (char)L, (char)4141, (char)4144, (char)NSM,
            (char)4145, (char)4145, (char)L, (char)4146, (char)4146, (char)NSM, (char)4147, (char)4149, (char)L, (char)4150, (char)4151, (char)NSM,
            (char)4152, (char)4152, (char)L, (char)4153, (char)4153, (char)NSM, (char)4154, (char)4183, (char)L, (char)4184, (char)4185, (char)NSM,
            (char)4186, (char)5759, (char)L, (char)5760, (char)5760, (char)WS, (char)5761, (char)5786, (char)L, (char)5787, (char)5788, (char)ON,
            (char)5789, (char)5905, (char)L, (char)5906, (char)5908, (char)NSM, (char)5909, (char)5937, (char)L, (char)5938, (char)5940, (char)NSM,
            (char)5941, (char)5969, (char)L, (char)5970, (char)5971, (char)NSM, (char)5972, (char)6001, (char)L, (char)6002, (char)6003, (char)NSM,
            (char)6004, (char)6070, (char)L, (char)6071, (char)6077, (char)NSM, (char)6078, (char)6085, (char)L, (char)6086, (char)6086, (char)NSM,
            (char)6087, (char)6088, (char)L, (char)6089, (char)6099, (char)NSM, (char)6100, (char)6106, (char)L, (char)6107, (char)6107, (char)ET,
            (char)6108, (char)6108, (char)L, (char)6109, (char)6109, (char)NSM, (char)6110, (char)6127, (char)L, (char)6128, (char)6137, (char)ON,
            (char)6138, (char)6143, (char)L, (char)6144, (char)6154, (char)ON, (char)6155, (char)6157, (char)NSM, (char)6158, (char)6158, (char)WS,
            (char)6159, (char)6312, (char)L, (char)6313, (char)6313, (char)NSM, (char)6314, (char)6431, (char)L, (char)6432, (char)6434, (char)NSM,
            (char)6435, (char)6438, (char)L, (char)6439, (char)6443, (char)NSM, (char)6444, (char)6449, (char)L, (char)6450, (char)6450, (char)NSM,
            (char)6451, (char)6456, (char)L, (char)6457, (char)6459, (char)NSM, (char)6460, (char)6463, (char)L, (char)6464, (char)6464, (char)ON,
            (char)6465, (char)6467, (char)L, (char)6468, (char)6469, (char)ON, (char)6470, (char)6623, (char)L, (char)6624, (char)6655, (char)ON,
            (char)6656, (char)8124, (char)L, (char)8125, (char)8125, (char)ON, (char)8126, (char)8126, (char)L, (char)8127, (char)8129, (char)ON,
            (char)8130, (char)8140, (char)L, (char)8141, (char)8143, (char)ON, (char)8144, (char)8156, (char)L, (char)8157, (char)8159, (char)ON,
            (char)8160, (char)8172, (char)L, (char)8173, (char)8175, (char)ON, (char)8176, (char)8188, (char)L, (char)8189, (char)8190, (char)ON,
            (char)8191, (char)8191, (char)L, (char)8192, (char)8202, (char)WS, (char)8203, (char)8205, (char)BN, (char)8206, (char)8206, (char)L,
            (char)8207, (char)8207, (char)R, (char)8208, (char)8231, (char)ON, (char)8232, (char)8232, (char)WS, (char)8233, (char)8233, (char)B,
            (char)8234, (char)8234, (char)LRE, (char)8235, (char)8235, (char)RLE, (char)8236, (char)8236, (char)PDF, (char)8237, (char)8237, (char)LRO,
            (char)8238, (char)8238, (char)RLO, (char)8239, (char)8239, (char)WS, (char)8240, (char)8244, (char)ET, (char)8245, (char)8276, (char)ON,
            (char)8277, (char)8278, (char)L, (char)8279, (char)8279, (char)ON, (char)8280, (char)8286, (char)L, (char)8287, (char)8287, (char)WS,
            (char)8288, (char)8291, (char)BN, (char)8292, (char)8297, (char)L, (char)8298, (char)8303, (char)BN, (char)8304, (char)8304, (char)EN,
            (char)8305, (char)8307, (char)L, (char)8308, (char)8313, (char)EN, (char)8314, (char)8315, (char)ET, (char)8316, (char)8318, (char)ON,
            (char)8319, (char)8319, (char)L, (char)8320, (char)8329, (char)EN, (char)8330, (char)8331, (char)ET, (char)8332, (char)8334, (char)ON,
            (char)8335, (char)8351, (char)L, (char)8352, (char)8369, (char)ET, (char)8370, (char)8399, (char)L, (char)8400, (char)8426, (char)NSM,
            (char)8427, (char)8447, (char)L, (char)8448, (char)8449, (char)ON, (char)8450, (char)8450, (char)L, (char)8451, (char)8454, (char)ON,
            (char)8455, (char)8455, (char)L, (char)8456, (char)8457, (char)ON, (char)8458, (char)8467, (char)L, (char)8468, (char)8468, (char)ON,
            (char)8469, (char)8469, (char)L, (char)8470, (char)8472, (char)ON, (char)8473, (char)8477, (char)L, (char)8478, (char)8483, (char)ON,
            (char)8484, (char)8484, (char)L, (char)8485, (char)8485, (char)ON, (char)8486, (char)8486, (char)L, (char)8487, (char)8487, (char)ON,
            (char)8488, (char)8488, (char)L, (char)8489, (char)8489, (char)ON, (char)8490, (char)8493, (char)L, (char)8494, (char)8494, (char)ET,
            (char)8495, (char)8497, (char)L, (char)8498, (char)8498, (char)ON, (char)8499, (char)8505, (char)L, (char)8506, (char)8507, (char)ON,
            (char)8508, (char)8511, (char)L, (char)8512, (char)8516, (char)ON, (char)8517, (char)8521, (char)L, (char)8522, (char)8523, (char)ON,
            (char)8524, (char)8530, (char)L, (char)8531, (char)8543, (char)ON, (char)8544, (char)8591, (char)L, (char)8592, (char)8721, (char)ON,
            (char)8722, (char)8723, (char)ET, (char)8724, (char)9013, (char)ON, (char)9014, (char)9082, (char)L, (char)9083, (char)9108, (char)ON,
            (char)9109, (char)9109, (char)L, (char)9110, (char)9168, (char)ON, (char)9169, (char)9215, (char)L, (char)9216, (char)9254, (char)ON,
            (char)9255, (char)9279, (char)L, (char)9280, (char)9290, (char)ON, (char)9291, (char)9311, (char)L, (char)9312, (char)9371, (char)EN,
            (char)9372, (char)9449, (char)L, (char)9450, (char)9450, (char)EN, (char)9451, (char)9751, (char)ON, (char)9752, (char)9752, (char)L,
            (char)9753, (char)9853, (char)ON, (char)9854, (char)9855, (char)L, (char)9856, (char)9873, (char)ON, (char)9874, (char)9887, (char)L,
            (char)9888, (char)9889, (char)ON, (char)9890, (char)9984, (char)L, (char)9985, (char)9988, (char)ON, (char)9989, (char)9989, (char)L,
            (char)9990, (char)9993, (char)ON, (char)9994, (char)9995, (char)L, (char)9996, (char)10023, (char)ON, (char)10024, (char)10024, (char)L,
            (char)10025, (char)10059, (char)ON, (char)10060, (char)10060, (char)L, (char)10061, (char)10061, (char)ON, (char)10062, (char)10062, (char)L,
            (char)10063, (char)10066, (char)ON, (char)10067, (char)10069, (char)L, (char)10070, (char)10070, (char)ON, (char)10071, (char)10071, (char)L,
            (char)10072, (char)10078, (char)ON, (char)10079, (char)10080, (char)L, (char)10081, (char)10132, (char)ON, (char)10133, (char)10135, (char)L,
            (char)10136, (char)10159, (char)ON, (char)10160, (char)10160, (char)L, (char)10161, (char)10174, (char)ON, (char)10175, (char)10191, (char)L,
            (char)10192, (char)10219, (char)ON, (char)10220, (char)10223, (char)L, (char)10224, (char)11021, (char)ON, (char)11022, (char)11903, (char)L,
            (char)11904, (char)11929, (char)ON, (char)11930, (char)11930, (char)L, (char)11931, (char)12019, (char)ON, (char)12020, (char)12031, (char)L,
            (char)12032, (char)12245, (char)ON, (char)12246, (char)12271, (char)L, (char)12272, (char)12283, (char)ON, (char)12284, (char)12287, (char)L,
            (char)12288, (char)12288, (char)WS, (char)12289, (char)12292, (char)ON, (char)12293, (char)12295, (char)L, (char)12296, (char)12320, (char)ON,
            (char)12321, (char)12329, (char)L, (char)12330, (char)12335, (char)NSM, (char)12336, (char)12336, (char)ON, (char)12337, (char)12341, (char)L,
            (char)12342, (char)12343, (char)ON, (char)12344, (char)12348, (char)L, (char)12349, (char)12351, (char)ON, (char)12352, (char)12440, (char)L,
            (char)12441, (char)12442, (char)NSM, (char)12443, (char)12444, (char)ON, (char)12445, (char)12447, (char)L, (char)12448, (char)12448, (char)ON,
            (char)12449, (char)12538, (char)L, (char)12539, (char)12539, (char)ON, (char)12540, (char)12828, (char)L, (char)12829, (char)12830, (char)ON,
            (char)12831, (char)12879, (char)L, (char)12880, (char)12895, (char)ON, (char)12896, (char)12923, (char)L, (char)12924, (char)12925, (char)ON,
            (char)12926, (char)12976, (char)L, (char)12977, (char)12991, (char)ON, (char)12992, (char)13003, (char)L, (char)13004, (char)13007, (char)ON,
            (char)13008, (char)13174, (char)L, (char)13175, (char)13178, (char)ON, (char)13179, (char)13277, (char)L, (char)13278, (char)13279, (char)ON,
            (char)13280, (char)13310, (char)L, (char)13311, (char)13311, (char)ON, (char)13312, (char)19903, (char)L, (char)19904, (char)19967, (char)ON,
            (char)19968, (char)42127, (char)L, (char)42128, (char)42182, (char)ON, (char)42183, (char)64284, (char)L, (char)64285, (char)64285, (char)R,
            (char)64286, (char)64286, (char)NSM, (char)64287, (char)64296, (char)R, (char)64297, (char)64297, (char)ET, (char)64298, (char)64310, (char)R,
            (char)64311, (char)64311, (char)L, (char)64312, (char)64316, (char)R, (char)64317, (char)64317, (char)L, (char)64318, (char)64318, (char)R,
            (char)64319, (char)64319, (char)L, (char)64320, (char)64321, (char)R, (char)64322, (char)64322, (char)L, (char)64323, (char)64324, (char)R,
            (char)64325, (char)64325, (char)L, (char)64326, (char)64335, (char)R, (char)64336, (char)64433, (char)AL, (char)64434, (char)64466, (char)L,
            (char)64467, (char)64829, (char)AL, (char)64830, (char)64831, (char)ON, (char)64832, (char)64847, (char)L, (char)64848, (char)64911, (char)AL,
            (char)64912, (char)64913, (char)L, (char)64914, (char)64967, (char)AL, (char)64968, (char)65007, (char)L, (char)65008, (char)65020, (char)AL,
            (char)65021, (char)65021, (char)ON, (char)65022, (char)65023, (char)L, (char)65024, (char)65039, (char)NSM, (char)65040, (char)65055, (char)L,
            (char)65056, (char)65059, (char)NSM, (char)65060, (char)65071, (char)L, (char)65072, (char)65103, (char)ON, (char)65104, (char)65104, (char)CS,
            (char)65105, (char)65105, (char)ON, (char)65106, (char)65106, (char)CS, (char)65107, (char)65107, (char)L, (char)65108, (char)65108, (char)ON,
            (char)65109, (char)65109, (char)CS, (char)65110, (char)65118, (char)ON, (char)65119, (char)65119, (char)ET, (char)65120, (char)65121, (char)ON,
            (char)65122, (char)65123, (char)ET, (char)65124, (char)65126, (char)ON, (char)65127, (char)65127, (char)L, (char)65128, (char)65128, (char)ON,
            (char)65129, (char)65130, (char)ET, (char)65131, (char)65131, (char)ON, (char)65132, (char)65135, (char)L, (char)65136, (char)65140, (char)AL,
            (char)65141, (char)65141, (char)L, (char)65142, (char)65276, (char)AL, (char)65277, (char)65278, (char)L, (char)65279, (char)65279, (char)BN,
            (char)65280, (char)65280, (char)L, (char)65281, (char)65282, (char)ON, (char)65283, (char)65285, (char)ET, (char)65286, (char)65290, (char)ON,
            (char)65291, (char)65291, (char)ET, (char)65292, (char)65292, (char)CS, (char)65293, (char)65293, (char)ET, (char)65294, (char)65294, (char)CS,
            (char)65295, (char)65295, (char)ES, (char)65296, (char)65305, (char)EN, (char)65306, (char)65306, (char)CS, (char)65307, (char)65312, (char)ON,
            (char)65313, (char)65338, (char)L, (char)65339, (char)65344, (char)ON, (char)65345, (char)65370, (char)L, (char)65371, (char)65381, (char)ON,
            (char)65382, (char)65503, (char)L, (char)65504, (char)65505, (char)ET, (char)65506, (char)65508, (char)ON, (char)65509, (char)65510, (char)ET,
            (char)65511, (char)65511, (char)L, (char)65512, (char)65518, (char)ON, (char)65519, (char)65528, (char)L, (char)65529, (char)65531, (char)BN,
            (char)65532, (char)65533, (char)ON, (char)65534, (char)65535, (char)L};
            
        static BidiOrder() {
            for (int k = 0; k < baseTypes.Length; ++k) {
                int start = baseTypes[k];
                int end = baseTypes[++k];
                sbyte b = (sbyte)baseTypes[++k];
                while (start <= end)
                    rtypes[start++] = b;
            }
        }        
    }
}
