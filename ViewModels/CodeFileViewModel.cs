﻿using System;
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
            return GetComponentChildren(project.Language, new CodeDelimiterModel());
        }

        private CodeObjectModel GetComponent(string statement, CodeComponentTypeModel parentType, CodeDelimiterModel currDelimiter)
        {
            var componentType = GetComponentType(statement, parentType);
            if(componentType == null)
            {
                SkipComponentContent(currDelimiter);
                return null;
            }
            CodeObjectModel output = new CodeObjectModel() { Type = componentType };
            bool firstCreation = SetComponentFields(statement, ref output, componentType);
            if(!currDelimiter.HasClose)
                return firstCreation ? output : null;
            currStart = currIndex;
            GetComponentChildren(componentType, currDelimiter).ForEach(child => output.Children.Add(child));            
            return firstCreation ? output : null;

        }
        private void SkipComponentContent(CodeDelimiterModel currDelimiter)
        {
            if (currDelimiter.HasClose)
            {
                int openDelimiters = 1;
                while (currIndex < code.Length && openDelimiters > 0)
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
        }
        private List<CodeObjectModel> GetComponentChildren(CodeComponentTypeModel componentType, CodeDelimiterModel currDelimiter)
        {
            List<CodeObjectModel> output = new List<CodeObjectModel>();
            CodeObjectModel nestedObj;
            int openDelimiters = 1;            
            var delimiters = GetDelimitersDict(componentType);
            while (currIndex < code.Length && (openDelimiters > 0 || componentType is CodeLanguageModel))
            {
                if (delimiters.ContainsKey(code[currIndex++]))
                {
                    nestedObj = GetComponent(code[currStart..currIndex], componentType, delimiters[code[currIndex - 1]]);
                    if (nestedObj != null)
                        output.Add(nestedObj);
                }
                else if (currDelimiter.OpenDelimiter == code[currIndex - 1])
                {
                    openDelimiters++;
                }
                else if (currDelimiter.CloseDelimiter == code[currIndex - 1])
                {
                    openDelimiters--;
                }
            }
            nestedObj = GetComponent(code[currStart..currIndex], componentType, new CodeDelimiterModel());
            if (nestedObj != null)
                output.Add(nestedObj);
            currStart = currIndex;
            return output;
        }

        private bool SetComponentFields(string statement, ref CodeObjectModel obj, CodeComponentTypeModel componentType)
        {
            obj.Name = Regex.Match(statement, componentType.NamePattern).Groups["Value"].Value;
            obj.FieldsFound.Add("Name", obj.Name);
            if (componentType.IsUniqueCollection)
            {
                if (!project.UniqueCollections.ContainsKey(componentType.Name))
                {
                    project.UniqueCollections.Add(componentType.Name, new Dictionary<string, CodeObjectModel>());
                }
                if (project.UniqueCollections[componentType.Name].ContainsKey(obj.Name))
                {
                    obj = project.UniqueCollections[componentType.Name][obj.Name];
                    return false;
                }
                else
                {
                    project.UniqueCollections[componentType.Name].Add(obj.Name, obj);
                }
            }
            #region Get Fields From Statement
            foreach (var fieldType in componentType.Fields)
            {
                var match = Regex.Match(statement, fieldType.Pattern);
                switch (fieldType.InputType)
                {
                    case FieldInputType.Textual:
                        if (match.Success)
                            obj.FieldsFound.Add(fieldType.Name, match.Groups["Value"].Value);
                        break;
                    case FieldInputType.Boolean:
                        obj.FieldsFound.Add(fieldType.Name, match.Success ? fieldType.TrueValue : fieldType.FalseValue);
                        break;
                    case FieldInputType.Switch:
                        foreach (var tryCase in fieldType.Cases)
                        {
                            if (Regex.IsMatch(match.Groups["Value"].Value, tryCase.Case))
                            {
                                obj.FieldsFound.Add(fieldType.Name, tryCase.Value);
                                break;
                            }
                        }
                        break;
                }
            }
            #endregion
            return true;
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

        private Dictionary<char, CodeDelimiterModel> GetDelimitersDict(CodeComponentTypeModel componentType)
        {
            Dictionary<char, CodeDelimiterModel> delimiters = new Dictionary<char, CodeDelimiterModel>();
            foreach (var delimiter in componentType.CodeDelimiters)
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