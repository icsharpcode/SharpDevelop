using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using System.IO;
using System.Runtime.InteropServices;

namespace ICSharpCode.CodeConversion
{
    public class ConvertCSharpSnippetToVbNet : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            CodeSnippetConverter converter = new CodeSnippetConverter() { ReferencedContents = ReferencedContentsSingleton.ReferencedContents };
            ConvertedSource = converter.CSharpToVB(ProvidedSource, out ErrorMessage);
            return (ErrorMessage.Length == 0);
        }
    }

    public class ConvertVbNetSnippetToCSharp : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            CodeSnippetConverter converter = new CodeSnippetConverter() { ReferencedContents = ReferencedContentsSingleton.ReferencedContents };
            ConvertedSource = converter.VBToCSharp(ProvidedSource, out ErrorMessage);
            return (ErrorMessage.Length == 0);
        }
    }

    public class ConvertCSharpToBoo : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            bool bSuccessfulConversion = BooHelpers.ConvertToBoo("convert.cs",
                ProvidedSource,
                out ConvertedSource,
                out ErrorMessage);

            return bSuccessfulConversion;
        }
    }

    public class ConvertVbNetToBoo : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            bool bSuccessfulConversion = BooHelpers.ConvertToBoo("convert.vb",
                ProvidedSource,
                out ConvertedSource,
                out ErrorMessage);

            return bSuccessfulConversion;
        }
    }
}