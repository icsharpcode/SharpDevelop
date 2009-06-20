using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.ShortcutsManagement.Data
{
    /// <summary>
    /// Gesture filtering mode
    /// </summary>
    public enum GestureFilterMode
    {
        /// <summary>
        /// Match is successful if template gesture strictly matches compared gesture
        /// </summary>
        StrictlyMatches,

        /// <summary>
        /// Match is successfull if template gesture partly matches compared geture.
        /// Template is found in any place within matched gesture 
        /// </summary>
        PartlyMatches,
        
        /// <summary>
        /// match is successfull if matched gesture starts with provided template
        /// </summary>
        StartsWith
    }
}
