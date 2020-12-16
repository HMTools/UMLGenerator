﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Models.CodeModels;

namespace UMLGenerator.ViewModels
{
    public class CodeFileViewModel
    {
        #region Properties
        public CodeFileModel Model { get; set; }
        #endregion
        #region Fields
        private int currStart = 0;
        private int currIndex = 0;
        private Dictionary<string, NamespaceModel> namespacesDict;
        private Dictionary<string, List<string>> classesDict;
        private Dictionary<string, List<string>> interfacesDict;
        private Queue<NamespaceModel> namespacesQueue;
        #endregion

        #region Constructors
        public CodeFileViewModel(string fileName, string code, Dictionary<string, NamespaceModel> namespacesDict,
            Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            this.namespacesDict = namespacesDict;
            this.classesDict = classesDict;
            this.interfacesDict = interfacesDict;
            namespacesQueue = new Queue<NamespaceModel>();

            Model = new CodeFileModel() { Name = fileName , Code = code};
            Model.Code = GetOnlyRelevantCode(Model.Code);
            ScanSubObjects(false, "");
        }
        #endregion

        #region Methods

        private List<object> ScanSubObjects(bool isInObject, string path)
        {
            List<object> output = new List<object>();
            while (currIndex < Model.Code.Length && Model.Code[currIndex] != '}')
            {
                if (";{".Contains(Model.Code[currIndex++]))
                {
                    output.Add(GetObject(Model.Code.Substring(currStart, currIndex - currStart), isInObject, path));
                }
            }
            currIndex++;
            return output;
        }

        private object GetObject(string header ,bool isInObject, string path)
        {
            currStart = currIndex;
            #region NameSpace Model
            if (Regex.IsMatch(header, NamespaceModel.BasePattern))
            {
                return GetNamespaceModel(header);
            }
            #endregion

            #region Class Model
            if (Regex.IsMatch(header, ClassModel.BasePattern))
            {
                return GetClassModel(header, path);
            }
            #endregion

            #region Interface Model
            if(Regex.IsMatch(header, InterfaceModel.BasePattern))
            {
                return GetInterfaceModel(header, path);
            }
            #endregion

            #region Record Model
            if (Regex.IsMatch(header, RecordModel.BasePattern))
            {
                return GetRecordModel(header, path);
            }
            #endregion

            #region Enum Model
            if (Regex.IsMatch(header, EnumModel.BasePattern))
            {
                return GetEnumModel(header, path);
            }
            #endregion

            if (isInObject)
            {
                #region Method Model
                if (Regex.IsMatch(header, MethodModel.BasePattern))
                {
                    return GetMethodModel(header);
                }
                #endregion

                #region Property Model
                if (Regex.IsMatch(header, PropertyModel.BasePattern))
                {
                    return GetPropertyModel(header);
                }
                #endregion

                #region Field Model
                if (Regex.IsMatch(header, FieldModel.BasePattern))
                {
                    return GetFieldModel(header);
                }
                #endregion

            }
            return null;
        }

        private NamespaceModel GetNamespaceModel(string statement)
        {
            NamespaceModel model = new NamespaceModel(statement);
            if (namespacesDict.ContainsKey(model.Name))
            {
                model = namespacesDict[model.Name];
            }
            else
            {
                namespacesDict.Add(model.Name, model);
            }
            namespacesQueue.Enqueue(model);
            ScanSubObjects(false, "");
            namespacesQueue.Dequeue();
            return model;
        }

        private ClassModel GetClassModel(string statement, string path)
        {
            ClassModel model = new ClassModel(statement, path, namespacesQueue.Peek().Name);
            if (classesDict.ContainsKey(model.Name))
            {
                classesDict[model.Name].Add($"{namespacesQueue.Peek().Name}.{path}");
            }
            else
            {
                classesDict.Add(model.Name, new List<string>() { $"{namespacesQueue.Peek().Name}.{path}" });
            }
            namespacesQueue.Peek().Classes.Add(model);
            model.AssociateChilds(ScanSubObjects(true, $"{path}{model.Name}__"));
            return model;
        }

        private InterfaceModel GetInterfaceModel(string statement, string path)
        {

            InterfaceModel model = new InterfaceModel(statement, path, namespacesQueue.Peek().Name);
            if (classesDict.ContainsKey(model.Name))
            {
                interfacesDict[model.Name].Add($"{namespacesQueue.Peek().Name}.{path}");
            }
            else
            {
                interfacesDict.Add(model.Name, new List<string>() { $"{namespacesQueue.Peek().Name}.{path}" });
            }
            namespacesQueue.Peek().Interfaces.Add(model);
            model.AssociateChilds(ScanSubObjects(true, $"{path}{model.Name}__"));
            return model;

        }

        private RecordModel GetRecordModel(string statement, string path)
        {

            RecordModel model = new RecordModel(statement, path, namespacesQueue.Peek().Name);
            namespacesQueue.Peek().Records.Add(model);
            #region Skip Content

            int countOpenBrackets = 1;
            while (countOpenBrackets > 0 && currIndex < Model.Code.Length)
            {
                if (Model.Code[currIndex] == '{')
                    countOpenBrackets++;
                else if (Model.Code[currIndex] == '}')
                    countOpenBrackets--;
                currIndex++;
            }
            currStart = currIndex;
            #endregion
            return model;

        }

        private EnumModel GetEnumModel(string statement, string path)
        {
            EnumModel model = new EnumModel(statement, path, namespacesQueue.Peek().Name);
            namespacesQueue.Peek().Enums.Add(model);
            #region Skip Content
            currStart = currIndex;
            while (Model.Code[currIndex] != '}' && currIndex < Model.Code.Length)
            {
                if (Model.Code[currIndex] == ',')
                {
                    model.Members.Add(Regex.Match(Model.Code.Substring(currStart, currIndex - currStart), @"^ *(?<Member>\w+)").Value);
                    currStart = currIndex + 1;
                }
                currIndex++;
            }
            model.Members.Add(Regex.Match(Model.Code.Substring(currStart, currIndex - currStart), @"^ *(?<Member>\w+)").Value);
            currStart = currIndex;
            #endregion
            return model;
        }

        private MethodModel GetMethodModel(string statement)
        {

            MethodModel model = new MethodModel(statement);

            #region Skip Content

            int countOpenBrackets = 1;
            while (countOpenBrackets > 0 && currIndex < Model.Code.Length)
            {
                if (Model.Code[currIndex] == '{')
                    countOpenBrackets++;
                else if (Model.Code[currIndex] == '}')
                    countOpenBrackets--;
                currIndex++;
            }
            currStart = currIndex;
            #endregion
            return model;

        }

        private PropertyModel GetPropertyModel(string statement)
        {

            PropertyModel model = new PropertyModel(statement);

            #region Skip Content

            int countOpenBrackets = 1;
            while (countOpenBrackets > 0 && currIndex < Model.Code.Length)
            {
                if (Model.Code[currIndex] == '{')
                    countOpenBrackets++;
                else if (Model.Code[currIndex] == '}')
                    countOpenBrackets--;
        
                currIndex++;
            }
            #region Default Value Checking
            int checkIndex = currIndex;
            while (checkIndex < Model.Code.Length && Model.Code[checkIndex] == ' ')
                checkIndex++;
            if (Model.Code[checkIndex] == '=')
            {
                while (checkIndex < Model.Code.Length && Model.Code[checkIndex] != ';')
                {
                    checkIndex++;
                }
                currIndex = checkIndex + 1;
            }
            #endregion
            currStart = currIndex;
            #endregion
            return model;

        }

        private FieldModel GetFieldModel(string statement)
        {

            FieldModel model = new FieldModel(statement);
            currStart = currIndex;
            return model;

        }


        private string GetOnlyRelevantCode(string str)
        {
            string res = Libraries.CodeCleanupMethods.RemoveComments(str);
            res = Libraries.CodeCleanupMethods.RemoveAllStrings(res);
            res = Libraries.CodeCleanupMethods.RemoveRegions(res);
            res = Libraries.CodeCleanupMethods.RemoveNewLinesNTab(res);
            return res;
        }
        #endregion
    }
}