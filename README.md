# Лабораторная работа 7. Анализ и преобразование кода с использованием Clang и LLVM

# Цель работы: 
Познакомиться с инструментарием Clang и LLVM, освоить получение абстрактного синтаксического дерева (AST) и промежуточного представления (LLVM IR) для кода на C/C++, научиться применять базовые оптимизации, строить графы потока управления (CFG), а также анализировать влияние оптимизаций на различные синтаксические конструкции языка.

Сведения об авторе 

Студент: Тарбаев Даба-Цырен 

Группа: АП-327


# Постановка задачи
Необходимо выполнить следующие шаги:
1. Установка среды
   
   Установить Clang, LLVM, opt и Graphviz (например, в Ubuntu 26.04).

2. Работа с AST

   Сгенерировать абстрактное синтаксическое дерево для заданного C/C++‑файла.

3. Генерация LLVM IR

   Получить промежуточное представление кода без оптимизаций (-O0) и с оптимизациями (-O2).

3. Оптимизация IR

   Применить оптимизации с помощью opt и/или флагов Clang, сравнить изменения.

5. Построение CFG

   Построить граф потока управления для одной или нескольких функций.

6. Индивидуальное задание (по варианту)

   Выполнить анализ конкретной синтаксической конструкции в соответствии с вариантом. Сформулировать, как LLVM обрабатывает выбранную конструкцию, какие оптимизации применяются.

7. Выводы
   Ответить на контрольные вопросы


# Установка среды 

Работа выполнялась в среде Ubuntu 22.04. Установлены следующие инструменты:
- clang — компилятор языка C/C++;
- llvm — инструменты анализа и оптимизации кода;
- opt — инструмент для работы с LLVM IR и применения оптимизаций;
- Graphviz — инструмент для визуализации кода.
Команда установки: sudo apt install clang llvm

<img width="815" height="758" alt="изображение" src="https://github.com/user-attachments/assets/95634fbd-1266-4548-8b82-29031cfe3484" />

Команда для установки библиотеки Graphviz: sudo apt install graphviz
<img width="974" height="631" alt="изображение" src="https://github.com/user-attachments/assets/bd0e56de-1c09-48ad-97a3-f06b8bdee41a" />

# Общая часть работы

Для общей части был создан файл main.c:

    #include <stdio.h>
    int square(int x) {
        return x * x;
    }
    int main() {
        int a = 5;
        int b = square(a);
        printf("%d\n", b);
        return 0;
    }
Сохранена в файл main.c.

<img width="453" height="423" alt="изображение" src="https://github.com/user-attachments/assets/4e34d72f-1584-44bd-b4f9-cd41f2780940" />

# Получение AST

Команда для получения AST:

    clang -Xclang -ast-dump -fsyntax-only main.c
AST наглядно показывает структуру программы: функцию square, её параметр x и операцию умножения BinaryOperator с двумя обращениями к переменной x.
<img width="974" height="627" alt="изображение" src="https://github.com/user-attachments/assets/b2312524-12c0-4e89-a5f8-bf2a11edf163" />

# Генерация LLVM IR без оптимизаций

Команда:

    clang -O0 -S -emit-llvm main.c -o main_O0.ll
Фрагмент IR без оптимизаций для функции square:
    
    define dso_local i32 @square(i32 noundef %0) #0 {
      %2 = alloca i32, align 4
      store i32 %0, ptr %2, align 4
      %3 = load i32, ptr %2, align 4
      %4 = load i32, ptr %2, align 4
      %5 = mul nsw i32 %3, %4
      ret i32 %5
    }
Видны все alloca (выделение памяти на стеке), load/store (чтение/запись в память). Код неоптимизирован, много лишних операций.
# Генерация LLVM IR с оптимизацией -O2

Команда:

    clang -O2 -S -emit-llvm main.c -o main_O2.ll
Фрагмент оптимизированного IR:

    define dso_local noundef i32 @main() local_unnamed_addr #1 {
      %1 = tail call i32 (ptr, ...) @printf(ptr noundef nonnull dereferenceable(1) @.str, i32 noundef 25)
      ret i32 0
    }
Анализ:
    Функция square полностью встроена.
    Её вызов square(5) вычислен на этапе компиляции - это 25.
    Все переменные a, b, x удалены из памяти.
    Программа свелась к одному вызову printf с константой 25.

# Построение CFG общей программы
Для построения CFG использовались команды:
  
    clang -O2 -S -emit-llvm main.c -o main_O2.ll
    opt -passes=dot-cfg -disable-output main_O2.ll
    dot -Tpng .main.dot -o cfg_main.png
    xdg-open cfg_main.png
CFG функции main после оптимизации:

<img width="660" height="144" alt="cfg_main" src="https://github.com/user-attachments/assets/75af97b2-f25b-4d9f-a4bd-a2a632f6defc" />

Файл cfg_main.png содержит один базовый блок (т.к. после оптимизаций в main нет ветвлений).

# Индивидуальное задание: Комплексные числа

Исходный код

Для индивидуального задания был создан файл main.cpp:

    #include <iostream>
    #include <complex>
    
    int main() {
        std::complex<double> z1(3.0, 4.0);
        std::complex<double> z2(1.0, 2.0);
        auto z3 = z1 * z2 + z1;
        std::cout << z3.real() << " " << z3.imag() << std::endl;
        return 0;
    }
Скриншот исходного кода:
<img width="621" height="375" alt="изображение" src="https://github.com/user-attachments/assets/e89d52d8-4a64-4901-bafc-871bc6b36b90" />
Смысл программы: складываем два комплексных числа и выводим получившиеся.

# Получение AST
Команда:
    
    ang -Xclang -ast-dump -Xclang -ast-dump-filter=main -fsyntax-only main.cpp
Скриншот AST:

<img width="974" height="638" alt="изображение" src="https://github.com/user-attachments/assets/2ff4a5f0-7528-424e-ba70-982924197b14" />

Фрагмент вывода AST:

    Dumping main:
    FunctionDecl 0x60d09e857090 <main.cpp:4:1, line:10:1> line:4:5 main 'int ()'
    `-CompoundStmt 0x60d09e85f470 <col:12, line:10:1>
      |-DeclStmt 0x60d09e857378 <line:5:3, col:36>
      | `-VarDecl 0x60d09e857238 <col:3, col:35> col:24 used z1 'std::complex<double>' callinit
      |   `-CXXConstructExpr 0x60d09e857340 <col:24, col:35> 'std::complex<double>' 'void (double, double)'
      |     |-FloatingLiteral 0x60d09e8572a0 <col:27> 'double' 3.000000e+00
      |     `-FloatingLiteral 0x60d09e8572c0 <col:32> 'double' 4.000000e+00
      |-DeclStmt 0x60d09e8575d8 <line:6:3, col:36>
      | `-VarDecl 0x60d09e857498 <col:3, col:35> col:24 used z2 'std::complex<double>' callinit
      |   `-CXXConstructExpr 0x60d09e8575a0 <col:24, col:35> 'std::complex<double>' 'void (double, double)'
      |     |-FloatingLiteral 0x60d09e857500 <col:27> 'double' 1.000000e+00
      |     `-FloatingLiteral 0x60d09e857520 <col:32> 'double' 2.000000e+00
      |-DeclStmt 0x60d09e858848 <line:7:3, col:25>
      | `-VarDecl 0x60d09e857608 <col:3, col:23> col:8 used z3 'complex<double>':'std::complex<double>' cinit
      |   `-ExprWithCleanups 0x60d09e858830 <col:13, col:23> 'complex<double>':'std::complex<double>'
      |     `-CXXOperatorCallExpr 0x60d09e858708 <col:13, col:23> 'complex<double>':'std::complex<double>' '+' adl
      |       |-ImplicitCastExpr 0x60d09e8586f0 <col:21> 'complex<double> (*)(const complex<double> &, const complex<double> &)' <FunctionToPointerDecay>
      |       | `-DeclRefExpr 0x60d09e858698 <col:21> 'complex<double> (const complex<double> &, const complex<double> &)' lvalue Function 0x60d09e8584d0 'operator+' 'complex<double> (const complex<double> &, const complex<double> &)'
      |       |-MaterializeTemporaryExpr 0x60d09e858668 <col:13, col:18> 'const complex<double>':'const std::complex<double>' lvalue
      |       | `-ImplicitCastExpr 0x60d09e858650 <col:13, col:18> 'const complex<double>':'const std::complex<double>' <NoOp>
      |       |   `-CXXOperatorCallExpr 0x60d09e857d88 <col:13, col:18> 'complex<double>':'std::complex<double>' '*' adl
      |       |     |-ImplicitCastExpr 0x60d09e857d70 <col:16> 'complex<double> (*)(const complex<double> &, const complex<double> &)' <FunctionToPointerDecay>
      |       |     | `-DeclRefExpr 0x60d09e857cf0 <col:16> 'complex<double> (const complex<double> &, const complex<double> &)' lvalue Function 0x60d09e857b40 'operator*' 'complex<double> (const complex<double> &, const complex<double> &)'
      |       |     |-ImplicitCastExpr 0x60d09e857cc0 <col:13> 'const complex<double>':'const std::complex<double>' lvalue <NoOp>
      |       |     | `-DeclRefExpr 0x60d09e857670 <col:13> 'std::complex<double>' lvalue Var 0x60d09e857238 'z1' 'std::complex<double>'
      |       |     `-ImplicitCastExpr 0x60d09e857cd8 <col:18> 'const complex<double>':'const std::complex<double>' lvalue <NoOp>
      |       |       `-DeclRefExpr 0x60d09e857690 <col:18> 'std::complex<double>' lvalue Var 0x60d09e857498 'z2' 'std::complex<double>'
      |       `-ImplicitCastExpr 0x60d09e858680 <col:23> 'const complex<double>':'const std::complex<double>' lvalue <NoOp>
      |         `-DeclRefExpr 0x60d09e857dc0 <col:23> 'std::complex<double>' lvalue Var 0x60d09e857238 'z1' 'std::complex<double>'
      |-CXXOperatorCallExpr 0x60d09e85f3d8 <line:8:3, col:54> '__ostream_type':'std::basic_ostream<char>' lvalue '<<'
      | |-ImplicitCastExpr 0x60d09e85f3c0 <col:46> '__ostream_type &(*)(__ostream_type &(*)(__ostream_type &))' <FunctionToPointerDecay>
      | | `-DeclRefExpr 0x60d09e85f340 <col:46> '__ostream_type &(__ostream_type &(*)(__ostream_type &))' lvalue CXXMethod 0x60d09e288c38 'operator<<' '__ostream_type &(__ostream_type &(*)(__ostream_type &))'
      | |-CXXOperatorCallExpr 0x60d09e85e038 <col:3, col:44> '__ostream_type':'std::basic_ostream<char>' lvalue '<<'
      | | |-ImplicitCastExpr 0x60d09e85e020 <col:33> '__ostream_type &(*)(double)' <FunctionToPointerDecay>
      | | | `-DeclRefExpr 0x60d09e85e000 <col:33> '__ostream_type &(double)' lvalue CXXMethod 0x60d09e28a518 'operator<<' '__ostream_type &(double)'
      | | |-CXXOperatorCallExpr 0x60d09e85cd78 <col:3, col:29> 'basic_ostream<char, char_traits<char>>':'std::basic_ostream<char>' lvalue '<<' adl
      | | | |-ImplicitCastExpr 0x60d09e85cd60 <col:26> 'basic_ostream<char, char_traits<char>> &(*)(basic_ostream<char, char_traits<char>> &, const char *)' <FunctionToPointerDecay>
      | | | | `-DeclRefExpr 0x60d09e85ccd8 <col:26> 'basic_ostream<char, char_traits<char>> &(basic_ostream<char, char_traits<char>> &, const char *)' lvalue Function 0x60d09e292380 'operator<<' 'basic_ostream<char, char_traits<char>> &(basic_ostream<char, char_traits<char>> &, const char *)'
      | | | |-CXXOperatorCallExpr 0x60d09e85b688 <col:3, col:24> '__ostream_type':'std::basic_ostream<char>' lvalue '<<'
      | | | | |-ImplicitCastExpr 0x60d09e85b670 <col:13> '__ostream_type &(*)(double)' <FunctionToPointerDecay>
      | | | | | `-DeclRefExpr 0x60d09e85b5f0 <col:13> '__ostream_type &(double)' lvalue CXXMethod 0x60d09e28a518 'operator<<' '__ostream_type &(double)'
      | | | | |-DeclRefExpr 0x60d09e8588b0 <col:3, col:8> 'ostream':'std::basic_ostream<char>' lvalue Var 0x60d09e31dce0 'cout' 'ostream':'std::basic_ostream<char>'
      | | | | | `-NestedNameSpecifier Namespace 0x60d09e837988 'std'
      | | | | `-CXXMemberCallExpr 0x60d09e8589a8 <col:16, col:24> 'double'
      | | | |   `-MemberExpr 0x60d09e858960 <col:16, col:19> '<bound member function type>' .real 0x60d09e820db8
      | | | |     `-ImplicitCastExpr 0x60d09e858990 <col:16> 'const std::complex<double>' lvalue <NoOp>
      | | | |       `-DeclRefExpr 0x60d09e8588e0 <col:16> 'complex<double>':'std::complex<double>' lvalue Var 0x60d09e857608 'z3' 'complex<double>':'std::complex<double>'
      | | | `-ImplicitCastExpr 0x60d09e85ccc0 <col:29> 'const char *' <ArrayToPointerDecay>
      | | |   `-StringLiteral 0x60d09e85b8a8 <col:29> 'const char[2]' lvalue " "
      | | `-CXXMemberCallExpr 0x60d09e85ce78 <col:36, col:44> 'double'
      | |   `-MemberExpr 0x60d09e85ce30 <col:36, col:39> '<bound member function type>' .imag 0x60d09e820f20
      | |     `-ImplicitCastExpr 0x60d09e85ce60 <col:36> 'const std::complex<double>' lvalue <NoOp>
      | |       `-DeclRefExpr 0x60d09e85cdb0 <col:36> 'complex<double>':'std::complex<double>' lvalue Var 0x60d09e857608 'z3' 'complex<double>':'std::complex<double>'
      | `-ImplicitCastExpr 0x60d09e85f328 <col:49, col:54> 'basic_ostream<char, char_traits<char>> &(*)(basic_ostream<char, char_traits<char>> &)' <FunctionToPointerDecay>
      |   `-DeclRefExpr 0x60d09e85f2f0 <col:49, col:54> 'basic_ostream<char, char_traits<char>> &(basic_ostream<char, char_traits<char>> &)' lvalue Function 0x60d09e28de10 'endl' 'basic_ostream<char, char_traits<char>> &(basic_ostream<char, char_traits<char>> &)' (FunctionTemplate 0x60d09e26a3d8 'endl')
      |     `-NestedNameSpecifier Namespace 0x60d09e837988 'std'
      `-ReturnStmt 0x60d09e85f460 <line:9:3, col:10>
        `-IntegerLiteral 0x60d09e85f440 <col:10> 'int' 0

# Генерация LLVM IR без оптимизаций
Команда:

    clang -O0 -S -emit-llvm dowhile.c -o dowhile_O0.ll
Скриншот IR без оптимизаций:

<img width="956" height="756" alt="изображение" src="https://github.com/user-attachments/assets/67b49d9f-1777-445a-824f-fdb243198976" />

Фрагмент IR:

    define dso_local noundef i32 @main() #0 {
      %1 = alloca i32, align 4
      %2 = alloca %"class.std::complex", align 8
      %3 = alloca %"class.std::complex", align 8
      %4 = alloca %"class.std::complex", align 8
      %5 = alloca %"class.std::complex", align 8
      store i32 0, ptr %1, align 4
      call void @_ZNSt7complexIdEC2Edd(ptr noundef nonnull align 8 dereferenceable(16) %2, double noundef 3.000000e+00, double noundef 4.000000e+00)
      call void @_ZNSt7complexIdEC2Edd(ptr noundef nonnull align 8 dereferenceable(16) %3, double noundef 1.000000e+00, double noundef 2.000000e+00)
      %6 = call { double, double } @_ZStmlIdESt7complexIT_ERKS2_S4_(ptr noundef nonnull align 8 dereferenceable(16) %2, ptr noundef nonnull align 8 dereferenceable(16) %3)
      %7 = getelementptr inbounds %"class.std::complex", ptr %5, i32 0, i32 0
      %8 = getelementptr inbounds { double, double }, ptr %7, i32 0, i32 0
      %9 = extractvalue { double, double } %6, 0
      store double %9, ptr %8, align 8
      %10 = getelementptr inbounds { double, double }, ptr %7, i32 0, i32 1
      %11 = extractvalue { double, double } %6, 1
      store double %11, ptr %10, align 8
      %12 = call { double, double } @_ZStplIdESt7complexIT_ERKS2_S4_(ptr noundef nonnull align 8 dereferenceable(16) %5, ptr noundef nonnull align 8 dereferenceable(16) %2)
      %13 = getelementptr inbounds %"class.std::complex", ptr %4, i32 0, i32 0
      %14 = getelementptr inbounds { double, double }, ptr %13, i32 0, i32 0
      %15 = extractvalue { double, double } %12, 0
      store double %15, ptr %14, align 8
      %16 = getelementptr inbounds { double, double }, ptr %13, i32 0, i32 1
      %17 = extractvalue { double, double } %12, 1
      store double %17, ptr %16, align 8
      %18 = call noundef double @_ZNKSt7complexIdE4realB5cxx11Ev(ptr noundef nonnull align 8 dereferenceable(16) %4)
      %19 = call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSolsEd(ptr noundef nonnull align 8 dereferenceable(8) @_ZSt4cout, double noundef %18)
      %20 = call noundef nonnull align 8 dereferenceable(8) ptr @_ZStlsISt11char_traitsIcEERSt13basic_ostreamIcT_ES5_PKc(ptr noundef nonnull align 8 dereferenceable(8) %19, ptr noundef @.str)
      %21 = call noundef double @_ZNKSt7complexIdE4imagB5cxx11Ev(ptr noundef nonnull align 8 dereferenceable(16) %4)
      %22 = call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSolsEd(ptr noundef nonnull align 8 dereferenceable(8) %20, double noundef %21)
      %23 = call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSolsEPFRSoS_E(ptr noundef nonnull align 8 dereferenceable(8) %22, ptr noundef @_ZSt4endlIcSt11char_traitsIcEERSt13basic_ostreamIT_T0_ES6_)
      ret i32 0
    }

# Генерация LLVM IR с оптимизацией -O1, -O2, -O3

Команда для оптимизации -О1:

    clang -O1 -S -emit-llvm main.c -o main_O1.ll
Скриншот IR после оптимизации:

<img width="945" height="758" alt="изображение" src="https://github.com/user-attachments/assets/91f70878-0c24-418e-864d-7942a7b238c3" />

Фрагмент оптимизированного IR -O1:

    define dso_local noundef i32 @main() local_unnamed_addr #0 {
      %1 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) @_ZSt4cout, double noundef -2.000000e+00)
      %2 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZSt16__ostream_insertIcSt11char_traitsIcEERSt13basic_ostreamIT_T0_ES6_PKS3_l(ptr noundef nonnull align 8 dereferenceable(8) %1, ptr noundef nonnull @.str, i64 noundef 1)
      %3 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) %1, double noundef 1.400000e+01)
      %4 = load ptr, ptr %3, align 8, !tbaa !5
      %5 = getelementptr i8, ptr %4, i64 -24
      %6 = load i64, ptr %5, align 8
      %7 = getelementptr inbounds i8, ptr %3, i64 %6
      %8 = getelementptr inbounds %"class.std::basic_ios", ptr %7, i64 0, i32 5
      %9 = load ptr, ptr %8, align 8, !tbaa !8
      %10 = icmp eq ptr %9, null
      br i1 %10, label %11, label %12
    
    11:                                               ; preds = %0
      tail call void @_ZSt16__throw_bad_castv() #3
      unreachable
    
    12:                                               ; preds = %0
      %13 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 8
      %14 = load i8, ptr %13, align 8, !tbaa !20
      %15 = icmp eq i8 %14, 0
      br i1 %15, label %19, label %16
    
    16:                                               ; preds = %12
      %17 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 9, i64 10
      %18 = load i8, ptr %17, align 1, !tbaa !23
      br label %24
    
    19:                                               ; preds = %12
      tail call void @_ZNKSt5ctypeIcE13_M_widen_initEv(ptr noundef nonnull align 8 dereferenceable(570) %9)
      %20 = load ptr, ptr %9, align 8, !tbaa !5
      %21 = getelementptr inbounds ptr, ptr %20, i64 6
      %22 = load ptr, ptr %21, align 8
      %23 = tail call noundef signext i8 %22(ptr noundef nonnull align 8 dereferenceable(570) %9, i8 noundef signext 10)
      br label %24
    
    24:                                               ; preds = %16, %19
      %25 = phi i8 [ %18, %16 ], [ %23, %19 ]
      %26 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo3putEc(ptr noundef nonnull align 8 dereferenceable(8) %3, i8 noundef signext %25)
      %27 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo5flushEv(ptr noundef nonnull align 8 dereferenceable(8) %26)
      ret i32 0
    }

Команда для оптимизации -О2:

    clang -O2 -S -emit-llvm main.c -o main_O2.ll
Скриншот IR после оптимизации:

<img width="940" height="754" alt="изображение" src="https://github.com/user-attachments/assets/83c7eaa4-c914-4bf2-8f51-84456aead64b" />

Фрагмент оптимизированного IR -O2:

    define dso_local noundef i32 @main() local_unnamed_addr #0 {
      %1 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) @_ZSt4cout, double noundef -2.000000e+00)
      %2 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZSt16__ostream_insertIcSt11char_traitsIcEERSt13basic_ostreamIT_T0_ES6_PKS3_l(ptr noundef nonnull align 8 dereferenceable(8) %1, ptr noundef nonnull @.str, i64 noundef 1)
      %3 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) %1, double noundef 1.400000e+01)
      %4 = load ptr, ptr %3, align 8, !tbaa !5
      %5 = getelementptr i8, ptr %4, i64 -24
      %6 = load i64, ptr %5, align 8
      %7 = getelementptr inbounds i8, ptr %3, i64 %6
      %8 = getelementptr inbounds %"class.std::basic_ios", ptr %7, i64 0, i32 5
      %9 = load ptr, ptr %8, align 8, !tbaa !8
      %10 = icmp eq ptr %9, null
      br i1 %10, label %11, label %12
    
    11:                                               ; preds = %0
      tail call void @_ZSt16__throw_bad_castv() #3
      unreachable
    
    12:                                               ; preds = %0
      %13 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 8
      %14 = load i8, ptr %13, align 8, !tbaa !20
      %15 = icmp eq i8 %14, 0
      br i1 %15, label %19, label %16
    
    16:                                               ; preds = %12
      %17 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 9, i64 10
      %18 = load i8, ptr %17, align 1, !tbaa !23
      br label %24
    
    19:                                               ; preds = %12
      tail call void @_ZNKSt5ctypeIcE13_M_widen_initEv(ptr noundef nonnull align 8 dereferenceable(570) %9)
      %20 = load ptr, ptr %9, align 8, !tbaa !5
      %21 = getelementptr inbounds ptr, ptr %20, i64 6
      %22 = load ptr, ptr %21, align 8
      %23 = tail call noundef signext i8 %22(ptr noundef nonnull align 8 dereferenceable(570) %9, i8 noundef signext 10)
      br label %24
    
    24:                                               ; preds = %16, %19
      %25 = phi i8 [ %18, %16 ], [ %23, %19 ]
      %26 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo3putEc(ptr noundef nonnull align 8 dereferenceable(8) %3, i8 noundef signext %25)
      %27 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo5flushEv(ptr noundef nonnull align 8 dereferenceable(8) %26)
      ret i32 0
    }
Команда для оптимизации -О3:

    clang -O3 -S -emit-llvm main.c -o main_O3.ll
Скриншот IR после оптимизации:

<img width="946" height="764" alt="изображение" src="https://github.com/user-attachments/assets/f96695ed-9120-413b-b770-e87bb0c8afbe" />

Фрагмент оптимизированного IR -O3:

    define dso_local noundef i32 @main() local_unnamed_addr #0 {
      %1 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) @_ZSt4cout, double noundef -2.000000e+00)
      %2 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZSt16__ostream_insertIcSt11char_traitsIcEERSt13basic_ostreamIT_T0_ES6_PKS3_l(ptr noundef nonnull align 8 dereferenceable(8) %1, ptr noundef nonnull @.str, i64 noundef 1)
      %3 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) %1, double noundef 1.400000e+01)
      %4 = load ptr, ptr %3, align 8, !tbaa !5
      %5 = getelementptr i8, ptr %4, i64 -24
      %6 = load i64, ptr %5, align 8
      %7 = getelementptr inbounds i8, ptr %3, i64 %6
      %8 = getelementptr inbounds %"class.std::basic_ios", ptr %7, i64 0, i32 5
      %9 = load ptr, ptr %8, align 8, !tbaa !8
      %10 = icmp eq ptr %9, null
      br i1 %10, label %11, label %12
    
    11:                                               ; preds = %0
      tail call void @_ZSt16__throw_bad_castv() #3
      unreachable
    
    12:                                               ; preds = %0
      %13 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 8
      %14 = load i8, ptr %13, align 8, !tbaa !20
      %15 = icmp eq i8 %14, 0
      br i1 %15, label %19, label %16
    
    16:                                               ; preds = %12
      %17 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 9, i64 10
      %18 = load i8, ptr %17, align 1, !tbaa !23
      br label %24
    
    19:                                               ; preds = %12
      tail call void @_ZNKSt5ctypeIcE13_M_widen_initEv(ptr noundef nonnull align 8 dereferenceable(570) %9)
      %20 = load ptr, ptr %9, align 8, !tbaa !5
      %21 = getelementptr inbounds ptr, ptr %20, i64 6
      %22 = load ptr, ptr %21, align 8
      %23 = tail call noundef signext i8 %22(ptr noundef nonnull align 8 dereferenceable(570) %9, i8 noundef signext 10)
      br label %24
    
    24:                                               ; preds = %16, %19
      %25 = phi i8 [ %18, %16 ], [ %23, %19 ]
      %26 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo3putEc(ptr noundef nonnull align 8 dereferenceable(8) %3, i8 noundef signext %25)
      %27 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo5flushEv(ptr noundef nonnull align 8 dereferenceable(8) %26)
      ret i32 0
    }

Сравнение:
Уровень -O1
    Выполняется inlining (встраивание) функций _ZStmlIdE... и _ZStplIdE....
    В IR исчезают call, вместо них появляются арифметические инструкции над double.
    Появляются инструкции fmul (умножение чисел с плавающей точкой) и fadd (сложение).

Уровень -O2
    Код становится ещё чище: компилятор переиспользует регистры, удаляет лишние временные переменные.
    Инструкции fmul/fadd сгруппированы для вычисления (3+4i)*(1+2i) + (3+4i).

Уровень -O3
    Добавляется агрессивное встраивание (если что-то осталось).
    Векторизация (SSE/AVX) для операций над double.
    В нашем примере, результат полностью вычислен на этапе компиляции: (-5 + 10i) + (3+4i) = (-2 + 14i).
    В IR остаётся только std::cout << (-2) << " " << (14) << std::endl.

Сравнение -O0 vs -O3:
-O0
Множество call функций
Множество alloca, load, store
Неэффективно

-O3
Нет вызовов, только fmul, fadd
Регистровые переменные (SSA)
Высокая производительность, расчёт констант


# Исследование встраивания операторов * и +

команда:

    grep -E "call.*_ZSt(ml|pl)" complex_O3.ll
Скриншот:

<img width="467" height="552" alt="Рисунок1_21" src="https://github.com/user-attachments/assets/19edd608-608f-42a6-a6b2-9ffb618518ff" />

фрагмент:

       ; ModuleID = 'main.cpp'
        source_filename = "main.cpp"
        target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-i128:128-f80:128-n8:16:32:64-S128"
        target triple = "x86_64-pc-linux-gnu"
        
        module asm ".globl _ZSt21ios_base_library_initv"
        
        %"class.std::basic_ostream" = type { ptr, %"class.std::basic_ios" }
        %"class.std::basic_ios" = type { %"class.std::ios_base", ptr, i8, i8, ptr, ptr, ptr, ptr }
        %"class.std::ios_base" = type { ptr, i64, i64, i32, i32, i32, ptr, %"struct.std::ios_base::_Words", [8 x %"struct.std::ios_base::_Words"], i32, ptr, %"class.std::locale" }
        %"struct.std::ios_base::_Words" = type { ptr, i64 }
        %"class.std::locale" = type { ptr }
        %"class.std::ctype" = type <{ %"class.std::locale::facet.base", [4 x i8], ptr, i8, [7 x i8], ptr, ptr, ptr, i8, [256 x i8], [256 x i8], i8, [6 x i8] }>
        %"class.std::locale::facet.base" = type <{ ptr, i32 }>
        
        @_ZSt4cout = external global %"class.std::basic_ostream", align 8
        @.str = private unnamed_addr constant [2 x i8] c" \00", align 1
        
        ; Function Attrs: mustprogress norecurse uwtable
        define dso_local noundef i32 @main() local_unnamed_addr #0 {
          %1 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) @_ZSt4cout, double noundef -2.000000e+00)
          %2 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZSt16__ostream_insertIcSt11char_traitsIcEERSt13basic_ostreamIT_T0_ES6_PKS3_l(ptr noundef nonnull align 8 dereferenceable(8) %1, ptr noundef nonnull @.str, i64 noundef 1)
          %3 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8) %1, double noundef 1.400000e+01)
          %4 = load ptr, ptr %3, align 8, !tbaa !5
          %5 = getelementptr i8, ptr %4, i64 -24
          %6 = load i64, ptr %5, align 8
          %7 = getelementptr inbounds i8, ptr %3, i64 %6
          %8 = getelementptr inbounds %"class.std::basic_ios", ptr %7, i64 0, i32 5
          %9 = load ptr, ptr %8, align 8, !tbaa !8
          %10 = icmp eq ptr %9, null
          br i1 %10, label %11, label %12
        
        11:                                               ; preds = %0
          tail call void @_ZSt16__throw_bad_castv() #3
          unreachable
        
        12:                                               ; preds = %0
          %13 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 8
          %14 = load i8, ptr %13, align 8, !tbaa !20
          %15 = icmp eq i8 %14, 0
          br i1 %15, label %19, label %16
        
        16:                                               ; preds = %12
          %17 = getelementptr inbounds %"class.std::ctype", ptr %9, i64 0, i32 9, i64 10
          %18 = load i8, ptr %17, align 1, !tbaa !23
          br label %24
        
        19:                                               ; preds = %12
          tail call void @_ZNKSt5ctypeIcE13_M_widen_initEv(ptr noundef nonnull align 8 dereferenceable(570) %9)
          %20 = load ptr, ptr %9, align 8, !tbaa !5
          %21 = getelementptr inbounds ptr, ptr %20, i64 6
          %22 = load ptr, ptr %21, align 8
          %23 = tail call noundef signext i8 %22(ptr noundef nonnull align 8 dereferenceable(570) %9, i8 noundef signext 10)
          br label %24
        
        24:                                               ; preds = %16, %19
          %25 = phi i8 [ %18, %16 ], [ %23, %19 ]
          %26 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo3putEc(ptr noundef nonnull align 8 dereferenceable(8) %3, i8 noundef signext %25)
          %27 = tail call noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo5flushEv(ptr noundef nonnull align 8 dereferenceable(8) %26)
          ret i32 0
        }
        
        declare noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo9_M_insertIdEERSoT_(ptr noundef nonnull align 8 dereferenceable(8), double noundef) local_unnamed_addr #1
        
        declare noundef nonnull align 8 dereferenceable(8) ptr @_ZSt16__ostream_insertIcSt11char_traitsIcEERSt13basic_ostreamIT_T0_ES6_PKS3_l(ptr noundef nonnull align 8 dereferenceable(8), ptr noundef, i64 noundef) local_unnamed_addr #1
        
        declare noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo3putEc(ptr noundef nonnull align 8 dereferenceable(8), i8 noundef signext) local_unnamed_addr #1
        
        declare noundef nonnull align 8 dereferenceable(8) ptr @_ZNSo5flushEv(ptr noundef nonnull align 8 dereferenceable(8)) local_unnamed_addr #1
        
        ; Function Attrs: noreturn
        declare void @_ZSt16__throw_bad_castv() local_unnamed_addr #2
        
        declare void @_ZNKSt5ctypeIcE13_M_widen_initEv(ptr noundef nonnull align 8 dereferenceable(570)) local_unnamed_addr #1
        
        attributes #0 = { mustprogress norecurse uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
        attributes #1 = { "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
        attributes #2 = { noreturn "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
        attributes #3 = { noreturn }
        
        !llvm.module.flags = !{!0, !1, !2, !3}
        !llvm.ident = !{!4}
        
        !0 = !{i32 1, !"wchar_size", i32 4}
        !1 = !{i32 8, !"PIC Level", i32 2}
        !2 = !{i32 7, !"PIE Level", i32 2}
        !3 = !{i32 7, !"uwtable", i32 2}
        !4 = !{!"Ubuntu clang version 18.1.3 (1ubuntu1)"}
        !5 = !{!6, !6, i64 0}
        !6 = !{!"vtable pointer", !7, i64 0}
        !7 = !{!"Simple C++ TBAA"}
        !8 = !{!9, !15, i64 240}
        !9 = !{!"_ZTSSt9basic_iosIcSt11char_traitsIcEE", !10, i64 0, !15, i64 216, !12, i64 224, !19, i64 225, !15, i64 232, !15, i64 240, !15, i64 248, !15, i64 256}
        !10 = !{!"_ZTSSt8ios_base", !11, i64 8, !11, i64 16, !13, i64 24, !14, i64 28, !14, i64 32, !15, i64 40, !16, i64 48, !12, i64 64, !17, i64 192, !15, i64 200, !18, i64 208}
        !11 = !{!"long", !12, i64 0}
        !12 = !{!"omnipotent char", !7, i64 0}
        !13 = !{!"_ZTSSt13_Ios_Fmtflags", !12, i64 0}
        !14 = !{!"_ZTSSt12_Ios_Iostate", !12, i64 0}
        !15 = !{!"any pointer", !12, i64 0}
        !16 = !{!"_ZTSNSt8ios_base6_WordsE", !15, i64 0, !11, i64 8}
        !17 = !{!"int", !12, i64 0}
        !18 = !{!"_ZTSSt6locale", !15, i64 0}
        !19 = !{!"bool", !12, i64 0}
        !20 = !{!21, !12, i64 56}
        !21 = !{!"_ZTSSt5ctypeIcE", !22, i64 0, !15, i64 16, !19, i64 24, !15, i64 32, !15, i64 40, !15, i64 48, !12, i64 56, !12, i64 57, !12, i64 313, !12, i64 569}
        !22 = !{!"_ZTSNSt6locale5facetE", !17, i64 8}
        !23 = !{!12, !12, i64 0}


Вывод:

Без оптимизаций (-O0): Операторы не встраиваются, вызываются как обычные функции.

С оптимизациями (-O1 и выше): Компилятор встраивает эти операторы, так как они короткие и определены в заголовочном файле (inline).

Причина: Это стандартная практика для шаблонных и inline функций в C++. LLVM на стадии -O1 применяет проход -inline.

# Построение CFG 

До оптимизации (-O0):
        
    clang++ -O0 -S -emit-llvm main.cpp -o main_O0.ll
    opt passes=dot-cfg -disable-output main_O0.ll
    dot -Tpng .main.dot -o cfg_main_O0.png

граф потока управления:

<img width="913" height="1024" alt="cfg_main" src="https://github.com/user-attachments/assets/1cc4074c-f510-4e25-9f80-2e2e41942e06" />

После оптимизации (-O2):

    clang++ -O2 -S -emit-llvm main.cpp -o main_O2.ll
    opt passes=dot-cfg -disable-output main_O2.ll
    dot -Tpng .main.dot -o cfg_main_O2.png

граф потока управления:

<img width="1761" height="1157" alt="cfg_mainO2" src="https://github.com/user-attachments/assets/a48388d2-dee6-40cc-ae46-dccb6bac5920" />

Сравнение CFG:
    До оптимизации (-O0): CFG функции main содержит много узлов (для каждого вызова оператора, для создания временных объектов). Есть блоки, отвечающие за вызов деструкторов временных объектов.

   После оптимизации (-O2): CFG функции main содержит 1-3 базовых блока (прямолинейный код). Узлы для вызовов операторов и деструкторов исчезли.

# Выводы по индивидуальному заданию
   LLVM обрабатывает пользовательские операторы (например, * для std::complex) как обычные функции.
   На уровне без оптимизаций (-O0) каждый оператор — это call, что удобно для отладки.
   На уровнях -O1, -O2, -O3:
   Происходит inlining этих функций.
   Операторы над комплексными числами раскладываются в элементарные операции над double (fmul, fadd, ...).
   После inlining становятся возможны свертка констант, удаление мёртвого кода и другие оптимизации.
   В итоге комплексная арифметика в оптимизированном коде так же эффективна, как и ручное написание операций над двумя double.

 # Дополнительное задание

Выражение из 5 лабараторной работы: Complex c1 = new Complex(1.2, 6.0); 
AST конструкции

<img width="644" height="245" alt="изображение" src="https://github.com/user-attachments/assets/b3f00180-e1f3-4d47-8d58-e165fbc392c8" />

Упрощенное IR / TAC конструкции

<img width="366" height="214" alt="изображение" src="https://github.com/user-attachments/assets/071bfcdb-eed0-48ba-a885-fed526b2cec1" />


# Локальная оптимизация 1: свёртка констант

Первая реализованная оптимизация — свёртка констант.

Смысл оптимизации: если операция выполняется над двумя известными числовыми константами, результат можно вычислить сразу и заменить операцию обычным присваиванием.

Пример: Если встречено `Complex c1 = new Complex(1.2 + 55 / 8 * 5, 6.0);` , то IR известны числовые значение из-за чего можем сразу вычислить и записать переменную как `Complex c1 = new Complex(35.575, 6);`

<img width="980" height="849" alt="изображение" src="https://github.com/user-attachments/assets/2dd8fef4-51ca-4251-acc5-ec8407910674" />


<img width="716" height="642" alt="изображение" src="https://github.com/user-attachments/assets/fc6bb130-939e-4312-8aa8-a9144453bf1a" />


# Локальная оптимизация 2: Упрощение комплексного числа

Смысл оптимизации: если Мнимая часть равна нулю, то есть можно заменить на вещественное.

Пример: Если встречено `Complex c1 = new Complex(1.2, 423.32*0);` , то IR должен вычислит благодаря первой локальной оптимизации, что мнимое число нулевое и можем переменную записать как `double c1 = 1.2`

<img width="980" height="852" alt="изображение" src="https://github.com/user-attachments/assets/d8aa4ce2-6c63-45f9-b5f6-615bbdc0db70" />

<img width="710" height="632" alt="изображение" src="https://github.com/user-attachments/assets/6c7f1ce1-3289-453a-997b-36138de7f9ab" />


# Выводы

1. Что такое Clang, и какова его роль в процессе компиляции программ?

Clang — это фронтенд компилятора для языков C, C++ и Objective-C. Он выполняет синтаксический и семантический анализ исходного кода, строит абстрактное синтаксическое дерево (AST) и передаёт промежуточное представление кода LLVM IR для дальнейшей оптимизации и генерации машинного кода. Clang обеспечивает точное диагностирование ошибок и предупреждений на этапе компиляции.

2. Что представляет собой LLVM и как он используется в современных компиляторах?

LLVM — это инфраструктура компилятора, включающая промежуточное представление (IR), оптимизации, транслятор и бэкэнд для генерации кода под различные архитектуры. Используется для унификации оптимизаций, упрощения поддержки разных платформ и обеспечения переносимости кода. LLVM позволяет применить широкий спектр локальных, глобальных и межпроцедурных оптимизаций.

3. Чем отличается AST от LLVM IR?

    AST отражает синтаксис и семантику исходного кода, представляя его в виде дерева с узлами для операторов, выражений и функций.
    LLVM IR — это линейное промежуточное представление, независимое от исходного синтаксиса, предназначенное для анализа и оптимизации кода, а также для генерации машинных инструкций.


4. Для чего необходимо IR в процессе компиляции?

IR используется для унифицированного анализа и оптимизации программы, независимого от конкретного исходного языка и целевой архитектуры. Оно облегчает применение локальных и глобальных трансформаций, улучшает переносимость и эффективность генерируемого кода.

5. Что делает инструкция alloca в LLVM IR, и зачем она используется в функциях?

Инструкция alloca выделяет область памяти на стеке для локальных переменных функции. Она обеспечивает доступ к переменным во время исполнения и позволяет хранить промежуточные значения и результаты вычислений.

6. Зачем нужна оптимизация кода в компиляторе?

Цель оптимизации — улучшение производительности, уменьшение объёма кода, снижение потребления ресурсов, при сохранении исходной семантики. Основные задачи: сокращение числа инструкций, устранение мёртвого кода, повышение скорости выполнения, улучшение кэширования и ветвления.

7. Что такое SSA-форма и почему она важна?

SSA-форма — это представление, в котором каждая переменная получает значение только один раз. Такая форма упрощает анализ зависимостей, распространение констант, удаление мёртвого кода и другие оптимизации.

8. Что такое CFG и как он помогает анализировать программу?

CFG — граф потока управления. Его вершины — базовые блоки, а рёбра показывают возможные переходы управления. CFG помогает анализировать ветвления, циклы, достижимость блоков и структуру выполнения программы.

9. Как представлены арифметические операции в LLVM IR?

Арифметические операции представлены отдельными инструкциями, например add, sub, mul, sdiv. Результат операции записывается в виртуальный регистр, например %7 = add nsw i32 %6, %5.

10. Почему функции в LLVM IR обычно являются отдельными единицами анализа и оптимизации?

Функция имеет собственные аргументы, локальные переменные, базовые блоки и CFG. Поэтому многие оптимизации выполняются на уровне отдельной функции. Межпроцедурные оптимизации тоже возможны, но они требуют анализа взаимодействия между функциями.

11. Что происходит с функцией в LLVM IR, если она вызывается один раз и очень короткая?

Такая функция может быть встроена в место вызова. После встраивания дальнейшие оптимизации могут удалить лишние инструкции и вычислить результат на этапе компиляции, если все значения известны.

12. Какие преимущества дают IR и CFG по сравнению с анализом исходного текста на C?

IR и CFG имеют более строгую и удобную структуру для автоматического анализа. В них явно представлены операции, переходы, зависимости данных и базовые блоки. Поэтому оптимизации проще и надёжнее выполнять на IR/CFG, чем напрямую на исходном тексте.
