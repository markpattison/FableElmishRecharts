module App.Client

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

type Page = | ProbabilityFunnel

type ChartData = { Name: string; Range1: float * float; Range2: float * float; Median: float }

type Model = { CurrentPage: Page; ProbabilityFunnelModel: ProbabilityFunnel.Model }

type Msg =
| ShowPage of Page
| ProbabilityFunnelMsg of ProbabilityFunnel.Msg

let init () : Model * Cmd<Msg> =
    { CurrentPage = ProbabilityFunnel
      ProbabilityFunnelModel = { DataChoice = ProbabilityFunnel.Data1 }
    }, []

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg with
    | ShowPage page -> { model with CurrentPage = page }, []
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
        [ str "General" ]
      Menu.list []
        [ menuItem "Probability funnel" ProbabilityFunnel currentPage dispatch ] ]

let navBar =
    div []
      [ Navbar.navbar [ Navbar.Color IsPrimary ]
          [ Navbar.Brand.div []
              [ Navbar.Item.div []
                  [ Heading.h4 [] [ str "FableElmishRecharts" ] ] ] ] ]

let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.View.root
          Section.section []
            [ Container.container []
                [ Columns.columns []
                    [ Column.column
                        [ Column.Width (Screen.All, Column.Is2) ]
                        [ menu model.CurrentPage dispatch ]
                      Column.column []
                        [ ProbabilityFunnel.view model.ProbabilityFunnelModel (ProbabilityFunnelMsg >> dispatch) ] ] ] ] ]

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
