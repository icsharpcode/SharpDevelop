#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@pdfsharp.com)
//
// Copyright (c) 2005-2007 empira Software GmbH, Cologne (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

namespace ICSharpCode.Reports.Core
{
  /// <summary>
  /// Specifies the unit of measure.
  /// </summary>
  public enum XGraphicsUnit  // NOT the same values as System.Drawing.GraphicsUnit
  {
    /// <summary>
    /// Specifies a printer's point (1/72 inch) as the unit of measure.
    /// </summary>
    Point = 0,  // must be 0 to let a new XUnit be 0 point

    /// <summary>
    /// Specifies the inch (2.54 cm) as the unit of measure.
    /// </summary>
    Inch = 1,

    /// <summary>
    /// Specifies the millimeter as the unit of measure.
    /// </summary>
    Millimeter = 2,

    /// <summary>
    /// Specifies the centimeter as the unit of measure.
    /// </summary>
    Centimeter = 3,

    /// <summary>
    /// Specifies a presentation point (1/96 inch) as the unit of measure.
    /// </summary>
    Presentation = 4,
    
    Pixel = 5
  }
}
