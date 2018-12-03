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

open Fulma
open Fable.Core

type Data = { Name: string; X1: float; X2: float; X3: float; X4: float; X5: float }
type Values = Data []

type View = Page1 | Page2
type ChartData = { Name: string; Range1: float * float; Range2: float * float; Median: float }

type Model = { View: View; Values1: ChartData[] option; Values2: ChartData[] option }

type Msg =
    | ShowPage1
    | ShowPage2

let data1 = [| { Name = "31/12/2017"; X1 = 1.0; X2 = 1.0; X3 = 1.0; X4 = 1.0; X5 = 1.0 }
               { Name = "31/12/2018"; X1 = 1.0; X2 = 2.0; X3 = 4.0; X4 = 5.0; X5 = 6.0 }
               { Name = "31/12/2019"; X1 = 2.0; X2 = 3.0; X3 = 4.0; X4 = 5.0; X5 = 6.0 }
            |]

let data2 = [| { Name = "31/12/2017"; X1 = 1.0; X2 = 1.0; X3 = 1.0; X4 = 1.0; X5 = 1.0 }
               { Name = "31/12/2018"; X1 = -1.0; X2 = 0.0; X3 = 1.0; X4 = 2.0; X5 = 3.0 }
               { Name = "31/12/2019"; X1 = -3.0; X2 = -2.0; X3 = -1.0; X4 = 0.0; X5 = 1.0 }
            |]

let convertDataForChart data =
  data
  |> Array.map (fun d ->
    {
      Range1 = (d.X1, d.X5)
      Range2 = (d.X2, d.X4)
      Median = d.X3
      Name = d.Name
    })

let init () : Model * Cmd<Msg> =
    { View = Page1
      Values1 = convertDataForChart data1 |> Some
      Values2 = convertDataForChart data2 |> Some
    }, []

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
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
                button "Show page 2" (fun _ -> dispatch ShowPage2)
                chart model.Values1 ]
        | Page2 ->
            div []
              [ div [] [ str "Page 2" ]
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
