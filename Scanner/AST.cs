using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scanner
{
    internal class AstNode
    {
        public string name { get; set; }
        public string type { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }
    internal class VariableDeclNode : AstNode
    {
        public VariableValueNode realPart { get; set; }
        public VariableValueNode imagPart { get; set; }
        public List<string> Print()
        {
            List<string> tree = new List<string>();
            tree.Add("ComplexDeclarationNode");
            tree.Add("├── name: " + name);
            tree.Add("├── type: " + type);
            tree.Add("└── initializer: ");
            tree.Add("        " + realPart.Print());
            tree.Add("        " + imagPart.Print());
            return tree;
        }
    }
    internal class VariableValueNode : AstNode
    {
        public double Value { get; set; }
        public string Print()
        {
            return $"└──{name}: {Value}";
        }
    } 
    class SymbolTable
    {
        private List<string> names = new List<string>();
        private List<string> types = new List<string>();
        private List<double> realParts = new List<double>();
        private List<double> imagPart = new List<double>();
        public bool declare(string name, string type, double real, double imag)
        {
            if (lookup(name)) return false;
            names.Add(name);
            types.Add(type);
            realParts.Add(real); 
            imagPart.Add(imag);
            return true;
        }
        public bool lookup(string name)
        {
            return names.Contains(name);
        }
        public string getName(int index)
        {
            return names[index];
        }
        public string getType(int index)
        {
            return types[index];
        }
        public double getRealPart(int index)
        {
            return realParts[index];
        }
        public double getImagePart(int index)
        {
            return imagPart[index];
        }
        public int Length()
        {
            return names.Count;
        }
    }
}
