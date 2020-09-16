# Templates in C#

### Example of different approaches for rendering output with a template engine.

I have used [DotLiquid](http://dotliquidmarkup.org/) and [RazorLight](https://github.com/toddams/RazorLight).

I prefer *DotLiquid*.

It would be interesting to try using the Razor SDK directly as in this example: [https://emilol.com/razor-mailer/](https://emilol.com/razor-mailer/), because we remove the dependency of *Microsoft.AspNetCore.Mvc* and furthermore, *RazorLight* package is [beta](https://www.nuget.org/packages/RazorLight/2.0.0-beta9) righnow. Anyway, *DotLiquid* does what it promises, it would be a good choice.

Other option to consider is [RazorEngine](https://github.com/biohazard999/RazorEngine). A example can be found here [Generate Outputs with Razor Engine in .NET Core](https://khalidabuhakmeh.com/generate-outputs-with-razor-engine-in-dotnet-core)
