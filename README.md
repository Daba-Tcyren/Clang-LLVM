# Clang-LLVM

https://docs.google.com/document/d/1k0JvGzGMbVJl3UGqt_UtvJmBLf4d8gD8JF5YT0Kh2sY/edit?tab=t.0#heading=h.ngkoxkn940cy


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


