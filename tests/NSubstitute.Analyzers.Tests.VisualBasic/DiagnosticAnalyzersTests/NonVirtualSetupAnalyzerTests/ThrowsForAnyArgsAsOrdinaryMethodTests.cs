﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupAnalyzerTests
{
    public class ThrowsForAnyArgsAsOrdinaryMethodTests : NonVirtualSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class FooTests
        Public Sub Test()
            ExceptionExtensions.ThrowsForAnyArgs({literal}, New Exception())
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    $"Member {literal} can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(9, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Shared Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            ExceptionExtensions.ThrowsForAnyArgs(Foo.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnValue = substitute.Bar()
            ExceptionExtensions.ThrowsForAnyArgs(returnValue, New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            ExceptionExtensions.ThrowsForAnyArgs(substitute(), New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public NotOverridable Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(26, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride Function Bar() As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Interface IFoo

        Function Bar() As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Interface IFoo

       Property Bar As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Interface IFoo(Of T)

        Function Bar(Of T)() As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo(Of Integer))()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(Of Integer), New Exception())
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Interface IFoo

        Default Property Item(ByVal i As Integer) As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of IFoo)
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
        End Sub
    End Class
End Namespace";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable ReadOnly Property Bar As Integer
            Get
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public ReadOnly Property Bar As Integer
            Get
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
        End Sub
    End Class
End Namespace";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Default Property Item(ByVal x As Integer) As Integer
            Set
                Throw New NotImplementedException
            End Set
            Get
                Throw New NotImplementedException
            End Get

        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
        End Sub
    End Class
End Namespace";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod()
        {
            var source = @"Imports System.Runtime.CompilerServices
Imports System
Namespace NSubstitute
    Public Class Foo
        Public Function Bar() As Integer
            Return 1
        End Function
    End Class

    Module ExceptionExtensions
        <Extension>
        Function ThrowsForAnyArgs(Of T)(ByVal returnValue As T, ex As Exception) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Integer
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.FooBar, New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 50)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public ReadOnly Property Bar As T
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.FooBar, New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 50)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function

        Public Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(1, 2), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(1), New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 50)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function

        Public Function Bar(Of T)(ByVal x As T, ByVal y As T) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(Of Integer)(1, 2), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar(1), New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 50)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1 ,2), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Default Public ReadOnly Property Item(ByVal x As T) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As T, ByVal y As T) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1 ,2), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 50)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType()
        {
             Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

             var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooBarBar
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.FooBar(), New Exception())
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar(1), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar.FooBar(), New Exception())
        End Sub
    End Class
End Namespace
";

             var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 50)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(42, 50)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(43, 50)
                    }
                }
            };

             await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooBarBar(Of T)
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.FooBar(), New Exception())
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar(Of Integer))()
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar(1), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar.FooBar(), New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 50)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(42, 50)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(43, 50)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports MyOtherNamespace

Namespace MyOtherNamespace
    Public Class FooBarBar
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class
End Namespace

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute(1), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.FooBar(), New Exception())
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar(1), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar.Bar, New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substituteFooBarBar.FooBar(), New Exception())
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(44, 50)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(45, 50)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(46, 50)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(MyNamespace.IFoo)~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Bar = NSubstitute.Substitute.[For](Of IBar)()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            ExceptionExtensions.ThrowsForAnyArgs(substitute.GetBar(), New Exception())
            ExceptionExtensions.ThrowsForAnyArgs(substitute.GetFooBar(), New Exception())
        End Sub
    End Class

    Module MyExtensions
        Public Property Bar As IBar

        <Extension()>
        Function GetBar(ByVal foo As IFoo) As Integer
            Return Bar.Foo()
            Return 1
        End Function

        <Extension()>
        Function GetFooBar(ByVal foo As IFoo) As Integer
            Return 1
        End Function
    End Module

    Interface IBar
        Function Foo() As Integer
    End Interface

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member GetFooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(12, 50)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }
    }
}