using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Models.CodeModels;

namespace UMLGenerator.ViewModels
{
    public class CodeFileViewModel
    {
        #region Fields
        private string code;
        private int currStart = 0;
        private int currIndex = 0;
        private CodeProjectModel project;
        #endregion


        #region Constructors
        public CodeFileViewModel(string code, CodeProjectModel project)
        {
            this.code = code;
            this.project = project;
            this.code = GetOnlyRelevantCode(this.code);
        }
        #endregion

        #region Methods
        public List<CodeObjectModel> GetLanguageObjects()
        {
            List<CodeObjectModel> output = new List<CodeObjectModel>();
            var delimiters = GetDelimitersDict(project.Language.CodeDelimiters);
            while (currIndex < code.Length)
            {
                if (delimiters.ContainsKey(code[currIndex++]))
                {
                    var obj = GetComponent(code[currStart..currIndex], project.Language, delimiters[code[currIndex - 1]]);
                    if (obj != null)
                        output.Add(obj);
                    currStart = currIndex;
                }
            }
            return output;
        }

        private CodeObjectModel GetComponent(string statement, CodeComponentTypeModel parentType, CodeDelimiterModel currDelimiter)
        {
            int openDelimiters = 1;
            var componentType = GetComponentType(statement, parentType);
            if (componentType == null)
            {
                if(currDelimiter.HasClose)
                {
                    while(currIndex < code.Length && openDelimiters > 0)
                    {
                        if (currDelimiter.OpenDelimiter == code[currIndex++ - 1])
                        {
                            openDelimiters++;
                        }
                        else if (currDelimiter.CloseDelimiter == code[currIndex - 1])
                        {
                            openDelimiters--;
                        }
                    }
                }
                currStart = currIndex;
                return null;
            }
            CodeObjectModel output = new CodeObjectModel() { Type = componentType};
            output.Name = Regex.Match(statement, componentType.NamePattern).Groups["Name"].Value;
            bool firstCreation = true;
            if(componentType.IsUniqueCollection)
            {
                if(!project.UniqueCollections.ContainsKey(componentType.Name))
                {
                    project.UniqueCollections.Add(componentType.Name, new Dictionary<string, CodeObjectModel>());
                }
                if(project.UniqueCollections[componentType.Name].ContainsKey(output.Name))
                {
                    output = project.UniqueCollections[componentType.Name][output.Name];
                    firstCreation = false;
                }
                else
                {
                    project.UniqueCollections[componentType.Name].Add(output.Name, output);
                    #region Get Fields From Statement
                    foreach (var fieldType in componentType.Fields)
                    {
                        var match = Regex.Match(statement, fieldType.Pattern);
                        switch (fieldType.InputType)
                        {
                            case FieldInputType.Textual:
                                output.FieldsFound.Add(fieldType.Name, match.Success ? match.Groups["Value"].Value : "");
                                break;
                            case FieldInputType.Boolean:
                                output.FieldsFound.Add(fieldType.Name, match.Success ? fieldType.TrueValue : fieldType.FalseValue);
                                break;
                            case FieldInputType.Switch:
                                break;
                        }
                    }
                    #endregion
                }
            }
            
            currStart = currIndex;
            #region Get Nested Components While Delimiter Not Closed (Run Only If Open)
            if (!currDelimiter.HasClose)
                return firstCreation ? output : null;
            var delimiters = GetDelimitersDict(componentType.CodeDelimiters);
            CodeObjectModel nestedObj;
            while (currIndex < code.Length && openDelimiters > 0)
            {
                if (delimiters.ContainsKey(code[currIndex++]))
                {
                    nestedObj = GetComponent(code[currStart..currIndex], componentType, delimiters[code[currIndex - 1]]);
                    if (nestedObj != null)
                        output.Children.Add(nestedObj);
                }
                else if(currDelimiter.OpenDelimiter == code[currIndex-1])
                {
                    openDelimiters++;
                }
                else if(currDelimiter.CloseDelimiter == code[currIndex - 1])
                {
                    openDelimiters--;
                }
            }
            #endregion
            nestedObj = GetComponent(code[currStart..currIndex], componentType, new CodeDelimiterModel() { });
            if (nestedObj != null)
                output.Children.Add(nestedObj);
            currStart = currIndex;
            return firstCreation ? output : null;

        }

        private CodeComponentTypeModel GetComponentType(string statement, CodeComponentTypeModel parentType)
        {
            foreach (var childName in parentType.SubComponents)
            {
                var childModel = project.Language.Components[childName];
                bool found = false;
                foreach (var pattern in childModel.TruePatterns)
                {
                    if (Regex.IsMatch(statement, pattern.Pattern))
                    {
                        found = true;
                        break;
                    }

                }
                if (!found)
                    continue;
                found = false;
                foreach (var pattern in childModel.FalsePatterns)
                {
                    if (Regex.IsMatch(statement, pattern.Pattern))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return childModel;
                }
            }
            return null;
        }

        private Dictionary<char, CodeDelimiterModel> GetDelimitersDict(IEnumerable<CodeDelimiterModel> delimitersOrigin)
        {
            Dictionary<char, CodeDelimiterModel> delimiters = new Dictionary<char, CodeDelimiterModel>();
            foreach (var delimiter in delimitersOrigin)
                delimiters.Add(delimiter.OpenDelimiter, delimiter);
            return delimiters;
        }


        private string GetOnlyRelevantCode(string str)
        {
            string res = str;
            foreach (var model in project.Language.CleanupModels)
            {
                string strToReturn = model.ReplaceWithNewLine ? Environment.NewLine : "";
                res = Regex.Replace(res, model.Pattern, strToReturn, model.RegexSignleLine ? RegexOptions.Singleline : RegexOptions.Multiline);
            }
            return res;
        }
       #endregion
    }
}