module App.BarChart

open Elmish
open Fable.React
open Fable.Recharts
open Fable.Recharts.Props
open Fulma

open Common

module P = Fable.React.Props

type ChartChoice = Simple | Stacked | Mixed

type Model = { ChartChoice: ChartChoice }

type Msg = ShowSimple | ShowStacked | ShowMixed

type ChartData = { Name: string; A: float; B: float; C: float }

let data = [|  { Name = "Q1"; A = 1.9; B = 3.0; C = 1.0 }
               { Name = "Q2"; A = 1.2; B = 2.2; C = 2.0 }
               { Name = "Q3"; A = 0.9; B = 2.8; C = 3.0 }
               { Name = "Q4"; A = 1.6; B = 3.0; C = 4.0 }
           |]

let init = { ChartChoice = Simple }

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
  match msg with
  | ShowSimple -> { model with ChartChoice = Simple }, []
  | ShowStacked -> { model with ChartChoice = Stacked }, []
  | ShowMixed -> { model with ChartChoice = Mixed }, []

let showChart values chartType =

  let stackIdA, stackIdB, stackIdC =
    
    match chartType with
    | Simple -> ("stack1", "stack2", "stack3")
    | Stacked -> ("stack1", "stack1", "stack1")
    | Mixed -> ("stack1", "stack1", "stack2")

  barChart
    [ Chart.Margin { top = 20.0; bottom = 20.0; right = 20.0; left = 20.0 }
      Chart.Width 640.0
      Chart.Height 320.0
      Chart.Data values ]
    [ cartesianGrid
        [ P.Stroke "#ccc"
          P.StrokeDasharray "3 3" ] []
      bar
        [ Cartesian.DataKey "A"; P.Fill "#AA3333"; Cartesian.StackId stackIdA ] []
      bar
        [ Cartesian.DataKey "B"; P.Fill "#33AA33"; Cartesian.StackId stackIdB ] []
      bar
        [ Cartesian.DataKey "C"; P.Fill "#3333AA"; Cartesian.StackId stackIdC ] []
      xaxis [ Cartesian.DataKey "Name" ] []
      yaxis [ Cartesian.Type "number" ] []
      legend [] [] ]

let view (model : Model) (dispatch : Msg -> unit) =
  
  div []
    [ Markdown.parseAsReactEl "" """
## Bar chart
A simple bar chart."""

      showChart data model.ChartChoice
      Button.list []
        [ selectButton (model.ChartChoice = Simple) (fun _ -> dispatch ShowSimple) "Simple"
          selectButton (model.ChartChoice = Stacked) (fun _ -> dispatch ShowStacked) "Stacked"
          selectButton (model.ChartChoice = Mixed) (fun _ -> dispatch ShowMixed) "Mixed" ]
      br []

      Markdown.parseAsReactEl "" """
###### Data type
A simple record type can be used for the chart data:

    type ChartData = { Name: string; A: float; B: float; C: float }

    let data = [|  { Name = "Q1"; A = 1.9; B = 3.0; C = 1.0 }
                   { Name = "Q2"; A = 1.2; B = 2.2; C = 2.0 }
                   { Name = "Q3"; A = 0.9; B = 2.8; C = 3.0 }
                   { Name = "Q4"; A = 1.6; B = 3.0; C = 4.0 }
               |]

###### Chart
We show a grid and a legend along with the three series.  The `DataKey` property specifies which member of the data type will be used for that series, and the `StackId` property determines which sets of bars are stacked together.
Of course, this doesn't need to be set at all for unstacked bar charts.

    let showChart values chartType =

      let stackIdA, stackIdB, stackIdC =
        
        match chartType with
        | Simple -> ("stack1", "stack2", "stack3")
        | Stacked -> ("stack1", "stack1", "stack1")
        | Mixed -> ("stack1", "stack1", "stack2")

      barChart
        [ Chart.Margin { top = 20.0; bottom = 20.0; right = 20.0; left = 20.0 }
          Chart.Width 640.0
          Chart.Height 320.0
          Chart.Data values ]
        [ cartesianGrid
            [ P.Stroke "#ccc"
              P.StrokeDasharray "3 3" ] []
          bar
            [ Cartesian.DataKey "A"; P.Fill "#AA3333"; Cartesian.StackId stackIdA ] []
          bar
            [ Cartesian.DataKey "B"; P.Fill "#33AA33"; Cartesian.StackId stackIdB ] []
          bar
            [ Cartesian.DataKey "C"; P.Fill "#3333AA"; Cartesian.StackId stackIdC ] []
          xaxis [ Cartesian.DataKey "Name" ] []
          yaxis [ Cartesian.Type "number" ] []
          legend [] [] ]

See also the [Recharts examples](http://recharts.org/en-US/examples/SimpleBarChart)."""
      ]
