// <copyright file="NoTagException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

/// <summary>
/// Thrown when there is no Aseprite tag that exists.
/// </summary>
public class NoTagException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoTagException"/> class.
    /// </summary>
    public NoTagException()
        : base("Aseprite tag does not exist.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoTagException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoTagException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoTagException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public NoTagException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
