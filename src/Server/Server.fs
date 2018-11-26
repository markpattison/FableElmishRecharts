open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let getDemoValues n : Task<Values> =
    task {
        let data =
            match n with
            | 1 -> [| { Name = "31/12/2017"; X1 = 1.0; X2 = 1.0; X3 = 1.0; X4 = 1.0; X5 = 1.0 }
                      { Name = "31/12/2018"; X1 = 1.0; X2 = 2.0; X3 = 4.0; X4 = 5.0; X5 = 6.0 }
                      { Name = "31/12/2019"; X1 = 2.0; X2 = 3.0; X3 = 4.0; X4 = 5.0; X5 = 6.0 }
                   |]
            | 2 -> [| { Name = "31/12/2017"; X1 = 1.0; X2 = 1.0; X3 = 1.0; X4 = 1.0; X5 = 1.0 }
                      { Name = "31/12/2018"; X1 = -1.0; X2 = 0.0; X3 = 1.0; X4 = 2.0; X5 = 3.0 }
                      { Name = "31/12/2019"; X1 = -3.0; X2 = -2.0; X3 = -1.0; X4 = 0.0; X5 = 1.0 }
                   |]
            | _ -> [| |]
        return data
    }           

let demoApi = {
    demoValues = getDemoValues >> Async.AwaitTask
}

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue demoApi
    |> Remoting.buildHttpHandler

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_gzip
}

run app
