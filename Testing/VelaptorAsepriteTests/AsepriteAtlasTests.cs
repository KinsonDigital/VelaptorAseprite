// <copyright file="AsepriteAtlasTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests;

using System.Drawing;
using FluentAssertions;
using Helpers;
using NSubstitute;
using Velaptor;
using Velaptor.Content;
using VelaptorAseprite;
using VelaptorAseprite.Data;

/// <summary>
/// Tests the <see cref="AsepriteAtlas"/> class.
/// </summary>
public class AsepriteAtlasTests
{
    private const int AnimationDuration = 16;
    #region Prop Tests
    [Fact]
    public void LoopingBehavior_WhenSettingAndGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.CurrentLoopCount = 44;

        // Act
        sut.LoopingBehavior = LoopingBehavior.Count;

        // Assert
        sut.LoopingBehavior.Should().Be(LoopingBehavior.Count);
        sut.CurrentLoopCount.Should().Be(0);
    }
    #endregion

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_AllPropsSetToCorrectDefaultValue()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Frames.Should().BeEmpty();
        sut.Enabled.Should().BeTrue();
        sut.IsAnimating.Should().BeFalse();
        sut.CurrentFrameIndex.Should().Be(0);
        sut.LoopingBehavior.Should().Be(LoopingBehavior.Infinite);
        sut.CurrentLoopCount.Should().Be(0);
        sut.MaxLoops.Should().Be(0);
        sut.TotalLoops.Should().Be(0);
        sut.Direction.Should().Be(AnimationDirection.Forward);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetCurrentFrame_WhenInvoked_ReturnsCorrectFrame()
    {
        // Arrange
        var frames = new Dictionary<int, AnimationFrame>
        {
            {
                0,
                new AnimationFrame
                {
                    Bounds = new Rectangle(11, 11, 11, 11),
                    SpriteSourceSize = new Rectangle(11, 11, 11, 11),
                    Duration = 11,
                    Rotated = true,
                    Trimmed = true,
                    SourceSize = new Size(11, 11),
                }
            },
        };
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;

        // Act
        var actual = sut.GetCurrentFrame();

        // Assert
        actual.Should().BeEquivalentTo(frames[0]);
    }

    [Fact]
    public void SetAnimationSpeed_WhenInvoked_SetsAnimationSpeed()
    {
        // Arrange
        var frames = new Dictionary<int, AnimationFrame>
        {
            {
                0,
                new AnimationFrame
                {
                    Bounds = new Rectangle(11, 11, 11, 11),
                    SpriteSourceSize = new Rectangle(11, 11, 11, 11),
                    SourceSize = new Size(11, 11),
                    Duration = 22,
                    Rotated = true,
                    Trimmed = true,
                }
            },
            {
                1,
                new AnimationFrame
                {
                    Bounds = new Rectangle(33, 33, 33, 33),
                    SpriteSourceSize = new Rectangle(33, 33, 33, 33),
                    SourceSize = new Size(33, 33),
                    Duration = 44,
                    Rotated = true,
                    Trimmed = true,
                }
            },
        };
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;

        // Act
        sut.SetAnimationSpeed(55);

        // Assert
        sut.Frames.Should().HaveCount(2);

        // Validate that the speed has been updated for all items
        sut.Frames[0].Duration.Should().Be(55);
        sut.Frames[1].Duration.Should().Be(55);

        // Validated that no other values were changed
        sut.Frames[0].Bounds.Should().BeEquivalentTo(frames[0].Bounds);
        sut.Frames[0].SpriteSourceSize.Should().BeEquivalentTo(frames[0].SpriteSourceSize);
        sut.Frames[0].SourceSize.Should().BeEquivalentTo(frames[0].SourceSize);
        sut.Frames[0].Rotated.Should().Be(frames[0].Rotated);
        sut.Frames[0].Trimmed.Should().Be(frames[0].Trimmed);
        sut.Frames[1].Bounds.Should().BeEquivalentTo(frames[1].Bounds);
        sut.Frames[1].SpriteSourceSize.Should().BeEquivalentTo(frames[1].SpriteSourceSize);
        sut.Frames[1].SourceSize.Should().BeEquivalentTo(frames[1].SourceSize);
        sut.Frames[1].Rotated.Should().Be(frames[1].Rotated);
        sut.Frames[1].Trimmed.Should().Be(frames[1].Trimmed);
    }

    [Fact]
    public void Play_WhenInvokedWithNoAnimationName_StartsAnimation()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoopingBehavior = LoopingBehavior.Count;
        sut.CurrentLoopCount = 22;

        // Act
        sut.Play();

        // Assert
        sut.IsAnimating.Should().BeTrue();
        sut.CurrentLoopCount.Should().Be(0);
    }

    [Fact]
    public void Play_WhenInvokedWithAnimationName_StartsAnimation()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoopingBehavior = LoopingBehavior.Count;
        sut.CurrentLoopCount = 22;
        sut.Meta = CreateMetaData();
        sut.Frames = CreateTestFrames();

        // Act
        sut.Play("animation-1");

        // Assert
        sut.IsAnimating.Should().BeTrue();
        sut.CurrentLoopCount.Should().Be(0);
    }

    [Fact]
    public void Play_WhenInvokedWithAnimationNameWhileTagDoesNotExist_StartsAnimation()
    {
        // NOTE: The internal tag name and animation name are the same thing
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoopingBehavior = LoopingBehavior.Count;
        sut.CurrentLoopCount = 22;
        sut.Frames = CreateTestFrames();

        // Act
        var act = () => sut.Play("animation-1");

        // Assert
        act.Should().Throw<NoTagException>().WithMessage("No Aseprite tag name exists that matches the animation of 'animation-1'.");
    }

    [Fact]
    public void Play_WhenInvokedWithNoAnimationNameWhileTagDoesNotExist_ThrowsException()
    {
        // NOTE: The internal tag name and animation name are the same thing
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoopingBehavior = LoopingBehavior.Count;
        sut.CurrentLoopCount = 22;
        sut.Frames = CreateTestFrames();

        // Act
        var act = () => sut.Play("animation-1");

        // Assert
        act.Should().Throw<NoTagException>().WithMessage("No Aseprite tag name exists that matches the animation of 'animation-1'.");
    }

    [Fact]
    public void Stop_WhenInvoked_StopsAnimation()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Play();

        // Act
        sut.Stop();

        // Assert
        sut.IsAnimating.Should().BeFalse();
    }

    [Fact]
    public void Reset_WhenInvoked_ResetsAnimation()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.CurrentFrameIndex = 4;
        sut.Play();
        Field.SetFieldValue("currentFrameElapsedMs", sut, 123);

        // Act
        sut.Reset();
        var currentFrameElapsesMs = Field.GetFieldValue<AsepriteAtlas, int>("currentFrameElapsedMs", sut);

        // Assert
        sut.CurrentFrameIndex.Should().Be(0);
        currentFrameElapsesMs.Should().Be(0);
        sut.IsAnimating.Should().BeFalse();
    }

    [Theory]
    [InlineData( // Forward animation with the last frame and a non-complete cycle
        AnimationDirection.Forward,
        LoopingBehavior.Infinite,
        2,
        0,
        0,
        true,
        3,
        0)]
    [InlineData( // Forward animation with the last frame and a complete cycle
        AnimationDirection.Forward,
        LoopingBehavior.Infinite,
        4,
        1,
        0,
        true,
        2,
        0)]
    [InlineData( // Backward animation with the first frame and a non-complete cycle
        AnimationDirection.Backward,
        LoopingBehavior.Infinite,
        3,
        0,
        0,
        true,
        2,
        0)]
    [InlineData( // Backward animation with the first frame and a complete cycle
        AnimationDirection.Backward,
        LoopingBehavior.Infinite,
        2,
        1,
        0,
        true,
        4,
        0)]
    public void Update_WithForwardAndBackwardAnimation_AnimatesInProperDirection(AnimationDirection direction,
        LoopingBehavior behavior,
        int currentFrameIndex,
        uint expectedTotalLoops,
        uint expectedCurrentLoopCount,
        bool expectedIsAnimating,
        int expectedCurrentFrameIndex,
        int expectedElapsedFrameTimeMs)
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = behavior;
        sut.Direction = direction;
        sut.Meta = CreateMetaData();
        sut.AnimationName = "animation-2";
        sut.CurrentFrameIndex = currentFrameIndex; // NOTE: This must be run after the setting of the animation name for the test
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);
        sut.Play();

        // Act
        // We have to run update twice to get to the second frame
        sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        var elapsedFrameTimeMs = Field.GetFieldValue<AsepriteAtlas, int>("currentFrameElapsedMs", sut);
        sut.CurrentFrameIndex.Should().Be(expectedCurrentFrameIndex);
        elapsedFrameTimeMs.Should().Be(expectedElapsedFrameTimeMs);
        sut.TotalLoops.Should().Be(expectedTotalLoops);
        sut.CurrentLoopCount.Should().Be(expectedCurrentLoopCount);
        sut.IsAnimating.Should().Be(expectedIsAnimating);
    }

    [Fact]
    public void Update_WhenInvokedWithNoAnimationNameWhileTagExists_StartsAnimation()
    {
        // NOTE: The internal tag name and animation name are the same thing
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoopingBehavior = LoopingBehavior.Count;
        sut.CurrentLoopCount = 22;
        sut.Frames = CreateTestFrames();

        // This is to simulate that the index is not the default value of 0
        Field.SetFieldValue("startingIndex", sut, 100);
        Field.SetFieldValue("endingIndex", sut, 200);

        // Act
        sut.AnimationName = null;

        // Assert
        Field.GetFieldValue<AsepriteAtlas, int>("startingIndex", sut).Should().Be(0);
        Field.GetFieldValue<AsepriteAtlas, int>("endingIndex", sut).Should().Be(4);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(10, true)]
    public void Update_WithCountLoopingBehavior_BehavesCorrectly(uint maxLoops, bool expectedIsAnimating)
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.Count;
        sut.Direction = AnimationDirection.Forward;
        sut.MaxLoops = maxLoops;
        sut.Meta = CreateMetaData();
        sut.AnimationName = "animation-2";
        sut.CurrentFrameIndex = 4; // NOTE: This must be run after the setting of the animation name for the test
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);
        sut.Play();

        // Act
        sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        sut.CurrentLoopCount.Should().Be(1);
        sut.IsAnimating.Should().Be(expectedIsAnimating);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void Update_WithNoLoopingBehavior_BehavesCorrectly(int currentFrameIndex, bool expectedIsAnimating)
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Forward;
        sut.Meta = CreateMetaData();
        sut.AnimationName = "animation-1";
        sut.CurrentFrameIndex = currentFrameIndex; // NOTE: This must be run after the setting of the animation name for the test
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);
        sut.Play();

        // Act
        sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        sut.IsAnimating.Should().Be(expectedIsAnimating);
    }

    [Fact]
    public void Update_WhenInvoking_UpdatesTimeElapsed()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Forward;
        sut.Play();

        // Act
        sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        Field.GetFieldValue<AsepriteAtlas, int>("currentFrameElapsedMs", sut).Should().Be(AnimationDuration);
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    public void Update_WhenEnabledOrNotAnimating_DoesNotUpdateAnimation(bool enabled, bool isAnimating, bool expectedIsAnimating)
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Forward;
        sut.Enabled = enabled;
        sut.IsAnimating = isAnimating;
        sut.CurrentFrameIndex += 1;
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);

        // Act
        sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        sut.TotalLoops.Should().Be(0);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(expectedIsAnimating);
        Field.GetFieldValue<AsepriteAtlas, int>("currentFrameElapsedMs", sut).Should().Be(AnimationDuration);
    }

    [Fact]
    public void Update_WithFullForwardAnimationCycle_AnimatesSuccessfully()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Forward;
        sut.Meta = CreateMetaData();
        sut.CurrentFrameIndex = 0; // NOTE: This must be run after the setting of the animation name for the test
        sut.Enabled = true;
        sut.Play();

        // Act
        // Each frame is 16ms long with 5 frames total, so we have to run update 10 times to get through a
        // full cycle. (5 frames * 16ms = 80ms total, 80ms / 16ms per update = 5 updates per cycle, 5 updates * 2 cycles = 10 updates)
        for (var i = 0; i < 10; i++)
        {
            sut.Update(default(FrameTime).SetMs(AnimationDuration));
        }

        // Assert
        sut.TotalLoops.Should().Be(1);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(false);
        Field.GetFieldValue<AsepriteAtlas, int>("currentFrameElapsedMs", sut).Should().Be(0);
    }

    [Fact]
    public void Update_WithInvalidAnimationDirectionWithNonNullAnimationName_ThrowsException()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = (AnimationDirection)400; // Invalid on purpose
        sut.Meta = CreateMetaData();
        sut.AnimationName = "animation-1";
        sut.CurrentFrameIndex = 1; // NOTE: This must be run after the setting of the animation name for the test
        sut.Enabled = true;
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);
        sut.Play();

        // Act
        var act = () => sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        act.Should().Throw<Exception>().WithMessage("Invalid direction value of '400'.");
    }

    [Fact]
    public void Update_WithInvalidAnimationDirectionWithNullAnimationName_ThrowsException()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = (AnimationDirection)400; // Invalid on purpose
        sut.Meta = CreateMetaData();
        sut.CurrentFrameIndex = 1; // NOTE: This must be run after the setting of the animation name for the test
        sut.Enabled = true;
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);
        sut.Play();

        // Act
        var act = () => sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        act.Should().Throw<Exception>().WithMessage("Invalid direction value of '400'.");
    }

    [Fact]
    public void Update_WithFullBackwardAnimationCycle_AnimatesSuccessfully()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Backward;
        sut.Enabled = true;
        sut.CurrentFrameIndex = 1;
        sut.Play();

        // Act
        sut.Update(default(FrameTime).SetMs(AnimationDuration));
        sut.Update(default(FrameTime).SetMs(AnimationDuration));
        sut.Update(default(FrameTime).SetMs(AnimationDuration));
        sut.Update(default(FrameTime).SetMs(AnimationDuration));

        // Assert
        sut.TotalLoops.Should().Be(1);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(false);
        Field.GetFieldValue<AsepriteAtlas, int>("currentFrameElapsedMs", sut).Should().Be(0);
    }
    #endregion

    /// <summary>
    /// Creates animation frames for testing.
    /// </summary>
    /// <returns>The frames.</returns>
    private static Dictionary<int, AnimationFrame> CreateTestFrames()
    {
        const int totalFrames = 5;
        var frames = new Dictionary<int, AnimationFrame>();

        for (var i = 0; i < totalFrames; i++)
        {
            frames.Add(i,
                new AnimationFrame
                {
                    Bounds = new Rectangle(totalFrames, totalFrames, totalFrames, totalFrames),
                    SpriteSourceSize = new Rectangle(totalFrames, totalFrames, totalFrames, totalFrames),
                    Duration = 16,
                    Rotated = true,
                    Trimmed = true,
                    SourceSize = new Size(totalFrames, totalFrames),
                });
        }

        return frames;
    }

    /// <summary>
    /// Creates a new instance of <see cref="MetaData"/> for testing.
    /// </summary>
    /// <returns>The new meta-data.</returns>
    private static MetaData CreateMetaData()
    {
        return new MetaData
        {
            App = "test-app",
            Version = "v1.2.3",
            ImageFileName = "test-atlas.png",
            Format = "test-format",
            Scale = "0",
            Size = new Size(11, 22),
            Tags = [new FrameTag { Name = "animation-1", From = 0, To = 1, }, new FrameTag { Name = "animation-2", From = 2, To = 4, }],
        };
    }

    /// <summary>
    /// Creates a new instance of <see cref="AsepriteAtlas"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private static AsepriteAtlas CreateSystemUnderTest() =>
        new ()
        {
            Texture = Substitute.For<ITexture>(),
            Name = "test-atlas",
            FilePath = "C:/my-game/test-atlas.png",
            Meta = new MetaData
            {
                App = "test-app",
                Version = "v1.2.3",
                ImageFileName = "test-atlas.png",
                Format = "test-format",
                Scale = "0",
            },
        };
}
