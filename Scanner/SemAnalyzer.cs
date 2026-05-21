using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace Scanner
{
    public class TACInstruction
    {
        public string Result { get; set; }
        public string Op1 { get; set; }
        public string Operation { get; set; }
        public string Op2 { get; set; }

        public TACInstruction(string result, string op1, string operation, string op2 = null)
        {
            Result = result;
            Op1 = op1;
            Operation = operation;
            Op2 = op2;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Op2))
                return $"{Result} {Operation} {Op1}";
            return $"{Result} = {Op1} {Operation} {Op2}";
        }
    }

    /// <summary>
    /// Генератор трёхадресного кода для объявления комплексного числа
    /// </summary>
    public class TACGenerator
    {
        private List<TACInstruction> instructions = new List<TACInstruction>();
        private int tempCounter = 0;
        private string lastResult = "";

        private string NewTemp() => $"t{++tempCounter}";

        /// <summary>
        /// Генерация TAC для выражения Complex имя = new Complex(число1, число2);
        /// </summary>
        public List<TACInstruction> Generate(string name, double real, double imag)
        {
            instructions.Clear();
            tempCounter = 0;

            // Временные переменные для хранения значений
            string realTemp = NewTemp();
            string imagTemp = NewTemp();

            // Присваивание значений временным переменным
            instructions.Add(new TACInstruction(realTemp, real.ToString(), "="));
            instructions.Add(new TACInstruction(imagTemp, imag.ToString(), "="));

            // Создание комплексного числа
            instructions.Add(new TACInstruction(name, realTemp, imagTemp, "COMPLEX"));

            return instructions;
        }

        /// <summary>
        /// Генерация TAC для выражения с арифметическими операциями
        /// </summary>
        public List<TACInstruction> GenerateWithExpression(string name, string realExpr, string imagExpr)
        {
            instructions.Clear();
            tempCounter = 0;

            // Генерация TAC для реальной части
            string realResult = GenerateExpression(realExpr);

            // Генерация TAC для мнимой части
            string imagResult = GenerateExpression(imagExpr);

            // Создание комплексного числа
            instructions.Add(new TACInstruction(name, realResult, imagResult, "COMPLEX"));

            return instructions;
        }

        /// <summary>
        /// Генерация TAC для арифметического выражения
        /// </summary>
        private string GenerateExpression(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
                return "0";

            expr = expr.Replace(" ", "");

            // Просто число - без операций
            if (double.TryParse(expr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double value))
            {
                string temp = NewTemp();
                instructions.Add(new TACInstruction(temp, value.ToString(), "="));
                return temp;
            }

            // Обработка сложения
            if (expr.Contains('+'))
            {
                string[] parts = expr.Split('+');
                if (parts.Length == 2)
                {
                    string left = GenerateExpression(parts[0]);
                    string right = GenerateExpression(parts[1]);
                    string temp = NewTemp();
                    instructions.Add(new TACInstruction(temp, left, "+", right));
                    return temp;
                }
            }

            // Обработка вычитания
            if (expr.Contains('-') && !expr.StartsWith("-"))
            {
                string[] parts = expr.Split('-');
                if (parts.Length == 2)
                {
                    string left = GenerateExpression(parts[0]);
                    string right = GenerateExpression(parts[1]);
                    string temp = NewTemp();
                    instructions.Add(new TACInstruction(temp, left, "-", right));
                    return temp;
                }
            }

            // Обработка умножения
            if (expr.Contains('*'))
            {
                string[] parts = expr.Split('*');
                if (parts.Length == 2)
                {
                    string left = GenerateExpression(parts[0]);
                    string right = GenerateExpression(parts[1]);
                    string temp = NewTemp();
                    instructions.Add(new TACInstruction(temp, left, "*", right));
                    return temp;
                }
            }

            // Обработка деления
            if (expr.Contains('/'))
            {
                string[] parts = expr.Split('/');
                if (parts.Length == 2)
                {
                    string left = GenerateExpression(parts[0]);
                    string right = GenerateExpression(parts[1]);
                    string temp = NewTemp();
                    instructions.Add(new TACInstruction(temp, left, "/", right));
                    return temp;
                }
            }

            return "0";
        }

        /// <summary>
        /// Вывод TAC в виде строки
        /// </summary>
        public string PrintTAC()
        {
            string result = "";
            for (int i = 0; i < instructions.Count; i++)
            {
                result += $"{instructions[i]}\n";
            }
            return result;
        }
    }

    /// <summary>
    /// Оптимизатор TAC
    /// </summary>
    public class TACOptimizer
    {
        private List<TACInstruction> instructions;
        private Dictionary<string, double> constants;

        public TACOptimizer(List<TACInstruction> instructions)
        {
            this.instructions = instructions;
            this.constants = new Dictionary<string, double>();
        }

        /// <summary>
        /// Свёртка констант
        /// </summary>
        public void FoldConstants()
        {
            // Собираем все константы
            foreach (var instr in instructions)
            {
                if (instr.Operation == "=" && double.TryParse(instr.Op1, out double val))
                {
                    constants[instr.Result] = val;
                }
            }

            // Выполняем свёртку
            for (int i = 0; i < instructions.Count; i++)
            {
                var instr = instructions[i];

                if (instr.Operation == "+" || instr.Operation == "-" ||
                    instr.Operation == "*" || instr.Operation == "/")
                {
                    double leftVal, rightVal;
                    bool leftIsConst = GetValue(instr.Op1, out leftVal);
                    bool rightIsConst = GetValue(instr.Op2, out rightVal);

                    if (leftIsConst && rightIsConst)
                    {
                        double result = 0;
                        switch (instr.Operation)
                        {
                            case "+": result = leftVal + rightVal; break;
                            case "-": result = leftVal - rightVal; break;
                            case "*": result = leftVal * rightVal; break;
                            case "/": result = leftVal / rightVal; break;
                        }
                        instructions[i] = new TACInstruction(instr.Result, result.ToString(), "=");
                        constants[instr.Result] = result;
                    }
                }
            }
        }

        private bool GetValue(string operand, out double value)
        {
            if (double.TryParse(operand, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out value))
                return true;

            return constants.TryGetValue(operand, out value);
        }

        /// <summary>
        /// Упрощение (удаление мёртвых переменных)
        /// </summary>
        public void Simplify()
        {
            // Находим используемые переменные
            HashSet<string> usedVars = new HashSet<string>();

            foreach (var instr in instructions)
            {
                if (!string.IsNullOrEmpty(instr.Op1) && !double.TryParse(instr.Op1, out _))
                    usedVars.Add(instr.Op1);
                if (!string.IsNullOrEmpty(instr.Op2) && !double.TryParse(instr.Op2, out _))
                    usedVars.Add(instr.Op2);
            }

            // Удаляем неиспользуемые временные переменные
            instructions.RemoveAll(instr =>
                instr.Operation == "=" &&
                instr.Result.StartsWith("t") &&
                !usedVars.Contains(instr.Result));
        }

        public List<TACInstruction> GetInstructions() => instructions;

        public string PrintOptimized()
        {
            string result = "";
            for (int i = 0; i < instructions.Count; i++)
            {
                result += $"{instructions[i]}\n";
            }
            return result;
        }
    }

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

    public class DoubleLiteralNode
    {
        public string name { get; set; }
        public string type { get; set; }
        public double Value { get; set; }
    }

    public class ComplexDeclarationNode
    {
        public string type { get; set; }
        public string name { get; set; }
        public DoubleLiteralNode realPart { get; set; }
        public DoubleLiteralNode imagPart { get; set; }
        public List<TACInstruction> realTAC { get; set; }
        public List<TACInstruction> imagTAC { get; set; }

        public List<string> Print()
        {
            List<string> output = new List<string>();
            output.Add($"ComplexDeclarationNode:");
            output.Add($"├── Type: {type}");
            output.Add($"├── Name: {name}");
            output.Add($"└── ValueRealPart : DoubleLiteralNode");
            output.Add($"    └── Value: {realPart?.Value ?? 0}");
            output.Add($"└── ValueImagePart : DoubleLiteralNode");
            output.Add($"    └── Value: {imagPart?.Value ?? 0}");

            if (realTAC != null && realTAC.Count > 0)
            {
                output.Add($"    RealPart TAC:");
                foreach (var instr in realTAC)
                {
                    output.Add($"        {instr}");
                }
            }
            if (imagTAC != null && imagTAC.Count > 0)
            {
                output.Add($"    ImagPart TAC:");
                foreach (var instr in imagTAC)
                {
                    output.Add($"        {instr}");
                }
            }
            return output;
        }
    }

    public class SymbolTableEntry
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double RealPart { get; set; }
        public double ImagPart { get; set; }
        public List<TACInstruction> RealTAC { get; set; }
        public List<TACInstruction> ImagTAC { get; set; }
    }

    public class SymbolTable
    {
        private List<SymbolTableEntry> entries = new List<SymbolTableEntry>();

        public bool declare(string name, string type, double real, double image,
                           List<TACInstruction> realTAC = null, List<TACInstruction> imagTAC = null)
        {
            if (entries.Any(e => e.Name == name))
                return false;

            entries.Add(new SymbolTableEntry
            {
                Name = name,
                Type = type,
                RealPart = real,
                ImagPart = image,
                RealTAC = realTAC,
                ImagTAC = imagTAC
            });
            return true;
        }

        public int Length() => entries.Count;
        public string getName(int i) => entries[i].Name;
        public string getType(int i) => entries[i].Type;
        public double getRealPart(int i) => entries[i].RealPart;
        public double getImagePart(int i) => entries[i].ImagPart;
        public List<TACInstruction> getRealTAC(int i) => entries[i].RealTAC;
        public List<TACInstruction> getImagTAC(int i) => entries[i].ImagTAC;
    }

    internal class SemAnalyzer
    {
        public List<SemError> errors = new List<SemError>();
        private SymbolTable symbolTable = new SymbolTable();
        public string AstOutput { get; private set; } = "";

        public List<List<string>> Analyze(List<Token> tokens)
        {
            List<List<string>> tree = new List<List<string>>();
            AstOutput = "";

            int i = 0;
            int tokenCount = tokens.Count;

            while (i < tokenCount)
            {
                if (i + 8 >= tokenCount) break;
                if (tokens[i].id != 1) { i++; continue; }
                if (tokens[i + 1].id != 3) { i++; continue; }

                string name = tokens[i + 1].name;
                Token nameToken = tokens[i + 1];

                if (tokens[i + 2].id != 4) { i++; continue; }
                if (tokens[i + 3].id != 2) { i++; continue; }
                if (tokens[i + 4].id != 1) { i++; continue; }
                if (tokens[i + 5].id != 6) { i++; continue; }

                int pos = i + 6;

                // Собираем токены реальной части
                List<Token> realTokens = new List<Token>();
                while (pos < tokenCount && tokens[pos].id != 12)
                {
                    realTokens.Add(tokens[pos]);
                    pos++;
                }

                if (pos >= tokenCount || tokens[pos].id != 12) { i++; continue; }
                pos++;

                // Собираем токены мнимой части
                List<Token> imagTokens = new List<Token>();
                while (pos < tokenCount && tokens[pos].id != 7)
                {
                    imagTokens.Add(tokens[pos]);
                    pos++;
                }

                if (pos >= tokenCount || tokens[pos].id != 7) { i++; continue; }
                pos++;

                if (pos >= tokenCount || tokens[pos].id != 13) { i++; continue; }

                // ========== ВЫВОД ИСХОДНОГО TAC ==========
                string outputBuffer = "";
                outputBuffer += "\nIR исходной конструкции в виде трехадресного кода (TAC):\n\n";

                // Генерируем TAC для реальной части через POLIZ
                POLIZ realPoliz = new POLIZ();
                string realResult = "0.0";
                List<Tetrad> realTetrads = realPoliz.calculate(realTokens, "R");

                if (realTokens.Count >= 3)
                {
                    realResult = PrintTetradsToString(realTetrads, ref outputBuffer);
                }
                else
                {
                    realResult = realTokens.First().name;
                }
                

                // Генерируем TAC для мнимой части через POLIZ
                POLIZ imagPoliz = new POLIZ();
                List<Tetrad> imagTetrads = imagPoliz.calculate(imagTokens, "I");
                string imagResult = PrintTetradsToString(imagTetrads, ref outputBuffer);
                outputBuffer += "\n";
                if (imagTokens.Count < 3) imagResult = imagTokens.First().name;

                // Финальное присваивание
                outputBuffer += $"{name} = new Complex({realResult}, {imagResult})\n\n";

                // Вычисляем значения через POLIZ
                
                double realValue = EvaluateTetrads(realTetrads);
                if (realTetrads.Count == 0) realValue = double.Parse(realResult.Replace('.',','));
                double imagValue = EvaluateTetrads(imagTetrads);
                if (imagTetrads.Count == 0) imagValue = double.Parse(imagResult.Replace('.', ','));

                // ========== ОПТИМИЗАЦИЯ 1 ==========
                outputBuffer += "Оптимизация №1: Свертка констант (constant folding)\n\n";

                List<Tetrad> optimizedRealTetrads = OptimizeTetrads(realTetrads);
                List<Tetrad> optimizedImagTetrads = OptimizeTetrads(imagTetrads);

                PrintOptimizedTetradsToString(realTetrads, imagTetrads, ref outputBuffer);

                double optimizedRealValue = realValue;
                double optimizedImagValue = imagValue;

                // ========== ОПТИМИЗАЦИЯ 2 ==========
                outputBuffer += "Оптимизация №2: Упрощение комплексного числа с нулевой мнимой частью\n\n";

                if (Math.Abs(optimizedImagValue) < double.Epsilon)
                {
                    outputBuffer += $"Условие: мнимая часть = 0\n";
                    outputBuffer += $"Complex {name} = new Complex({optimizedRealValue}, 0);\n";
                    outputBuffer += $"↓\n";
                    outputBuffer += $"double {name} = {optimizedRealValue};\n\n";
                }
                else
                {
                    outputBuffer += $"Условие не выполнено (мнимая часть = {optimizedImagValue})\n\n";
                }

                // ========== РЕЗУЛЬТАТ ==========
                outputBuffer += "Результат после оптимизаций:\n\n";

                if (Math.Abs(optimizedImagValue) < double.Epsilon)
                {
                    outputBuffer += $"double {name} = {optimizedRealValue}\n";
                }
                else
                {
                    outputBuffer += $"Complex {name} = new Complex({optimizedRealValue}, {optimizedImagValue})\n";
                }

                outputBuffer += "\n";


                // Проверка уникальности
                if (!symbolTable.declare(name, "Complex", optimizedRealValue, optimizedImagValue))
                {
                    AddError(nameToken, $"Ошибка: идентификатор \"{name}\" уже был объявлен ранее");
                }

                // AST
                var astNode = new ComplexDeclarationNode
                {
                    type = "Complex",
                    name = name,
                    realPart = new DoubleLiteralNode { name = "RealPart", type = "double", Value = optimizedRealValue },
                    imagPart = new DoubleLiteralNode { name = "ImagPart", type = "double", Value = optimizedImagValue }
                };
                tree.Add(astNode.Print());
                TACGenerator generator = new TACGenerator();
                var tac1 = generator.Generate(name, optimizedRealValue, optimizedImagValue);
                outputBuffer = outputBuffer.Insert( 0,"промежуточное представление (IR):\n\n" + generator.PrintTAC());
                AstOutput += outputBuffer;

                i = pos + 1;
            }

            return tree;
        }

        private string PrintTetradsToString(List<Tetrad> tetrads, ref string output)
        {
            string lastResult = "";

            foreach (var tet in tetrads)
            {
                output += $"{tet.result} = {tet.argument1} {tet.operation} {tet.argument2}\n";
                lastResult = $"{tet.result}";
            }

            return lastResult;
        }

        private void PrintOptimizedTetradsToString(List<Tetrad> realTet, List<Tetrad> imagTet, ref string output)
        {
            double x, y, resultOperation = 0;
            Dictionary<string, double> tetradResult = new Dictionary<string, double>();

            foreach (Tetrad tetrad in realTet)
            {
                if (!double.TryParse(tetrad.argument1.Replace('.', ','), out x))
                {
                    tetradResult.TryGetValue(tetrad.argument1, out x);
                }
                if (!double.TryParse(tetrad.argument2.Replace('.', ','), out y))
                {
                    tetradResult.TryGetValue(tetrad.argument2, out y);
                }
                switch (tetrad.operation[0])
                {
                    case '-':
                        resultOperation = x - y;
                        break;
                    case '+':
                        resultOperation = x + y;
                        break;
                    case '%':
                        resultOperation = x % y;
                        break;
                    case '/':
                        resultOperation = x / y;
                        break;
                    case '*':
                        resultOperation = x * y;
                        break;
                }
                tetradResult.Add(tetrad.result, resultOperation);
                output += $"{tetrad.result} = {resultOperation}\n";
            }
            output += "\n";

            foreach (Tetrad tetrad in imagTet)
            {

                if (!double.TryParse(tetrad.argument1.Replace('.', ','), out x))
                {
                    tetradResult.TryGetValue(tetrad.argument1, out x);
                }
                if (!double.TryParse(tetrad.argument2.Replace('.', ','), out y))
                {
                    tetradResult.TryGetValue(tetrad.argument2, out y);
                }
                switch (tetrad.operation[0])
                {
                    case '-':
                        resultOperation = x - y;
                        break;
                    case '+':
                        resultOperation = x + y;
                        break;
                    case '%':
                        resultOperation = x % y;
                        break;
                    case '/':
                        resultOperation = x / y;
                        break;
                    case '*':
                        resultOperation = x * y;
                        break;
                }
                tetradResult.Add(tetrad.result, resultOperation);
                output += $"{tetrad.result} = {resultOperation}\n";
            }
        }

        private List<Tetrad> OptimizeTetrads(List<Tetrad> tetrads)
        {
            List<Tetrad> result = new List<Tetrad>();
            Dictionary<string, double> computed = new Dictionary<string, double>();

            foreach (var tet in tetrads)
            {
                double leftVal, rightVal;
                bool leftIsConst = TryGetValue(tet.argument1, computed, out leftVal);
                bool rightIsConst = TryGetValue(tet.argument2, computed, out rightVal);

                if (leftIsConst && rightIsConst)
                {
                    double computedVal = Compute(tet.operation, leftVal, rightVal);
                    string tempName = tet.result;
                    computed[tempName] = computedVal;
                    result.Add(new Tetrad("=", computedVal.ToString(), "", tempName));
                }
                else
                {
                    result.Add(tet);
                }
            }

            return result;
        }

        private double EvaluateTetrads(List<Tetrad> tetrads)
        {
            if (tetrads.Count == 0) return 0;

            Dictionary<string, double> values = new Dictionary<string, double>();

            foreach (var tet in tetrads)
            {
                double leftVal, rightVal;
                bool leftIsConst = TryGetValue(tet.argument1, values, out leftVal);
                bool rightIsConst = TryGetValue(tet.argument2, values, out rightVal);

                if (leftIsConst && rightIsConst)
                {
                    values[tet.result] = Compute(tet.operation, leftVal, rightVal);
                }
            }

            if (tetrads.Count > 0 && values.ContainsKey(tetrads.Last().result))
                return values[tetrads.Last().result];

            return 0;
        }

        private bool TryGetValue(string arg, Dictionary<string, double> computed, out double val)
        {
            if (double.TryParse(arg, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out val))
                return true;

            return computed.TryGetValue(arg, out val);
        }

        private double Compute(string op, double left, double right)
        {
            switch (op)
            {
                case "+": return left + right;
                case "-": return left - right;
                case "*": return left * right;
                case "/": return left / right;
                case "%": return left % right;
                default: return 0;
            }
        }

        private void AddError(Token token, string message)
        {
            errors.Add(new SemError(message, token?.location ?? "неизвестно"));
        }
    }
}