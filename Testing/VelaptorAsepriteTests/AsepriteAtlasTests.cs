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
    private const uint AnimationDuration = 16;
    private readonly Random random = new ();

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
                0, new AnimationFrame
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
                0, new AnimationFrame
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
                1, new AnimationFrame
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

    [Theory]
    [InlineData(
        false,
        "animation-1",
        LoopingBehavior.Count,
        100,
        true,
        "animation-1",
        1,
        0)]
    [InlineData(
        false,
        null,
        LoopingBehavior.None,
        200,
        true,
        "",
        1,
        200)]
    [InlineData(
        true,
        "test-animation",
        LoopingBehavior.None,
        200,
        true,
        "",
        0,
        200)]
    public void Play_WhenInvoked_PlaysAnimation(
        bool isAnimating,
        string? animationName,
        LoopingBehavior loopingBehavior,
        uint currentLoopCount,
        bool expectingIsAnimating,
        string? expectedAnimationName,
        uint expectedCurrentFrameIndex,
        uint expectedCurrentLoopCount)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Frames = CreateTestFrames();
        sut.Meta = CreateMetaData();
        sut.IsAnimating = isAnimating;
        sut.LoopingBehavior = loopingBehavior;
        sut.CurrentLoopCount = currentLoopCount;

        // Act
        sut.Play(animationName);

        // Assert
        sut.IsAnimating.Should().Be(expectingIsAnimating);
        sut.AnimationName.Should().Be(expectedAnimationName);
        sut.CurrentFrameIndex.Should().Be(expectedCurrentFrameIndex);
        sut.CurrentLoopCount.Should().Be(expectedCurrentLoopCount);
    }

    [Fact]
    public void Play_WhenInvokedWithAnimationNameWhileTagDoesNotExist_ThrowsException()
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
        Field.SetFieldValue("currentFrameElapsedMs", sut, 123u);

        // Act
        sut.Reset();
        var currentFrameElapsesMs = Field.GetFieldValue<AsepriteAtlas, uint>("currentFrameElapsedMs", sut);

        // Assert
        sut.CurrentFrameIndex.Should().Be(0);
        currentFrameElapsesMs.Should().Be(0);
        sut.IsAnimating.Should().BeFalse();
    }

    [Fact]
    public void Update_WithAnimationNameAndFullForwardCycleWithNoLooping_AnimatesSuccessfully()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Forward;
        sut.Meta = CreateMetaData();
        sut.Enabled = true;
        sut.Play("animation-2");

        // Act
        var update = () => sut.Update(CreateFrameTiming());
        update.RunUntil(() => !sut.IsAnimating);

        // Assert
        sut.TotalLoops.Should().Be(1);
        sut.TotalFramesRan.Should().Be(4);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(false);
    }

    [Fact]
    public void Update_WithAnimationNameAndFullBackwardCycleWithNoLooping_AnimatesSuccessfully()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Backward;

        // Set this one value past the last index of the animation
        // The play method is going to decrement the current frame index
        sut.CurrentFrameIndex = 5;

        sut.Meta = CreateMetaData();
        sut.Enabled = true;
        sut.Play("animation-2");

        // Act
        var update = () => sut.Update(CreateFrameTiming());
        update.RunUntil(() => !sut.IsAnimating);

        // Assert
        sut.TotalLoops.Should().Be(1);
        sut.TotalFramesRan.Should().Be(3);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(false);
    }

    [Theory]
    [InlineData(10, 3, false)]
    // [InlineData(1, true)]
    public void Update_WhenCompletingFullAnimationCycleWithCountLoopingBehavior_BehavesCorrectly(
        uint expectedTotalFramesToRan,
        uint expectedCurrentLoopCount,
        bool expectedIsAnimating)
    {
        /*
         * NOTE:
         * The expected frames to complete is always the total number of full animation
         * loops multiplied by the number of frames in the animation.
         */

        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.Count;
        sut.Direction = AnimationDirection.Forward;
        sut.MaxLoops = 3;
        sut.Meta = CreateMetaData();
        sut.Play("animation-2");

        // Act
        // It should not take 1000 updates to complete the animation, but just in case to
        // prevent an infinite loop.
        var update = () =>
        {
            var frameTiming = CreateFrameTiming();
            sut.Update(frameTiming);
        };
        update.RunUntil(() => !sut.IsAnimating);

        // Assert
        sut.TotalLoops.Should().Be(3);
        sut.CurrentLoopCount.Should().Be(expectedCurrentLoopCount);
        sut.TotalFramesRan.Should().Be(expectedTotalFramesToRan);
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
        sut.Update(default(FrameTime).SetMs(16));

        // Assert
        Field.GetFieldValue<AsepriteAtlas, uint>("currentFrameElapsedMs", sut).Should().Be(AnimationDuration);
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    public void Update_WhenDisabledOrNotAnimating_DoesNotUpdateAnimation(bool disabled, bool isAnimating, bool expectedIsAnimating)
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Forward;
        sut.Enabled = disabled;
        sut.IsAnimating = isAnimating;
        sut.CurrentFrameIndex += 1;
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);

        // Act
        sut.Update(CreateFrameTiming());

        // Assert
        sut.TotalLoops.Should().Be(0);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(expectedIsAnimating);
        Field.GetFieldValue<AsepriteAtlas, uint>("currentFrameElapsedMs", sut).Should().Be(AnimationDuration);
    }

    [Fact]
    public void Update_WithFullForwardAnimationCycleAndNoLooping_AnimatesSuccessfully()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Direction = AnimationDirection.Forward;
        sut.Meta = CreateMetaData();
        sut.CurrentFrameIndex = 0;
        sut.Enabled = true;
        sut.Play();

        // Act
        var update = () => sut.Update(CreateFrameTiming());
        update.RunUntil(() => !sut.IsAnimating);

        // Assert
        sut.TotalLoops.Should().Be(1);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(false);
        Field.GetFieldValue<AsepriteAtlas, uint>("currentFrameElapsedMs", sut).Should().Be(0);
    }

    [Fact]
    public void Update_WithInvalidAnimationDirectionWithNonNullAnimationName_ThrowsException()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Meta = CreateMetaData();
        sut.CurrentFrameIndex = 1;
        sut.Enabled = true;
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);
        sut.Play("animation-1");
        sut.Direction = (AnimationDirection)400; // Invalid on purpose and must be executed after the play method

        // Act
        var act = () => sut.Update(CreateFrameTiming());

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Invalid direction value of '400'.");
    }

    [Fact]
    public void Update_WithInvalidAnimationDirectionWithNullAnimationName_ThrowsException()
    {
        // Arrange
        var frames = CreateTestFrames();
        var sut = CreateSystemUnderTest();
        sut.Frames = frames;
        sut.LoopingBehavior = LoopingBehavior.None;
        sut.Meta = CreateMetaData();
        sut.CurrentFrameIndex = 1;
        sut.Enabled = true;
        Field.SetFieldValue("currentFrameElapsedMs", sut, AnimationDuration);
        sut.Play();
        sut.Direction = (AnimationDirection)400; // Invalid on purpose

        // Act
        var act = () => sut.Update(CreateFrameTiming());

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
        sut.Update(CreateFrameTiming());
        sut.Update(CreateFrameTiming());
        sut.Update(CreateFrameTiming());
        sut.Update(CreateFrameTiming());

        // Assert
        sut.TotalLoops.Should().Be(1);
        sut.CurrentLoopCount.Should().Be(0);
        sut.IsAnimating.Should().Be(false);
        Field.GetFieldValue<AsepriteAtlas, uint>("currentFrameElapsedMs", sut).Should().Be(0);
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

    /// <summary>
    /// Creates a random elapsed frame time for testing.
    /// </summary>
    /// <returns>The new frame timing.</returns>
    private FrameTime CreateFrameTiming() => default(FrameTime).SetMs((int)this.random.NextRange(13f, 16f));
}
