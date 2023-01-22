using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx.Infrastructure;
using FluentAssertions;
using heitech.pwXtCli;
using heitech.pwXtCli.Commands;
using heitech.pwXtCli.Options;
using heitech.pwXtCli.Store;
using heitech.pwXtCli.ValueObjects;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace pwXt_cli.Tests;

public sealed class IntegrationTests : IDisposable
{
    private readonly string _pathToDb;
    private readonly IConsole _console;
    private readonly IOptions<PwXtOptions> _iOptions;
    private readonly IClipboardService _clipBoard;
    private readonly WriterMock _writer;

    private const string Key = "movie";
    private const string Password = "swordfish";

    public IntegrationTests()
    {
        var dbDir = Path.Combine(Environment.CurrentDirectory, "test");
        Directory.CreateDirectory(dbDir);
        _pathToDb = Path.Combine(dbDir, "pw-store.db");
        if (File.Exists(_pathToDb))
            File.Delete(_pathToDb);

        _iOptions = Init();
        _console = Substitute.For<IConsole>();
        _writer = new WriterMock(_console);
        _console.Output.Returns(_writer);
        _clipBoard = Substitute.For<IClipboardService>();
    }
    
    private IOptions<PwXtOptions> Init()
    {
        var options = new PwXtOptions
        {
            Passphrase = "also swordfish",
            ConnectionString = $"Filename={_pathToDb}; Connection=Shared;",
            Salt = "and pepper"
        };
        var iOptions = Substitute.For<IOptions<PwXtOptions>>();
        iOptions.Value.Returns(options);
        return iOptions;
    }

    [Fact]
    public async Task Running_Main_Starts_CliFx()
    {
        // Arrange

        // Act
        var act = async () => await Program.Main(new [] { "list"} );

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task List_Lists_All_Keys_In_Store()
    {
        // Arrange
        using var store = new LiteDbStore(_iOptions);
        var pws = new[] { "key1", "key2", "key3" }.ToList();
        foreach (var pw in pws)
            await store.AddPassword(new Password(pw, Password, Array.Empty<byte>()));

        var command = new ListPasswords(store);

        // Act
        await command.ExecuteAsync(_console);

        // Assert
        _writer.Values.Should().BeEquivalentTo(pws);
    }

    [Fact]
    public async Task AddedPassword_Should_Be_Readable()
    {
        // Arrange
        using var store = new LiteDbStore(_iOptions);
        var encrypted = await _iOptions.Value.EncryptAsync(Key, Password);
        await store.AddPassword(encrypted);
        var read = new GetPassword(_iOptions, store, _clipBoard);
        read.Key = Key;

        // Act
        await read.ExecuteAsync(_console);

        // Assert
        await _clipBoard.Received().SetText(Password);
    }

    [Fact]
    public async Task Create_Read_Update_Read_Delete_Test()
    {
        // _________________________________________________________________________
        // phase 1 create db
        // _________________________________________________________________________

        // phase1.arrange
        var createCommand = new CreatePasswordStore(_clipBoard) { Path = _pathToDb };

        // phase1.act
        var act = async () => await createCommand.ExecuteAsync(_console);

        // phase1.assert
        await act.Should().NotThrowAsync<Exception>();
        await _clipBoard.Received().SetText(_pathToDb);

        // _________________________________________________________________________
        // phase 2 create first entry
        // _________________________________________________________________________
        // Arrange
        using var store = new LiteDbStore(_iOptions);
        var mutateCommand = new MutatePassword(_iOptions, store)
        {
            Key = Key,
            Value = Password,
            Operation = "add"
        };

        // Act
        await mutateCommand.ExecuteAsync(_console);

        // Assert
        var result = await store.GetPassword(Key);
        result.Key.Should().Be(Key);

        // _________________________________________________________________________
        // phase 3 update and read
        // _________________________________________________________________________
        var update = new MutatePassword(_iOptions, store)
        {
            Key = Key,
            Value = "better-swordfish",
            Operation = "alter"
        };

        // Act
        await update.ExecuteAsync(_console);

        // Assert
        var result2 = await store.GetPassword(Key);
        result2.Key.Should().Be(Key);
        result2.Value.Should().NotBeEquivalentTo(result.Value);

        // _________________________________________________________________________
        // phase 4 delete and no result on read
        // _________________________________________________________________________
        var del = new MutatePassword(_iOptions, store)
        {
            Key = Key,
            Operation = "del"
        };

        // Act
        await del.ExecuteAsync(_console);

        // Assert
        var empty = await store.GetPassword(Key);
        empty.IsEmpty.Should().BeTrue();
        store.Dispose();
    }


    public void Dispose()
        => Directory.Delete(Path.Combine(Environment.CurrentDirectory, "test"), true);
    
    private sealed class WriterMock : ConsoleWriter
    {
        public List<string> Values { get; } = new();

        public WriterMock(IConsole console) : base(console, new MemoryStream())
        {
        }

        public override void Write(string? value)
        {
            // intentionally left blank
            Values.Add(value!);
        }

        public override Task WriteLineAsync(string? value)
        {
            Values.Add(value!);
            return Task.CompletedTask;
        }
    }   
}