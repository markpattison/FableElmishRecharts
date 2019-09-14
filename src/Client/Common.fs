module App.Common

open Fable.Helpers.React
open Fulma

let button onClick txt =
  Button.button
    [ yield Button.OnClick onClick ]
    [ str txt ]
