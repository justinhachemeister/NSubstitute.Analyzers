﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public class ReturnsAsOrdinaryMethodWithGenericTypeSpecifiedTests : NonVirtualSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar(), 1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            SubstituteExtensions.Returns<{type}>({literal}, {literal});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    $"Member {literal} can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    GetExpectedLocation()
                }
            };

            DiagnosticResultLocation GetExpectedLocation()
            {
                switch (type)
                {
                    case "int":
                        return new DiagnosticResultLocation(9, 47);
                    case "char":
                        return new DiagnosticResultLocation(9, 48);
                    case "bool":
                        return new DiagnosticResultLocation(9, 48);
                    case "string":
                        return new DiagnosticResultLocation(9, 50);
                }

                return default(DiagnosticResultLocation);
            }

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public static int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            SubstituteExtensions.Returns<int>(Foo.Bar(), 1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar(), 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class Foo2 : Foo
    {
        public override int Bar() => 1;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo2>();
            SubstituteExtensions.Returns<int>(substitute.Bar(), 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            var returnValue = substitute.Bar();
            SubstituteExtensions.Returns<int>(returnValue, 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate()
        {
            var source = @"using NSubstitute;
using System;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For<Func<int>>();
            SubstituteExtensions.Returns<int>(substitute(), 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class Foo2 : Foo
    {
        public sealed override int Bar() => 1;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo2>();
           SubstituteExtensions.Returns<int>(substitute.Bar(), 1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(23, 46)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public abstract class Foo
    {
        public abstract int Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar(), 1);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.Returns<int>(substitute.Bar(), 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo<T>
    {
        int Bar<T>();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            SubstituteExtensions.Returns<int>(substitute.Bar<int>(), 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public abstract class Foo
    {
        public abstract int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int this[int i] { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.Returns<int>(substitute[1], 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute[1], 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute[1], 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod()
        {
            var source = @"

namespace NSubstitute
{
    public class Foo
    {
        public int Bar()
        {
            return 1;
        }
    }

    public static class SubstituteExtensions
    {
        public static T Returns<T>(this T returnValue, T returnThis)
        {
            return default(T);
        }
    }

    public class FooTests
    {
        public void Test()
        {
            Foo substitute = null;
            SubstituteExtensions.Returns<int>(substitute.Bar(), 1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; }

        public int FooBar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
            SubstituteExtensions.Returns<int>(substitute.FooBar, 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 47)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo<T>
    {
        public T Bar { get; }

        public int FooBar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
            SubstituteExtensions.Returns<int>(substitute.FooBar, 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 47)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar(int x)
        {
            return 1;
        }

        public int Bar(int x, int y)
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar(1, 2), 1);
            SubstituteExtensions.Returns<int>(substitute.Bar(1), 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 47)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar(int x)
        {
            return 1;
        }

        public int Bar<T>(T x, T y)
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute.Bar<int>(1, 2), 1);
            SubstituteExtensions.Returns<int>(substitute.Bar(1), 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 47)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
        public int this[int x, int y] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute[1,2], 1);
            SubstituteExtensions.Returns<int>(substitute[1], 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo<T>
    {
        public int this[T x] => 0;
        public int this[T x, T y] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            SubstituteExtensions.Returns<int>(substitute[1, 2], 1);
            SubstituteExtensions.Returns<int>(substitute[1], 1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType()
        {
             Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

             var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooBarBar
    {
        public int Bar { get;set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute[1], 1);
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
            SubstituteExtensions.Returns<int>(substitute.FooBar(), 1);

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            SubstituteExtensions.Returns<int>(substituteFooBarBar[1], 1);
            SubstituteExtensions.Returns<int>(substituteFooBarBar.Bar, 1);
            SubstituteExtensions.Returns<int>(substituteFooBarBar.FooBar(), 1);
        }
    }
}";

             var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(35, 47)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(36, 47)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(37, 47)
                    }
                }
            };

             await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo<T>
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooBarBar<T>
    {
        public int Bar { get;set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            SubstituteExtensions.Returns<int>(substitute[1], 1);
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
            SubstituteExtensions.Returns<int>(substitute.FooBar(), 1);

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            SubstituteExtensions.Returns<int>(substituteFooBarBar[1], 1);
            SubstituteExtensions.Returns<int>(substituteFooBarBar.Bar, 1);
            SubstituteExtensions.Returns<int>(substituteFooBarBar.FooBar(), 1);
        }
    }
}";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(35, 47)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(36, 47)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(37, 47)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyOtherNamespace
{
    public class FooBarBar
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }
}

namespace MyNamespace
{
    using MyOtherNamespace;
    public class Foo
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns<int>(substitute[1], 1);
            SubstituteExtensions.Returns<int>(substitute.Bar, 1);
            SubstituteExtensions.Returns<int>(substitute.FooBar(), 1);

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            SubstituteExtensions.Returns<int>(substituteFooBarBar[1], 1);
            SubstituteExtensions.Returns<int>(substituteFooBarBar.Bar, 1);
            SubstituteExtensions.Returns<int>(substituteFooBarBar.FooBar(), 1);
        }
    }
}";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(39, 47)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(40, 47)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 47)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            SubstituteExtensions.Returns<int>(substitute.GetBar(), 1);
            SubstituteExtensions.Returns<int>(substitute.GetFooBar(), 1);
        }
    }

    public static class MyExtensions
    {
        public static IBar Bar { get; set; }

        public static int GetBar(this object @object)
        {
            return Bar.Foo(@object);
        }

        public static int GetFooBar(this object @object)
        {
            return 1;
        }
    }

    public interface IBar
    {
        int Foo(object @obj);
    }
}";
            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member GetFooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(12, 47)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }
    }
}