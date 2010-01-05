using System;
using System.Collections;
using System.Text;
using System.util;
using iTextSharp.text.pdf;

/*
 * $Id: RectangleReadOnly.cs,v 1.2 2008/05/13 11:25:12 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text {
    /// <summary>
    /// A RectangleReadOnly is the representation of a geometric figure.
    /// It's the same as a Rectangle but immutable.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    /// <seealso cref="T:iTextSharp.text.Cell"/>
    /// <seealso cref="T:iTextSharp.text.HeaderFooter"/>
    public class RectangleReadOnly : Rectangle {
    
        // constructors
    
        /// <summary>
        /// Constructs a RectangleReadOnly-object.
        /// </summary>
        /// <param name="llx">lower left x</param>
        /// <param name="lly">lower left y</param>
        /// <param name="urx">upper right x</param>
        /// <param name="ury">upper right y</param>
        public RectangleReadOnly(float llx, float lly, float urx, float ury) : base(llx, lly, urx, ury) {
        }
    
        /// <summary>
        /// Constructs a RectangleReadOnly-object starting from the origin (0, 0).
        /// </summary>
        /// <param name="urx">upper right x</param>
        /// <param name="ury">upper right y</param>
        public RectangleReadOnly(float urx, float ury) : base(0, 0, urx, ury) {}
    
        /// <summary>
        /// Constructs a RectangleReadOnly-object.
        /// </summary>
        /// <param name="rect">another Rectangle</param>
        public RectangleReadOnly(Rectangle rect) : base(rect.Left, rect.Bottom, rect.Right, rect.Top) {
            base.CloneNonPositionParameters(rect);
        }

        /**
        * Copies all of the parameters from a <CODE>Rectangle</CODE> object
        * except the position.
        * 
        * @param rect
        *            <CODE>Rectangle</CODE> to copy from
        */
        public override void CloneNonPositionParameters(Rectangle rect) {
            ThrowReadOnlyError();
        }

        private void ThrowReadOnlyError() {
            throw new InvalidOperationException("RectangleReadOnly: this Rectangle is read only.");
        }

        /**
        * Copies all of the parameters from a <CODE>Rectangle</CODE> object
        * except the position.
        * 
        * @param rect
        *            <CODE>Rectangle</CODE> to copy from
        */

        public override void SoftCloneNonPositionParameters(Rectangle rect) {
            ThrowReadOnlyError();
        }

        // methods
    
        /**
        * Switches lowerleft with upperright
        */
        public override void Normalize() {
            ThrowReadOnlyError();
        }

        // methods to set the membervariables
    
        /// <summary>
        /// Get/set the upper right y-coordinate. 
        /// </summary>
        /// <value>a float</value>
        public override float Top {
            set {
                ThrowReadOnlyError();
            }
        }

        /**
        * Enables the border on the specified side.
        * 
        * @param side
        *            the side to enable. One of <CODE>LEFT, RIGHT, TOP, BOTTOM
        *            </CODE>
        */
        public override void EnableBorderSide(int side) {
            ThrowReadOnlyError();
        }

        /**
        * Disables the border on the specified side.
        * 
        * @param side
        *            the side to disable. One of <CODE>LEFT, RIGHT, TOP, BOTTOM
        *            </CODE>
        */
        public override void DisableBorderSide(int side) {
            ThrowReadOnlyError();
        }


        /// <summary>
        /// Get/set the border
        /// </summary>
        /// <value>a int</value>
        public override int Border {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the grayscale of the rectangle.
        /// </summary>
        /// <value>a float</value>
        public override float GrayFill {
            set {
                ThrowReadOnlyError();
            }
        }
    
        // methods to get the membervariables
    
        /// <summary>
        /// Get/set the lower left x-coordinate.
        /// </summary>
        /// <value>a float</value>
        public override float Left {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the upper right x-coordinate.
        /// </summary>
        /// <value>a float</value>
        public override float Right {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the lower left y-coordinate.
        /// </summary>
        /// <value>a float</value>
        public override float Bottom {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override Color BorderColorBottom {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override Color BorderColorTop {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override Color BorderColorLeft {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override Color BorderColorRight {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the borderwidth.
        /// </summary>
        /// <value>a float</value>
        public override float BorderWidth {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /**
         * Gets the color of the border.
         *
         * @return    a value
         */
        /// <summary>
        /// Get/set the color of the border.
        /// </summary>
        /// <value>a Color</value>
        public override Color BorderColor {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /**
         * Gets the backgroundcolor.
         *
         * @return    a value
         */
        /// <summary>
        /// Get/set the backgroundcolor.
        /// </summary>
        /// <value>a Color</value>
        public override Color BackgroundColor {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthLeft {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthRight {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthTop {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthBottom {
            set {
                ThrowReadOnlyError();
            }
        }

        /**
        * Sets a parameter indicating if the rectangle has variable borders
        * 
        * @param useVariableBorders
        *            indication if the rectangle has variable borders
        */
        public override bool UseVariableBorders{
            set {
                ThrowReadOnlyError();
            }
        }

	    public override String ToString() {
		    StringBuilder buf = new StringBuilder("RectangleReadOnly: ");
		    buf.Append(Width);
		    buf.Append('x');
		    buf.Append(Height);
		    buf.Append(" (rot: ");
		    buf.Append(rotation);
		    buf.Append(" degrees)");
		    return buf.ToString();
	    }
    }
}
