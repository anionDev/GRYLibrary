using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.Miscellaneous.TextTransformUtilities
{
    public static class FunctionsForCommonProjectStructure
    {
        public static string GenerateConstants()
        {
            string solutionDirectory = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory()).FullName;
            string constantsFolder = System.IO.Path.Combine(solutionDirectory, "Other", "Resources", "Constants");
            List<string> contentLines = new List<string>();
            if (System.IO.Directory.Exists(constantsFolder))
            {
                foreach (string file in System.IO.Directory.GetFiles(constantsFolder))
                {
                    if (file.EndsWith(".constant.xml"))
                    {
                        IDictionary<string, string> constantProperties = GetConstantProperties(file);
                        List<string> constantDocumentationSummaryLines = new List<string>();
                        if (constantProperties["documentationsummary"].Contains('\n'))
                        {
                            constantDocumentationSummaryLines.AddRange(constantProperties["documentationsummary"].Split('\n'));
                        }
                        else
                        {
                            constantDocumentationSummaryLines.Add(constantProperties["documentationsummary"]);
                        }
                        contentLines.Add(string.Empty);
                        foreach (string line in constantDocumentationSummaryLines)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                contentLines.Add($"        /// {line}");
                            }
                        }
                        string constantValue = constantProperties["value"];
                        string constantValueEscaped = constantValue.Replace("\\", "\\\\");
                        string constantName = constantProperties["name"];
                        contentLines.Add($"        internal const string {constantName} = \"{constantValueEscaped}\";");
                    }
                }
            }
            string constants = string.Join(Environment.NewLine, contentLines);
            return constants;
        }
        private static IDictionary<string, string> GetConstantProperties(string file)
        {
            //TODO validate against xsd
            IDictionary<string, string> result = new Dictionary<string, string>();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            var folder = System.IO.Path.GetDirectoryName(file);
            doc.Load(file);
            var nsmgr = new System.Xml.XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("cps", "https://projects.aniondev.de/PublicProjects/Common/ProjectTemplates/-/tree/main/Conventions/RepositoryStructure/CommonProjectStructure");
            result["name"] = doc.DocumentElement.SelectSingleNode("/cps:constant/cps:name", nsmgr).InnerText;
            result["documentationsummary"] = doc.DocumentElement.SelectSingleNode("/cps:constant/cps:documentationsummary", nsmgr).InnerText.Replace("\r", string.Empty);
            string path = doc.DocumentElement.SelectSingleNode("/cps:constant/cps:path", nsmgr).InnerText;
            string absolutePath = System.IO.Path.GetFullPath(new Uri(System.IO.Path.Combine(folder, path)).LocalPath);
            result["value"] = System.IO.File.ReadAllText(absolutePath, new UTF8Encoding(false));
            return result;
        }
    }
}
