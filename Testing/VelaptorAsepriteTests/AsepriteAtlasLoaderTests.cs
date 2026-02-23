// <copyright file="AsepriteAtlasLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests;

using System.Collections.Concurrent;
using System.Drawing;
using System.IO.Abstractions;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using NSubstitute;
using Velaptor;
using VelaptorAseprite;
using Velaptor.Content;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.ReactableData;
using Velaptor.Services;
using VelaptorAseprite.Data;
using AsepriteIJsonService = VelaptorAseprite.Services.IJsonService;

// ReSharper disable once GrammarMistakeInComment
/*
 * The number of arguments passed to NSubstitute.Substitute.For<Velaptor.Content.Factories.ITextureFactory> do not match the number of
 * constructor arguments for Velaptor.Content.Factories.ITextureFactory. Check the constructors for Velaptor.Content.Factories.ITextureFactory
 * and make sure you have passed the required number of arguments.
 */
#pragma warning disable NS2002
// Argument matcher used with a non-virtual member of a class.
#pragma warning disable NS1004

/// <summary>
/// Tests the <see cref="AsepriteAtlasLoader"/>.
/// </summary>
public class AsepriteAtlasLoaderTests
{
    private const string AtlasTextureExtension = ".png";
    private const string AtlasDataExtension = ".json";
    private const string AtlasTextureContentName = "test-atlas";
    private const string AtlasTextureFileName = $"{AtlasTextureContentName}{AtlasTextureExtension}";
    private const string AtlasDataFileName = $"{AtlasTextureContentName}{AtlasDataExtension}";
    private const uint TextureId = 123u;

    private readonly ITextureFactory mockTextureFactory;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IContentPathResolver mockAtlasDataPathResolver;
    private readonly IImageService mockImageService;
    private readonly AsepriteIJsonService mockJsonService;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IPath mockPath;
    private readonly IPushReactable<DisposeTextureData> mockDisposeReactable;
    private readonly IDisposable mockShutdownUnsubscriber;
    private IReceiveSubscription? mockShutdownSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsepriteAtlasLoaderTests"/> class.
    /// </summary>
    public AsepriteAtlasLoaderTests()
    {
        this.mockTextureFactory = Substitute.For<ITextureFactory>();

        this.mockShutdownUnsubscriber = Substitute.For<IDisposable>();
        var mockShutdownReactable = Substitute.For<IPushReactable>();
        mockShutdownReactable.Subscribe(Arg.Any<IReceiveSubscription>()).Returns(this.mockShutdownUnsubscriber);
        mockShutdownReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription>();

                if (subscription.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.mockShutdownSubscription = subscription;
                }
            });

        this.mockDisposeReactable = Substitute.For<IPushReactable<DisposeTextureData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(this.mockDisposeReactable);
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockShutdownReactable);

        this.mockAtlasDataPathResolver = Substitute.For<IContentPathResolver>();
        this.mockImageService = Substitute.For<IImageService>();
        this.mockJsonService = Substitute.For<AsepriteIJsonService>();
        this.mockDirectory = Substitute.For<IDirectory>();

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);

        this.mockPath = Substitute.For<IPath>();
    }

    #region Ctor Tests
    [Fact]
    public void Ctor_WithNullTextureFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            null,
            this.mockReactableFactory,
            this.mockAtlasDataPathResolver,
            this.mockImageService,
            this.mockJsonService,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureFactory')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            this.mockTextureFactory,
            null,
            this.mockAtlasDataPathResolver,
            this.mockImageService,
            this.mockJsonService,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullAtlasDataPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            null,
            this.mockImageService,
            this.mockJsonService,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'atlasDataPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            this.mockAtlasDataPathResolver,
            null,
            this.mockJsonService,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullJsonServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            this.mockAtlasDataPathResolver,
            this.mockImageService,
            null,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'jsonService')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            this.mockAtlasDataPathResolver,
            this.mockImageService,
            this.mockJsonService,
            null,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            this.mockAtlasDataPathResolver,
            this.mockImageService,
            this.mockJsonService,
            this.mockDirectory,
            null,
            this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new AsepriteAtlasLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            this.mockAtlasDataPathResolver,
            this.mockImageService,
            this.mockJsonService,
            this.mockDirectory,
            this.mockFile,
            null);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Load_WithNullOrEmptyParameter_ThrowsException(string? atlasPathOrName)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(atlasPathOrName);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Load_WithRootedAtlasPathOrName_LoadsAtlasData()
    {
        // Arrange
        const string dirName = "my-game";
        var fullDirPath = Path.Combine("C:", dirName);
        var rootedAtlasTexturePath = Path.Combine(fullDirPath, AtlasTextureFileName);
        var rootedAtlasDataPath = Path.Combine(fullDirPath, AtlasDataFileName);
        var texture = Substitute.For<ITexture>();
        var frames = new Dictionary<int, AnimationFrame>
        {
            {
                0,
                new AnimationFrame
                {
                    Bounds = new Rectangle(11, 11, 11, 11),
                    SpriteSourceSize = new Rectangle(11, 11, 11, 11),
                    SourceSize = new Size(11, 11),
                    Duration = 11,
                    Rotated = true,
                    Trimmed = true,
                }
            },
        };
        var atlasData = new AsepriteAtlas
        {
            Texture = texture,
            Name = "test-name",
            FilePath = rootedAtlasDataPath,
            Frames = frames,
            Meta = new MetaData
            {
                App = "test-app",
                Format = "test-format",
                Size = new Size(11, 11),
                ImageFileName = "test-img-filename",
                Scale = "test-scale",
                Version = "test-version",
            },
        };
        var imgData = new ImageData(new[,]
        {
            { Color.White },
            { Color.Coral },
        });

        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        this.mockPath.GetDirectoryName(Arg.Any<string>()).Returns(fullDirPath);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(AtlasTextureContentName);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(AtlasTextureExtension);
        this.mockPath.Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasDataExtension}").Returns(rootedAtlasDataPath);
        this.mockPath.Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasTextureExtension}").Returns(rootedAtlasTexturePath);
        this.mockFile.ReadAllText(rootedAtlasDataPath).Returns("json-data");
        this.mockJsonService.Deserialize<AsepriteAtlas>(Arg.Any<string>()).Returns(atlasData);
        this.mockImageService.Load(Arg.Any<string>()).Returns(imgData);
        this.mockTextureFactory.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>()).Returns(texture);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(rootedAtlasTexturePath);

        // Assert
        this.mockPath.Received(1).GetDirectoryName(rootedAtlasTexturePath);
        this.mockPath.Received(1).GetFileNameWithoutExtension(rootedAtlasTexturePath);
        this.mockPath.Received(1).GetExtension(rootedAtlasTexturePath);
        this.mockPath.Received(1).Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasDataExtension}");
        this.mockPath.Received(1).Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasTextureExtension}");
        this.mockFile.Received(1).ReadAllText(rootedAtlasDataPath);
        this.mockJsonService.Received(1).Deserialize<AsepriteAtlas>(Arg.Any<string>());
        this.mockImageService.Received(1).Load(rootedAtlasTexturePath);
        this.mockTextureFactory.Received(1).Create(AtlasTextureContentName, rootedAtlasTexturePath, imgData);
        sut.TotalCachedItems.Should().Be(1);
        atlasData.Name.Should().Be(AtlasTextureContentName);
        atlasData.FilePath.Should().Be(rootedAtlasTexturePath);
        atlasData.Texture.Should().BeSameAs(texture);
        actual.Should().BeEquivalentTo(atlasData);
    }

    [Fact]
    public void Load_WithNonRootedAtlasPathOrName_LoadsAtlasData()
    {
        // Arrange
        const string dirName = "my-game";
        var fullDirPath = Path.Combine("C:", dirName);
        var rootedAtlasTexturePath = Path.Combine(fullDirPath, AtlasTextureFileName);
        var rootedAtlasDataPath = Path.Combine(fullDirPath, AtlasDataFileName);
        var mockTexture = Substitute.For<ITexture>();
        var frames = new Dictionary<int, AnimationFrame>
        {
            {
                0,
                new AnimationFrame
                {
                    Bounds = new Rectangle(11, 11, 11, 11),
                    SpriteSourceSize = new Rectangle(11, 11, 11, 11),
                    SourceSize = new Size(11, 11),
                    Duration = 11,
                    Rotated = true,
                    Trimmed = true,
                }
            },
        };
        var atlasData = new AsepriteAtlas
        {
            Texture = mockTexture,
            Name = "test-name",
            FilePath = rootedAtlasDataPath,
            Frames = frames,
            Meta = new MetaData
            {
                App = "test-app",
                Format = "test-format",
                Size = new Size(11, 11),
                ImageFileName = "test-img-filename",
                Scale = "test-scale",
                Version = "test-version",
            },
        };
        var imgData = new ImageData(new[,]
        {
            { Color.White },
            { Color.Coral },
        });

        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(false);
        this.mockAtlasDataPathResolver.ResolveDirPath().Returns(fullDirPath);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(AtlasTextureContentName);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(AtlasTextureExtension);
        this.mockPath.Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasDataExtension}").Returns(rootedAtlasDataPath);
        this.mockPath.Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasTextureExtension}").Returns(rootedAtlasTexturePath);
        this.mockFile.ReadAllText(rootedAtlasDataPath).Returns("json-data");
        this.mockJsonService.Deserialize<AsepriteAtlas>(Arg.Any<string>()).Returns(atlasData);
        this.mockImageService.Load(Arg.Any<string>()).Returns(imgData);
        this.mockTextureFactory.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>()).Returns(mockTexture);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(AtlasTextureContentName);

        // Assert
        this.mockPath.Received(1).GetFileNameWithoutExtension(AtlasTextureContentName);
        this.mockPath.Received(1).Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasDataExtension}");
        this.mockPath.Received(1).Combine(fullDirPath, $"{AtlasTextureContentName}{AtlasTextureExtension}");
        this.mockFile.Received(1).ReadAllText(rootedAtlasDataPath);
        this.mockJsonService.Received(1).Deserialize<AsepriteAtlas>(Arg.Any<string>());
        this.mockImageService.Received(1).Load(rootedAtlasTexturePath);
        this.mockTextureFactory.Received(1).Create(AtlasTextureContentName, rootedAtlasTexturePath, imgData);
        sut.TotalCachedItems.Should().Be(1);
        atlasData.Name.Should().Be(AtlasTextureContentName);
        atlasData.FilePath.Should().Be(rootedAtlasTexturePath);
        atlasData.Texture.Should().BeSameAs(mockTexture);
        actual.Should().BeEquivalentTo(atlasData);
    }

    [Fact]
    public void Load_WithInvalidRootedAtlasPathExtension_ThrowsException()
    {
        // Arrange
        const string dirName = "my-game";
        var fullDirPath = Path.Combine("C:", dirName);
        var rootedAtlasTexturePath = Path.Combine(fullDirPath, $"{AtlasTextureContentName}.txt");
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(".txt");

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(rootedAtlasTexturePath);

        // Assert
        act.Should().Throw<LoadAtlasException>()
            .WithMessage("When loading atlas data with fully qualified paths, the files must be a '.png' or '.json' extension.");
    }

    [Fact]
    public void Load_WhenAtlasDataFilePathDoesNotExist_ThrowsException()
    {
        // Arrange
        const string dirName = "my-game";
        var contentDirPath = Path.Combine("C:", dirName);
        var atlasDataFilePath = Path.Combine(contentDirPath, AtlasDataFileName);
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);
        this.mockAtlasDataPathResolver.ResolveDirPath().Returns(contentDirPath);
        this.mockPath.Combine(Arg.Any<string>(), Arg.Any<string>()).Returns(atlasDataFilePath);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(AtlasTextureContentName);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The atlas data directory '{contentDirPath}' does not contain the required '{atlasDataFilePath}' atlas data file.");
    }

    [Fact]
    public void Load_WhenAtlasTextureFilePathDoesNotExist_ThrowsException()
    {
        // Arrange
        const string dirName = "my-game";
        var contentDirPath = Path.Combine("C:", dirName);
        var atlasDataFilePath = Path.Combine(contentDirPath, AtlasDataFileName);
        var atlasTextureFilePath = Path.Combine(contentDirPath, AtlasTextureFileName);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(AtlasTextureContentName);
        this.mockFile.Exists(atlasDataFilePath).Returns(true);
        this.mockFile.Exists(atlasTextureFilePath).Returns(false);
        this.mockAtlasDataPathResolver.ResolveDirPath().Returns(contentDirPath);
        this.mockPath.Combine(contentDirPath, AtlasDataFileName).Returns(atlasDataFilePath);
        this.mockPath.Combine(contentDirPath, AtlasTextureFileName).Returns(atlasTextureFilePath);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(AtlasTextureContentName);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The atlas data directory '{contentDirPath}' does not contain the required '{atlasTextureFilePath}' atlas image file.");
    }

    [Fact]
    public void Unload_WhenInvoked_UnloadsAtlas()
    {
        // Arrange
        const string contentDirPath = "C:/my-game";
        var textureFilePath = Path.Combine(contentDirPath, AtlasTextureFileName);
        var dataFilePath = Path.Combine(contentDirPath, AtlasDataFileName);

        var mockTexture = Substitute.For<ITexture>();
        mockTexture.Id.Returns(TextureId);

        var atlasData = CreateAtlasData(mockTexture, dataFilePath);
        var mockAtlasData = Substitute.For<IAsepriteAtlas>();
        mockAtlasData.Texture.Returns(mockTexture);
        mockAtlasData.FilePath.Returns(textureFilePath);

        this.mockAtlasDataPathResolver.ResolveDirPath().Returns(contentDirPath);
        this.mockDirectory.Exists(Arg.Any<string>()).Returns(true);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(false);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(AtlasTextureContentName);
        this.mockPath.Combine(contentDirPath, AtlasDataFileName).Returns(dataFilePath);
        this.mockPath.Combine(contentDirPath, AtlasTextureFileName).Returns(textureFilePath);
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
        this.mockJsonService.Deserialize<AsepriteAtlas>(Arg.Any<string>()).Returns(atlasData);

        var sut = CreateSystemUnderTest();
        sut.Load(AtlasTextureContentName);

        // Act
        sut.Unload(mockAtlasData);

        // Assert
        var atlasCacheField = Field.GetFieldValue<
            AsepriteAtlasLoader,
            ConcurrentDictionary<string, (ITexture atlasTexture, AsepriteAtlas subTextureData)>>("atlasCache", sut);
        _ = mockTexture.Received(1).Id;
        _ = mockAtlasData.Received(1).FilePath;
        atlasCacheField.Count.Should().Be(0);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void Reactables_WhenUnsubscribing_DisposesOfSubscription()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.mockShutdownSubscription.OnUnsubscribe();

        // Assert
        this.mockShutdownUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    public void ShutdownReactable_WhenInvoked_ShutsDownLoader()
    {
        // Arrange
        const string contentDirPath = "C:/my-game";
        var textureFilePath = Path.Combine(contentDirPath, AtlasTextureFileName);
        var dataFilePath = Path.Combine(contentDirPath, AtlasDataFileName);

        var mockTexture = Substitute.For<ITexture>();
        mockTexture.Id.Returns(TextureId);
        var disposeTextureData = new DisposeTextureData { TextureId = TextureId, };

        var atlasData = CreateAtlasData(mockTexture, dataFilePath);

        this.mockAtlasDataPathResolver.ResolveDirPath().Returns(contentDirPath);
        this.mockDirectory.Exists(Arg.Any<string>()).Returns(true);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(false);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(AtlasTextureContentName);
        this.mockPath.Combine(contentDirPath, AtlasDataFileName).Returns(dataFilePath);
        this.mockPath.Combine(contentDirPath, AtlasTextureFileName).Returns(textureFilePath);
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
        this.mockJsonService.Deserialize<AsepriteAtlas>(Arg.Any<string>()).Returns(atlasData);
        this.mockTextureFactory.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>()).Returns(mockTexture);

        var sut = CreateSystemUnderTest();
        sut.Load(AtlasTextureContentName);

        // Act
        this.mockShutdownSubscription.OnReceive();
        this.mockShutdownSubscription.OnReceive(); // Tests idempotent behavior of the shutdown process

        // Assert
        sut.TotalCachedItems.Should().Be(0);
        this.mockDisposeReactable.Received(1)
            .Push(PushNotifications.TextureDisposedId, disposeTextureData);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of the <see cref="AsepriteAtlasLoader"/> class for testing.
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <param name="dataFilePath">The atlas data file path.</param>
    /// <returns>The atlas data.</returns>
    private static AsepriteAtlas CreateAtlasData(ITexture texture, string dataFilePath)
    {
        var frames = new Dictionary<int, AnimationFrame>
        {
            {
                0,
                new AnimationFrame
                {
                    Bounds = new Rectangle(11, 11, 11, 11),
                    SpriteSourceSize = new Rectangle(11, 11, 11, 11),
                    SourceSize = new Size(11, 11),
                    Duration = 11,
                    Rotated = true,
                    Trimmed = true,
                }
            },
        };

        var atlasData = new AsepriteAtlas
        {
            Texture = texture,
            Name = "test-name",
            FilePath = dataFilePath,
            Frames = frames,
            Meta = new MetaData
            {
                App = "test-app",
                Format = "test-format",
                Size = new Size(11, 11),
                ImageFileName = "test-img-filename",
                Scale = "test-scale",
                Version = "test-version",
            },
        };

        return atlasData;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AsepriteAtlasLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AsepriteAtlasLoader CreateSystemUnderTest()
        => new (
            this.mockTextureFactory,
            this.mockReactableFactory,
            this.mockAtlasDataPathResolver,
            this.mockImageService,
            this.mockJsonService,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);
}
