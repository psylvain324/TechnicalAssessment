using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Data
{
    public class XmlUpload
    {
        public void ParseXML(string filePath)
        {
            // create document instance using XML file path
            XDocument doc = XDocument.Load(filePath);

            // get the namespace to that within of the XML (xmlns="...")
            XElement root = doc.Root;
            XNamespace ns = root.GetDefaultNamespace();

            // obtain a list of elements with specific tag
            IEnumerable<XElement> elements = from c in doc.Descendants(ns + "exampleTagName") select c;

            // obtain a single element with specific tag (first instance), useful if only expecting one instance of the tag in the target doc
            XElement element = (from c in doc.Descendants(ns + "exampleTagName" select c).First();

            // obtain an element from within an element, same as from doc
            XElement embeddedElement = (from c in element.Descendants(ns + "exampleEmbeddedTagName" select c).First();

            // obtain an attribute from an element
            XAttribute attribute = element.Attribute("exampleAttributeName");
        }
    }
}
