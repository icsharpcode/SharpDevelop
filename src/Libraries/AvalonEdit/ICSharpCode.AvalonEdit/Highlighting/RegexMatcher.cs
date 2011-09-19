// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Text.RegularExpressions;

namespace ICSharpCode.AvalonEdit.Highlighting {

    public class RegexMatcher : IMatcher {

        private readonly Regex regex;

        public RegexMatcher(Regex regex) {
            this.regex = regex;
        }

        private IMatch Convert(Match match) {
            return match == null ? null : new RegexMatch(match);
        }

        public IMatch Match(string input, int startat) {
            return Convert(regex.Match(input, startat));
        }

        public IMatch Match(string input, int beginning, int length) {
            return Convert(regex.Match(input, beginning, length));
        }

        public override string ToString() {
            return regex.ToString();
        }
    }
}
