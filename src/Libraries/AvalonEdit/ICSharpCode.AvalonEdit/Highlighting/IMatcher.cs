// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)


namespace ICSharpCode.AvalonEdit.Highlighting {

    public interface IMatcher {

        IMatch Match(string input, int startat);
        IMatch Match(string input, int beginning, int length);
    }
}