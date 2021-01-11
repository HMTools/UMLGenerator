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
        private Dictionary<string, NamespaceModel> namespacesDict;
        private Dictionary<string, List<string>> classesDict;
        private Dictionary<string, List<string>> interfacesDict;
        private Queue<NamespaceModel> namespacesQueue;
        #endregion

        #region Constructors
        public CodeFileViewModel(string code, Dictionary<string, NamespaceModel> namespacesDict,
            Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            this.namespacesDict = namespacesDict;
            this.classesDict = classesDict;
            this.interfacesDict = interfacesDict;
            namespacesQueue = new Queue<NamespaceModel>();

            this.code = code;
            this.code = GetOnlyRelevantCode(this.code);
            ScanSubObjects(false, "");
        }
        #endregion

        #region Methods

        private List<BaseCodeModel> ScanSubObjects(bool isInObject, string path)
        {
            List<BaseCodeModel> output = new List<BaseCodeModel>();
            while (currIndex < code.Length && code[currIndex] != '}')
            {
                if (";{".Contains(code[currIndex++]))
                {
                    var obj = GetObject(code[currStart..currIndex], isInObject, path);
                    if(obj != null)
                        output.Add(obj);
                }
            }
            currIndex++;
            return output;
        }
        private BaseCodeModel GetObject(string header ,bool isInObject, string path)
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
                if (Regex.IsMatch(header, MethodModel.BasePattern) && !Regex.IsMatch(header, MethodModel.BaseFalsePattern))
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
            namespacesQueue.Peek().Children.Add(model);
            foreach(var obj in ScanSubObjects(true, $"{path}{model.Name}__"))
            {
                model.Children.Add(obj);
            }
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
            namespacesQueue.Peek().Children.Add(model);
            foreach (var obj in ScanSubObjects(true, $"{path}{model.Name}__"))
            {
                model.Children.Add(obj);
            }
            return model;

        }

        private RecordModel GetRecordModel(string statement, string path)
        {

            RecordModel model = new RecordModel(statement, path, namespacesQueue.Peek().Name);
            namespacesQueue.Peek().Children.Add(model);
            #region Skip Content

            int countOpenBrackets = 1;
            while (countOpenBrackets > 0 && currIndex < code.Length)
            {
                if (code[currIndex] == '{')
                    countOpenBrackets++;
                else if (code[currIndex] == '}')
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
            namespacesQueue.Peek().Children.Add(model);
            #region Skip Content
            currStart = currIndex;
            while (code[currIndex] != '}' && currIndex < code.Length)
            {
                if (code[currIndex] == ',')
                {
                    model.Children.Add(new EnumMemberModel(code[currStart..currIndex]));
                    currStart = currIndex + 1;
                }
                currIndex++;
            }
            model.Children.Add(new EnumMemberModel(code[currStart..currIndex]));
            currStart = currIndex;
            #endregion
            return model;
        }

        private MethodModel GetMethodModel(string statement)
        {

            MethodModel model = new MethodModel(statement);

            #region Skip Content

            int countOpenBrackets = 1;
            while (countOpenBrackets > 0 && currIndex < code.Length)
            {
                if (code[currIndex] == '{')
                    countOpenBrackets++;
                else if (code[currIndex] == '}')
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
            while (countOpenBrackets > 0 && currIndex < code.Length)
            {
                if (code[currIndex] == '{')
                    countOpenBrackets++;
                else if (code[currIndex] == '}')
                    countOpenBrackets--;
        
                currIndex++;
            }
            #region Default Value Checking
            int checkIndex = currIndex;
            while (checkIndex < code.Length && code[checkIndex] == ' ')
                checkIndex++;
            if (code[checkIndex] == '=')
            {
                while (checkIndex < code.Length && code[checkIndex] != ';')
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


        private static string GetOnlyRelevantCode(string str)
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