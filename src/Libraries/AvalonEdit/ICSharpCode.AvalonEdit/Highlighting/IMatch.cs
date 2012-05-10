// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)


namespace ICSharpCode.AvalonEdit.Highlighting {

    public interface IMatch {

        bool Success { get; }
        int Index { get; }
        int Length { get; }
    }
}