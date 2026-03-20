using System;
using System.IO;
using NUnit.Framework;

namespace Lib.ChatConsole.Tests
{
    public class Tests
    {
        private TextWriter _originalOut;

        [SetUp]
        public void SetUp()
        {
            _originalOut = Console.Out;
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOut);
        }

        private CommandExecutionContext CreateContext()
        {
            ReplOptions options = new ReplOptions();
            FakeTextGenerator generator = new FakeTextGenerator();
            return new CommandExecutionContext(options, generator);
        }

        [Test]
        public void TempCommand_Execute_DoesNotThrow()
        {
            TempCommand command = new TempCommand();
            CommandExecutionContext context = CreateContext();

            Assert.DoesNotThrow(() =>
            {
                command.Execute(new string[] { "/temp", "0.5" }, context);
            });
        }

        [Test]
        public void TempCommand_InvalidValue_DoesNotChangeTemperature()
        {
            TempCommand command = new TempCommand();
            CommandExecutionContext context = CreateContext();
            float before = context.Options.Temperature;

            command.Execute(new string[] { "/temp", "abc" }, context);

            Assert.That(context.Options.Temperature, Is.EqualTo(before));
        }

        [Test]
        public void TopKCommand_ValidValue_ChangesTopK()
        {
            TopKCommand command = new TopKCommand();
            CommandExecutionContext context = CreateContext();

            command.Execute(new string[] { "/topk", "7" }, context);

            Assert.That(context.Options.TopK, Is.EqualTo(7));
        }

        [Test]
        public void TopKCommand_Zero_DoesNotChangeTopK()
        {
            TopKCommand command = new TopKCommand();
            CommandExecutionContext context = CreateContext();
            int before = context.Options.TopK;

            command.Execute(new string[] { "/topk", "0" }, context);

            Assert.That(context.Options.TopK, Is.EqualTo(before));
        }

        [Test]
        public void SeedCommand_ValidInteger_ChangesSeed()
        {
            SeedCommand command = new SeedCommand();
            CommandExecutionContext context = CreateContext();

            command.Execute(new string[] { "/seed", "42" }, context);

            Assert.That(context.Options.Seed, Is.EqualTo(42));
        }

        [Test]
        public void SeedCommand_Null_SetsSeedToNull()
        {
            SeedCommand command = new SeedCommand();
            CommandExecutionContext context = CreateContext();
            context.Options.Seed = 99;

            command.Execute(new string[] { "/seed", "null" }, context);

            Assert.That(context.Options.Seed, Is.Null);
        }

        [Test]
        public void QuitCommand_Execute_StopsRepl()
        {
            QuitCommand command = new QuitCommand();
            CommandExecutionContext context = CreateContext();
            context.Options.IsRunning = true;

            command.Execute(new string[] { "/quit" }, context);

            Assert.That(context.Options.IsRunning, Is.False);
        }

        [Test]
        public void HelpCommand_Execute_PrintsRegisteredCommands()
        {
            CommandRegistry registry = new CommandRegistry();
            registry.Register(new TempCommand());
            registry.Register(new QuitCommand());

            HelpCommand command = new HelpCommand(registry);
            CommandExecutionContext context = CreateContext();

            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            command.Execute(new string[] { "/help" }, context);

            string output = writer.ToString();

            StringAssert.Contains("/temp", output);
            StringAssert.Contains("/quit", output);
        }

        private class FakeTextGenerator : ITextGenerator
        {
            public string Generate(string prompt, int maxTokens, float temperature, int topK, int? seed = null)
            {
                return "fake";
            }
        }
    }
}