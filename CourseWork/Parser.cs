using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace CourseWork
{
    public class Parser
    {
        
        readonly char Separator;
        bool isRadians;
       
        double X { get; set; } // Текущее значение переменной
        

        public Parser()
        { 
            Separator = Char.Parse(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }


        const string NumberMaker = "#";
        const string OperatorMarker = "$";
        const string FunctionMarker = "@";
        const string VariableMarker = "~";
        

        const string Plus = OperatorMarker + "+";
        const string UnPlus = OperatorMarker + "un+";
        const string Minus = OperatorMarker + "-";
        const string UnMinus = OperatorMarker + "un-";
        const string Multiply = OperatorMarker + "*";
        const string Divide = OperatorMarker + "/";
        const string Degree = OperatorMarker + "^";
        const string LeftParent = OperatorMarker + "(";
        const string RightParent = OperatorMarker + ")";
        const string Sqrt = FunctionMarker + "sqrt";
        const string Sin = FunctionMarker + "sin";
        const string Cos = FunctionMarker + "cos";
        const string Tg = FunctionMarker + "tg";
        const string Ctg = FunctionMarker + "ctg";
        const string Sh = FunctionMarker + "sh";
        const string Ch = FunctionMarker + "ch";
        const string Th = FunctionMarker + "th";
        const string Ln = FunctionMarker + "ln";
        const string Abs = FunctionMarker + "abs";

        readonly Dictionary<string, string> supportedVariable = // поддерживаемая переменная
            new Dictionary<string, string>
            {
                {"x",VariableMarker}
            };
        readonly Dictionary<string, string> supportedOperators = //поддерживаемые операторы
            new Dictionary<string, string>
            {
                { "+", Plus },
                { "-", Minus },
                { "*", Multiply },
                { "/", Divide },
                { "^", Degree },
                { "(", LeftParent },
                { ")", RightParent }
            };
        readonly Dictionary<string, string> supportedFunctions = // поддерживаемые функции
            new Dictionary<string, string>
            {
                { "sqrt", Sqrt },
                { "sin", Sin },
                { "cos", Cos },
                { "tg", Tg },
                { "ctg", Ctg },
                { "sh", Sh },
                { "ch", Ch },
                { "th", Th },
                {"ln",Ln },
                { "abs", Abs }
            };
        readonly Dictionary<string, string> supportedConstants = // поддерживаемые константы
            new Dictionary<string, string>
            {
                {"pi", NumberMaker +  Math.PI.ToString() },
                {"e", NumberMaker + Math.E.ToString() }
            };

        public string Parse(string expression)
        {
            try
            {
                return ConvertToRPN(FormatString(expression));// форматируем и конвертируем строку
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Вычисляет значение введенной строки, записанной в обратной польской записи, при указаном значении Х
        /// </summary>
        /// <param name="expression">Математическое выражение в постфиксной записи(обратная польская запись)</param>
        /// <param name="X">Текущее значение переменной</param>
        /// <returns>Результат вычисления(число)</returns>
        public double Calculate(string expression, double X, bool isRadians = true)
        {
            try
            {
                this.isRadians = isRadians;
                this.X = X;

                int pos = 0; // Счетчик 
                var stack = new Stack<double>(); // Стек который будет содержать операнды

                // Анализ введенного выражения
                while (pos < expression.Length)
                {
                    string token = LexicalAnalysisRPN(expression, ref pos);
                    // получает токены из строки, записанной в форме обратной польской записи
                    stack = SyntaxAnalysisRPN(stack, token);
                    // формирует стек на основе токена и содержимого стека
                }

                //В конце анализа в стеке должен быть только один операнд (результат подсчета)
                if (stack.Count > 1)
                {
                    throw new ArgumentException("Имеется избыточный операнд.");
                }
                return stack.Pop();
            }
            catch (InvalidOperationException )
            {
                throw new InvalidOperationException("Введены некорректные операнды.");
            }
            catch (Exception e)
            {
                throw e;
            }

        }



        /// <summary>
        /// Конвертирует строку в формат обратную польской записи (reverse polition notation)
        /// </summary>
        /// <param name="expression">Математическое выражение в инфиксной записи</param>
        /// <returns>Математичексое выражение в постфиксной записи (RPN)</returns>
        private string ConvertToRPN(string expression) 
        {
            int pos = 0; // счетчик строки с выражением
            StringBuilder outputString = new StringBuilder();// строка, в которой будем формировать обратную польскую запись
                                                             // т.к необходимо будет 
                                                             //использовать повторяющиеся модификации строки, 
                                                             //издержки, связанные с созданием нового объекта String, 
                                                             //могут оказаться значительными, 
                                                             //поэтому будем использовать StringBuilder

            Stack<string> stack = new Stack<string>();// в стек будут записываться операции и функции 

            // Пока есть симмволы в необработанном мат. выражении
            while (pos < expression.Length)
            {
                string token = LexicalAnalysisInfixNotation(expression, ref pos);
                // Производит токен
                outputString = SyntaxAnalysisInfixNotation(token, outputString, stack);
                // Формирует обратную польскую запись по блок-схеме, описанной в 2.2
            }

            // Извлекаем элементы из стека в строку            
            while (stack.Count > 0)
            {
                // В этом стеке должны быть только операторы, но в него также добавлялсь и функции
                if (stack.Peek()[0] == OperatorMarker[0])
                {
                    outputString.Append(stack.Pop());
                }
                // Функция остатся в стеке только в том случае, если она записана без скобок
                else
                {
                    throw new FormatException("Есть функция без круглых скобок.");
                }
            }
            return outputString.ToString();
        }

        /// <summary>
        /// Производит токен с помощью данного математического выражения
        /// </summary>
        /// <param name="expression">Математическое выражение в инфиксной записи</param>
        /// <param name="pos">Текущая позиция в строке для лексического анализа</param>
        /// <returns>Токен</returns>
        string LexicalAnalysisInfixNotation(string expression, ref int pos)
        {
            // Получаем символ
            StringBuilder token = new StringBuilder();
            token.Append(expression[pos]);
            // Если это оператор
            if (supportedOperators.ContainsKey(token.ToString()))
            {
                // Определеям, унарный ли это оператор 
                bool isUnary = pos == 0 || expression[pos - 1] == '(';
                pos++;
                switch (token.ToString())
                {
                    case "+":
                        return isUnary ? UnPlus : Plus;
                    case "-":
                        return isUnary ? UnMinus : Minus;
                    default:
                        return supportedOperators[token.ToString()];// возвращаем искомый токен
                }
            }
            // Если встретились буквы, то это либо переменная, либо функция, либо константа
            else if (Char.IsLetter(token[0])
                || supportedFunctions.ContainsKey(token.ToString())
                || supportedConstants.ContainsKey(token.ToString())
                || supportedVariable.ContainsKey(token.ToString()))
            {
                // Читаем имя функции, перемнной или константы полностью
                while (++pos < expression.Length
                    && Char.IsLetter(expression[pos]))
                {
                    token.Append(expression[pos]);
                }

                if (supportedFunctions.ContainsKey(token.ToString()))
                {
                    return supportedFunctions[token.ToString()];
                }
                else if (supportedConstants.ContainsKey(token.ToString()))
                {
                    return supportedConstants[token.ToString()];
                }
                else if (supportedVariable.ContainsKey(token.ToString()))
                {
                    return supportedVariable[token.ToString()];
                }
                else
                {// если токена нет ни в одном словаре, то этот символ является некорректным
                    throw new ArgumentException("Некорректно введено выражение.");
                }

            }
            // Если встретились числа
            else if (Char.IsDigit(token[0]) || token[0] == Separator)
            {
                // Читаем число

                // Читаем целую часть числа
                if (Char.IsDigit(token[0]))
                {
                    while (++pos < expression.Length && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                }
                else
                {
                    // Удалим разделитель,потому что он будет добавлен ниже
                    token.Clear();
                }

                // Читаем дробную часть числа
                if (pos < expression.Length
                    && expression[pos] == Separator)
                {
                    // Добавляем существующий системный десятичный разделитель
                    token.Append(Separator);
                    while (++pos < expression.Length
                    && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                    
                }

                return NumberMaker + token.ToString();
            }
            else
            {
                // если символ не относится ни к одному словарю и не является числом, то символ некорректный
                throw new ArgumentException("Некорректно введено выражение.");
            }
        }

        /// <summary>
        /// Синтаксический анализ инфиксной записи
        /// </summary>
        /// <param name="token">Токен</param>
        /// <param name="outputString">Строка (мат выражение в обратной польской записи)</param>
        /// <param name="stack">Стек который содержит операторы или функции</param>
        /// <returns>Строка (мат выражение в обратной польской записи)</returns>
        private StringBuilder SyntaxAnalysisInfixNotation(string token, StringBuilder outputString, Stack<string> stack)
        {
            // Если это число или переменная, просто введем в строку            
            if (token[0] == NumberMaker[0] || token[0]==VariableMarker[0])
            {
                outputString.Append(token);
            }
            else if (token[0] == FunctionMarker[0])
            {
                // Если это функция, положим в стек
                stack.Push(token);
            }
            else if (token == LeftParent)
            {
                // Если это '(' , положим в стек
                stack.Push(token);
            }
            else if (token == RightParent)
            {
                // Если это ')' то все элементы из стека введем в строку, пока не дойдем до '('               
                string elem;
                while ((elem = stack.Pop()) != LeftParent)
                {
                    outputString.Append(elem);
                }
                // если функция находится в вершине стека, то запишем ее в строку
                if (stack.Count > 0 &&
                    stack.Peek()[0] == FunctionMarker[0])
                {
                    outputString.Append(stack.Pop());//извлекаем функцию из стека и вставляем в строку
                }
            }
            else
            {
                // Пока приоритет элемента находящегося на вершине стека >= (>) приоритета токена запишем этот элемент из стека в строку
                while (stack.Count > 0 &&
                    Priority(token, stack.Peek()))
                {
                    outputString.Append(stack.Pop());//добавляем верхний элемент из стека в строку
                }
                stack.Push(token);// вставлем токен в стек
            }
            return outputString;
        }


        /// <summary>
        /// Вычисление обратной польской записи
        /// </summary>
        /// <param name="stack">Стек который содержит операнды</param>
        /// <param name="token">Текущий токен</param>
        /// <returns>Стек который в итоге должен содержать только один операнд</returns>
        private Stack<double> SyntaxAnalysisRPN(Stack<double> stack, string token)
        {
            // если это операнд, то просто вставляем его в стек
            if (token[0] == NumberMaker[0] )
            {
                stack.Push(double.Parse(token.Remove(0, 1)));
            }
            // если это переменная, то вставляем текущее значение переменной в стек
            else if (token[0] == VariableMarker[0])
            {
                stack.Push(X);
            }
            // В противном случае применяем оператор или функцию к элементам в стеке
            else if (NumberOfArguments(token) == 1)
            {
                double arg = stack.Pop();
                double rst;

                switch (token)
                {
                    case UnPlus:
                        rst = arg;
                        break;
                    case UnMinus:
                        rst = -arg;
                        break;
                    case Sqrt:
                        rst = Math.Sqrt(arg);
                        break;
                    case Sin:
                        rst = ApplyTrigFunction(Math.Sin, arg);
                        break;
                    case Cos:
                        rst = ApplyTrigFunction(Math.Cos, arg);
                        break;
                    case Tg:
                        rst = ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case Ctg:
                        rst = 1 / ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case Sh:
                        rst = Math.Sinh(arg);
                        break;
                    case Ch:
                        rst = Math.Cosh(arg);
                        break;
                    case Th:
                        rst = Math.Tanh(arg);
                        break;
                    case Ln:
                        rst = Math.Log(arg);
                        break;
                    case Abs:
                        rst = Math.Abs(arg);
                        break;
                    default:
                        throw new ArgumentException("Встретился неизвестный оператор.");
                }
                stack.Push(rst);
            }
            else
            {
                // В противном случае число аргументов оператора равно 2
                double arg2 = stack.Pop();
                double arg1 = stack.Pop();

                double rst;

                switch (token)
                {
                    case Plus:
                        rst = arg1 + arg2;
                        break;
                    case Minus:
                        rst = arg1 - arg2;
                        break;
                    case Multiply:
                        rst = arg1 * arg2;
                        break;
                    case Divide:
                        if (arg2 == 0)
                        {
                            throw new DivideByZeroException("Встретилось деление на ноль.");
                        }
                        rst = arg1 / arg2;
                        break;
                    case Degree:
                        rst = Math.Pow(arg1, arg2);
                        break;

                    default:
                        throw new ArgumentException("Неизвестный оператор.");
                }

                stack.Push(rst);
            }
            return stack;
        }

        /// <summary>
        ///Преобразование и применение тригонометрической функции
        /// </summary>
        /// <param name="func">Тригонометрическая функция</param>
        /// <param name="arg">Ее аргумент</param>
        /// <returns>Результат функции</returns>
        private double ApplyTrigFunction(Func<double, double> func, double arg)
        {
            if (!isRadians)
            {
                arg = arg * Math.PI / 180; // Преобразует значение
            }

            return func(arg);
        }

        /// <summary>
        ///Проверяет, является ли приоритет токена меньше ИЛИ меньшим или равным приоритету p
        /// </summary>
        private bool Priority(string token, string p)
        {
            return IsRightAssociated(token) ?
                GetPriority(token) < GetPriority(p) :
                GetPriority(token) <= GetPriority(p);
        }

        /// <summary>
        /// Проверяет, относится ли оператор, к оперторам с ассоциациями справа
        /// </summary>
        private bool IsRightAssociated(string token)
        {
            return token == Degree;
        }

        /// <summary>
        /// Считывает токены из строки записанной в форме обратной польской записи
        /// </summary>
        /// <param name="expression">Математическое выражение в обратной польской записи</param>
        /// <param name="pos">Текущая позиция лексического анализа</param>
        /// <returns>Токен</returns>
        private string LexicalAnalysisRPN(string expression, ref int pos)
        {
            StringBuilder token = new StringBuilder();

            // Читаем токен от маркера до нового маркера

            token.Append(expression[pos++]);

            while (pos < expression.Length && expression[pos] != NumberMaker[0]
                && expression[pos] != OperatorMarker[0]
                && expression[pos] != FunctionMarker[0]
                && expression[pos]!= VariableMarker[0])
            {
                token.Append(expression[pos++]);
            }

            return token.ToString();
        }

        private int GetPriority(string token)
        {
            switch (token)
            {
                case LeftParent:
                    return 0;
                case Plus:
                case Minus:
                    return 2;
                case UnPlus:
                case UnMinus:
                    return 6;
                case Multiply:
                case Divide:
                    return 4;
                case Degree:
                case Sqrt:
                    return 8;
                case Sin:
                case Cos:
                case Tg:
                case Ctg:
                case Sh:
                case Ch:
                case Th:
                case Ln:
                case Abs:
                    return 10;
                default:
                    throw new ArgumentException("Встретился неизвестный оператор.");
            }
        }

        private int NumberOfArguments(string token)
        {
            switch (token)
            {
                case UnPlus:
                case UnMinus:
                case Sqrt:
                case Tg:
                case Sh:
                case Ch:
                case Th:
                case Ln:
                case Ctg:
                case Sin:
                case Cos:
                case Abs:
                    return 1;
                case Plus:
                case Minus:
                case Multiply:
                case Divide:
                case Degree:
                    return 2;
                default:
                    throw new ArgumentException("Встретился неизвестный оператор.");
            }
        }

        /// <summary>
        /// Создает форматированную строку с помощью данной строки
        /// </summary>
        /// <param name="expression">Неформатированное математическое выражение</param>
        /// <returns>Форматированное матиматическое выражение</returns>
        string FormatString(string expression) 
        {
            if (string.IsNullOrEmpty(expression))// проверяем,есть ли что в строке
            {
                throw new ArgumentNullException("Строка или ссылка является нулевой.");
            }
            StringBuilder formattedString = new StringBuilder();
            int balanceOfParenth = 0; // Кол-во скобок без пары
            // Формируем новую строку и проверяем число скобок
            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];// извлекаем символ из строки с инфиксным выражение
                if (ch == '(')
                {
                    balanceOfParenth++;
                }
                else if (ch == ')')
                {
                    balanceOfParenth--;
                }

                if (Char.IsWhiteSpace(ch))
                {
                    continue;
                }
                else if (Char.IsUpper(ch))
                {
                    formattedString.Append(Char.ToLower(ch));
                }
                else
                {
                    formattedString.Append(ch);
                }
            }
            if (balanceOfParenth != 0)
            {
                throw new FormatException("Количество открытых и закрытых скобок не равно.");
            }
            return formattedString.ToString();
        }

    }
}
