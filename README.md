Status Code Handlers Middleware
=====================

[![Build status](https://ci.appveyor.com/api/projects/status/ox3wa91nq1wiw57t)](https://ci.appveyor.com/project/damianh/StatusCodeHandlersMiddleware) [![NuGet Status](http://img.shields.io/nuget/v/StatusCodeHandlersMiddleware.svg?style=flat)](https://www.nuget.org/packages/StatusCodeHandlersMiddleware/) [![NuGet Status](http://img.shields.io/nuget/v/StatusCodeHandlersMiddleware.OwinAppBuilder.svg?style=flat)](https://www.nuget.org/packages/StatusCodeHandlersMiddleware.OwinAppBuilder/)

Middleware to allow you to specify custom handlers for status codes. Handlers will only be invoked if child middleware sets a status code but does **not**

#### Installation

There are two nuget packages. The main one is pure owin and this has no dependencies.

`install-package StatusCodeHandlersMiddleware`

The second package provides integration with IAppBuilder, which is deprecated but provided here for legacy and compatability reasons.

`install-package StatusCodeHandlersMiddleware.OwinAppBuilder`

An asp.net vNext builder integration package will be forthcoming.

#### Using

See [the tests](https://github.com/damianh/StatusCodeHandlersMiddleware/blob/master/src/StatusCodeHandlersMiddleware.Tests/StatusCodeHandlersMiddlewareTests.cs) as examples of usage.


#### Help

[@randompunter](https://twitter.com/randompunter) or [OWIN room on Jabbr](https://jabbr.net/#/rooms/owin)

##### Developed with:

[![Resharper](http://neventstore.org/images/logo_resharper_small.gif)](http://www.jetbrains.com/resharper/)
[![TeamCity](http://neventstore.org/images/logo_teamcity_small.gif)](http://www.jetbrains.com/teamcity/)
[![dotCover](http://neventstore.org/images/logo_dotcover_small.gif)](http://www.jetbrains.com/dotcover/)
[![dotTrace](http://neventstore.org/images/logo_dottrace_small.gif)](http://www.jetbrains.com/dottrace/)
