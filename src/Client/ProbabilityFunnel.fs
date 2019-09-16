module App.ProbabilityFunnel

open Elmish
open Fable.React
open Fable.Recharts
open Fable.Recharts.Props
open Fulma

open Common

module P = Fable.React.Props

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

let init = { DataChoice = Data1 }

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
  match msg with
  | ToggleData ->
    let newData = if model.DataChoice = Data1 then Data2 else Data1
    { model with DataChoice = newData }, []

let valueFormatter (value: obj) =
  match value with
  | :? float as v -> sprintf "£%.2f" v
  | :? (float[]) as arr when arr.Length = 2 -> sprintf "£%.2f-£%.2f" arr.[0] arr.[1]
  | x -> x.ToString()

let showChart values =
  composedChart
    [ Chart.Margin { top = 20.0; bottom = 20.0; right = 20.0; left = 20.0 }
      Chart.Width 640.0
      Chart.Height 320.0
      Chart.Data values ]
    [ cartesianGrid
        [ P.Stroke "#ccc"
          P.StrokeDasharray "5 5" ] []
      area
        [ Cartesian.DataKey "OuterRange"
          Cartesian.Name "90% within"
          P.Stroke "none"
          P.Fill "#000050" ] []
      area                  
        [ Cartesian.DataKey "InnerRange"
          Cartesian.Name "68% within"
          P.Stroke "none"
          P.Fill "#000050" ] []
      line
        [ Cartesian.DataKey "Median"
          Cartesian.Dot false
          P.Stroke "#666666"
          P.StrokeWidth 2. ] []
      xaxis [ Cartesian.DataKey "Name"; Cartesian.Scale ScaleType.Point ] []
      yaxis [ Cartesian.Type "number"; Cartesian.Domain [| box 0.0; box 10.0 |]; Cartesian.AllowDataOverflow true ] []
      tooltip [ Tooltip.Formatter valueFormatter ] [] ]

let view (model : Model) (dispatch : Msg -> unit) =
  let chartData =
    match model.DataChoice with
    | Data1 -> data1
    | Data2 -> data2
  
  div []
    [ Markdown.parseAsReactEl "" """
## Probability funnel
Used to show a range of possible outcomes by plotting percentile values."""

      showChart chartData
      Button.list [] [ button (fun _ -> dispatch ToggleData) "Toggle data set" ]
      br []

      Markdown.parseAsReactEl "" """
In the chart above we show an outer range (in this case between the 5th and 95th percentiles, hence containing 90% of outcomes), an inner range (16th-84th percentiles, containing 68% of outcomes) and the median (50th percentile).  This chart is slightly more complicated than a typical area chart because the plotted areas don't start from zero, and also overlap.

###### Data types
For the ranges we can use a tuple with two values, which will be converted into Javascript as an array with two values:

    type ChartData = { Name: string; OuterRange: float * float; InnerRange: float * float; Median: float }

    let data1 = [| { Name = "31/12/2018"; OuterRange = (1.0, 1.0); InnerRange = (1.0, 1.0); Median = 1.0 }
                   { Name = "31/12/2019"; OuterRange = (1.2, 3.0); InnerRange = (1.6, 2.4); Median = 2.0 }
                   { Name = "31/12/2020"; OuterRange = (1.4, 5.2); InnerRange = (2.4, 3.8); Median = 3.0 }
                   { Name = "31/12/2021"; OuterRange = (1.5, 7.4); InnerRange = (3.2, 5.2); Median = 4.0 }
                |]

###### Tooltips
In this example we use a custom value formatter to show the ranges sensibly.  This function takes a data point (which will either be a single value or an array of two values) and returns a string with the relevant tooltip text:

    let valueFormatter (value: obj) =
      match value with
      | :? float as v -> sprintf "£%.2f" v
      | :? (float[]) as arr when arr.Length = 2 -> sprintf "£%.2f-£%.2f" arr.[0] arr.[1]
      | x -> x.ToString()

###### Chart
We compose a simple grid with two area-series for the ranges and one line-series for the median.  The `DataKey` property specifies which member of the data type will be used for that series.

For the y-axis we set the range manually to avoid the chart rescaling when different data is used.  We also use the `AllowDataOverflow` property to avoid rescaling when values go outside this range.

    let showChart values =
      composedChart
        [ Chart.Margin { top = 20.0; bottom = 20.0; right = 0.0; left = 0.0 }
          Chart.Width 640.0
          Chart.Height 320.0
          Chart.Data values ]
        [ cartesianGrid
            [ P.Stroke "#ccc"
              P.StrokeDasharray "5 5" ] []
          area
            [ Cartesian.DataKey "OuterRange"
              Cartesian.Name "90% within"
              P.Stroke "none"
              P.Fill "#000050" ] []
          area                  
            [ Cartesian.DataKey "InnerRange"
              Cartesian.Name "68% within"
              P.Stroke "none"
              P.Fill "#000050" ] []
          line
            [ Cartesian.DataKey "Median"
              Cartesian.Dot false
              P.Stroke "#666666"
              P.StrokeWidth 2. ] []
          xaxis [ Cartesian.DataKey "Name"; Cartesian.Scale ScaleType.Point ] []
          yaxis [ Cartesian.Type "number"; Cartesian.Domain [| box 0.0; box 10.0 |]; Cartesian.AllowDataOverflow true ] []
          tooltip [ Tooltip.Formatter valueFormatter ] [] ]"""
      ]
