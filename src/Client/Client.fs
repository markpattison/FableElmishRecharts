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

importAll "./sass/main.sass"

type Data = { Name: string; X1: float; X2: float; X3: float; X4: float; X5: float }
type Values = Data []

type View = Page1 | Page2
type ChartData = { Name: string; Range1: float * float; Range2: float * float; Median: float }

type Model = { View: View; Values1: ChartData[] option; Values2: ChartData[] option }

type Msg = | ShowPage of View

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
    | ShowPage page -> { currentModel with View = page }, []

let margin t r b l =
    Chart.Margin { top = t; bottom = b; right = r; left = l }

let menuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.Props [ OnClick (fun _ -> ShowPage page |> dispatch) ] ]
      [ str label ]

let menu currentPage dispatch =
  Menu.menu []
    [ Menu.label []
        [ str "General" ]
      Menu.list []
        [ menuItem "Page 1" Page1 currentPage dispatch
          menuItem "Page 2" Page2 currentPage dispatch ] ]

let navBar =
    div []
      [ Navbar.navbar [ Navbar.Color IsPrimary ]
          [ Navbar.Brand.div []
              [ Navbar.Item.div []
                  [ Heading.h4 [] [ str "FableElmishRecharts" ] ] ] ] ]

let valueFormatter (value: obj) =
    match value with
    | :? (float[]) as arr ->
        match arr with
        | [| v1; v2 |] -> sprintf "£%.2f-£%.2f" v1 v2
        | x -> x.ToString()
    | :? float as v -> sprintf "£%.2f" v
    | x -> x.ToString()

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
                  yaxis [ Cartesian.Type "number"; Cartesian.Domain [| box 0.0; box 10.0 |]; Cartesian.AllowDataOverflow true ] []
                  tooltip [ Tooltip.Formatter valueFormatter ] [] ]
        | None -> str "No data loaded"
    
    let content =
        match model.View with
        | Page1 ->
            div []
              [ div [] [ Heading.h4 [] [ str "Page 1" ] ]
                chart model.Values1 ]
        | Page2 ->
            div []
              [ div [] [ Heading.h4 [] [ str "Page 2" ] ]
                chart model.Values2 ]
    div []
        [ div [ ClassName "navbar-bg" ] [ navBar ]
          Section.section []
            [ Columns.columns []
                [ Column.column
                    [ Column.Width (Screen.All, Column.Is2) ]
                    [ menu model.View dispatch ]
                  Column.column []
                    [ content ] ] ] ]

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
