﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public class ReturnsAsOrdinaryMethodWithGenericTypeSpecifiedTests : NonVirtualSetupAnalyzerTest
    {
        public override async Task AnalyzerReturnsDiagnostic_WhenSettingValueForNonVirtualMethod()
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        public override async Task AnalyzerReturnsDiagnostic_WhenSettingValueForLiteral(string literal, string type)
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        public override async Task AnalyzerReturnsDiagnostic_WhenSettingValueForStaticMethod()
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForVirtualMethod()
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
            await VerifyCSharpDiagnostic(source);
        }

        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod()
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
            await VerifyCSharpDiagnostic(source);
        }

        public override async Task AnalyzerReturnsNoDiagnostics_WhenDataFlowAnalysisIsRequired()
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
            await VerifyCSharpDiagnostic(source);
        }

        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForDelegate()
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
            await VerifyCSharpDiagnostic(source);
        }

        public override async Task AnalyzerReturnsDiagnostics_WhenSettingValueForSealedOverrideMethod()
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForAbstractMethod()
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

            await VerifyCSharpDiagnostic(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceMethod()
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
            await VerifyCSharpDiagnostic(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceProperty()
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
            await VerifyCSharpDiagnostic(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
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
            await VerifyCSharpDiagnostic(source);
        }

        public override async Task AnalyzerReturnsNoDiagnostic_WhenSettingValueForAbstractProperty()
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

            await VerifyCSharpDiagnostic(source);
        }

        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceIndexer()
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
            await VerifyCSharpDiagnostic(source);

        }

        public override async Task AnalyzerReturnsNoDiagnostic_WhenSettingValueForVirtualProperty()
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

            await VerifyCSharpDiagnostic(source);
        }


        public override async Task AnalyzerReturnsDiagnostic_WhenSettingValueForNonVirtualProperty()
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForVirtualIndexer()
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
            await VerifyCSharpDiagnostic(source);
        }


        public override async Task AnalyzerReturnsDiagnostics_WhenSettingValueForNonVirtualIndexer()
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }
    }
}