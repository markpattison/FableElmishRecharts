module Client

open Elmish
open Elmish.React

open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
open Fable.Recharts
open Fable.Recharts.Props
module P = Fable.Helpers.React.Props

open Shared

open Fulma
open Fable.Core

type View = Page1 | Page2
type ChartData = { Name: string; Range1: float * float; Range2: float * float; Median: float }

type Model = { View: View; Values1: ChartData[] option; Values2: ChartData[] option }

type Msg =
    | RequestValues of int
    | ReceiveValues of int * Result<Values, exn>
    | ShowPage1
    | ShowPage2

module Server =

    open Shared
    open Fable.Remoting.Client

    let api : IDemoApi =
      Remoting.createApi()
      |> Remoting.withRouteBuilder Route.builder
      |> Remoting.buildProxy<IDemoApi>

let init () : Model * Cmd<Msg> =
    { View = Page1; Values1 = None; Values2 = None }, []

let loadDataCmd n =
    Cmd.ofAsync
        Server.api.demoValues
        n
        (fun v -> ReceiveValues (n, Ok v))
        (fun exn -> ReceiveValues (n, Error exn))

let convertDataForChart data =
  data
  |> Array.map (fun d ->
    {
      Range1 = (d.X1, d.X5)
      Range2 = (d.X2, d.X4)
      Median = d.X3
      Name = d.Name
    })

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | RequestValues n -> currentModel, loadDataCmd n
    | ReceiveValues (1, Ok values) -> { currentModel with Values1 = values |> convertDataForChart |> Some }, []
    | ReceiveValues (2, Ok values) -> { currentModel with Values2 = values |> convertDataForChart |> Some }, []
    | ReceiveValues _ -> currentModel, []
    | ShowPage1 -> { currentModel with View = Page1 }, []
    | ShowPage2 -> { currentModel with View = Page2 }, []

let button txt onClick =
    Button.button
        [ Button.Color IsPrimary
          Button.OnClick onClick ]
        [ str txt ]

let margin t r b l =
    Chart.Margin { top = t; bottom = b; right = r; left = l }

let view (model : Model) (dispatch : Msg -> unit) =
    let chart values =
        match values with
        | Some values ->
            composedChart
                [ margin 5. 20. 5. 0.
                  Chart.Width 600.
                  Chart.Height 300.
                  Chart.Data values ]
                [ cartesianGrid
                    [ P.Stroke "#ccc"
                      P.StrokeDasharray "5 5" ]
                    []
                  area
                    [ Cartesian.DataKey "Range1"
                      Cartesian.Name "90% within"
                      P.Stroke "#666666"
                      P.Fill "#000050" ]
                    []
                  area                  
                    [ Cartesian.DataKey "Range2"
                      Cartesian.Name "68% within"
                      P.Stroke "#666666"
                      P.Fill "#000050" ]
                    []
                  line
                    [ Cartesian.DataKey "Median"
                      Cartesian.Dot false
                      P.Stroke "#666666"
                      P.StrokeWidth 2. ]
                    []

                  xaxis [ Cartesian.DataKey "Name"; Cartesian.Scale ScaleType.Point ] []
                  yaxis [ ] []
                  tooltip [] [] ]
        | None -> str "No data loaded"
    let content =
        match model.View with
        | Page1 ->
            div []
              [ div [] [ str "Page 1" ]
                button "Get data" (fun _ -> RequestValues 1 |> dispatch)
                button "Show page 2" (fun _ -> dispatch ShowPage2)
                chart model.Values1 ]
        | Page2 ->
            div []
              [ div [] [ str "Page 2" ]
                button "Get data" (fun _ -> RequestValues 2 |> dispatch)
                button "Show page 1" (fun _ -> dispatch ShowPage1 )
                chart model.Values2 ]
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]
          content ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
|> Program.run
