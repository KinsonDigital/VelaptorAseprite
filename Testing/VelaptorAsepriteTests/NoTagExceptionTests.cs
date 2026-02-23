// <copyright file="NoTagExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests;

using FluentAssertions;
using VelaptorAseprite;

/// <summary>
/// Tests the <see cref="NoTagException"/> class.
/// </summary>
public class NoTagExceptionTests
{
    #region Ctor Tests
    [Fact]
    public void Ctor_WhenInvokedWithNoParams_CorrectlySetsErrorMessage()
    {
        // Arrange & Act
        var sut = new NoTagException();

        // Assert
        sut.Message.Should().Be("Aseprite tag does not exist.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsErrorMessage()
    {
        // Arrange & Act
        var sut = new NoTagException("test-message");

        // Assert
        sut.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithBothParams_CorrectlySetsErrorMessage()
    {
        // Arrange & Act
        var innerException = new Exception("inner-exception-message");
        var sut = new NoTagException("test-message", innerException);

        // Assert
        sut.Message.Should().Be("test-message");
        sut.InnerException.Message.Should().Be("inner-exception-message");
    }
    #endregion
}
