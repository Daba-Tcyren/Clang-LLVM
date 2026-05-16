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
            this.tokens = MergeErrorTokensSimple(tokens);
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

            if (SkipToNumberOrSign())
            {
                return SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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

            if (SkipToNumberOrSign())
            {
                return SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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

            if (SkipToNumberOrSign())
            {
                return SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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
            if (currentToken.id == 2)
            {
                GetNextToken();
                return TYPE();
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

            if (SkipToNumberOrSign())
            {
                return SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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

            if (SkipToNumberOrSign())
            {
                return SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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
                return SIGN();
            }

            AddError(currentToken, "открывающая скобка '('");
            int tempPos;
            if (tokens.Count == 5) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToNumberOrSign())
            {
                return SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "знак ('+' или '-') или число");

            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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

        //  7) <SIGN> -> '+' <DIGIT_REAL> | '-' <DIGIT_REAL> | digit <REAL>
        private bool SIGN()
        {
            // Знак '+' '-'
            if (currentToken.id == 9 || currentToken.id == 8)
            {
                GetNextToken();
                return DIGIT_REAL();
            }
            
            // Число без знака
            if (currentToken.id == 10 || currentToken.id == 11)
            {
                return REAL();
            }

            AddError(currentToken, "знак ('+' или '-') или число");
            int tempPos;
            if (tokens.Count == 6) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToNumberOrSignOrCommo())
            {
                if (currentToken.id == 9 || currentToken.id == 8)
                {
                    GetNextToken();
                    return DIGIT_REAL();
                }
                return REAL();
            }
            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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

        //  8) <DIGIT_REAL> -> digit <REAL>
        private bool DIGIT_REAL()
        {
            if (currentToken.id == 10 || currentToken.id == 11)
            {
                return REAL();
            }

            AddError(currentToken, "число после знака реальной части");
            int tempPos;
            if (tokens.Count == 7) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToNumber())
            {
                return REAL();
            }

            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

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

        //  9) <REAL> -> digit <REAL> | '.' <REAL_DOT> | ',' <IMAG_SIGN>
        private bool REAL()
        {
            if (currentToken.id == 10 || currentToken.id == 11)
            {
                // Проверяем, есть ли в числе точка
                if (currentToken.name.Contains('.'))
                {
                    return REAL_DOT();
                }

                GetNextToken();
            }

            // Проверка на запятую между частями
            if (currentToken.id == 12)
            {
                GetNextToken();
                return IMAG_SIGN();
            }

            int tempPos;
            if (tokens.Count <= 8) tempPos = currentPos - 1;
            else tempPos = currentPos;
            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");

            if (SkipToNumberOrSign() && currentPos != tokens.Count - 1)
            {
                return IMAG_SIGN();
            }

            AddError(currentToken, "знак ('+' или '-') или число");
            currentPos = tempPos;
            currentToken = tokens[tempPos];
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

        //  10) <REAL_DOT> -> digit <REAL_FRACTION>
        private bool REAL_DOT()
        {
            string number = currentToken.name;
            int dotIndex = number.IndexOf('.');

            if (dotIndex >= 0 && dotIndex < number.Length - 1)
            {
                GetNextToken();
                return REAL_FRACTION();
            }

            AddError(currentToken, "число после точки в реальной части");

            int tempPos;
            if (tokens.Count <= 8) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToToken(10)) // ','
            {
                GetNextToken();
                return REAL_FRACTION();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            
            if (SkipToToken(12)) // ','
            {
                GetNextToken();
                return IMAG_SIGN();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            AddError(currentToken, "запятая ','");

            if (SkipToNumberOrSign() && currentPos != tokens.Count - 1)
            {
                return IMAG_SIGN();
            }

            AddError(currentToken, "знак ('+' или '-') или число");
            currentPos = tempPos;
            currentToken = tokens[tempPos];
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

        //  11) <REAL_FRACTION> -> digit <REAL_FRACTION> | ',' <IMAG_SIGN>
        private bool REAL_FRACTION()
        {
            if (currentToken.id == 12)
            {
                GetNextToken();
                return IMAG_SIGN();
            }

            AddError(currentToken, "  ',' между действительной и мнимой частями");
            int tempPos;
            if (tokens.Count <= 8) tempPos = currentPos - 1;
            else tempPos = currentPos;
            if (SkipToToken(12))
            {
                return IMAG_SIGN();
            }
            currentPos = tempPos;
            currentToken = tokens[tempPos];
            
            if (SkipToNumberOrSign() && currentPos != tokens.Count - 1)
            {
                return IMAG_SIGN();
            }
            AddError(currentToken, "знак ('+' или '-') или число");
            currentPos = tempPos;
            currentToken = tokens[tempPos];
            
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

        //  12) <IMAG_SIGN> -> '+' <DIGIT_IMAG> | '-' <DIGIT_IMAG> | digit <IMAG>
        private bool IMAG_SIGN()
        {
            // Знак '+' '-'
            if (currentToken.id == 9 || currentToken.id == 8)
            {
                GetNextToken();
                return DIGIT_IMAG();
            }

            // Число без знака
            if (currentToken.id == 10 || currentToken.id == 11)
            {
                return IMAG();
            }

            AddError(currentToken, "знак ('+' или '-') или число для мнимой части");

            int tempPos;
            if (tokens.Count <= 9) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToNumberOrSign())
            {
                if (currentToken.id == 9 || currentToken.id == 8)
                {
                    GetNextToken();
                    return DIGIT_IMAG();
                }
                return IMAG();
            }

            currentPos = tempPos;
            currentToken = tokens[tempPos];
            if (SkipToToken(7)) // ')'
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

        //  13) <DIGIT_IMAG> -> digit <IMAG>
        private bool DIGIT_IMAG()
        {
            if (currentToken.id == 10 || currentToken.id == 11)
            {
                return IMAG();
            }

            AddError(currentToken, "число после знака мнимой части");
            int tempPos;
            if (tokens.Count <= 10) tempPos = currentPos - 1;
            else tempPos = currentPos;
            if (SkipToNumber())
            {
                return IMAG();
            }

            if (SkipToToken(7)) // ')'
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

        //  14) <IMAG> -> digit <IMAG> | '.' <IMAG_DOT> | ')' <END>
        private bool IMAG()
        {
            if (currentToken.id == 10 || currentToken.id == 11)
            {
                if (currentToken.name.Contains('.'))
                {
                    return IMAG_DOT();
                }
                GetNextToken();
            }

            // Проверка на закрывающую скобку
            if (currentToken.id == 7)
            {
                GetNextToken();
                return END();
            }

            int tempPos;
            if (tokens.Count <= 11) tempPos = currentPos - 1;
            else tempPos = currentPos;

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

        //  15) <IMAG_DOT> -> digit <IMAG_FRACTION>
        private bool IMAG_DOT()
        {
            string number = currentToken.name;
            int dotIndex = number.IndexOf('.');

            if (dotIndex >= 0 && dotIndex < number.Length - 1)
            {
                GetNextToken();
                return IMAG_FRACTION();
            }

            AddError(currentToken, "число после точки в мнимой части");

            int tempPos;
            if (tokens.Count <= 11) tempPos = currentPos - 1;
            else tempPos = currentPos;

            if (SkipToToken(7)) // ')'
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

        //  16) <IMAG_FRACTION> -> digit <IMAG_FRACTION> | ')' <END>
        private bool IMAG_FRACTION()
        {
            if (currentToken.id == 7)
            {
                GetNextToken();
                return END();
            }

            AddError(currentToken, "закрывающая скобка ')'");
            int tempPos;
            if (tokens.Count <= 12) tempPos = currentPos - 1;
            else tempPos = currentPos;
            if (SkipToToken(7))
            {
                GetNextToken();
                return END();
            }
            currentPos = tempPos;
            currentToken = tokens[tempPos];
            if (SkipToToken(13)) // ';'
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
        private void AddError(Token token, string expected)
        {
            string invalidFragment = token?.name ?? "Конец строки";
            string location = token?.location ?? "позиция неизвестна";
            string description = $"Ожидалось {expected}. Встречено '{invalidFragment}'";
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
        private bool SkipToNumberOrSign()
        {
            while (currentPos < tokens.Count)
            {
                if (currentToken.id == 8 || currentToken.id == 9 ||
                    currentToken.id == 10 || currentToken.id == 11)
                {
                    return true;
                }
                GetNextToken();
            }
            return false;
        }
        private bool SkipToNumberOrSignOrCommo()
        {
            while (currentPos < tokens.Count)
            {
                if (currentToken.id == 8 || currentToken.id == 9 ||
                    currentToken.id == 10 || currentToken.id == 11 || currentToken.id == 12)
                {
                    return true;
                }
                GetNextToken();
            }
            return false;
        }
        private bool SkipToNumber()
        {
            while (currentPos < tokens.Count)
            {
                if (currentToken.id == 10 || currentToken.id == 11)
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

        public List<Token> MergeErrorTokensSimple(List<Token> tokens)
        {
            List<Token> result = new List<Token>();

            for (int i = 0; i < tokens.Count; i++)
            {
                Token currentToken = tokens[i];

                if (currentToken.id == 1 || currentToken.id == 3 || currentToken.id == -1)
                {
                    string mergedName = currentToken.name;
                    string location = currentToken.location;
                    int lastIndex = i;
                    bool hasErrors = (currentToken.id == -1);

                    while (lastIndex + 1 < tokens.Count)
                    {
                        Token nextToken = tokens[lastIndex + 1];

                        // Если следующий токен - пробел, прерываем объединение
                        if (nextToken.id == 5)
                        {
                            break;
                        }

                        // Если следующий токен - оператор или разделитель
                        if (nextToken.id == 6 || nextToken.id == 7 || nextToken.id == 11 ||
                            nextToken.id == 12 || nextToken.id == 4 || nextToken.id == 8 ||
                            nextToken.id == 13)
                        {
                            break;
                        }

                        // Объединяем части идентификатора
                        if (nextToken.id == 1 || nextToken.id == 3 || nextToken.id == -1)
                        {
                            mergedName += nextToken.name;
                            lastIndex++;
                            if (nextToken.id == -1) hasErrors = true;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Создаём итоговый токен
                    if (lastIndex > i || hasErrors)
                    {
                        int finalId = hasErrors || mergedName.Any(c => !char.IsLetterOrDigit(c) && c != '_') ? -1 :
                                     mergedName == "Complex" ? 1 :
                                     mergedName == "new" ? 2 : 3;

                        string finalType = finalId == -1 ? "Недопустимый идентификатор" :
                                          finalId == 1 ? "Ключевое слово Complex" :
                                          finalId == 2 ? "Ключевое слово new" :
                                          "Идентификатор";

                        result.Add(new Token(finalId, finalType, mergedName, location));
                        i = lastIndex;
                    }
                    else
                    {
                        result.Add(currentToken);
                    }
                }
                else if (currentToken.id != 5) // Пропускаем токены пробелов
                {
                    result.Add(currentToken);
                }
            }

            return result;
        }
    }
}
