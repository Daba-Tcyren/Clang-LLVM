using System;
using System.Collections.Generic;

namespace Scanner
{
    public class Token
    {
        public readonly int id;
        public readonly string type;
        public readonly string name;
        public readonly string location;
        public Token(int id, string type, string name, string location)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.location = location;
        }
    }
    public class scanner
    {
        private string text;
        private char liter;
        private int currentPosition = 0;
        private int positionLine = 0;
        private int currentLine = 1;
        private string buffer = "";
        private List<Token> tokens = new List<Token>();
        public List<Token> analyze(string inputText)
        {
            text = inputText;

            getNext();

            while (currentPosition <= text.Length)
            {
                if (char.IsLetter(liter))
                {
                    buffer += liter;
                    while (char.IsLetterOrDigit(liter = getChar()))
                    {
                        buffer += liter;
                    }
                    switch (buffer)
                    {
                        case "Complex":
                            addToken(1, "Ключевое слово Complex", buffer);
                            break;
                        case "new":
                            addToken(2, "Ключевое слово new", buffer);
                            break;
                        default:
                            addToken(3, "Идентификатор", buffer);
                            break;
                    }
                    buffer = "";
                }
                else if (char.IsDigit(liter))
                {
                    buffer += liter;
                    while (char.IsDigit(liter = getChar()))
                    {
                        buffer += liter;
                    }
                    if (liter == '.')
                    {
                        buffer += liter;
                        while (char.IsDigit(liter = getChar()))
                        {
                            buffer += liter;
                        }
                        addToken(11, "Вещественное число", buffer);
                    }
                    else
                    {
                        addToken(10, "Целое без знака", buffer);
                    }
                    buffer = "";
                }
                else
                {
                    switch (liter)
                    {
                        case '\0':
                            getNext();
                            break;
                        case '\n':
                            positionLine = 0;
                            currentLine++;
                            getNext();
                            break;
                        case '=':
                            buffer += liter;
                            getNext();
                            addToken(4, "Оператор присваивания", buffer);
                            buffer = "";
                            break;
                        case ' ':
                            buffer += liter;
                            getNext();
                            addToken(5, "Разделитель", buffer);
                            buffer = "";
                            break;
                        case '(':
                            buffer += liter;
                            getNext();
                            addToken(6, "Оператор конструктора", buffer);
                            buffer = "";
                            break;
                        case ')':
                            buffer += liter;
                            getNext();
                            addToken(7, "Оператор конструктора", buffer);
                            buffer = "";
                            break;
                        case '-':
                            buffer += liter;
                            getNext();
                            addToken(8, "Знак минуса", buffer);
                            buffer = "";
                            break;
                        case '+':
                            buffer += liter;
                            getNext();
                            addToken(9, "Знак плюса", buffer);
                            buffer = "";
                            break;
                        case ',':
                            buffer += liter;
                            getNext();
                            addToken(12, "Оператор перечисления", buffer);
                            buffer = "";
                            break;
                        case ';':
                            buffer += liter;
                            getNext();
                            addToken(13, "Оператор заверешения", buffer);
                            buffer = "";
                            break;
                        case '"':
                            buffer += liter;
                            while ((liter = getChar())!='"' && liter != '\0' && currentPosition < text.Length)
                            {
                                buffer += liter;
                            }
                            buffer += liter;
                            getNext();
                            addToken(14, "Строка", buffer);
                            buffer = "";
                            break;
                        default:
                            buffer += liter;
                            getNext();
                            addToken(-1, "Недопустимый символ", buffer);
                            buffer = "";
                            break;
                    }
                }
            }

            return tokens;
        }
        private char getChar()
        {
            try
            {
                if (currentPosition >= text.Length)
                {
                    currentPosition++;
                    positionLine++;
                    return '\0';
                }
                char liter1 = text[currentPosition];
                currentPosition++;
                positionLine++;
                return liter1;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception("В конце строки не обнаружено ;");
            }
        }
        private void getNext()
        {
            liter = getChar();
        }
        private void addToken(int id, string type, string name)
        {
            int Length = name.Length;
            int leng = positionLine - Length;
            string loc = $"строка {currentLine}, {leng}-{positionLine - 1}";
            tokens.Add(new Token(id, type, name, loc));
        }
    }
}
