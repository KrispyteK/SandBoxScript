﻿using Xunit;
using Skrypt;
using System;
using Xunit.Abstractions;

namespace Skrypt.Tests {
    [TestCaseOrderer("Skrypt.Tests.PriorityOrderer", "Skrypt.Tests")]
    public class ModuleTests {

        private readonly SkryptEngine _engine;
        private readonly ITestOutputHelper _output;

        public ModuleTests(ITestOutputHelper output) {
            _output = output;
            _engine = new SkryptEngine();

            _engine
                .SetValue("assert", new Action<bool>(Assert.True))
                .SetValue("equal", new Action<object, object>(Assert.Equal))
                .SetValue("log", new Action<string>(_output.WriteLine))
                ;
        }

        private void RunTest(string source) {
            _engine.Run(source).ReportErrors().CreateGlobals();

            if (_engine.ErrorHandler.HasErrors) {
                _output.WriteLine($"Errors:");

                foreach (var err in _engine.ErrorHandler.Errors) {
                    _output.WriteLine($"({err.Line},{err.CharInLine}) {err.Message}");
                }

                throw new FatalErrorException();
            }
        }

        [Fact, TestPriority(1)]
        public void ShouldParseModule() {
            RunTest(@"
struct TestModule {
    A = 0
    B = ""Hello world!""

    fn Print() {
        log(self.B)
    }
}
            ");
        }

        [Fact, TestPriority(0)]
        public void ShouldImportModule() {
            RunTest(@"
import Math

a = Sin(PI)
            ");
        }
    }
}