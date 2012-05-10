// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Text.RegularExpressions;

namespace ICSharpCode.AvalonEdit.Highlighting {

    public class RegexMatch : IMatch {

        private readonly Match match;

        public RegexMatch(Match match) {
            this.match = match;
        }

        public bool Success {
            get { return match.Success; }
        }

        public int Index {
            get { return match.Index; }
        }

        public int Length {
            get { return match.Length; }
        }
    }
}