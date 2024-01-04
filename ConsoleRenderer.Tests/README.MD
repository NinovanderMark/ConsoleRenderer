# Notes on unit tests
It's currently not supported to run unit tests directly from the Visual Studio test runner.

Unfortunately that's due to there not being a Console window, and certain operations, such as `Console.CursorLeft()` throw an exception under these conditions.

Please use `dotnet test` instead.