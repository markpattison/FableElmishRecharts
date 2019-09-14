module App.Client

open Elmish
open Elmish.React
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

importAll "./sass/main.sass"

type Page = LineChart | ProbabilityFunnel

type Model =
  { CurrentPage: Page
    LineChartModel: LineChart.Model
    ProbabilityFunnelModel: ProbabilityFunnel.Model }

type Msg =
| ShowPage of Page
| LineChartMsg of LineChart.Msg
| ProbabilityFunnelMsg of ProbabilityFunnel.Msg

let init () : Model * Cmd<Msg> =
    { CurrentPage = LineChart
      LineChartModel = LineChart.init
      ProbabilityFunnelModel = ProbabilityFunnel.init
    }, []

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg with
    | ShowPage page -> { model with CurrentPage = page }, []
    | LineChartMsg innerMsg ->
        let updated, cmd = LineChart.update innerMsg model.LineChartModel
        { model with LineChartModel = updated }, Cmd.map LineChartMsg cmd
    | ProbabilityFunnelMsg innerMsg ->
        let updated, cmd = ProbabilityFunnel.update innerMsg model.ProbabilityFunnelModel
        { model with ProbabilityFunnelModel = updated }, Cmd.map ProbabilityFunnelMsg cmd

let menuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.Props [ OnClick (fun _ -> ShowPage page |> dispatch) ] ]
      [ str label ]

let menu currentPage dispatch =
  Menu.menu []
    [ Menu.label []
        [ str "Chart Types" ]
      Menu.list []
        [ menuItem "Line chart" LineChart currentPage dispatch
          menuItem "Probability funnel" ProbabilityFunnel currentPage dispatch ] ]

let pageContent (model : Model) (dispatch : Msg -> unit) =
  match model.CurrentPage with
  | LineChart -> LineChart.view model.LineChartModel (LineChartMsg >> dispatch)
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
