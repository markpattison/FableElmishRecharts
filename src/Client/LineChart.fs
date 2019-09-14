module App.LineChart

open Elmish
open Fable.Helpers.React
open Fable.Recharts
open Fable.Recharts.Props
open Fulma

open Common

module P = Fable.Helpers.React.Props

type LineChoice = Thick | Dashed
type DotChoice = Dots | NoDots

type Model = { LineChoice: LineChoice; DotsChoice: DotChoice }

type Msg = ToggleLine | ToggleDots

type ChartData = { Name: string; A: float; B: float; C: float }

let data = [|  { Name = "Spring"; A = 1.9; B = 3.0; C = 1.0 }
               { Name = "Summer"; A = 1.2; B = 2.2; C = 2.0 }
               { Name = "Autumn"; A = 0.9; B = 2.8; C = 3.0 }
               { Name = "Winter"; A = 1.6; B = 3.0; C = 4.0 }
           |]

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
  match msg with
  | ToggleLine ->
    { model with LineChoice = if model.LineChoice = Thick then Dashed else Thick }, []
  | ToggleDots ->
    { model with DotsChoice = if model.DotsChoice = Dots then NoDots else Dots }, []

let showChart values lineStyle showDots =

  let lineProps : P.IProp list =
    match lineStyle with
    | Thick -> [ P.StrokeWidth 3.0 ]
    | Dashed -> [ P.StrokeDasharray "5 5" ]
  
  let dotProps : P.IProp list =
    match showDots with
    | Dots -> []
    | NoDots -> [ Cartesian.Dot false ]

  lineChart
    [ Chart.Margin { top = 20.0; bottom = 20.0; right = 20.0; left = 20.0 }
      Chart.Width 640.0
      Chart.Height 320.0
      Chart.Data values ]
    [ cartesianGrid
        [ P.Stroke "#ccc"
          P.StrokeDasharray "3 3" ] []
      line
        ([ Cartesian.DataKey "A"; P.Stroke "#AA3333" ] @ lineProps @ dotProps) []
      line
        ([ Cartesian.DataKey "B"; P.Stroke "#33AA33" ] @ lineProps @ dotProps) []
      line
        ([ Cartesian.DataKey "C"; P.Stroke "#3333AA" ] @ lineProps @ dotProps) []
      xaxis [ Cartesian.DataKey "Name"; Cartesian.Scale ScaleType.Point ] []
      yaxis [ Cartesian.Type "number" ] [] ]

let view (model : Model) (dispatch : Msg -> unit) =
  
  div []
    [ Markdown.parseAsReactEl "" """
## Line chart
A simple line chart."""

      showChart data model.LineChoice model.DotsChoice
      Button.list []
        [ button (fun _ -> dispatch ToggleLine) "Toggle line style"
          button (fun _ -> dispatch ToggleDots) "Toggle dots" ]
      br []

      Markdown.parseAsReactEl "" """
###### Data type
A simple record type can be used for the chart data:

    type ChartData = { Name: string; A: float; B: float; C: float }

    let data = [|  { Name = "Spring"; A = 1.9; B = 3.0; C = 1.0 }
                   { Name = "Summer"; A = 1.2; B = 2.2; C = 2.0 }
                   { Name = "Autumn"; A = 0.9; B = 2.8; C = 3.0 }
                   { Name = "Winter"; A = 1.6; B = 3.0; C = 4.0 }
               |]

###### Chart
We compose a  grid with three line-series.  The `DataKey` property specifies which member of the data type will be used for that series.

We also specify the colour for the series, plus some examples of line styles.

    let showChart values lineStyle showDots =

      let lineProps : P.IProp list =
        match lineStyle with
        | Thick -> [ P.StrokeWidth 3.0 ]
        | Dashed -> [ P.StrokeDasharray "5 5" ]
      
      let dotProps : P.IProp list =
        match showDots with
        | Dots -> []
        | NoDots -> [ Cartesian.Dot false ]

      lineChart
        [ Chart.Margin { top = 20.0; bottom = 20.0; right = 20.0; left = 20.0 }
          Chart.Width 640.0
          Chart.Height 320.0
          Chart.Data values ]
        [ cartesianGrid
            [ P.Stroke "#ccc"
              P.StrokeDasharray "3 3" ] []
          line
            ([ Cartesian.DataKey "A"; P.Stroke "#AA3333" ] @ lineProps @ dotProps) []
          line
            ([ Cartesian.DataKey "B"; P.Stroke "#33AA33" ] @ lineProps @ dotProps) []
          line
            ([ Cartesian.DataKey "C"; P.Stroke "#3333AA" ] @ lineProps @ dotProps) []
          xaxis [ Cartesian.DataKey "Name"; Cartesian.Scale ScaleType.Point ] []
          yaxis [ Cartesian.Type "number" ] [] ]

See also the [Recharts example](http://recharts.org/en-US/examples/SimpleLineChart)."""
      ]
