using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private ComplexDeclarationNode сomplexDeclarationNode = new ComplexDeclarationNode();
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
            int k_stringVar = -1;
            int k_stringValueReal = -1;
            int k_stringValueImage = -1;

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
                if(token.id == 4)
                {
                    k_stringVar++;
                    continue;
                }
                if (token.id == 14 && k_stringVar == i && k_valueReal == i && k_valueImage == i)
                {
                    AddError(token, $"Ошибка: инициализирующее значение строка - {token.name} не соотвествует объявленному типу Complex.");
                    k_valueReal++;
                    k_valueImage++;
                    k_stringValueReal++;
                    k_stringValueImage++;
                    i++;
                    continue;
                }
                if (token.id == 6 )
                {
                    k_stringVar--;
                    k_stringValueReal++;
                    continue;
                }
                if (token.id == 14 && k_stringValueReal == i && k_valueReal == i && k_valueImage == i)
                {
                    AddError(token, $"Ошибка: инициализирующее значение строка - {token.name} не соотвествует типу Double.");
                    k_valueReal++;
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
                        AddError(token, $"Ошибка: значение \"{valueReal.Substring(0, 5)}...\" выходит за допустимые пределы.\nОжидалось значение от отрицательного 1,79769313486232e308 до положительного 1,79769313486232e308");
                        valueReal = "";
                    }
                    k_valueReal++;
                    continue;
                }
                if (token.id == 14 && k_stringValueReal == i && k_valueReal - 1 == i && k_valueImage == i)
                {
                    AddError(token, $"Ошибка: инициализирующее значение строка - {token.name} не соотвествует типу Double.");
                    k_valueImage++;
                    k_stringVar++;
                    i++;
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
                        AddError(token, $"Ошибка: значение \"{valueImage.Substring(0,5)}...\" выходит за допустимые пределы.\nОжидалось значение от отрицательного 1,79769313486232e308 до положительного 1,79769313486232e308");
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
                    k_stringVar = i;
                    i++;
                }
            }
            for (i = 0; i < symbolTable.Length(); i++)
            {
                сomplexDeclarationNode.type = symbolTable.getType(i);
                сomplexDeclarationNode.name = symbolTable.getName(i);
                DoubleLiteralNode tempReal = new DoubleLiteralNode();
                tempReal.name = "RealPart";
                tempReal.type = "double";
                tempReal.Value = symbolTable.getRealPart(i);
                сomplexDeclarationNode.realPart = tempReal;
                DoubleLiteralNode tempImage = new DoubleLiteralNode();
                tempImage.name = "ImagePart";
                tempImage.type = "double";
                tempImage.Value = symbolTable.getImagePart(i);
                сomplexDeclarationNode.imagPart = tempImage;
                tree.Add(сomplexDeclarationNode.Print());
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
