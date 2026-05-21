using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class SyntError
    {
        public string invalidFragment { get; set; }
        public string location { get; set; }
        public string description { get; set; }

        public SyntError(string invalidFragment, string location, string description)
        {
            this.invalidFragment = invalidFragment;
            this.location = location;
            this.description = description;
        }
    }

    internal class Parser
    {
        private List<SyntError> errors = new List<SyntError>();
        private List<Token> tokens;
        private int currentPos;
        private Token currentToken;
        private int declarationCount = 0;
        public List<SyntError> Parse(List<Token> tokens)
        {
            //this.tokens = MergeErrorTokensSimple(tokens);
            this.tokens = tokens;
            this.errors = new List<SyntError>();
            this.declarationCount = 0;

            if (tokens == null || tokens.Count == 0)
            {
                errors.Add(new SyntError("", "", "Пустая строка. Введите строку на распознование"));
                return errors;
            }

            currentToken = this.tokens[currentPos];

            while (currentPos < this.tokens.Count)
            {
                if (currentPos >= this.tokens.Count)
                    break;

                int startPos = currentPos;

                bool result1 = START();

                if (result1)
                {
                    declarationCount++;
                }
                else
                {
                    SkipToNextDeclaration();
                }

                if (currentPos <= startPos)
                {
                    GetNextToken(); 
                }
            }


            if (declarationCount > 0 && errors.Count == 0)
            {
                errors.Add(new SyntError("Успешно", "", "Синтаксический анализ завершен без ошибок"));
            }

            return errors;
        }
        private void SkipToNextDeclaration()
        {
            while (currentPos < tokens.Count)
            {
                if (currentToken.id == 1 && currentToken.name == "Complex")
                {
                    return;
                }
                GetNextToken();
            }
        }

        //  1) <START> -> 'Complex' <ID>
        private bool START()
        {
            if (currentToken.id == 1)
            {
                GetNextToken();
                return ID();
            }

            AddError(currentToken, "ключевое слово 'Complex'");
            int tempPos;
            if (tokens.Count == 1) tempPos = currentPos - 1;
            else tempPos = currentPos;
            GetNextToken();

            if (SkipToToken(3))
            {
                return ID();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            GetNextToken();
            AddError(currentToken, "идентификатор (имя переменной)");

            if (SkipToToken(4))
            {
                GetNextToken();
                return NEW();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор присваивания  '='");

            if (SkipToToken(2))
            {
                return NEW();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "ключевое слово 'new'");

            if (SkipToToken(1))
            {
                return TYPE();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "тип 'Complex'");

            if (SkipToToken(6))
            {
                return OPEN_PAREN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "открывающая скобка '('");

            //if (SkipToNumberOrSign())
            //{
            //    return SIGN();
            //}

            //currentPos = tempPos;
            //currentToken = tokens[tempPos];
            //AddError(currentToken, "знак ('+' или '-') или число");

            //if (SkipToToken(12)) // ','
            //{
            //    GetNextToken();
            //    return IMAG_SIGN();
            //}

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");

            if (SkipToToken(7))
            {
                GetNextToken();
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "закрывающая скобка ')'");

            if (SkipToToken(13))
            {
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор заверешения ';' в конце");
            return false;
        }

        //  2) <ID> -> letter <IDREM>
        private bool ID()
        {
            if (currentToken.id == 3)
            {
                GetNextToken();
                return IDREM();
            }

            AddError(currentToken, "идентификатор (имя переменной)");
            int tempPos;
            if (tokens.Count == 1) tempPos = currentPos - 1;
            else tempPos = currentPos;
            if (SkipToToken(4)) 
            {
                GetNextToken();
                return NEW();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор присваивания '='");

            if (SkipToToken(2)) // 'new'
            {
                return NEW();
            }
            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "ключевое слово 'new'");

            if (SkipToToken(1) && tokens.Count != 1)
            {
                return TYPE();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "тип 'Complex' после new");

            if (SkipToToken(6))
            {
                return OPEN_PAREN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "открывающая скобка '('");

            //if (SkipToNumberOrSign())
            //{
            //    return SIGN();
            //}

            //currentPos = tempPos;
            //currentToken = tokens[tempPos];
            //AddError(currentToken, "знак ('+' или '-') или число");

            //if (SkipToToken(12)) // ','
            //{
            //    GetNextToken();
            //    return IMAG_SIGN();
            //}

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(7))
            {
                GetNextToken();
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "закрывающая скобка ')'");

            if (SkipToToken(13))
            {
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор заверешения ';' в конце");
            return false;
        }

        //  3) <IDREM> -> '=' <NEW>
        private bool IDREM()
        {
            if (currentToken.id == 4)
            {
                GetNextToken();
                return NEW();
            }

            AddError(currentToken, "оператор присваивания '='");
            int tempPos;
            if (tokens.Count == 2) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToToken(2)) // 'new'
            {
                return NEW();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "ключевое слово 'new'");

            if (SkipToToken(1)) // 'Complex'
            {
                return TYPE();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "тип 'Complex' после new");

            if (SkipToToken(6))
            {
                return OPEN_PAREN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "открывающая скобка '('");

            //if (SkipToNumberOrSign())
            //{
            //    return SIGN();
            //}

            //currentPos = tempPos;
            //currentToken = tokens[tempPos];
            //AddError(currentToken, "знак ('+' или '-') или число");

            //if (SkipToToken(12)) // ','
            //{
            //    GetNextToken();
            //    return IMAG_SIGN();
            //}

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(7))
            {
                GetNextToken();
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "закрывающая скобка ')'");

            if (SkipToToken(13))
            {
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор заверешения ';' в конце");
            return false;

        }

        //  4) <NEW> -> 'new' <TYPE>
        private bool NEW()
        {
            if (currentToken.id == 2 )
            {
                GetNextToken();
                return TYPE();
            }
            else if (currentToken.id == 14)
            {
                GetNextToken();
                return END();
            }

            AddError(currentToken, "ключевое слово 'new'");
            int tempPos;
            if (tokens.Count == 3) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToToken(1)) // 'Complex'
            {
                return TYPE();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "тип 'Complex' после new");

            if (SkipToToken(6))
            {
                return OPEN_PAREN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "открывающая скобка '('");

            //if (SkipToNumberOrSign())
            //{
            //    return SIGN();
            //}

            //currentPos = tempPos;
            //currentToken = tokens[tempPos];
            //AddError(currentToken, "знак ('+' или '-') или число");

            //if (SkipToToken(12)) // ','
            //{
            //    GetNextToken();
            //    return IMAG_SIGN();
            //}

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(7))
            {
                GetNextToken();
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "закрывающая скобка ')'");

            if (SkipToToken(13))
            {
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор заверешения ';' в конце");
            return false;
        }

        //  5) <TYPE> -> 'Complex' <OPEN_PAREN>
        private bool TYPE()
        {
            if (currentToken.id == 1)
            {
                GetNextToken();
                return OPEN_PAREN();
            }

            AddError(currentToken, "тип 'Complex' после new");

            int tempPos;
            if (tokens.Count <= 4) tempPos = currentPos - 1;
            else tempPos = currentPos;
            if (SkipToToken(6)) // '('
            {
                return OPEN_PAREN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "открывающая скобка '('");

            //if (SkipToNumberOrSign())
            //{
            //    return SIGN();
            //}

            //currentPos = tempPos;
            //currentToken = tokens[tempPos];
            //AddError(currentToken, "знак ('+' или '-') или число");

            //if (SkipToToken(12)) // ','
            //{
            //    GetNextToken();
            //    return IMAG_SIGN();
            //}

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(7))
            {
                GetNextToken();
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "закрывающая скобка ')'");

            if (SkipToToken(13))
            {
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор заверешения ';' в конце");
            return false;
        }

        //  6) <OPEN_PAREN> -> '(' <SIGN>
        private bool OPEN_PAREN()
        {
            if (currentToken.id == 6)
            {
                GetNextToken();
                E();

                // Проверка на запятую между частями
                if (currentToken.id == 12)
                {
                    GetNextToken();
                    E();

                    // Проверка на закрывающую скобку
                    if (currentToken.id == 7)
                    {
                        GetNextToken();
                        return END();
                    }
                }
            }

            AddError(currentToken, "открывающая скобка '('");
            int tempPos;
            if (tokens.Count == 5) tempPos = currentPos - 1;
            else tempPos = currentPos;

            //if (SkipToNumberOrSign())
            //{
            //    return SIGN();
            //}

            //currentPos = tempPos;
            //currentToken = tokens[tempPos];
            //AddError(currentToken, "знак ('+' или '-') или число");

            //if (SkipToToken(12)) // ','
            //{
            //    GetNextToken();
            //    return IMAG_SIGN();
            //}

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(7))
            {
                GetNextToken();
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "закрывающая скобка ')'");

            if (SkipToToken(13))
            {
                return END();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "оператор заверешения ';' в конце");
            return false;
        }


        //  17) <END> -> ';'
        private bool END()
        {
            if (currentToken.id == 13)
            {
                GetNextToken();
                return true;
            }

            AddError(currentToken, "оператор заверешения ';' в конце");
            return true;
        }
        private void AddError(Token token, string expected, bool desc = true)
        {
            string invalidFragment = token?.name ?? "Конец строки";
            string location = token?.location ?? "позиция неизвестна";
            string description = $"Ожидалось {expected}. Встречено '{invalidFragment}'";
            if (desc == false)
            {
                errors.Add(new SyntError(invalidFragment, location, expected));
                return;
            }
            errors.Add(new SyntError(invalidFragment, location, description));
        }
        private bool SkipToToken(int id)
        {
            while (currentPos < tokens.Count)
            {
                if (currentToken.id == id)
                {
                    return true;
                }
                GetNextToken();
            }
            return false;
        }
        private void GetNextToken()
        {
            currentPos++;
            if (currentPos < tokens.Count)
            {
                currentToken = tokens[currentPos];
            }
        }
        
        private void E()
        {
            if (currentToken.id == 14)
            {
                GetNextToken();
                return;
            }
            T();
            A();
        }

        // T -> FB

        private void T()
        {
            F();
            B();
        }

        // A -> epsilon | + TA | - TA

        private void A()
        {
            // Выход по пустой цепочке
            if (currentToken == null) return;

            // Знаки операции
            else if (currentToken.id == 8 || currentToken.id == 9)
            {
                GetNextToken();
                T();
                A();
            }
        }

        // B -> epsilon | * FB | / FB | % FB

        private void B()
        {
            // Выход по пустой цепочке
            if (currentToken == null) return;

            // Знаки операции
            else if (currentToken.id == 15 || currentToken.id == 16 || currentToken.id == 17)
            {
                GetNextToken();
                F();
                B();
            }

            // встретили аргумент
            else if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 6)
            {
                AddError(currentToken, "знак оператора");
                GetNextToken();
                if (currentToken == null) return;
                else if (currentToken.id == 8 || currentToken.id == 9) A();
                else if (currentToken.id == 15 || currentToken.id == 16 || currentToken.id == 17) B();
                else if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 6) T();

            }

            // встретили закрывающую скобку, но нет открывающей
            else if (currentToken.id == 7 && countParenthesis <= 0)
            {
                string bufferName = "";
                string bufferLocation = currentToken.location;
                // проходим по циклу до допустимых символов
                while (currentToken != null)
                {
                    bufferName += currentToken.name;
                    // выход
                    if (currentToken.id != 7)
                    {
                        bufferName = bufferName.Replace(currentToken.name, "");
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, "Лишняя скобка", false);
                        B();
                        break;
                    }
                    GetNextToken();
                    // закончились лексемы
                    if (currentToken == null)
                    {
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, "Лишняя скобка", false);
                        return;
                    }
                }
            }

            // недопустимый символ
            else if (currentToken.id == -1)
            {
                string bufferName = "";
                string bufferLocation = currentToken.location;

                // проходим по циклу до допустимых символов
                while (currentToken != null)
                {
                    bufferName += currentToken.name;
                    // выход
                    if (currentToken.id != -1)
                    {
                        bufferName = bufferName.Replace(currentToken.name, "");
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, "Лексическая ошибка", false);
                        break;
                    }
                    GetNextToken();
                    // закончились лексемы
                    if (currentToken == null)
                    {
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, "Лексическая ошибка", false);
                        return;
                    }
                }
                if (currentToken.id == 8 || currentToken.id == 9) A();
                else if (currentToken.id == 15 || currentToken.id == 16 || currentToken.id == 17) B();
                else if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 6)
                {
                    AddError(currentToken, "знак оператора");
                    T();
                }
            }

        }

        // F -> num | id | ( E )
        private int countParenthesis = 1; // кол-во пар скобок

        private void F()
        {
            // встретили закрывающую скобку или конец строки
            if (currentToken == null || currentToken.id == 7)
            {
                AddError(currentToken, "число, индентификатор или открывающая скобка (");
                if (currentToken == null) return;
                string bufferName = "";
                string bufferLocation = currentToken.location;
                // проходим по циклу до допустимых символов
                while (currentToken != null)
                {
                    bufferName += currentToken.name;
                    // выход
                    if (countParenthesis > 0) break;
                    if (currentToken.id != 7 && countParenthesis <= 0)
                    {
                        bufferName = bufferName.Replace(currentToken.name, "");
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, "Лишняя скобка", false);
                        B();
                        break;
                    }
                    GetNextToken();
                    // закончились лексемы
                    if (currentToken == null)
                    {
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, "Лишняя скобка", false);
                        return;
                    }
                }
                if (currentToken == null) return;
                else if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 7) T();
                else if (currentToken.id == 8 || currentToken.id == 9) A();
                else if (currentToken.id == 15 || currentToken.id == 16 || currentToken.id == 17) B();
                else if (currentToken.id == 8) return;
            }

            // встретили идентификатор или число
            else if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11)
            {
                GetNextToken();
                return;
            }

            // встретили открывающую скобку
            else if (currentToken.id == 6)
            {
                countParenthesis++;
                GetNextToken();
                E();

                if (currentToken == null)
                {
                    AddError(currentToken, "закрывающая скобка )");
                    return;
                }
                else if (currentToken.id == 7)
                {
                    countParenthesis--;
                    GetNextToken();
                    if (currentToken == null || currentToken.id != 7) return;
                    if (countParenthesis > 0) return;
                    string bufferName = "";
                    string bufferLocation = currentToken.location;

                    while (currentToken != null)
                    {
                        bufferName += currentToken.name;
                        if (currentToken.id != 7)
                        {
                            bufferName = bufferName.Replace(currentToken.name, "");
                            Token lastToken = tokens[currentPos - 1];
                            if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                            {
                                bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                            }
                            else { bufferLocation += " \n" + lastToken.location; }
                            Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                            AddError(errorToken, "Лишняя скобка", false);
                            B();
                            return;
                        }
                        GetNextToken();
                        if (currentToken == null)
                        {
                            Token lastToken = tokens[currentPos - 1];
                            if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                            {
                                bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                            }
                            else { bufferLocation += " \n" + lastToken.location; }
                            Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                            AddError(errorToken, "Лишняя скобка", false);
                            return;
                        }
                    }
                }
                if (currentToken.id == -1)
                {
                    while (currentToken.id == -1)
                    {
                        AddError(currentToken, "Лексическая ошибка", false);
                        GetNextToken();
                        if (currentToken == null) break;
                    }

                }
                if (currentToken == null) { }
                else if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 6) T();
                else if (currentToken.id == 8 || currentToken.id == 9) A();
                else if (currentToken.id == 15 || currentToken.id == 16 || currentToken.id == 17) B();
                else if (currentToken.id == 8) return;

                AddError(currentToken, "закрывающая скобка )");
                GetNextToken();
                return;
            }

            // встретили недопустимый символ
            else if (currentToken.id == -1)
            {
                string bufferName1 = currentToken.name;
                string bufferLocation1 = currentToken.location;

                while (currentToken != null)
                {
                    GetNextToken();
                    if (currentToken == null)
                    {
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation1.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation1 = bufferLocation1.Replace('-' + bufferLocation1.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation1 += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName1, bufferLocation1);
                        AddError(errorToken, "Лексическая ошибка", false);

                        AddError(currentToken, "число, индентификатор или открывающая скобка (");
                        return;
                    }
                    bufferName1 += currentToken.name;
                    if (currentToken.id != -1)
                    {
                        bufferName1 = bufferName1.Replace(currentToken.name, "");
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation1.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation1 = bufferLocation1.Replace('-' + bufferLocation1.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation1 += " \n" + lastToken.location; }
                        Token errorToken = new Token(-1, "", bufferName1, bufferLocation1);
                        AddError(errorToken, "Лексическая ошибка", false);
                        break;
                    }
                }
                if (currentToken.id == 8 || currentToken.id == 9)
                {
                    AddError(currentToken, "число, индентификатор или открывающая скобка (");
                    A();
                }
                else if (currentToken.id == 15 || currentToken.id == 16 || currentToken.id == 1)
                {
                    AddError(currentToken, "число, индентификатор или открывающая скобка (");
                    B();
                }
                else if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 6)
                {
                    T();
                }
                return;
            }

            // Проходимся до идентификатора итд
            else
            {
                string bufferName = "";
                string bufferLocation = currentToken.location;

                while (currentToken != null)
                {
                    if (currentToken.id == -1)
                    {
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, " число, индентификатор или открывающая скобка (");


                        return;
                    }
                    bufferName += currentToken.name;
                    if (currentToken.id == 3 || currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 6 || currentToken.id == 7)
                    {
                        bufferName = bufferName.Replace(currentToken.name, "");
                        Token lastToken = tokens[currentPos - 1];
                        if (bufferLocation.Split()[1] == lastToken.location.Split()[1])
                        {
                            bufferLocation = bufferLocation.Replace('-' + bufferLocation.Split()[2].Split('-')[1], '-' + lastToken.location.Split()[2].Split('-')[1]);
                        }
                        else { bufferLocation += " \n" + lastToken.location; }

                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, " число, индентификатор или открывающая скобка (");
                        F();
                        return;
                    }
                    GetNextToken();
                    if (currentToken == null)
                    {
                        Token errorToken = new Token(-1, "", bufferName, bufferLocation);
                        AddError(errorToken, " число, индентификатор или открывающая скобка (");
                        return;
                    }
                }
            }
        }
    }
}
