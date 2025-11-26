namespace VelaptorAseprite.Services;

/// <summary>
/// Provides application wide services.
/// </summary>
internal interface IAppService
{
    /// <summary>
    /// Gets the directory of the application.
    /// </summary>
    string AppDirectory { get; }

    /// <summary>
    /// Starts the application initialization process.
    /// </summary>
    void Init();
}
