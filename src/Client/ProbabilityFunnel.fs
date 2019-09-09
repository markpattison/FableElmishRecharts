module App.ProbabilityFunnel

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

type DataChoice = Data1 | Data2
type Model = { DataChoice: DataChoice }

type Msg = | ToggleData

type Data = { Name: string; X1: float; X2: float; X3: float; X4: float; X5: float }
type Values = Data []

type ChartData = { Name: string; Range1: float * float; Range2: float * float; Median: float }

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

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
  match msg with
  | ToggleData ->
    let newData = if model.DataChoice = Data1 then Data2 else Data1
    { model with DataChoice = newData }, []

let valueFormatter (value: obj) =
  match value with
  | :? (float[]) as arr ->
    match arr with
    | [| v1; v2 |] -> sprintf "£%.2f-£%.2f" v1 v2
    | x -> x.ToString()
  | :? float as v -> sprintf "£%.2f" v
  | x -> x.ToString()

let showChart values =
  composedChart
    [ Chart.Margin { top = 5.0; bottom = 20.0; right = 5.0; left = 0.0 }
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
      yaxis [ Cartesian.Type "number"; Cartesian.Domain [| box 0.0; box 10.0 |]; Cartesian.AllowDataOverflow true ] []
      tooltip [ Tooltip.Formatter valueFormatter ] [] ]

let button onClick  txt =
  Control.div []
    [ Button.button
        [ yield Button.OnClick onClick ]
        [ str txt ] ]

let view (model : Model) (dispatch : Msg -> unit) =
  let chartData, title =
    match model.DataChoice with
    | Data1 -> data1, "Data set 1"
    | Data2 -> data2, "Data set 2"
  
  div []
    [ Heading.h4 [] [ str title ]
      chartData |> convertDataForChart |> showChart 
      button (fun _ -> dispatch ToggleData) "Toggle data" ]
