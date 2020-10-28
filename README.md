# FableElmishRecharts

A sample app showing how to use Fable.Recharts.

See it live [here](http://www.markpattison.net/fableelmishrecharts/).

## Requirements

* [dotnet SDK](https://www.microsoft.com/net/download/core) 3.0 or higher
* [node.js](https://nodejs.org) with [npm](https://www.npmjs.com/)
* An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/).

## Building and running

First run `dotnet tool restore` as a one-off.  Then to run the app locally use `dotnet fake build -t run`.  This will open the app in your browser at http://localhost:8080/.

To bundle for production just use `dotnet fake build`.

[![Build status](https://markpattison.visualstudio.com/FableElmishRecharts/_apis/build/status/build)](https://markpattison.visualstudio.com/FableElmishRecharts/_build/latest?definitionId=6)
