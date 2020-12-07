using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UMLGenerator.Libraries
{
    public static class CodeCleanupMethods
    {
        public static string RemoveComments(string str)
        {
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            string noComments = Regex.Replace(str, blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                    me => {
                        if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                            return me.Value.StartsWith("//") ? Environment.NewLine : "";
                        // Keep the literal strings
                        return me.Value;
                    },
                RegexOptions.Singleline);
            return noComments;
        }
        public static string RemoveRegions(string str)
        {
            return Regex.Replace(str, @"^[ \t]*\#[ \t]*(region|endregion).*\n", "", RegexOptions.Multiline);
        }

        public static string RemoveNewLinesNTab(string str)
        {
            return Regex.Replace(str, @"\t|\n|\r", "");
        }

        public static string RemoveAllStrings(string str)
        {
            return Regex.Replace(str, "((\".*\")|(\'.*\'))", "");
        }
    }
}
