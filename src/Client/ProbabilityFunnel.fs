module App.ProbabilityFunnel

open Elmish
open Fable.Helpers.React
open Fable.Recharts
open Fable.Recharts.Props
open Fulma

module P = Fable.Helpers.React.Props

type DataChoice = Data1 | Data2
type Model = { DataChoice: DataChoice }

type Msg = | ToggleData

type ChartData = { Name: string; OuterRange: float * float; InnerRange: float * float; Median: float }

let data1 = [| { Name = "31/12/2018"; OuterRange = (1.0, 1.0); InnerRange = (1.0, 1.0); Median = 1.0 }
               { Name = "31/12/2019"; OuterRange = (1.2, 3.0); InnerRange = (1.6, 2.4); Median = 2.0 }
               { Name = "31/12/2020"; OuterRange = (1.4, 5.2); InnerRange = (2.4, 3.8); Median = 3.0 }
               { Name = "31/12/2021"; OuterRange = (1.5, 7.4); InnerRange = (3.2, 5.2); Median = 4.0 }
            |]

let data2 = [| { Name = "31/12/2018"; OuterRange = (1.0, 1.0); InnerRange = (1.0, 1.0); Median = 1.0 }
               { Name = "31/12/2019"; OuterRange = (0.5, 1.9); InnerRange = (1.1, 1.7); Median = 1.5 }
               { Name = "31/12/2020"; OuterRange = (-0.3, 2.1); InnerRange = (0.7, 1.8); Median = 1.3 }
               { Name = "31/12/2021"; OuterRange = (-0.9, 1.7); InnerRange = (0.1, 1.1); Median = 0.5 }
            |]

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
    [ Chart.Margin { top = 20.0; bottom = 20.0; right = 0.0; left = 0.0 }
      Chart.Width 640.0
      Chart.Height 320.0
      Chart.Data values ]
    [ cartesianGrid
        [ P.Stroke "#ccc"
          P.StrokeDasharray "5 5" ]
        []
      area
        [ Cartesian.DataKey "OuterRange"
          Cartesian.Name "90% within"
          P.Stroke "none"
          P.Fill "#000050" ]
        []
      area                  
        [ Cartesian.DataKey "InnerRange"
          Cartesian.Name "68% within"
          P.Stroke "none"
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
    [ Heading.h4 [] [ str "Probability funnel" ]
      p [] [ str "Used to show a range of possible outcomes by plotting percentile values." ]
      chartData |> showChart 
      button (fun _ -> dispatch ToggleData) "Toggle data set" ]
