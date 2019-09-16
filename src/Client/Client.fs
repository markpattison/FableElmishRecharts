module App.Client

open Elmish
open Elmish.React
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

importAll "./sass/main.sass"

type Page = LineChart | BarChart | ProbabilityFunnel

type Model =
  { CurrentPage: Page
    LineChartModel: LineChart.Model
    BarChartModel: BarChart.Model
    ProbabilityFunnelModel: ProbabilityFunnel.Model }

type Msg =
| ShowPage of Page
| LineChartMsg of LineChart.Msg
| BarChartMsg of BarChart.Msg
| ProbabilityFunnelMsg of ProbabilityFunnel.Msg

let init () : Model * Cmd<Msg> =
    { CurrentPage = LineChart
      LineChartModel = LineChart.init
      BarChartModel = BarChart.init
      ProbabilityFunnelModel = ProbabilityFunnel.init
    }, []

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg with
    | ShowPage page -> { model with CurrentPage = page }, []
    | LineChartMsg innerMsg ->
        let updated, cmd = LineChart.update innerMsg model.LineChartModel
        { model with LineChartModel = updated }, Cmd.map LineChartMsg cmd
    | BarChartMsg innerMsg ->
        let updated, cmd = BarChart.update innerMsg model.BarChartModel
        { model with BarChartModel = updated }, Cmd.map BarChartMsg cmd
    | ProbabilityFunnelMsg innerMsg ->
        let updated, cmd = ProbabilityFunnel.update innerMsg model.ProbabilityFunnelModel
        { model with ProbabilityFunnelModel = updated }, Cmd.map ProbabilityFunnelMsg cmd

let menuLink currentPage dispatch label page =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.Props [ OnClick (fun _ -> ShowPage page |> dispatch) ] ]
      [ str label ]

let menu currentPage dispatch =
  let menuItem = menuLink currentPage dispatch
  Menu.menu []
    [ Menu.label []
        [ str "Chart Types" ]
      Menu.list []
        [ menuItem "Line chart" LineChart
          menuItem "Bar chart" BarChart
          menuItem "Probability funnel" ProbabilityFunnel ] ]

let pageContent (model : Model) (dispatch : Msg -> unit) =
  match model.CurrentPage with
  | LineChart -> LineChart.view model.LineChartModel (LineChartMsg >> dispatch)
  | BarChart -> BarChart.view model.BarChartModel (BarChartMsg >> dispatch)
  | ProbabilityFunnel -> ProbabilityFunnel.view model.ProbabilityFunnelModel (ProbabilityFunnelMsg >> dispatch)

let view (model : Model) (dispatch : Msg -> unit) =
  div []
    [ Navbar.view
      Section.section []
        [ Container.container []
            [ Columns.columns []
                [ Column.column
                    [ Column.Width (Screen.All, Column.Is3) ]
                    [ menu model.CurrentPage dispatch ]
                  Column.column []
                    [ pageContent model dispatch ] ] ] ] ]

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
