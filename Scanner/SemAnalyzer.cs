using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class SemError
    {
        public string description { get; set; }
        public string location { get; set; }

        public SemError(string description, string location)
        {
            this.description = description;
            this.location = location;
        }
    }
    internal class SemAnalyzer
    {
        public List<SemError> errors = new List<SemError>();
        private Token Name;
        private SymbolTable symbolTable = new SymbolTable();
        private VariableDeclNode variableDeclNode = new VariableDeclNode();
        public List<List<string>> Analyze(List<Token> tokens)
        {
            List<List<string>> tree = new List<List<string>>();
            
            string name = "";
            string type = "";
            string valueReal = "";
            double real = 0.0;
            string valueImage = "";
            double image = 0.0;
            int i = 0;
            int k_id = 0;
            int k_type = 0;
            int k_valueReal = 0;
            int k_valueImage = 0;

            foreach (Token token in tokens)
            {
                if (token.id == 3 && k_id == i)
                {
                    name = token.name;
                    Name = token;
                    k_id++;
                    continue;
                }
                if (token.id == 1 && k_type == i)
                {
                    type = token.type;
                    k_type++;
                    continue;
                }
                if ((token.id == 8 || token.id == 9) && k_valueReal == i)
                {
                    valueReal = token.name;
                    continue;
                }
                if ((token.id == 10 || token.id == 11) && k_valueReal == i)
                {
                    valueReal += token.name.Replace('.', ',');
                    if (!double.TryParse(valueReal, out real)) 
                    {
                        AddError(token, $"Ошибка: значение \"{valueReal}\" выходит за допустимые пределы");
                        valueReal = "";
                    }
                    k_valueReal++;
                    continue;
                }
                if ((token.id == 8 || token.id == 9) && k_valueImage == i )
                {
                    valueImage = token.name;
                    continue;
                }
                if ((token.id == 10 || token.id == 11) && k_valueImage == i)
                {
                    valueImage += token.name.Replace('.', ',');
                    if (!double.TryParse(valueImage, out image))
                    {
                        AddError(token, $"Ошибка: значение \"{valueImage}\" выходит за допустимые пределы");
                        valueImage = "";
                    }

                    k_valueImage++;
                    continue;
                }
                if (k_id == i+1 && k_type == i+1 && k_valueReal == i+1 & k_valueImage == i+1)
                {
                    if(valueReal == "" || valueImage == "")
                    {
                        valueReal = "";
                        valueImage = "";
                        i++;
                        continue;
                    }
                    if (!symbolTable.declare(name, type.Split().Last(), real, image))
                    {
                        AddError(Name, $"Ошибка: идентификатор \"{name}\" уже был объявлен ранее");
                    }
                    valueReal = "";
                    valueImage = "";
                    i++;
                }
            }
            for (i = 0; i < symbolTable.Length(); i++)
            {
                variableDeclNode.type = symbolTable.getType(i);
                variableDeclNode.name = symbolTable.getName(i);
                VariableValueNode tempReal = new VariableValueNode();
                tempReal.name = "RealPart";
                tempReal.type = "double";
                tempReal.Value = symbolTable.getRealPart(i);
                variableDeclNode.realPart = tempReal;
                VariableValueNode tempImage = new VariableValueNode();
                tempImage.name = "ImagePart";
                tempImage.type = "double";
                tempImage.Value = symbolTable.getImagePart(i);
                variableDeclNode.imagPart = tempImage;
                tree.Add(variableDeclNode.Print());
            }
            return tree;
        }
        private void AddError(Token token, string expected)
        {
            string location = token?.location ?? "позиция неизвестна";
            string description = expected;
            errors.Add(new SemError(description, location));
        }
    }
}
